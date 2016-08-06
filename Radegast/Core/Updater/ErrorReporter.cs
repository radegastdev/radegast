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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using OpenMetaverse;

namespace Radegast
{
    public class ErrorReporter
    {
        string url;
        StringBuilder postString;
        
        public ErrorReporter(string url)
        {
            this.url = url;
        }

        void AddStacktrace(ref StringBuilder report, Exception ex)
        {
            if (ex == null) return;

            report.AppendFormat("{0}: ", ex.ToString());
            report.AppendLine(ex.Message);
            report.AppendLine(ex.StackTrace);
            report.AppendLine();
            if (ex.InnerException != null && ex.InnerException != ex)
            {
                AddStacktrace(ref report, ex.InnerException);
            }
        }

        public void SendExceptionReport(Exception ex)
        {
            try
            {
                // Build the params we want to send
                postString = new StringBuilder();
                StringBuilder report = new StringBuilder();
                AddStacktrace(ref report, ex);
                AddPostField("report", report.ToString());
                AddPostField("version", RadegastBuild.VersionString);
                AddPostField("build", RadegastBuild.BuildName);

                // Send the request
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                byte[] postData = Encoding.UTF8.GetBytes(postString.ToString());
                request.ContentLength = postData.Length;
                request.ContentType = "application/x-www-form-urlencoded";
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(postData, 0, postData.Length);
                dataStream.Close();

                // Read the response
                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
                Logger.Log("Error reporting server said: " + responseFromServer, Helpers.LogLevel.Info);
            }
            catch (Exception e)
            {
                Logger.Log("Failed to send error report: " + e.Message, Helpers.LogLevel.Error, e);
            }
        }

        void AddPostField(string name, string value)
        {
            if (postString.Length > 0)
            {
                postString.Append("&");
            }

            postString.Append(name);
            postString.Append("=");
            postString.Append(System.Web.HttpUtility.UrlEncode(value));
        }

    }
}
