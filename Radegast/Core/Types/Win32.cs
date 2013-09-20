// 
// Radegast Metaverse Client
// Copyright (c) 2009-2013, Radegast Development Team
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of the application "Radegast", nor the names of its
//       contributors may be used to endorse or promote products derived from
//       this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

using System;
using System.Runtime.InteropServices;

namespace Radegast
{
    /// <summary>
    /// Summary description for Win32.
    /// </summary>
    public static class Win32
    {

        #region Consts
        public const int WM_USER = 0x400;
        public const int WM_PAINT = 0xF;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_CHAR = 0x102;

        public const int EM_GETSCROLLPOS = (WM_USER + 221);
        public const int EM_SETSCROLLPOS = (WM_USER + 222);

        public const int VK_CONTROL = 0x11;
        public const int VK_UP = 0x26;
        public const int VK_DOWN = 0x28;
        public const int VK_NUMLOCK = 0x90;

        public const short KS_ON = 0x01;
        public const short KS_KEYDOWN = 0x80;

        public const int EM_GETCHARFORMAT = WM_USER + 58;
        public const int EM_SETCHARFORMAT = WM_USER + 68;

        public const int SCF_SELECTION = 0x0001;
        public const int SCF_WORD = 0x0002;
        public const int SCF_ALL = 0x0004;
        #endregion

        #region Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CHARFORMAT2_STRUCT
        {
            public UInt32 cbSize;
            public UInt32 dwMask;
            public UInt32 dwEffects;
            public Int32 yHeight;
            public Int32 yOffset;
            public Int32 crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szFaceName;
            public UInt16 wWeight;
            public UInt16 sSpacing;
            public int crBackColor; // Color.ToArgb() -> int
            public int lcid;
            public int dwReserved;
            public Int16 sStyle;
            public Int16 wKerning;
            public byte bUnderlineType;
            public byte bAnimation;
            public byte bRevAuthor;
            public byte bReserved1;
        }
        #endregion

        #region CHARFORMAT2 Flags
        public const UInt32 CFE_BOLD = 0x0001;
        public const UInt32 CFE_ITALIC = 0x0002;
        public const UInt32 CFE_UNDERLINE = 0x0004;
        public const UInt32 CFE_STRIKEOUT = 0x0008;
        public const UInt32 CFE_PROTECTED = 0x0010;
        public const UInt32 CFE_LINK = 0x0020;
        public const UInt32 CFE_AUTOCOLOR = 0x40000000;
        public const UInt32 CFE_SUBSCRIPT = 0x00010000;		/* Superscript and subscript are */
        public const UInt32 CFE_SUPERSCRIPT = 0x00020000;		/*  mutually exclusive			 */

        public const int CFM_SMALLCAPS = 0x0040;			/* (*)	*/
        public const int CFM_ALLCAPS = 0x0080;			/* Displayed by 3.0	*/
        public const int CFM_HIDDEN = 0x0100;			/* Hidden by 3.0 */
        public const int CFM_OUTLINE = 0x0200;			/* (*)	*/
        public const int CFM_SHADOW = 0x0400;			/* (*)	*/
        public const int CFM_EMBOSS = 0x0800;			/* (*)	*/
        public const int CFM_IMPRINT = 0x1000;			/* (*)	*/
        public const int CFM_DISABLED = 0x2000;
        public const int CFM_REVISED = 0x4000;

        public const int CFM_BACKCOLOR = 0x04000000;
        public const int CFM_LCID = 0x02000000;
        public const int CFM_UNDERLINETYPE = 0x00800000;		/* Many displayed by 3.0 */
        public const int CFM_WEIGHT = 0x00400000;
        public const int CFM_SPACING = 0x00200000;		/* Displayed by 3.0	*/
        public const int CFM_KERNING = 0x00100000;		/* (*)	*/
        public const int CFM_STYLE = 0x00080000;		/* (*)	*/
        public const int CFM_ANIMATION = 0x00040000;		/* (*)	*/
        public const int CFM_REVAUTHOR = 0x00008000;


        public const UInt32 CFM_BOLD = 0x00000001;
        public const UInt32 CFM_ITALIC = 0x00000002;
        public const UInt32 CFM_UNDERLINE = 0x00000004;
        public const UInt32 CFM_STRIKEOUT = 0x00000008;
        public const UInt32 CFM_PROTECTED = 0x00000010;
        public const UInt32 CFM_LINK = 0x00000020;
        public const UInt32 CFM_SIZE = 0x80000000;
        public const UInt32 CFM_COLOR = 0x40000000;
        public const UInt32 CFM_FACE = 0x20000000;
        public const UInt32 CFM_OFFSET = 0x10000000;
        public const UInt32 CFM_CHARSET = 0x08000000;
        public const UInt32 CFM_SUBSCRIPT = CFE_SUBSCRIPT | CFE_SUPERSCRIPT;
        public const UInt32 CFM_SUPERSCRIPT = CFM_SUBSCRIPT;

        public const byte CFU_UNDERLINENONE = 0x00000000;
        public const byte CFU_UNDERLINE = 0x00000001;
        public const byte CFU_UNDERLINEWORD = 0x00000002; /* (*) displayed as ordinary underline	*/
        public const byte CFU_UNDERLINEDOUBLE = 0x00000003; /* (*) displayed as ordinary underline	*/
        public const byte CFU_UNDERLINEDOTTED = 0x00000004;
        public const byte CFU_UNDERLINEDASH = 0x00000005;
        public const byte CFU_UNDERLINEDASHDOT = 0x00000006;
        public const byte CFU_UNDERLINEDASHDOTDOT = 0x00000007;
        public const byte CFU_UNDERLINEWAVE = 0x00000008;
        public const byte CFU_UNDERLINETHICK = 0x00000009;
        public const byte CFU_UNDERLINEHAIRLINE = 0x0000000A; /* (*) displayed as ordinary underline	*/

        public const int LVM_SETITEMCOUNT = 4143;
        public const int LVSICF_NOSCROLL = 2;
        #endregion

        #region Imported functions
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr IntPtr, int msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32")]
        public static extern int SendMessage(IntPtr IntPtr, int wMsg, int wParam, IntPtr lParam);
        [DllImport("user32")]
        public static extern int PostMessage(IntPtr IntPtr, int wMsg, int wParam, int lParam);
        [DllImport("user32")]
        public static extern short GetKeyState(int nVirtKey);
        [DllImport("user32")]
        public static extern int LockWindowUpdate(IntPtr IntPtr);
        #endregion
    }
}
