using System.Windows.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Phase_v1._0
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
