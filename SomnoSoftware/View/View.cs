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
        private bool rec_logo = false;

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
            Change_Rec_logo();
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

            act.DrawActivity((int)rand.Next(Statics.max_act+1));
            
            //act.DrawActivity(e.activity);

            //pos.DrawPosition(e.sleepPosition);


            if (counter < Statics.num_of_lines - 1)
            {
                counter++;
                if ((counter % (int)(Statics.FS / Statics.FFTSize)) == 0)
                {
                    rec_logo = !rec_logo;
                    Change_Rec_logo();
                }
            }
            else
            {
                counter = 0;
                pos.DrawPosition((int)rand.Next(3));
            }


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

        /// <summary>
        /// Event handler for resizing of window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_Resize(object sender, EventArgs e)
        {
            //Minimize sets Height/Width to zero --> causes error
            if (ClientRectangle.Height != 0 && ClientRectangle.Width != 0)
            SetSize();
        }

        /// <summary>
        /// Sets position and size of all surface elements at the beginning and in case of resizing
        /// </summary>
        private void SetSize()
        {
            Rectangle Audio = new Rectangle();
            Rectangle Spec = new Rectangle();
            Rectangle Activity = new Rectangle();
            Rectangle Position = new Rectangle();
            Rectangle Logo = new Rectangle(pb_logo.Location, pb_logo.Size);            
            
            // Variables which define position of all surface elements

            int x_right = 100;      // Distance from right edge to audio/spectrogram
            
            int y_top = 55;         // Distance from top edge to audio/spectrogram
            int y_bot = 100;        // Distance from bottom edge to audio/spectrogram

            int dist = 10;          // Distance between surface elements
                        
            Audio.Height = (int)(this.ClientRectangle.Height - y_top - y_bot - dist) / 2;
            Audio.Width = (int)(this.ClientRectangle.Width - x_right - dist);            
            
            Spec.Width = Audio.Width;            
            Spec.Height = Audio.Height;

            Audio.X = dist;
            Audio.Y = y_top;

            Spec.X = Audio.X;
            Spec.Y = Audio.Y + Audio.Height + 10;

            Logo.X = this.ClientRectangle.Width - dist - Logo.Width;
            Logo.Y = pb_logo.Location.Y;

            Position.Height = Audio.Height;
            Position.Width = x_right - 2 * dist;
            
            Activity.Width = Position.Width;
            Activity.Height = Spec.Height;

            Position.X = Audio.X + Audio.Width + dist;
            Position.Y = Audio.Y;

            Activity.X = Position.X;
            Activity.Y = Spec.Y;            
            
            zedGraphAudio.Location = Audio.Location;
            zedGraphAudio.Size = Audio.Size;

            pb_spec.Location = Spec.Location;
            pb_spec.Size = Spec.Size;

            pb_logo.Location = Logo.Location;
            
            pb_position.Location = Position.Location;
            pb_position.Size = Position.Size;
            
            pb_activity.Location = Activity.Location;
            pb_activity.Size = Activity.Size;

            tbData.Size = new Size(350, y_bot - 2 * dist);
            tbData.Location = new Point((int)this.ClientRectangle.Width / 2 - tbData.Size.Width / 2, Spec.Y + Spec.Height + dist);

            buttonSave.Size = new Size(180, y_bot - 2 * dist-10);
            buttonSave.Location = new Point(5 * dist, tbData.Location.Y);

            buttonConnect.Size = buttonSave.Size;
            buttonConnect.Location = new Point((int)this.ClientRectangle.Width - buttonSave.Location.X - buttonConnect.Size.Width, tbData.Location.Y);

            pb_rec.Size = new Size(120, 45);
            pb_rec.Location = new Point(buttonSave.Location.X + buttonSave.Width + dist, this.ClientRectangle.Height-y_bot+20);

            spec = new ShowSpectrogram(ref pb_spec);
            act = new ShowActivity(ref pb_activity);
            pos = new ShowPosition(ref pb_position);

        }

        public void ChangeConnectButtonState(bool state)
        {
            buttonConnect.Enabled = state;
        }

        public void ChangeConnectButtonText(bool connected)
        {
            if (connected)
                buttonConnect.Text = "Verbindung trennen";
            else
            {
                buttonConnect.Text = "Verbindung herstellen";
            }
        }

        public void ChangeSaveButtonText(bool save)
        {
            if (save)
                buttonSave.Text = "Aufnahme starten";
            else
            {
                buttonSave.Text = "Aufnahme beenden";
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

        public void ChangeSaveImage()
        {
            if (controller.save)
                buttonSave.Image = Properties.Resources.logoStopRec;    
            else
                buttonSave.Image = Properties.Resources.logoRec;
        }

        /// <summary>
        /// Show/Hide record logo 
        /// </summary>
        private void Change_Rec_logo()
        {

            if (rec_logo && controller.save)
                pb_rec.Image = Properties.Resources.logoRec;
            else
                pb_rec.Image = null;
        }

    }
}
