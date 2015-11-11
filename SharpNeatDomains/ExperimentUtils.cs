﻿/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */

#region

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;
using ExperimentEntities;
using SharpNeat.Core;
using SharpNeat.Decoders;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Genomes.Neat;
using SharpNeat.Loggers;
using SharpNeat.MinimalCriterias;

#endregion

namespace SharpNeat.Domains
{
    /// <summary>
    ///     Static helper methods for experiment initialization.
    /// </summary>
    public static class ExperimentUtils
    {
        /// <summary>
        ///     Create a network activation scheme from the scheme setting in the provided config XML.
        /// </summary>
        /// <returns></returns>
        public static NetworkActivationScheme CreateActivationScheme(XmlElement xmlConfig, string activationElemName)
        {
            // Get root activation element.
            XmlNodeList nodeList = xmlConfig.GetElementsByTagName(activationElemName, "");
            if (nodeList.Count != 1)
            {
                throw new ArgumentException("Missing or invalid activation XML config setting.");
            }

            XmlElement xmlActivation = nodeList[0] as XmlElement;
            string schemeStr = XmlUtils.TryGetValueAsString(xmlActivation, "Scheme");
            switch (schemeStr)
            {
                case "Acyclic":
                    return NetworkActivationScheme.CreateAcyclicScheme();
                case "CyclicFixedIters":
                    int iters = XmlUtils.GetValueAsInt(xmlActivation, "Iters");
                    return NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(iters);
                case "CyclicRelax":
                    double deltaThreshold = XmlUtils.GetValueAsDouble(xmlActivation, "Threshold");
                    int maxIters = XmlUtils.GetValueAsInt(xmlActivation, "MaxIters");
                    return NetworkActivationScheme.CreateCyclicRelaxingActivationScheme(deltaThreshold, maxIters);
            }
            throw new ArgumentException(string.Format("Invalid or missing ActivationScheme XML config setting [{0}]",
                schemeStr));
        }

        /// <summary>
        ///     Create a complexity regulation strategy based on the provided XML config values.
        /// </summary>
        public static IComplexityRegulationStrategy CreateComplexityRegulationStrategy(string strategyTypeStr,
            int? threshold)
        {
            ComplexityCeilingType ceilingType;
            if (!Enum.TryParse(strategyTypeStr, out ceilingType))
            {
                return new NullComplexityRegulationStrategy();
            }

            if (null == threshold)
            {
                throw new ArgumentNullException("threshold",
                    string.Format("threshold must be provided for complexity regulation strategy type [{0}]",
                        ceilingType));
            }

            return new DefaultComplexityRegulationStrategy(ceilingType, threshold.Value);
        }

        /// <summary>
        ///     Read Parallel Extensions options from config XML.
        /// </summary>
        /// <param name="xmlConfig"></param>
        /// <returns></returns>
        public static ParallelOptions ReadParallelOptions(XmlElement xmlConfig)
        {
            // Get parallel options.
            ParallelOptions parallelOptions;
            int? maxDegreeOfParallelism = XmlUtils.TryGetValueAsInt(xmlConfig, "MaxDegreeOfParallelism");
            if (null != maxDegreeOfParallelism)
            {
                parallelOptions = new ParallelOptions {MaxDegreeOfParallelism = maxDegreeOfParallelism.Value};
            }
            else
            {
                parallelOptions = new ParallelOptions();
            }
            return parallelOptions;
        }

        /// <summary>
        ///     Read Radial Basis Function settings from config XML.
        /// </summary>
        public static void ReadRbfAuxArgMutationConfig(XmlElement xmlConfig, out double mutationSigmaCenter,
            out double mutationSigmaRadius)
        {
            // Get root activation element.
            XmlNodeList nodeList = xmlConfig.GetElementsByTagName("RbfAuxArgMutationConfig", "");
            if (nodeList.Count != 1)
            {
                throw new ArgumentException("Missing or invalid RbfAuxArgMutationConfig XML config settings.");
            }

            XmlElement xmlRbfConfig = nodeList[0] as XmlElement;
            double? center = XmlUtils.TryGetValueAsDouble(xmlRbfConfig, "MutationSigmaCenter");
            double? radius = XmlUtils.TryGetValueAsDouble(xmlRbfConfig, "MutationSigmaRadius");
            if (null == center || null == radius)
            {
                throw new ArgumentException("Missing or invalid RbfAuxArgMutationConfig XML config settings.");
            }

            mutationSigmaCenter = center.Value;
            mutationSigmaRadius = radius.Value;
        }

