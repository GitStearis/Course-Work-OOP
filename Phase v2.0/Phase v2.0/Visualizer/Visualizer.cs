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

        static public event EventHandler visualizationMode;

        static public void StartVisualization(string path, int mode)
        {
            if (mode != 0)
            {
                if (path != filepath)
                {
                    reader = new Mp3FileReader(path);
                    frame = reader.ReadNextFrame();
                }
                filepath = path;

                visualizationTimer = new DispatcherTimer();
                visualizationTimer.Interval = TimeSpan.FromMilliseconds(500);

                ChoseVisualizationMode(mode);

                visualizationTimer.Start();
            }     
        }

        static public void ChoseVisualizationMode(int mode)
        {
            if (frame != null)
            {
                switch (mode)
                {
                    case 0:
                        {
                            StopVisualizaton();
                        };
                        break;
                    case 1:
                        {
                            StopVisualizaton();
                            visualizationMode = new EventHandler(DrawHeight);
                            ContinueCurrentVisualization();
                        }; break;
                    case 2:
                        {
                            StopVisualizaton();
                            visualizationMode = new EventHandler(DrawLength);
                            ContinueCurrentVisualization();
                        }; break;
                    case 3:
                        {
                            StopVisualizaton();
                            visualizationMode = new EventHandler(DrawMesh);
                            ContinueCurrentVisualization();
                        }; break;
                    case 4:
                        {
                            StopVisualizaton();
                            visualizationMode = new EventHandler(DrawWave);
                            ContinueCurrentVisualization();
                        }; break;
                    default:
                        {
                            StopVisualizaton();
                            visualizationMode = new EventHandler(DrawLength);
                            ContinueCurrentVisualization();
                        }; break;
                }
            }
        }

        static public void PauseCurrentVisualization()
        {
            visualizationTimer.Tick -= visualizationMode;
        }

        static public void ContinueCurrentVisualization()
        {
            visualizationTimer.Tick += visualizationMode;
            visualizationTimer.Start();
        }

        static public void StopVisualizaton()
        {
            visualizationTimer.Tick -= visualizationMode;
            visualizationTimer.Stop();
        }

        static private Color GenerateColor(int shift)
        {
            int index = Player.player.Position.Seconds;
            int constant = 0;
            if (Player.CurrentTrack != null)
            {
                constant = Player.CurrentTrack.GetHashCode() % 100;
            }
            else
            {
                constant = 0;
            }
                

            if (frame.RawData[index + shift] == 0 || frame.RawData[index + shift] == 85)//IDK why 85
            {
                frame = reader.ReadNextFrame();
            }

            byte r = (byte)(constant + frame.RawData[index + shift]);
            byte g = (byte)(constant + frame.RawData[index + shift] * frame.BitRateIndex);
            byte b = (byte)(frame.RawData[index + shift] ^ frame.SampleRate);
            byte a = (byte)(Player.player.Volume * 50 + 50);

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

            for (int i = 0; i < currentByte / 16; i++)
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
