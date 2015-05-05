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
    public partial class SaveDialog : Form
    {
        private Controller controller;
        public string patientName;

        public SaveDialog()
        {
            InitializeComponent();
        }

        public void setController(Controller controller)
        {
            this.controller = controller;
            //this.buttonExit.Click += new EventHandler(controller.Exit);
            this.buttonCancel.Click += new EventHandler(controller.CancelDialog);
            //this.buttonLoad.Click += new EventHandler(controller.LoadData);
            //this.buttonConfirm.Click += new EventHandler(controller.StartRecording);
            this.FormClosing += new FormClosingEventHandler(controller.CancelDialog);
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            patientName = textBoxName.Text;
            controller.StartRecording(textBoxName.Text,dateTimePickerBDay.Value,Convert.ToChar(textBoxGender.Text));
        }


    }
}
