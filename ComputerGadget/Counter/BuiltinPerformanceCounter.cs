using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ComputerGadget.Counter
{
    public abstract class BuiltinPerformanceCounter : IPerformanceDataCollector, IDisposable
    {
        private bool disposed = false;
        private PerformanceCounter counter;
        private PerformanceCounterCategory category;
        private Dictionary<string, CounterSample> samples = new Dictionary<string, CounterSample>();

        protected string[] instances = null;
        protected Dictionary<string, List<double>> data = new Dictionary<string, List<double>>();

        public abstract string BriefMessage { get; }

        public BuiltinPerformanceCounter(string categoryName, string counterName)
        {
            counter = new PerformanceCounter(categoryName, counterName);
            category = new PerformanceCounterCategory(categoryName);

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
            int subsampleSize = (int)Math.Ceiling((double)sampleSize / Math.Max(1, data.Count));
            UpdateData(subsampleSize);
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
                    //Debug.WriteLine($"bi {s} {dat}");
                    data[s].Add(Math.Min(dat / 100, 1));
                    samples[s] = newSample;
                    if (data[s].Count > sampleSize)
                        data[s].RemoveRange(0, data[s].Count - sampleSize);
                }
        }

        protected bool IsCore(string s) => !s.Contains("_Total");

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
