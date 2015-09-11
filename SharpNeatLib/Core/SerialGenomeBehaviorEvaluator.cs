﻿#region

using System.Collections.Generic;
using SharpNeat.Utility;

#endregion

namespace SharpNeat.Core
{
    /// <summary>
    ///     A concrete implementation of IGenomeFitnessEvaluator that evaulates genome's phenotypic behaviors independently
    ///     of
    ///     each other and in series on a single thread.
    ///     Genome decoding is performed by a provided IGenomeDecoder.
    ///     Phenome evaluation is performed by a provided IPhenomeEvaluator.
    ///     This class evaluates on a single thread only, and therefore is a good choice when debugging code.
    /// </summary>
    /// <typeparam name="TGenome">The genome type that is decoded.</typeparam>
    /// <typeparam name="TPhenome">The phenome type that is decoded to and then evaluated.</typeparam>
    public class SerialGenomeBehaviorEvaluator<TGenome, TPhenome> : IGenomeEvaluator<TGenome>
        where TGenome : class, IGenome<TGenome>
        where TPhenome : class
    {
        private readonly BatchEvaluationMethod _batchEvaluationMethod;
        private readonly EliteArchive<TGenome> _eliteArchive;
        private readonly bool _enablePhenomeCaching;
        private readonly IGenomeDecoder<TGenome, TPhenome> _genomeDecoder;
        private readonly int _nearestNeighbors;
        private readonly IPhenomeEvaluator<TPhenome, BehaviorInfo> _phenomeEvaluator;
        private readonly PopulationEvaluationMethod _populationEvaluationMethod;

        /// <summary>
        ///     Constructs serial genome list behavior evaluator, customizing only the phenome behavior evaluator and the
        ///     evaluation method.  Also sets the number of nearest neighbors to utilize in behavior distance calculations and
        ///     accepts an optional elite archive.
        /// </summary>
        /// <param name="genomeDecoder">The genome decoder to use.</param>
        /// <param name="phenomeEvaluator">The phenome evaluator.</param>
        /// <param name="nearestNeighbors">The number of nearest neighbors to use in behavior distance calculations.</param>
        /// <param name="archive">A reference to the elite archive (optional).</param>
        public SerialGenomeBehaviorEvaluator(IGenomeDecoder<TGenome, TPhenome> genomeDecoder,
            IPhenomeEvaluator<TPhenome, BehaviorInfo> phenomeEvaluator, int nearestNeighbors,
            EliteArchive<TGenome> archive = null)
        {
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluator = phenomeEvaluator;
            _enablePhenomeCaching = true;
            _populationEvaluationMethod = EvaluateAllBehaviors_Caching;
            _batchEvaluationMethod = EvaluateBatchBehavior_Caching;
            _nearestNeighbors = nearestNeighbors;
            _eliteArchive = archive;
        }

        /// <summary>
        ///     Constructs serial genome list behavior evaluator, customizing only the phenome behavior evaluator and setting the
        ///     caching method.  Also sets the number of nearest neighbors to utilize in behavior distance calculations and accepts
        ///     an optional elite archive.
        /// </summary>
        /// <param name="genomeDecoder">The genome decoder to use.</param>
        /// <param name="phenomeEvaluator">The phenome evaluator.</param>
        /// <param name="enablePhenomeCaching">Whether or not to enable phenome caching.</param>
        /// <param name="nearestNeighbors">The number of nearest neighbors to use in behavior distance calculations.</param>
        /// <param name="archive">A reference to the elite archive (optional).</param>
        public SerialGenomeBehaviorEvaluator(IGenomeDecoder<TGenome, TPhenome> genomeDecoder,
            IPhenomeEvaluator<TPhenome, BehaviorInfo> phenomeEvaluator,
            bool enablePhenomeCaching, int nearestNeighbors, EliteArchive<TGenome> archive = null)
        {
            _genomeDecoder = genomeDecoder;
            _phenomeEvaluator = phenomeEvaluator;
            _enablePhenomeCaching = enablePhenomeCaching;
            _nearestNeighbors = nearestNeighbors;
            _eliteArchive = archive;

            if (_enablePhenomeCaching)
            {
                _populationEvaluationMethod = EvaluateAllBehaviors_Caching;
                _batchEvaluationMethod = EvaluateBatchBehavior_Caching;
            }
            else
            {
                _populationEvaluationMethod = EvaluateAllBehaviors_NonCaching;
                _batchEvaluationMethod = EvaluateBatchBehavior_NonCaching;
            }
        }

        /// <summary>
        ///     Gets the total number of individual genome evaluations that have been performed by this evaluator.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _phenomeEvaluator.EvaluationCount; }
        }

