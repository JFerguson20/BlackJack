using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack.Net
{
    /// <summary>
    /// A layer holds the neurons in the layer as well as the values, graidents and weights for each neuron.
    /// Layer is called from net in training and testing then further calls each neuron to get the results.
    /// </summary>
    [Serializable()]
    class Layer
    {
        private Neuron[] Neurons;
        public float[] values { get; set; }
        public float[] gradients { get; set; }
        public float[][] weights { get; set; } //[num neuron][num weight for prev neuron] 
        private bool isOutput;
        
        /// <summary>
        /// Creates a either a hidden or output layer 
        /// </summary>
        /// <param name="size">Number of neurons in layer</param>
        /// <param name="prevSize">Number of neurons (or inputs) in layer before this one</param>
        /// <param name="isOutput">Is it an output layer</param>
        public Layer(int size, int prevSize,  bool isOutput = false)
        {
            Neurons = new Neuron[size];
            values = new float[size];
            gradients = new float[size];
            this.isOutput = isOutput;
            weights = new float[prevSize][];
            for(int i = 0; i < size; i++)
            {
                Neurons[i] = new Neuron(prevSize, this.isOutput);
            }

            for(int j = 0; j < prevSize; j++)
            {
                weights[j] = new float[size];
            }
            updateWeightsT(); //init our layer weights;
        }

        /// <summary>
        /// Makes a forward pass for each neuron in the layer.
        /// Also updates the layers value array with each neurons value. 
        /// </summary>
        /// <param name="prevValues">Values of layer in layear before the current one</param>
        public void forward(float[] prevValues)
        {
            int i = 0;
            foreach (Neuron n in Neurons)
            {
                n.forward(prevValues);
                values[i] = n.value;
                i++;
            }
        }

        /// <summary>
        /// Calculates the gradients for each neuron in the layer
        /// </summary>
        /// <param name="goals">The expected output(s) for this layer.</param>
        /// <param name="prevWeights">Only used in the hidden layers. It is the weights of the layer after the current one</param>
        public void backward(float[] goals, float[][] prevWeights = null)
        {
            int i = 0;
            foreach(Neuron n in Neurons)
            {
                if(isOutput)
                {
                    n.backward(goals[i]);
                    gradients[i] = n.gradient;
                }
                else
                {
                    double sigmaHidden = n.value;
                    double gradient = 0.0;
 
                    for(int g = 0; g < goals.Length; g++)
                    {
                        gradient += sigmaHidden * (1 - sigmaHidden) * goals[g] * prevWeights[i][g];
                    }
                    //n.backward(goals[g], prevWeights[i]);
                    gradients[i] = (float)gradient;
                }
                
                i++;
            }
        }

        /// <summary>
        /// Updates the weights of each neuron in the layer based upon the gradient that was calculated
        /// </summary>
        public void updateWeights()
        {
            for (int i = 0; i < Neurons.Length; i++)
            {
                Neurons[i].updateWeights(gradients[i]);
            }
            updateWeightsT();
        }

        /// <summary>
        /// Updates the layers weights matrix.
        /// Gets the weights from each neuron and transposes.
        /// Must be called after each neruons weights are updated.
        /// </summary>
        private void updateWeightsT()
        {
            for (int i = 0; i < Neurons.Length; i++)
            {
                float[] nWeights = Neurons[i].weights;
                for (int j = 0; j < nWeights.Length; j++)
                {
                    this.weights[j][i] = nWeights[j];
                }
            }
        }
    }
}
