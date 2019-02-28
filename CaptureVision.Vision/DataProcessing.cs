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
            if (bmp == null)
                throw new ArgumentException("No valid bitmap");

            Bitmap cropBmp = bmp.Clone(selection, bmp.PixelFormat);
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
                    texto = texto + "\n"; // this is to make the enter between lines   (\r\n)
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return texto;
        }

        public static IEnumerable<Tuple<string, string>> BinaryToSymbol(string vector, string symbols)
        {
            string inputVector = String.Empty;
            string outputSymbol = String.Empty;
            string[] vectorArray = vector.Split('\n');

            vector = vector.Replace("\n", "");
            string[][] data = CreateArray<string>(vectorArray.Length, vectorArray[0].Length);

            foreach (var item in symbols)
            {
                int k = 0;
    

                for (int i = 0; i < vectorArray.Length-1; i++)
                {
                    for (int j = 0; j < vectorArray[0].Length; j++)
                    {
                            data[i][j] = vector[k++].ToString();
                    }
                }
                var newMultidimensionalArray = RemovingNoises(data);

                for (int i = 0; i < newMultidimensionalArray.Length-1; i++)
                {
                    for (int j = 0; j < newMultidimensionalArray[i].Length; j++)
                    {
                        inputVector += data[i][j].ToString();
                    }
                    inputVector = inputVector + "\n";
                }
                outputSymbol = item.ToString();
                yield return new Tuple<string, string>(inputVector, outputSymbol); 
            }
        }

        static T[][] CreateArray<T>(int rows, int cols)
        {
            T[][] array = new T[rows][];
            for (int i = 0; i < array.GetLength(0); i++)
                array[i] = new T[cols];

            return array;
        }

        public static string[][] RemovingNoises(string[][] inputMultidimensionalArray)
        {
            for (int i = 0; i < inputMultidimensionalArray.Length; i++)
            {
                for (int j = 0; j < inputMultidimensionalArray[i].Length; j++)
                {
                    if (j >= 1)
                       if (inputMultidimensionalArray[i][j] == "0" && inputMultidimensionalArray[i][j - 1] == "1" && inputMultidimensionalArray[i][j + 1] == "1")
                            inputMultidimensionalArray[i][j] = "1";
                }
            }

            return inputMultidimensionalArray;
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
            //Erosion erosion = new Erosion();
            //Dilatation dilatation = new Dilatation();
            //FiltersSequence seq = new FiltersSequence(inverter, inverter, bc, inverter, cc, cor, bc, inverter);
            FiltersSequence seq = new FiltersSequence(new Opening(), 
                                                      new GaussianSharpen(), 
                                                      new Invert(), 
                                                      new Invert(), 
                                                      new BlobsFiltering() { MinHeight = 15, MinWidth = 15 }, 
                                                      new Invert(), 
                                                      new ContrastCorrection(), //15
                                                      new ColorFiltering() { Blue = new IntRange(200, 255), Red = new IntRange(200, 255), Green = new IntRange(200, 255) }, 
                                                      new BlobsFiltering() { MinHeight = 15, MinWidth = 15 }, 
                                                      new Invert(), 
                                                      new Closing()); //without closing ?
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
