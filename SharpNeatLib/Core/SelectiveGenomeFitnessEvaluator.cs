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

#endregion

namespace SharpNeat.Core
{
    /// <summary>
    ///     An IGenomeFitnessEvaluator that wraps another IGenomeFitnessEvaluator and filters/selects
    ///     the genomes that are to be passed to the wrapped evaluator based on some predicate/test.
    ///     This class supports evaluation schemes whereby not all genomes in a population are evaluated
    ///     on each generation. E.g. if we wish to evaluate a genome that persists between generations
    ///     (i.e. elite genomes) just once (deterministic fitness score), or every N generations.
    ///     A typical use would be to wrap SimpleGenomeListEvaulator or ParallelGenomeFitnessEvaluator.
    ///     Genomes that skip evaluation have their EvaluationInfo.EvaluationPassCount property
    ///     incremented.
    /// </summary>
    public class SelectiveGenomeFitnessEvaluator<TGenome> : IGenomeEvaluator<TGenome>
        where TGenome : class, IGenome<TGenome>
    {
        private readonly IGenomeEvaluator<TGenome> _innerFitnessEvaluator;
        private readonly Predicate<TGenome> _selectionPredicate;
        private uint _currentGeneration;

        #region Constructor

        /// <summary>
        ///     Construct with the provided IGenomeDecoder and IPhenomeEvaluator.
        /// </summary>
        public SelectiveGenomeFitnessEvaluator(IGenomeEvaluator<TGenome> innerFitnessEvaluator,
            Predicate<TGenome> selectionPredicate)
        {
            _innerFitnessEvaluator = innerFitnessEvaluator;
            _selectionPredicate = selectionPredicate;
        }

        #endregion

        #region IGenomeFitnessEvaluator<TGenome> Members

        /// <summary>
        ///     Gets the total number of individual genome evaluations that have been performed by this evaluator.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _innerFitnessEvaluator.EvaluationCount; }
        }

        /// <summary>
        ///     Gets a value indicating whether some goal fitness has been achieved and that
        ///     the the evolutionary algorithm/search should stop. This property's value can remain false
        ///     to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _innerFitnessEvaluator.StopConditionSatisfied; }
        }

        /// <summary>
        ///     Evalutes a single genome against a list of genomes.
        /// </summary>
        public void Evaluate(IList<TGenome> genomesToEvaluate, IList<TGenome> population, uint currentGeneration,
            bool runSimulation = true)
        {
            // TODO: Need to implement this
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Invokes initialization routine on inner evaluator.
        /// </summary>
        public void Initialize()
        {
            _innerFitnessEvaluator.Initialize();
        }

        /// <summary>
        ///     Cleans up inner evaluator state after end of execution or upon execution interruption.
        /// </summary>
        public void Cleanup()
        {
            _innerFitnessEvaluator.Cleanup();
        }

        /// <summary>
        ///     Evaluates a list of genomes. Here we select the genomes to be evaluated before invoking
        ///     _innerFitnessEvaluator to evaluate them.
        /// </summary>
        public void Evaluate(IList<TGenome> genomeList, uint currentGeneration, bool runSimulation = true)
        {
            // Select the genomes to be evaluated. Place them in a temporary list of genomes to be 
            // evaluated after the genome selection loop. The selection is not performed in series
            // so that we can wrap parallel execution versions of IGenomeFitnessEvaluator.
            List<TGenome> filteredList = new List<TGenome>(genomeList.Count);
            foreach (TGenome genome in genomeList)
            {
                if (_selectionPredicate.Invoke(genome))
                {
                    // Add the genome to the temp list for evaluation later.
                    filteredList.Add(genome);
                }
                else
                {
                    // Register that the genome skipped an evaluation.
                    genome.EvaluationInfo.EvaluationPassCount++;
                }
            }

            // EvaluateFitness selected genomes.
            _innerFitnessEvaluator.Evaluate(filteredList, currentGeneration, runSimulation);
        }

        /// <summary>
        ///     Reset the internal state of the evaluation scheme if any exists.
        /// </summary>
        public void Reset()
        {
            _innerFitnessEvaluator.Reset();
        }

        #endregion

        #region Commonly Used Predicates

        /// <summary>
        ///     Test that selects genomes that have never been evaluated.
        /// </summary>
        public static Predicate<TGenome> CreatePredicate_OnceOnly()
        {
            return delegate(TGenome genome) { return !genome.EvaluationInfo.IsEvaluated; };
        }

        /// <summary>
        ///     Selects genomes for evaluation every N attempts/generations.
        /// </summary>
        public static Predicate<TGenome> CreatePredicate_PeriodicReevaluation(int period)
        {
            if (period < 1)
            {
                throw new ArgumentOutOfRangeException("period", "Period argument must be >= 1");
            }

            return delegate(TGenome genome) { return genome.EvaluationInfo.TotalEvaluationCount%period == 0; };
        }

        #endregion
    }
}