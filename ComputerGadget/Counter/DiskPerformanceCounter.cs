using System.Text;

namespace ComputerGadget.Counter
{
    public sealed class DiskPerformanceCounter : BuiltinPerformanceCounter
    {
        public override string BriefMessage
        {
            get
            {
                if (data.Count == 0)
                    return "Disk data not available";
                StringBuilder msg = new StringBuilder();
                for (int i = 0; i < instances.Length; i++)
                    if (IsCore(instances[i]))
                    {
                        msg.Append(instances[i]);
                        msg.Append('|');
                    }
                return msg.ToString(0, msg.Length - 1);
            }
        }

        public DiskPerformanceCounter() : base("PhysicalDisk", "% Disk Time")
        {
        }
    }
}
