﻿#region

using System.Collections.Generic;

#endregion

namespace SharpNeat.Loggers
{
    /// <summary>
    ///     Fields capturing data related to population statistics per generation/batch.
    /// </summary>
    public static class EvolutionFieldElements
    {
        /// <summary>
        ///     The number of elements in this log file/table.
        /// </summary>
        public static readonly int NumFieldElements = 50;

        /// <summary>
        ///     The generation of the observation.
        /// </summary>
        public static readonly FieldElement Generation = new FieldElement(0, "Generation");

        /// <summary>
        ///     The run phase (initialization or primary) at the time of the observation.
        /// </summary>
        public static readonly FieldElement RunPhase = new FieldElement(1, "Run Phase");

        /// <summary>
        ///     The number of species in the population.
        /// </summary>
        public static readonly FieldElement SpecieCount = new FieldElement(2, "Specie Count");

        /// <summary>
        ///     The number of offspring that were produced asexually.
        /// </summary>
        public static readonly FieldElement AsexualOffspringCount = new FieldElement(3, "Asexual Offspring Count");

        /// <summary>
        ///     The number of offspring that were produced sexually.
        /// </summary>
        public static readonly FieldElement SexualOffspringCount = new FieldElement(4, "Sexual Offspring Count");

        /// <summary>
        ///     The number of offspring that were produced via interspecies mating.
        /// </summary>
        public static readonly FieldElement InterspeciesOffspringCount = new FieldElement(5,
            "Interspecies Offspring Count");

        /// <summary>
        ///     The total number of offspring that were produced.
        /// </summary>
        public static readonly FieldElement TotalOffspringCount = new FieldElement(6, "Total Offspring Count");

        /// <summary>
        ///     The current population size.
        /// </summary>
        public static readonly FieldElement PopulationSize = new FieldElement(7, "Population Size");

        /// <summary>
        ///     The current minimal criteria threshold (only changes if dynamic).
        /// </summary>
        public static readonly FieldElement
            MinimalCriteriaThreshold = new FieldElement(8, "Minimal Criteria Threshold");

        /// <summary>
        ///     The X position of the minimal criteria point in euclidean space.
        /// </summary>
        public static readonly FieldElement MinimalCriteriaPointX = new FieldElement(9,
            "Minimal Criteria MazeStructurePoint X");

        /// <summary>
        ///     The Y position of the minimal criteria point in euclidean space.
        /// </summary>
        public static readonly FieldElement MinimalCriteriaPointY = new FieldElement(10,
            "Minimal Criteria MazeStructurePoint Y");

        /// <summary>
        ///     The maximum fitness in the current population.
        /// </summary>
        public static readonly FieldElement MaxFitness = new FieldElement(11, "Max Fitness");

        /// <summary>
        ///     The mean fitness of the current population.
        /// </summary>
        public static readonly FieldElement MeanFitness = new FieldElement(12, "Mean Fitness");

        /// <summary>
        ///     The mean fitness of the best-performing genome from each species.
        /// </summary>
        public static readonly FieldElement MeanSpecieChampFitness = new FieldElement(13, "Mean Specie Champ Fitness");

        /// <summary>
        ///     The least complex organism in the current population.
        /// </summary>
        public static readonly FieldElement MinComplexity = new FieldElement(14, "Min Complexity");

        /// <summary>
        ///     The most complex organism in the current populuation.
        /// </summary>
        public static readonly FieldElement MaxComplexity = new FieldElement(15, "Max Complexity");

        /// <summary>
        ///     The mean complexity of all organisms in the current population.
        /// </summary>
        public static readonly FieldElement MeanComplexity = new FieldElement(16, "Mean Complexity");

        /// <summary>
        ///     The size of the smallest extant species.
        /// </summary>
        public static readonly FieldElement MinSpecieSize = new FieldElement(17, "Min Specie Size");

        /// <summary>
        ///     The size of the largest extant species.
        /// </summary>
        public static readonly FieldElement MaxSpecieSize = new FieldElement(18, "Max Specie Size");

