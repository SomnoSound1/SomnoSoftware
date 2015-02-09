﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SomnoSoftware
{

    public class NewDataAvailableEvent : EventArgs
    {
        public Int16[] audio;
        public Int16[] gyro;
        public Int16[] accelerationRaw;
        public double[] FFT;

        public NewDataAvailableEvent(Int16[] audio, Int16[] gyro, Int16[] accelerationRaw)
        {
            this.audio = audio;
            this.gyro = gyro;
            this.accelerationRaw = accelerationRaw;
        }

        public NewDataAvailableEvent(Int16[] audio, double[] FFT)
        {
            this.audio = audio;
            this.FFT = FFT;
        }


    }
}
