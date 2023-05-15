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
            PE44820 = 0,
            PE43703
        };

        public static List<byte> GenerateCommand(int type, int addr, double data)
        {
            switch (type)
            {
                case (int)UnitType.PE44820:
                    return PE44820Command(addr, data);
                case (int)UnitType.PE43703:
                    return PE43703Command(addr, data);
                default:
                    throw new Exception("设备类型不正确！");
            }
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

                dt.Columns.Add(new DataColumn("UnitType"));
                dt.Columns.Add(new DataColumn("UnitAddr"));
                dt.Columns.Add(new DataColumn("UnitData"));

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
            List<byte> command = new List<byte>(2);
            command[0] = (byte)addr;
            command[1] = (byte)attenuation_i;
            return command;
        }
    }
}
