﻿using Microsoft.ML.Data;
using System.Drawing;

namespace CaptureVision.NN
{
    public class TrainingData
    {
        [Column("0")] // [ColumnName("InputVector")]]
        public string InputVector { get; set; }

        [Column("1")] //[ColumnName("OutputVector")]
        public string OutputVector { get; set; }//output (Labels)
    }

    public class TrainingDataForSymbol
    {
        [Column("0")]
        public string InputVector { get; set; }

        [Column("1")]
        public string OutputVector { get; set; }//output (Labels)
    }
}
