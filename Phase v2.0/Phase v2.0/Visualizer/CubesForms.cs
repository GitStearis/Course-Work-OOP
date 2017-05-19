using System.Windows.Media;

namespace Phase_v2._0
{
    static class CubesForms
    {
        static public void Cross(int xIndex, int yIndex, Color col)
        {
                //Draw center of a cross
                CubesContainer.SetCube(xIndex, yIndex, col);

                //Changing color for peripherals
                col.A -= 10;
                col.R += 10;
                col.G += 10;
                col.B += 10;

                //Left and right
                CubesContainer.SetCube(xIndex + 1, yIndex, col);
                CubesContainer.SetCube(xIndex - 1, yIndex, col);

                //Top and bottom
                CubesContainer.SetCube(xIndex, yIndex + 1, col);
                CubesContainer.SetCube(xIndex, yIndex - 1, col);

        }

        static public void Circle(int xIndex, int yIndex, Color col)
        {
            Cross(xIndex, yIndex, col);

            col.A -= 20;
            col.R += 20;
            col.G += 20;
            col.B += 20;

            CubesContainer.SetCube(xIndex + 1, yIndex + 1, col);
            CubesContainer.SetCube(xIndex + 1, yIndex - 1, col);
            CubesContainer.SetCube(xIndex - 1, yIndex + 1, col);
            CubesContainer.SetCube(xIndex - 1, yIndex - 1, col);

            //Top section
            CubesContainer.SetCube(xIndex - 1, yIndex + 2, col);
            CubesContainer.SetCube(xIndex, yIndex + 2, col);
            CubesContainer.SetCube(xIndex + 1, yIndex + 2, col);

            //Bottom section
            CubesContainer.SetCube(xIndex - 1, yIndex - 2, col);
            CubesContainer.SetCube(xIndex, yIndex - 2, col);
            CubesContainer.SetCube(xIndex + 1, yIndex - 2, col);

            //Right section
            CubesContainer.SetCube(xIndex + 2, yIndex + 1, col);
            CubesContainer.SetCube(xIndex + 2, yIndex, col);
            CubesContainer.SetCube(xIndex + 2, yIndex - 1, col);

            //Left secion
            CubesContainer.SetCube(xIndex - 2, yIndex + 1, col);
            CubesContainer.SetCube(xIndex - 2, yIndex, col);
            CubesContainer.SetCube(xIndex - 2, yIndex - 1, col);
        }

        static public void HorizontalLine(int yIndex, Color col)
        {
            for (int i = 0; i < CubesContainer.maxCubesX; i++)
            {
                CubesContainer.SetCube(i, yIndex, col);
            }
        }

        static public void VerticalLine(int xIndex, Color col)
        {
            for (int i = 0; i < CubesContainer.maxCubesY; i++)
            {
                CubesContainer.SetCube(xIndex, i, col);
            }
        }
    }
}
