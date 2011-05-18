using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using IrrlichtNETCP;

namespace IdealistRadegastPlugin
{
    public partial class IdealistViewControl : Control, System.ComponentModel.ISupportInitialize
    {                
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr IntPtr, int wMsg, int wParam, IntPtr lParam);
        public IntPtr msgTarget;

        public IdealistViewControl()
        {
            InitializeComponent();
            this.SizeChanged += Viewer_OnResize;   
        }

        private void UIFace_Click(object sender, EventArgs e)
        {

        }
               
        protected override void WndProc(ref Message m)
        {
           //where is the handle? SendMessage(this.Parent.Handle, m.Msg, m.WParam.ToInt32(), m.LParam);
            base.WndProc(ref m);
        }

        private void Viewer_OnResize(object sender, EventArgs e)
        {
           // Dimension2D size = new Dimension2D(this.ClientSize.Height, this.ClientSize.Width);
           // Rect rect = new Rect(new Position2D(0,0),size);
           // if (this.Renderer!=null) this.Renderer.Device.VideoDriver.ViewPort = rect;
           //// SendMessage(msgTarget,0,0,0)
           // //??Renderer.GuiEnvironment.VideoDriver.ScreenSize = size;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
          //  base.OnPaint(pe);
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
          //  base.OnPaintBackground(pevent);
        }

        private void IdealistView_Click(object sender, EventArgs e)
        {

        }

        public void BeginInit()
        {
           // throw new NotImplementedException();
        }

        public void EndInit()
        {
         //   throw new NotImplementedException();
        }
    }
}
