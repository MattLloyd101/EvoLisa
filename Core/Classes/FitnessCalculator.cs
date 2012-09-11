using System.Drawing;
using System.Drawing.Imaging;
using GenArt.AST;
using System;
using GenArt.Core.AST;

namespace GenArt.Classes
{  
    public struct Pixel
    {
        public byte B;
        public byte G;
        public byte R;
        public byte A;
    }

    public class NewFitnessCalculator : IDisposable
    {
        private Bitmap _bmp;
        private Graphics _g;

        public NewFitnessCalculator()
        {
            _bmp = new Bitmap(Tools.MaxWidth, Tools.MaxHeight);
            _g = Graphics.FromImage(_bmp);
        }

        public void Dispose()
        {
            _g.Dispose();
            _bmp.Dispose();
        }

        public double GetDrawingFitness(AbstractDnaDrawing newDrawing, Pixel[] sourcePixels)
        {
            double error = 0;


            newDrawing.Render(_g, 1);

            //Renderer.Render(newDrawing, _g, 1);

            BitmapData bd = _bmp.LockBits(
                new Rectangle(0, 0, Tools.MaxWidth, Tools.MaxHeight),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            unchecked
            {
                unsafe
                {
                    fixed (Pixel* psourcePixels = sourcePixels)
                    {
                        Pixel* p1 = (Pixel*)bd.Scan0.ToPointer();
                        Pixel* p2 = psourcePixels;
                        for (int i = sourcePixels.Length; i > 0; i--, p1++, p2++)
                        {
                            int r = p1->R - p2->R;
                            int g = p1->G - p2->G;
                            int b = p1->B - p2->B;
                            error += r * r + g * g + b * b;
                        }
                    }
                }
            }
            _bmp.UnlockBits(bd);

            return error;
        }

    }

}