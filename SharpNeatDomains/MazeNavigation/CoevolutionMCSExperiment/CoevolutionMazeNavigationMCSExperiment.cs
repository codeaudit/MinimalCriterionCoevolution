﻿#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using SharpNeat.Core;
using SharpNeat.Decoders;
using SharpNeat.Decoders.Maze;
using SharpNeat.Decoders.Neat;
using SharpNeat.Domains.MazeNavigation.Bootstrappers;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Maze;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using SharpNeat.Phenomes.Mazes;

#endregion

namespace SharpNeat.Domains.MazeNavigation.CoevolutionMCSExperiment
{
    public class CoevolutionMazeNavigationMCSExperiment : ICoevolutionExperiment
    {
        #region Public Methods

        /// <summary>
        ///     Creates a new genome factory for maze navigator agents.
        /// </summary>
        /// <returns>The constructed agent genome factory.</returns>
        public IGenomeFactory<NeatGenome> CreateAgentGenomeFactory()
        {
            return new NeatGenomeFactory(AnnInputCount, AnnOutputCount, _neatGenomeParameters);
        }

        /// <summary>
        ///     Creates a new genome factory for mazes.
        /// </summary>
        /// <returns>The constructed maze genome factory.</returns>
        public IGenomeFactory<MazeGenome> CreateMazeGenomeFactory()
        {
            return new MazeGenomeFactory();
        }

        /// <summary>
        ///     Save a population of agent genomes to an XmlWriter.
        /// </summary>
        public void SaveAgentPopulation(XmlWriter xw, IList<NeatGenome> agentGenomeList)
        {
            NeatGenomeXmlIO.WriteComplete(xw, agentGenomeList, false);
        }

        /// <summary>
        ///     Save a population of maze genomes to an XmlWriter.
        /// </summary>
        public void SaveMazePopulation(XmlWriter xw, IList<MazeGenome> mazeGenomeList)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Initializes the coevolution maze navigation experiment by reading in all of the configuration parameters and
        ///     setting up the bootstrapping/initialization algorithm.
        /// </summary>
        /// <param name="name">The name of the experiment.</param>
        /// <param name="xmlConfig">The reference to the XML configuration file.</param>
        /// <param name="evolutionDataLogger">The evolution data logger.</param>
        /// <param name="evaluationDataLogger">The evaluation data logger.</param>
        public void Initialize(string name, XmlElement xmlConfig, IDataLogger evolutionDataLogger = null,
            IDataLogger evaluationDataLogger = null)
        {
            // Set boiler plate properties
            Name = name;
            Description = XmlUtils.GetValueAsString(xmlConfig, "Description");
            _activationScheme = ExperimentUtils.CreateActivationScheme(xmlConfig, "Activation");
            _parallelOptions = ExperimentUtils.ReadParallelOptions(xmlConfig);
            _serializeGenomeToXml = XmlUtils.TryGetValueAsBool(xmlConfig, "DecodeGenomesToXml") ?? false;

            // Set the genome parameters
            //_neatEvolutionAlgorithmParameters = ExperimentUtils.ReadNeatEvolutionAlgorithmParameters(xmlConfig);
            _neatGenomeParameters = ExperimentUtils.ReadNeatGenomeParameters(xmlConfig);
            _mazeGenomeParameters = ExperimentUtils.ReadMazeGenomeParameters(xmlConfig);

            // Configure evolutionary algorithm parameters
            AgentDefaultPopulationSize = XmlUtils.GetValueAsInt(xmlConfig, "AgentPopulationSize");
            MazeDefaultPopulationSize = XmlUtils.GetValueAsInt(xmlConfig, "MazePopulationSize");
            AgentSeedGenomeCount = XmlUtils.GetValueAsInt(xmlConfig, "AgentSeedGenomeCount");
            MazeSeedGenomeCount = XmlUtils.GetValueAsInt(xmlConfig, "MazeSeedGenomeCount");
            _behaviorCharacterizationFactory = ExperimentUtils.ReadBehaviorCharacterizationFactory(xmlConfig,
                "BehaviorConfig");
            _batchSize = XmlUtils.GetValueAsInt(xmlConfig, "OffspringBatchSize");

            // Set run-time bounding parameters
            _maxGenerations = XmlUtils.TryGetValueAsInt(xmlConfig, "MaxGenerations");
            _maxEvaluations = XmlUtils.TryGetValueAsULong(xmlConfig, "MaxEvaluations");

            // Set experiment-specific parameters
            _maxTimesteps = XmlUtils.GetValueAsInt(xmlConfig, "MaxTimesteps");
            _minSuccessDistance = XmlUtils.GetValueAsInt(xmlConfig, "MinSuccessDistance");
            _mazeHeight = XmlUtils.GetValueAsInt(xmlConfig, "MazeHeight");
            _mazeWidth = XmlUtils.GetValueAsInt(xmlConfig, "MazeWidth");
            _mazeScaleMultiplier = XmlUtils.GetValueAsInt(xmlConfig, "MazeScaleMultiplier");

            // Get success/failure criteria constraints
            _numMazeSuccessCriteria = XmlUtils.GetValueAsInt(xmlConfig, "NumMazesSolvedCriteria");
            _numAgentSuccessCriteria = XmlUtils.GetValueAsInt(xmlConfig, "NumAgentsSolvedCriteria");
            _numAgentFailedCriteria = XmlUtils.GetValueAsInt(xmlConfig, "NumAgentsFailedCriteria");

            // TODO: Setup logging here

            // Initialize the initialization algorithm
            _mazeNavigationInitializer = new FitnessCoevolutionMazeNavigationInitializer();

            // Setup initialization algorithm
            _mazeNavigationInitializer.SetAlgorithmParameters(
                xmlConfig.GetElementsByTagName("InitializationAlgorithmConfig", "")[0] as XmlElement, AnnInputCount,
                AnnOutputCount, _numAgentSuccessCriteria, _numAgentFailedCriteria);

            // Pass in maze experiment specific parameters 
            // (note that a new maze structure is created here for the sole purpose of extracting the maze dimensions and calculating max distance to target)
            _mazeNavigationInitializer.SetEnvironmentParameters(_maxTimesteps, _minSuccessDistance,
                new MazeDecoder(_mazeHeight, _mazeWidth, _mazeScaleMultiplier).Decode(
                    new MazeGenomeFactory(_mazeGenomeParameters).CreateGenome(0)));
        }

