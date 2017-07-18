using BluetoothCommunication.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BluetoothCommunication.Master
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<string> diagnosticData = new ObservableCollection<string>();
        private SenseHatColor senseHatColor = new SenseHatColor();
        private StreamSocket streamSocket;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var device = await BluetoothCommunicationHelper.GetFirstPairedDeviceAvailable();
                await CloseConnection();

                streamSocket = await BluetoothCommunicationHelper.Connect(device);

                DiagnosticInfo.Display(diagnosticData, "Connected to: " + device.HostName);
            }
            catch(Exception ex)
            {
                DiagnosticInfo.Display(diagnosticData, ex.Message);
            }
        }

        private async Task CloseConnection()
        {
            if(streamSocket != null)
            {
                await streamSocket.CancelIOAsync();
                streamSocket.Dispose();
                streamSocket = null;
            }
        }

        private async void ButtonSendColor_Click(object sender, RoutedEventArgs e)
        {
            if(streamSocket != null)
            {
                var commandData = CommandHelper.PrepareLedColorCommand(senseHatColor.Brush.Color);
                await SerialCommunicationHelper.WriteBytes(streamSocket.OutputStream, commandData);
                DiagnosticInfo.Display(diagnosticData, CommandHelper.CommandToString(commandData));
            }
            else
            {
                DiagnosticInfo.Display(diagnosticData, "no active connection.");
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            diagnosticData.Clear();
        }
    }
}
