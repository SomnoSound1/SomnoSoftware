using System;
using System.Collections.Generic;
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

        public SaveData(int nrSignals, int sampleDuration, string fileName)
        {
            edfFile = new EdfFile(fileName, false, false, false, false);
            edfFile.CreateNewFile(nrSignals, true);
            this.nrSignals = nrSignals;
            edfFile.FileInfo.SampleRecDuration = sampleDuration;
            buffer = new List<short>[nrSignals];
            for (int i = 0; i < nrSignals; i++)
                buffer[i]=new List<short>();
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
        public void addSignal(int signalNr, string name, string dim, int nrSamples, double max, double min)
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
        /// Writes Data into the Data Buffer
        /// </summary>
        /// <param name="signalNr"></param>
        /// <param name="data"></param>
        private void writeDataBuffer(int signalNr, short[] data)
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
