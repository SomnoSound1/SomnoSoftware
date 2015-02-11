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
        private int num_of_boxes = 10;

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
            Graphics g = Graphics.FromImage(bmp_front);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            pb.Image = null;
            g.Clear(Color.Transparent);

            float height_box = (h - 5 - 15) / num_of_boxes;
            float width_box = w - 10;


            for (int i = 0; i < act; i++)
            {
                float y_box = h - 5 - ((i+1) * height_box);
                g.FillRectangle(b_black, new RectangleF(5, y_box, width_box, height_box));
            }
                
            pb.Image = bmp_front;

        }
    }
}
