using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        Stopwatch stopWatch = new Stopwatch();

        //Save Variables
        
        public bool save = false;
        bool exitProgram = false;
        bool stopProgram = false;

        //PackageNumber Variable
        private bool firstPackage = true;

        /// <summary>
        /// Constructor of the controller class!
        /// </summary>
        public Controller()
        {
            PrepareView();
            form1.setController(this);
            connectDialog.setController(this);
            processData = new ProcessData(54);
            UpdateStatus(this, new UpdateStatusEvent("Wilkommen zu SomnoSoftware 0.2"));
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
                serial = new SerialCommunication(this);
                serial.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(NewSerialDataRecieved);
                processData = new ProcessData(54);

                string[] portNames;
                portNames = serial.GetPortNames();
                for (int i = 0; i < portNames.Length; i++)
                {
                    UpdateStatus(this, new UpdateStatusEvent("Versuche mit " + portNames[i] + " zu verbinden"));
                    serial.Connect(portNames[i]);
                    serial.CallSensor();
                    Delay(300);
                    if (processData.sensorAnswer)
                    {
                        UpdateStatus(this, new UpdateStatusEvent("Verbunden mit " + portNames[i]));
                        firstPackage = true;
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
            form1.EnableTimer(false);
            TimeSpan time;
            time = DateTime.Now - dcTime;
            if (time.Seconds >= 4)
            {
                UpdateStatus(this, new UpdateStatusEvent("Verbindung unterbrochen, suche Sensor!"));
                //If reconnect successful 
                if (serial.Reconnect())
                {
                    ////read new packageNumber
                    //firstPackage = true;
                    ////Start new File
                    //if (save)
                    //{
                    //    EndSave();
                    //    serial.StartSensor();
                    //    StartRecording(saveDialog.patientName, DateTime.Today, 'M');
                    //}
                    //else
                    //processData = new ProcessData(54);
                    serial.StartSensor();

                    UpdateStatus(this, new UpdateStatusEvent("Verbindung wiederhergestellt!"));
                }
            }
            form1.EnableTimer(true);
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

            saveData = new SaveData(1, fileName, Statics.complexSave,this);
            saveData.addInformation("test",Statics.sensorName,birthDate,gender,name);
            save = true;
            saveDialog.Dispose();
            form1.Enabled = true;
            form1.Focus();
            form1.ChangeSaveButtonText(false);
            form1.ChangeSaveImage();
            UpdateStatus(this, new UpdateStatusEvent("Messung gestartet"));
        }

        private void EndSave()
        {
            if (save)
            {
                UpdateStatus(this, new UpdateStatusEvent("Messung beendet"));
                save = false;
                //saveData.fixSampleRate();
                saveData.commitChanges();
                //Deletes the SaveData Object
                saveData = new SaveData(this);
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

                    //Reset all Values if its the very first package that arrives
                    if (firstPackage)
                    {
                        firstPackage = false;
                        processData.SetPackageNumber();
                        stopWatch.Reset();
                        processData.packageCount=0;
                    }
                    //Check package number
                    else
                    {
                        if (!processData.CheckPackageNumber())
                        UpdateStatus(this, new UpdateStatusEvent("Paket verloren gegangen"));

                        if (!processData.CheckPackageNumber() && save)
                        {
                            saveData.FillMissingData(processData.lostPackages);
                            processData.packageCount += processData.lostPackages;
                        }
                    }

                    //Start Stop Watch the first time
                    if (!stopWatch.IsRunning){stopWatch.Reset();stopWatch.Start();}
                    
                    //Count till 2500 Packages have arrive (which should be 10 Seconds) 
                    processData.packageCount++;
                    if (processData.packageCount >= 250*10)
                    {
                        processData.packageCount -= 250*10;
                        stopWatch.Stop();
                        //Add time deviation to ensure time synchronic
                        if(save) saveData.AddLostTime(stopWatch,10);
                        //Reset and Start Stop Watch again
                        stopWatch.Reset();
                        stopWatch.Start();
                    }

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


        /// <summary>
        /// Shows dialog for recording stop
        /// </summary>
        /// <returns>Dialog result</returns>
        public void SendToTextBox(string messageBoxText)
        {
            UpdateStatus(this, new UpdateStatusEvent(messageBoxText));
        }

    }
}
