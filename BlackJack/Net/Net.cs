using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack.Net
{
    /// <summary>
    /// The Neural Network Class.
    /// </summary>
    public class Net
    {
        Layer[] hiddenLayers;
        Layer outputLayer;
        private int numHidden;

        /// <summary>
        /// Creates a net.
        /// </summary>
        /// <param name="data">The data class for this problem</param>
        /// <param name="hiddenSizes">Array of sizes of hidden layers. Example [10, 20, 10]</param>
        public Net(int numInputs, int[] hiddenSizes, int numOutputs)
        {
            numInputs += 2;// add the actions
            numHidden = hiddenSizes.Length;
            hiddenLayers = new Layer[numHidden];
            int prevSize = numInputs;
            for(int i = 0; i < hiddenLayers.Length; i++)
            {
                int size = hiddenSizes[i];
                hiddenLayers[i] = new Layer(size, prevSize);
                prevSize = size;
            }
            outputLayer = new Layer(numOutputs, prevSize, true);
        }

        /// <summary>
        /// Trains the net for for the specified number of epochs.
        /// </summary>
        /// <param name="epochs">number of epochs to train for</param>
        /// <returns>array of rmse values for each epochs</returns>
        /*
        public float[] train(int epochs)
        {
            float[] rmseArr = new float[epochs];
            int right = 0;
            int wrong = 0;
           
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                foreach(float[] input in data.inputs)
                {
                    forward(input);
                    backward();
                }
                float errSum = 0.0f;
                int count = 0;
                for(int i = 0; i < data.outputs.Length; i++)
                {
                    for (int j = 0; j < data.numOutputs; j ++)
                    {
                        errSum += (float)System.Math.Pow(data.goalValues[i][j] - data.outputs[i][j], 2);

                        int goal = (int)data.goalValues[i][j];
                        
                        if(goal == 1 && data.outputs[i][j] > .50)
                        {
                            right++;
                        }else if(goal == 0 && data.outputs[i][j] < .50)
                        {
                            right++;
                        }
                        else
                        {
                            wrong++;
                        }

                        count++;
                    }
                }

                Console.WriteLine(epoch + ": Right " + right + ", Wrong " + wrong);
                right = 0;
                wrong = 0;
                float rmse = (float)System.Math.Sqrt((1.0 / (2.0 * count)) * errSum);
                rmseArr[epoch] = rmse;
            }

            return rmseArr;
        }
        */

        /// <summary>
        /// Make a forward pass of the net and set the results in data.outputs.
        /// </summary>
        /// <param name="input">An array of the input values</param>
        public float[] forward(float[] input)
        {
            hiddenLayers[0].forward(input);

            for (int i = 1; i < hiddenLayers.Length; i++)
            {
                hiddenLayers[i].forward(hiddenLayers[i - 1].values);
            }
            outputLayer.forward(hiddenLayers[numHidden - 1].values);
            return outputLayer.values;
        }

        /// <summary>
        /// Calculates the Gradients and updates teh weights in each layer.
        /// Is called right after forward in the training phase.
        /// </summary>
        public void backward(float [] goalValues)
        {
            outputLayer.backward(goalValues);

            
            float[] gradients = outputLayer.gradients;

            Layer prevLayer = outputLayer;
            for(int i = numHidden - 1; i >= 0; i--)
            {
                hiddenLayers[i].backward(prevLayer.gradients, prevLayer.weights);
                prevLayer = hiddenLayers[i];
            }

            //update weights in Layers
            foreach(var hl in hiddenLayers)
            {
                hl.updateWeights();
            }
            outputLayer.updateWeights();

        }
    }
}
