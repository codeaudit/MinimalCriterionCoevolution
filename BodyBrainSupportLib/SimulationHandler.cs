using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using MCC_Domains.BodyBrain;
using MCC_Domains.Utils;
using SharpNeat;
using SharpNeat.Decoders;
using SharpNeat.Genomes.HyperNeat;
using SharpNeat.Genomes.Substrate;
using SharpNeat.Network;
using SharpNeat.Phenomes.Voxels;

namespace BodyBrainSupportLib
{
    /// <summary>
    ///     Provides methods for reading/writing simulation configuration and log files, and for configuring and executing the
    ///     simulator.
    /// </summary>
    public static class SimulationHandler
    {
        #region Public static methods

        /// <summary>
        ///     Writes a simulation configuration file for a given body and brain.
        /// </summary>
        /// <param name="body">The body to serialize to the configuration file.</param>
        /// <param name="brain">The brain to serialize to the configuration file.</param>
        /// <param name="directory">
        ///     The directory into which the generated Voxelyze simulation configuration file is written.
        /// </param>
        /// <param name="experimentName">The name of the experiment that was executed.</param>
        /// <param name="run">The run number of the experiment that was executed.</param>
        /// <param name="configTemplate">The simulation configuration template including simulation parameter defaults.</param>
        /// <param name="mcValue">The minimal criterion value.</param>
        public static void WriteConfigFile(VoxelBody body, VoxelBrain brain, string directory,
            string experimentName, int run, string configTemplate, double mcValue)
        {
            // Construct the output file path and name
            var outputFile = BodyBrainExperimentUtils.ConstructVoxelyzeFilePath("config", "vxa",
                directory, experimentName, run, brain.GenomeId, body.GenomeId, false);

            // Write the configuration file
            BodyBrainExperimentUtils.WriteVoxelyzeSimulationFile(configTemplate, outputFile, ".", brain, body,
                mcValue);
        }

