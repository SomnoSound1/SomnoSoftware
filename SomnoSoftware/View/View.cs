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
        //Hallo
        //und nochmal ein Test
        //Essen?
        //sehr gerne ich habe Hunger!
        private Controller controller;
        public Int32 time = 0;
        Spectrogram spec;
        Rectangle rect;

        /// <summary>
        /// Constructor, initializing the components
        /// </summary>
        public View()
        {
            InitializeComponent();
            LoadZedGraph();
            SetSize();
            spec = new Spectrogram(ref pb_spec);
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

            //Go on time Axes
            time += e.audio.Length;
            if (time > (229*e.audio.Length))
                time=0;

            //Update Graphs
            UpdateZedGraph(zedGraphAudio,e.audio);
            spec.DrawSpectrogram(time, e.FFT);
        }

        private void UpdateZedGraph(ZedGraphControl zedGraph, Int16[] Data)
        {
            int localTime = 0;
            double seconds = 0;
            
            // Make sure that the curvelist has at least one curve
            if (zedGraph.GraphPane.CurveList.Count <= 0)
                return;

            // Get the first CurveItem in the graph
            LineItem curve = zedGraph.GraphPane.CurveList[0] as LineItem;
            if (curve == null)
                return;

            // Get the PointPairList
            IPointListEdit list = curve.Points as IPointListEdit;
            
            //If time hits zero, Graph gets cleared and redrawn
            if (time == 0)
            {
                list.Clear();
                // Force a redraw
                zedGraph.Invalidate();
            }

            // If this is null, it means the reference at curve.Points does not
            // support IPointListEdit, so we won't be able to modify it
            if (list == null)
                return;
            
            for (int i = 0; i < Data.Length; i++)
            {
                
                seconds = (double)(time + localTime)/4890;
                // Time is measured in seconds
                localTime++;
                // 3 seconds per cycle
                list.Add(seconds, Data[i]);
            }

            //Only redraw a certain area
            var xPix = (int)zedGraph.GraphPane.XAxis.Scale.Transform(seconds);
            rect.X = xPix - 10; //- 20;
            rect.Width = 21;
            zedGraph.Invalidate(rect);

        }
        
        /// <summary>
        /// Initializes the ZedGraph
        /// </summary>
        private void LoadZedGraph()
        {
            RectangleF rectF;
            GraphPane myPane = zedGraphAudio.GraphPane;
            myPane.Title.Text = "Audio Data";
            myPane.XAxis.Title.Text = "Seconds";
            myPane.YAxis.Title.Text = "Amplitude";

            //Manually set the axis for the Graph
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 10;
            myPane.XAxis.Scale.MinorStep = 1;
            myPane.XAxis.Scale.MajorStep = 2;

            myPane.YAxis.Scale.Min = 0;
            myPane.YAxis.Scale.Max = 1024;
            myPane.YAxis.Scale.MinorStep = 40;
            myPane.YAxis.Scale.MajorStep = 200;
            
            // Save 1200 points.  At 50 ms sample rate, this is one minute
            // The RollingPointPairList is an efficient storage class that always
            // keeps a rolling set of point data without needing to shift any data values
            RollingPointPairList list = new RollingPointPairList(1200);

            // Initially, a curve is added with no data points (list is empty)
            // Color is blue, and there will be no symbols
            LineItem curve = myPane.AddCurve("Audio", list, Color.Blue, SymbolType.None);
            
            // Scale the axes
            zedGraphAudio.AxisChange();

            //Initialize the Rectangle for redrawing 
            rectF = myPane.Chart.Rect;
            rect = new Rectangle((int)rectF.X, (int)rectF.Y, (int)rectF.Width, (int)rectF.Height);
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
    }
}
