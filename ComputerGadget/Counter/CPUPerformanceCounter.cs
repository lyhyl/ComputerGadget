using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace ComputerGadget.Counter
{
    class CPUPerformanceCounter : ICounter
    {
        private PerformanceCounter counter = new PerformanceCounter("Processor Information", "% Processor Time");
        private PerformanceCounterCategory category = new PerformanceCounterCategory("Processor Information");
        private string[] instances = null;
        private Dictionary<string, CounterSample> samples = new Dictionary<string, CounterSample>();
        private Dictionary<string, List<double>> data = new Dictionary<string, List<double>>();

        public int DataSize { set; get; }

        public string Message
        {
            get
            {
                StringBuilder msg = new StringBuilder();
                for (int i = 0; i < data.Count; i++)
                {
                    msg.Append(i);
                    msg.Append('|');
                }
                return msg.ToString(0, msg.Length - 1);
            }
        }

        public CPUPerformanceCounter()
        {
            instances = category.GetInstanceNames();
            foreach (var s in instances)
                if (IsCore(s))
                {
                    counter.InstanceName = s;
                    samples.Add(s, counter.NextSample());
                    data.Add(s, new List<double>());
                }
        }


        public IReadOnlyList<double>[] UpdateAndGetData()
        {
            UpdateData();
            IReadOnlyList<double>[] dat = new IReadOnlyList<double>[data.Count];
            int i = 0;
            foreach (var d in data)
                dat[i++] = d.Value;
            return dat;
        }

        private void UpdateData()
        {
            foreach (var s in instances)
                if (IsCore(s))
                {
                    counter.InstanceName = s;
                    data[s].Add(Calculate(samples[s], counter.NextSample()));
                    samples[s] = counter.NextSample();
                    if (data[s].Count > DataSize)
                        data[s].RemoveRange(0, data[s].Count - DataSize);
                }
        }

        private bool IsCore(string s) => !s.Contains("_Total");

        private double Calculate(CounterSample oldSample, CounterSample newSample)
        {
            double difference = newSample.RawValue - oldSample.RawValue;
            double timeInterval = newSample.TimeStamp100nSec - oldSample.TimeStamp100nSec;
            if (timeInterval != 0)
                return 1 - difference / timeInterval;
            else
                return 0;
        }
    }
}
