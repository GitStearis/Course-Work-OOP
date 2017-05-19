using System;
using System.Windows.Media;

namespace Phase_v2._0
{
    //Class created to generate random color
    //with asked alpha channel

    static class RandomColor
    {
        static Random rnd = new Random();
        static public Color Generate(byte alpha)
        {
            
            Color color = new Color()
            {
                R = (byte)rnd.Next(255),
                G = (byte)rnd.Next(255),
                B = (byte)rnd.Next(255),
                A = alpha
            };
            return color;
        }
    }
}
