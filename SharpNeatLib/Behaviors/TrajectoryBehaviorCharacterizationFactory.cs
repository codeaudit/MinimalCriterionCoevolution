﻿#region

using SharpNeat.Core;

#endregion

namespace SharpNeat.Behaviors
{
    /// <summary>
    ///     Defines a factory for creating new trajectory behavior characterization instances.
    /// </summary>
    public class TrajectoryBehaviorCharacterizationFactory : IBehaviorCharacterizationFactory
    {
        /// <summary>
        ///     The minimal criteria to set on the constructed behavior characterizations (optional).
        /// </summary>
        private readonly IMinimalCriteria _minimalCriteria;

        /// <summary>
        ///     The minimal criteria to set on the constructed behavior characterizations (optional).
        /// </summary>
        /// <param name="minimalCriteria">The minimal criteria to set on the constructed trajectory behavior characterizations.</param>
        public TrajectoryBehaviorCharacterizationFactory(IMinimalCriteria minimalCriteria)
        {
            _minimalCriteria = minimalCriteria;
        }

        /// <summary>
        ///     Constructs a new trajectory behavior characterization with the minimal criteria held by the factory (if
        ///     applicable).
        /// </summary>
        /// <returns>Constructed trajectory behavior characterization.</returns>
        public IBehaviorCharacterization CreateBehaviorCharacterization()
        {
            return new TrajectoryBehaviorCharacterization(_minimalCriteria);
        }

        /// <summary>
        ///     Constructs a new trajectory behavior characterization with the specified minimal criteria.
        /// </summary>
        /// <param name="minimalCriteria">The custom minimal criteria to set on the behavior characterization.</param>
        /// <returns>Constructed trajectory behavior characterization with the custom minimal criteria.</returns>
        public IBehaviorCharacterization CreateBehaviorCharacterization(IMinimalCriteria minimalCriteria)
        {
            return new TrajectoryBehaviorCharacterization(minimalCriteria);
        }
    }
}