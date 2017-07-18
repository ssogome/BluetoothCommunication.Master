using BluetoothCommunication.Common.Enums;
using System;
using Windows.UI;

namespace BluetoothCommunication.Common.Helpers
{
    public static class CommandHelper
    {
        public static byte FrameLength { get; } = 16;
        public static byte CommandIdIndex { get; } = 1;
        public static byte CommandDataBeginIndex { get; } = 2;

        public static byte StartByte { get; } = 0xAA;
        public static byte StopByte { get; } = 0xBB;

        private static byte startByteIndex = 0;
        private static int errorByteIndex = FrameLength - 3;
        private static int checkSumIndex = FrameLength - 2;
        private static int stopByteIndex = FrameLength - 1;

        public static byte[] PrepareSetFrequencyCommand(double hzBlinkFrequency)
        {
            // Prepare command
            var command = PrepareCommandStructure(CommandId.BlinkingFrequency);

            // Set command data
            var commandData = BitConverter.GetBytes(hzBlinkFrequency);
            Array.Copy(commandData, 0, command, CommandDataBeginIndex, commandData.Length);

            // ... and checksum
            SetChecksum(command);

            return command;
        }

        public static byte[] PrepareBlinkingStatusCommand(bool? isBlinking)
        {
            // Prepare command
            var command = PrepareCommandStructure(CommandId.BlinkingStatus);

            // Set command data
            command[CommandDataBeginIndex] = Convert.ToByte(isBlinking);

            // ... and checksum
            SetChecksum(command);

            return command;
        }

        public static byte[] PrepareLedColorCommand(Color color)
        {
            // Prepare command
            var command = PrepareCommandStructure(CommandId.LedColor);

            // Set command data
            command[CommandDataBeginIndex] = color.R;
            command[CommandDataBeginIndex + 1] = color.G;
            command[CommandDataBeginIndex + 2] = color.B;

            // ... and checksum
            SetChecksum(command);

            return command;
        }

        public static string CommandToString(byte[] commandData)
        {
            string commandString = string.Empty;

            if (commandData != null)
            {
                foreach (byte b in commandData)
                {
                    commandString += " " + b;
                }
            }

            return commandString.Trim();
        }

        public static ErrorCode VerifyCommand(byte[] command)
        {
            Check.IsNull(command);
            Check.IsLengthEqualTo(command.Length, FrameLength);

            var errorCode = ErrorCode.OK;

            var actualChecksum = command[checkSumIndex];
            var expectedChecksum = CalculateChecksum(command);

            errorCode = VerifyCommandByte(actualChecksum, expectedChecksum,
                ErrorCode.UnexpectedCheckSum);

            errorCode = VerifyCommandByte(command[startByteIndex], StartByte,
                ErrorCode.UnexpectedStartByte | errorCode);

            errorCode = VerifyCommandByte(command[startByteIndex], StartByte,
                ErrorCode.UnexpectedStopByte | errorCode);

            return errorCode;
        }

        private static ErrorCode VerifyCommandByte(byte actualValue, byte expectedValue, ErrorCode errorToSet)
        {
            var errorCode = ErrorCode.OK;

            if (actualValue != expectedValue)
            {
                errorCode = errorToSet;
            }

            return errorCode;
        }

        private static byte[] PrepareCommandStructure(CommandId commandId)
        {
            var command = new byte[FrameLength];

            command[startByteIndex] = StartByte;
            command[CommandIdIndex] = (byte)commandId;
            command[stopByteIndex] = StopByte;

            return command;
        }

        private static void SetChecksum(byte[] command)
        {
            command[checkSumIndex] = CalculateChecksum(command);
        }

        private static byte CalculateChecksum(byte[] command)
        {
            long sum = 0;

            for (int i = 0; i < FrameLength; i++)
            {
                if (i != checkSumIndex)
                {
                    sum += command[i];
                }
            }

            return (byte)(sum % byte.MaxValue);
        }
    }
}