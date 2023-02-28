using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace Filters
{
    abstract class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);
        public Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = 0; i<sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for (int j = 0; j< sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }

            return resultImage;
        }

        public int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        
    }
     class InvertFilter : Filters
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourceColor.R,
                                               255 - sourceColor.G,
                                               255 - sourceColor.B);

            return resultColor;
        }
        
    }

    class MatrixFilter : Filters
        
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;

            for(int l = -radiusY; l <= radiusY; l++)
            {
                for( int k = -radiusX; k<= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            }

            return Color.FromArgb(Clamp((int)resultR, 0, 255),
                                   Clamp((int)resultG, 0, 255),
                                   Clamp((int)resultB, 0, 255));


        }

    }

    class BlurFilter : MatrixFilter
    {
        public BlurFilter()
        {
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i =0; i< sizeX; i++)
            {
                for(int j = 0; j< sizeY; j++)
                {
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
                }
            }
        }
    }

    class GaussianFilter : MatrixFilter
    {
       
        public void createGaussianKernel(int radius, float sigma)
        {
            int size = 2 * radius + 1;
            kernel = new float[size, size];
            float norm = 0;
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            }

            for(int i =0; i<size; i++)
            {
                for(int j = 0; j<size; j++)
                {
                    kernel[i, j] /= norm;
                }
            }
        }
        public GaussianFilter() {
            createGaussianKernel(3, 2); 
        }
    }

    class SobelFilter : MatrixFilter
    {

        static int[,] maskX = new int[3, 3] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
        static int[,] maskY = new int[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };

        protected override Color calculateNewPixelColor(Bitmap source, int x, int y)
        {
            int radiusX = maskX.GetLength(0) / 2;
            int radiusY = maskX.GetLength(1) / 2;

            float[] RGBy = new float[3] { 0f, 0f, 0f };
            float[] RGBx = new float[3] { 0f, 0f, 0f };
            for (int i = -radiusY; i <= radiusY; i++)
            {
                for (int j = -radiusX; j <= radiusX; j++)
                {
                    int idX = Clamp(x + j, 0, source.Width - 1);
                    int idY = Clamp(y + i, 0, source.Height - 1);
                    Color neighborColor = source.GetPixel(idX, idY);
                    RGBx[0] += neighborColor.R * maskX[j + radiusX, i + radiusY];
                    RGBx[1] += neighborColor.G * maskX[j + radiusX, i + radiusY];
                    RGBx[2] += neighborColor.B * maskX[j + radiusX, i + radiusY];

                    RGBy[0] += neighborColor.R * maskY[j + radiusX, i + radiusY];
                    RGBy[1] += neighborColor.G * maskY[j + radiusX, i + radiusY];
                    RGBy[2] += neighborColor.B * maskY[j + radiusX, i + radiusY];
                }
            }

            return Color.FromArgb(
            Clamp((int)Math.Sqrt(RGBx[0] * RGBx[0] + RGBy[0] * RGBy[0]), 0, 255),
            Clamp((int)Math.Sqrt(RGBx[1] * RGBx[1] + RGBy[1] * RGBy[1]), 0, 255),
            Clamp((int)Math.Sqrt(RGBx[2] * RGBx[2] + RGBy[2] * RGBy[2]), 0, 255));
        }
    }


        //private static int Clamp(int value, int min, int max)
        //{
        //    if (value < min) return min;
        //    if (value > max) return max;
        //    return value;
        //}
    



        //protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        //{

        //    int radiusX = kernel.GetLength(0) / 2;
        //    int radiusY = kernel.GetLength(1) / 2;

        //    float[] resultR = new float[] { 0, 0, 0 };
        //    float[] resultG = new float[] { 0, 0, 0 };
        //    float[] resultB = new float[] { 0, 0, 0 };

        //    //int radius = 1;
        //    //int size = 2 * radius + 1;


        //    int[,] _x = new int[,] { { -1, 0, 1 },
        //                                     { -2, 0, 2 },
        //                                     { -1, 0, 1 } };


        //    int[,] _y = new int[,]{ { -1, -2, -1 },
        //                                     { 0, 0, 0 },
        //                                     { 1, 2, 1 } };

        //    int i = 0;

        //    for (int l = -radiusY; l <= radiusY; l++)
        //    {
        //        for (int k = -radiusX; k <= radiusX; k++)
        //        {
        //            int idX = Clamp(x + k, 0, sourceImage.Width - 1);
        //            int idY = Clamp(y + l, 0, sourceImage.Height - 1);
        //            Color neighborColor = sourceImage.GetPixel(idX, idY);
        //            resultR[i] += neighborColor.R * kernel[k + radiusX, l + radiusY];
        //            resultG[i] += neighborColor.G * kernel[k + radiusX, l + radiusY];
        //            resultB[i] += neighborColor.B * kernel[k + radiusX, l + radiusY];
        //            i++;
        //        }
        //    }
        

        //    return Color.FromArgb(Clamp((int)resultR, 0, 255),
        //                           Clamp((int)resultG, 0, 255),
        //                           Clamp((int)resultB, 0, 255));


        

        //    for (int i = 0; i <= size; i++)
        //    {
        //        for (int j = -radius; j <= radius; j++)
        //        {
        //            _x [i + radius, j + radius] = Clamp((int)(Math.Sqrt((i * i + j * j))), 0, 255);
        //            _y [i + radius, j + radius] = Clamp((int)(Math.Sqrt((i * i + j * j))), 0, 255);
        //        } 
        //    }

            //return Color.FromArgb(
            //                        Clamp((int)Math.Sqrt(_x[0] * RGBx[0] + RGBy[0] * RGBy[0]), 0, 255),
            //                        Clamp((int)Math.Sqrt(RGBx[1] * RGBx[1] + RGBy[1] * RGBy[1]), 0, 255),
            //                        Clamp((int)Math.Sqrt(RGBx[2] * RGBx[2] + RGBy[2] * RGBy[2]), 0, 255));
       



    class GrayScaleFilter : Filters
    {

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);

            Color resultColor = Color.FromArgb((int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B),
                                               (int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B),
                                               (int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B));

            return resultColor;
        }

    }

    class Sepia : Filters
    {

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);

            int r, g, b, intensity;
            int k = 40;

            intensity = (int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B);
            r = intensity + 2 * k;
            g = (int)(intensity + 0.5 * k);
            b = intensity - 1 * k;


            //sourceColor.R = (int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B);


            Color resultColor = Color.FromArgb(Clamp((int)r, 0, 255),
                                   Clamp((int)g, 0, 255),
                                   Clamp((int)b, 0, 255));

            return resultColor;
        }

    }

    class IncreaseBrightness : Filters
    {

        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);

            int r, g, b;
            int k = 100;

            
            r = sourceColor.R + k;
            g = sourceColor.G + k;
            b =  sourceColor.B + k;


            //sourceColor.R = (int)(0.36 * sourceColor.R + 0.53 * sourceColor.G + 0.11 * sourceColor.B);


            Color resultColor = Color.FromArgb(Clamp((int)r, 0, 255),
                                   Clamp((int)g, 0, 255),
                                   Clamp((int)b, 0, 255));

            return resultColor;
        }

    }
}
