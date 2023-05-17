using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace APARControllerMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {

        private APARSerial serial;

        #region Delegate and Event
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public delegate void AddSerialText(string text);
        public event AddSerialText AddTextEvent;

        public void AddText(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort _port = (SerialPort)sender;
            // get the serial data

            byte[] recvData = new byte[_port.BytesToRead];
            _port.Read(recvData, 0, _port.BytesToRead);
            string str = Encoding.ASCII.GetString(recvData);
            SerialRecvInfo += DateTime.Now.ToLongTimeString() + " " + str + "\r\n";
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this; // register the context

            serial = new APARSerial();
            serial.Port = new SerialPort();
            SerialList = serial.GetPortList(); // Get Ports Default

            this.UnitTypeComboBox.ItemsSource = new string[] { "PE44820", "PE43703" }; // set unit list.
            AddTextEvent += AddText; // Add event handler for AddTextEvent;
        }

        #region Binding Data

        private List<string> serialList;
        public List<string> SerialList
        {
            get { return serialList; }
            set
            {
                serialList = value;
                RaisePropertyChanged("SerialList");
            }
        }

        private string portName;
        public string PortName
        {
            get { return portName; }
            set
            {
                portName = value;
                RaisePropertyChanged("PortName");
            }
        }

        private string serialRecvInfo;
        public string SerialRecvInfo
        {
            get { return serialRecvInfo; }
            set
            {
                serialRecvInfo = value;
                RaisePropertyChanged(nameof(SerialRecvInfo));
            }
        }

        private DataTable unitDataTable;
        public DataTable UnitDataTable
        {
            get { return unitDataTable; }
            set
            {
                unitDataTable = value;
                RaisePropertyChanged(nameof(UnitDataTable));
            }
        }

        #endregion

        #region Click Event
        private void OpenSerialButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.OpenSerialButton.Content.Equals("开启串口"))
            {
                if (PortName == null)
                {
                    MessageBox.Show("请选择指定的串口！");
                    return;
                }
                try
                {
                    serial.OpenClosePort(PortName, 256000);
                    serial.Port.DataReceived += new SerialDataReceivedEventHandler(AddText);
                }
                catch (IOException)
                {
                    MessageBox.Show("串口无法正常打开！");
                    return;
                }
                this.OpenSerialButton.Content = "关闭串口";
            }
            else
            {
                serial.OpenClosePort(PortName, 256000);
                this.OpenSerialButton.Content = "开启串口";
            }
        }

        private void SingleInputButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.UnitTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("请选择设备类型！");
                return;
            }
            if(serial.Port == null || !serial.Port.IsOpen)
            {
                MessageBox.Show("串口尚未打开！");
                return;
            }
            string unitType = (string)this.UnitTypeComboBox.SelectedValue;
            int unitAddr = UInt16.Parse(this.UnitAddrTextBox.Text);
            double unitData = Double.Parse(this.UnitDataTextBox.Text);
            try
            {
                List<byte> command = APARCommands.GenerateCommand(unitType, unitAddr, unitData);
                byte[] frame = APARProtocol.GenerateFrame(command, APARCommands.GetUnitTypeInt(unitType));
                serial.SendData(frame);
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
                return;
            }
            
        }

        private void BatchInputButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.UnitDataTable.Rows.Count < 1)
            {
                MessageBox.Show("尚未读取任何数据！");
                return;
            }
            if (serial.Port == null || !serial.Port.IsOpen)
            {
                MessageBox.Show("串口尚未打开！");
                return;
            }

            List<DataTable> dataList = this.UnitDataTable.AsEnumerable()
                            .GroupBy(row => row.Field<string>("UnitType"))
                            .Select(g => g.CopyToDataTable())
                            .ToList();
            try
            {
                for(int i = 0; i < dataList.Count; i++)
                {
                    string type = dataList[i].Rows[0].Field<string>(0);
                    List<byte> command = APARCommands.GenerateCommand(
                        type,
                        dataList[i].AsEnumerable().Select(r => r.Field<int>("UnitAddr")).ToList(),
                        dataList[i].AsEnumerable().Select(r => r.Field<double>("UnitData")).ToList());
                    byte[] frame = APARProtocol.GenerateFrame(command, APARCommands.GetUnitTypeInt(type));
                    serial.SendData(frame);
                }
            }
            catch(Exception e1)
            {
                MessageBox.Show(e1.Message);
                return;
            }
        }

        private void ClearRecvButton_Click(object sender, RoutedEventArgs e)
        {
            this.SerialRecvInfo = "";
        }

        private void ReadCSVButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                Filter = "CSV Files(*.csv)|*.csv"
            };
            if (fileDialog.ShowDialog() == true)
            {
                try
                {
                    this.UnitDataTable = APARCommands.ReadDataFromCSV(fileDialog.FileName);
                }
                catch (Exception e1)
                {

                    MessageBox.Show(e1.Message);
                    return;
                }
            }
            
        }
        #endregion

        
    }
}
