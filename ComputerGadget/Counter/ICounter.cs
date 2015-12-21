using System.Collections.Generic;

namespace ComputerGadget.Counter
{
    interface ICounter
    {
        IReadOnlyList<double>[] UpdateAndGetData(int sampleSize);
        string Message { get; }
    }
}
