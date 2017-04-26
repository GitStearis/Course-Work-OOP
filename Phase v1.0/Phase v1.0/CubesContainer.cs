using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Phase_v1._0
{
    class CubesContainer
    {
        static private List<List<Rectangle>> field = new List<List<Rectangle>>();
        static public int maxCubesX;
        static public int maxCubesY;

        static public void InitializeField(Canvas pad)
        {
            try
            {
                for (int y = 0; y < pad.Height; y += 20)
                {
                    List<Rectangle> line = new List<Rectangle>(20);

                    for (int x = 0; x < pad.Width; x += 20)
                    {
                        line.Add(NewCube(x, y));
                    }

                    field.Add(line);
                }
                Console.WriteLine("List of cubes initialized...");

                //Borders of field
                maxCubesX = field[0].IndexOf(field[0].Last()) + 1;
                maxCubesY = field.IndexOf(field.Last()) + 1;
                Console.WriteLine("Borders initialized...");

                AddToCanvas(pad);
            }
            catch (System.Exception error)
            {
                Console.WriteLine(error.Message);
                Console.WriteLine(error.Data);
            }
        }

        static private Rectangle NewCube(int x, int y)
        {
            Rectangle cube = new Rectangle()
            {
                Stroke = new SolidColorBrush(Colors.Wheat),
                Fill = new SolidColorBrush(Colors.Wheat),
                Width = 20,
                Height = 20
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

            Console.WriteLine("List of cubes has been successfully added to canvas!");
        }

        static public void SetCube(int xIndex, int yIndex, Color col)
        {
            if (xIndex >= 0 && yIndex >= 0 && xIndex < CubesContainer.maxCubesX && yIndex < CubesContainer.maxCubesY)
            {
                field[yIndex][xIndex].Stroke = new SolidColorBrush(col);
                field[yIndex][xIndex].Fill = new SolidColorBrush(col);
            }
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

    }
}
