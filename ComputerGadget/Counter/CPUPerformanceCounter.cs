using System.Text;

namespace ComputerGadget.Counter
{
    public sealed class CPUPerformanceCounter : BuiltinPerformanceCounter
    {
        public override string BriefMessage
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

        public CPUPerformanceCounter() : base("Processor", "% Processor Time")
        {
        }
    }
}