        /// <summary>
        ///     Zero argument wrapper method for instantiating the coveolution algorithm container.  This uses default agent and
        ///     maze population sizes as the only configuration parameters.
        /// </summary>
        /// <returns>The instantiated coevolution algorithm container.</returns>
        public ICoevolutionAlgorithmContainer<NeatGenome, MazeGenome> CreateCoevolutionAlgorithmContainer()
        {
            return CreateCoevolutionAlgorithmContainer(AgentSeedGenomeCount, MazeSeedGenomeCount);
        }

        /// <summary>
        ///     Creates the coevolution algorithm container using the given agent and maze population sizes.
        /// </summary>
        /// <param name="populationSize1">The agent population size.</param>
        /// <param name="populationSize2">The maze population size.</param>
        /// <returns>The instantiated coevolution algorithm container.</returns>
        public ICoevolutionAlgorithmContainer<NeatGenome, MazeGenome> CreateCoevolutionAlgorithmContainer(
            int populationSize1, int populationSize2)
        {
            // Create a genome factory for the NEAT genomes
            IGenomeFactory<NeatGenome> neatGenomeFactory = new NeatGenomeFactory(AnnInputCount, AnnOutputCount,
                _neatGenomeParameters);

            // Create a genome factory for the maze genomes
            IGenomeFactory<MazeGenome> mazeGenomeFactory = new MazeGenomeFactory(_mazeGenomeParameters);

            // Create an initial population of maze navigators
            List<NeatGenome> neatGenomeList = neatGenomeFactory.CreateGenomeList(populationSize1, 0);

            // Create an initial population of mazes
            // NOTE: the population is set to 1 here because we're just starting with a single, completely open maze space
            List<MazeGenome> mazeGenomeList = mazeGenomeFactory.CreateGenomeList(populationSize2, 0);

            // Create the evolution algorithm container
            return CreateCoevolutionAlgorithmContainer(neatGenomeFactory, mazeGenomeFactory, neatGenomeList,
                mazeGenomeList);
        }

