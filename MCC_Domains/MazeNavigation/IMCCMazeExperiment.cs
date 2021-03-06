﻿#region

using System.Collections.Generic;
using System.Xml;
using SharpNeat.Core;
using SharpNeat.Genomes.Maze;
using SharpNeat.Genomes.Neat;

#endregion

namespace MCC_Domains.MazeNavigation
{
    /// <summary>
    ///     Interface for classes implementing MCC maze experiments; specifically, those coevolving mazes with
    ///     NEAT-based controllers.
    /// </summary>
    public interface IMCCMazeExperiment
    {
        /// <summary>
        ///     Gets the name of the experiment.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets human readable explanatory text for the experiment.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Gets the default agent population size to use for the experiment.
        /// </summary>
        int AgentDefaultPopulationSize { get; }

        /// <summary>
        ///     Gets the default maze population size to use for the experiment.
        /// </summary>
        int MazeDefaultPopulationSize { get; }

        /// <summary>
        ///     The number of genomes in the initial agent population.
        /// </summary>
        int AgentSeedGenomeCount { get; }

        /// <summary>
        ///     The number of genomes in the initial maze population.
        /// </summary>
        int MazeSeedGenomeCount { get; }

        /// <summary>
        ///     Create an agent genome factory for the experiment.
        /// </summary>
        /// <returns>An initialized agent genome factory.</returns>
        IGenomeFactory<NeatGenome> CreateAgentGenomeFactory();

        /// <summary>
        ///     Create a maze genome factory for the experiment.
        /// </summary>
        /// <returns>An initialized maze genome factory.</returns>
        IGenomeFactory<MazeGenome> CreateMazeGenomeFactory();

        /// <summary>
        ///     Save a population of agent genomes to an XmlWriter.
        /// </summary>
        /// <param name="xw">Reference to the XML writer.</param>
        /// <param name="agentGenomeList">The list of navigator genomes to write.</param>
        void SaveAgentPopulation(XmlWriter xw, IList<NeatGenome> agentGenomeList);

        /// <summary>
        ///     Save a population of maze genomes to an XmlWriter.
        /// </summary>
        /// <param name="xw">Reference to the XML writer.</param>
        /// <param name="mazeGenomeList">The list of maze genomes to write.</param>
        void SaveMazePopulation(XmlWriter xw, IList<MazeGenome> mazeGenomeList);

        /// <summary>
        ///     Creates and returns a MCC algorithm container, which encapsulates two evolutionary algorithms.  This
        ///     initializes the algorithms with their default population sizes and automatically generates a starting population of
        ///     that size.
        /// </summary>
        /// <returns>MCC algorithm container.</returns>
        IMCCAlgorithmContainer<NeatGenome, MazeGenome> CreateMCCAlgorithmContainer();

        /// <summary>
        ///     Creates and returns a MCC algorithm container, which encapsulates two evolutionary algorithms.  This
        ///     initializes the algorithms with the given population sizes and automatically generates a starting population for
        ///     each based on their respective size.
        /// </summary>
        /// <param name="populationSize1">The first population size.</param>
        /// <param name="populationSize2">The second population size.</param>
        /// <returns>MCC algorithm container.</returns>
        IMCCAlgorithmContainer<NeatGenome, MazeGenome> CreateMCCAlgorithmContainer(int populationSize1,
            int populationSize2);

        /// <summary>
        ///     Creates and returns a MCC algorithm container, which encapsulates two evolutionary algorithms.  This
        ///     initializes the algorithms with two preconstructed populations and the genome factories used to create them.
        /// </summary>
        /// <param name="genomeFactory1">The first population genome factory.</param>
        /// <param name="genomeFactory2">The second population genome factory.</param>
        /// <param name="genomeList1">The first population genome list.</param>
        /// <param name="genomeList2">The second population genome list.</param>
        /// <param name="isGenomeList1Preevolved">
        ///     Indicates whether the genomes in the first population genome list have been
        ///     pre-evolved to satisfy the MC with respect to population 2.
        /// </param>
        /// <returns>MCC algorithm container.</returns>
        IMCCAlgorithmContainer<NeatGenome, MazeGenome> CreateMCCAlgorithmContainer(
            IGenomeFactory<NeatGenome> genomeFactory1, IGenomeFactory<MazeGenome> genomeFactory2,
            List<NeatGenome> genomeList1, List<MazeGenome> genomeList2, bool isGenomeList1Preevolved);
    }
}