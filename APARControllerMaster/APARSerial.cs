using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace APARControllerMaster
{
    public class APARSerial
    {
        private SerialPort port;

        private List<List<byte>> sendLogs;

        public SerialPort Port
        {
            get { return port; }
            set { port = value; }
        }

        public List<string> GetPortList()
        {
            return SerialPort.GetPortNames().ToList();
        }

        public void OpenClosePort(string comName, int baudRate)
        {
            if(!Port.IsOpen)
            {
                Port.PortName = comName;
                Port.BaudRate = baudRate;
                Port.DataBits = 8;
                Port.StopBits = StopBits.One;
                Port.Parity = Parity.None;
                try
                {
                    Port.Open();
                }
                catch (IOException)
                {

                    throw;
                }

                //Port.DataReceived += new SerialDataReceivedEventHandler(ReceiveData);
            }
            else
            {
                Port.Close();
            }
        }

        public void ReceiveData(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort _port = (SerialPort)sender;
            // get the serial data
            byte[] recvData = new byte[_port.BytesToRead];
            _port.Read(recvData, 0, _port.BytesToRead);

            Debug.WriteLine("Received data: " + recvData);
        }

        public void SendData(byte[] data)
        {
            if(Port != null && Port.IsOpen)
            {
                try
                {
                    Port.Write(data, 0, data.Length);
                    sendLogs.Add(data.ToList()); // store the frame into logs.
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
