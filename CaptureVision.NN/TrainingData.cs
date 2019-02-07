using Microsoft.ML.Data;
using System.Drawing;

namespace CaptureVision.NN
{
    public class TrainingData
    {
        [Column("0")]
        public Bitmap Input { get; set; }

        [Column("1")]
        public string Output { get; set; }//output (Labels)
    }
}
