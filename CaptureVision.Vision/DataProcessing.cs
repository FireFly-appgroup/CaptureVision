using AForge;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace CaptureVision.Vision
{
    public class DataProcessing
    {
        private static Bitmap _bitmap;
        private static System.Drawing.Image _image;

        public static Bitmap GetMask(string input)
        {
            var bytes = Convert.FromBase64String(input);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                _image = System.Drawing.Image.FromStream(ms);
            }

            _bitmap = AddingFilters(new Bitmap(_image));
            var CuttingBitmap = CutSection(_bitmap, new Rectangle(5, 5, _bitmap.Width - 10, _bitmap.Height-10));
            CuttingBitmap.Save(String.Format("D:\\test.bmp"));
            //var palette = new Dictionary<Color, int>();
            //for (var x = 0; x < _bitmap.Width; x++)
            //{
            //    for (var y = 0; y < _bitmap.Height; y++)
            //    {
            //        var clr = _bitmap.GetPixel(x, y);
            //        if (!palette.ContainsKey(clr))
            //        {
            //            palette.Add(clr, 1);
            //        }
            //        else
            //        {
            //            palette[clr] = palette[clr] + 1;
            //        }
            //    }
            //}
            //var i = 0;

            //foreach (var c in palette)
            //{
            //    if (c.Value > 30)
            //    {
            //        var temp = ClearBitmap(_bitmap, c.Key);
            //        if (i == 0)
            //        {
            //            _bitmap = new Bitmap(temp);
            //            temp.Save(String.Format("D:\\mask-{0}.bmp", i));
            //        }
            //        i++;
            //    }
            //}

            return CuttingBitmap;
        }

        public static Bitmap CutSection(Bitmap image, Rectangle selection)
        {
            Bitmap bmp = image as Bitmap;

            // Check if it is a bitmap:
            if (bmp == null)
                throw new ArgumentException("No valid bitmap");

            // Crop the image:
            Bitmap cropBmp = bmp.Clone(selection, bmp.PixelFormat);

            // Release the resources:
            image.Dispose();

            return cropBmp;
        }

        public static string ImageToBinary(Bitmap img)
        {
            string texto = String.Empty;
            try
            {
                for (int i = 0; i < img.Height; i++)
                {
                    for (int j = 0; j < img.Width; j++)
                    {
                        texto = (img.GetPixel(j, i).A.ToString() == "255" && img.GetPixel(j, i).B.ToString() == "255" &&
                                 img.GetPixel(j, i).G.ToString() == "255" && img.GetPixel(j, i).R.ToString() == "255") 
                                 ? texto + "0" : texto + "1";
                    }
                    texto = texto + "\r\n"; // this is to make the enter between lines  
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return texto;
        }

        public static string BinaryToSymbol(string vector)
        {
            string symbol = String.Empty;
            string[] vectorArray = vector.Split('\n');
            string[,] multidimensionalArray = new string[vectorArray.Length, vectorArray[0].Length];
         
            for (int i = 0; i < vectorArray.Length; i++)
            {
                for (int j = 0; j < vectorArray[0].Length; j++)
                {
                    multidimensionalArray[i,j] = vector[j].ToString();
                }
            }

            return symbol;
        }
  
        public static Bitmap ClearBitmap(Bitmap input, Color clr)
        {
            var result = new Bitmap(input.Width, input.Height);
            for (var x = 0; x < input.Width; x++)
            {
                for (var y = 0; y < input.Height; y++)
                {
                    var color = input.GetPixel(x, y);
                    result.SetPixel(x, y, clr == color ? Color.Black : Color.White);
                }
            }

            return result;
        }


        public static Bitmap AddingFilters(Bitmap bitmap)
        {
            Grayscale grayscale_filter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = grayscale_filter.Apply(bitmap);

            Threshold threshold_filter = new Threshold(220);
            threshold_filter.ApplyInPlace(grayImage);

            grayImage = grayImage.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format24bppRgb);
            Erosion erosion = new Erosion();
            Dilatation dilatation = new Dilatation();
            Invert inverter = new Invert();
            ColorFiltering cor = new ColorFiltering();
            cor.Blue = new IntRange(200, 255);
            cor.Red = new IntRange(200, 255);
            cor.Green = new IntRange(200, 255);
            Opening open = new Opening();
            BlobsFiltering bc = new BlobsFiltering();
            Closing close = new Closing(); //?
            GaussianSharpen gs = new GaussianSharpen(); //?
            ContrastCorrection cc = new ContrastCorrection();
            bc.MinHeight = 15; //10 
            bc.MinWidth = 15; // ?
            //FiltersSequence seq = new FiltersSequence(inverter, inverter, bc, inverter, cc, cor, bc, inverter);
            FiltersSequence seq = new FiltersSequence(open, gs, inverter, inverter, bc, inverter, cc, cor, bc, inverter);
            var result = ScaleByPercent(grayImage, 200);
            
            return seq.Apply(result);
        }

        public static Bitmap ScaleByPercent(Bitmap imgPhoto, int Percent)
        {
            float nPercent = ((float)Percent / 100);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);

            var bmPhoto = new Bitmap(destWidth, destHeight,
                                     PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                                  imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                              new Rectangle(0, 0, destWidth, destHeight),
                              new Rectangle(0, 0, sourceWidth, sourceHeight),
                              GraphicsUnit.Pixel);
            grPhoto.Dispose();

            return bmPhoto;
        }
    }
}
