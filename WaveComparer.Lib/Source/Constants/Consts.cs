using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveComparer.Lib
{
    /// <summary>
    /// Container for global constants
    /// </summary>
    class Constants
    {
        // Wave file chunk IDs
        public const int ChunkIdSize = 4;
        public const string RIFFChunkID = "RIFF";
        public const string FmtChunkID = "fmt ";
        public const string FactChunkID = "fact";
        public const string DataChunkID = "data";
        // RIFF types
        public const string WAVEtype = "WAVE";
        // Wave format consts *** these are hard coded for now
        public const uint Fmt_dwChunkSize = 16;
        public const ushort Fmt_wFormatTag = 1; // 1 if uncompressed
        public const ushort Fmt_wChannels = 1; // Mono
    }
}
