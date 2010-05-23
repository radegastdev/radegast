using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using OpenMetaverse;
using OpenMetaverse.Http;

namespace Radegast
{
    public partial class MapControl : UserControl
    {
        RadegastInstance Instance;
        GridClient Client { get { return Instance.Client; } }
        ParallelDownloader downloader;
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
        int PixRegS;

        public MapControl(RadegastInstance instance)
        {
            Zoom = 1.25f;
            InitializeComponent();
            Disposed += new EventHandler(MapControl_Disposed);
            this.Instance = instance;

            downloader = new ParallelDownloader();

            background = Color.FromArgb(4, 4, 75);
            textFont = new Font(FontFamily.GenericSansSerif, 8.0f, FontStyle.Bold);
            textBrush = new SolidBrush(Color.FromArgb(255, 200, 200, 200));
            textBackgroudBrush = new SolidBrush(Color.Black);
            CenterMap(1130, 1071, 128, 128);
        }

        void MapControl_Disposed(object sender, EventArgs e)
        {
            downloader.Dispose();

            lock (regionTiles)
            {
                foreach (Image img in regionTiles.Values)
                    if (img != null)
                        img.Dispose();
                regionTiles.Clear();
            }
        }

        public float Zoom
        {
            get { return zoom; }
            set
            {
                if (value >= 1f && value <= 6f)
                {
                    zoom = value;
                    pixelsPerMeter = 1f / zoom;
                    PixRegS = (int)(regionSize / zoom);
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

        Dictionary<ulong, string> regionNames = new Dictionary<ulong, string>();
        Regex regName = new Regex("\"(?<regName>[^\"]+)", RegexOptions.Compiled);
        List<ulong> nameRequests = new List<ulong>();

        string GetRegionName(ulong handle)
        {
            if (regionNames.ContainsKey(handle))
            {
                return regionNames[handle];
            }
            else
            {
                lock (nameRequests)
                {
                    if (nameRequests.Contains(handle)) return RadegastInstance.INCOMPLETE_NAME;
                    nameRequests.Add(handle);
                }

                uint regX, regY;
                Utils.LongToUInts(handle, out regX, out regY);
                regX /= regionSize;
                regY /= regionSize;
                WebClient req = new WebClient();
                req.DownloadStringCompleted += (object sender, DownloadStringCompletedEventArgs e) =>
                    {
                        lock (nameRequests)
                        {
                            nameRequests.Remove(handle);
                        }

                        if (e.Error == null)
                        {
                            if (e.Result.Contains("error:"))
                            {
                                lock (regionNames)
                                {
                                    regionNames[handle] = string.Empty;
                                    if (IsHandleCreated) BeginInvoke(new MethodInvoker(() => Invalidate()));
                                }
                            }
                            else
                            {
                                Match m = regName.Match(e.Result);
                                if (m.Success)
                                {
                                    lock (regionNames)
                                    {
                                        regionNames[handle] = m.Groups["regName"].Value;
                                        if (IsHandleCreated) BeginInvoke(new MethodInvoker(() => Invalidate()));
                                    }
                                }
                            }
                        }
                    };
                req.DownloadStringAsync(new Uri(string.Format("http://slurl.com/get-region-name-by-coords?var=slRegionName&grid_x={0}&grid_y={1}", regX, regY)));

                lock (regionNames)
                {
                    regionNames[handle] = RadegastInstance.INCOMPLETE_NAME;
                }

                return RadegastInstance.INCOMPLETE_NAME;
            }
        }

        Dictionary<ulong, Image> regionTiles = new Dictionary<ulong, Image>();
        List<ulong> tileRequests = new List<ulong>();

        Image GetRegionTile(ulong handle)
        {
            if (regionTiles.ContainsKey(handle))
            {
                return regionTiles[handle];
            }
            else
            {
                lock (tileRequests)
                {
                    if (tileRequests.Contains(handle)) return null;
                    tileRequests.Add(handle);
                }

                uint regX, regY;
                Utils.LongToUInts(handle, out regX, out regY);
                regX /= regionSize;
                regY /= regionSize;

                downloader.QueueDownlad(
                    new Uri(string.Format("http://map.secondlife.com/map-1-{0}-{1}-objects.jpg", regX, regY)),
                    null,
                    20 * 1000,
                    null,
                    (HttpWebRequest request, HttpWebResponse response, byte[] responseData, Exception error) =>
                    {
                        if (error == null && responseData != null)
                        {
                            try
                            {
                                using (MemoryStream s = new MemoryStream(responseData))
                                {
                                    lock (regionTiles)
                                    {
                                        regionTiles[handle] = Image.FromStream(s);
                                        if (IsHandleCreated) BeginInvoke(new MethodInvoker(() => Invalidate()));
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                );

                lock (regionTiles)
                {
                    regionTiles[handle] = null;
                }

                return null;
            }
        }

        void DrawRegion(Graphics g, int x, int y, ulong handle)
        {
            uint regX, regY;
            Utils.LongToUInts(handle, out regX, out regY);
            regX /= regionSize;
            regY /= regionSize;
            string name = GetRegionName(handle);
            Image tile = GetRegionTile(handle);

            if (tile != null)
                g.DrawImage(tile, new Rectangle(x, y - PixRegS, PixRegS, PixRegS));

            //if (!string.IsNullOrEmpty(name))
            //    Print(g, x + 2, y - 16, name);
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

            int regLeft = (int)regX - ((int)(pixCenterRegionX / PixRegS) + 1);
            if (regLeft < 0) regLeft = 0;
            int regBottom = (int)regY - ((int)((e.ClipRectangle.Height - pixCenterRegionY) / PixRegS) + 1);
            if (regBottom < 0) regBottom = 0;

            for (int ry = regBottom; pixCenterRegionY - (ry - (int)regY) * PixRegS > 0; ry++)
            {
                for (int rx = regLeft; pixCenterRegionX - ((int)regX - rx) * PixRegS < e.ClipRectangle.Width; rx++)
                {
                    DrawRegion(g,
                        pixCenterRegionX - ((int)regX - rx) * PixRegS,
                        pixCenterRegionY - (ry - (int)regY) * PixRegS,
                        Utils.UIntsToLong((uint)rx * regionSize, (uint)ry * regionSize));
                }
            }
        }

        #region Mouse handling
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta < 0)
                Zoom += 0.25f;
            else
                Zoom -= 0.25f;
        }

        bool dragging = false;
        int dragX, dragY, downX, downY;

        private void MapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                downX = dragX = e.X;
                downY = dragY = e.Y;
            }
        }

        private void MapControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = false;
                if (e.X == downX && e.Y == downY) // click
                {
                    CenterMap(1130, 1071, 128, 128);
                    Zoom = 1.25f;
                }
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

    public class ParallelDownloader : IDisposable
    {
        Thread worker;
        int m_ParallelDownloads = 10;
        bool done = false;
        AutoResetEvent queueHold = new AutoResetEvent(false);
        Queue<QueuedItem> queue = new Queue<QueuedItem>();
        List<HttpWebRequest> activeDownloads = new List<HttpWebRequest>();

        public int ParallelDownloads
        {
            get { return m_ParallelDownloads; }
            set
            {
                m_ParallelDownloads = value;
            }
        }

        public ParallelDownloader()
        {
            worker = new Thread(new ThreadStart(Worker));
            worker.Name = "Parallel Downloader";
            worker.IsBackground = true;
            worker.Start();
        }

        public void Dispose()
        {
            done = true;
            queueHold.Set();
            queue.Clear();

            lock (activeDownloads)
            {
                for (int i = 0; i < activeDownloads.Count; i++)
                {
                    try
                    {
                        activeDownloads[i].Abort();
                    }
                    catch { }
                }
            }

            if (worker.IsAlive)
                worker.Abort();
        }

        private void Worker()
        {
            Logger.DebugLog("Parallel dowloader starting");

            while (!done)
            {
                lock (queue)
                {
                    if (queue.Count > 0)
                    {
                        int nr = 0;
                        lock (activeDownloads) nr = activeDownloads.Count;

                        for (int i = nr; i < ParallelDownloads && queue.Count > 0; i++)
                        {
                            QueuedItem item = queue.Dequeue();
                            HttpWebRequest req = CapsBase.DownloadStringAsync(
                                item.address,
                                item.clientCert,
                                item.millisecondsTimeout,
                                item.downloadProgressCallback,
                                (HttpWebRequest request, HttpWebResponse response, byte[] responseData, Exception error) =>
                                {
                                    lock (activeDownloads) activeDownloads.Remove(request);
                                    item.completedCallback(request, response, responseData, error);
                                    queueHold.Set();
                                }
                            );

                            lock (activeDownloads) activeDownloads.Add(req);
                        }
                    }
                }

                queueHold.WaitOne();
            }
            
            Logger.DebugLog("Parallel dowloader exiting");
        }

        public void QueueDownlad(Uri address, X509Certificate2 clientCert, int millisecondsTimeout,
            CapsBase.DownloadProgressEventHandler downloadProgressCallback, CapsBase.RequestCompletedEventHandler completedCallback)
        {
            lock (queue)
            {
                queue.Enqueue(new QueuedItem(
                    address,
                    clientCert,
                    millisecondsTimeout,
                    downloadProgressCallback,
                    completedCallback
                    ));
            }
            queueHold.Set();
        }

        public class QueuedItem
        {
            public Uri address;
            public X509Certificate2 clientCert;
            public int millisecondsTimeout;
            public CapsBase.DownloadProgressEventHandler downloadProgressCallback;
            public CapsBase.RequestCompletedEventHandler completedCallback;

            public QueuedItem(Uri address, X509Certificate2 clientCert, int millisecondsTimeout,
            CapsBase.DownloadProgressEventHandler downloadProgressCallback, CapsBase.RequestCompletedEventHandler completedCallback)
            {
                this.address = address;
                this.clientCert = clientCert;
                this.millisecondsTimeout = millisecondsTimeout;
                this.downloadProgressCallback = downloadProgressCallback;
                this.completedCallback = completedCallback;
            }
        }

    }
}
