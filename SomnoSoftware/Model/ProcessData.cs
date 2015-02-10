using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SomnoSoftware.Model
{
    class ProcessData
    {
        public bool sensorAnswer = false;
        private byte[] dataPaket;
        private int packageSize;
        private Hsuprot hsuprot;
        private ProcessIMU processImu; 

        //Data
        public Int16[] audio = new Int16[20];
        public Int16[] gyro = new Int16[3];
        public Int16[] accelerationRaw = new Int16[3];
        public Int16[] gyroRaw = new Int16[3];
        public int activity;
        public int sleepPosition;

        //Lists
        private List<Int16> buffer = new List<Int16>();
        public Int16[] audioArray = new Int16[Statics.FFTSize];
        public double[] fft = new double[Statics.FFTSize / 2];


        //Offset Korrektur für Gyro-Werte
        private const int offsetSteps = 200;
        private int stepsDone = 0;
        private double[,] offset = new double[3, offsetSteps];
        private double[] offsetVektor = new double[3];

        public ProcessData(int packageSize)
        {
            processImu = new ProcessIMU();
            hsuprot = new Hsuprot();
            this.packageSize = packageSize;
            dataPaket = new byte[packageSize];
        }
        
        /// <summary>
        /// Imports the Byte Arrays and detecteds Messages
        /// </summary>
        /// <param name="Data">Data to Import</param>
        /// <returns></returns>
        public bool ImportByte(byte Data)
        {
            // Determine if we have a "packet" in the queue
            if (hsuprot.ByteImport(Data) == 1)
            {
                if (!((hsuprot.inPck_.ID & (byte)(0x80)) != 0)) //If Data
                    dataPaket = hsuprot.inPck_.Bytes;
                else
                    if (hsuprot.inPck_.Bytes[0] == 5) //If not Data and Message equals five
                        sensorAnswer = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Converts Bytes to real Numbers and fills the appropriate Arrays with them
        /// </summary>
        public void Convert2Byte()
        {
            //Wenn das Datenpaket leer ist, return
            if (dataPaket == null)
            {
                return;
            }

            //Variablen für die Daten neu initiieren (leeren)
            audio = new Int16[20];
            gyro = new Int16[3];
            accelerationRaw = new Int16[3];
            gyroRaw = new Int16[3];

            //Lesen der ersten 40 Bytes (20 Werte) für die Audio Informationen
            for (int i = 0; i < 40; i = i + 2)
                audio[i / 2] = (Int16)(((char)dataPaket[i + 1]) | (char)dataPaket[i] << 8);

            //Die ersten Werte des IMU's werden genutzt um Offsets der Gyrometer zu korrigieren
            if (stepsDone < offsetSteps)
            {
                for (int i = 0; i < 3; i++)
                    offset[i, stepsDone] = (Int16)(((char)dataPaket[46 + (i * 2)] << 8) + (char)dataPaket[47 + (i * 2)]);
                stepsDone++;
            }
            //Wenn genügend Werte gesammelt wurden wird mithilfe der Varianz der Offset bestimmt
            else if (stepsDone == offsetSteps)
            {
                offsetVektor = OffsetKorrekur(offset);
                stepsDone++;
            }
            //Danach können die Werte direkt eingelesen werden
            else if (stepsDone > offsetSteps)
            {
                //Lesen der Gyro Werte mit Korrektur des Offsets
                for (int i = 0; i < 3; i++)
                    gyroRaw[i] = (Int16)((((char)dataPaket[46 + (i * 2)] << 8) + (char)dataPaket[47 + (i * 2)]) - offsetVektor[i]);

                //Lesen der Beschleunigungswerte
                for (int i = 0; i < 3; i++)
                    accelerationRaw[i] = (Int16)(((char)dataPaket[40 + (i * 2)] << 8) + (char)dataPaket[41 + (i * 2)]);

                //Umrechnen der Rohwerte in grad/s oder rad/s
                gyro[0] = (Int16)(gyroRaw[0] / 32.8);
                gyro[1] = (Int16)(gyroRaw[1] / 32.8);
                gyro[2] = (Int16)(-(gyroRaw[2]) / 32.8);
            }
        }

        /// <summary>
        /// Calculates the Offset of the Gyroscope
        /// </summary>
        /// <param name="offsetKorrekturArrayAll"></param>
        /// <returns></returns>
        public double[] OffsetKorrekur(double[,] offsetKorrekturArrayAll)
        {
            var offsetKorrekur = new double[3];

            //Für alle 3 Raumrichtungen
            for (int j = 0; j < 3; j++)
            {
                var offsetKorrekturArray = new double[offsetSteps];
                //Transformation des 2D Arrays in ein 1D Array
                System.Buffer.BlockCopy(offsetKorrekturArrayAll, (0 + (j * offsetSteps * 8)), offsetKorrekturArray, 0, offsetSteps * 8);

                var average = new double();
                int anzahl = 0;
                int grenzeOben = offsetKorrekturArray.Length;
                int grenzeUnten = 0;


                Array.Sort(offsetKorrekturArray);
                foreach (double t in offsetKorrekturArray)
                {
                    average += t;
                    anzahl++;
                }

                average = average / anzahl;

                var summeVarianz = offsetKorrekturArray.Sum(t => ((t - average) * (t - average)));
                double varianz = Math.Sqrt(1 / (anzahl - 1) * summeVarianz);

                foreach (double t in offsetKorrekturArray)
                {
                    if (t > (average + varianz)) grenzeOben--;
                    if (t < (average - varianz)) grenzeUnten++;
                }

                anzahl = 0;

                for (int i = grenzeUnten; i < grenzeOben + 1; i++)
                {
                    try
                    {
                        offsetKorrekur[j] += offsetKorrekturArray[i];
                        anzahl++;
                    }
                    catch
                    {
                        MessageBox.Show(@"Datenpakete haben falsches Format. Sensor bitte neustarten.");
                        offsetKorrekur[j] = offsetKorrekur[j] / anzahl;
                        return offsetKorrekur;
                    }
                }
                offsetKorrekur[j] = offsetKorrekur[j] / anzahl;
            }
            return offsetKorrekur;
        }

        /// <summary>
        /// Buffers Audio Data and processes FFT
        /// </summary>
        public bool Buffering()
        {
            double[] audioDoubles = new double[Statics.FFTSize];

            if (buffer.Count < Statics.FFTSize)
            {
                buffer.AddRange(audio);
                return false;
            }
            else
            {
                buffer.CopyTo(0, audioArray, 0, Statics.FFTSize);
                buffer.RemoveRange(0, Statics.FFTSize);
                buffer.AddRange(audio);

                audioDoubles = Array.ConvertAll(audioArray, item => (double)item);

                double offset = audioDoubles.Average();

                for (int i = 0; i < audioDoubles.Length; i++)
                    audioDoubles[i] -= offset;                

                fft = FourierTransform.FFT(audioDoubles);
                return true;
            }
        }

        /// <summary>
        /// Updates Madgwick and calculates the activity and the sleep position
        /// </summary>
        public void CalculateIMU()
        {
            processImu.UpdateIMU(gyro[0],gyro[1],gyro[2],accelerationRaw[0],accelerationRaw[1],accelerationRaw[2]);
            activity = processImu.MeasureActivity();
            sleepPosition = processImu.MeasureSleepPosition();
        }
    }
}