        /// <summary>
        ///     The number of evaluations that were executed at the time of the observation.
        /// </summary>
        public static readonly FieldElement TotalEvaluations = new FieldElement(19, "Total Evaluations");

        /// <summary>
        ///     The average number of evaluations executed per second during the current generation/batch.
        /// </summary>
        public static readonly FieldElement EvaluationsPerSecond = new FieldElement(20, "Evaluations per Second");

        /// <summary>
        ///     The ID of the global best performing genome.
        /// </summary>
        public static readonly FieldElement ChampGenomeGenomeId = new FieldElement(21, "Champ Genome ID");

        /// <summary>
        ///     The fitness of the global best performing genome.
        /// </summary>
        public static readonly FieldElement ChampGenomeFitness = new FieldElement(22, "Champ Genome Fitness");

        /// <summary>
        ///     The birth generation of the global best performing genome.
        /// </summary>
        public static readonly FieldElement ChampGenomeBirthGeneration = new FieldElement(23,
            "Champ Genome Birth Generation");

        /// <summary>
        ///     The number of connections in the global best performing genome.
        /// </summary>
        public static readonly FieldElement ChampGenomeConnectionGeneCount = new FieldElement(24,
            "Champ Genome Connection Gene Count");

        /// <summary>
        ///     The number of neurons in the global best performing genome.
        /// </summary>
        public static readonly FieldElement ChampGenomeNeuronGeneCount = new FieldElement(25,
            "Champ Genome Neuron Gene Count");

        /// <summary>
        ///     The total number of genes in the global best performing genome.
        /// </summary>
        public static readonly FieldElement ChampGenomeTotalGeneCount = new FieldElement(26,
            "Champ Genome Total Gene Count");

        /// <summary>
        ///     The total number of evaluations undergone by the global best performing genome.
        /// </summary>
        public static readonly FieldElement ChampGenomeEvaluationCount = new FieldElement(27,
            "Champ Genome Evaluation Count");

        /// <summary>
        ///     The X position of the global best performing genome in euclidean space.
        /// </summary>
        public static readonly FieldElement ChampGenomeBehaviorX = new FieldElement(28, "Champ Genome Behavior X");

        /// <summary>
        ///     The Y position of the global best performing genome in euclidean space.
        /// </summary>
        public static readonly FieldElement ChampGenomeBehaviorY = new FieldElement(29, "Champ Genome Behavior Y");

        /// <summary>
        ///     The distance to the objective of the global best performing genome.
        /// </summary>
        public static readonly FieldElement ChampGenomeDistanceToTarget = new FieldElement(30,
            "Champ Genome Distance to Target");

        /// <summary>
        ///     The genome XML definition for the global best performing genome.
        /// </summary>
        public static readonly FieldElement ChampGenomeXml = new FieldElement(31, "Champ Genome XML");

        /// <summary>
        ///     The minimum number of walls in a maze within the maze population.
        /// </summary>
        public static readonly FieldElement MinWalls = new FieldElement(32, "Min Walls");

        /// <summary>
        ///     The maximum number of walls in a maze within the maze population.
        /// </summary>
        public static readonly FieldElement MaxWalls = new FieldElement(33, "Max Walls");

        /// <summary>
        ///     The mean number of walls among mazes within the maze population.
        /// </summary>
        public static readonly FieldElement MeanWalls = new FieldElement(34, "Mean Walls");

        /// <summary>
        ///     The minimum number of waypoints in a maze within the maze population.
        /// </summary>
        public static readonly FieldElement MinWaypoints = new FieldElement(35, "Min Waypoints");

        /// <summary>
        ///     The maximum number of waypoints in a maze within the maze population.
        /// </summary>
        public static readonly FieldElement MaxWaypoints = new FieldElement(36, "Max Waypoints");

        /// <summary>
        ///     The mean number of waypoints among mazes within the maze population.
        /// </summary>
        public static readonly FieldElement MeanWaypoints = new FieldElement(37, "Mean Waypoints");

