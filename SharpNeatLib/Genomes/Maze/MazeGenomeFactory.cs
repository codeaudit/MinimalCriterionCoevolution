﻿#region

using System;
using System.Collections.Generic;
using SharpNeat.Core;
using SharpNeat.Utility;

#endregion

namespace SharpNeat.Genomes.Maze
{
    public class MazeGenomeFactory : IGenomeFactory<MazeGenome>
    {
        #region Constructors

        public MazeGenomeFactory()
        {
            MazeGenomeParameters = new MazeGenomeParameters();
        }

        #endregion

        #region Maze Genome Factory Methods

        public MazeGenome CreateGenome(uint id, uint birthGeneration)
        {
            return new MazeGenome(this, id, birthGeneration);
        }

        public MazeGenome CreateGenomeCopy(MazeGenome copyFrom, uint id, uint birthGeneration)
        {
            return new MazeGenome(copyFrom, id, birthGeneration);
        }
        
        #endregion

        #region Interface Properties

        public UInt32IdGenerator GenomeIdGenerator { get; private set; }

        public int SearchMode
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #endregion

        #region Maze Genome Factory Properties

        public readonly FastRandom Rng = new FastRandom();

        public MazeGenomeParameters MazeGenomeParameters { get; }

        #endregion

        #region Interface Methods

        public List<MazeGenome> CreateGenomeList(int length, uint birthGeneration)
        {
            List<MazeGenome> genomeList = new List<MazeGenome>(length);

            for (int i = 0; i < length; i++)
            {
                genomeList.Add(CreateGenome(birthGeneration));
            }

            return genomeList;
        }

        public List<MazeGenome> CreateGenomeList(int length, uint birthGeneration, MazeGenome seedGenome)
        {
            List<MazeGenome> genomeList = new List<MazeGenome>(length);

            // Add an exact copy of the seed to the list.
            MazeGenome newGenome = CreateGenomeCopy(seedGenome, GenomeIdGenerator.NextId, birthGeneration);
            genomeList.Add(newGenome);

            // For the remainder we create mutated offspring from the seed.
            for (int i = 1; i < length; i++)
            {
                genomeList.Add(seedGenome.CreateOffspring(birthGeneration));
            }
            return genomeList;
        }

        public List<MazeGenome> CreateGenomeList(int length, uint birthGeneration, List<MazeGenome> seedGenomeList)
        {
            if (seedGenomeList.Count == 0)
            {
                throw new SharpNeatException("CreateGenomeList() requires at least on seed genome in seedGenomeList.");
            }

            // Create a copy of the list so that we can shuffle the items without modifying the original list.
            seedGenomeList = new List<MazeGenome>(seedGenomeList);
            Utilities.Shuffle(seedGenomeList, Rng);

            // Make exact copies of seed genomes and insert them into our new genome list.
            List<MazeGenome> genomeList = new List<MazeGenome>(length);
            int idx = 0;
            int seedCount = seedGenomeList.Count;
            for (int seedIdx = 0; idx < length && seedIdx < seedCount; idx++, seedIdx++)
            {
                // Add an exact copy of the seed to the list.
                MazeGenome newGenome = CreateGenomeCopy(seedGenomeList[seedIdx], GenomeIdGenerator.NextId,
                    birthGeneration);
                genomeList.Add(newGenome);
            }

            // Keep spawning offspring from seed genomes until we have the required number of genomes.
            for (; idx < length;)
            {
                for (int seedIdx = 0; idx < length && seedIdx < seedCount; idx++, seedIdx++)
                {
                    genomeList.Add(seedGenomeList[seedIdx].CreateOffspring(birthGeneration));
                }
            }
            return genomeList;
        }

        public MazeGenome CreateGenome(uint birthGeneration)
        {
            return CreateGenome(GenomeIdGenerator.NextId, birthGeneration);
        }

        public bool CheckGenomeType(MazeGenome genome)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}