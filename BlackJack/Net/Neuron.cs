using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack.Net
{
    /// <summary>
    /// The neuron class implements both the forward and backward stages that are called.
    /// </summary>
    class Neuron
    {
        public float value { get; set; }
        public float gradient { get; set; }
        public float[] weights { get; set; }

        private float[] inputs;
        private float bias;
        private float[] deltaWeights;
        private float deltaBias;
        private bool isOutput;

        /// <summary>
        /// Creates a neuron that lives in a layer
        /// </summary>
        /// <param name="numInputs">Number of neurons (or inputs) from the previous layer</param>
        /// <param name="isOutput">Is this neuron an output or hidden neuron</param>
        public Neuron(int numInputs, bool isOutput)
        {
            this.isOutput = isOutput;
            inputs = new float[numInputs];
            weights = new float[numInputs];
            deltaWeights = new float[numInputs];
            bias = 0.0f;
            deltaBias = 0.0f;
            gradient = 0.0f;
            randomizeWeights();
        }

        /// <summary>
        /// Calculates the value for this neuron
        /// </summary>
        /// <param name="prevVals">The values from the layer before the current one</param>
        public void forward(float[] prevVals)
        {
            inputs = (float[])prevVals.Clone();
            value = CpuFuncs.sigmoid(CpuFuncs.inner_product(weights, inputs));

            if(isOutput) //linear if output
                value = CpuFuncs.inner_product(weights, inputs);
        }

        public void backward(float goal, float[] nextWeight = null)
        {
            calcGradient(goal, nextWeight);
            //updateWeights();
        }

        /// <summary>
        /// Calculates the gradient for this neuron.
        /// This is only used in the backwards training phase.
        /// </summary>
        /// <param name="goal">Expected value of this neuron (gradient or goal value)</param>
        /// <param name="nextWeight">Null if output neuron, otherwise equals to weights to this neuron from layer after it.</param>
        public void calcGradient(float goal, float[] nextWeight)
        {
            float derivitive = CpuFuncs.sigmoid_prime(value);
            if(isOutput)
            {
                gradient = (goal - value);
            }
            else
            {
                float sum = 0.0f;
                for (int i = 0; i < nextWeight.Length; i++)
                {
                    sum += goal * nextWeight[i];
                }

                gradient = derivitive * sum;
            }
        }

        /// <summary>
        /// Update each weight to the previous layer based upon the gradient
        /// </summary>
        public void updateWeights(float grad)
        {
            for(int i = 0; i < weights.Length; i++)
            {
                float delta = NetCore.learningRate * grad * inputs[i];
                delta += (NetCore.momentum * deltaWeights[i]);
                weights[i] += delta;
                deltaWeights[i] = delta;

            }
            float deltaB = NetCore.learningRate * grad;
            bias += deltaB + (NetCore.momentum * deltaBias);
            deltaBias = deltaB; 
        }

        /// <summary>
        /// Randomly assign a value between -0.1 and 0.1 to each weight
        /// </summary>
        private void randomizeWeights()
        {
            int sign;
            for(int i = 0; i < weights.Length; i++)
            {
                sign = 1;
                float rand = (float)NetCore.r.NextDouble() / 10.0f;
                if (NetCore.r.NextDouble() >= 0.5) sign = -1;
                weights[i] = rand * sign;
                deltaWeights[i] = 0.0f;
            }
            sign = 1;
            if (NetCore.r.NextDouble() >= 0.5) sign = -1;
            bias = ((float)NetCore.r.NextDouble() / 10.0f) * sign;
        }
    }
}
