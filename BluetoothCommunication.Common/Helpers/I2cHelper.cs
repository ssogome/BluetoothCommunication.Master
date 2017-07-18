using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.Foundation;

namespace BluetoothCommunication.Common.Helpers
{
     public class I2cHelper
    {
        public static IAsyncOperation<I2cDevice> GetI2cDevice(byte address)
        {
            return GetI2cDeviceHelper(address).AsAsyncOperation();
        }

        private static async Task<I2cDevice> GetI2cDeviceHelper(byte address)
        {
            I2cDevice device = null;
            var settings = new I2cConnectionSettings(address);
            string deviceSelectorString = I2cDevice.GetDeviceSelector();
            var matchedDeviceList = await DeviceInformation.FindAllAsync(deviceSelectorString);
            if(matchedDeviceList.Count > 0)
            {
                var deviceInformation = matchedDeviceList.First();
                device = await I2cDevice.FromIdAsync(deviceInformation.Id, settings);
            }

            return device;
        }
    }
}
