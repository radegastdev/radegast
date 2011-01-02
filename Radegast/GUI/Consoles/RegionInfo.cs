using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class RegionInfo : RadegastTabControl
    {
        Timer refresh;

        public RegionInfo()
            : this(null)
        {
        }

        public RegionInfo(RadegastInstance instance)
            :base(instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(RegionInfo_Disposed);

            refresh = new Timer()
            {
                Enabled = false,
                Interval = 1000,
            };

            refresh.Tick += new EventHandler(refresh_Tick);
            refresh.Enabled = true;
        }

        void RegionInfo_Disposed(object sender, EventArgs e)
        {
            refresh.Enabled = false;
            refresh.Dispose();
            refresh = null;
        }

        void refresh_Tick(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        void UpdateDisplay()
        {
            if (!client.Network.Connected) return;
            if (!Visible) return;
            
            var s = client.Network.CurrentSim.Stats;
            
            lblRegionName.Text = client.Network.CurrentSim.Name;
            lblDilation.Text = string.Format("{0:0.000}", s.Dilation);
            lblFPS.Text = s.FPS.ToString();
            lblMainAgents.Text = s.Agents.ToString();
            lblChildAgents.Text = s.ChildAgents.ToString();
            lblObjects.Text = s.Objects.ToString();
            lblActiveObjects.Text = s.ScriptedObjects.ToString();
            lblActiveScripts.Text = s.ActiveScripts.ToString();
            lblPendingDownloads.Text = s.PendingDownloads.ToString();
            lblPendingUploads.Text = (s.PendingLocalUploads + s.PendingUploads).ToString();

            float total = s.NetTime + s.PhysicsTime + s.OtherTime + s.AgentTime + s.AgentTime +
                s.ImageTime + s.ImageTime + s.ScriptTime;
            lblTotalTime.Text = string.Format("{0:0.0} ms", s.FrameTime);
            lblNetTime.Text = string.Format("{0:0.0} ms", s.NetTime);
            lblPhysicsTime.Text = string.Format("{0:0.0} ms", s.PhysicsTime);
            lblSimTime.Text = string.Format("{0:0.0} ms", s.OtherTime);
            lblAgentTime.Text = string.Format("{0:0.0} ms", s.AgentTime);
            lblImagesTime.Text = string.Format("{0:0.0} ms", s.ImageTime);
            lblScriptTime.Text = string.Format("{0:0.0} ms", s.ScriptTime);
            lblSpareTime.Text = string.Format("{0:0.0} ms", Math.Max(0f, 1000f / 45f - total));
        }
    }
}
