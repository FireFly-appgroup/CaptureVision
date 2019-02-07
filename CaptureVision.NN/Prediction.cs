using Microsoft.ML.Data;

namespace CaptureVision.NN
{
    public class Prediction
    {
        [ColumnName("Score")]
        public float Output { get; set; }
    }
}
