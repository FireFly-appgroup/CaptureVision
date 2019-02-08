using Microsoft.ML.Data;

namespace CaptureVision.NN
{
    public class Prediction
    {
        [ColumnName("PredictedLabel")]
        public string Output { get; set; } //or bool
    }
}
