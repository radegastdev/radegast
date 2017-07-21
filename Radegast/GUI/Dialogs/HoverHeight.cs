using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Radegast
{
    public partial class frmHoverHeight : Form, INotifyPropertyChanged
    {
        private const int HoverHeightPrecision = 100;

        private bool IsMouseDown { get; set; }
        private bool IsScrolling { get; set; }
        private bool IsMono { get; }

        private double _realHoverHeight;

        /// <summary>
        /// Gets or sets the real hover height
        /// </summary>
        public double RealHoverHeight
        {
            get => _realHoverHeight;
            set
            {
                if (Math.Abs(RealHoverHeight - value) > double.Epsilon)
                {
                    _realHoverHeight = value;
                    OnPropertyChanged();

                    if (!IsScrolling)
                    {
                        OnHoverHeightChanged(_realHoverHeight);
                    }
                    else
                    {
                        OnHoverHeightPreview(_realHoverHeight);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the hover height for use in the TrackBar control.
        /// </summary>
        public int FakeHoverHeight
        {
            get => (int)(RealHoverHeight * HoverHeightPrecision);
            set
            {
                if (FakeHoverHeight != value)
                {
                    RealHoverHeight = value / (double) HoverHeightPrecision;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>Minimum hover height</summary>
        public double MinHoverHeight => -2.0;

        /// <summary>Maximum hover height</summary>
        public double MaxHoverHeight => 2.0;

        /// <summary>
        /// Constructs a new hover height dialog.
        /// </summary>
        /// <param name="initialHoverHeight">Initial hover height</param>
        /// <param name="isMono">Determines if mono hacks should be applied</param>
        public frmHoverHeight(double initialHoverHeight, bool isMono)
        {
            IsMono = isMono;

            InitializeComponent();

            tbHoverHeight.Maximum = (int)(MaxHoverHeight * HoverHeightPrecision);
            tbHoverHeight.Minimum = (int)(MinHoverHeight * HoverHeightPrecision);
            tbHoverHeight.DataBindings.Add(new Binding("Value", this, nameof(FakeHoverHeight), true, DataSourceUpdateMode.OnPropertyChanged));

            var textBinding = new Binding("Text", this, nameof(RealHoverHeight), true, DataSourceUpdateMode.OnPropertyChanged);
            textBinding.Parse += textBinding_Parse;
            txtHoverHeight.DataBindings.Add(textBinding);

            RealHoverHeight = initialHoverHeight;
        }

        private void textBinding_Parse(object sender, ConvertEventArgs e)
        {
            var sourceBinding = sender as Binding;
            var sourceControl = sourceBinding.Control as TextBox;

            var attemptedHoverHeight = 0.0;
            if (double.TryParse(sourceControl.Text, out attemptedHoverHeight))
            {
                sourceControl.ForeColor = TextBox.DefaultForeColor;
                if (attemptedHoverHeight < MinHoverHeight)
                {
                    e.Value = MinHoverHeight;
                    sourceControl.Text = e.Value.ToString();
                }
                else if(attemptedHoverHeight > MaxHoverHeight)
                {
                    e.Value = MaxHoverHeight;
                    sourceControl.Text = e.Value.ToString();
                }
            }
            else
            {
                sourceControl.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void tbHoverHeight_MouseCaptureChanged(object sender, EventArgs e)
        {
            IsMouseDown = false;

            if (IsScrolling)
            {
                IsScrolling = false;
                OnHoverHeightChanged(RealHoverHeight);
            }
        }

        private void tbHoverHeight_MouseDown(object sender, MouseEventArgs e)
        {
            IsMouseDown = true;
        }

        private void tbHoverHeight_Scroll(object sender, EventArgs e)
        {
            if (IsMouseDown)
            {
                IsScrolling = true;
            }
        }

        /// <summary>Raised whenever the hover height changes.</summary>
        public event EventHandler<HoverHeightChangedEventArgs> HoverHeightChanged;

        /// <summary>
        /// Raises the HoverHeightChanged event
        /// </summary>
        /// <param name="newHoverHeight">New hover height</param>
        private void OnHoverHeightChanged(double newHoverHeight)
        {
            HoverHeightChanged?.Invoke(this, new HoverHeightChangedEventArgs(newHoverHeight));
        }

        /// <summary>
        /// Raised whenever the hover height preview changes. This event is raised rapidly and
        /// should only be used to preview the hover height locally.
        /// </summary>
        public event EventHandler<HoverHeightChangedEventArgs> HoverHeightPreview;

        /// <summary>
        /// Raises the HoverHeightPreview event
        /// </summary>
        /// <param name="newHoverHeight">New hover height</param>
        private void OnHoverHeightPreview(double newHoverHeight)
        {
            HoverHeightPreview?.Invoke(this, new HoverHeightChangedEventArgs(newHoverHeight));
        }

        /// <summary>Raised whenever a property changes</summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (IsMono)
            {
                // TODO: PropertyChanged is always null on mono, we must manually update our controls
                switch (propertyName)
                {
                    case nameof(FakeHoverHeight):
                        tbHoverHeight.Value = FakeHoverHeight;
                        break;
                    case nameof(RealHoverHeight):
                        txtHoverHeight.Text = RealHoverHeight.ToString("F3");
                        break;
                }
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class HoverHeightChangedEventArgs : EventArgs
    {
        public double HoverHeight { get; set; }

        public HoverHeightChangedEventArgs(double newHoverHeight)
        {
            HoverHeight = newHoverHeight;
        }
    }
}
