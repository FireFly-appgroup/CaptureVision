using CaptureVision.BLL.Services;
using CaptureVision.DAL.Models;
using CaptureVision.Vision;
using Microsoft.ML;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CaptureVision.NN
{
    public class NeuralNetwork
    {
        private List<Tuple<string, Bitmap>> _processedImage = new List<Tuple<string, Bitmap>>();
        private List<Tuple<string, string>> _processedBinaryImage = new List<Tuple<string, string>>();
        private List<Tuple<string, Bitmap>> _predictData = new List<Tuple<string, Bitmap>>();
        //private List<TrainingData> _listOfData = new List<TrainingData>();
        //private TrainingData _data;
        //private object _lockObject = new object();

        public static void RunProcessing()
        {
            var neuralNetwork = new NeuralNetwork();
            neuralNetwork.RunAsync();
        }

        private void RunAsync()
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
                _processedBinaryImage.Add(new Tuple<string, string>(t.Item1, DataProcessing.ImageToBinary(t.Item2)));
            });

            MLContext mlContext = new MLContext(seed: 0);

            //IDataView trainingDataView = mlContext.CreateStreamingDataView(_processedImage.Cast<List<Tuple<string, Bitmap>>>());
            //IDataView testDataView = mlContext.CreateStreamingDataView(_processedImage.Cast<List<Tuple<string, Bitmap>>>());

            //var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText("Input", "Output");

            //var trainer = mlContext.BinaryClassification.Trainers.FastTree(labelColumn: "Output", featureColumn: "Input");
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
