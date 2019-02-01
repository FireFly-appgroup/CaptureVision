using CaptureVision.BLL.Services;
using CaptureVision.DAL.Models;
using CaptureVision.Vision;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace CaptureVision.NN
{
    public class NeuralNetwork
    {
        private Bitmap _processedImage;

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

            foreach (var item in CaptchasFromDB)
            {
                _processedImage = DataProcessing.GetMask(item.CaptureImage);
                //   string Text = DataProcessing.OCR(_processedImage).ToString();
            } 
        }
    }
}
