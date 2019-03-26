using Microsoft.ML.Data;
using System.Drawing;

namespace CaptureVision.NN
{
    public class TrainingData
    {
        [ColumnName("InputVector")] // [ColumnName("InputVector")]]
        public string InputVector { get; set; }

        [ColumnName("Label")] //[ColumnName("OutputVector")]
        public string OutputVector { get; set; }//output (Labels)
    }

    public class TrainingDataForSymbol
    {
        [ColumnName("InputVector")]
        public string InputVector { get; set; }

        [ColumnName("Label")]
        public string OutputVector { get; set; }//output (Labels)
    }
}
