namespace CaptureVision.DAL.Models
{
    public class Capture
    {
        public int ID { get; set; }
        public string CaptureImage { get; set; }
        public string BinaryVector { get; set; }
        public string Result { get; set; }
        public float Percent { get; set; }
        public float TimeInSecond { get; set; }
        public string FileName { get; set; }
    }
}
