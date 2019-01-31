using CaptureVision.BLL.Services;
using CaptureVision.Vision;
using System.Drawing;

namespace CaptureVision.NN
{
    public class NeuralNetwork
    {
        private Queries _query;
        private Bitmap _processedImage;
        public static void RunProcessing()
        {
            var neuralNetwork = new NeuralNetwork();
            neuralNetwork.RunAsync();
        }

        private void RunAsync()
        {
            _query = new Queries();
            var ListOfCapture = _query.GetPicturesFromDB();

            foreach (var item in ListOfCapture)
            {
                _processedImage = DataProcessing.GetMask(item.CaptureImage);
               // string Text = DataProcessing.OCR(_processedImage);
            }
        }
    }
}
