/**
 * Radegast Metaverse Client
 * Copyright(c) 2009-2014, Radegast Development Team
 * Copyright(c) 2016-2020, Sjofn, LLC
 * All rights reserved.
 *  
 * Radegast is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.If not, see<https://www.gnu.org/licenses/>.
 */

using System;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;
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

            report.AppendFormat("{0}: ", ex);
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
                AddPostField("version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
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