        /// <summary>
        ///     Read NEAT genome parameter settings from the configuration file.
        /// </summary>
        /// <param name="xmlConfig">The reference to the XML configuration file.</param>
        /// <returns>An initialized NEAT genome parameters object.</returns>
        public static NeatGenomeParameters ReadNeatGenomeParameters(XmlElement xmlConfig)
        {
            // Create new NEAT genome parameters with default values
            var genomeParameters = new NeatGenomeParameters();

            // Get root of neat genome configuration section
            var nodeList = xmlConfig.GetElementsByTagName("GenomeConfig", "");

            if (nodeList.Count == 1)
            {
                // Convert to an XML element
                var xmlNeatGenomeConfig = nodeList[0] as XmlElement;

                // Read all of the applicable parameters in
                double? initialConnectionProportion = XmlUtils.TryGetValueAsDouble(xmlNeatGenomeConfig,
                    "InitialConnectionProportion");
                double? weightMutationProbability = XmlUtils.TryGetValueAsDouble(xmlNeatGenomeConfig,
                    "WeightMutationProbability");
                double? addConnectionProbability = XmlUtils.TryGetValueAsDouble(xmlNeatGenomeConfig,
                    "AddConnnectionProbability");
                double? addNodeProbability = XmlUtils.TryGetValueAsDouble(xmlNeatGenomeConfig, "AddNodeProbability");
                double? deleteConnectionProbability = XmlUtils.TryGetValueAsDouble(xmlNeatGenomeConfig,
                    "DeleteConnectionProbability");
                double? connectionWeightRange = XmlUtils.TryGetValueAsDouble(xmlNeatGenomeConfig,
                    "ConnectionWeightRange");

                // Set each if it's specified in the configuration (otherwise, accept the default)
                if (initialConnectionProportion != null)
                {
                    genomeParameters.InitialInterconnectionsProportion = initialConnectionProportion ?? default(double);
                }
                if (weightMutationProbability != null)
                {
                    genomeParameters.ConnectionWeightMutationProbability = weightMutationProbability ?? default(double);
                }
                if (addConnectionProbability != null)
                {
                    genomeParameters.AddConnectionMutationProbability = addConnectionProbability ?? default(double);
                }
                if (addNodeProbability != null)
                {
                    genomeParameters.AddNodeMutationProbability = addNodeProbability ?? default(double);
                }
                if (deleteConnectionProbability != null)
                {
                    genomeParameters.DeleteConnectionMutationProbability = deleteConnectionProbability ??
                                                                           default(double);
                }
                if (connectionWeightRange != null)
                {
                    genomeParameters.ConnectionWeightRange = connectionWeightRange ?? default(double);
                }
            }

            return genomeParameters;
        }

