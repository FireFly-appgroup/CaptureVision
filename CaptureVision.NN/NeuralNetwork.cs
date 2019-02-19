using CaptureVision.BLL.Services;
using CaptureVision.DAL.Models;
using CaptureVision.Vision;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace CaptureVision.NN
{
    public class NeuralNetwork
    {
        private List<Tuple<string, Bitmap>> _processedImage = new List<Tuple<string, Bitmap>>();
        //private List<Tuple<string, string>> _processedBinaryImage = new List<Tuple<string, string>>();
        private List<TrainingData> _trainingData = new List<TrainingData>();
        private List<TrainingDataForSymbol> _trainingDataForSymbol = new List<TrainingDataForSymbol>();
        private static object _syncRoot = new Object();

        public static void RunProcessing()
        {
            var neuralNetwork = new NeuralNetwork();
            neuralNetwork.RunAsync();
        }

        private void RunAsync()
        {
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

                _processedImage.ForEach(t =>
                {
                    _trainingData.Add(new TrainingData() { InputVector = DataProcessing.ImageToBinary(t.Item2), OutputVector = t.Item1 });
                });

                _trainingData.ForEach(t =>
                {
                    var result = DataProcessing.BinaryToSymbol(t.InputVector, t.OutputVector);
                    _trainingDataForSymbol.Add(new TrainingDataForSymbol() { InputVector = result.Item1, OutputVector = result.Item2 });
                });
            }

            MLContext mlContext = new MLContext(seed: 0);

            IDataView trainingDataView = mlContext.CreateStreamingDataView<TrainingData>(_trainingData);

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
    }
}
