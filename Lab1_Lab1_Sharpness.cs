using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    class Sharpness: MatrixFilter
    {
        public Sharpness()
        {
            kernel = new float[3, 3]{{-1, -1, -1 },
                                    { -1,  9, -1 },
                                    { -1, -1, -1 }};
        }
    }
}

