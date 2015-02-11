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
        private ShowSpectrogram spec;
        private ShowActivity act;
        private ShowPosition pos;
        private ZGraph zGraph;
        private int counter = 0;

        private Random rand = new Random();

        /// <summary>
        /// Constructor, initializing the components
        /// </summary>
        public View()
        {
            InitializeComponent();
            SetSize();
            zGraph = new ZGraph(ref zedGraphAudio);
            spec = new ShowSpectrogram(ref pb_spec);
            act = new ShowActivity(ref pb_activity);
            pos = new ShowPosition(ref pb_position);
            
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

            act.DrawActivity((int)rand.Next(11));
            
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
            int maxTextLength = 10000; // maximum text length in text box
            if (tbData.TextLength > maxTextLength)
                tbData.Text = tbData.Text.Remove(0, tbData.TextLength - maxTextLength);

            tbData.AppendText(DateTime.Now.ToString("dd. MMM HH:mm:ss") + ": " + e.text + "\r\n");
            tbData.ScrollToCaret();
        }

        private void View_Resize(object sender, EventArgs e)
        {
            SetSize();
        }

        // Set the size and location of the ZedGraphControl
        private void SetSize()
        {
            // Control is always 10 pixels inset from the client rectangle of the form
            Rectangle Audio = this.ClientRectangle;
            Rectangle Spec = this.ClientRectangle;
            Rectangle Activity = new Rectangle(); 
            
            Spec.Height = (int)(this.ClientRectangle.Height - 90) / 2;
            Audio.Height = (int)(this.ClientRectangle.Height - 90) / 2;

            Spec.Width = (int)(this.ClientRectangle.Width - 20);
            Audio.Width = (int)(this.ClientRectangle.Width - 100);

            Audio.X += 10;
            Spec.X += 10;

            Audio.Y += 70;
            Spec.Y =  Audio.Y+Audio.Height + 10;

            Activity.Width = (this.ClientRectangle.Width - Audio.X - Audio.Width) - 20;
            Activity.Height = Audio.Height;
                        
            Activity.Y = Audio.Y;
            Activity.X = Audio.X + Audio.Width + 10;      

 
            if (zedGraphAudio.Size != Audio.Size)
            {
                zedGraphAudio.Location = Audio.Location;
                zedGraphAudio.Size = Audio.Size;

                pb_activity.Location = Activity.Location;
                pb_activity.Size = Activity.Size;
            }
           
            
            if (pb_spec.Size != Spec.Size)
            {
                pb_spec.Location = Spec.Location;
                pb_spec.Size = Spec.Size;
            }

            spec = new ShowSpectrogram(ref pb_spec);
            act = new ShowActivity(ref pb_activity);

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

        private void View_Load(object sender, EventArgs e)
        {

        }

    }
}
