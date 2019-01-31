using AForge.Imaging.Filters;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Tesseract;

namespace CaptureVisionTestForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap temp2;
        private void button1_Click(object sender, EventArgs e)
        {
            //   openFileDialog1.ShowDialog();
            var bytes = Convert.FromBase64String("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");
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                pictureBox1.Image = Image.FromStream(ms);
            }



    
            textBox1.Text = reconhecerCaptcha(pictureBox1.Image);
            pictureBox3.Image = temp2;
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Bitmap image = new Bitmap(openFileDialog1.FileName);
            if (image != null)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(openFileDialog1.FileName);
                textBox1.Text = reconhecerCaptcha2(image);
            }
            image.Dispose();
        }
     
        private string reconhecerCaptcha(Image img)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            int threshold_value = 220; //0-255

            Image<Gray, Byte> img2 = new Image<Gray, byte>(bmp);
            pictureBox2.Image = img2.ToBitmap(); 
            img2 = img2.ThresholdBinary(new Gray(threshold_value), new Gray(255));

            pictureBox2.Image = img2.ToBitmap(); 
            Bitmap imagem = new Bitmap(pictureBox2.Image);
            imagem = imagem.Clone(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Erosion erosion = new Erosion();
            Dilatation dilatation = new Dilatation();
            Invert inverter = new Invert();
            ColorFiltering cor = new ColorFiltering();
            cor.Blue = new AForge.IntRange(200, 255);
            cor.Red = new AForge.IntRange(200, 255);
            cor.Green = new AForge.IntRange(200, 255);
           // Opening open = new Opening();
            BlobsFiltering bc = new BlobsFiltering();
            Closing close = new Closing();
          //  GaussianSharpen gs = new GaussianSharpen();
            ContrastCorrection cc = new ContrastCorrection();
            bc.MinHeight = 10;
            FiltersSequence seq = new FiltersSequence(inverter,  inverter, bc, inverter,cc, cor, bc, inverter);

            var result = ScaleByPercent(imagem, 200);
            pictureBox2.Image = seq.Apply(result);



            Bitmap bmp2 = new Bitmap(pictureBox2.Image);

            var palette = new Dictionary<Color, int>();
            for (var x = 0; x < bmp2.Width; x++)
            {
                for (var y = 0; y < bmp2.Height; y++)
                {
                    var clr = bmp2.GetPixel(x, y);
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
                    var temp = this.ClearBitmap(bmp2, c.Key);
                    if (i == 0)
                        temp2 = new Bitmap(temp);
                    temp.Save(String.Format("D:\\mask-{0}.bmp", i));
                    i++;
                }
            }



            string reconhecido = OCR((Bitmap)pictureBox2.Image);
            
            return reconhecido;
        }
        public Bitmap ClearBitmap(Bitmap input, Color clr)
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
        private string reconhecerCaptcha2(Image img)
        {
            Bitmap imagem = new Bitmap(img);
            imagem = imagem.Clone(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Erosion erosion = new Erosion();
            Dilatation dilatation = new Dilatation();
            Invert inverter = new Invert();
            ColorFiltering cor = new ColorFiltering();
            cor.Blue = new AForge.IntRange(200, 255);
            cor.Red = new AForge.IntRange(200, 255);
            cor.Green = new AForge.IntRange(200, 255);
            Opening open = new Opening();
            BlobsFiltering bc = new BlobsFiltering();
            Closing close = new Closing();
            GaussianSharpen gs = new GaussianSharpen();
            ContrastCorrection cc = new ContrastCorrection();
            bc.MinHeight = 10;
            FiltersSequence seq = new FiltersSequence(gs, inverter, open, inverter, bc, inverter, open, cc, cor, bc, inverter);
            pictureBox2.Image = seq.Apply(imagem);
            string reconhecido = OCR((Bitmap)pictureBox2.Image);
            return reconhecido;
        }

        public Bitmap ScaleByPercent(Bitmap imgPhoto, int Percent)
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
                              new System.Drawing.Rectangle(0, 0, destWidth, destHeight),
                              new System.Drawing.Rectangle(0, 0, sourceWidth, sourceHeight),
                              GraphicsUnit.Pixel);
           // bmPhoto.Save(@"D:\Scale.png", System.Drawing.Imaging.ImageFormat.Png);
            grPhoto.Dispose();
            return bmPhoto;
        }
        private string OCR(Bitmap b)
        {
            string res = "";
            using (var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default))
            {
                engine.SetVariable("tessedit_char_whitelist", "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                engine.SetVariable("tessedit_unrej_any_wd", true);

                using (var page = engine.Process(b, PageSegMode.SingleLine))
                    res = page.GetText();
            }
            return res;
        }


    }
}
