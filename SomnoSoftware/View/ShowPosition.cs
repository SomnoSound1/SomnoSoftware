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

            g.DrawString("Schlafposition", font, b_black, 50, 2);

            pb.BackgroundImage = bmp_back;
        }

        public void DrawPosition(int pos)
        {
            Graphics g = Graphics.FromImage(bmp_front);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            //Image img = Image.FromFile(".//Resources/side.PNG");

            g.DrawImage(img, 10, 10);

            pb.Image = bmp_front;

        }
    }
}
