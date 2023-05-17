using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace APARControllerMaster
{
    public class APARCommands
    {
        private enum UnitType
        {
            PE44820 = 2,
            PE43703 = 3
        };

        private static Dictionary<int, string> StatusDict = new Dictionary<int, string>()
        {
            { 1, "[INFO] ok" },
            { 2, "[INFO] parse ok"},
            { 3, "[INFO] exec ok" },
            { 128, "[ERROR] parse error" },
            { 129, "[ERROR] exec error" },
            { 130, "[ERROR] param out of bound" },
            { 131, "[ERROR] addr out of bound" },
            { 132, "[ERROR] gpio write error" }
        };

        public static string GetStatusMsg(int code)
        {
            if (StatusDict.Keys.Contains(code))
            {
                return StatusDict[code];
            }
            else
            {
                return "[UNDEFINED] undefined code";
            }
        }

        public static int GetUnitTypeInt(string unitTypeStr)
        {
            UnitType type;
            try
            {
                type = (UnitType)(Enum.Parse(typeof(UnitType), unitTypeStr));
            }
            catch
            {
                throw new Exception("传入的设备类型不存在！");
            }
            return (int)type;
        }

        public static List<byte> GenerateCommand(string type, int addr, double data)
        {
            switch (type)
            {
                case nameof(UnitType.PE44820):
                    return PE44820Command(addr, data);
                case nameof(UnitType.PE43703):
                    return PE43703Command(addr, data);
                default:
                    throw new Exception("设备类型不正确！");
            }
        }

        public static List<byte> GenerateCommand(string type, List<int> addrs, List<double> datas)
        {
            var commands = new List<byte>();
            switch (type)
            {
                case nameof(UnitType.PE44820):
                    for(int i = 0; i < addrs.Count; i++)
                    {
                        commands.AddRange(PE44820Command(addrs[i], datas[i]));
                    }
                    break;
                case nameof(UnitType.PE43703):
                    for (int i = 0; i < addrs.Count; i++)
                    {
                        commands.AddRange(PE43703Command(addrs[i], datas[i]));
                    }
                    break;
                default:
                    throw new Exception("设备类型不正确！");
            }
            return commands;
        }

        public static DataTable ReadDataFromCSV(string filepath)
        {
            if(File.Exists(filepath))
            {
                DataTable dt = new DataTable();
                FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);

                string strLine = "";
                string[] arrLine; // str split to arr

                dt.Columns.Add(new DataColumn("UnitType",typeof(string)));
                dt.Columns.Add(new DataColumn("UnitAddr",typeof(int)));
                dt.Columns.Add(new DataColumn("UnitData",typeof(double)));

                while ((strLine = sr.ReadLine()) != null)
                {
                    arrLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    // add data
                    for(int i = 0;i < 3; i++)
                    {
                        dr[i] = arrLine[i];
                    }
                    dt.Rows.Add(dr);
                }

                sr.Close();
                fs.Close();

                dt.DefaultView.Sort = "UnitType DESC";

                return dt;
            }
            else
            {
                throw new Exception("选取的文件路径不存在！");
            }
        }

        private static List<byte> PE44820Command(int addr, double phase)
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

        private static List<byte> PE43703Command(int addr, double attenuation)
        {
            if(attenuation < 0 || attenuation > 32)
            {
                throw new Exception("衰减取值应当在0到32之间！");
            }
            int attenuation_i = (int)(attenuation / 0.25);
            List<byte> command = new List<byte>
            {
                (byte)addr,
                (byte)attenuation_i
            };
            return command;
        }
    }
}
