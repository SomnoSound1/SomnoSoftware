using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SomnoSoftware.Control;
using ZedGraph;

namespace SomnoSoftware
{
    public partial class View : Form
    {
        private Controller controller;
        Spectrogram spec;
        private ZGraph zGraph;
        private int counter = 0;

        /// <summary>
        /// Constructor, initializing the components
        /// </summary>
        public View()
        {
            InitializeComponent();
            SetSize();
            zGraph = new ZGraph(ref zedGraphAudio);
            spec = new Spectrogram(ref pb_spec);
            zGraph.LoadZedGraph();
        }
        
        /// <summary>
        /// sets the controller and hooks up all the relevant event handlers
        /// </summary>
        /// <param name="controller"> passes the controller to hook up all the event handlers to the events</param>
        public void setController(Controller controller)
        {
            this.controller = controller;
            //this.buttonExit.Click += new EventHandler(controller.Exit);
            this.buttonSave.Click += new EventHandler(controller.OpenSaveDialog);
            //this.buttonLoad.Click += new EventHandler(controller.LoadData);
            this.buttonConnect.Click += new EventHandler(controller.Connect);
            this.FormClosing += new FormClosingEventHandler(controller.Exit);
            this.timerDisconnect.Tick += new EventHandler(controller.Reconnect);

            //Subscribe to new Data Event
            controller.NewDataAvailable += new EventHandler<NewDataAvailableEvent>(NewDataRecieved);
            controller.UpdateStatus += new EventHandler<UpdateStatusEvent>(UpdateStatus);
        }

        /// <summary>
        /// If new Data is availible to Draw/Plot/...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NewDataRecieved(object sender, NewDataAvailableEvent e)
        {
            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<NewDataAvailableEvent>(NewDataRecieved), new object[] { sender, e });
                return;
            }
            
            //Update Graphs
            zGraph.UpdateZedGraph(zedGraphAudio,e.audio,counter*e.audio.Length);                       
                      
            spec.DrawSpectrogram(e.FFT, counter);

            if (counter < Statics.num_of_lines-1)
                counter++;
            else
                counter = 0;
        }
       

        private void UpdateStatus(object sender, UpdateStatusEvent e)
        {
            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<UpdateStatusEvent>(UpdateStatus), new object[] { sender, e });
                return;
            }
            labelStatus.Text = e.text;
        }

        private void View_Resize(object sender, EventArgs e)
        {
            SetSize();
        }

        // Set the size and location of the ZedGraphControl
        private void SetSize()
        {
            // Control is always 10 pixels inset from the client rectangle of the form
            Rectangle formRect = this.ClientRectangle;
            Rectangle formRect1 = this.ClientRectangle;
            
            formRect1.Height = (int)(this.ClientRectangle.Height - 90) / 2;
            formRect.Height = (int)(this.ClientRectangle.Height - 90) / 2;

            formRect1.Width = (int)(this.ClientRectangle.Width - 20);
            formRect.Width = (int)(this.ClientRectangle.Width - 20);

            formRect.X += 10;
            formRect1.X += 10;

            formRect.Y += 70;
            formRect1.Y =  formRect.Y+formRect.Height + 10;


            if (zedGraphAudio.Size != formRect.Size)
            {
                zedGraphAudio.Location = formRect.Location;
                zedGraphAudio.Size = formRect.Size;
            }
           
            
            if (pb_spec.Size != formRect1.Size)
            {
                pb_spec.Location = formRect1.Location;
                pb_spec.Size = formRect1.Size;
            }

            spec = new Spectrogram(ref pb_spec);

        }

        public void ChangeConnectButtonState(bool state)
        {
            buttonConnect.Enabled = state;
        }

        public void ChangeConnectButtonText(bool connected)
        {
            if (connected)
                buttonConnect.Text = "Disconnect";
            else
            {
                buttonConnect.Text = "Connect";
            }
        }

        public void ChangeSaveButtonText(bool save)
        {
            if (save)
                buttonSave.Text = "Start Recording";
            else
            {
                buttonSave.Text = "Stop Recording";
            }
        }

        public void ChangeSaveButtonState(bool state)
        {
                buttonSave.Enabled = state;
        }

        public void EnableTimer(bool state)
        {
            timerDisconnect.Enabled = state;
        }

    }
}
