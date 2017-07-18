using BluetoothCommunication.Common.Enums;
using BluetoothCommunication.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;
using Windows.UI;

namespace BluetoothCommunication.Leds.LedsArray
{
    public sealed class LedArray
    {
        public static byte Length { get; private set; } = 8;
        public static byte ColorChannelCount { get; private set; } = 3;
        private Color[,] Buffer;
        private I2cDevice device;
        private byte[] pixelbyteBuffer;
        private byte[] color5BitLut;

        public LedArray(I2cDevice device)
        {
            Check.IsNull(device);
            this.device = device;

            Buffer = new Color[Length, Length];
            pixelbyteBuffer = new byte[Length * Length * ColorChannelCount + 1];

            GenerateColorLut();
        }

        public void Reset(Color color)
        {
            ResetBuffer(color);
            UpdateDevice();
        }

        private void ResetBuffer(Color color)
        {
            for(int i =0; i < Length; i++)
            {
                for (int y =0; y < Length; y++)
                {
                    Buffer[i, y] = color;
                }
            }
        }
        
        private void UpdateDevice()
        {
            Serialize();
            device.Write(pixelbyteBuffer);
        }

        private void Serialize()
        {
            int index;
            var widthStep = Length * ColorChannelCount;

            Array.Clear(pixelbyteBuffer, 0, pixelbyteBuffer.Length);
            for(int x = 0; x < Length; x++)
            {
                for(int y = 0; y < Length; y++)
                {
                    var colorByteArray = ColorByteToArray(Buffer[x, y]);
                    for(int i =0; i < ColorChannelCount; i++)
                    {
                        index = x + i * Length + y * widthStep + 1;
                        pixelbyteBuffer[index] = colorByteArray[i];
                    }
                }
            }
        }

        private byte[] ColorByteToArray(Color color)
        {
            return new byte[]
            {
                color5BitLut[color.R],
                color5BitLut[color.G],
                color5BitLut[color.B]
            };
        }

        private void GenerateColorLut()
        {
            const float maxValue5Bit = 31.0f;
            int colorLutLength = byte.MaxValue + 1;
            color5BitLut = new byte[colorLutLength];

            for(var i = 0; i < colorLutLength; i++)
            {
                var value5Bit = Math.Ceiling(i * maxValue5Bit / byte.MaxValue);
                value5Bit = Math.Min(value5Bit, maxValue5Bit);
                color5BitLut[i] = Convert.ToByte(value5Bit);
            }

        }

    }
}
