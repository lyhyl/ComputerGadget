﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ComputerGadget.Counter
{
    class NetPerformanceCounter : ICounter
    {
        private const long K = 1024;
        private const long K5 = K * 5;
        private const long K10 = K * 10;
        private const long K50 = K * 50;
        private const long K100 = K * 100;
        private const long M = K * K;
        private const long M5 = M * 5;
        private const long M10 = M * 10;
        private const long M50 = M * 10;
        private const long M100 = M * 100;
        private const long G = M * K;
        private const long G5 = G * 5;
        private const long G10 = G * 10;
        private const long G50 = G * 50;
        private const long G100 = G * 100;

        private readonly List<long> Units = new List<long>() {
            K, K5, K10, K50, K100,
            M, M5, M10, M50, M100,
            G, G5, G10, G50, G100
        };
        private readonly string[] UnitsN = {
            "1K", "5K", "10K", "50K", "100K",
            "1M", "5M", "10M", "50M", "100M",
            "1G", "5G", "10G", "50G", "100G"
        };

        private NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
        private Dictionary<string, List<double>> data = new Dictionary<string, List<double>>();
        private Dictionary<string, long> oldData = new Dictionary<string, long>();
        private Dictionary<string, long> limitUnit = new Dictionary<string, long>();
        private Dictionary<string, OperationalStatus> status = new Dictionary<string, OperationalStatus>();
        private Dictionary<string, bool> available = new Dictionary<string, bool>();
        private long lastTime;

        public NetPerformanceCounter()
        {
            foreach (NetworkInterface ni in interfaces)
            {
                data[ni.Name] = new List<double>();
                status[ni.Name] = ni.OperationalStatus;
                available[ni.Name] = IsVailable(ni);
                if (available[ni.Name])
                {
                    IPv4InterfaceStatistics dat = ni.GetIPv4Statistics();
                    oldData[ni.Name] = dat.BytesReceived;
                }
                else
                    oldData[ni.Name] = 0;
            }
            lastTime = DateTime.Now.Ticks;
        }

        public string Message { set; get; }

        public IReadOnlyList<double>[] UpdateAndGetData(int sampleSize)
        {
            UpdateAvailable();
            UpdateData((int)Math.Ceiling((double)sampleSize/AvailableCount()));
            UpdateUnit();
            UpdateMessage();

            List<IReadOnlyList<double>> dat = new List<IReadOnlyList<double>>();
            foreach (var ni in interfaces)
                if (available[ni.Name])
                    dat.Add(data[ni.Name].ConvertAll(v => v / limitUnit[ni.Name]));
            return dat.ToArray();
        }

        private int AvailableCount()
        {
            int cnt = 0;
            foreach (var a in available)
                if (a.Value)
                    cnt++;
            return cnt;
        }

        private void UpdateAvailable()
        {
            foreach (NetworkInterface ni in interfaces)
                available[ni.Name] = IsVailable(ni);
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

        private void UpdateData(int sampleSize)
        {
            long now = DateTime.Now.Ticks;
            double dt = (now - lastTime) / 10000000.0;
            foreach (NetworkInterface ni in interfaces)
            {
                status[ni.Name] = ni.OperationalStatus;
                if (available[ni.Name])
                {
                    IPv4InterfaceStatistics dat = ni.GetIPv4Statistics();
                    long nowBytes = dat.BytesReceived;
                    double cdata = ((double)nowBytes - oldData[ni.Name]) / dt;
                    data[ni.Name].Add(cdata);
                    oldData[ni.Name] = nowBytes;
                }
                else
                    data[ni.Name].Add(0);
                if (data[ni.Name].Count > sampleSize)
                    data[ni.Name].RemoveRange(0, data[ni.Name].Count - sampleSize);
            }
            lastTime = now;
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
            StringBuilder msg = new StringBuilder();
            foreach (var ni in interfaces)
                if (available[ni.Name])
                {
                    msg.Append(ni.Name);
                    msg.Append(':');
                    msg.Append(UnitsN[Units.FindIndex(v => v == limitUnit[ni.Name])]);
                    msg.Append('|');
                }
            Message = msg.ToString(0, msg.Length - 1);
        }
    }
}
