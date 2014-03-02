// 
// Radegast Metaverse Client
// Copyright (c) 2009-2014, Radegast Development Team
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
// $Id$
//
using System;
using System.IO;
using log4net.Appender;
using log4net.Core;

namespace Radegast
{
    /// <summary>
    /// Writes log information out onto the console
    /// </summary>
    public class RadegastAppender : AnsiColorTerminalAppender
    {
        #region Events
        private static EventHandler<LogEventArgs> m_Log;

        protected static void OnLog(object sender, LogEventArgs e)
        {
            EventHandler<LogEventArgs> handler = m_Log;
            if (handler != null)
                try { handler(sender, e); }
                catch { }
        }

        private static readonly object m_LogLock = new object();

        /// <summary>Raised when the GridClient object in the main Radegast instance is changed</summary>
        public static event EventHandler<LogEventArgs> Log
        {
            add { lock (m_LogLock) { m_Log += value; } }
            remove { lock (m_LogLock) { m_Log -= value; } }
        }
        #endregion Events

        protected override void Append(LoggingEvent le)
        {
            try
            {
                if (m_Log != null)
                    OnLog(this, new LogEventArgs(le));
                
                Console.Write("{0} [", le.TimeStamp.ToString("HH:mm:ss"));
                WriteColorText(DeriveColor(le.Level.Name), le.Level.Name);
                Console.Write("]: - ");

                if (le.Level == Level.Error)
                {
                    WriteColorText(ConsoleColor.Red, le.MessageObject.ToString());
                }
                else if (le.Level == Level.Warn)
                {
                    WriteColorText(ConsoleColor.Yellow, le.MessageObject.ToString());
                }
                else
                {
                    Console.Write(le.MessageObject.ToString());
                }
                Console.WriteLine();

                if (RadegastInstance.GlobalInstance.GlobalLogFile != null && (!RadegastInstance.GlobalInstance.GlobalSettings.ContainsKey("log_to_file") || RadegastInstance.GlobalInstance.GlobalSettings["log_to_file"]))
                    File.AppendAllText(RadegastInstance.GlobalInstance.GlobalLogFile, RenderLoggingEvent(le) + Environment.NewLine);
            }
            catch (Exception) { }
        }

        private void WriteColorText(ConsoleColor color, string sender)
        {
            try
            {
                lock (this)
                {
                    try
                    {
                        Console.ForegroundColor = color;
                        Console.Write(sender);
                        Console.ResetColor();
                    }
                    catch (ArgumentNullException)
                    {
                        // Some older systems dont support coloured text.
                        Console.WriteLine(sender);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private static ConsoleColor DeriveColor(string input)
        {
            int colIdx = (input.ToUpper().GetHashCode() % 6) + 9;
            return (ConsoleColor)colIdx;
        }
    }

    public class LogEventArgs : EventArgs
    {
        public LoggingEvent LogEntry;

        public LogEventArgs(LoggingEvent e)
        {
            LogEntry = e;
        }
    }
}

