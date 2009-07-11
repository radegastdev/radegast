using System;
using System.Drawing;
using System.Windows.Forms;

namespace Radegast
{
    class TransparentLabel : Label
    {
        public TransparentLabel()
            : base()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        //private void PaintParentBackground(Graphics g)
        //{
        //    if (Parent != null)
        //    {
        //        Rectangle rect = new Rectangle(Left, Top, Width, Height);

        //        g.TranslateTransform(-rect.X, -rect.Y);

        //        try
        //        {
        //            using (PaintEventArgs pea =
        //                        new PaintEventArgs(g, rect))
        //            {
        //                pea.Graphics.SetClip(rect);
        //                InvokePaintBackground(Parent, pea);
        //                InvokePaint(Parent, pea);
        //            }
        //        }
        //        finally
        //        {
        //           g.TranslateTransform(rect.X, rect.Y);
        //        }
        //    }
        //    else
        //    {
        //        g.FillRectangle(SystemBrushes.Control, ClientRectangle);
        //    }
        //}

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    PaintParentBackground(e.Graphics);
        //    e.Graphics.DrawString("foo", Font, new SolidBrush(ForeColor), 0, 0);
        //}

        //protected override void OnPaintBackground(PaintEventArgs pevent)
        //{
        //    PaintParentBackground(pevent.Graphics);
        //}

    }
}
