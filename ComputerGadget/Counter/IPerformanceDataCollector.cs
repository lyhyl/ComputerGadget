using System.Collections.Generic;

namespace ComputerGadget.Counter
{
    public interface IPerformanceDataCollector
    {
        IReadOnlyList<double>[] UpdateAndGetData(int sampleSize);
        string BriefMessage { get; }
    }
}
