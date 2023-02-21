using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Lab1
{
    abstract class Filters
    {
        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);

        public int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public Bitmap processImage (Bitmap sourseImage)
        {
            Bitmap resultImage = new Bitmap(sourseImage.Width, sourseImage.Height);
            for (int i = 0; i < sourseImage.Width; i++)
            {
                for (int j = 0; j < sourseImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourseImage, i, j));
                }
            }
            return resultImage;
        }
    }

    
}
