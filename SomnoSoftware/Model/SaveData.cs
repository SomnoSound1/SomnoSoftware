using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NeuroLoopGainLibrary;
using NeuroLoopGainLibrary.Edf;
using SomnoSoftware.Control;

namespace SomnoSoftware.Model
{
    class SaveData
    {
        private EdfFile edfFile;
        private int nrSignals = 0;
        private long lostTime = 0;
        private Int32 dataBlockNr = 0;
        private DateTime saveStart;
        private List<Int16>[] buffer;
        private Controller controller;
        private List<Int16> storedDataAudio = new List<short>();
        private List<Int16> storedDataPos = new List<short>();

        public SaveData(Controller controller)
        {
            this.controller = controller;
        }

        public SaveData(int sampleDuration, string fileName, bool complex, Controller controller)
        {
            this.controller = controller;
            if (complex)
                nrSignals = 9;
            else
                nrSignals = 3;

            edfFile = new EdfFile(fileName, false, false, false, false);
            edfFile.CreateNewFile(nrSignals, true);
            
            edfFile.FileInfo.SampleRecDuration = sampleDuration;
            buffer = new List<short>[nrSignals];

            for (int i = 0; i < nrSignals; i++)
                buffer[i]=new List<short>();


            addSignal(0, "Audio", "Amplitude", Statics.FS, 1024, 0);
            addSignal(1, "Aktivitaet", "Aktivitaet", Statics.FS /20, 100, 0);
            addSignal(2, "Position", "Position", Statics.FS / 20, 3, 0);
            if (complex)
            {
                addSignal(3, "Gyro X", "Winkelgeschwindigkeit", Statics.FS / 20, 255, 0);
                addSignal(4, "Gyro Y", "Winkelgeschwindigkeit", Statics.FS / 20, 255, 0);
                addSignal(5, "Gyro Z", "Winkelgeschwindigkeit", Statics.FS / 20, 255, 0);

                addSignal(6, "Acc X", "Beschleunigung", Statics.FS / 20, 255, 0);
                addSignal(7, "Acc X", "Beschleunigung", Statics.FS / 20, 255, 0);
                addSignal(8, "Acc X", "Beschleunigung", Statics.FS / 20, 255, 0);
            }

        }
        
        /// <summary>
        /// Adds information about a single signal to the edfFile
        /// </summary>
        /// <param name="signalNr">Number of the signal</param>
        /// <param name="name">Name of the signal</param>
        /// <param name="dim">Dimension of the signal</param>
        /// <param name="nrSamples">Nr Samples per Data Block</param>
        /// <param name="max">max value</param>
        /// <param name="min">min value</param>
        private void addSignal(int signalNr, string name, string dim, int nrSamples, double max, double min)
        {
            edfFile.SignalInfo[signalNr].PreFilter = "Filter";
            edfFile.SignalInfo[signalNr].PhysiDim = dim;
            edfFile.SignalInfo[signalNr].PhysiMax = max;
            edfFile.SignalInfo[signalNr].PhysiMin = min;
            edfFile.SignalInfo[signalNr].DigiMax = (short)max;
            edfFile.SignalInfo[signalNr].DigiMin = (short)min;
            edfFile.SignalInfo[signalNr].SignalLabel = name;
            //Daten pro Datenpaket in diesem Kanal (NrSamples/SampleRexDuration = Hz)
            edfFile.SignalInfo[signalNr].NrSamples = nrSamples;
            edfFile.SignalInfo[signalNr].Reserved = "Reserviert";
            edfFile.SignalInfo[signalNr].TransducerType = "Normal";
            edfFile.SignalInfo[signalNr].ThousandSeparator = (char)0;
        }

        /// <summary>
        /// Adds Informations about the recording and the patient to the edfFile
        /// </summary>
        /// <param name="recDescription">Description of the recording</param>
        /// <param name="patientInfo">Information about the patient</param>
        /// <param name="birthDate">BirthDate of the patient</param>
        /// <param name="gender">Gender of patient</param>
        /// <param name="name">Name of patient</param>
        public void addInformation(string recDescription, string patientInfo, DateTime birthDate, char gender, string name)
        {
            //Copy header info from the original signal
            edfFile.FileInfo.Recording = recDescription;
            edfFile.FileInfo.Patient = patientInfo;
            edfFile.FileInfo.PatientBirthDate = DateTime.Today;
            edfFile.FileInfo.PatientGender = gender;
            edfFile.FileInfo.PatientName = name;
            edfFile.FileInfo.StartDate = DateTime.Today;
            edfFile.FileInfo.StartTime = DateTime.Now.TimeOfDay;
            saveStart = DateTime.Now;
            edfFile.CommitChanges();
        }

