using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using FurtherMath.Base.Collections;

using WaveComparer.Lib.Gen_Utils;

namespace WaveComparer.Lib.Analysis
{
    public class FreqPartition : INotifyChanged
    {
        public event ChangedEventHandler Changed;

        float _percentage; // Percentage of fft size
        float _hertz;

        public FreqPartition(float percentage)
        {
            _percentage = percentage;
            _hertz = percentage * AudioFile.BaseSampleRate / 2;
        }

        public float Percentage
        {
            get
            {
                return _percentage;
            }
            set
            {
               _percentage = value;
                _hertz = value  * AudioFile.BaseSampleRate / 2;
                this.OnChanged();
            }
        } // Percentage of fft size
        public float Hertz { get { return _hertz; } }

        public void OnChanged()
        {
            if (this.Changed != null)
                this.Changed(this, EventArgs.Empty);
        }
    }

    public sealed class FrequencyPartitionList :  ObservableList<FreqPartition>
    {
        private static readonly FrequencyPartitionList _instance = new FrequencyPartitionList();
  
        public const float partitionFloor = 100 / (AudioFile.BaseSampleRate / 2); // floor at 100 Htz

        private FrequencyPartitionList() { }

        public void AddPartitions(params float[] frequencyPartitions)
        {
            foreach (float value in frequencyPartitions)
            {
                if (value < 0 || value > 1) { throw new ArgumentException("Partition must be a percentage between 0 and 1"); }
                var partition = new FreqPartition(value);
                this.Add(partition);
            }
            this.Add(new FreqPartition(1));
        }

        public static FrequencyPartitionList Instance
        {
            get
            {
                return _instance;
            }
        }

        public FreqPartition[] GetBand(FreqPartition partition)
        {
            var index = this.IndexOf(partition);
            return GetBand(index);
        }

        public FreqPartition[] GetBand(int index)
        {
            //var partitionFloor_Ceiling = new FreqPartition[2];
            if (index == 0)
            {
                return new FreqPartition[] { new FreqPartition(partitionFloor), this[index] };

            }
            else if (index > 0 && index < this.Count)
            {
                return new FreqPartition[] { this[index - 1], this[index] };
            }
            else { throw new ArgumentException("index out of bounds"); }
        }
    }
}