        /// <summary>
        ///     Gets and/or creates the output directory for a body/brain simulation configuration file. Note that this also
        ///     subdivides into body size and proportion (i.e. the percentage of non-empty positions in the voxel lattice).
        /// </summary>
        /// <param name="baseOutputDirectory">The top-level output directory into which simulation configuration files are written.</param>
        /// <param name="experimentName">The name of the experiment that was executed.</param>
        /// <param name="run">The run number of the experiment that was executed.</param>
        /// <param name="body">The voxel body from which to extract the size and proportion.</param>
        /// <returns>The full file system path for the configuration file.</returns>
        public static string GetConfigOutputDirectory(string baseOutputDirectory, string experimentName, int run,
            VoxelBody body)
        {
            // Construct the output directory
            var directory = Path.Combine(baseOutputDirectory, experimentName, $"Run{run}",
                $"Size {body.LengthX}x{body.LengthY}x{body.LengthZ}",
                $"Proportion {Math.Round(body.FullProportion, 1).ToString(CultureInfo.InvariantCulture)}");

            // Create directory if it doesn't already exist
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        /// <summary>
        ///     Constructs the simulation configuration file path.
        /// </summary>
        /// <param name="configDirectory">The directory into which to write the configuration file.</param>
        /// <param name="experimentName">The name of the experiment that was executed.</param>
        /// <param name="run">The run number of the experiment that was executed.</param>
        /// <param name="brainGenomeId">The genome ID of the brain controller.</param>
        /// <param name="bodyGenomeId">The genome ID of the voxel body.</param>
        /// <returns>The fully-qualified simulation configuration file path.</returns>
        public static string GetConfigFilePath(string configDirectory, string experimentName, int run,
            uint brainGenomeId, uint bodyGenomeId)
        {
            return BodyBrainExperimentUtils.ConstructVoxelyzeFilePath("config", "vxa", configDirectory, experimentName,
                run, brainGenomeId, bodyGenomeId, false);
        }

        /// <summary>
        ///     Constructs the simulation log file path.
        /// </summary>
        /// <param name="simLogDirectory">The directory into which to write the simulation log file.</param>
        /// <param name="experimentName">The name of the experiment that was executed.</param>
        /// <param name="run">The run number of the experiment that was executed.</param>
        /// <param name="brainGenomeId">The genome ID of the brain controller.</param>
        /// <param name="bodyGenomeId">The genome ID of the voxel body.</param>
        /// <returns>The fully-qualified simulation log file path.</returns>
        public static string GetSimLogFilePath(string simLogDirectory, string experimentName, int run,
            uint brainGenomeId, uint bodyGenomeId)
        {
            return BodyBrainExperimentUtils.ConstructVoxelyzeFilePath("simlog", "xml", simLogDirectory, experimentName,
                run, brainGenomeId, bodyGenomeId, false);
        }

        /// <summary>
        ///     Writes the body-brain simulation configuration file and executes the simulation for a specified period of time.
        /// </summary>
        /// <param name="configTemplate">The path to the simulation configuration template file.</param>
        /// <param name="configFilePath">The path of the generated simulation configuration file.</param>
        /// <param name="simExecutablePath">The path of the simulation executor.</param>
        /// <param name="simLogFilePath">The path of the generated simulation log file.</param>
        /// <param name="simulationTime">The simulation duration.</param>
        /// <param name="brain">The voxel brain controller.</param>
        /// <param name="body">The voxel body.</param>
        /// <param name="deleteConfigFile">
        ///     Controls whether to delete the simulation configuration file after the simulation has
        ///     executed (default is true).
        /// </param>
        public static void ExecuteTimeboundBodyBrainSimulation(string configTemplate, string configFilePath,
            string simExecutablePath, string simLogFilePath, double simulationTime, VoxelBrain brain, VoxelBody body,
            bool deleteConfigFile = true)
        {
            // Stop condition of 2 causes the simulation to run for a specified amount of time
            const int stopConditionType = 2;

            BodyBrainExperimentUtils.WriteVoxelyzeSimulationFile(configTemplate, configFilePath, simLogFilePath,
                stopConditionType, simulationTime, brain, body);

            // Configure the simulation, execute and wait for completion
            using (var process =
                Process.Start(
                    BodyBrainExperimentUtils.ConfigureSimulationExecution(simExecutablePath, configFilePath)))
            {
                process?.WaitForExit();
            }

            // Delete the simulation configuration file
            if (deleteConfigFile)
                File.Delete(configFilePath);
        }

        /// <summary>
        ///     Parses the simulation log file and constructs a body/brain simulation unit to be persisted to a data file.
        /// </summary>
        /// <param name="brainGenomeId">The genome ID of the brain controller.</param>
        /// <param name="bodyGenomeId">The genome ID of the voxel body.</param>
        /// <param name="simLogFilePath">The path of the generated simulation log file.</param>
        /// <param name="deleteSimLogFile">Controls whether to delete the simulation log file after parsing it (default is true).</param>
        /// <returns>A body/brain simulation unit that encapsulates properties of the simulation.</returns>
        /// <exception cref="SharpNeatException"></exception>
        public static BodyBrainSimulationUnit ReadSimulationLog(uint brainGenomeId, uint bodyGenomeId,
            string simLogFilePath, bool deleteSimLogFile = true)
        {
            var simulationUnit = new BodyBrainSimulationUnit(brainGenomeId, bodyGenomeId);

            // Load the results file
            var resultsDoc = new XmlDocument();
            resultsDoc.Load(simLogFilePath);

            // Attempt to get the root of the simulation log document
            var simLogEntries = resultsDoc.GetElementsByTagName("SimLogData", "");

            if (simLogEntries.Count <= 0)
            {
                throw new SharpNeatException($"Failed to read simulation log file: [{simLogFilePath}]");
            }

            foreach (var simEntryElem in from object simEntry in simLogEntries[0] select simEntry as XmlElement)
            {
                if (simEntryElem is null)
                {
                    throw new SharpNeatException($"Encountered null element in simulation log file [{simLogFilePath}]");
                }

                // Get the current timestep
                var timestep = int.Parse(simEntryElem.GetAttribute("TimeStep"));

                // Extract simulation entry data
                var simTime = XmlUtils.GetValueAsDouble(simEntryElem, "Time");
                var xPos = XmlUtils.GetValueAsDouble(simEntryElem, "xPos");
                var yPos = XmlUtils.GetValueAsDouble(simEntryElem, "yPos");
                var zPos = XmlUtils.GetValueAsDouble(simEntryElem, "zPos");
                var distance = XmlUtils.GetValueAsDouble(simEntryElem, "Distance");
                var voxelsTouchingFloor = XmlUtils.GetValueAsInt(simEntryElem, "VoxelsTouchingFloor");
                var maxVoxelVel = XmlUtils.GetValueAsDouble(simEntryElem, "MaxVoxelVelocity");
                var maxVoxelDisp = XmlUtils.GetValueAsDouble(simEntryElem, "MaxVoxelDisplacement");
                var xDisp = XmlUtils.GetValueAsDouble(simEntryElem, "xDisplacement");
                var yDisp = XmlUtils.GetValueAsDouble(simEntryElem, "yDisplacement");
                var zDisp = XmlUtils.GetValueAsDouble(simEntryElem, "zDisplacement");

                simulationUnit.AddTimestepInfo(timestep, simTime, xPos, yPos, zPos, distance, voxelsTouchingFloor,
                    maxVoxelVel, maxVoxelDisp, xDisp, yDisp, zDisp);
            }

            // Delete the simulation log file
            if (deleteSimLogFile)
                File.Delete(simLogFilePath);

            return simulationUnit;
        }

        #endregion

        #region Private static methods

        /// <summary>
        ///     Creates the appropriate network activation scheme based on whether recurrent properties are specified.
        /// </summary>
        /// <param name="activationIters">
        ///     The number of times a recurrent connection should be activated (only applicable for
        ///     RNNs).
        /// </param>
        /// <returns>The instantiated NetworkActivationScheme.</returns>
        private static NetworkActivationScheme GetNetworkActivationScheme(int? activationIters)
        {
            return activationIters != null
                ? NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(activationIters ?? 0)
                : NetworkActivationScheme.CreateAcyclicScheme();
        }

        /// <summary>
        ///     Creates a new voxel brain genome factory given the input/output neuron counts.
        /// </summary>
        /// <param name="brainCppnInputCount">The number of input neurons.</param>
        /// <param name="brainCppnOutputCount">The number of output neurons.</param>
        /// <returns>The instantiated voxel brain genome factory.</returns>
        private static CppnGenomeFactory CreateBrainGenomeFactory(int brainCppnInputCount = 5,
            int brainCppnOutputCount = 32)
        {
            return new CppnGenomeFactory(brainCppnInputCount, brainCppnOutputCount);
        }

        /// <summary>
        ///     Creates a new voxel body genome factory based on the body dimensions, size and CPPN input/output neuron counts.
        /// </summary>
        /// <param name="xDim">The body size along the x-dimension.</param>
        /// <param name="yDim">The body size along the y-dimension.</param>
        /// <param name="zDim">The body size along the z-dimension.</param>
        /// <param name="maxSize">The maximum possible body size.</param>
        /// <param name="bodyCppnInputCount">The number of input neurons.</param>
        /// <param name="bodyCppnOutputCount">The number of output neurons.</param>
        /// <returns>The instantiated voxel body genome factory.</returns>
        private static NeatSubstrateGenomeFactory CreateBodyGenomeFactory(int xDim, int yDim, int zDim, int maxSize,
            int bodyCppnInputCount = 5, int bodyCppnOutputCount = 2)
        {
            return new NeatSubstrateGenomeFactory(bodyCppnInputCount, bodyCppnOutputCount,
                DefaultActivationFunctionLibrary.CreateLibraryCppn(), new NeatSubstrateGenomeParameters(), xDim, yDim,
                zDim, maxSize);
        }

        #endregion
    }
}