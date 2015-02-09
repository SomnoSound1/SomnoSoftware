using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SomnoSoftware
{
    public class SerialDataEventArgs : EventArgs
    {
        /// <summary>
        /// Byte array containing data from serial port
        /// </summary>
        public byte[] Data;

        public SerialDataEventArgs(byte[] dataInByteArray)
        {
            Data = dataInByteArray;
        }
    }
}
