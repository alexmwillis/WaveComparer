using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

using WaveComparer.Lib.Gen_Utils;

namespace WaveComparer.Lib.AudioFileIO
{
	/// <summary>
	/// This class gives you repurposable read/write access to a wave file.
	/// </summary>
	public class WaveFileReader : IDisposable
	{   
		BinaryReader _reader;
        
        long _fileLength;
        WaveFileFormat _waveFile;

		public WaveFileReader(string FileName)
		{
            _reader = new BinaryReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
            _waveFile = new WaveFileFormat();
		}

        public long FilePosition
        {
            get
            {
                return _reader.BaseStream.Position;
            }
            set
            {
                _reader.BaseStream.Position = value;
            }
        }

        public WaveFileFormat ReadData()
        {   
            _waveFile.riffChunk = ReadMainFileHeader();

            _fileLength = _waveFile.riffChunk.dwFileLength;

            while (this.FilePosition + Constants.ChunkIdSize < _fileLength)
            {
                var chunkID = this.ReadString(Constants.ChunkIdSize);
                var chunkSize = this.ReadUInt();

                var startPosition = this.FilePosition;

                switch (chunkID)
                {
                    case Constants.FmtChunkID:

                        _waveFile.formatChunk = ReadFormatHeader(chunkSize);
                        break;

                    case Constants.FactChunkID:

                        _waveFile.factChunk = ReadFactHeader(chunkSize);
                        break;

                    case Constants.DataChunkID:

                        _waveFile.dataChunk = ReadDataHeader(chunkSize);
                        break;

                    default:

                        //This provides the required skipping of unsupported chunks.
                        AdvanceToNext(chunkSize);
                        break;
                }
                this.FilePosition = startPosition + chunkSize;
                //if (GetPosition() + format.dwChunkSize == mainfile.dwFileLength) { break; }
            }
            _waveFile.dataChunk.doubleArray = ReadWaveForm();
            var v = Validate(_waveFile);
            if (v.IsValid)
            {
                return _waveFile;
            }
            else
            {
                throw new Exception(v.Errors[0]);
            }
        }

        protected ValidationResult Validate(WaveFileFormat waveFile)
        {
            var errors = new List<string>();
            var isValid = true;

            if (waveFile.dataChunk == null)
            {
                errors.Add("Data chunk not found");
                isValid = false;
            }
            if (waveFile.dataChunk.doubleArray.Length == 0)
            {
                errors.Add("No samples");
                isValid = false;
            }
            return new ValidationResult(isValid, errors);
        }

        private uint ReadUInt()
        {
            try
            {
                return _reader.ReadUInt32();
            }
            catch
            {
                throw new IOException("Unable to read uint");
            }
        }
		
        string ReadString(int length)
        {
            try
            {
                return new string(_reader.ReadChars(length));
            }
            catch
            {
                throw new IOException("Unable to read string");
            }
        }

		/*
		 * void AdvanceToNext() - 2004 August 2
		 * Advances to the next chunk in the file.  This is fine, 
		 * since we only really care about the fmt and data 
		 * streams for now.
		 */
		void AdvanceToNext(uint chunkSize)
		{
			//Seek to the next offset from current position
            _reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
		}

        /*
		 * WaveFileFormat ReadMainFileHeader - 2004 July 28
		 * Read in the main file header.  Not much more to say, really.
		 * For XML serialization purposes, I "correct" the dwFileLength
		 * field to describe the whole file's length.
		 */
        RIFFChunk ReadMainFileHeader()
        {
            var mainfile = new RIFFChunk();

            var sGroupID = new string(_reader.ReadChars(4));
            mainfile.dwFileLength = _reader.ReadUInt32() + 8;
            var sRiffType = new string(_reader.ReadChars(4));
            if (sGroupID != Constants.RIFFChunkID || sRiffType != Constants.WAVEtype)
            {
                throw new InvalidDataException("Invalid Wave File");
            }
            return mainfile;
        }

        FormatChunk ReadFormatHeader(uint chunkSize)
		{
            var format = new FormatChunk();

			format.dwChunkSize = chunkSize;
			format.wFormatTag = _reader.ReadUInt16();
			format.wChannels = _reader.ReadUInt16();
			format.dwSamplesPerSec = _reader.ReadUInt32();
			format.dwAvgBytesPerSec = _reader.ReadUInt32();
			format.wBlockAlign = _reader.ReadUInt16();
			format.dwBitsPerSample = _reader.ReadUInt16();            
			return format;
		}

        FactChunk ReadFactHeader(uint chunkSize)
		{
			var fact = new FactChunk();

			fact.dwChunkSize = chunkSize;
			fact.dwNumSamples = _reader.ReadUInt32();
			return fact;
		}

        DataChunk ReadDataHeader(uint chunkSize)
		{
			var data = new DataChunk(); 

			data.dwChunkSize = chunkSize;
			data.lFilePosition = _reader.BaseStream.Position;
			if (_waveFile.factChunk != null)
                data.dwNumSamples = _waveFile.factChunk.dwNumSamples;
			else
                data.dwNumSamples = data.dwChunkSize / ((uint)_waveFile.formatChunk.dwBitsPerSample / 8 * _waveFile.formatChunk.wChannels);
			//The above could be written as data.dwChunkSize / format.wBlockAlign, but I want to emphasize what the frames look like.
            data.dwMinLength = (data.dwChunkSize / _waveFile.formatChunk.dwAvgBytesPerSec) / 60;
            data.dSecLength = ((double)data.dwChunkSize / (double)_waveFile.formatChunk.dwAvgBytesPerSec) - (double)data.dwMinLength * 60;
            return data;
		}

        double[] ReadWaveForm()
        {
            var doubleArray = new double[_waveFile.dataChunk.dwNumSamples];
            
            // Read waveform
            this.FilePosition = _waveFile.dataChunk.lFilePosition;

            for (uint i = 0; i < _waveFile.dataChunk.dwNumSamples; i++)
            {
                switch (_waveFile.formatChunk.dwBitsPerSample)
                {
                    case 8:

                        throw new NotImplementedException();

                    case 16:
                        
                        doubleArray[i] = (float)_reader.ReadInt16();
                        break;

                    case 32:

                        throw new NotImplementedException();

                }
            }
            return doubleArray;
        }

        public void Dispose() 
		{
			if(_reader != null) 
				_reader.Close();
		}
	}
}
