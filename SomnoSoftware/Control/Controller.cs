﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Timers;
using System.Windows.Forms.VisualStyles;
using SomnoSoftware.Model;
using ZedGraph;

namespace SomnoSoftware.Control
{
    public class Controller
    {
        private Connect connectDialog;
        private View form1;
        private SaveDialog saveDialog;
        private SerialCommunication serial;
        private ProcessData processData;
        private SaveData saveData;
        public event EventHandler<NewDataAvailableEvent> NewDataAvailable;
        public event EventHandler<UpdateStatusEvent> UpdateStatus;
        private DateTime dcTime;

        //Save Variables
        public bool save = false;
        bool exitProgram = false;
        bool stopProgram = false;

        /// <summary>
        /// Constructor of the controller class!
        /// </summary>
        public Controller()
        {
            PrepareView();
            form1.setController(this);
            connectDialog.setController(this);
            processData = new ProcessData(52);
            UpdateStatus(this, new UpdateStatusEvent("Wilkommen zu SomnoSoftware 0.1"));
            UpdateStatus(this, new UpdateStatusEvent("Bitte beachten Sie die Anweisungen bevor Sie eine Verbindung mit dem Sensor herstellen"));
            runner();
        }

        /// <summary>
        /// Prepares the Display/View
        /// </summary>
        public void PrepareView()
        {
            form1 = new View();
            connectDialog = new Connect();
            connectDialog.StartPosition = form1.StartPosition = FormStartPosition.CenterScreen;
            connectDialog.Show();
            //form1.Show();
        }

        /// <summary>
        /// Runs the program while it is neither stopped nor exited
        /// </summary>
        private void runner()
        {
            while (!exitProgram)
            {
                if (!stopProgram)
                {
                    Delay(30);
                }
            }
        }

        /// <summary>
        /// Delays program flow by specified time while allowing to still DoEvents
        /// </summary>
        /// <param name="time">time in milliseconds (ms) the program flow gets delayed</param>
        private void Delay(long time)
        {
            long time1 = System.Environment.TickCount;
            while ((System.Environment.TickCount - time1) < time){Thread.Sleep(1); Application.DoEvents();}
        }
        
        /// <summary>
        /// Establishes the SerialCommunication
        /// </summary>
        /// <param name="sender">Object that calls the function</param>
        /// <param name="e">Default, empty and useless event argument</param>
        public void Connect(Object sender, EventArgs e)
        {
            //Deactivate Button
            form1.ChangeConnectButtonState(false);
            connectDialog.ChangeConnectButtonState(false);

            //Searches all availible Prots for Sensor .. or disconnects
            if (!processData.sensorAnswer)
            {
                serial = new SerialCommunication();
                serial.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(NewSerialDataRecieved);
                processData = new ProcessData(52);

                string[] portNames;
                portNames = serial.GetPortNames();
                for (int i = 0; i < portNames.Length; i++)
                {
                    UpdateStatus(this, new UpdateStatusEvent("Versuche mit " + portNames[i] + " zu verbinden."));
                    serial.Connect(portNames[i]);
                    serial.CallSensor();
                    Delay(300);
                    if (processData.sensorAnswer)
                    {
                        UpdateStatus(this, new UpdateStatusEvent("Verbunden mit " + portNames[i]));
                        break;
                    }
                }
                if (processData.sensorAnswer)
                {
                    serial.StartSensor();
                    connectDialog.Hide();
                    form1.Show();
                }
                else
                {
                    UpdateStatus(this, new UpdateStatusEvent("Kein Sensor gefunden"));
                    UpdateStatus(this, new UpdateStatusEvent("Stellen Sie sicher, dass Bluetooth am Computer aktiviert ist und der Sensor eingeschaltet ist"));
                    UpdateStatus(this, new UpdateStatusEvent("Schalten Sie den Sensor aus und wieder ein und versuchen Sie es erneut"));
                }
            }
            else
            {
                if (ShowDialog("Möchten Sie die Verbindung zum Sensor wirklich schließen?", "Verbindung schließen") == DialogResult.OK)
                    Disconnect();            
            }

            //Enable/Disable Disconnect Timer
            form1.EnableTimer(processData.sensorAnswer);
            //Save Button aktivieren/deaktivieren
            form1.ChangeSaveButtonState(processData.sensorAnswer);
            //Connect Button anpassen
            form1.ChangeConnectButtonText(processData.sensorAnswer);
            //Activate Button
            form1.ChangeConnectButtonState(true);
            connectDialog.ChangeConnectButtonState(true);
        }

        /// <summary>
        /// Disconnects from the Serial Communication
        /// </summary>
        public void Disconnect()
        {
            serial.Close();
            form1.ChangeSaveButtonState(false);
            processData.sensorAnswer = false;
            UpdateStatus(this, new UpdateStatusEvent("Die Verbindung wurde getrennt"));
            form1.Hide();
            connectDialog.Show();
            connectDialog.BringToFront();
            //Finalize EDF-File and end save process
            EndSave();
        }

