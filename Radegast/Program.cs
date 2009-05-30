using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Radegast
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RadegastInstance instance = new RadegastInstance();
            Application.Run(instance.MainForm);
            instance = null;
        }
    }
}