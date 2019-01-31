using System;
using System.Collections.Generic;
using System.Text;
using Capture = CaptureVision.DAL.Models.Capture;

namespace CaptureVision.BLL.Services
{
    public interface IQueries
    {
        void InsertPicturesToDB(string Picture, string FileName, string Result);
        List<Capture> GetPicturesFromDB();
    }
}
