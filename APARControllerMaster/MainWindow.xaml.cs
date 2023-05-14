using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this; // register the context
            SerialList = APARSerial.GetPortList();
        }

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
                    APARSerial.OpenClosePort(PortName, 256000);
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
                APARSerial.OpenClosePort(PortName, 256000);
                this.OpenSerialButton.Content = "开启串口";
            }
        }
    }
}
