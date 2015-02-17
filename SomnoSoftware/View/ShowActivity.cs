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
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pb">reference of picturebox for activity display</param>
        public ShowActivity(ref PictureBox pb)
        {
            this.pb = pb;
            
            h = pb.Height;
            w = pb.Width;        
  
            bmp_front = new Bitmap(pb.Width, pb.Height);
            bmp_back = new Bitmap(pb.Width, pb.Height);            
           
            Initialize();
        }

        /// <summary>
        /// Draw font and frame
        /// </summary>
        private void Initialize()
        {
            Graphics g = Graphics.FromImage(bmp_back);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            g.DrawString("Aktivität", font, b_black, 16, 5);

            g.DrawRectangle(p_frame, new Rectangle(1, 1, w - 2, h - 2));

            pb.BackgroundImage = bmp_back;

        }

        /// <summary>
        /// Draw activity 
        /// </summary>
        /// <param name="act">activity (O...20)</param>
        public void DrawActivity(int act)
        {

            Graphics g = Graphics.FromImage(bmp_front);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            pb.Image = null;                // Clear image before redrawing
            g.Clear(Color.Transparent);

            float height_box = (float)(h - 5 - 20) / (float)Statics.max_act;    // Height of one box
            float width_box = w - 10;                                           // Width of one box

            for (int i = 1; i <= act; i++)
            {
                Color c = MapRainbowColor(i, Statics.max_act, 0);
                b_black.Color = c;
                float y_box = h - 5 - (i * height_box);             // y-position of box
                g.FillRectangle(b_black, new RectangleF(5, y_box, width_box, height_box - 2));
            }
       
            pb.Image = bmp_front;

        }

        /// <summary>
        /// Map Value (0...20) to color (green to red)
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="red_value">20</param>
        /// <param name="green_value">0</param>
        /// <returns></returns>
        private Color MapRainbowColor(int value, int red_value, int green_value)
        {                    
            
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
            
        }
    }
}