        /// <summary>
        ///     The minimum number of junctures in a maze within the maze population.
        /// </summary>
        public static readonly FieldElement MinJunctures = new FieldElement(38, "Min Junctures");

        /// <summary>
        ///     The maximum number of junctures in a maze within the maze population.
        /// </summary>
        public static readonly FieldElement MaxJunctures = new FieldElement(39, "Max Junctures");

        /// <summary>
        ///     The mean number of junctures among mazes within the maze population.
        /// </summary>
        public static readonly FieldElement MeanJunctures = new FieldElement(40, "Mean Junctures");

        /// <summary>
        ///     The minimum number of openings facing the trajectory in a maze within the maze population.
        /// </summary>
        public static readonly FieldElement MinTrajectoryFacingOpenings =
            new FieldElement(41, "Min Trajectory Facing Openings");

        /// <summary>
        ///     The maximum number of openings facing the trajectory in a maze within the maze population.
        /// </summary>
        public static readonly FieldElement MaxTrajectoryFacingOpenings =
            new FieldElement(42, "Max Trajectory Facing Openings");

        /// <summary>
        ///     The mean number of openings facing the trajectory among mazes within the maze population.
        /// </summary>
        public static readonly FieldElement MeanTrajectoryFacingOpenings =
            new FieldElement(43, "Mean Trajectory Facing Openings");

        /// <summary>
        ///     The minimum height of a maze within the maze population.
        /// </summary>
        public static readonly FieldElement MinHeight = new FieldElement(44, "Min Height");

        /// <summary>
        ///     The maximum height of a maze within the maze population.
        /// </summary>
        public static readonly FieldElement MaxHeight = new FieldElement(45, "Max Height");

        /// <summary>
        ///     The mean height among mazes within the maze population.
        /// </summary>
        public static readonly FieldElement MeanHeight = new FieldElement(46, "Mean Height");

        /// <summary>
        ///     The minimum width of a maze within the maze population.
        /// </summary>
        public static readonly FieldElement MinWidth = new FieldElement(47, "Min Width");

        /// <summary>
        ///     The maximum width of a maze within the maze population.
        /// </summary>
        public static readonly FieldElement MaxWidth = new FieldElement(48, "Max Width");

        /// <summary>
        ///     The mean width among mazes within the maze population.
        /// </summary>
        public static readonly FieldElement MeanWidth = new FieldElement(49, "Mean Width");

        /// <summary>
        ///     Pre-constructs an evolution log field enable map with all of the fields enabled by default.
        /// </summary>
        /// <returns>Evolution log field enable map with all fields enabled.</returns>
        public static Dictionary<FieldElement, bool> PopulateEvolutionFieldElementsEnableMap()
        {
            return new Dictionary<FieldElement, bool>
            {
                {Generation, true},
                {RunPhase, true},
                {SpecieCount, true},
                {AsexualOffspringCount, true},
                {SexualOffspringCount, true},
                {InterspeciesOffspringCount, true},
                {TotalOffspringCount, true},
                {PopulationSize, true},
                {MinimalCriteriaThreshold, true},
                {MinimalCriteriaPointX, true},
                {MinimalCriteriaPointY, true},
                {MaxFitness, true},
                {MeanFitness, true},
                {MeanSpecieChampFitness, true},
                {MinComplexity, true},
                {MaxComplexity, true},
                {MeanComplexity, true},
                {MinSpecieSize, true},
                {MaxSpecieSize, true},
                {TotalEvaluations, true},
                {EvaluationsPerSecond, true},
                {ChampGenomeGenomeId, true},
                {ChampGenomeFitness, true},
                {ChampGenomeBirthGeneration, true},
                {ChampGenomeConnectionGeneCount, true},
                {ChampGenomeNeuronGeneCount, true},
                {ChampGenomeTotalGeneCount, true},
                {ChampGenomeEvaluationCount, true},
                {ChampGenomeBehaviorX, true},
                {ChampGenomeBehaviorY, true},
                {ChampGenomeDistanceToTarget, true},
                {ChampGenomeXml, true},
                {MinWalls, true},
                {MaxWalls, true},
                {MeanWalls, true},
                {MinWaypoints, true},
                {MaxWaypoints, true},
                {MeanWaypoints, true},
                {MinJunctures, true},
                {MaxJunctures, true},
                {MeanJunctures, true},
                {MinTrajectoryFacingOpenings, true},
                {MaxTrajectoryFacingOpenings, true},
                {MeanTrajectoryFacingOpenings, true},
                {MinHeight, true},
                {MaxHeight, true},
                {MeanHeight, true},
                {MinWidth, true},
                {MaxWidth, true},
                {MeanWidth, true}
            };
        }
    }

