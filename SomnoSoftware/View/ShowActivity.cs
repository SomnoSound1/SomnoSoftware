using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SomnoSoftware.Control;

namespace SomnoSoftware
{
    class ShowActivity : Show
    {
        public ShowActivity(ref PictureBox pb)
        {
            this.pb = pb;
            
            h = pb.Height;
            w = pb.Width;        
  
            bmp_front = new Bitmap(pb.Width, pb.Height);
            bmp_back = new Bitmap(pb.Width, pb.Height);            
           
            Initialize();
        }

        private void Initialize()
        {
            Graphics g = Graphics.FromImage(bmp_back);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            g.DrawString("Aktivität", font, b_black, 10, 2); 

            pb.BackgroundImage = bmp_back;

        }

        public void DrawActivity(int act)
        {
            //TODO Kein Balken bei Null!! Bitte bearbeiten!!
            //   -Laborleiter Doktor Kalkbrenner
            Graphics g = Graphics.FromImage(bmp_front);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            pb.Image = null;
            g.Clear(Color.Transparent);

            float height_box = (h - 5 - 20) / Statics.max_act;
            float width_box = w - 10;


            for (int i = 0; i <= act; i++)
            {
                Color c = MapRainbowColor(i, Statics.max_act, 0);
                b_black.Color = c;
                float y_box = h - 5 - ((i+1) * height_box);
                g.FillRectangle(b_black, new RectangleF(5, y_box, width_box, height_box-2));
            }
                
            pb.Image = bmp_front;

        }


        private Color MapRainbowColor(int value, int red_value, int green_value)
        {

            //// Convert into a value between 0 and 1023.
            //int int_value = (int)(511 * (value - red_value) / (green_value - red_value));

            
            
            // Map different color bands.
            if (value <= 10)
            {
                // Yellow to green. (255, 255, 0) to (0, 255, 0).            
                return Color.FromArgb(value*25,255,0);
            }
            else 
            {
                // Red to yellow. (255, 0, 0) to (255, 255, 0).
                return Color.FromArgb(255, 255 - (value - 10) * 25, 0);
            }
            //else if (int_value < 768)
            //{
            //    // Green to aqua. (0, 255, 0) to (0, 255, 255).
            //    int_value -= 512;
            //    return Color.FromArgb(0, 255, int_value);
            //}
            //else
            //{
            //    // Aqua to blue. (0, 255, 255) to (0, 0, 255).
            //    int_value -= 768;
            //    return Color.FromArgb(0, 255 - int_value, 255);
            //}
        }
    }
}
