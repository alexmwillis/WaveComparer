using System;

namespace WaveComparerLib.AudioFileIO
{
    //public abstract class Chunk
    //{
    //    readonly string _ID;
    //    readonly int _size;

    //    public const int ChunkIDLength = 4;

    //    public Chunk(int size)
    //    {
    //        _size = size;
    //    }

    //    public string ID { get { return _ID; } }
    //    public int Size { get { return _size; } }

    //}

	/// <summary>
	/// A class for loading into memory and manipulating wave file data.
	/// </summary>
    public class RIFFChunk
    {
        //These three fields constitute the riff header
        public readonly string sGroupID = Constants.RIFFChunkID;
        public uint dwFileLength { get; set; }    //In bytes, measured from offset 8
        public readonly string sRiffType = Constants.WAVEtype;

        public RIFFChunk() { }

        public RIFFChunk(uint dwFileLength)
        {
            this.dwFileLength = dwFileLength;
        }
    }

    public class FormatChunk
    {
        public readonly string sChunkID = Constants.FmtChunkID; //Four bytes: "fmt "
        public uint dwChunkSize { get; set; }                 //Length of header
        public ushort wFormatTag { get; set; }                  //1 if uncompressed
        public ushort wChannels { get; set; }                   //Number of channels: 1-5
        public uint dwSamplesPerSec { get; set; }               //In Hz
        public uint dwAvgBytesPerSec { get; set; }              //For estimating RAM allocation
        public ushort wBlockAlign { get; set; }                 //Sample frame size in bytes
        public ushort dwBitsPerSample { get; set; }             //Bits per sample               ***changed from uint

        public FormatChunk() { }
        
        public FormatChunk(uint dwChunkSize, ushort wFormatTag, ushort wChannels, uint dwSamplesPerSec,
            uint dwAvgBytesPerSec, ushort wBlockAlign, ushort dwBitsPerSample)
        {
            this.dwChunkSize = dwChunkSize;
            this.wFormatTag = wFormatTag;
            this.wChannels = wChannels;
            this.dwSamplesPerSec = dwSamplesPerSec;
            this.dwAvgBytesPerSec = dwAvgBytesPerSec;
            this.wBlockAlign = wBlockAlign;
            this.dwBitsPerSample = dwBitsPerSample;
        }
    }

	/* The fact chunk is used for specifying the compression
	ratio of the data */
    public class FactChunk
    {
        public readonly string sChunkID = Constants.FactChunkID;  //Four bytes: "fact"
        public uint dwChunkSize { get; set; }           //Length of header
        public uint dwNumSamples { get; set; }      	        //Number of audio frames;
        //numsamples/samplerate should equal file length in seconds.

        public FactChunk() { }

        public FactChunk(uint dwChunkSize, uint dwNumSamples)
        {
            this.dwChunkSize = dwChunkSize;
            this.dwNumSamples = dwNumSamples;
        }
    }

    public class DataChunk
    {
        public readonly string sChunkID = Constants.DataChunkID;   //Four bytes: "data"
        public uint dwChunkSize { get; set; }                   //Length of header

        //The following non-standard fields were created to simplify
        //editing.  We need to know, for filestream seeking purposes,
        //the beginning file position of the data chunk.  It's useful to
        //hold the number of samples in the data chunk itself.  Finally,
        //the minute and second length of the file are useful to output
        //to XML.
        public long lFilePosition { get; set; }         //Position of data chunk in file
        public uint dwMinLength { get; set; }           //Length of audio in minutes
        public double dSecLength { get; set; }		    //Length of audio in seconds
        public uint dwNumSamples { get; set; }		    //Number of audio frames
        // Represents sampled wave form
        public double[] doubleArray { get; set; }     // ***
    }

    public class WaveFileFormat
    {
        public RIFFChunk riffChunk { get; set; }
        public FormatChunk formatChunk { get; set; }
        public FactChunk factChunk { get; set; }
        public DataChunk dataChunk { get; set; }

        public WaveFileFormat()
        { }

        public WaveFileFormat(double[] samples, uint sampleRate, ushort bitDepth)
        {
            this.riffChunk = new RIFFChunk();

            this.formatChunk = new FormatChunk();
            formatChunk.dwChunkSize = Constants.Fmt_dwChunkSize;
            formatChunk.wFormatTag = Constants.Fmt_wFormatTag;
            formatChunk.wChannels = Constants.Fmt_wChannels;
            formatChunk.dwSamplesPerSec = sampleRate;
            formatChunk.wBlockAlign = (ushort)(Constants.Fmt_wChannels * (bitDepth / 8)); ;
            formatChunk.dwAvgBytesPerSec = sampleRate * formatChunk.wBlockAlign;
            formatChunk.dwBitsPerSample = bitDepth;

            this.dataChunk = new DataChunk();
            dataChunk.doubleArray = samples;
            // Calculate data chunk size in bytes
            dataChunk.dwChunkSize = (uint)(dataChunk.doubleArray.Length * (formatChunk.dwBitsPerSample / 8));
        }
    }
}