    /// <summary>
    ///     Fields capturing data related to individual organism evaluations.
    /// </summary>
    public static class EvaluationFieldElements
    {
        /// <summary>
        ///     The number of elements in this log file/table.
        /// </summary>
        public static readonly int NumFieldElements = 9;

        /// <summary>
        ///     The generation of the observation.
        /// </summary>
        public static readonly FieldElement Generation = new FieldElement(0, "Generation");

        /// <summary>
        ///     The number of evaluations that were executed at the time of the observation.
        /// </summary>
        public static readonly FieldElement EvaluationCount = new FieldElement(1, "Evaluation Count");

        /// <summary>
        ///     The run phase (initialization or primary) at the time of the observation.
        /// </summary>
        public static readonly FieldElement RunPhase = new FieldElement(3, "Run Phase");

        /// <summary>
        ///     Whether or not the organism was considered viable (i.e. satisfied some objective/non-objective criterion).
        /// </summary>
        public static readonly FieldElement IsViable = new FieldElement(4, "Is Viable");

        /// <summary>
        ///     Whether or not the experiment stop condition was satisfied.
        /// </summary>
        public static readonly FieldElement StopConditionSatisfied = new FieldElement(5, "Stop Condition Satisfied");

        /// <summary>
        ///     The distance to the objective location.
        /// </summary>
        public static readonly FieldElement DistanceToTarget = new FieldElement(6, "Distance to Target");

        /// <summary>
        ///     The X position of the organism in euclidean space.
        /// </summary>
        public static readonly FieldElement AgentXLocation = new FieldElement(7, "Agent X Location");

        /// <summary>
        ///     The Y position of the organism in euclidean space.
        /// </summary>
        public static readonly FieldElement AgentYLocation = new FieldElement(8, "Agent Y Location");

        /// <summary>
        ///     Pre-constructs an evaluation log field enable map with all of the fields enabled by default.
        /// </summary>
        /// <returns>Evaluation log field enable map with all fields enabled.</returns>
        public static Dictionary<FieldElement, bool> PopulateEvaluationFieldElementsEnableMap()
        {
            return new Dictionary<FieldElement, bool>
            {
                {Generation, true},
                {EvaluationCount, true},
                {RunPhase, true},
                {IsViable, true},
                {StopConditionSatisfied, true},
                {DistanceToTarget, true},
                {AgentXLocation, true},
                {AgentYLocation, true}
            };
        }
    }

    /// <summary>
    ///     Fields capturing the extant genomes (denoted by their ID) during a point in the run, given by the run phase and
    ///     generation..
    /// </summary>
    public static class PopulationFieldElements
    {
        /// <summary>
        ///     The number of elements in this log file/table.
        /// </summary>
        public static readonly int NumFieldElements = 4;

        /// <summary>
        ///     The run phase (i.e. initialization or primary) during which the given observation executed.
        /// </summary>
        public static readonly FieldElement RunPhase = new FieldElement(0, "Run Phase");

        /// <summary>
        ///     The generation in which the given population is extant.
        /// </summary>
        public static readonly FieldElement Generation = new FieldElement(1, "Generation");

        /// <summary>
        ///     The ID of the genome definition being logged.
        /// </summary>
        public static readonly FieldElement GenomeId = new FieldElement(2, "Genome ID");

