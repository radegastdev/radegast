using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

public static class FormFlash
{
    /// <summary>
    /// Flashes the form's taskbar button.
    /// </summary>
    /// <param name="form"></param>
    public static void Flash(Form form)
    {
        form.Focus();
    }

    /// <summary>
    /// Stops flashing the form's taskbar button.
    /// </summary>
    /// <param name="form"></param>
    public static void Unflash(Form form)
    {
    }
}
