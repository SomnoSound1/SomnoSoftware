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
    public partial class Connect : Form
    {
        private Controller controller;

        public Connect()
        {
            InitializeComponent();
        }

        public void setController(Controller controller)
        {
            this.controller = controller;
            //this.buttonExit.Click += new EventHandler(controller.Exit);
            //this.buttonSave.Click += new EventHandler(controller.OpenSaveDialog);
            //this.buttonLoad.Click += new EventHandler(controller.LoadData);
            this.buttonConnect.Click += new EventHandler(controller.Connect);
            this.FormClosing += new FormClosingEventHandler(controller.Exit);
            //this.timerDisconnect.Tick += new EventHandler(controller.Reconnect);

            //Subscribe to new Data Event
            controller.UpdateStatus += new EventHandler<UpdateStatusEvent>(UpdateStatus);
        }

        private void UpdateStatus(object sender, UpdateStatusEvent e)
        {
            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<UpdateStatusEvent>(UpdateStatus), new object[] { sender, e });
                return;
            }
            int maxTextLength = 10000; // maximum text length in text box
            if (tbData.TextLength > maxTextLength)
                tbData.Text = tbData.Text.Remove(0, tbData.TextLength - maxTextLength);

            tbData.AppendText(DateTime.Now.ToString("dd. MMM HH:mm:ss") + ": " + e.text + "\r\n");
            tbData.ScrollToCaret();
        }

        public void ChangeConnectButtonState(bool state)
        {
            buttonConnect.Enabled = state;
        }


    }
}
