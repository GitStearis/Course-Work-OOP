using System;
using System.Windows.Media;
using NAudio.Wave;
using System.Windows.Threading;
using System.Threading;

namespace Phase_v2._0
{
    static class Visualizer
    {
        static private Mp3FileReader reader;
        static public DispatcherTimer visualizationTimer = new DispatcherTimer();
        static private string filepath = "";
        static private Mp3Frame frame;

        static public void StartVisualization(string path)
        {        
            if (path != filepath)
            {
                reader = new Mp3FileReader(path);
                frame = reader.ReadNextFrame();
            }
            filepath = path;

            visualizationTimer = new DispatcherTimer();
            visualizationTimer.Interval = TimeSpan.FromMilliseconds(100);

            visualizationTimer.Tick += new EventHandler(DrawWave);
            //visualizationTimer.Tick += new EventHandler(DrawBeats);

            visualizationTimer.Start();          
        }

        static public void PauseCurrentVisualization()
        {
            visualizationTimer.Tick -= new EventHandler(DrawWave);
            //visualizationTimer.Tick -= new EventHandler(DrawBeats);
        }

        static public void StopVisualizaton()
        {
            visualizationTimer.Tick -= new EventHandler(DrawWave);
            //visualizationTimer.Tick -= new EventHandler(DrawBeats);
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

        static private void DrawColumn(int shift)
        {
            int index = Player.player.Position.Seconds;

            byte currentByte = frame.RawData[index + shift];

            if (currentByte == 0 || currentByte == 85)//IDK why 85
            {
                frame = reader.ReadNextFrame();
            }

            for (int i = 0; i < currentByte % 12; i++)
            {
                CubesContainer.SetCube(shift, i, GenerateColor(shift));
                CubesContainer.SetCube(shift, CubesContainer.maxCubesY - 1 - i, GenerateColor(shift));
            }
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

        static private void DrawHeight(object sender, EventArgs e)
        {
            for (int i = 0; i < CubesContainer.maxCubesY; i++)
            {
                CubesForms.HorizontalLine(i, GenerateColor(i));
            }
        }

        static private void DrawWave(object sender, EventArgs e)
        {
            CubesContainer.SetWithColor(Colors.Moccasin);
            for (int i = 0; i < CubesContainer.maxCubesX; i++)
            {
                DrawColumn(i);
            }
        }
    }
}
