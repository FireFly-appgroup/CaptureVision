using CaptureVision.NN;
using System;

namespace CaptureVision
{
    class Program
    {
        static void Main(string[] args)
        {
            //Data.SetNewData();
            NeuralNetwork.RunProcessing();
            Console.ReadKey();
        }
    }
}