        /// <summary>
        /// Collects Data and sends it to the DataBuffer
        /// </summary>
        /// <param name="signalNr"></param>
        /// <param name="data"></param>
        public void sendData(int signalNr, short data)
        {
            Int16[] data_ = new Int16[1];
            data_[0] = data;

            Int16[] Data = new short[edfFile.SignalInfo[signalNr].NrSamples];

            if (buffer[signalNr].Count < edfFile.SignalInfo[signalNr].NrSamples)
                buffer[signalNr].AddRange(data_);
            else
            {
                buffer[signalNr].CopyTo(0, Data, 0, edfFile.SignalInfo[signalNr].NrSamples);
                buffer[signalNr].RemoveRange(0, edfFile.SignalInfo[signalNr].NrSamples);
                buffer[signalNr].AddRange(data_);

                writeDataBuffer(signalNr, Data);
            }
        }


        /// <summary>
        /// Collects Data and sends it to the DataBuffer
        /// </summary>
        /// <param name="signalNr"></param>
        /// <param name="data"></param>
        public void sendData(int signalNr, short[] data)
        {
            Int16[] Data = new short[edfFile.SignalInfo[signalNr].NrSamples];

            if (signalNr == 0)
            {
                if (storedDataAudio.Count < edfFile.SignalInfo[signalNr].NrSamples)
                    storedDataAudio.AddRange(data);
                else
                {
                    storedDataAudio.RemoveRange(0, data.Length);
                    storedDataAudio.AddRange(data);
                }
            }

            if (signalNr == 2)
            {
                if (storedDataPos.Count < edfFile.SignalInfo[signalNr].NrSamples)
                    storedDataPos.AddRange(data);
                else
                {
                    storedDataPos.RemoveRange(0, data.Length);
                    storedDataPos.AddRange(data);
                }
            }

            if (buffer[signalNr].Count < edfFile.SignalInfo[signalNr].NrSamples)
                buffer[signalNr].AddRange(data);
            else
            {
                buffer[signalNr].CopyTo(0, Data, 0, edfFile.SignalInfo[signalNr].NrSamples);
                buffer[signalNr].RemoveRange(0, edfFile.SignalInfo[signalNr].NrSamples);
                buffer[signalNr].AddRange(data);
                writeDataBuffer(signalNr,Data);
            }
        }


        /// <summary>
        /// Notes lost Time and adds Empty Values to EDF File after 1 second of lost time.
        /// </summary>
        /// <param name="time"></param>
        public void AddLostTime(Stopwatch stopwatch, int elapsedSeconds)
        {
            lostTime += (stopwatch.ElapsedTicks - (Stopwatch.Frequency*elapsedSeconds));
            
            //If more than 2 seconds are missing
            if (lostTime >= Stopwatch.Frequency*2)
            {
                while (lostTime >= Stopwatch.Frequency)
                {
                    lostTime -= Stopwatch.Frequency;
                    //Fügt eine Sekunde zur EDF Datei hinzu
                    for (int i = edfFile.SignalInfo.Count; i > 0; i--)
                    {
                        Int16[] Data = new short[edfFile.SignalInfo[i - 1].NrSamples];
                        Array.Clear(Data, 0, Data.Length);
                        if (i == 1)
                            for (int k = 0; k < Data.Length; k++)
                                Data[k] = (Int16)Statics.offset;
                        writeDataBuffer(i - 1, Data);
                    }
                }
            }
            //If only one seconds needs to be filled in
            else if (lostTime >= Stopwatch.Frequency)
            {
                lostTime -= Stopwatch.Frequency;
                //Fügt eine Sekunde zur EDF Datei hinzu
                for (int i = edfFile.SignalInfo.Count; i > 0; i--)
                {
                    Int16[] Data = new short[edfFile.SignalInfo[i - 1].NrSamples];
                    Array.Clear(Data, 0, Data.Length);
                    
                    if (i == 3)
                    {
                        storedDataPos.CopyTo(0, Data, 0, edfFile.SignalInfo[i - 1].NrSamples);
                        //Array.Reverse(Data);
                    }
                    if (i == 1)
                    {
                        storedDataAudio.CopyTo(0, Data, 0, edfFile.SignalInfo[i - 1].NrSamples);
                        Array.Reverse(Data);
                    }
                    writeDataBuffer(i - 1, Data);
                }
            }
        }

