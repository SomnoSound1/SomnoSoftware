using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NeuroLoopGainLibrary;
using NeuroLoopGainLibrary.Edf;

namespace SomnoSoftware.Model
{
    class SaveData
    {
        private EdfFile edfFile;
        private int nrSignals = 0;
        private Int32 dataBlockNr = 0;

        private List<Int16>[] buffer;

        public SaveData()
        {

        }

        public SaveData(int sampleDuration, string fileName, bool complex)
        {
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
            addSignal(1, "Aktivitaet", "Aktivitaet", 1, 10, 0);
            addSignal(2, "Position", "Position", 1, 3, 0);
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
        /// Fills all channels of EDF-File with zeroes over a certain time 
        /// </summary>
        /// <param name="time">Time to fill with zeroes</param>
        public void FillMissingData(TimeSpan time)
        {
           for (int j = 0; j < time.Seconds; j++)
           {
               for (int i = edfFile.SignalInfo.Count; i > 0; i--)
                {
                    Int16[] Data = new short[edfFile.SignalInfo[i-1].NrSamples];
                    Array.Clear(Data,0,Data.Length);
                    writeDataBuffer(i-1, Data);
                }
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
            { Exception exception = new Exception("Data size differs from defined size in edfFile"); throw exception; }

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
        /// Writes Data in DataBuffer to edfFile
        /// </summary>
        private void writeDataBlock()
        {
            edfFile.WriteDataBlock(dataBlockNr);
            dataBlockNr++;
            commitChanges();
        }

        /// <summary>
        /// Commits Changes to the Header of edfFile (like changed size)
        /// </summary>
        public void commitChanges()
        {
            edfFile.CommitChanges();
        }

    }
}
