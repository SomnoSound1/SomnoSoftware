using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using SomnoSoftware.Control;

namespace SomnoSoftware.Model
{
    class SerialCommunication
    {
        private SerialPort serialPort;
        public event EventHandler<SerialDataEventArgs> NewSerialDataRecieved;
        private Controller controller;

        public SerialCommunication(Controller controller)
        {
            this.controller = controller;
            serialPort = new SerialPort();
        }
        
        /// <summary>
        /// Gets all availible Port Names sorting them (descending)
        /// </summary>
        /// <returns>String-Array with sorted Port Names</returns>
        public string[] GetPortNames()
        {
            string[] portNames = SerialPort.GetPortNames();
            Array.Sort(portNames);
            Array.Reverse(portNames);
            return portNames;
        }

        /// <summary>
        /// Connects to a serial port
        /// </summary>
        /// <param name="portName">Name of the Port (example: "COM1")</param>
        public void Connect(string portName)
        {
            // Closing serial port if it is open
            if (serialPort != null && serialPort.IsOpen)
                serialPort.Close();

            serialPort.ReadTimeout = 500;
            serialPort.WriteTimeout = 500;
            serialPort.PortName = portName;

            // Subscribe to event and open serial port for data
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
            try
            {
                serialPort.Open();
                CheatBaudRate(230400);
            }
            catch
            {
                controller.SendToTextBox("Fehler: " + portName + " konnte nicht geöffnet werden");
            }

        }

        /// <summary>
        /// Sends init Message to Bluetooth to check if Port is correct
        /// </summary>
        public void CallSensor()
        {
            if (serialPort != null && serialPort.IsOpen)
                SendMessage(5);
        }

        /// <summary>
        /// Sends Message to Bluetooth to Start Sensor
        /// </summary>
        public void StartSensor()
        {
            if (serialPort != null && serialPort.IsOpen)
                SendMessage(3);
        }

        /// <summary>
        /// Closes the serial port
        /// </summary>
        public void Close()
        {
            serialPort.Close();
        }

        /// <summary>
        /// Event gets called whenever Data is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (serialPort.IsOpen)
            {
                int dataLength = serialPort.BytesToRead;
                byte[] data = new byte[dataLength];
                int nbrDataRead = serialPort.Read(data, 0, dataLength);
                if (nbrDataRead == 0)
                    return;

                // Send data to whom ever interested
                if (NewSerialDataRecieved != null)
                    NewSerialDataRecieved(this, new SerialDataEventArgs(data));
            }
            }
            catch (Exception)
            {
                controller.SendToTextBox("Fehler: Störung während des Datenemfpangs");
            }
        }


        /// <summary>
        /// Method to increase BaudRate because System.IO.Ports.SerialPort doesn't support anything above 115200
        /// </summary>
        /// <param name="br">Baudrate</param>
        private void CheatBaudRate(int br) //C#
        {
            //method to set System.IO.Ports.SerialPort baudrate to greater than 115200. For this to work, your hardware must support these speeds
            //Written by Nathan Harward
            if (br <= 115200)
                serialPort.BaudRate = br;
            else
            {
                //serialPort.BaudRate = br; //can't do this cause System.IO.Ports.SerialPort doesn't support anything above 115200

                //using reflections method to change baud rate
                //port has to have been already opened

                //Microsoft.Win32.UnsafeNativeMethods.COMMPROP //http://msdn.microsoft.com/en-us/library/windows/desktop/aa363189%28v=VS.85%29.aspx
                object commpropObj = serialPort.BaseStream.GetType().GetField("commProp", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(serialPort.BaseStream);
                commpropObj.GetType().GetField("dwMaxBaud", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(commpropObj, (Int32)br);
                Int32 dwmaxbaud = (Int32)commpropObj.GetType().GetField("dwMaxBaud", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(commpropObj);
                if (dwmaxbaud != br)
                    throw new ApplicationException("Error setting commProp.dwMaxBaud. Hardware does not support this baud rate?");
                //Microsoft.Win32.UnsafeNativeMethods.DCB //http://msdn.microsoft.com/en-us/library/windows/desktop/aa363214%28v=VS.85%29.aspx
                object dcbObj = serialPort.BaseStream.GetType().GetField("dcb", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(serialPort.BaseStream);
                dcbObj.GetType().GetField("BaudRate", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(dcbObj, (UInt32)br);
                UInt32 baudr = (UInt32)dcbObj.GetType().GetField("BaudRate", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(dcbObj);
                if (baudr != br)
                    controller.SendToTextBox("Fehler: Hardware unterstüzt BaudRate nicht");
            }
        }

        /// <summary>
        /// Sends a DataPackage declared as Message over Bluetooth
        /// </summary>
        /// <param name="message_">Message to send</param>
        public void SendMessage(int message_)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    byte[] message = new byte[1];
                    message[0] = (byte) 170;
                    serialPort.Write(message, 0, message.Length);
                    message[0] = (byte) 128;
                    serialPort.Write(message, 0, message.Length);
                    message[0] = (byte) message_;
                    serialPort.Write(message, 0, message.Length);
                    message[0] = (byte) (128 + message_);
                    serialPort.Write(message, 0, message.Length);
                    message[0] = (byte) 171;
                    serialPort.Write(message, 0, message.Length);
                }
                catch
                {
                    controller.SendToTextBox("Fehler: Konnte nicht an COM-Port senden");
                }
            }

        }

        public bool Reconnect()
        {
            Connect(serialPort.PortName);
            return (serialPort.IsOpen);
        }
    }
}
