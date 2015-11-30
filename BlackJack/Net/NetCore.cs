using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack.Net
{
    /// <summary>
    /// Holds our "global variables and settings" for the network
    /// </summary>
    static class NetCore
    {
        public static float learningRate = .2f;
        public static float momentum = .1f;
        public static Random r = new Random();
        public static int mode = CPU;
        public const int CPU = 0;
        public const int GPU = 1;
    }
}
