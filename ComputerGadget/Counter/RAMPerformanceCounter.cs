using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ComputerGadget.Counter
{
    public sealed class RAMPerformanceCounter : ICounter
    {
        private static class NativeMethods
        {
            [DllImport("psapi.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetPerformanceInfo([Out] out RawRAMInfo raw, [In] int Size);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RawRAMInfo
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        private const long MB = 1024 * 1024;

        private long total;
        private double lastData = 0;
        private List<double> data = new List<double>();

        public string Message => $"{total} MB:{Math.Round(lastData * 100)}%";

        public RAMPerformanceCounter()
        {
            RawRAMInfo raw = new RawRAMInfo();
            if (NativeMethods.GetPerformanceInfo(out raw, Marshal.SizeOf(raw)))
                total = raw.PhysicalTotal.ToInt64() * raw.PageSize.ToInt64() / MB;
            else
                total = long.MaxValue;
        }

        public IReadOnlyList<double>[] UpdateAndGetData(int sampleSize)
        {
            UpdateData(sampleSize);
            return new IReadOnlyList<double>[] { data };
        }

        private void UpdateData(int sampleSize)
        {
            lastData = GetPhysicalPercentage();
            data.Add(lastData);
            if (data.Count > sampleSize)
                data.RemoveRange(0, data.Count - sampleSize);
        }

        private double GetPhysicalPercentage()
        {
            RawRAMInfo raw = new RawRAMInfo();
            if (NativeMethods.GetPerformanceInfo(out raw, Marshal.SizeOf(raw)))
            {
                long av = raw.PhysicalAvailable.ToInt64() * raw.PageSize.ToInt64() / MB;
                return (double)(total - av) / total;
            }
            return 0;
        }
    }
}