        /// <summary>
        ///     The unique identifier for the species of which the genome is a member.
        /// </summary>
        public static readonly FieldElement SpecieId = new FieldElement(3, "Specie ID");

        /// <summary>
        ///     Pre-constructs an evaluation log field enable map with all of the fields enabled by default.
        /// </summary>
        /// <returns>Evaluation log field enable map with all fields enabled.</returns>
        public static Dictionary<FieldElement, bool> PopulatePopulationFieldElementsEnableMap()
        {
            return new Dictionary<FieldElement, bool>
            {
                {RunPhase, true},
                {Generation, true},
                {GenomeId, true},
                {SpecieId, true}
            };
        }
    }

    /// <summary>
    ///     Fields capturing the XML definition of genomes throughout the course of a run.
    /// </summary>
    public static class GenomeFieldElements
    {
        /// <summary>
        ///     The number of elements in this log file/table.
        /// </summary>
        public static readonly int NumFieldElements = 3;

        /// <summary>
        ///     The run phase (i.e. initialization or primary) during which the given observation executed.
        /// </summary>
        public static readonly FieldElement RunPhase = new FieldElement(0, "Run Phase");

        /// <summary>
        ///     The ID of the genome definition being logged.
        /// </summary>
        public static readonly FieldElement GenomeId = new FieldElement(1, "Genome ID");

        /// <summary>
        ///     The XML definition of the genome.
        /// </summary>
        public static readonly FieldElement GenomeXml = new FieldElement(2, "Genome XML");

        /// <summary>
        ///     Pre-constructs an evaluation log field enable map with all of the fields enabled by default.
        /// </summary>
        /// <returns>Evaluation log field enable map with all fields enabled.</returns>
        public static Dictionary<FieldElement, bool> PopulateGenomeFieldElementsEnableMap()
        {
            return new Dictionary<FieldElement, bool>
            {
                {RunPhase, true},
                {GenomeId, true},
                {GenomeXml, true}
            };
        }
    }

    /// <summary>
    ///     Fields capturing the resource usage of mazes throughout the course of a run.
    /// </summary>
    public static class ResourceUsageFieldElements
    {
        /// <summary>
        ///     The number of elements in this log file/table.
        /// </summary>
        public static readonly int NumFieldElements = 3;

        /// <summary>
        ///     The generation at which the resource usage was recorded.
        /// </summary>
        public static readonly FieldElement Generation = new FieldElement(0, "Generation");

        /// <summary>
        ///     The ID of the genome whose usage is being logged.
        /// </summary>
        public static readonly FieldElement GenomeId = new FieldElement(1, "Genome ID");

        /// <summary>
        ///     The resource usage count of the given genome.
        /// </summary>
        public static readonly FieldElement UsageCount = new FieldElement(2, "Usage Count");

        /// <summary>
        ///     Pre-constructs a resource usage log field enable map with all of the fields enabled by default.
        /// </summary>
        /// <returns>Resource usage log field enable map with all fields enabled.</returns>
        public static Dictionary<FieldElement, bool> PopulateResourceUsageFieldElementsEnableMap()
        {
            return new Dictionary<FieldElement, bool>
            {
                {Generation, true},
                {GenomeId, true},
                {UsageCount, true}
            };
        }
    }

    /// <summary>
    ///     Encapsulates the position and name of a field within an experiment data log.
    /// </summary>
    public class FieldElement
    {
        /// <summary>
        ///     FieldElement constructor.
        /// </summary>
        /// <param name="position">The absolute position of the field within the log file/table.</param>
        /// <param name="friendlyName">The name of the field.</param>
        public FieldElement(int position, string friendlyName)
        {
            Position = position;
            FriendlyName = friendlyName;
        }

        /// <summary>
        ///     The absolute position of the field within the log file/table.
        /// </summary>
        public int Position { get; }

        /// <summary>
        ///     The name of the field.
        /// </summary>
        public string FriendlyName { get; }
    }
}