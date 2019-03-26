using CaptureVision.BLL.Services;
using CaptureVision.DAL.Models;
using CaptureVision.Vision;
using Microsoft.Data.DataView;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace CaptureVision.NN
{
    public class NeuralNetwork
    {
        private List<Tuple<string, Bitmap>> _processedImage = new List<Tuple<string, Bitmap>>();
        public static List<TrainingDataForSymbol> TrainingDataForSymbol = new List<TrainingDataForSymbol>();
        private static readonly object _syncRoot = new Object();

        public static void RunProcessing()
        {
            new NeuralNetwork().RunAsync();
        }

        private void RunAsync()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            lock (_syncRoot)
            {
                List<Capture> CaptchasFromDB = Task.Factory.StartNew(() =>
                {
                    return new Queries().GetPicturesFromDB();
                }).Result;

                CaptchasFromDB.ForEach(t =>
                {
                    _processedImage.Add(new Tuple<string, Bitmap>(t.Result, DataProcessing.GetMask(t.CaptureImage)));
                });

                new ParallelClass<List<Tuple<string, Bitmap>>, string, Bitmap>(_processedImage).Run();
            }

            timer.Stop();
            TimeSpan ts = timer.Elapsed;
            Console.WriteLine(ts.ToString());

            MLContext mlContext = new MLContext(seed: 0);
            IDataView data = mlContext.Data.LoadFromEnumerable<TrainingDataForSymbol>(TrainingDataForSymbol);
            var model = NeuralNetwork.Train(mlContext, data);

            foreach (var item in TrainingDataForSymbol)
            {
                var _predict = EvaluateSinglePrediction(item, mlContext, model);
            }

            // var predict = mlContext.Model.CreatePredictionEngine<TrainingDataForSymbol, Prediction>(model);
            //IDataView trainingDataView = mlContext.CreateStreamingDataView(_processedBinaryImage.Cast<List<Tuple<string, string>>>());
            //IDataView testDataView = mlContext.CreateStreamingDataView(_processedImage.Cast<List<Tuple<string, string>>>());
        }

        public static ITransformer Train(MLContext mlContext, IDataView dataView)
        {
            return mlContext.Transforms.CopyColumns("OutputVector", "Label")
                  .Append(mlContext.Transforms.Concatenate("Features", "InputVector"))
                  //.Append(mlContext.Transforms.Text.ProduceWordBags("OutputVector", "InputVector")).Fit(dataView);
                  .Append(mlContext.Transforms.Conversion.MapKeyToValue()).Fit(dataView);

        }

        private static IEnumerable<string> EvaluateSinglePrediction(TrainingDataForSymbol currentData, MLContext mlContext, ITransformer model)
        {
            var predictionFunction = model.CreatePredictionEngine<TrainingDataForSymbol, Prediction>(mlContext);
            var prediction = predictionFunction.Predict(currentData);

            yield return prediction.Output;
        }
    }

    public sealed class ParallelClass<T, K, U> where T : List<Tuple<K, U>>
    {
        private static T _processedImage { get; set; }
        private static List<TrainingData> _trainingData = new List<TrainingData>();

        public ParallelClass(T processedImage)
        {
            _processedImage = processedImage;
        }

        public void Run()
        {
            Parallel.ForEach(_processedImage, ParallelCycleForTrainingData);
            Parallel.ForEach(_trainingData, ParallelCycleForTrainingSymbol);
        }

        private static void ParallelCycleForTrainingData(Tuple<K, U> item)
        {
            _trainingData.Add(new TrainingData() { InputVector = DataProcessing.ImageToBinary(item.Item2 as Bitmap),
                                                   OutputVector = item.Item1 as String });
        }

        private static void ParallelCycleForTrainingSymbol(TrainingData item)
        {
            foreach (Tuple<string, string> tuple in DataProcessing.BinaryToSymbol(item.InputVector, item.OutputVector))
                     NeuralNetwork.TrainingDataForSymbol.Add(new TrainingDataForSymbol() { InputVector = tuple.Item1,
                                                                                           OutputVector = tuple.Item2, });
        }
    }
}