        /// <summary>
        /// Checks the time the last package arrived and tries to reconnect if time exceeds 3 seconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Reconnect(Object sender, EventArgs e)
        {
            form1.EnableTimer(!processData.sensorAnswer);
            TimeSpan time;
            time = DateTime.Now - dcTime;
            if (time.Seconds >= 4)
            {
                UpdateStatus(this, new UpdateStatusEvent("Verbindung unterbrochen, suche Sensor!"));
                //If reconnect successful 
                if (serial.Reconnect())
                {
                    //Replace missing Data in Save-File
                    if (save)
                    {
                        time = DateTime.Now - dcTime;
                        saveData.FillMissingData(time);
                    }
                    UpdateStatus(this, new UpdateStatusEvent("Verbindung wiederhergestellt!"));
                }
                
            }
            form1.EnableTimer(processData.sensorAnswer);
        }
        
        /// <summary>
        /// Exits the program
        /// </summary>
        /// <param name="sender">Object that calls the function</param>
        /// <param name="e">Default, empty and useless event argument</param>
        public void Exit(Object sender, FormClosingEventArgs e)
        {
            if (ShowDialog("Möchten Sie die das Programm wirklich beenden?", "Programm beenden") == DialogResult.OK)
            {
                //Finalize EDF-File and end save process
                EndSave();
                exitProgram = true;
            }
            else
                e.Cancel = true;
        }

        /// <summary>
        /// Opens up the Save Dialog or Stops Recording
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OpenSaveDialog(Object sender, EventArgs e)
        {
            if (!save)
            {
                form1.Enabled = false;
                saveDialog = new SaveDialog();
                saveDialog.StartPosition = FormStartPosition.CenterScreen;
                saveDialog.Show();
                saveDialog.Focus();
                saveDialog.setController(this);
            }
            else
            {
                if (ShowDialog("Möchten Sie die Aufnahmne wirklich beenden?", "Aufnahmen beenden") == DialogResult.OK)
                //Finalize EDF-File and end save process
                EndSave();
            }
            
        }

        /// <summary>
        /// Closes the Save Dialog if canceled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CancelDialog(Object sender, EventArgs e)
        {
            saveDialog.Dispose();
            form1.Enabled = true;
            form1.Focus();
        }

        /// <summary>
        /// Starts Recording if Save Dialog confirmed
        /// </summary>
        /// <param name="name"></param>
        /// <param name="birthDate"></param>
        /// <param name="gender"></param>
        public void StartRecording(string name, DateTime birthDate, char gender)
        {
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string subPath = Path.Combine("Messungen", name);
            string fileName = DateTime.Now.ToString("yyyy_MM_dd") + "_" + DateTime.Now.ToLongTimeString() + ".edf";
            subPath = Path.Combine(documents, subPath);
            fileName = fileName.Replace(':', '_');
            fileName = Path.Combine(subPath, fileName);
            if (!Directory.Exists(subPath)) Directory.CreateDirectory(subPath);

            saveData = new SaveData(1, fileName, Statics.complexSave);
            saveData.addInformation("test","",birthDate,gender,name);
            save = true;
            saveDialog.Dispose();
            form1.Enabled = true;
            form1.Focus();
            form1.ChangeSaveButtonText(false);
            form1.ChangeSaveImage();
        }

        private void EndSave()
        {
            if (save)
            {
                UpdateStatus(this, new UpdateStatusEvent("Messung beendet"));
                save = false;
                saveData.commitChanges();
                //Deletes the SaveData Object
                saveData = new SaveData();
                form1.ChangeSaveButtonText(true);
                form1.ChangeSaveImage();
            }
        }

        /// <summary>
        /// New serial data is received and gets processed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            //Reset Timer for disconnect
            dcTime = DateTime.Now;

            foreach (byte t in e.Data)
            {
                //Import Byte and convert if one package complete
                if (processData.ImportByte(t))
                {
                    processData.Convert2Byte();
                    
                    //IMU
                    processData.CalculateIMU();

                    //FFT
                    if (processData.Buffering())
                    {
                        // Send data to Form
                        if (NewDataAvailable != null)
                            NewDataAvailable(this, new NewDataAvailableEvent(processData.audioArray,processData.fft,processData.activity,processData.sleepPosition));
                    }

                    //Save Data
                    if (save)
                    {
                        if (Statics.complexSave)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                saveData.sendData(i + 3, processData.gyro[i]);
                                saveData.sendData(i + 6, processData.accelerationRaw[i]);
                            }
                        }
                        saveData.sendData(1, (short)processData.activity);
                        saveData.sendData(2, (short)processData.sleepPosition);
                        saveData.sendData(0, processData.audio);
                    }
                }
            }

           
        }

        /// <summary>
        /// Shows dialog for recording stop
        /// </summary>
        /// <returns>Dialog result</returns>
        private DialogResult ShowDialog(string messageBoxText, string caption)
        {
            MessageBoxButtons b = MessageBoxButtons.OKCancel;
            MessageBoxIcon icon = MessageBoxIcon.Warning;
            MessageBoxDefaultButton d = MessageBoxDefaultButton.Button2;
            DialogResult result = MessageBox.Show(messageBoxText, caption, b, icon, d);
            return result;
        }
    }
}
