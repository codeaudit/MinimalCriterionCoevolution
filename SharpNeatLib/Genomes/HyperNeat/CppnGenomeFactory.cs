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

using SharpNeat.Core;
using SharpNeat.Genomes.Neat;
using SharpNeat.Network;
using SharpNeat.Utility;

namespace SharpNeat.Genomes.HyperNeat
{
    /// <summary>
    ///     A sub-class of NeatGenomeFactory for creating CPPN genomes.
    /// </summary>
    public class CppnGenomeFactory : NeatGenomeFactory
    {
        #region Public Methods [NeatGenome Specific / CPPN Overrides]

        /// <summary>
        ///     Override that randomly assigns activation functions to neuron's from an activation function library
        ///     based on each library item's selection probability.
        /// </summary>
        public override NeuronGene CreateNeuronGene(uint innovationId, NodeType neuronType)
        {
            int activationFnId;
            switch (neuronType)
            {
                case NodeType.Bias:
                case NodeType.Input:
                case NodeType.Output:
                {
                    // Use the ID of the first function. By convention this will be the Linear function but in actual 
                    // fact bias and input neurons don't use their activation function.
                    activationFnId = _activationFnLibrary.GetFunctionList()[0].Id;
                    break;
                }
                default:
                {
                    activationFnId = _activationFnLibrary.GetRandomFunction(_rng).Id;
                    break;
                }
            }

            return new NeuronGene(innovationId, neuronType, activationFnId);
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructs with default NeatGenomeParameters, ID generators initialized to zero, a default
        ///     IActivationFunctionLibrary and genome validator.
        /// </summary>
        public CppnGenomeFactory(int inputNeuronCount, int outputNeuronCount,
            IGenomeValidator<NeatGenome> genomeValidator = null)
            : base(inputNeuronCount, outputNeuronCount, DefaultActivationFunctionLibrary.CreateLibraryCppn(),
                genomeValidator)
        {
        }

        /// <summary>
        ///     Constructs with default NeatGenomeParameters, ID generators initialized to zero, the provided
        ///     IActivationFunctionLibrary and the genome validator.
        /// </summary>
        public CppnGenomeFactory(int inputNeuronCount, int outputNeuronCount,
            IActivationFunctionLibrary activationFnLibrary, IGenomeValidator<NeatGenome> genomeValidator = null)
            : base(inputNeuronCount, outputNeuronCount, activationFnLibrary, genomeValidator)
        {
        }

        /// <summary>
        ///     Constructs with the provided IActivationFunctionLibrary, NeatGenomeParameters and genome validator.
        /// </summary>
        public CppnGenomeFactory(int inputNeuronCount, int outputNeuronCount,
            IActivationFunctionLibrary activationFnLibrary, NeatGenomeParameters neatGenomeParams,
            IGenomeValidator<NeatGenome> genomeValidator = null)
            : base(inputNeuronCount, outputNeuronCount, activationFnLibrary, neatGenomeParams, genomeValidator)
        {
        }

        /// <summary>
        ///     Constructs with the provided IActivationFunctionLibrary, NeatGenomeParameters, ID generators
        ///     and genome validator.
        /// </summary>
        public CppnGenomeFactory(int inputNeuronCount, int outputNeuronCount,
            IActivationFunctionLibrary activationFnLibrary, NeatGenomeParameters neatGenomeParams,
            UInt32IdGenerator genomeIdGenerator, UInt32IdGenerator innovationIdGenerator,
            IGenomeValidator<NeatGenome> genomeValidator = null)
            : base(inputNeuronCount, outputNeuronCount, activationFnLibrary, neatGenomeParams, genomeIdGenerator,
                innovationIdGenerator, genomeValidator)
        {
        }

        /// <summary>
        ///     CppnGenomeFactory copy constructor.
        /// </summary>
        public CppnGenomeFactory(CppnGenomeFactory factory) : this(factory.InputNeuronCount, factory.OutputNeuronCount,
            factory.ActivationFnLibrary, factory.NeatGenomeParameters, factory.GenomeIdGenerator,
            factory.InnovationIdGenerator)
        {
        }

        #endregion
    }
}