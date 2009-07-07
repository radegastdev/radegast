using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Radegast
{
    /// <summary>
    /// Used in conjuction with ritch text box to display lone numbers
    /// </summary>
    [Browsable(true),
        Category("Containers"),
        Description("Used in conjuction with ritch text box to display lone numbers")]
    public partial class LineNumberPanel : Control
    {
        private RRichTextBox rtb;
        private SolidBrush brush;

        /// <summary>
        /// Associated ritch text box control
        /// </summary>
        [Category("Behavior")]
        public RRichTextBox RTB
        {
            get
            {
                if (rtb == null)
                {
                    return new RRichTextBox();
                }
                else
                {
                    return rtb;
                }
            }

            set
            {
                if (rtb != null)
                {
                    rtb.VScroll -= new EventHandler(rtb_InvalidateNumbers);
                    rtb.TextChanged -= new EventHandler(rtb_InvalidateNumbers);
                    rtb.Resize -= new EventHandler(rtb_InvalidateNumbers);
                }
                rtb = value;
                rtb.VScroll += new EventHandler(rtb_InvalidateNumbers);
                rtb.TextChanged += new EventHandler(rtb_InvalidateNumbers);
                rtb.Resize += new EventHandler(rtb_InvalidateNumbers);
            }
        }

        void rtb_InvalidateNumbers(object sender, EventArgs e)
        {
            Invalidate();
        }

        public LineNumberPanel()
            : base()
        {
            SetStyle(
                ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint,
                true
            );

            UpdateStyles();
            brush = new SolidBrush(ForeColor);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (rtb == null)
            {
                return;
            }

            float font_height;
            Graphics g = e.Graphics;
            brush.Color = ForeColor;
            if (rtb.Lines.Length < 3)
            {
                font_height = rtb.Font.Height + 1.5f;
            }
            else
            {
                font_height = rtb.GetPositionFromCharIndex(rtb.GetFirstCharIndexFromLine(2)).Y - rtb.GetPositionFromCharIndex(rtb.GetFirstCharIndexFromLine(1)).Y;
            }

            Point pos = new Point(0, (int)(g.VisibleClipBounds.Y + font_height / 3));
            int first_index, first_line, first_line_y;
            first_index = rtb.GetCharIndexFromPosition(pos);
            first_line = rtb.GetLineFromCharIndex(first_index);
            first_line_y = 1 + rtb.GetPositionFromCharIndex(first_index).Y;

            int i = first_line;
            int x = 0;
            Single y = 0;
            int total_lines = rtb.GetLineFromCharIndex(Int32.MaxValue) + 1;
            StringFormat align = new StringFormat(StringFormatFlags.NoWrap);
            align.Alignment = StringAlignment.Far;
            align.LineAlignment = StringAlignment.Center;
            int maxWidth = 0;

            while (y < g.VisibleClipBounds.Y + g.VisibleClipBounds.Height)
            {
                SizeF tSize = g.MeasureString(i.ToString(), rtb.Font);

                if (maxWidth < (int)tSize.Width)
                {
                    maxWidth = (int)tSize.Width;
                }

                y = first_line_y - 1 + font_height * (i - first_line - 1);
                x = Width - (int)tSize.Width - 5;

                if (i <= total_lines)
                {
                    g.DrawString((i).ToString(), rtb.Font, brush, x, y);
                }
                i++;
            }

            maxWidth++;

            if (Width != (maxWidth + 13))
            {
                Width = maxWidth + 13;
                Invalidate();
            }
   
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            pevent.Graphics.Clear(BackColor);
        }
    }
}
