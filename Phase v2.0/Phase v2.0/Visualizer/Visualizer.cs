using System;
using System.Windows.Media;
using NAudio.Wave;
using System.Windows.Threading;

namespace Phase_v2._0
{
    static class Visualizer
    {
        static private Mp3FileReader reader;
        static public DispatcherTimer visualizationTimer = new DispatcherTimer();

        static private Mp3Frame frame;

        static public void StartVisualization(string path)
        {
            reader = new Mp3FileReader(path);
            frame = reader.ReadNextFrame();

            visualizationTimer = new DispatcherTimer();
            visualizationTimer.Interval = TimeSpan.FromMilliseconds(500);

            //visualizationTimer.Tick += new EventHandler(DrawLength);
            visualizationTimer.Tick += new EventHandler(DrawMesh);

            visualizationTimer.Start();          
        }

        static public void PauseCurrentVisualization()
        {
            //visualizationTimer.Tick -= new EventHandler(DrawLength);
            visualizationTimer.Tick -= new EventHandler(DrawMesh);
        }

        static public void StopVisualizaton()
        {
            //visualizationTimer.Tick -= new EventHandler(DrawLength);
            visualizationTimer.Tick -= new EventHandler(DrawMesh);
            visualizationTimer.Stop();
        }

        static private Color GenerateColor(int shift)
        {

            int index = Player.player.Position.Seconds;
            int constant = Player.CurrentTrack.GetHashCode() % 100;

            if (frame.RawData[index + shift] == 0 || frame.RawData[index + shift] == 85)//IDK why 85
            {
                frame = reader.ReadNextFrame();
            }

            byte r = (byte)(constant + frame.RawData[index + shift]);
            byte g = (byte)(constant + frame.RawData[index + shift] * frame.BitRateIndex);
            byte b = (byte)(constant + frame.RawData[index + shift] ^ frame.SampleRate);
            byte a = (byte)(Player.player.Volume * 50 + 50);

            //Console.WriteLine("Const: " + constant + " Frame: " + frame.RawData[index + shift]);
            //Console.WriteLine("R: " + r + " G: " + g + " B: " + b + " A: " + a);

            Color color = new Color()
            {
                R = r,
                G = g,
                B = b,
                A = a
            };

            return color;
        }

        static private void DrawLength(object sender, EventArgs e)
        {
            for (int i = 0; i < CubesContainer.maxCubesX; i++)
            {
                CubesForms.VerticalLine(i, GenerateColor(i));
            }
        }

        static private void DrawMesh(object sender, EventArgs e)
        {
            for (int i = 0; i < CubesContainer.maxCubesX / 2; i++)
            {
                int doubleI = i * 2;
                CubesForms.VerticalLine(doubleI, GenerateColor(i));

                if ((i * 2) <= (CubesContainer.maxCubesY / 2))
                {
                    CubesForms.HorizontalLine(doubleI, GenerateColor(i + 1));
                    CubesForms.HorizontalLine(CubesContainer.maxCubesY - doubleI - 1, GenerateColor(i + 1));
                }
            }
        }
    }
}
