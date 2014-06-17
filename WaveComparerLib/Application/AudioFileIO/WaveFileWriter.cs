using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WaveComparerLib.AudioFileIO
{
    class WaveFileWriter
    {
        BinaryWriter writer;
        WaveFileFormat waveFile;

        public WaveFileWriter(string fileName)
		{
            writer = new BinaryWriter(new FileStream(fileName, FileMode.Create));
            waveFile = new WaveFileFormat();
		}

        public WaveFileWriter()
        {
            writer = new BinaryWriter(new MemoryStream());
            waveFile = new WaveFileFormat();
        }

        void WriteMainFileChunk(RIFFChunk chunk)
        {
            writer.Write(chunk.sGroupID.ToCharArray());
            writer.Write(chunk.dwFileLength);
            writer.Write(chunk.sRiffType.ToCharArray());
        }

        void WriteFormatChunk(FormatChunk chunk)
        {
            writer.Write(chunk.sChunkID.ToCharArray());
            writer.Write(chunk.dwChunkSize);
            writer.Write(chunk.wFormatTag);
            writer.Write(chunk.wChannels);
            writer.Write(chunk.dwSamplesPerSec);
            writer.Write(chunk.dwAvgBytesPerSec);
            writer.Write(chunk.wBlockAlign);
            writer.Write(chunk.dwBitsPerSample);
        }

        void WriteDataChunk(DataChunk chunk)
        {
            // Write the data chunk
            writer.Write(chunk.sChunkID.ToCharArray());
            writer.Write(chunk.dwChunkSize);
            WriteWaveForm(chunk.doubleArray);
        }

        void WriteWaveForm(double[] data)
        {
            foreach (var dataPoint in data)
            {
                // *** assumption is that data is in 16 bit format!
                writer.Write((short)dataPoint);
            }
        }

        public Stream WriteToStream(WaveFileFormat waveFile)
        {
            WriteMainFileChunk(waveFile.riffChunk);
            WriteFormatChunk(waveFile.formatChunk);
            WriteDataChunk(waveFile.dataChunk);
            
            // go back and correct the file length
            writer.Seek(4, SeekOrigin.Begin);
            uint filesize = (uint)writer.BaseStream.Length;
            writer.Write(filesize - 8);

            if (writer.BaseStream.GetType() == typeof(MemoryStream))
            {
                writer.BaseStream.Position = 0;
                return writer.BaseStream;
            }
            // Clean up
            writer.Close();
            return null;
        }
    }
}
