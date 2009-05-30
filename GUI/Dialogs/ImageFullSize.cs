using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Radegast
{
    public partial class ImageFullSize : Form
    {
        public ImageFullSize(System.Drawing.Image img)
        {
            InitializeComponent();
            this.Height = img.Height + 30;
            this.Width = img.Width + 30;
            this.pictureBox1.Image = img;
        }
    }
}