        /// <summary>
        ///     Reads novelty parameter settings from the configuration file.
        /// </summary>
        /// <param name="xmlConfig">The reference to the XML configuration file.</param>
        /// <param name="archiveAdditionThreshold">The specified archive addition threshold.</param>
        /// <param name="archiveThresholdDecreaseMultiplier">The specified archive threshold decrease multiplier.</param>
        /// <param name="archiveThresholdIncreaseMultiplier">The specified archive threshold increase multiplier.</param>
        /// <param name="maxGenerationalArchiveAddition">
        ///     The specified maximum number of genomes added to the archive within a
        ///     generation.
        /// </param>
        /// <param name="maxGenerationsWithoutArchiveAddition">
        ///     The specified maximum number of generations without an archive
        ///     addition.
        /// </param>
        public static void ReadNoveltyParameters(XmlElement xmlConfig,
            out double archiveAdditionThreshold,
            out double archiveThresholdDecreaseMultiplier, out double archiveThresholdIncreaseMultiplier,
            out int maxGenerationalArchiveAddition, out int maxGenerationsWithoutArchiveAddition)
        {
            // Get root of novelty configuration section
            var nodeList = xmlConfig.GetElementsByTagName("NoveltyConfig", "");

            Debug.Assert(nodeList.Count == 1);

            // Convert to an XML element
            var xmlNoveltyConfig = nodeList[0] as XmlElement;

            archiveAdditionThreshold = XmlUtils.GetValueAsDouble(xmlNoveltyConfig, "ArchiveAdditionThreshold");
            archiveThresholdDecreaseMultiplier = XmlUtils.GetValueAsDouble(xmlNoveltyConfig,
                "ArchiveThresholdDecreaseMultiplier");
            archiveThresholdIncreaseMultiplier = XmlUtils.GetValueAsDouble(xmlNoveltyConfig,
                "ArchiveThresholdIncreaseMultiplier");
            maxGenerationalArchiveAddition = XmlUtils.GetValueAsInt(xmlNoveltyConfig,
                "MaxGenerationalArchiveAddition");
            maxGenerationsWithoutArchiveAddition = XmlUtils.GetValueAsInt(xmlNoveltyConfig,
                "MaxGenerationsWithoutArchiveAddition");
        }

        public static IDataLogger ReadDataLogger(XmlElement xmlConfig, LoggingType loggingType)
        {
            IDataLogger dataLogger = null;
            XmlElement xmlLoggingConfig = null;
            int cnt = 0;

            // Get root of novelty configuration section
            XmlNodeList nodeList = xmlConfig.GetElementsByTagName("LoggingConfig", "");

            // Iterate through the list of logging configurations, finding one that matches the specified logging type
            foreach (XmlElement curXmlLoggingConfig in nodeList)
            {
                if (loggingType ==
                    LoggingParameterUtils.ConvertStringToLoggingType(XmlUtils.TryGetValueAsString(curXmlLoggingConfig,
                        "Type")))
                {
                    xmlLoggingConfig = curXmlLoggingConfig;
                    break;
                }
            }

            // If no appropriate logger was found, just return null (meaning there won't be any logging for this type)
            if (xmlLoggingConfig == null) return null;

            // Get the logging destination
            LoggingDestination loggingDestination =
                LoggingParameterUtils.ConvertStringToLoggingDestination(
                    XmlUtils.TryGetValueAsString(xmlLoggingConfig,
                        "Destination"));

            // Configure a file-based logger
            if (LoggingDestination.File == loggingDestination)
            {
                // Read in the log file name
                string logFileName = XmlUtils.TryGetValueAsString(xmlLoggingConfig, "LogFile");

                // Instantiate the file data logger
                dataLogger = new FileDataLogger(logFileName);
            }
            else if (LoggingDestination.Database == loggingDestination)
            {
                // Read in the experiment configuration and run number
                string experimentConfigurationName = XmlUtils.TryGetValueAsString(xmlLoggingConfig,
                    "ExperimentConfigurationName");

                if (LoggingType.Evolution == loggingType)
                {
                    // Instantiate the evolution database data logger
                    dataLogger = new NoveltyExperimentEvaluationEntityDataLogger(experimentConfigurationName);
                }
                else if (LoggingType.Evaluation == loggingType)
                {
                    // Instantiate the evaluation database data logger
                    dataLogger = new NoveltyExperimentOrganismStateEntityDataLogger(experimentConfigurationName);
                }
            }

            return dataLogger;
        }

