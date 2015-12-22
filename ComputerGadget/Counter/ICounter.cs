using System.Collections.Generic;

namespace ComputerGadget.Counter
{
    public interface ICounter
    {
        IReadOnlyList<double>[] UpdateAndGetData(int sampleSize);
        string Message { get; }
    }
}
