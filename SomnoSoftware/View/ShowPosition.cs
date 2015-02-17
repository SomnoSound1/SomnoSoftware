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
    class ShowPosition : Show
    {   
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pb">reference of picturebox for position display</param>
        public ShowPosition(ref PictureBox pb)
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

            g.DrawString("Schlaflage", font, b_black, new PointF(w / 2 - 30, 5));

            g.DrawRectangle(p_frame, new Rectangle(1, 1, w - 2, h - 2));

            pb.BackgroundImage = bmp_back;
    
        }

        
        /// <summary>
        /// Draw Image and font
        /// </summary>
        /// <param name="pos">position value (0...2)</param>
        public void DrawPosition(int pos)
        {
            Graphics g = Graphics.FromImage(bmp_front);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            pb.Image = null;                    // Clear Image before redrawing
            g.Clear(Color.Transparent);

            float pic_w = 55;                   // Width of image
            float pic_h = 130;                  // Height of image    

            float pic_x = 13;                   // x-position of image
            float pic_y = h / 2 - pic_h / 2;    // y-position of image

            int font_x = w/2 - 22;              // x-position of font
            int font_y = h -30;                 // y-position of font
            
            switch (pos)
            {
                case 0: // Rücken
                    {
                        g.DrawImage(Properties.Resources.faceup, new RectangleF(pic_x, pic_y, pic_w, pic_h));
                        g.DrawString("Rücken", font, b_black, font_x, font_y);
                        break;
                    }
                case 1: // Seite
                    {
                        g.DrawImage(Properties.Resources.side, new RectangleF(pic_x, pic_y, pic_w, pic_h));
                        g.DrawString("Seite", font, b_black, font_x+5, font_y);
                        break;
                    }
                case 2: // Bauch
                    {
                        g.DrawImage(Properties.Resources.facedown, new RectangleF(pic_x, pic_y, pic_w, pic_h));
                        g.DrawString("Bauch", font, b_black, font_x+2, font_y);
                        break;
                    }
                default:
                    break;
            }            

            pb.Image = bmp_front;

        }
    }
}
