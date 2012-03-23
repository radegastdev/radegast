using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using IdealistViewer;
using IrrlichtNETCP;
using System.Xml;
using System.IO;
using IrrlichtNETCP.Extensions;
using System.Windows;
using Viewer = IdealistRadegastPlugin.RadegastViewer;
namespace IdealistRadegastPlugin
{
    public class RaegastRenderer: IdealistViewer.Renderer
    {
    //    public IrrlichtNETCP.Quaternion CoordinateConversion_XYZ_XZY = new IrrlichtNETCP.Quaternion();

        private RadegastViewer m_viewer;

        /// <summary>
        /// Irrlicht Instance.  A handle to the Irrlicht device.
        /// </summary>
        //public IrrlichtDevice Device;
        //public VideoDriver Driver;

        //public SceneManager SceneManager;
        //public GUIEnvironment GuiEnvironment;
        //public IdealistUserControl ViewerControl;
        public RaegastRenderer(RadegastViewer viewer, IrrlichtDevice device)
            : base(viewer)
        {
          //  ViewerControl = viewer.ViewerControl;
            m_viewer = viewer;
            Device = device;
        }

        public override void Startup()
        {

            //Create a New Irrlicht Device
            //device.Timer.Stop();
            Device.Timer.Speed = 1;
            Device.WindowCaption = "IdealistViewer 0.001";
         //   viewerRenderPlane.Device = Device;
          //  viewerRenderPlane.Renderer = this;
           // Device.Resizeable = true;

            // Sets directory to load assets from
            Device.FileSystem.WorkingDirectory = m_viewer.StartupDirectory + "/" + Util.MakePath("media", "materials", "textures", "");  //We set Irrlicht's current directory to %application directory%/media


            Driver = Device.VideoDriver;
            SceneManager = Device.SceneManager;

            GuiEnvironment = Device.GUIEnvironment;

            // Compose Coordinate space converter quaternion
            IrrlichtNETCP.Matrix4 m4 = new IrrlichtNETCP.Matrix4();
            m4.SetM(0, 0, 1);
            m4.SetM(1, 0, 0);
            m4.SetM(2, 0, 0);
            m4.SetM(3, 0, 0);
            m4.SetM(0, 1, 0);
            m4.SetM(1, 1, 0);
            m4.SetM(2, 1, 1);
            m4.SetM(3, 1, 0);
            m4.SetM(0, 2, 0);
            m4.SetM(1, 2, 1);
            m4.SetM(2, 2, 0);
            m4.SetM(3, 2, 0);
            m4.SetM(0, 3, 0);
            m4.SetM(1, 3, 0);
            m4.SetM(2, 3, 0);
            m4.SetM(3, 3, 1);


            CoordinateConversion_XYZ_XZY = new IrrlichtNETCP.Quaternion(m4);
            CoordinateConversion_XYZ_XZY.makeInverse();
           
        }
    }

}
