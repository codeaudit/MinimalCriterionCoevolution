﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ExperimentEntities.entities
{
    [Table("ExperimentDictionary_vw")]
    public partial class ExperimentDictionary
    {
        public int ExperimentDictionaryId { get; set; }
        public string ExperimentName { get; set; }
        public string ConfigurationFile { get; set; }

        public int? MaxEvaluations { get; set; }
        public int MaxTimesteps { get; set; }
        public int MinSuccessDistance { get; set; }
        public int? MaxDistanceToTarget { get; set; }
        public int? ResourceLimit { get; set; }
        public string ExperimentDomainName { get; set; }
        public int? Initialization_OffspringBatchSize { get; set; }
        public int? Initialization_PopulationEvaluationFrequency { get; set; }
        public string Initialization_ComplexityRegulationStrategy { get; set; }
        public int? Initialization_ComplexityThreshold { get; set; }
        public string Initialization_SelectionAlgorithmName { get; set; }
        public int? Primary_OffspringBatchSize { get; set; }
        public int? Primary_PopulationEvaluationFrequency { get; set; }
        public string Primary_ComplexityRegulationStrategy { get; set; }
        public int? Primary_ComplexityThreshold { get; set; }
        public string Primary_SelectionAlgorithmName { get; set; }
        public string Initialization_SearchAlgorithmName { get; set; }
        public string Initialization_BehaviorCharacterizationName { get; set; }
        public int? Initialization_NoveltySearch_NearestNeighbors { get; set; }
        public int? Initialization_NoveltySearch_ArchiveAdditionThreshold { get; set; }
        public double? Initialization_NoveltySearch_ArchiveThresholdDecreaseMultiplier { get; set; }
        public double? Initialization_NoveltySearch_ArchiveThresholdIncreaseMultiplier { get; set; }
        public int? Initialization_NoveltySearch_MaxGenerationsWithArchiveAddition { get; set; }
        public int? Initialization_NoveltySearch_MaxGenerationsWithoutArchiveAddition { get; set; }
        public double? Initialization_MCS_MinimalCriteriaThreshold { get; set; }
        public double? Initialization_MCS_MinimalCriteriaStartX { get; set; }
        public double? Initialization_MCS_MinimalCriteriaStartY { get; set; }
        public string Initialization_MCS_MinimalCriteriaName { get; set; }
        public string Primary_SearchAlgorithmName { get; set; }
        public string Primary_BehaviorCharacterizationName { get; set; }
        public int? Primary_NoveltySearch_NearestNeighbors { get; set; }
        public int? Primary_NoveltySearch_ArchiveAdditionThreshold { get; set; }
        public double? Primary_NoveltySearch_ArchiveThresholdDecreaseMultiplier { get; set; }
        public double? Primary_NoveltySearch_ArchiveThresholdIncreaseMultiplier { get; set; }
        public int? Primary_NoveltySearch_MaxGenerationsWithArchiveAddition { get; set; }
        public int? Primary_NoveltySearch_MaxGenerationsWithoutArchiveAddition { get; set; }
        public double? Primary_MCS_MinimalCriteriaThreshold { get; set; }
        public double? Primary_MCS_MinimalCriteriaStartX { get; set; }
        public double? Primary_MCS_MinimalCriteriaStartY { get; set; }
        public string Primary_MCS_MinimalCriteriaName { get; set; }
        public bool SerializeGenomeToXml { get; set; }
        public int? Initialization_MCS_BridgingMagnitude { get; set; }
        public int? Primary_MCS_BridgingMagnitude { get; set; }
        public int? Initialization_PopulationSize { get; set; }
        public int? Initialization_NumSpecies { get; set; }
        public double? Initialization_ElitismProportion { get; set; }
        public double? Initialization_SelectionProportion { get; set; }
        public double? Initialization_AsexualProbability { get; set; }
        public double? Initialization_CrossoverProbability { get; set; }
        public double? Initialization_InterspeciesMatingProbability { get; set; }
        public double? Initialization_MutateConnectionWeightsProbability { get; set; }
        public double? Initialization_MutateAddNeuronProbability { get; set; }
        public double? Initialization_MutateAddConnectionProbability { get; set; }
        public double? Initialization_MutateDeleteConnectionProbability { get; set; }
        public double? Initialization_ConnectionProportion { get; set; }
        public int? Initialization_ConnectionWeightRange { get; set; }
        public int Primary_PopulationSize { get; set; }
        public int Primary_NumSpecies { get; set; }
        public double Primary_ElitismProportion { get; set; }
        public double Primary_SelectionProportion { get; set; }
        public double Primary_AsexualProbability { get; set; }
        public double Primary_CrossoverProbability { get; set; }
        public double Primary_InterspeciesMatingProbability { get; set; }
        public double Primary_MutateConnectionWeightsProbability { get; set; }
        public double Primary_MutateAddNeuronProbability { get; set; }
        public double Primary_MutateAddConnectionProbability { get; set; }
        public double Primary_MutateDeleteConnectionProbability { get; set; }
        public double Primary_ConnectionProportion { get; set; }
        public int Primary_ConnectionWeightRange { get; set; }
        public int? Initialization_MCS_BridgingApplications { get; set; }
        public int? Primary_MCS_BridgingApplications { get; set; }
        public int MaxRestarts { get; set; }
        public int? Initialization_NicheCapacity { get; set; }
        public int? Initialization_NicheGridDensity { get; set; }
        public double? Initialization_ReproductionProportion { get; set; }
        public int? Primary_NicheCapacity { get; set; }
        public int? Primary_NicheGridDensity { get; set; }
        public double? Primary_ReproductionProportion { get; set; }
        public int? PopulationLoggingBatchInterval { get; set; }
        public int Primary_Maze_PopulationSize { get; set; }
        public double? Primary_Maze_MutateWallLocationProbability { get; set; }
        public double? Primary_Maze_MutatePassageLocationProbability { get; set; }
        public double? Primary_Maze_MutateAddWallProbability { get; set; }
        public double? Primary_Maze_PerturbanceMagnitude { get; set; }
        public int? Primary_Maze_MazeHeight { get; set; }
        public int? Primary_Maze_MazeWidth { get; set; }
        public int? Primary_Maze_MazeScaleMultiplier { get; set; }
        public int? Primary_Maze_OffspringBatchSize { get; set; }
        public int? Primary_Maze_PopulationEvaluationFrequency { get; set; }
        public int? Primary_Maze_NicheCapacity { get; set; }
        public int? Primary_Maze_NicheGridDensity { get; set; }
        public double? Primary_Maze_ReproductionProportion { get; set; }
        public string Primary_Maze_ComplexityRegulationStrategy { get; set; }
        public int? Primary_Maze_ComplexityThreshold { get; set; }
        public string Primary_Maze_SelectionAlgorithmName { get; set; }
        public int? Primary_Coevolution_MCS_SuccessMinimalCriteriaThreshold { get; set; }
        public int? Primary_Coevolution_MCS_FailureMinimalCriteriaThreshold { get; set; }
        public int? Primary_MCS_MinimalCriteriaUpdateInterval { get; set; }
        public int? Primary_MCS_MaxCriteriaUpdateCyclesWithoutChange { get; set; }
        public string Primary_Maze_SearchAlgorithmName { get; set; }
        public string Primary_Maze_BehaviorCharacterizationName { get; set; }
        public int? Primary_Maze_NoveltySearch_NearestNeighbors { get; set; }
        public int? Primary_Maze_NoveltySearch_ArchiveAdditionThreshold { get; set; }
        public double? Primary_Maze_NoveltySearch_ArchiveThresholdDecreaseMultiplier { get; set; }
        public double? Primary_Maze_NoveltySearch_ArchiveThresholdIncreaseMultiplier { get; set; }
        public int? Primary_Maze_NoveltySearch_MaxGenerationsWithArchiveAddition { get; set; }
        public int? Primary_Maze_NoveltySearch_MaxGenerationsWithoutArchiveAddition { get; set; }
        public double? Primary_Maze_MCS_MinimalCriteriaThreshold { get; set; }
        public int? Primary_Maze_Coevolution_MCS_SuccessMinimalCriteriaThreshold { get; set; }
        public int? Primary_Maze_Coevolution_MCS_FailureMinimalCriteriaThreshold { get; set; }
        public int? Primary_Maze_MCS_MinimalCriteriaUpdateInterval { get; set; }
        public int? Primary_Maze_MCS_MaxCriteriaUpdateCyclesWithoutChange { get; set; }
        public double? Primary_Maze_MCS_MinimalCriteriaStartX { get; set; }
        public double? Primary_Maze_MCS_MinimalCriteriaStartY { get; set; }
        public string Primary_Maze_MCS_MinimalCriteriaName { get; set; }
        public int? Primary_Maze_MCS_BridgingMagnitude { get; set; }
        public int? Primary_Maze_MCS_BridgingApplications { get; set; }
        public int? MaxGenerations { get; set; }
        public int Primary_Maze_NumSpecies { get; set; }
        public short? Primary_Maze_MinimumWalls { get; set; }
        public bool? Primary_SpecieSizeFixed { get; set; }
        public bool? Primary_Maze_SpecieSizeFixed { get; set; }
        public int NumSeedAgentGenomes { get; set; }
        public int? NumSeedEnvironmentGenomes { get; set; }
        public double? Primary_Maze_MutateDeleteWallProbability { get; set; }
        public double? Primary_Maze_MutatePathWaypointLocationProbability { get; set; }
        public double? Primary_Maze_MutateAddPathWaypointProbability { get; set; }
        public double? Primary_Maze_MutateExpandMazeProbability { get; set; }
        public double? Primary_Maze_VerticalWallBias { get; set; }
        public int? Primary_Maze_QuadrantHeight { get; set; }
        public int? Primary_Maze_QuadrantWidth { get; set; }
        public string Primary_ActivationScheme { get; set; }
        public int? Primary_ActivationIters { get; set; }
        public double? Primary_ActivationDeltaThreshold { get; set; }
    }
}