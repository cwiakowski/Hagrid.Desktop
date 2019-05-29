using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace Hagrid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isConnected = false;
        private string[] _ports;
        private SerialPort _port;

        public MainWindow()
        {
            InitializeComponent();
            DisableControls();
            GetAvailableComPorts();

            foreach (var port in _ports)
            {
                ComboBox1.Items.Add(port);
            }
            if (_ports[0] != null)
            {
                ComboBox1.SelectedItem = _ports[0];
            }
        }

        private void DisableControls()
        {
            Button2.IsEnabled = false;
            TextBox.IsEnabled = false;
        }

        private void GetAvailableComPorts()
        {
            _ports = SerialPort.GetPortNames();
        }

        private void Button1_Click(object sender, EventArgs a)
        {
            if (!_isConnected)
            {
                ConnectToDevice();
            }
            else
            {
                Disconnect();
            }
        }

        private void Disconnect()
        {
            _isConnected = false;
            _port.Write("#STOP\n");
            _port.Close();
            Button1.Content = "Connect";
            DisableControls();
            TextBox.Text = string.Empty;
        }

        private void ConnectToDevice()
        {
            _isConnected = true;
            string selectedPort = _ports[ComboBox1.SelectedIndex];
            _port = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
            _port.RtsEnable = true;
            _port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            _port.Open();
            _port.Write("#START\n");
            Button1.Content = "Disconnect";
            EnableControls();
        }

        private void EnableControls()
        {
            Button2.IsEnabled = true;
            TextBox.IsEnabled = true;
        }

        private void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            SendMessage(sender);
        }

        private void SendMessage(object sender)
        {
            if (_isConnected)
            {
                _port.Write($"{TextBox.Text}\n");
                TextBox2.Text += $"> {TextBox.Text}\n";
                TextBox.Text = string.Empty;
            }
            else
            {
                TextBox2.Text += $">ERROR\n";
            }
        }

        private delegate void UpdateUiTextDelegate(string text);

        private void DataReceivedHandler(
                    object sender,
                    SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), indata);
        }

        private void WriteData(string text)
        {
            TextBox2.Text += text;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SendMessage(sender);
            }
        }
    }
}