        /// <summary>
        ///     Reads behavior characterization parameters from the configuration file.
        /// </summary>
        /// <param name="xmlConfig">The reference to the XML configuration file.</param>
        /// <returns>The behavior characterization parameters.</returns>
        public static IBehaviorCharacterization ReadBehaviorCharacterization(XmlElement xmlConfig,
            string behaviorConfigTagName)
        {
            // Get root of behavior configuration section
            XmlNodeList behaviorNodeList = xmlConfig.GetElementsByTagName(behaviorConfigTagName, "");

            // Ensure that the behavior node list was found
            if (behaviorNodeList.Count != 1)
            {
                throw new ArgumentException("Missing or invalid BehaviorConfig XML config settings.");
            }

            XmlElement xmlBehaviorConfig = behaviorNodeList[0] as XmlElement;

            // Parse and generate the appropriate behavior characterization
            IBehaviorCharacterization behaviorCharacterization = BehaviorCharacterizationUtil
                .GenerateBehaviorCharacterization(
                    XmlUtils.TryGetValueAsString(xmlBehaviorConfig, "BehaviorCharacterization"));

            // Try to get the child minimal criteria configuration
            XmlNodeList minimalCriteriaNodeList = xmlBehaviorConfig.GetElementsByTagName("MinimalCriteriaConfig", "");

            // If a minimal criteria is specified, read in its configuration and add it to the behavior characterization
            if (minimalCriteriaNodeList.Count == 1)
            {
                XmlElement xmlMinimalCriteriaConfig = minimalCriteriaNodeList[0] as XmlElement;

                // Extract the minimal criteria constraint name
                string minimalCriteriaConstraint = XmlUtils.TryGetValueAsString(xmlMinimalCriteriaConfig,
                    "MinimalCriteriaConstraint");

                // Get the appropriate minimal criteria type
                MinimalCriteriaType mcType =
                    BehaviorCharacterizationUtil.ConvertStringToMinimalCriteria(minimalCriteriaConstraint);

                switch (mcType)
                {
                    case MinimalCriteriaType.EuclideanLocation:

                        // Read in the min/max location bounds
                        double xMin = XmlUtils.GetValueAsDouble(xmlMinimalCriteriaConfig, "XMin");
                        double xMax = XmlUtils.GetValueAsDouble(xmlMinimalCriteriaConfig, "XMax");
                        double yMin = XmlUtils.GetValueAsDouble(xmlMinimalCriteriaConfig, "YMin");
                        double yMax = XmlUtils.GetValueAsDouble(xmlMinimalCriteriaConfig, "YMax");

                        // Set the euclidean location minimal criteria on the behavior characterization
                        behaviorCharacterization.MinimalCriteria = new EuclideanLocationCriteria(xMin, xMax, yMin, yMax);

                        break;

                    case MinimalCriteriaType.EuclideanDistance:

                        // Read in the starting coordinates and the minimum required distance traveled
                        double xStart = XmlUtils.GetValueAsDouble(xmlMinimalCriteriaConfig, "XStart");
                        double yStart = XmlUtils.GetValueAsDouble(xmlMinimalCriteriaConfig, "YStart");
                        double minimumDistanceTraveled = XmlUtils.GetValueAsDouble(xmlMinimalCriteriaConfig,
                            "MinimumRequiredDistance");

                        // Set the euclidean distance minimal criteria on the behavior characterization
                        behaviorCharacterization.MinimalCriteria = new EuclideanDistanceCriteria(xStart, yStart,
                            minimumDistanceTraveled);

                        break;
                }
            }

            return behaviorCharacterization;
        }

        /// <summary>
        ///     Reads behavior characterization parameters from the database.
        /// </summary>
        /// <param name="experiment">The experiment dictionary entity.</param>
        /// <param name="isPrimary">
        ///     Boolean flag indicating whether this is the primary behavior characterization or the behavior
        ///     characterization used for experiment initialization.
        /// </param>
        /// <returns></returns>
        public static IBehaviorCharacterization ReadBehaviorCharacterization(ExperimentDictionary experiment,
            bool isPrimary)
        {
            // Read behavior characterization
            String behaviorCharacterizationName = isPrimary
                ? experiment.Primary_BehaviorCharacterizationName
                : experiment.Initialization_BehaviorCharacterizationName;

            // Ensure that the behavior was specified
            if (behaviorCharacterizationName == null)
            {
                throw new ArgumentException("Missing or invalid BehaviorConfig settings.");
            }

            // Parse and generate the appropriate behavior characterization
            IBehaviorCharacterization behaviorCharacterization = BehaviorCharacterizationUtil
                .GenerateBehaviorCharacterization(behaviorCharacterizationName);

            // TODO: Need to handle associated minimal criteria

            return behaviorCharacterization;
        }
    }
}