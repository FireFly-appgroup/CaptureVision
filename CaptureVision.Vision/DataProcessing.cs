using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace CaptureVision.Vision
{
    public class DataProcessing
    {
        private static Bitmap _bitmap;
        private static Image _image;

        public static Bitmap GetMask(string input)
        {
            var bytes = Convert.FromBase64String(input);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                _image = Image.FromStream(ms);
            }

            _bitmap = AddingFilters(_image);

            var palette = new Dictionary<Color, int>();
            for (var x = 0; x < _bitmap.Width; x++)
            {
                for (var y = 0; y < _bitmap.Height; y++)
                {
                    var clr = _bitmap.GetPixel(x, y);
                    if (!palette.ContainsKey(clr))
                    {
                        palette.Add(clr, 1);
                    }
                    else
                    {
                        palette[clr] = palette[clr] + 1;
                    }
                }
            }
            var i = 0;

            foreach (var c in palette)
            {
                if (c.Value > 30)
                {
                    var temp = ClearBitmap(_bitmap, c.Key);
                    if (i == 0)
                        _bitmap = new Bitmap(temp);
                //    temp.Save(String.Format("D:\\mask-{0}.bmp", i));
                    i++;
                }
            }

            return _bitmap;
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


        public static Bitmap AddingFilters(Image image)
        {
            Bitmap bitmap = new Bitmap(image);

            Grayscale grayscale_filter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = grayscale_filter.Apply(bitmap);

            Threshold threshold_filter = new Threshold(220);
            threshold_filter.ApplyInPlace(grayImage);

            grayImage = grayImage.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format24bppRgb);
            Erosion erosion = new Erosion();
            Dilatation dilatation = new Dilatation();
            Invert inverter = new Invert();
            ColorFiltering cor = new ColorFiltering();
            cor.Blue = new AForge.IntRange(200, 255);
            cor.Red = new AForge.IntRange(200, 255);
            cor.Green = new AForge.IntRange(200, 255);
            BlobsFiltering bc = new BlobsFiltering();
            Closing close = new Closing();
            ContrastCorrection cc = new ContrastCorrection();
            bc.MinHeight = 10;
            FiltersSequence seq = new FiltersSequence(inverter, inverter, bc, inverter, cc, cor, bc, inverter);

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
            // bmPhoto.Save(@"D:\Scale.png", System.Drawing.Imaging.ImageFormat.Png);
            grPhoto.Dispose();

            return bmPhoto;
        }


        //public async void GetFormat()
        //{
        //    using (var sourceStream = await sourceFile.OpenAsync(FileAccessMode.Read))
        //    {
        //        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(sourceStream);
        //        BitmapTransform transform = new BitmapTransform() { ScaledHeight = 80, ScaledWidth = 80 };
        //        PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
        //            BitmapPixelFormat.Rgba8,
        //            BitmapAlphaMode.Straight,
        //            transform,
        //            ExifOrientationMode.RespectExifOrientation,
        //            ColorManagementMode.DoNotColorManage);

        //        using (var destinationStream = await destinationFile.OpenAsync(FileAccessMode.ReadWrite))
        //        {
        //            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, destinationStream);
        //            encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied, 80, 80, 96, 96, pixelData.DetachPixelData());
        //            await encoder.FlushAsync();
        //        }
        //    }
        //}

    }
}
