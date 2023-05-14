using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace APARControllerMaster
{
    public class APARCommands
    {
        public static List<byte> GenerateCommand(int type, int addr, double data)
        {
            switch (type)
            {
                case 0:
                    return PE44820Command(addr, data);
                case 1:
                    return PE43703Command(addr, data);
                default:
                    throw new Exception("设备类型不正确！");
            }
        }

        public static List<byte> PE44820Command(int addr, double phase)
        {
            if(phase < 0 || phase > 360) {
                throw new Exception("相位取值应当在0到360之间！");
            }
            int phase_i = (int)(phase / 1.4);
            List<byte> command = new List<byte>(2);
            command.Add((byte)addr);
            command.Add((byte)phase_i);
            return command;
        }

        public static List<byte> PE43703Command(int addr, double attenuation)
        {
            if(attenuation < 0 || attenuation > 32)
            {
                throw new Exception("衰减取值应当在0到32之间！");
            }
            int attenuation_i = (int)(attenuation / 0.25);
            List<byte> command = new List<byte>(2);
            command[0] = (byte)addr;
            command[1] = (byte)attenuation_i;
            return command;
        }
    }
}
