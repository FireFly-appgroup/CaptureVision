using CaptureVision.BLL.Services;
using CaptureVision.DAL.Models;
using CaptureVision.Vision;
using Microsoft.ML;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;
using System;
using System.Collections.Generic;
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

            //IDataView trainingDataView = mlContext.CreateStreamingDataView<TrainingData>(_trainingData);
            //var model = NeuralNetwork.Train(mlContext, trainingDataView);
            //foreach (var item in _trainingData)
            //{
            //    var _predict = EvaluateSinglePrediction(item, mlContext, model).FirstOrDefault();
            //}




            //IDataView trainingDataView = mlContext.CreateStreamingDataView(_processedBinaryImage.Cast<List<Tuple<string, string>>>());
            //IDataView testDataView = mlContext.CreateStreamingDataView(_processedImage.Cast<List<Tuple<string, string>>>());

            //var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText("Input", "Label");
            //var trainer = mlContext.BinaryClassification.Trainers.FastTree(labelColumn: "Features", featureColumn: "Input");
            //var trainingPipeline = dataProcessPipeline.Append(trainer);
            //ITransformer trainedModel = trainingPipeline.Fit(trainingDataView);

            //var predictions = trainedModel.Transform(testDataView);
            //var metrics = mlContext.BinaryClassification.Evaluate(predictions, "Output", "Score");

            //var predEngine = trainedModel.CreatePredictionEngine<TrainingData, Prediction>(mlContext);

            //var predictData = new Queries().GetPictureForPredict();

            //TrainingData data = new TrainingData();
            //data.Input = DataProcessing.GetMask(predictData.CaptureImage);
            //data.Output = predictData.Result;

            //var resultprediction = predEngine.Predict(data);
        }

        //public static ITransformer Train(MLContext mlContext, IDataView dataView)
        //{
        //    /////////////VERSION 1////////////////

        //    //return mlContext.Transforms.Conversion.MapValueToKey("OutputVector")
        //    //    .Append(mlContext.Transforms.Categorical.OneHotEncoding("InputVector"))
        //    //    .Append(mlContext.Transforms.Concatenate("Features", "InputVector"))
        //    //    .Append(mlContext.MulticlassClassification.Trainers.StochasticDualCoordinateAscent(labelColumn: "OutputVector", featureColumn: "InputVector"))
        //    //    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel")).Fit(dataView);


        //    /////////////VERSION 2////////////////
        //    //var pipeline =
        //    //    mlContext.Transforms.Text.FeaturizeText("InputVector", "TextFeatures")
        //    //    .Append(mlContext.Transforms.Text.NormalizeText("OutputVector", "InputVector"))
        //    //    .Append(new WordBagEstimator(mlContext, "BagOfWords", "OutputVector"))
        //    //    .Append(new WordHashBagEstimator(mlContext, "BagOfBigrams", "OutputVector", ngramLength: 2, allLengths: false))
        //    //    .Append(mlContext.Transforms.Text.TokenizeCharacters("MessageChars", "InputVector"))
        //    //    .Append(new NgramExtractingEstimator(mlContext, "BagOfTrichar", "MessageChars",
        //    //                ngramLength: 3, weighting: NgramExtractingEstimator.WeightingCriteria.TfIdf))
        //    //    .Append(mlContext.Transforms.Text.TokenizeWords("TokenizedMessage", "OutputVector"))
        //    //    .Append(mlContext.Transforms.Text.ExtractWordEmbeddings("Embeddings", "TokenizedMessage",
        //    //                WordEmbeddingsExtractingTransformer.PretrainedModelKind.GloVeTwitter25D));
        //    //return pipeline.Fit(dataView); //pipeline.Fit(data).Transform(data);
        //}

        //private static IEnumerable<string> EvaluateSinglePrediction(TrainingData currentData, MLContext mlContext, ITransformer model)
        //{
        //    var predictionFunction = model.CreatePredictionEngine<TrainingData, Prediction>(mlContext);
        //    var prediction = predictionFunction.Predict(currentData);


        //    yield return prediction.Output;
        //}
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
                                                                                            OutputVector = tuple.Item2 });
        }
    }
}
