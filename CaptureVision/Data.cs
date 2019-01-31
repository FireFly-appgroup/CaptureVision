using CaptureVision.BLL.Services;
using System;
using System.Drawing;
using System.IO;

namespace CaptureVision
{
    public static class Data
    {
        public static void SetNewData()
        {
            int i = 1;
            Queries query;
            foreach (var pathToFile in Directory.EnumerateFiles(Directory.GetCurrentDirectory() + "\\Captchas", "*", SearchOption.TopDirectoryOnly))
            {
                query = new Queries();
                string fileName = Path.GetFileName(pathToFile);
                string Base64Picture = GetBase64StringForImage(pathToFile);
                var Result = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "decoded.txt").Split(new string[] { "|", "\n" }, StringSplitOptions.None);
                query.InsertPicturesToDB(Base64Picture, fileName, Result[i]);
                i += 2;
            }
        }

        public static string GetBase64StringForImage(string imgPath)
        {
            byte[] imageBytes = File.ReadAllBytes(imgPath);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }

        public static Bitmap Base64StringToBitmap(string base64String)
        {
            Bitmap bmpReturn = null;
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);
            memoryStream.Position = 0;
            bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);
            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;
            return bmpReturn;
        }
    }
}
