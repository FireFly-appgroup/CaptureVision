using Microsoft.ML.Data;
using System.Drawing;

namespace CaptureVision.NN
{
    public class TrainingData
    {
        [Column("0")]
        public string InputVector { get; set; }

        [Column("1")]
        public string OutputVector { get; set; }//output (Labels)
    }
}
