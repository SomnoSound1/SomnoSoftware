using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SomnoSoftware
{
    public static class Statics
    {
        public static int FS = 4890;
        public static int FFTSize = 256;
        public static int timeDisplay = 6 * FS;
        public static int num_of_lines = (int)Math.Round((double)(Statics.timeDisplay / Statics.FFTSize));

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
    }
}