        /// <summary>
        ///     Creates the evolution algorithm container using the given factories and genome lists.
        /// </summary>
        /// <param name="genomeFactory1">The agent genome factory.</param>
        /// <param name="genomeFactory2">The maze genome factory.</param>
        /// <param name="genomeList1">The agent genome list.</param>
        /// <param name="genomeList2">The maze genome list.</param>
        /// <returns>The instantiated coevolution algorithm container.</returns>
        public ICoevolutionAlgorithmContainer<NeatGenome, MazeGenome> CreateCoevolutionAlgorithmContainer(
            IGenomeFactory<NeatGenome> genomeFactory1,
            IGenomeFactory<MazeGenome> genomeFactory2, List<NeatGenome> genomeList1, List<MazeGenome> genomeList2)
        {
            ulong initializationEvaluations;

            // Instantiate the internal initialization algorithm
            _mazeNavigationInitializer.InitializeAlgorithm(_parallelOptions, genomeList1,
                new NeatGenomeDecoder(_activationScheme), 0);

            // Run the initialization algorithm until the requested number of viable seed genomes are found
            List<NeatGenome> seedAgentPopulation =
                _mazeNavigationInitializer.EvolveViableGenomes(out initializationEvaluations);

            using (XmlWriter xw = XmlWriter.Create("ViableSeedGenomes.xml", new XmlWriterSettings() {Indent = true}))
            {
                SaveAgentPopulation(xw, seedAgentPopulation);
            }

            using (XmlWriter xw = XmlWriter.Create("StartingMazeGenome.xml", new XmlWriterSettings() {Indent = true}))
            {
                SaveMazePopulation(xw, genomeList2);
            }

            // Create the NEAT (i.e. navigator) queueing evolution algorithm
            AbstractEvolutionAlgorithm<NeatGenome> neatEvolutionAlgorithm =
            new QueueingNeatEvolutionAlgorithm<NeatGenome>(new NeatEvolutionAlgorithmParameters(), null, _batchSize);

            // Create the maze queueing evolution algorithm
            AbstractEvolutionAlgorithm<MazeGenome> mazeEvolutionAlgorithm =
                new QueueingNeatEvolutionAlgorithm<MazeGenome>(new NeatEvolutionAlgorithmParameters(), null, _batchSize);

            // Create the maze phenome evaluator
            IPhenomeEvaluator<MazeStructure, BehaviorInfo> mazeEvaluator = new MazeEnvironmentMCSEvaluator(
                _maxTimesteps,
                _minSuccessDistance, _behaviorCharacterizationFactory, _numAgentSuccessCriteria, _numAgentFailedCriteria);

            // Create navigator phenome evaluator
            IPhenomeEvaluator<IBlackBox, BehaviorInfo> navigatorEvaluator = new MazeNavigatorMCSEvaluator(
                _maxTimesteps, _minSuccessDistance, _behaviorCharacterizationFactory, _numMazeSuccessCriteria);

            // Create maze genome decoder
            IGenomeDecoder<MazeGenome, MazeStructure> mazeGenomeDecoder = new MazeDecoder(_mazeHeight, _mazeWidth);

            // Create navigator genome decoder
            IGenomeDecoder<NeatGenome, IBlackBox> navigatorGenomeDecoder = new NeatGenomeDecoder(_activationScheme);

            // Create the maze genome evaluator
            IGenomeEvaluator<MazeGenome> mazeFitnessEvaluator =
                new SerialGenomeBehaviorEvaluator<MazeGenome, MazeStructure>(mazeGenomeDecoder, mazeEvaluator,
                    SelectionType.Queueing, SearchType.MinimalCriteriaSearch);

            // Create navigator genome evaluator
            IGenomeEvaluator<NeatGenome> navigatorFitnessEvaluator =
                new SerialGenomeBehaviorEvaluator<NeatGenome, IBlackBox>(navigatorGenomeDecoder, navigatorEvaluator,
                    SelectionType.Queueing, SearchType.MinimalCriteriaSearch);

            // Create the coevolution container
            ICoevolutionAlgorithmContainer<NeatGenome, MazeGenome> coevolutionAlgorithmContainer =
                new CoevolutionAlgorithmContainer<NeatGenome, MazeGenome>(neatEvolutionAlgorithm, mazeEvolutionAlgorithm);

            // Initialize the container and component algorithms
            coevolutionAlgorithmContainer.Initialize(navigatorFitnessEvaluator, genomeFactory1, seedAgentPopulation,
                AgentDefaultPopulationSize, mazeFitnessEvaluator, genomeFactory2, genomeList2, MazeDefaultPopulationSize,
                _maxGenerations, _maxEvaluations);

            return coevolutionAlgorithmContainer;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The name of the experiment.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     The description of the experiment.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        ///     The default (max) agent population size.
        /// </summary>
        public int AgentDefaultPopulationSize { get; private set; }

        /// <summary>
        ///     The default (max) maze population size.
        /// </summary>
        public int MazeDefaultPopulationSize { get; private set; }

        /// <summary>
        ///     The number of agent genomes in the agent seed population.
        /// </summary>
        public int AgentSeedGenomeCount { get; private set; }

        /// <summary>
        ///     The number of maze genomes in the maze seed population.
        /// </summary>
        public int MazeSeedGenomeCount { get; private set; }

        #endregion

        #region Instance Variables

        /// <summary>
        ///     The activation scheme (i.e. cyclic or acyclic).
        /// </summary>
        private NetworkActivationScheme _activationScheme;

        /// <summary>
        ///     Switches between synchronous and asynchronous execution (with user-defined number of threads).
        /// </summary>
        private ParallelOptions _parallelOptions;

        /// <summary>
        ///     Dictates whether genome XML should be serialized and logged.
        /// </summary>
        private bool _serializeGenomeToXml;

        /// <summary>
        ///     The NEAT genome parameters to use for the experiment.
        /// </summary>
        private NeatGenomeParameters _neatGenomeParameters;

        /// <summary>
        ///     The maze genome parameters to use for the experiment.
        /// </summary>
        private MazeGenomeParameters _mazeGenomeParameters;

        /// <summary>
        ///     The number of neural network inputs.
        /// </summary>
        private const int AnnInputCount = 10;

        /// <summary>
        ///     The number of neural network outputs.
        /// </summary>
        private const int AnnOutputCount = 2;

        /// <summary>
        ///     The number of individuals to be evaluated in a single evaluation "batch".
        /// </summary>
        private int _batchSize;

        /// <summary>
        ///     The factory used for producing behavior characterizations.
        /// </summary>
        private IBehaviorCharacterizationFactory _behaviorCharacterizationFactory;

        /// <summary>
        ///     Initialization algorithm for producing an initial population with the requisite number of viable genomes.
        /// </summary>
        private FitnessCoevolutionMazeNavigationInitializer _mazeNavigationInitializer;

        /// <summary>
        ///     The maximum number of evaluations allowed (optional).
        /// </summary>
        private ulong? _maxEvaluations;

        /// <summary>
        ///     The maximum number of generations allowed (optional).
        /// </summary>
        private int? _maxGenerations;

        /// <summary>
        ///     The maximum number of timesteps allowed for a single simulation.
        /// </summary>
        private int _maxTimesteps;

        /// <summary>
        ///     The minimum distance to the target required in order to have "solved" the maze.
        /// </summary>
        private int _minSuccessDistance;

        /// <summary>
        ///     The minimum number of mazes that the agent under evaluation must solve in order to meet the minimal criteria.
        /// </summary>
        private int _numMazeSuccessCriteria;

        /// <summary>
        ///     The minimum number of agents that must solve the maze under evaluation in order to meet this portion of the minimal
        ///     criteria.
        /// </summary>
        private int _numAgentSuccessCriteria;

        /// <summary>
        ///     The minimum number of agents that must fail to solve the maze under evaluation in order to meet this portion of the
        ///     minimal criteria.
        /// </summary>
        private int _numAgentFailedCriteria;

        /// <summary>
        ///     The width of the evolved maze environments.
        /// </summary>
        private int _mazeHeight;

        /// <summary>
        ///     The height of the evolved maze environments.
        /// </summary>
        private int _mazeWidth;

        /// <summary>
        ///     The multiplier for scaling the maze to larger sizes.
        /// </summary>
        private int _mazeScaleMultiplier;

        #endregion
    }
}