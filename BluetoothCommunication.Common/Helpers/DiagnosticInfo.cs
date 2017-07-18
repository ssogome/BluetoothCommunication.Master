using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BluetoothCommunication.Common.Helpers
{
    public static class DiagnosticInfo
    {
        private const string timeFormat = "HH:mm:fff";

        public static void Display(ICollection<string> collection, string info)
        {
            if(collection != null)
            {
                collection.Add(info);
            }
            
            DisplayDebugMessage(info);
        }

        private static void DisplayDebugMessage(string message)
        {
            string debugString = string.Format("{0} | {1}",
                DateTime.Now.ToString(timeFormat),
                message);

            Debug.WriteLine(debugString);
        }
    }
}
