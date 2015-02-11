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
    abstract class Show
    {
        protected Bitmap bmp_front;
        protected Bitmap bmp_back;
        protected PictureBox pb;
        protected int h = 0;
        protected int w = 0;

        protected Pen p_black = new Pen(Color.Black, 1.5f);
        protected SolidBrush b_black = new SolidBrush(Color.Black);
        protected Font font = new Font("Arial", 9);
    }
}