        /// <summary>
        /// Fills all channels of EDF-File with zeroes over a certain time 
        /// </summary>
        /// <param name="time">Time to fill with zeroes</param>
        public void FillMissingData(TimeSpan time)
        {
           for (int j = 0; j < time.TotalSeconds; j++)
           {
               for (int i = edfFile.SignalInfo.Count; i > 0; i--)
                {
                    Int16[] Data = new short[edfFile.SignalInfo[i-1].NrSamples];
                    Array.Clear(Data,0,Data.Length);
                    if (i == 1)
                        for (int k = 0; k < Data.Length; k++)
                                Data[k] = (Int16)Statics.offset;
                    writeDataBuffer(i-1, Data);
                }
           }
        }

        /// <summary>
        /// Fills all channels of EDF-File with zeroes over a certain time 
        /// </summary>
        /// <param name="time">Amount of lost Packages</param>
        public void FillMissingData(int lostPackages)
        {
          Int16[] audio = new Int16[20];
          Int16[] gyro = new Int16[3];
          Int16[] accelerationRaw = new Int16[3];
          int activity = 0;
          int sleepPosition = 0;

          Array.Clear(audio, 0, audio.Length);
          Array.Clear(gyro, 0, gyro.Length);
          Array.Clear(accelerationRaw, 0, accelerationRaw.Length);

              for (int i = 0; i < audio.Length; i++)
              audio[i] = (Int16)Statics.offset;
          

            for (int j = 0; j < lostPackages; j++)
            {
                if (Statics.complexSave)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        sendData(i + 3, gyro[i]);
                        sendData(i + 6, accelerationRaw[i]);
                    }
                }
                sendData(1, (short)activity);
                sendData(2, (short)sleepPosition);
                sendData(0, audio);
            }
        }

        /// <summary>
        /// Writes Data into the Data Buffer
        /// </summary>
        /// <param name="signalNr"></param>
        /// <param name="data"></param>
        public void writeDataBuffer(int signalNr, short[] data)
        {
            //Check if data size is equal to assigned size in edffile
            if (edfFile.SignalInfo[signalNr].NrSamples != data.Length)
            {
                controller.SendToTextBox("Fehler: Falsche Buffergröße für EDF-File");
                return;
            }

            //Calculate Offset in DataBuffer
            int offset = 0;
            for (int i = 0; i < signalNr; i++)
                offset += edfFile.SignalInfo[i].NrSamples;

            //Writes Data into DataBuffer
            for (int i = 0; i < edfFile.SignalInfo[signalNr].NrSamples; i++)
                edfFile.DataBuffer[i + offset] = data[i];

            //If Audio Signal reached (longest Signal) write!
            if(signalNr==(0))
            writeDataBlock();
        }

        /// <summary>
        /// Measures Time between Start and End of Data Recording and fixes wrong SampleRates
        /// </summary>
        public void fixSampleRate()
        {
            double sampleRecDuration = 0;
            TimeSpan recordingTime = DateTime.Now - saveStart;

            sampleRecDuration = ((double)recordingTime.TotalSeconds / (double)dataBlockNr);
            //If measured duration out of bounds, something went wrong & we dont make any changes
            if (sampleRecDuration >= 0.9 && sampleRecDuration <= 1.1)
            {
                edfFile.FileInfo.SampleRecDuration = sampleRecDuration;
                commitChanges();
            }
        }

        /// <summary>
        /// Writes Data in DataBuffer to edfFile
        /// </summary>
        private void writeDataBlock()
        {
            try
            {
                edfFile.WriteDataBlock(dataBlockNr);
                dataBlockNr++;
                commitChanges();
            }
            catch
            {
                controller.SendToTextBox("Fehler: Probleme beim Zugriff auf die EDF-Datei");
            }
        }

        /// <summary>
        /// Commits Changes to the Header of edfFile (like changed size)
        /// </summary>
        public void commitChanges()
        {
            try
            {
                edfFile.CommitChanges();
            }
            catch
            {
                controller.SendToTextBox("Fehler: Probleme beim Zugriff auf die EDF-Datei");
            }
        }

    }
}
