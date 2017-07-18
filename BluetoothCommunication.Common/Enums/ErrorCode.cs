using System;

namespace BluetoothCommunication.Common.Enums
{
    [Flags]
    public enum ErrorCode : byte
    {
        OK = 0x00,
        UnexpectedStartByte = 0x01,
        UnexpectedStopByte = 0x02,
        UnexpectedCheckSum = 0x04,
    }
}
