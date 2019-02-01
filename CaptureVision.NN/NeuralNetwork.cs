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
        private List<Bitmap> _processedImage = new List<Bitmap>();
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

            //lock (_lockObject)
            //{
            //        Parallel.ForEach(CaptchasFromDB, element =>
            //        {
            //        _processedImage.Add(DataProcessing.GetMask(element.CaptureImage));
            //        });
            //}

            CaptchasFromDB.ForEach(t =>
            {
                _processedImage.Add(DataProcessing.GetMask(t.CaptureImage));
            });


            //foreach (var item in CaptchasFromDB)
            //{
            //    _processedImage = DataProcessing.GetMask(item.CaptureImage);
            //    //   string Text = DataProcessing.OCR(_processedImage).ToString();
            //} 
        }
    }
}
