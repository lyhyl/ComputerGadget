using System;
using System.IO;
using System.Xml;

namespace ComputerGadget
{
    class Config
    {
        [Flags]
        public enum Side { Top = 1, Bottom = 2, Left = 4, Right = 8 }

        public Side Position { set; get; } = Side.Right | Side.Bottom;
        public float FontSize { set; get; } = 6;

        public Config()
        {
            using (Stream stream = File.Open(".\\cfg.xml", FileMode.OpenOrCreate))
            {
                try
                {
                    ReadProperties(stream);
                }
                catch (Exception)
                {
                    CreateDefaultConfigFile(stream);
                }
            }
        }

        private void ReadProperties(Stream stream)
        {
            using (XmlReader cfgFile = XmlReader.Create(stream))
            {
                cfgFile.Read();
                ReadInProperties(cfgFile);
            }
        }

        private void CreateDefaultConfigFile(Stream stream)
        {
            stream.SetLength(0);
            using (XmlWriter ou = XmlWriter.Create(stream))
            {
                ou.WriteStartElement("root");
                ou.WriteElementString("position", "10");
                ou.WriteElementString("font-size", "6");
                ou.WriteEndElement();
            }
        }

        private void ReadInProperties(XmlReader cfgFile)
        {
            cfgFile.ReadStartElement();
            cfgFile.ReadStartElement();
            Position = (Side)cfgFile.ReadContentAsInt();
            cfgFile.ReadEndElement();
            cfgFile.ReadStartElement();
            FontSize = cfgFile.ReadContentAsFloat();
            cfgFile.ReadEndElement();
            cfgFile.ReadEndElement();
        }
    }
}
