//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ExperimentEntities
{
    using System;
    using System.Collections.Generic;
    
    public partial class ExperimentDictionary
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ExperimentDictionary()
        {
            this.NoveltyExperimentOrganismStateDatas = new HashSet<NoveltyExperimentOrganismStateData>();
            this.NoveltyExperimentEvaluationDatas = new HashSet<NoveltyExperimentEvaluationData>();
            this.MCSExperimentEvaluationDatas = new HashSet<MCSExperimentEvaluationData>();
            this.MCSExperimentOrganismStateDatas = new HashSet<MCSExperimentOrganismStateData>();
        }
    
        public int ExperimentDictionaryID { get; set; }
        public string ExperimentName { get; set; }
        public string ConfigurationFile { get; set; }
        public int MaxEvaluations { get; set; }
        public int MaxTimesteps { get; set; }
        public int MinSuccessDistance { get; set; }
        public int MaxDistanceToTarget { get; set; }
        public string ExperimentDomainName { get; set; }
        public Nullable<int> Initialization_OffspringBatchSize { get; set; }
        public Nullable<int> Initialization_PopulationEvaluationFrequency { get; set; }
        public string Initialization_ComplexityRegulationStrategy { get; set; }
        public Nullable<int> Initialization_ComplexityThreshold { get; set; }
        public string Initialization_SelectionAlgorithmName { get; set; }
        public Nullable<int> Primary_OffspringBatchSize { get; set; }
        public Nullable<int> Primary_PopulationEvaluationFrequency { get; set; }
        public string Primary_ComplexityRegulationStrategy { get; set; }
        public Nullable<int> Primary_ComplexityThreshold { get; set; }
        public string Primary_SelectionAlgorithmName { get; set; }
        public string Initialization_SearchAlgorithmName { get; set; }
        public string Initialization_BehaviorCharacterizationName { get; set; }
        public Nullable<int> Initialization_NoveltySearch_NearestNeighbors { get; set; }
        public Nullable<int> Initialization_NoveltySearch_ArchiveAdditionThreshold { get; set; }
        public Nullable<double> Initialization_NoveltySearch_ArchiveThresholdDecreaseMultiplier { get; set; }
        public Nullable<double> Initialization_NoveltySearch_ArchiveThresholdIncreaseMultiplier { get; set; }
        public Nullable<int> Initialization_NoveltySearch_MaxGenerationsWithArchiveAddition { get; set; }
        public Nullable<int> Initialization_NoveltySearch_MaxGenerationsWithoutArchiveAddition { get; set; }
        public Nullable<double> Initialization_MCS_MinimalCriteriaThreshold { get; set; }
        public Nullable<double> Initialization_MCS_MinimalCriteriaStartX { get; set; }
        public Nullable<double> Initialization_MCS_MinimalCriteriaStartY { get; set; }
        public string Initialization_MCS_MinimalCriteriaName { get; set; }
        public string Primary_SearchAlgorithmName { get; set; }
        public string Primary_BehaviorCharacterizationName { get; set; }
        public Nullable<int> Primary_NoveltySearch_NearestNeighbors { get; set; }
        public Nullable<int> Primary_NoveltySearch_ArchiveAdditionThreshold { get; set; }
        public Nullable<double> Primary_NoveltySearch_ArchiveThresholdDecreaseMultiplier { get; set; }
        public Nullable<double> Primary_NoveltySearch_ArchiveThresholdIncreaseMultiplier { get; set; }
        public Nullable<int> Primary_NoveltySearch_MaxGenerationsWithArchiveAddition { get; set; }
        public Nullable<int> Primary_NoveltySearch_MaxGenerationsWithoutArchiveAddition { get; set; }
        public Nullable<double> Primary_MCS_MinimalCriteriaThreshold { get; set; }
        public Nullable<double> Primary_MCS_MinimalCriteriaStartX { get; set; }
        public Nullable<double> Primary_MCS_MinimalCriteriaStartY { get; set; }
        public string Primary_MCS_MinimalCriteriaName { get; set; }
        public bool SerializeGenomeToXml { get; set; }
        public Nullable<int> Initialization_MCS_BridgingMagnitude { get; set; }
        public Nullable<int> Primary_MCS_BridgingMagnitude { get; set; }
        public Nullable<int> Initialization_PopulationSize { get; set; }
        public Nullable<int> Initialization_NumSpecies { get; set; }
        public Nullable<double> Initialization_ElitismProportion { get; set; }
        public Nullable<double> Initialization_SelectionProportion { get; set; }
        public Nullable<double> Initialization_AsexualProbability { get; set; }
        public Nullable<double> Initialization_CrossoverProbability { get; set; }
        public Nullable<double> Initialization_InterspeciesMatingProbability { get; set; }
        public Nullable<double> Initialization_MutateConnectionWeightsProbability { get; set; }
        public Nullable<double> Initialization_MutateAddNeuronProbability { get; set; }
        public Nullable<double> Initialization_MutateAddConnectionProbability { get; set; }
        public Nullable<double> Initialization_MutateDeleteConnectionProbability { get; set; }
        public Nullable<double> Initialization_ConnectionProportion { get; set; }
        public Nullable<int> Initialization_ConnectionWeightRange { get; set; }
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
        public Nullable<int> Initialization_MCS_BridgingApplications { get; set; }
        public Nullable<int> Primary_MCS_BridgingApplications { get; set; }
        public int MaxRestarts { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NoveltyExperimentOrganismStateData> NoveltyExperimentOrganismStateDatas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NoveltyExperimentEvaluationData> NoveltyExperimentEvaluationDatas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MCSExperimentEvaluationData> MCSExperimentEvaluationDatas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MCSExperimentOrganismStateData> MCSExperimentOrganismStateDatas { get; set; }
    }
}
