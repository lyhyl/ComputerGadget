using ComputerGadget.Counter;
using System.Drawing;

namespace ComputerGadget.View
{
    interface IDataViwer
    {
        void Draw(Graphics graphics, Rectangle clip, ICounter counter);
    }
}
