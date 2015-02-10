using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SomnoSoftware
{
    class FourierTransform       
    {
        private struct komplex
        {
            public double re;
            public double im;
        }

        public static double[] FFT(double[] daten)
        // Method which is externally called to calculate FFT
        {
            int samples = daten.Length;
            
            komplex[] data = new komplex[samples];

            for (int i = 0; i < samples; i++)
            {
                data[i].re = daten[i];
                data[i].im = 0.0;
            }

            data = FFT(data, samples);

            double[] OutPut = new double[samples / 2];

            for (int i = 0; i < samples / 2; i++)
            {
                OutPut[i] = Math.Sqrt(data[i].re * data[i].re + data[i].im * data[i].im);
            }
            return OutPut;
        }

        // Methods which actually do the calculations
        /// <summary>
        /// Methode FFT aus dem Buch :
        /// 
        /// "Praktische Informationstechnik mit C#", Oliver Kluge
        /// Springer Verlag
        /// 
        /// </summary>
        private static komplex[] FFT(komplex[] data, int samples)
        {
            double tempr = 0; // für Tausch bei Bit-Umkehr
            double tempi = 0; // für Tausch bei Bit-Umkehr
            double wreal = 0; // Drehfaktor (Realteil)
            double wimag = 0; // Drehfaktor (Imaginärteil)
            double real1 = 0; // Hilfsvariable
            double imag1 = 0; // Hilfsvariable
            double real2 = 0; // Hilfsvariable
            double imag2 = 0; // Hilfsvariable
            int i = 0;
            int j = 0;
            int k = 0;
            int stufen = 0;
            int sprung = 0;
            int schritt = 0;
            int element = 0;
            // Bit-Umkehr
            for (j = 0; j < samples - 1; j++)
            {
                if (j < i)
                {
                    tempr = data[j].re;
                    tempi = data[j].im;
                    data[j].re = data[i].re;
                    data[j].im = data[i].im;
                    data[i].re = tempr;
                    data[i].im = tempi;
                }
                k = samples / 2;
                while (k <= i)
                {
                    i -= k;
                    k /= 2;
                }
                i += k;
            }

            stufen = (int)(Math.Log10((double)samples) /
            Math.Log10((double)2));
            sprung = 2;
            for (i = 0; i < stufen; i++)
            {
                // jede Iterationsstufe startet mit dem 1. Wert
                element = 0;
                for (j = samples / sprung; j >= 1; j--)
                {
                    schritt = sprung / 2;
                    for (k = 0; k < schritt; k++)
                    {
                        // 1. Zweig des Butterfly
                        wreal = Math.Cos(k * 2.0 * Math.PI / sprung);
                        wimag = Math.Sin(k * 2.0 * Math.PI / sprung);
                        real1 = data[element + k].re + (wreal * data[element + k + schritt].re - wimag * data[element + k + schritt].im);
                        imag1 = data[element + k].im + (wreal * data[element + k + schritt].im + wimag * data[element + k + schritt].re);
                        // 2. Zweig des Butterfly
                        wreal *= -1.0;
                        wimag *= -1.0;
                        real2 = data[element + k].re + (wreal * data[element + k + schritt].re - wimag * data[element + k + schritt].im);
                        imag2 = data[element + k].im + (wreal * data[element + k + schritt].im + wimag * data[element + k + schritt].re);
                        // Ergebnisse übernehemen
                        data[element + k].re = real1;
                        data[element + k].im = imag1;
                        data[element + k + schritt].re = real2;
                        data[element + k + schritt].im = imag2;
                    }
                    element += sprung;
                }
                sprung *= 2;
            }

            return data;
        }

    }
}
