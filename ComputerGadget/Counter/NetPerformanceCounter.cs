using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ComputerGadget.Counter
{
    class NetPerformanceCounter : ICounter
    {
        private const long K = 1024 * 8;
        private const long K10 = K * 10;
        private const long K100 = K * 100;
        private const long M = K * K;
        private const long M10 = M * 10;
        private const long M100 = M * 100;
        private const long G = M * K;
        private const long G10 = G * 10;
        private const long G100 = G * 100;

        private readonly List<long> Units = new List<long>() { K, K10, K100, M, M10, M100, G, G10, G100 };
        private readonly string[] UnitsN = { "1K", "10K", "100K", "1M", "10M", "100M", "1G", "10G", "100G" };

        private NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
        private Dictionary<string, List<long>> data = new Dictionary<string, List<long>>();
        private Dictionary<string, long> oldData = new Dictionary<string, long>();
        private Dictionary<string, long> unit = new Dictionary<string, long>();
        private Dictionary<string, OperationalStatus> status = new Dictionary<string, OperationalStatus>();

        public NetPerformanceCounter()
        {
            foreach (NetworkInterface ni in interfaces)
            {
                data[ni.Name] = new List<long>();
                status[ni.Name] = ni.OperationalStatus;
                if (IsVailable(ni))
                {
                    IPv4InterfaceStatistics dat = ni.GetIPv4Statistics();
                    oldData[ni.Name] = dat.BytesReceived;
                }
                else
                    oldData[ni.Name] = 0;
            }
        }

        public int DataSize { set; get; }

        public string Message { set; get; }

        public IReadOnlyList<double>[] UpdateAndGetData()
        {
            UpdateData();
            UpdateUnit();
            UpdateMessage();
            List<IReadOnlyList<double>> dat = new List<IReadOnlyList<double>>();
            foreach (var ni in interfaces)
                if (IsVailable(ni))
                    dat.Add(data[ni.Name].ConvertAll(v => (double)v / unit[ni.Name]));

            return dat.ToArray();
        }

        private bool IsVailable(NetworkInterface ni)
        {
            switch (ni.NetworkInterfaceType)
            {
                case NetworkInterfaceType.Wireless80211:
                case NetworkInterfaceType.FastEthernetFx:
                case NetworkInterfaceType.GigabitEthernet:
                case NetworkInterfaceType.Ethernet:
                    return status[ni.Name] == OperationalStatus.Up;
                case NetworkInterfaceType.Unknown:
                case NetworkInterfaceType.Loopback:
                case NetworkInterfaceType.TokenRing:
                case NetworkInterfaceType.Fddi:
                case NetworkInterfaceType.BasicIsdn:
                case NetworkInterfaceType.PrimaryIsdn:
                case NetworkInterfaceType.Ppp:
                case NetworkInterfaceType.Ethernet3Megabit:
                case NetworkInterfaceType.Slip:
                case NetworkInterfaceType.Atm:
                case NetworkInterfaceType.GenericModem:
                case NetworkInterfaceType.FastEthernetT:
                case NetworkInterfaceType.Isdn:
                case NetworkInterfaceType.AsymmetricDsl:
                case NetworkInterfaceType.RateAdaptDsl:
                case NetworkInterfaceType.SymmetricDsl:
                case NetworkInterfaceType.VeryHighSpeedDsl:
                case NetworkInterfaceType.IPOverAtm:
                case NetworkInterfaceType.Tunnel:
                case NetworkInterfaceType.MultiRateSymmetricDsl:
                case NetworkInterfaceType.HighPerformanceSerialBus:
                case NetworkInterfaceType.Wman:
                case NetworkInterfaceType.Wwanpp:
                case NetworkInterfaceType.Wwanpp2:
                default:
                    return false;
            }
        }

        private void UpdateData()
        {
            foreach (NetworkInterface ni in interfaces)
            {
                status[ni.Name] = ni.OperationalStatus;
                if (IsVailable(ni))
                {
                    IPv4InterfaceStatistics dat = ni.GetIPv4Statistics();
                    long now = dat.BytesReceived;
                    data[ni.Name].Add(now - oldData[ni.Name]);
                    oldData[ni.Name] = now;
                }
                else
                    data[ni.Name].Add(0);
                if (data[ni.Name].Count > DataSize)
                    data[ni.Name].RemoveRange(0, data[ni.Name].Count - DataSize);
            }
        }

        private void UpdateUnit()
        {
            foreach (var s in status)
            {
                long max = data[s.Key].Max();
                for (int i = 0; i < Units.Count; i++)
                    if (Units[i] > max)
                    {
                        unit[s.Key] = Units[i];
                        break;
                    }
            }
        }

        private void UpdateMessage()
        {
            StringBuilder msg = new StringBuilder();
            foreach (var ni in interfaces)
                if (IsVailable(ni))
                {
                    msg.Append(ni.Name);
                    msg.Append(':');
                    msg.Append(UnitsN[Units.FindIndex(v => v == unit[ni.Name])]);
                    msg.Append('|');
                }
            Message = msg.ToString(0, msg.Length - 1);
        }
    }
}
