using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;

namespace BluetoothCommunication.Common.Helpers
{
    public class BluetoothCommunicationHelper
    {
        public static async Task<DeviceInformationCollection> FindPaireDevices()
        {
            var defaultSelector = BluetoothDevice.GetDeviceSelector();
            return await DeviceInformation.FindAllAsync(defaultSelector);
        }           

        public static async Task<BluetoothDevice> GetFirstPairedDeviceAvailable()
        {
            var serialDeviceCollection = await FindPaireDevices();
            var serialDeviceInformation = serialDeviceCollection.FirstOrDefault();
            if(serialDeviceInformation != null)
            {
                return await BluetoothDevice.FromIdAsync(serialDeviceInformation.Id);
            }
            else
            {
                return null;
            }
        }

        public static async Task<StreamSocket> Connect(BluetoothDevice bluetoothDevice)
        {
            Check.IsNull(bluetoothDevice);
            var rfcommService =  bluetoothDevice.RfcommServices.FirstOrDefault();
            if(rfcommService != null)
            {
                return await ConnectToStreamSocket(bluetoothDevice, rfcommService.ConnectionServiceName);
            }
            else
            {
                throw new Exception("Selected bluetooth device does not advertise any RFCOMM service");
            }
        }

        private async static Task<StreamSocket> ConnectToStreamSocket(BluetoothDevice bluetoothDevice, string connectionStringName)
        {
            try
            {
                var streamSocket = new StreamSocket();
                await streamSocket.ConnectAsync(bluetoothDevice.HostName, connectionStringName);

                return streamSocket;
            }
            catch (Exception)
            {
                throw new Exception("Connection cannot be established. Verify that device is paired");
            }
        }
    }
}
