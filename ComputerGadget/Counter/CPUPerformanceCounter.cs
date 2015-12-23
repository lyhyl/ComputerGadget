using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ComputerGadget.Counter
{
    public sealed class CPUPerformanceCounter : ICounter, IDisposable
    {
        private bool disposed = false;

        private PerformanceCounter counter = new PerformanceCounter("Processor", "% Processor Time");
        private PerformanceCounterCategory category = new PerformanceCounterCategory("Processor");
        private string[] instances = null;
        private Dictionary<string, CounterSample> samples = new Dictionary<string, CounterSample>();
        private Dictionary<string, List<double>> data = new Dictionary<string, List<double>>();

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
        
        public IReadOnlyList<double>[] UpdateAndGetData(int sampleSize)
        {
            UpdateData((int)Math.Ceiling((double)sampleSize / data.Count));
            IReadOnlyList<double>[] dat = new IReadOnlyList<double>[data.Count];
            int i = 0;
            foreach (var d in data)
                dat[i++] = d.Value;
            return dat;
        }

        private void UpdateData(int sampleSize)
        {
            foreach (var s in instances)
                if (IsCore(s))
                {
                    counter.InstanceName = s;
                    CounterSample newSample = counter.NextSample();
                    float dat = CounterSampleCalculator.ComputeCounterValue(samples[s], newSample);
                    //Debug.WriteLine($"CPU {s} {dat}");
                    data[s].Add(dat / 100);
                    samples[s] = newSample;
                    if (data[s].Count > sampleSize)
                        data[s].RemoveRange(0, data[s].Count - sampleSize);
                }
        }

        private bool IsCore(string s) => !s.Contains("_Total");

        public void Dispose()
        {
            if (disposed)
                return;

            counter.Dispose();
            GC.SuppressFinalize(this);
            disposed = true;
        }
    }
}
