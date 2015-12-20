using System.Collections.Generic;

namespace ComputerGadget.Counter
{
    interface ICounter
    {
        IReadOnlyList<double>[] UpdateAndGetData();
        int DataSize { get; set; }
        string Message { get; }
    }
}
