﻿#region

using System.Collections.Generic;
using System.Xml;
using ExperimentEntities;
using SharpNeat.Behaviors;
using SharpNeat.Core;
using SharpNeat.DistanceMetrics;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Loggers;
using SharpNeat.MinimalCriterias;
using SharpNeat.Phenomes;
using SharpNeat.SpeciationStrategies;

#endregion

namespace SharpNeat.Domains.MazeNavigation.MCSExperiment
{
    public class SteadyStateMazeNavigationMCSExperiment : BaseMazeNavigationExperiment
    {
        private int _batchSize;
        private IBehaviorCharacterization _behaviorCharacterization;

        /// <summary>
        ///     Path/File to which to write generational data log.
        /// </summary>
        private string _generationalLogFile;

        private int _populationEvaluationFrequency;
        private string _mcsSelectionMethod;
        private IDataLogger _evaluationDataLogger;
        private IDataLogger _evolutionDataLogger;

        public override void Initialize(string name, XmlElement xmlConfig)
        {
            base.Initialize(name, xmlConfig);

            // Read in the behavior characterization
            _behaviorCharacterization = ExperimentUtils.ReadBehaviorCharacterization(xmlConfig);

            // Read in steady-state specific parameters
            _batchSize = XmlUtils.GetValueAsInt(xmlConfig, "OffspringBatchSize");
            _populationEvaluationFrequency = XmlUtils.GetValueAsInt(xmlConfig, "PopulationEvaluationFrequency");

            // Read in MCS selection method
            _mcsSelectionMethod = XmlUtils.TryGetValueAsString(xmlConfig, "McsSelectionMethod");

            // Read in log file path/name
            _evolutionDataLogger = ExperimentUtils.ReadDataLogger(xmlConfig, LoggingType.Evolution);
            _evaluationDataLogger = ExperimentUtils.ReadDataLogger(xmlConfig, LoggingType.Evaluation);
        }

        public override void Initialize(ExperimentDictionary experimentDictionary)
        {
            base.Initialize(experimentDictionary);

            // Read in the behavior characterization
            _behaviorCharacterization = new EndPointBehaviorCharacterization(new EuclideanDistanceCriteria((double)experimentDictionary.McsStartX,
                (double)experimentDictionary.McsStartY, (double)experimentDictionary.MinimumRequiredDistance));

            // Read in steady-state specific parameters
            _batchSize = experimentDictionary.OffspringBatchSize ?? default(int);
            _populationEvaluationFrequency = experimentDictionary.PopulationEvaluationFrequency ?? default(int);

            // Read in MCS selection method
            _mcsSelectionMethod = experimentDictionary.AlgorithmType;

            // Read in log file path/name
            _evolutionDataLogger = new NoveltyExperimentEvaluationEntityDataLogger(experimentDictionary.ExperimentName);
            _evaluationDataLogger = new NoveltyExperimentOrganismStateEntityDataLogger(experimentDictionary.ExperimentName);
        }

        /// <summary>
        ///     Create and return a SteadyStateNeatEvolutionAlgorithm object (specific to fitness-based evaluations) ready for
        ///     running the
        ///     NEAT algorithm/search based on the given genome factory and genome list.  Various sub-parts of the algorithm are
        ///     also constructed and connected up.
        /// </summary>
        /// <param name="genomeFactory">The genome factory from which to generate new genomes</param>
        /// <param name="genomeList">The current genome population</param>
        /// <returns>Constructed evolutionary algorithm</returns>
        public override INeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(
            IGenomeFactory<NeatGenome> genomeFactory,
            List<NeatGenome> genomeList)
        {            
            // Create distance metric. Mismatched genes have a fixed distance of 10; for matched genes the distance is their weigth difference.
            IDistanceMetric distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            ISpeciationStrategy<NeatGenome> speciationStrategy =
                new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetric, ParallelOptions);

            // Create complexity regulation strategy.
            var complexityRegulationStrategy =
                ExperimentUtils.CreateComplexityRegulationStrategy(ComplexityRegulationStrategy, Complexitythreshold);

            // Create the evolution algorithm.
            AbstractNeatEvolutionAlgorithm<NeatGenome> ea;
            if ("Random".Equals(_mcsSelectionMethod))
            {
                ea = new SteadyStateNeatEvolutionAlgorithm<NeatGenome>(NeatEvolutionAlgorithmParameters,
                    speciationStrategy, complexityRegulationStrategy, _batchSize, _populationEvaluationFrequency, _evolutionDataLogger);
            }
            else
            {
                ea = new QueueingNeatEvolutionAlgorithm<NeatGenome>(NeatEvolutionAlgorithmParameters, speciationStrategy,
                    complexityRegulationStrategy, _batchSize, _populationEvaluationFrequency, _evolutionDataLogger);
            }

            // Create IBlackBox evaluator.
            var mazeNavigationEvaluator = new MazeNavigationMCSEvaluator(MaxDistanceToTarget, MaxTimesteps,
                MazeVariant,
                MinSuccessDistance, _behaviorCharacterization);

            // Create genome decoder.
            var genomeDecoder = CreateGenomeDecoder();

//            IGenomeEvaluator<NeatGenome> fitnessEvaluator =
//                new SerialGenomeBehaviorEvaluator<NeatGenome, IBlackBox>(genomeDecoder, mazeNavigationEvaluator,
//                    EvaluationType.MinimalCriteriaSearch);

            IGenomeEvaluator<NeatGenome> fitnessEvaluator =
                new ParallelGenomeBehaviorEvaluator<NeatGenome, IBlackBox>(genomeDecoder, mazeNavigationEvaluator,
                    EvaluationType.MinimalCriteriaSearchQueueing, _evaluationDataLogger);

            // Initialize the evolution algorithm.
            ea.Initialize(fitnessEvaluator, genomeFactory, genomeList, MaxGenerations);

            // Finished. Return the evolution algorithm
            return ea;
        }
    }
}