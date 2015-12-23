using ComputerGadget.Counter;
using System.Drawing;

namespace ComputerGadget.View
{
    public interface IDataViwer
    {
        void Draw(Graphics graphics, Rectangle clip, IPerformanceDataCollector counter);
        Theme Theme { set; get; }
    }
}
