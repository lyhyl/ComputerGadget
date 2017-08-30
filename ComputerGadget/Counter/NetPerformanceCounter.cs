using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ComputerGadget.Counter
{
    public sealed class NetPerformanceCounter : IPerformanceDataCollector
    {
        private const long K = 1024;
        private const long K5 = K * 5;
        private const long K10 = K * 10;
        private const long K50 = K * 50;
        private const long K100 = K * 100;
        private const long K500 = K * 500;
        private const long M = K * K;
        private const long M5 = M * 5;
        private const long M10 = M * 10;
        private const long M50 = M * 10;
        private const long M100 = M * 100;
        private const long M500 = M * 500;
        private const long G = M * K;
        private const long G5 = G * 5;
        private const long G10 = G * 10;
        private const long G50 = G * 50;
        private const long G100 = G * 100;
        private const long G500 = G * 500;

        private readonly List<long> Units = new List<long>() {
            K, K5, K10, K50, K100, K500,
            M, M5, M10, M50, M100, M500,
            G, G5, G10, G50, G100, G500
        };
        private readonly string[] UnitsName = {
            "1K", "5K", "10K", "50K", "100K", "500K",
            "1M", "5M", "10M", "50M", "100M", "500M",
            "1G", "5G", "10G", "50G", "100G", "500G"
        };

        private NetworkInterface[] interfaces;
        private Dictionary<string, List<double>> data = new Dictionary<string, List<double>>();
        private Dictionary<string, long> oldData = new Dictionary<string, long>();
        private Dictionary<string, long> limitUnit = new Dictionary<string, long>();
        private Dictionary<string, OperationalStatus> status = new Dictionary<string, OperationalStatus>();
        private Dictionary<string, bool> available = new Dictionary<string, bool>();
        private long lastTime;

        public NetPerformanceCounter()
        {
            lastTime = DateTime.Now.Ticks;
        }

        public string BriefMessage { set; get; }

        public IReadOnlyList<double>[] UpdateAndGetData(int sampleSize)
        {
            UpdateAvailable();
            int availableCount = AvailableCount();
            int subsampleSize = (int)Math.Ceiling((double)sampleSize / Math.Max(1, availableCount));
            UpdateData(subsampleSize);
            UpdateUnit();
            UpdateMessage();

            List<IReadOnlyList<double>> dat = new List<IReadOnlyList<double>>();
            foreach (var ni in interfaces)
                if (available[ni.Name])
                    dat.Add(data[ni.Name].ConvertAll((v) => {
                        //Debug.WriteLine($"Net {v / limitUnit[ni.Name]}");
                        return v / limitUnit[ni.Name];
                    }));
            return dat.ToArray();
        }

        private void UpdateAvailable()
        {
            interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                status[ni.Name] = ni.OperationalStatus;
                available[ni.Name] = IsVailable(ni);
                if (!data.ContainsKey(ni.Name))
                {
                    data[ni.Name] = new List<double>();
                    oldData[ni.Name] = GetReceivedBytes(ni);
                }
            }
        }

        private void UpdateData(int sampleSize)
        {
            long now = DateTime.Now.Ticks;
            double dt = (now - lastTime) / 10000000.0;
            lastTime = now;
            foreach (NetworkInterface ni in interfaces)
            {
                if (available[ni.Name])
                {
                    long nowBytes = ni.GetIPv4Statistics().BytesReceived;
                    double cdata = (nowBytes - oldData[ni.Name]) / dt;
                    data[ni.Name].Add(cdata);
                    oldData[ni.Name] = nowBytes;
                }
                else
                    data[ni.Name].Add(0);
                if (data[ni.Name].Count > sampleSize)
                    data[ni.Name].RemoveRange(0, data[ni.Name].Count - sampleSize);
            }
        }

        private void UpdateUnit()
        {
            foreach (var s in status)
            {
                double max = data[s.Key].Max();
                for (int i = 0; i < Units.Count; i++)
                    if (Units[i] > max)
                    {
                        limitUnit[s.Key] = Units[i];
                        break;
                    }
            }
        }

        private void UpdateMessage()
        {
            if (AvailableCount() == 0)
                BriefMessage = "No network avaliable";
            else
            {
                StringBuilder msg = new StringBuilder();
                foreach (var ni in interfaces)
                    if (available[ni.Name])
                    {
                        msg.Append(ni.Name);
                        msg.Append(':');
                        msg.Append(UnitsName[Units.FindIndex(v => v == limitUnit[ni.Name])]);
                        msg.Append('|');
                    }
                BriefMessage = msg.ToString(0, Math.Max(0, msg.Length - 1));
            }
        }

        private int AvailableCount()
        {
            int cnt = 0;
            foreach (var a in available)
                if (a.Value)
                    cnt++;
            return cnt;
        }

        private long GetReceivedBytes(NetworkInterface ni)
        {
            if (available[ni.Name])
                return ni.GetIPv4Statistics().BytesReceived;
            else
                return 0;
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
    }
}
