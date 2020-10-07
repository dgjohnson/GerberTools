﻿using GerberLibrary;
using GerberLibrary.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PnP_Processor
{
    public partial class BoardDisplay :  WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public PnPMain pnp;
        public bool PostDisplay = false;
        Bounds TheBox = new Bounds();
        
        public BoardDisplay(PnPMain parent, bool postdisplay)
        {
            InitializeComponent();
            pnp = parent;
            PostDisplay = postdisplay;
        }



        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            TheBox.Reset();
            TheBox.FitPoint(0, 0);


            e.Graphics.Clear(Color.Black);

            //            Bitmap B = new Bitmap(10, 10);
            Graphics G = e.Graphics;
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            G.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            if (pnp.Set == null) return;

            Font F = new Font("Arial", 16);
            Font F2 = new Font("Arial Bold", 26);


            G.TranslateTransform(10, 10);

            float S = (float)Math.Min(pictureBox1.Width / (TheBox.Width() - 20), pictureBox1.Height / (TheBox.Height() - 20));


            bool TopView = false;
            if (PostDisplay) TopView = pnp.FlipBoard?false:true;

            if (TopView)
            {
                G.ScaleTransform(S * 0.8f, -S * 0.8f);
                G.TranslateTransform((float)-TheBox.TopLeft.X, (float)-TheBox.TopLeft.Y - (float)TheBox.Height());
            }
            else
            {
                G.ScaleTransform(-S * 0.8f, -S * 0.8f);
                G.TranslateTransform((float)(-TheBox.TopLeft.X - TheBox.Width()), (float)-TheBox.TopLeft.Y - (float)TheBox.Height());

            }
            RenderLayerSets(G, S, BoardSide.Both, BoardLayer.Outline, Color.White, true);


        }

        private void RenderLayerSets(Graphics G, float S, BoardSide side, BoardLayer layer, Color C, bool lines = true)
        {            
            foreach (var l in pnp.Set.PLSs)
            {
                if (l.Side == side && l.Layer == layer)
                {
                     RenderOutline(G, S, l, C, lines);
                }
            }
        }

        private static void RenderOutline(Graphics G, float S, ParsedGerber d, Color C, bool lines = true)
        {
            foreach (var ds in d.DisplayShapes)
            {
                List<PointF> Pts = new List<PointF>();
                foreach (var V in ds.Vertices)
                {
                    Pts.Add(new PointF((float)((V.X)), (float)((V.Y))));
                }

                if (Pts.Count > 2)
                {
                    if (lines)
                    {
                        G.DrawLines(new Pen(C, 1 / S), Pts.ToArray());
                    }
                    else
                    {
                        G.FillPolygon(new SolidBrush(C), Pts.ToArray());
                    }
                }
            }
        }

        private void DrawMarker(Graphics g, BOMEntry.RefDesc r, bool soldered, float S, bool current, bool activedes)
        {
            float R = 2;
            float cx = (float)r.x - R / S;
            float cy = (float)r.y - R / S;



            Color CurrentColor = soldered ? Color.Green : Color.Yellow;
            if (current)
            {
                float R2 = 5;
                float cx2 = (float)r.x - R2 / S;
                float cy2 = (float)r.y - R2 / S;
                g.FillRectangle(new SolidBrush(CurrentColor), cx2, cy2, R2 / S * 2, R2 / S * 2);
            }
            if (activedes)
            {
                float R2 = 8;
                float cx2 = (float)r.x - R2 / S;
                float cy2 = (float)r.y - R2 / S;
                g.FillRectangle(new SolidBrush(Color.HotPink), cx2, cy2, R2 / S * 2, R2 / S * 2);

            }
            g.FillRectangle(soldered ? Brushes.Green : Brushes.Red, cx, cy, R / S * 2, R / S * 2);


        }

    }
}
