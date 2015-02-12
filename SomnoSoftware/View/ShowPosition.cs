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
        public ShowPosition(ref PictureBox pb)
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

            g.DrawString("Schlafposition", font, b_black, new PointF(w / 2 - 38, 5));

            g.DrawRectangle(p_frame, new Rectangle(1, 1, w - 2, h - 2));

            pb.BackgroundImage = bmp_back;
    
        }

        public void DrawPosition(int pos)
        {
            Graphics g = Graphics.FromImage(bmp_front);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            pb.Image = null;
            g.Clear(Color.Transparent);

            switch (pos)
            {
                case 0:
                    {
                        g.DrawImage(Properties.Resources.faceup, new RectangleF(w / 2 - 60, 20, 120, 60));
                        g.DrawString("Rückenlage", font, b_black, w / 2 - 30, 80);
                        break;
                    }
                case 1:
                    {
                        g.DrawImage(Properties.Resources.side, new RectangleF(w / 2 - 60, 20, 120, 60));
                        g.DrawString("Seitenlage", font, b_black, w / 2 - 30, 80);
                        break;
                    }
                case 2:
                    {
                        g.DrawImage(Properties.Resources.facedown, new RectangleF(w / 2 - 60, 20, 120, 60));
                        g.DrawString("Bauchlage", font, b_black, w / 2 - 30, 80);
                        break;
                    }
                default:
                    break;
            }            

            pb.Image = bmp_front;

        }
    }
}
