using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using OpenMetaverse;

namespace Radegast
{
    public partial class MapControl : UserControl
    {
        RadegastInstance Instance;
        GridClient Client { get { return Instance.Client; } }
        Color background;
        float zoom;
        Font textFont;
        Brush textBrush;
        Brush textBackgroudBrush;
        uint regionSize = 256;
        float pixelsPerMeter;
        double centerX;
        double centerY;
        bool centered = false;

        public MapControl(RadegastInstance instance)
        {
            Zoom = 2f;

            InitializeComponent();
            Disposed += new EventHandler(MapControl_Disposed);
            this.Instance = instance;

            background = Color.FromArgb(4, 4, 75);
            textFont = new Font(FontFamily.GenericSansSerif, 8.0f, FontStyle.Bold);
            textBrush = new SolidBrush(Color.FromArgb(255, 200, 200, 200));
            textBackgroudBrush = new SolidBrush(Color.Black);
            CenterMap(1130, 1071, 128, 128);
        }

        void MapControl_Disposed(object sender, EventArgs e)
        {
        }

        public float Zoom
        {
            get { return zoom; }
            set
            {
                if (value >= 1f && value <= 4f)
                {
                    zoom = value;
                    pixelsPerMeter = 1f / zoom;
                    Invalidate();
                }
            }
        }

        public void CenterMap(uint regionHandle, uint localX, uint localY)
        {
            uint regionX, regionY;
            Utils.LongToUInts(regionHandle, out regionX, out regionY);
            CenterMap(regionX, regionY, localX, localY);
        }

        public void CenterMap(uint regionX, uint regionY, uint localX, uint localY)
        {
            centerX = (double)regionX * 256 + (double)localX;
            centerY = (double)regionY * 256 + (double)localY;
            centered = true;
        }

        public static ulong GlobalPosToRegionHandle(double globalX, double globalY, out float localX, out float localY)
        {
            uint x = ((uint)globalX / 256) * 256;
            uint y = ((uint)globalY / 256) * 256;
            localX = (float)(globalX - (double)x);
            localY = (float)(globalY - (double)y);
            return Utils.UIntsToLong(x, y);
        }


        void Print(Graphics g, float x, float y, string text)
        {
            g.DrawString(text, textFont, textBackgroudBrush, x + 1, y + 1);
            g.DrawString(text, textFont, textBrush, x, y);
        }

        void DrawRegion(Graphics g, int x, int y, ulong handle)
        {
            uint regX, regY;
            Utils.LongToUInts(handle, out regX, out regY);
            regX /= regionSize;
            regY /= regionSize;
            string name = string.Format("{0:0}, {1:0}", regX, regY);
            Print(g, x + 2, y - 16, name);
            float regS = regionSize / zoom;
            g.DrawRectangle(SystemPens.Window, (float)x, y - regS, regS, regS);
        }

        private void MapControl_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(background);
            if (!centered) return;
            int h = e.ClipRectangle.Height, w = e.ClipRectangle.Width;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;


            float localX, localY;
            ulong centerRegion = GlobalPosToRegionHandle(centerX, centerY, out localX, out localY);
            int pixCenterRegionX = (int)(w / 2 - localX / zoom);
            int pixCenterRegionY = (int)(h / 2 + localY / zoom);

            uint regX, regY;
            Utils.LongToUInts(centerRegion, out regX, out regY);
            regX /= regionSize;
            regY /= regionSize;

            int pixRegS = (int)(regionSize / zoom);
            int regLeft = (int)regX - ((int)(pixCenterRegionX / pixRegS) + 1);
            if (regLeft < 0) regLeft = 0;
            int regTop = (int)regY - ((int)(pixCenterRegionY / pixRegS));
            if (regTop < 0) regTop = 0;

            for (int ry = regTop; ry < regTop + e.ClipRectangle.Height / pixRegS + 2; ry++)
            {
                for (int rx = regLeft; rx < regLeft + e.ClipRectangle.Width / pixRegS + 2; rx++)
                {
                    DrawRegion(g,
                        pixCenterRegionX - ((int)regX - rx) * pixRegS,
                        pixCenterRegionY + (ry - (int)regY) * pixRegS,
                        Utils.UIntsToLong((uint)rx * regionSize, (uint)ry * regionSize));
                    //System.Console.Write("({0}, {1}) ", rx, ry);
                }
                //System.Console.WriteLine();
            }
            //System.Console.WriteLine("Reg left: {0}, reg top {1}", regLeft, regTop);

            //DrawRegion(g, pixCenterRegionX, pixCenterRegionY, centerRegion);
        }

        #region Mouse handling
        private void MapControl_Click(object sender, EventArgs e)
        {
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0)
                Zoom += 0.25f;
            else
                Zoom -= 0.25f;
        }

        bool dragging = false;
        int dragX, dragY;

        private void MapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                dragX = e.X;
                dragY = e.Y;
            }
        }

        private void MapControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = false;
            }

        }

        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                centerX -= (e.X - dragX) / pixelsPerMeter;
                centerY += (e.Y - dragY) / pixelsPerMeter;
                dragX = e.X;
                dragY = e.Y;
                Invalidate();
            }
        }

        private void MapControl_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }
        #endregion Mouse handling
    }
}
