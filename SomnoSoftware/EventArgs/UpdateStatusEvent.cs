using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SomnoSoftware
{
    public class UpdateStatusEvent : EventArgs
    {
        public string text;

        public UpdateStatusEvent(string text)
        {
            this.text = text;
        }
    }
}
