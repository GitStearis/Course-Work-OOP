using Microsoft.Win32;
using NAudio.Wave;
using System;

namespace Phase_v2._0
{
    static class Analyzer
    {
        static public Mp3FileReader GetInfo(string filepath)
        {
            Mp3FileReader reader = new Mp3FileReader(filepath);

            return reader;
        }
    }
}
