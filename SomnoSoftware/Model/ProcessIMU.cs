using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SomnoSoftware.Model
{
    class ProcessIMU
    {
        MadgwickAHRS AHRS = new MadgwickAHRS(1f / 238f, 0.2f);
        private float[] rotMatrix = new float[9];

        private double[] vektorSensorX = new double[3];
        private double[] vektorSensorY = new double[3];
        private double[] vektorSensorZ = new double[3];
        private double[] vektorSensorXYZ = new double[3];
        private double[] vektorSensorStored = new double[3];

        private double[] vektorWeltX = new double[3] { 1, 0, 0 };
        private double[] vektorWeltY = new double[3] { 0, 1, 0 };
        private double[] vektorWeltZ = new double[3] { 0, 0, 1 };

        private double[] vektorSensorWinkelX = new double[3];
        private double[] vektorSensorWinkelY = new double[3];
        private double[] vektorSensorWinkelZ = new double[3];
        
        private double[] activity = new double[10];


        public void UpdateIMU(Int16 GyroX, Int16 GyroY, Int16 GyroZ, Int16 AccX, Int16 AccY, Int16 AccZ)
        {
            AHRS.Update(GyroX,GyroY,GyroZ,AccX,AccY,AccZ);
            rotMatrix = new x_IMU_API.QuaternionData(AHRS.Quaternion).ConvertToConjugate().ConvertToRotationMatrix();

            //Copys Vectors from the RotationMatrx into single Vector Arrays
            for (int i = 0; i < 3; i++)
            {
                this.vektorSensorX[i] = Convert.ToDouble((1 * rotMatrix[i * 3] + 0 * rotMatrix[i * 3 + 1] + 0 * rotMatrix[i * 3 + 2]));
                this.vektorSensorY[i] = Convert.ToDouble((0 * rotMatrix[i * 3] + 1 * rotMatrix[i * 3 + 1] + 0 * rotMatrix[i * 3 + 2]));
                this.vektorSensorZ[i] = Convert.ToDouble((0 * rotMatrix[i * 3] + 0 * rotMatrix[i * 3 + 1] + 1 * rotMatrix[i * 3 + 2]));
                this.vektorSensorXYZ[i] = Convert.ToDouble((1 * rotMatrix[i * 3] + 1 * rotMatrix[i * 3 + 1] + 1 * rotMatrix[i * 3 + 2]));
            }

            //Measures Angle Between World and Sensor Vectors
            vektorSensorStored[0] = vektorSensorWinkelX[0];
            vektorSensorWinkelX[0] = Statics.angle_between_vectors(vektorSensorX, vektorWeltX);
            vektorSensorWinkelX[1] = Statics.angle_between_vectors(vektorSensorX, vektorWeltY);
            vektorSensorWinkelX[2] = Statics.angle_between_vectors(vektorSensorX, vektorWeltZ);

            vektorSensorStored[1] = vektorSensorWinkelY[0];
            vektorSensorWinkelY[0] = Statics.angle_between_vectors(vektorSensorY, vektorWeltX);
            vektorSensorWinkelY[1] = Statics.angle_between_vectors(vektorSensorY, vektorWeltY);
            vektorSensorWinkelY[2] = Statics.angle_between_vectors(vektorSensorY, vektorWeltZ);

            vektorSensorStored[2] = vektorSensorWinkelZ[0];
            vektorSensorWinkelZ[0] = Statics.angle_between_vectors(vektorSensorZ, vektorWeltX);
            vektorSensorWinkelZ[1] = Statics.angle_between_vectors(vektorSensorZ, vektorWeltY);
            vektorSensorWinkelZ[2] = Statics.angle_between_vectors(vektorSensorZ, vektorWeltZ);
        }

        public int MeasureActivity()
        {
            //Calculate Activity
            for (int i = 0; i > activity.Length; i++)
            {
                activity[i] = (Math.Abs(vektorSensorStored[0] - vektorSensorWinkelX[0]) +
                                Math.Abs(vektorSensorStored[1] - vektorSensorWinkelY[0]) +
                                Math.Abs(vektorSensorStored[2] - vektorSensorWinkelZ[0])) *
                                (325 / activity.Length)*2;
            }

            if (activity.Sum() > 20)
                return 20;
            return (int)activity.Sum();
        }

        public int MeasureSleepPosition()
        {
            //Falls der Sensor anders im Gehäuse liegt muss das neu bestimmt werden
            //stehen
            //if (vektor_sensor_winkel_y[2] >= Rechner.deg2rad(140))
            //    position = 4;
            //bauch
            if (vektorSensorWinkelZ[2] >= Statics.deg2rad(140))
                return 2;
            //seite
            if (vektorSensorWinkelZ[2] >= Statics.deg2rad(50) && vektorSensorWinkelZ[2] < Statics.deg2rad(140))
                return 1;
            //rücken
            if (vektorSensorWinkelZ[2] >= Statics.deg2rad(0) && vektorSensorWinkelZ[2] < Statics.deg2rad(50))
                return 0;
            //n. def.
                return -1;
        }



    }
}
