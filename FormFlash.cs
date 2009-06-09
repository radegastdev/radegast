using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

public static class FormFlash
{
    [DllImport("user32.dll")]
    public static extern int FlashWindowEx(ref FLASHWINFO pfwi);

    /// <summary>
    /// Flashes the form's taskbar button.
    /// </summary>
    /// <param name="form"></param>
    public static void StartFlash(Form form)
    {
        if (Type.GetType("Mono.Runtime") != null || form.Focused)
        {
            return;
        }

        FLASHWINFO fw = new FLASHWINFO()
        {
            cbSize = Convert.ToUInt32(Marshal.SizeOf(typeof(FLASHWINFO))),
            hwnd = form.Handle,
            dwFlags = (Int32)(FLASHWINFOFLAGS.FLASHW_TRAY | FLASHWINFOFLAGS.FLASHW_TIMERNOFG),
            uCount = 5,
            dwTimeout = 0
        };

        FlashWindowEx(ref fw);
    }

    /// <summary>
    /// Stops flashing the form's taskbar button.
    /// </summary>
    /// <param name="form"></param>
    public static void StopFlash(Form form)
    {
        if (Type.GetType("Mono.Runtime") != null)
        {
            return;
        }

        FLASHWINFO fw = new FLASHWINFO()
        {
            cbSize = Convert.ToUInt32(Marshal.SizeOf(typeof(FLASHWINFO))),
            hwnd = form.Handle,
            dwFlags = (Int32)(FLASHWINFOFLAGS.FLASHW_STOP),
            uCount = 0,
            dwTimeout = 0
        };

        FlashWindowEx(ref fw);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct FLASHWINFO
{
    public UInt32 cbSize;
    public IntPtr hwnd;
    public Int32 dwFlags;
    public UInt32 uCount;
    public Int32 dwTimeout;
}

public enum FLASHWINFOFLAGS
{
    FLASHW_STOP = 0,
    FLASHW_CAPTION = 0x00000001,
    FLASHW_TRAY = 0x00000002,
    FLASHW_ALL = (FLASHW_CAPTION | FLASHW_TRAY),
    FLASHW_TIMER = 0x00000004,
    FLASHW_TIMERNOFG = 0x0000000C
}
