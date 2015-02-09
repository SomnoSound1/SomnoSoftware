using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Timers;
using SomnoSoftware.Model;
using ZedGraph;

namespace SomnoSoftware.Control
{
    public class Controller
    {
        private View form1;
        private SaveDialog saveDialog;
        private SerialCommunication serial;
        private ProcessData processData;
        private SaveData saveData;
        public event EventHandler<NewDataAvailableEvent> NewDataAvailable;
        public event EventHandler<UpdateStatusEvent> UpdateStatus; 

        //Save Variables
        private const int sampleRate = 4890;
        private bool save = false;

        bool exitProgram = false;
        bool stopProgram = false;

        /// <summary>
        /// Constructor of the controller class
        /// </summary>
        public Controller()
        {
            PrepareView();
            form1.setController(this);
            processData = new ProcessData(52);
            UpdateStatus(this, new UpdateStatusEvent("Press to Connect to Sensor"));
            runner();
        }

        /// <summary>
        /// Prepares the Display/View
        /// </summary>
        public void PrepareView()
        {
            form1 = new View();
            form1.Show();
        }

        /// <summary>
        /// Runs the program while it is neither stopped nor exited
        /// </summary>
        private void runner()
        {
            while (!exitProgram)
            {
                if (!stopProgram)
                    //model.Tick(currentDirectionChange);
                Delay(30);
            }
        }

        /// <summary>
        /// Delays program flow by specified time while allowing to still DoEvents
        /// </summary>
        /// <param name="time">time in milliseconds (ms) the program flow gets delayed</param>
        private void Delay(long time)
        {
            long time1 = System.Environment.TickCount;
            while ((System.Environment.TickCount - time1) < time) Application.DoEvents();
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
                    UpdateStatus(this, new UpdateStatusEvent("Try to Connect to " + portNames[i]));
                    serial.Connect(portNames[i]);
                    serial.CallSensor();
                    Delay(300);
                    if (processData.sensorAnswer)
                    {
                        UpdateStatus(this, new UpdateStatusEvent("Connected to " + portNames[i]));
                        break;
                    }
                }
                if (processData.sensorAnswer)
                    serial.StartSensor();
                else
                    UpdateStatus(this, new UpdateStatusEvent("No Sensor found"));
            }
            else
            Disconnect();

            //Save Button aktivieren/deaktivieren
            form1.ChangeSaveButtonState(processData.sensorAnswer);
            //Connect Button anpassen
            form1.ChangeConnectButtonText(processData.sensorAnswer);
            //Activate Button
            form1.ChangeConnectButtonState(true);
        }

        /// <summary>
        /// Disconnects from the Serial Communication
        /// </summary>
        public void Disconnect()
        {
            serial.Close();
            form1.ChangeSaveButtonState(false);
            processData.sensorAnswer = false;
            UpdateStatus(this, new UpdateStatusEvent("Disconnected"));
            //Finalize EDF-File and end save process
            if(save)
            EndSave();
        }

        /// <summary>
        /// Exits the program
        /// </summary>
        /// <param name="sender">Object that calls the function</param>
        /// <param name="e">Default, empty and useless event argument</param>
        public void Exit(Object sender, EventArgs e)
        {
            //Finalize EDF-File and end save process
            if(save)
            EndSave();
            exitProgram = true;
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
                form1.Hide();
                saveDialog = new SaveDialog();
                saveDialog.Show();
                saveDialog.setController(this);
            }
            else
            {
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
            form1.Show();
        }

        /// <summary>
        /// Starts Recording if Save Dialog confirmed
        /// </summary>
        /// <param name="name"></param>
        /// <param name="birthDate"></param>
        /// <param name="gender"></param>
        public void StartRecording(string name, DateTime birthDate, char gender)
        {
            saveData = new SaveData(1,1,"output.edf");
            saveData.addSignal(0, "audio", "amplitude", sampleRate, 1024, 0);
            saveData.addInformation("test","",birthDate,gender,name);
            save = true;
            saveDialog.Dispose();
            form1.Show();
            form1.ChangeSaveButtonText(false);
        }

        private void EndSave()
        {
            save = false;
            saveData.commitChanges();
            //Deletes the SaveData Object
            saveData = new SaveData();
            form1.ChangeSaveButtonText(true);
        }

        /// <summary>
        /// New serial data is received and gets processed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            foreach (byte t in e.Data)
            {
                //Import Byte and convert if one package complete
                if (processData.ImportByte(t))
                {
                    processData.Convert2Byte();

                    if (processData.Buffering())
                    {
                        // Send data to Form
                        if (NewDataAvailable != null)
                            NewDataAvailable(this, new NewDataAvailableEvent(processData.audioArray,processData.fft));
                    }
                    //Save Data
                    if (save)
                    saveData.sendData(0,processData.audio);
                }
            }

           
        }




    }
}
