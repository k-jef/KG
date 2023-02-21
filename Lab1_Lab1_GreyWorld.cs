using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Lab1
{
    class Contrast: Filters
    {
        public int Avg;
        public int Ravg;
        public int Gavg;
        public int Bavg;


        public int MaxB(Bitmap sourseImage)
        {
            Color sourseColor = sourseImage.GetPixel(0, 0);
            int maxBresult = sourseColor.B;
            for (int i = 0; i < sourseImage.Width; i++)
            {
                for (int j = 0; j < sourseImage.Height; j++)
                {
                    sourseColor = sourseImage.GetPixel(i, j);
                    if (maxBresult < sourseColor.B)
                        maxBresult = sourseColor.B;
                    if (maxBresult == 255)
                        break;
                }
                if (maxBresult == 255)
                    break;
            }
            return maxBresult;
        }
        public int AvgR(Bitmap sourseImage)
        {
            Color sourseColor = sourseImage.GetPixel(0, 0);
            int avgRresult = 0;
            for (int i = 0; i < sourseImage.Width; i++)
            {
                for (int j = 0; j < sourseImage.Height; j++)
                {
                    sourseColor = sourseImage.GetPixel(i, j);
                    avgRresult = avgRresult + sourseColor.R;
                }
            }
            return avgRresult/ (sourseImage.Width * sourseImage.Height);
        }
        public int AvgG(Bitmap sourseImage)
        {
            Color sourseColor = sourseImage.GetPixel(0, 0);
            int avgGresult = 0;
            for (int i = 0; i < sourseImage.Width; i++)
            {
                for (int j = 0; j < sourseImage.Height; j++)
                {
                    sourseColor = sourseImage.GetPixel(i, j);
                    avgGresult = avgGresult + sourseColor.G;
                }
            }
            return avgGresult / (sourseImage.Width * sourseImage.Height);
        }
        public int AvgB(Bitmap sourseImage)
        {
            Color sourseColor = sourseImage.GetPixel(0, 0);
            int avgBresult = 0;
            for (int i = 0; i < sourseImage.Width; i++)
            {
                for (int j = 0; j < sourseImage.Height; j++)
                {
                    sourseColor = sourseImage.GetPixel(i, j);
                    avgBresult = avgBresult + sourseColor.B;
                }
            }
            return avgBresult / (sourseImage.Width * sourseImage.Height);
        }



        protected override Color calculateNewPixelColor(Bitmap sourseImage, int x, int y)
        {
            Color sourseColor = sourseImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(sourseColor.R * Avg / Ravg,
                                               sourseColor.G * Avg / Gavg,
                                               sourseColor.B * Avg / Bavg);
            return resultColor;
        }
    }
}