        /// <summary>
        ///     Gets a value indicating whether some goal fitness has been achieved and that
        ///     the the evolutionary algorithm/search should stop. This property's value can remain false
        ///     to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _phenomeEvaluator.StopConditionSatisfied; }
        }

        /// <summary>
        ///     Evaluates a the behavior-based fitness of a list of genomes. Here we decode each genome in series using the
        ///     contained IGenomeDecoder and evaluate the resulting TPhenome using the contained IPhenomeEvaluator.
        /// </summary>
        /// <param name="genomeList">The list of genomes under evaluation.</param>
        public void Evaluate(IList<TGenome> genomeList)
        {
            _populationEvaluationMethod(genomeList);
        }

        /// <summary>
        ///     Evaluates a the behavior-based fitness of a single genome versus the given list of genomes. Here we decode each
        ///     genome in series using the contained IGenomeDecoder and evaluate the resulting TPhenome using the contained
        ///     IPhenomeEvaluator.
        /// </summary>
        /// <param name="genomesToEvaluate">The genomes under evaluation.</param>
        /// <param name="population">The genomes against which to evaluate.</param>
        public void Evaluate(IList<TGenome> genomesToEvaluate, IList<TGenome> population)
        {
            _batchEvaluationMethod(genomesToEvaluate, population);
        }

        /// <summary>
        ///     Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {
            _phenomeEvaluator.Reset();
        }

        private void EvaluateBehavior_NonCaching(TGenome genome)
        {
            var phenome = _genomeDecoder.Decode(genome);
            if (null == phenome)
            {
                // Non-viable genome.
                genome.EvaluationInfo.SetFitness(0.0);
                genome.EvaluationInfo.AuxFitnessArr = null;
                genome.EvaluationInfo.BehaviorCharacterization = new double[0];
            }
            else
            {
                // EvaluateFitness the behavior and update the genome's behavior characterization
                var behaviorInfo = _phenomeEvaluator.Evaluate(phenome);
                genome.EvaluationInfo.BehaviorCharacterization = behaviorInfo.Behaviors;
            }
        }

        private void EvaluateBehavior_Caching(TGenome genome)
        {
            var phenome = _genomeDecoder.Decode(genome);
            if (null == phenome)
            {
                // Decode the phenome and store a ref against the genome.
                phenome = _genomeDecoder.Decode(genome);
                genome.CachedPhenome = phenome;
            }

            if (null == phenome)
            {
                // Non-viable genome.
                genome.EvaluationInfo.SetFitness(0.0);
                genome.EvaluationInfo.AuxFitnessArr = null;
                genome.EvaluationInfo.BehaviorCharacterization = new double[0];
            }
            else
            {
                // EvaluateFitness the behavior and update the genome's behavior characterization
                var behaviorInfo = _phenomeEvaluator.Evaluate(phenome);
                genome.EvaluationInfo.BehaviorCharacterization = behaviorInfo.Behaviors;
            }
        }

        public void EvaluateFitness(TGenome genome, IList<TGenome> genomeList)
        {
            // Compare the current genome's behavior to its k-nearest neighbors in behavior space
            var fitness =
                BehaviorUtils<TGenome>.CalculateBehavioralDistance(genome.EvaluationInfo.BehaviorCharacterization,
                    genomeList, _nearestNeighbors, _eliteArchive);

            // Update the fitness as the behavioral novelty
            var fitnessInfo = new FitnessInfo(fitness, fitness);
            genome.EvaluationInfo.SetFitness(fitnessInfo._fitness);
            genome.EvaluationInfo.AuxFitnessArr = fitnessInfo._auxFitnessArr;
        }

        private void EvaluateAllBehaviors_NonCaching(IList<TGenome> genomeList)
        {
            // Decode and evaluate the behavior of each genome in turn.
            foreach (var genome in genomeList)
            {
                EvaluateBehavior_NonCaching(genome);
            }

            // After the behavior of each genome in the current population has been evaluated,
            // iterate again through each genome and compare its behavioral novelty (distance)
            // to its k-nearest neighbors in behavior space (and the archive if applicable)
            foreach (var genome in genomeList)
            {
                EvaluateFitness(genome, genomeList);

                // Add the genome to the archive if it qualifies
                _eliteArchive?.TestAndAddCandidateToArchive(genome);
            }
        }

        private void EvaluateBatchBehavior_NonCaching(IList<TGenome> genomesToEvaluate, IList<TGenome> population)
        {
            // Decode and evaluate the behavior of the genomes under evaluation
            foreach (var genome in genomesToEvaluate)
            {
                EvaluateBehavior_NonCaching(genome);
            }

            // After the behavior of each genome in the offspring batch has been evaluated,
            // iterate through each genome and compare its behavioral novelty (distance) to its 
            // k -nearest neighbors from the population in behavior space (and the archive if applicable)
            foreach (var genome in genomesToEvaluate)
            {
                EvaluateFitness(genome, population);

                // Add the genome to the archive if it qualifies
                _eliteArchive?.TestAndAddCandidateToArchive(genome);
            }
        }

        private void EvaluateAllBehaviors_Caching(IList<TGenome> genomeList)
        {
            // Decode and evaluate the behavior of each genome in turn.
            foreach (var genome in genomeList)
            {
                EvaluateBehavior_Caching(genome);
            }

            // After the behavior of each genome in the current population has been evaluated,
            // iterate again through each genome and compare its behavioral novelty (distance)
            // to its k-nearest neighbors in behavior space (and the archive if applicable)
            foreach (var genome in genomeList)
            {
                EvaluateFitness(genome, genomeList);

                // Add the genome to the archive if it qualifies
                _eliteArchive?.TestAndAddCandidateToArchive(genome);
            }
        }

        private void EvaluateBatchBehavior_Caching(IList<TGenome> genomesToEvaluate, IList<TGenome> population)
        {
            // Decode and evaluate the behavior of the genomes under evaluation
            foreach (var genome in genomesToEvaluate)
            {
                EvaluateBehavior_Caching(genome);
            }

            // After the behavior of each genome in the offspring batch has been evaluated,
            // iterate through each genome and compare its behavioral novelty (distance) to its 
            // k -nearest neighbors from the population in behavior space (and the archive if applicable)
            foreach (var genome in genomesToEvaluate)
            {
                EvaluateFitness(genome, population);

                // Add the genome to the archive if it qualifies
                _eliteArchive?.TestAndAddCandidateToArchive(genome);
            }
        }

        private delegate void PopulationEvaluationMethod(IList<TGenome> genomeList);

        private delegate void BatchEvaluationMethod(IList<TGenome> genomesToEvaluate, IList<TGenome> population);
    }
}