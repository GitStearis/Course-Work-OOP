using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Phase_v2._0
{
    class CubesContainer
    {
        static private List<List<Rectangle>> field = new List<List<Rectangle>>();
        static public int maxCubesX;
        static public int maxCubesY;

        const int cubeSize = 20;

        static public void InitializeField(Canvas pad)
        {
            for (int y = 0; y < pad.Height; y += cubeSize)
            {
                List<Rectangle> line = new List<Rectangle>();

                for (int x = 0; x < pad.Width; x += cubeSize)
                {
                    line.Add(NewCube(x, y));
                }

                field.Add(line);
            }

            //Borders of field
            maxCubesX = field[0].IndexOf(field[0].Last()) + 1;
            maxCubesY = field.IndexOf(field.Last()) + 1;

            AddToCanvas(pad);
        }

        static private Rectangle NewCube(int x, int y)
        {
            Rectangle cube = new Rectangle()
            {
                Stroke = new SolidColorBrush(Colors.Wheat),
                Fill = new SolidColorBrush(Colors.Wheat),
                Width = cubeSize,
                Height = cubeSize
            };
            Canvas.SetLeft(cube, x);
            Canvas.SetTop(cube, y);

            return cube;
        }

        static private void AddToCanvas(Canvas pad)
        {
            foreach (var line in field)
            {
                foreach (var element in line)
                {
                    pad.Children.Add(element);
                }
            }
        }

        static public void SetCube(int xIndex, int yIndex, Color col)
        {
            field[yIndex][xIndex].Stroke = new SolidColorBrush(col);
            field[yIndex][xIndex].Fill = new SolidColorBrush(col);
        }

        static public void Clear()
        {
            foreach (var line in field)
            {
                foreach (var element in line)
                {
                    element.Stroke = new SolidColorBrush(Colors.Beige);
                    element.Fill = new SolidColorBrush(Colors.Beige);
                }
            }
        }

        static public void SetRandomly()
        {
            foreach (var line in field)
            {
                foreach (var element in line)
                {
                    Color tempColor = RandomColor.Generate(60);
                    element.Stroke = new SolidColorBrush(tempColor);
                    element.Fill = new SolidColorBrush(tempColor);
                }
            }
        }

        static public void SetWithColor(Color color)
        {
            foreach (var line in field)
            {
                foreach (var element in line)
                {
                    element.Stroke = new SolidColorBrush(color);
                    element.Fill = new SolidColorBrush(color);
                }
            }
        }

    }
}
