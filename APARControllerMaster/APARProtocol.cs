using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace APARControllerMaster
{
    public class APARProtocol
    {

        public static byte[] GenerateFrame(byte[] content, byte frameType)
        {
            List<byte> frame = new List<byte>();
            frame.AddRange(Encoding.ASCII.GetBytes("BI"));
            frame.Add(frameType);
            frame.AddRange(UInt16ToBytes((UInt16)content.Length));
            frame.AddRange(content);
            frame.Add(CRC8Maxim(frame));
            frame.AddRange(Encoding.ASCII.GetBytes("T"));
            return frame.ToArray();
        }

        private static byte[] UInt16ToBytes(UInt16 num)
        {
            byte[] bytes = new byte[2];
            bytes[0] = Convert.ToByte(num & 0x00FF);
            bytes[1] = Convert.ToByte((num & 0xFF00) >> 8);
            return bytes;
        }

        public static byte CRC8Maxim(List<byte> data)
        {
            int length = data.Count;
            byte i;
            byte crc = 0;
            for(int j = 2; j < length; j++)
            {
                crc ^= data[j];
                for(i = 0; i < 8; i++)
                {
                    if((crc & 1) == 1)
                    {
                        crc = (byte)((crc >> 1) ^ 0X8C);
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return crc;
        }
    }
}
