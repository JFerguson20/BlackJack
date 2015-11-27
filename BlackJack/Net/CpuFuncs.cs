using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack.Net
{
    /// <summary>
    /// Class of commonly used functions implemented for the cpu
    /// </summary>
    public static class CpuFuncs
    {
        /// <summary>
        /// Calculates the inner product of two float arrays.
        /// Assumes arrays are the same size.
        /// </summary>
        /// <param name="x">Vector 1</param>
        /// <param name="y">Vector 2</param>
        /// <returns>The inner product of the two arrays</returns>
        public static float inner_product(float[] x, float[] y)
        {
            float sum = 0;
            for (int i = 0; i < x.Length; i++)
            {
                sum += x[i] * y[i];
            }
            return sum;
        }

        /// <summary>
        /// Sigmoid activation function
        /// </summary>
        /// <param name="x">raw value before activation</param>
        /// <returns>value after sigmoid activation</returns>
        public static float sigmoid(float x)
        {
            return 1.0f / (1.0f + (float)System.Math.Exp(-x));
        }

        /// <summary>
        /// Calculates the derivitive of the sigmoid function for one value
        /// </summary>
        /// <param name="x">Value to calculate derivitive of</param>
        /// <returns></returns>
        public static float sigmoid_prime(float x)
        {
            return (1.0f - x) * x;
        }
    }
}
