using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace BluetoothCommunication.Common.Helpers
{
    public static class SerialCommunicationHelper
    {
        private const int msDefaultTimeOut = 1000;

        public static async Task<DeviceInformationCollection> FindSerialDevices()
        {
            var defaultSelector = SerialDevice.GetDeviceSelector();

            return await DeviceInformation.FindAllAsync(defaultSelector);
        }        

        public static async Task<SerialDevice> GetFirstDeviceAvailable()
        {
            var serialDeviceCollection = await FindSerialDevices();

            var serialDeviceInformation = serialDeviceCollection.FirstOrDefault();

            if (serialDeviceInformation != null)
            {
                return await SerialDevice.FromIdAsync(serialDeviceInformation.Id);
            }
            else
            {
                return null;
            }
        }

        public static void SetDefaultConfiguration(SerialDevice serialDevice)
        {
            if (serialDevice != null)
            {
                serialDevice.WriteTimeout = TimeSpan.FromMilliseconds(msDefaultTimeOut);
                serialDevice.ReadTimeout = TimeSpan.FromMilliseconds(msDefaultTimeOut);

                serialDevice.BaudRate = 115200;
                serialDevice.Parity = SerialParity.None;
                serialDevice.DataBits = 8;
                serialDevice.Handshake = SerialHandshake.None;
                serialDevice.StopBits = SerialStopBitCount.One;
            }
        }

        public static async Task<uint> WriteBytes(SerialDevice serialDevice, byte[] commandToWrite)
        {            
            return await WriteBytes(serialDevice.OutputStream, commandToWrite);
        }

        public static async Task<byte[]> ReadBytes(SerialDevice serialDevice)
        {         
            return await ReadBytes(serialDevice.InputStream);
        }
        
        public static async Task<uint> WriteBytes(IOutputStream outputStream, byte[] commandToWrite)
        {
            Check.IsNull(outputStream);
            Check.IsNull(commandToWrite);

            uint bytesWritten = 0;

            using (var dataWriter = new DataWriter(outputStream))
            {
                dataWriter.WriteBytes(commandToWrite);
                bytesWritten = await dataWriter.StoreAsync();

                dataWriter.DetachStream();
            }

            return bytesWritten;
        }

        public static async Task<byte[]> ReadBytes(IInputStream inputStream)
        {
            Check.IsNull(inputStream);

            byte[] dataReceived = null;

            using (var dataReader = new DataReader(inputStream))
            {
                await dataReader.LoadAsync(CommandHelper.FrameLength);

                dataReceived = new byte[dataReader.UnconsumedBufferLength];

                dataReader.ReadBytes(dataReceived);

                dataReader.DetachStream();
            }

            return dataReceived;
        }
       
    }
}
