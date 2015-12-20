using System.Collections.Generic;
using System.Drawing;

namespace ComputerGadget.View
{
    interface IDataViwer
    {
        void Draw(Graphics graphics, Rectangle clip, IReadOnlyList<double>[] data, string msg);
    }
}
