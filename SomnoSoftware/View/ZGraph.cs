using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ZedGraph;

namespace SomnoSoftware
{
    class ZGraph
    {
        private ZedGraphControl zedgraph;
        Rectangle rect;

        public ZGraph(ref ZedGraphControl zedgraph)
        {
            this.zedgraph = zedgraph;
        }

        /// <summary>
        /// Initializes the ZedGraph
        /// </summary>
        public void LoadZedGraph()
        {
            RectangleF rectF;
            GraphPane myPane = zedgraph.GraphPane;
            myPane.Title.Text = "Audio Data";
            myPane.XAxis.Title.Text = "Seconds";
            myPane.YAxis.Title.Text = "Amplitude";

            //Manually set the axis for the Graph
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = Statics.timeDisplay / Statics.FS;
            myPane.XAxis.Scale.MinorStep = 1;
            myPane.XAxis.Scale.MajorStep = 2;

            myPane.YAxis.Scale.Min = 0;
            myPane.YAxis.Scale.Max = 1024;
            myPane.YAxis.Scale.MinorStep = 40;
            myPane.YAxis.Scale.MajorStep = 200;

            // Save 1200 points.  At 50 ms sample rate, this is one minute
            // The RollingPointPairList is an efficient storage class that always
            // keeps a rolling set of point data without needing to shift any data values
            RollingPointPairList list = new RollingPointPairList(Statics.FS);

            // Initially, a curve is added with no data points (list is empty)
            // Color is blue, and there will be no symbols
            LineItem curve = myPane.AddCurve("Audio", list, Color.Blue, SymbolType.None);

            // Scale the axes
            zedgraph.AxisChange();

            //Initialize the Rectangle for redrawing 
            rectF = myPane.Chart.Rect;
            rect = new Rectangle((int)rectF.X, (int)rectF.Y, (int)rectF.Width, (int)rectF.Height);
        }

        /// <summary>
        /// Updates the ZedGraph
        /// </summary>
        /// <param name="zedGraph"></param>
        /// <param name="Data"></param>
        /// <param name="time"></param>
        public void UpdateZedGraph(ZedGraphControl zedGraph, Int16[] Data, int time)
        {
            if (zedGraph.GraphPane == null)
                return;

            int localTime = 0;
            double seconds = 0;
            rect = new Rectangle((int)zedGraph.GraphPane.Chart.Rect.X, (int)zedGraph.GraphPane.Chart.Rect.Y, (int)zedGraph.GraphPane.Chart.Rect.Width, (int)zedGraph.GraphPane.Chart.Rect.Height);

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

                seconds = (double)(time + localTime) / 4890;
                // Time is measured in seconds
                localTime++;
                // 3 seconds per cycle
                list.Add(seconds, Data[i]);
            }

            //Only redraw a certain area
            var xPix = (int)zedGraph.GraphPane.XAxis.Scale.Transform(seconds);
            rect.X = xPix - ((int)zedGraph.GraphPane.Chart.Rect.Width / 6); //- 20;
            rect.Width = (int)zedGraph.GraphPane.Chart.Rect.Width/3;
            zedGraph.Invalidate(rect);
            }


    }
}
