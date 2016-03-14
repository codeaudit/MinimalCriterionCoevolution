﻿#region

using System;
using SharpNeat.Behaviors;
using SharpNeat.Core;

#endregion

namespace SharpNeat.Domains
{
    /// <summary>
    ///     Defines the behavior characterization type to use (end point, trajectory, etc.) for a given experiment.
    /// </summary>
    public enum BehaviorCharacterizationType
    {
        /// <summary>
        ///     Indicates the end point behavior characterization type.
        /// </summary>
        EndPoint,

        /// <summary>
        ///     Indicates the trajectory behavior characterization type.
        /// </summary>
        Trajectory
    }

    /// <summary>
    ///     Defines the minimal criteria type to use (euclidean location, etc.) for a given experiment.
    /// </summary>
    public enum MinimalCriteriaType
    {
        /// <summary>
        ///     Indicates the euclidean location minimal criteria.
        /// </summary>
        EuclideanLocation,

        /// <summary>
        ///     Indicates the fixed point euclidean distance minimal criteria.
        /// </summary>
        FixedPointEuclideanDistance,

        /// <summary>
        ///     Indicates the population centroid euclidean distance criteria.
        /// </summary>
        PopulationCentroidEuclideanDistance,

        /// <summary>
        ///     Indicates the mileage minimal criteria.
        /// </summary>
        Mileage
    }

    /// <summary>
    ///     Provides utility methods for behavior characterizations.
    /// </summary>
    public static class BehaviorCharacterizationUtil
    {
        /// <summary>
        ///     Determines the appropriate behavior characterization type based on the given string value.
        /// </summary>
        /// <param name="strBehavioralCharacterization">The string-valued behavior characterization.</param>
        /// <returns>The behavior characterization domain type.</returns>
        public static BehaviorCharacterizationType ConvertStringToBehavioralCharacterization(
            String strBehavioralCharacterization)
        {
            if ("EndMazeStructurePoint".Equals(strBehavioralCharacterization, StringComparison.InvariantCultureIgnoreCase) ||
                "End MazeStructurePoint".Equals(strBehavioralCharacterization, StringComparison.InvariantCultureIgnoreCase))
            {
                return BehaviorCharacterizationType.EndPoint;
            }
            return BehaviorCharacterizationType.Trajectory;
        }

        /// <summary>
        ///     Determines the appropriate minimal criteria type based on the given string value.
        /// </summary>
        /// <param name="strMinimalCriteria">The string-valued minimal criteria.</param>
        /// <returns>The minimal criteria domain type.</returns>
        public static MinimalCriteriaType ConvertStringToMinimalCriteria(String strMinimalCriteria)
        {
            if ("EuclideanLocation".Equals(strMinimalCriteria, StringComparison.InvariantCultureIgnoreCase) ||
                "Euclidean Location".Equals(strMinimalCriteria, StringComparison.InvariantCultureIgnoreCase))
            {
                return MinimalCriteriaType.EuclideanLocation;
            }
            if ("FixedPointEuclideanDistance".Equals(strMinimalCriteria, StringComparison.InvariantCultureIgnoreCase) ||
                "Fixed MazeStructurePoint Euclidean Distance".Equals(strMinimalCriteria, StringComparison.InvariantCultureIgnoreCase))
            {
                return MinimalCriteriaType.FixedPointEuclideanDistance;
            }
            if (
                "PopulationCentroidEuclideanDistance".Equals(strMinimalCriteria,
                    StringComparison.InvariantCultureIgnoreCase) ||
                "Population Centroid Euclidean Distance".Equals(strMinimalCriteria,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return MinimalCriteriaType.PopulationCentroidEuclideanDistance;
            }

            return MinimalCriteriaType.Mileage;
        }

        /// <summary>
        ///     Creates a new behavior characterization based on the given string-valued behavior characterization.
        /// </summary>
        /// <param name="strBehaviorCharacterization">
        ///     String representation of the behavior charcterization type for which to create a new behavior characterization.
        /// </param>
        /// <returns>An instantiated behavior characterization.</returns>
        public static IBehaviorCharacterization GenerateBehaviorCharacterization(
            String strBehaviorCharacterization)
        {
            switch (ConvertStringToBehavioralCharacterization(strBehaviorCharacterization))
            {
                case BehaviorCharacterizationType.EndPoint:
                    return new EndPointBehaviorCharacterization();
                default:
                    return new TrajectoryBehaviorCharacterization();
            }
        }

        /// <summary>
        ///     Creates a new behavior characterization based on the given behavior characterization type.
        /// </summary>
        /// <param name="behaviorCharacterizationType">
        ///     The behavior charcterization type for which to create a new behavior
        ///     characterization.
        /// </param>
        /// <returns>An instantiated behavior characterization.</returns>
        public static IBehaviorCharacterization GenerateBehaviorCharacterization(
            BehaviorCharacterizationType behaviorCharacterizationType)
        {
            switch (behaviorCharacterizationType)
            {
                case BehaviorCharacterizationType.EndPoint:
                    return new EndPointBehaviorCharacterization();
                default:
                    return new TrajectoryBehaviorCharacterization();
            }
        }

        /// <summary>
        ///     Creates a new behavior characterization factory based on the given string-valued behavior characterization.
        /// </summary>
        /// <param name="strBehaviorCharacterization">
        ///     String representation of the behavior charcterization type for which to create a new behavior characterization
        ///     factory.
        /// </param>
        /// <param name="minimalCriteria">The minimal criteria to impose upon generated behavior characterizations.</param>
        /// <returns>An instantiated behavior characterization factory.</returns>
        public static IBehaviorCharacterizationFactory GenerateBehaviorCharacterizationFactory(
            String strBehaviorCharacterization, IMinimalCriteria minimalCriteria)
        {
            switch (ConvertStringToBehavioralCharacterization(strBehaviorCharacterization))
            {
                case BehaviorCharacterizationType.EndPoint:
                    return new EndPointBehaviorCharacterizationFactory(minimalCriteria);
                default:
                    return new TrajectoryBehaviorCharacterizationFactory(minimalCriteria);
            }
        }

        /// <summary>
        ///     Creates a new behavior characterization factory based on the given behavior characterization type.
        /// </summary>
        /// <param name="behaviorCharacterizationType">
        ///     The behavior charcterization type for which to create a new behavior
        ///     characterization factory.
        /// </param>
        /// <param name="minimalCriteria">The minimal criteria to impose upon generated behavior characterizations.</param>
        /// <returns>An instantiated behavior characterization factory.</returns>
        public static IBehaviorCharacterizationFactory GenerateBehaviorCharacterizationFactory(
            BehaviorCharacterizationType behaviorCharacterizationType, IMinimalCriteria minimalCriteria)
        {
            switch (behaviorCharacterizationType)
            {
                case BehaviorCharacterizationType.EndPoint:
                    return new EndPointBehaviorCharacterizationFactory(minimalCriteria);
                default:
                    return new TrajectoryBehaviorCharacterizationFactory(minimalCriteria);
            }
        }
    }
}