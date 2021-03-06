/* ***************************************************************************
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using log4net;
using Redzen.Random;
using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.EvolutionAlgorithms.Statistics;
using SharpNeat.Genomes.Neat;
using SharpNeat.Loggers;
using SharpNeat.NoveltyArchives;
using SharpNeat.SpeciationStrategies;
using SharpNeat.Utility;

#endregion

// Disable missing comment warnings for non-private variables.
#pragma warning disable 1591

namespace SharpNeat.EvolutionAlgorithms
{
    /// <summary>
    ///     Abstract class providing some common/baseline data and methods for implementions of INeatEvolutionAlgorithm.
    /// </summary>
    /// <typeparam name="TGenome">The genome type that the algorithm will operate on.</typeparam>
    public abstract class AbstractComplexifyingEvolutionAlgorithm<TGenome> : AbstractEvolutionAlgorithm<TGenome>,
        IComplexifyingEvolutionAlgorithm<TGenome>, ILoggable
        where TGenome : class, IGenome<TGenome>
    {
        private static readonly ILog __log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Logging Methods

        /// <summary>
        ///     Returns AbstractNeatEvolutionAlgorithm LoggableElements.
        /// </summary>
        /// <param name="logFieldEnableMap">
        ///     Dictionary of logging fields that can be enabled or disabled based on the specification
        ///     of the calling routine.
        /// </param>
        /// <returns>The LoggableElements for AbstractNeatEvolutionAlgorithm.</returns>
        public List<LoggableElement> GetLoggableElements(IDictionary<FieldElement, bool> logFieldEnableMap = null)
        {
            return new List<LoggableElement>
            {
                (logFieldEnableMap?.ContainsKey(EvolutionFieldElements.SpecieCount) == true &&
                 logFieldEnableMap[EvolutionFieldElements.SpecieCount])
                    ? new LoggableElement(EvolutionFieldElements.SpecieCount, SpecieList?.Count)
                    : null,
                (logFieldEnableMap?.ContainsKey(EvolutionFieldElements.RunPhase) == true &&
                 logFieldEnableMap[EvolutionFieldElements.RunPhase])
                    ? new LoggableElement(EvolutionFieldElements.RunPhase, RunPhase)
                    : null,
                (logFieldEnableMap?.ContainsKey(EvolutionFieldElements.PopulationSize) == true &&
                 logFieldEnableMap[EvolutionFieldElements.PopulationSize])
                    ? new LoggableElement(EvolutionFieldElements.PopulationSize, GenomeList.Count)
                    : null
            };
        }

        #endregion

        #region Base Constructor

        /// <summary>
        ///     Abstract constructor accepting custom NEAT parameters.
        /// </summary>
        protected AbstractComplexifyingEvolutionAlgorithm(EvolutionAlgorithmParameters eaParams, IEvolutionAlgorithmStats stats)
        {
            EaParams = eaParams;
            EaParamsComplexifying = EaParams;
            EaParamsSimplifying = EaParams.CreateSimplifyingParameters();
            Statistics = stats;
            ComplexityRegulationMode = ComplexityRegulationMode.Complexifying;
        }

        #endregion

        #region Helper methods

        /// <summary>
        ///     Updates _currentBestGenome and _bestSpecieIdx, these are the fittest genome and index of the specie
        ///     containing the fittest genome respectively.
        ///     This method assumes that all specie genomes are sorted fittest first and can therefore save much work
        ///     by not having to scan all genomes.
        ///     Note. We may have several genomes with equal best fitness, we just select one of them in that case.
        /// </summary>
        protected void UpdateBestGenome(bool isMaximization = true, bool useAuxFitness = false)
        {
            // If all genomes have the same fitness (including zero) then we simply return the first genome.
            TGenome bestGenome = null;
            var bestFitness = -1.0;
            var bestSpecieIdx = -1;

            var count = SpecieList.Count;
            for (var i = 0; i < count; i++)
            {
                // Get the specie's first genome. Genomes are sorted, therefore this is also the fittest 
                // genome in the specie.
                var genome = SpecieList[i].GenomeList[0];

                double specieChampGenomeFitness = useAuxFitness
                    ? genome.EvaluationInfo.AuxFitnessArr[0]._value
                    : genome.EvaluationInfo.Fitness;

                if (isMaximization)
                {
                    if (specieChampGenomeFitness > bestFitness)
                    {
                        bestGenome = genome;
                        bestFitness = specieChampGenomeFitness;
                        bestSpecieIdx = i;
                    }
                }
                else
                {
                    if (bestFitness <= 0 || specieChampGenomeFitness < bestFitness)
                    {
                        bestGenome = genome;
                        bestFitness = specieChampGenomeFitness;
                        bestSpecieIdx = i;
                    }
                }
            }

            CurrentChampGenome = bestGenome;
            BestSpecieIndex = bestSpecieIdx;
        }

        /// <summary>
        ///     Updates _currentBestGenome without taking species into consideration.  This is considered the fittest genome in the
        ///     population.
        /// </summary>
        protected void UpdateBestGenomeWithoutSpeciation(bool isMaximization, bool useAuxFitness)
        {
            // If all genomes have the same fitness (including zero) then we simply return the first genome.
            TGenome bestGenome = null;
            var bestFitness = -1.0;

            // Iterate through the genome list, testing for the highest fitness genome
            foreach (TGenome genome in GenomeList)
            {
                // Use either the primary fitness or auxiliary fitness based on the
                // choice upon method invocation
                double curGenomeFitness = useAuxFitness
                    ? genome.EvaluationInfo.AuxFitnessArr[0]._value
                    : genome.EvaluationInfo.Fitness;

                if (isMaximization)
                {
                    if (curGenomeFitness > bestFitness)
                    {
                        bestGenome = genome;
                        bestFitness = curGenomeFitness;
                    }
                }
                else
                {
                    if (bestFitness <= 0 || curGenomeFitness < bestFitness)
                    {
                        bestGenome = genome;
                        bestFitness = curGenomeFitness;
                    }
                }
            }

            CurrentChampGenome = bestGenome;
        }

        /// <summary>
        ///     Updates the NeatAlgorithmStats object.
        /// </summary>
        protected void UpdateStats(bool updateSpeciesStats, bool useAuxFitness = false)
        {
            Statistics.Generation = CurrentGeneration;
            Statistics.TotalEvaluationCount = GenomeEvaluator.EvaluationCount;

            // Evaluation per second.
            var now = DateTime.Now;
            var duration = now - Statistics.EvalsPerSecLastSampleTime;

            // To smooth out the evals per sec statistic we only update if at least 1 second has elapsed 
            // since it was last updated.
            if (duration.Ticks > 9999)
            {
                var evalsSinceLastUpdate =
                    (long) (GenomeEvaluator.EvaluationCount - Statistics.EvalsCountAtLastUpdate);
                Statistics.EvaluationsPerSec = (int) ((evalsSinceLastUpdate*1e7)/duration.Ticks);

                // Reset working variables.
                Statistics.EvalsCountAtLastUpdate = GenomeEvaluator.EvaluationCount;
                Statistics.EvalsPerSecLastSampleTime = now;
            }

            // Fitness and complexity stats.
            var totalFitness = useAuxFitness
                ? GenomeList[0].EvaluationInfo.AuxFitnessArr[0]._value
                : GenomeList[0].EvaluationInfo.Fitness;
            var totalComplexity = GenomeList[0].Complexity;
            var minComplexity = totalComplexity;
            var maxComplexity = totalComplexity;

            var count = GenomeList.Count;
            for (var i = 1; i < count; i++)
            {
                totalFitness += useAuxFitness
                    ? GenomeList[i].EvaluationInfo.AuxFitnessArr[0]._value
                    : GenomeList[i].EvaluationInfo.Fitness;
                totalComplexity += GenomeList[i].Complexity;
                minComplexity = Math.Min(minComplexity, GenomeList[i].Complexity);
                maxComplexity = Math.Max(maxComplexity, GenomeList[i].Complexity);
            }

            Statistics.MaxFitness = useAuxFitness
                ? CurrentChampGenome.EvaluationInfo.AuxFitnessArr[0]._value
                : CurrentChampGenome.EvaluationInfo.Fitness;
            Statistics.MeanFitness = totalFitness/count;

            Statistics.MinComplexity = minComplexity;
            Statistics.MaxComplexity = maxComplexity;
            Statistics.MeanComplexity = totalComplexity/count;

            if (updateSpeciesStats)
            {
                // Specie champs mean fitness.
                var totalSpecieChampFitness = useAuxFitness
                    ? SpecieList[0].GenomeList[0].EvaluationInfo.AuxFitnessArr[0]._value
                    : SpecieList[0].GenomeList[0].EvaluationInfo.Fitness;
                var specieCount = SpecieList.Count;
                for (var i = 1; i < specieCount; i++)
                {
                    totalSpecieChampFitness += useAuxFitness
                        ? SpecieList[i].GenomeList[0].EvaluationInfo.AuxFitnessArr[0]._value
                        : SpecieList[i].GenomeList[0].EvaluationInfo.Fitness;
                }
                Statistics.MeanSpecieChampFitness = totalSpecieChampFitness/specieCount;
            }

            // Moving averages.
            Statistics.PrevBestFitnessMa = Statistics.BestFitnessMa.Mean;
            Statistics.BestFitnessMa.Enqueue(Statistics.MaxFitness);

            Statistics.PrevMeanSpecieChampFitnessMa = Statistics.MeanSpecieChampFitnessMa.Mean;
            Statistics.MeanSpecieChampFitnessMa.Enqueue(Statistics.MeanSpecieChampFitness);

            Statistics.PrevComplexityMa = Statistics.ComplexityMa.Mean;
            Statistics.ComplexityMa.Enqueue(Statistics.MeanComplexity);

            // Compute population-specific statistics
            Statistics.ComputeAlgorithmSpecificPopulationStats(GenomeList);
        }

        /// <summary>
        ///     Sorts the genomes within each species fittest first, secondary sorts on age.
        /// </summary>
        protected void SortSpecieGenomes()
        {
            int minSize = SpecieList[0].GenomeList.Count;
            int maxSize = minSize;
            int specieCount = SpecieList.Count;

            for (int i = 0; i < specieCount; i++)
            {
                SpecieList[i].GenomeList.Sort(GenomeFitnessComparer<TGenome>.Singleton);
                minSize = Math.Min(minSize, SpecieList[i].GenomeList.Count);
                maxSize = Math.Max(maxSize, SpecieList[i].GenomeList.Count);
            }

            // Update stats.
            Statistics.MinSpecieSize = minSize;
            Statistics.MaxSpecieSize = maxSize;
        }

        /// <summary>
        ///     Clear the genome list within each specie.
        /// </summary>
        protected void ClearAllSpecies()
        {
            foreach (Specie<TGenome> specie in SpecieList)
            {
                specie.GenomeList.Clear();
            }
        }

        /// <summary>
        ///     Rebuild _genomeList from genomes held within the species.
        /// </summary>
        protected void RebuildGenomeList()
        {
            GenomeList.Clear();
            foreach (Specie<TGenome> specie in SpecieList)
            {
                ((List<TGenome>) GenomeList).AddRange(specie.GenomeList);
            }
        }

        #endregion

        #region INeatEvolutionAlgorithm<TGenome> Members

        /// <summary>
        ///     Gets the algorithm statistics object.
        /// </summary>
        public IEvolutionAlgorithmStats Statistics { get; protected set; }

        /// <summary>
        ///     Gets the current complexity regulation mode.
        /// </summary>
        public ComplexityRegulationMode ComplexityRegulationMode { get; protected set; }

        /// <summary>
        ///     Gets a list of all current species. The genomes contained within the species are the same genomes
        ///     available through the GenomeList property.
        /// </summary>
        public IList<Specie<TGenome>> SpecieList { get; protected set; }

        #endregion

        #region Instance fields

        /// <summary>
        ///     Parameters for NEAT evolutionary algorithm control (mutation rate, crossover rate, etc.).
        /// </summary>
        protected EvolutionAlgorithmParameters EaParams;

        /// <summary>
        ///     EA Parameters for complexification.
        /// </summary>
        protected EvolutionAlgorithmParameters EaParamsComplexifying;

        /// <summary>
        ///     EA Parameters for simplification.
        /// </summary>
        protected EvolutionAlgorithmParameters EaParamsSimplifying;

        /// <summary>
        ///     The speciation strategy.
        /// </summary>
        protected ISpeciationStrategy<TGenome> SpeciationStrategy;

        /// <summary>
        ///     The complexity regulation strategy (for simplifying networks).
        /// </summary>
        protected IComplexityRegulationStrategy ComplexityRegulationStrategy;

        /// <summary>
        ///     Index of the specie that contains _currentBestGenome.
        /// </summary>
        protected int BestSpecieIndex;

        /// <summary>
        ///     Random number generator.
        /// </summary>
        protected readonly IRandomSource RandomNumGenerator = RandomDefaults.CreateRandomSource();

        /// <summary>
        ///     Optional map of logging fields and their respective "enabled status" to dynamically control what is logged.
        /// </summary>
        protected IDictionary<FieldElement, bool> _logFieldEnabledMap;

        #endregion

        #region Initialization Methods

        /// <summary>
        ///     Initializes the evolution algorithm with the provided IGenomeFitnessEvaluator, IGenomeFactory
        ///     and an initial population of genomes.
        /// </summary>
        /// <param name="genomeFitnessEvaluator">The genome evaluation scheme for the evolution algorithm.</param>
        /// <param name="genomeFactory">
        ///     The factory that was used to create the genomeList and which is therefore referenced by the
        ///     genomes.
        /// </param>
        /// <param name="genomeList">An initial genome population.</param>
        /// <param name="maxGenerations">The maximum number of generations that the algorithm is allowed to run.</param>
        /// <param name="maxEvaluations">The maximum number of evaluations that the algorithm is allowed to run.</param>
        /// <param name="abstractNoveltyArchive">The cross-generational archive of high-performing/novel genomes (optional).</param>
        public override void Initialize(IGenomeEvaluator<TGenome> genomeFitnessEvaluator,
            IGenomeFactory<TGenome> genomeFactory,
            List<TGenome> genomeList,
            int? maxGenerations,
            ulong? maxEvaluations,
            AbstractNoveltyArchive<TGenome> abstractNoveltyArchive = null)
        {
            base.Initialize(genomeFitnessEvaluator, genomeFactory, genomeList, maxGenerations, maxEvaluations,
                abstractNoveltyArchive);
            Initialize();
        }

        /// <summary>
        ///     Initializes the evolution algorithm with the provided IGenomeFitnessEvaluator, IGenomeFactory
        ///     and an initial population of genomes.
        /// </summary>
        /// <param name="genomeFitnessEvaluator">The genome evaluation scheme for the evolution algorithm.</param>
        /// <param name="genomeFactory">
        ///     The factory that was used to create the genomeList and which is therefore referenced by the
        ///     genomes.
        /// </param>
        /// <param name="genomeList">An initial genome population.</param>
        /// <param name="targetPopulationSize">The ceiling population size at which the algorithm should cap out.</param>
        /// <param name="maxGenerations">The maximum number of generations that the algorithm is allowed to run.</param>
        /// <param name="maxEvaluations">The maximum number of evaluations that the algorithm is allowed to run.</param>
        /// <param name="abstractNoveltyArchive">
        ///     The persistent archive of genomes posessing a unique trait with respect to a behavior
        ///     characterization (optional).
        /// </param>
        public override void Initialize(IGenomeEvaluator<TGenome> genomeFitnessEvaluator,
            IGenomeFactory<TGenome> genomeFactory,
            List<TGenome> genomeList, int targetPopulationSize, int? maxGenerations, ulong? maxEvaluations,
            AbstractNoveltyArchive<TGenome> abstractNoveltyArchive = null)
        {
            base.Initialize(genomeFitnessEvaluator, genomeFactory, genomeList, targetPopulationSize, maxGenerations,
                maxEvaluations,
                abstractNoveltyArchive);
            Initialize();
        }

        /// <summary>
        ///     Initializes the evolution algorithm with the provided IGenomeFitnessEvaluator
        ///     and an IGenomeFactory that can be used to create an initial population of genomes.
        /// </summary>
        /// <param name="genomeFitnessEvaluator">The genome evaluation scheme for the evolution algorithm.</param>
        /// <param name="genomeFactory">
        ///     The factory that was used to create the genomeList and which is therefore referenced by the
        ///     genomes.
        /// </param>
        /// <param name="populationSize">The number of genomes to create for the initial population.</param>
        /// <param name="maxGenerations">The maximum number of generations that the algorithm is allowed to run.</param>
        /// <param name="maxEvaluations">The maximum number of evaluations that the algorithm is allowed to run.</param>
        /// <param name="abstractNoveltyArchive">The cross-generational archive of high-performing/novel genomes (optional).</param>
        public override void Initialize(IGenomeEvaluator<TGenome> genomeFitnessEvaluator,
            IGenomeFactory<TGenome> genomeFactory,
            int populationSize,
            int? maxGenerations,
            ulong? maxEvaluations,
            AbstractNoveltyArchive<TGenome> abstractNoveltyArchive = null)
        {
            base.Initialize(genomeFitnessEvaluator, genomeFactory, populationSize, maxGenerations, maxEvaluations,
                abstractNoveltyArchive);
            Initialize();
        }

        /// <summary>
        ///     Code common to both public Initialize methods.
        /// </summary>
        protected virtual void Initialize()
        {
            // Open the loggers
            EvolutionLogger?.Open();
            PopulationLogger?.Open();

            // Set the run phase on the loggers
            EvolutionLogger?.UpdateRunPhase(RunPhase);
            PopulationLogger?.UpdateRunPhase(RunPhase);

            // Write out the headers (for the champ genome, we don't care which genome is used)
            EvolutionLogger?.LogHeader(GetLoggableElements(_logFieldEnabledMap),
                Statistics.GetLoggableElements(_logFieldEnabledMap),
                (GenomeList[0] as NeatGenome)?.GetLoggableElements(_logFieldEnabledMap));
            PopulationLogger?.LogHeader(new List<LoggableElement>
            {
                _logFieldEnabledMap.ContainsKey(PopulationFieldElements.RunPhase) &&
                _logFieldEnabledMap[PopulationFieldElements.RunPhase]
                    ? new LoggableElement(PopulationFieldElements.RunPhase, null)
                    : null,
                _logFieldEnabledMap.ContainsKey(PopulationFieldElements.Generation) &&
                _logFieldEnabledMap[PopulationFieldElements.Generation]
                    ? new LoggableElement(PopulationFieldElements.Generation, null)
                    : null,
                _logFieldEnabledMap.ContainsKey(PopulationFieldElements.GenomeId) &&
                _logFieldEnabledMap[PopulationFieldElements.GenomeId]
                    ? new LoggableElement(PopulationFieldElements.GenomeId, null)
                    : null,
                _logFieldEnabledMap[PopulationFieldElements.SpecieId]
                    ? new LoggableElement(PopulationFieldElements.SpecieId, null)
                    : null
            });
            GenomeLogger?.LogHeader(new List<LoggableElement>
            {
                _logFieldEnabledMap.ContainsKey(GenomeFieldElements.RunPhase) &&
                _logFieldEnabledMap[GenomeFieldElements.RunPhase]
                    ? new LoggableElement(GenomeFieldElements.RunPhase, null)
                    : null,
                _logFieldEnabledMap.ContainsKey(GenomeFieldElements.GenomeId) &&
                _logFieldEnabledMap[GenomeFieldElements.GenomeId]
                    ? new LoggableElement(GenomeFieldElements.GenomeId, null)
                    : null,
                _logFieldEnabledMap.ContainsKey(GenomeFieldElements.GenomeXml) &&
                _logFieldEnabledMap[GenomeFieldElements.GenomeXml]
                    ? new LoggableElement(GenomeFieldElements.GenomeXml, null)
                    : null
            });

            // Initialize the genome evalutor
            GenomeEvaluator.Initialize();

            // Evaluate the genomes.
            GenomeEvaluator.Evaluate(GenomeList, CurrentGeneration);

            // Speciate the genomes.
            SpecieList = SpeciationStrategy.InitializeSpeciation(GenomeList, EaParams.SpecieCount);
            Debug.Assert(!SpeciationUtils<TGenome>.TestEmptySpecies(SpecieList),
                "Speciation resulted in one or more empty species.");

            // Sort the genomes in each specie fittest first, secondary sort youngest first.
            SortSpecieGenomes();

            // Store ref to best genome.
            UpdateBestGenome();
        }

        #endregion
    }
}