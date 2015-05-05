using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SomnoSoftware
{
    public static class Statics
    {
        public static int FS = 5000;

        public static int timeDisplay = 10 * FS;
        //false = 3 channel edf file, true 9 channel edf file (with imu data)
        public static bool complexSave = false;
        public static int FFTSize = 256;
        public static int num_of_lines = (int)Math.Round((double)(Statics.timeDisplay / Statics.FFTSize));
        public static string sensorName = "n.def.";

        // Spectrum in dB
        public static bool dB = true;
        public static int dB_factor = 10;

        // Audio Data Offset
        public static int offset = 516; //Somno 3
        //public static int offset = 512; //Somno 2

        // Maximum value activity
        public static int max_act = 20;

        /// <summary>
        /// Converts a Degree Angle into Rad
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static float deg2rad(float degrees)
        {
            return (float)(Math.PI / 180) * degrees;
        }

        /// <summary>
        /// Converts a Radiant Angle into Degree
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static double rad2deg(double rad)
        {

            return (180 / Math.PI) * rad;

        }

        /// <summary>
        /// Calculates Angle between two vectors in 3D
        /// </summary>
        /// <param name="a">vector 1</param>
        /// <param name="b">vector 2</param>
        /// <returns>angle in deg</returns>
        public static double angle_between_vectors(double[] a, double[] b)
        {

            return Math.Acos((a[0] * b[0] + a[1] * b[1] + a[2] * b[2]) / (Math.Sqrt(a[0] * a[0] + a[1] * a[1] + a[2] * a[2]) * Math.Sqrt(b[0] * b[0] + b[1] * b[1] + b[2] * b[2])));

        }

        /// <summary>
        ///  Sets static Variables for different Sensors
        /// </summary>
        /// <param name="sensorNumber">Sensor Number</param>
        public static void changeSensorVariables(int sensorNumber)
        {

            switch (sensorNumber)
            {
                case 1:
                {
                    sensorName = "Somno1";
                    offset = 519;
                    break;
                }
                case 2:
                {
                    sensorName = "Somno2";
                    offset = 512;
                    break;
                }
                case 3:
                {
                    sensorName = "Somno3";
                    offset = 517;
                    break;
                }
                case 4:
                {
                    sensorName = "Somno4";
                    offset = 512;
                    break;
                }
                case 5:
                {
                    sensorName = "Somno5";
                    offset = 525;
                    break;
                }
                default:
                    break;
            }

        }
    }
}
