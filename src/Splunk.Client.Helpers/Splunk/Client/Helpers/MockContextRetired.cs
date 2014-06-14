/*
 * Copyright 2014 Splunk, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"): you may
 * not use this file except in compliance with the License. You may obtain
 * a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 */

namespace Splunk.Client.Helpers
{
    using Splunk.Client;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Provides a class for faking HTTP requests and responses from a Splunk server.
    /// </summary>
    public class MockContextRetired : Context
    {
        #region Constructors

        public MockContextRetired(Scheme protocol, string host, int port)
            : base(protocol, host, port, new MockHttpMessageHandler(FilePath, IsRecording))
        {
            this.LoadRequestResponseXml();
        }

        #endregion

        #region Properties

        public static string FilePath
        {
            get { return filePath; }
        }

        public static bool IsEnabled
        { 
            get { return isEnabled; }
        }

        public static bool IsRecording
        { 
            get { return isRecording; }
        }

        public string Name
        {
            get { return "mock-context"; }
        }

        #endregion

        #region Methods

        public override async Task<Response> GetAsync(Namespace ns, ResourceName resource, params IEnumerable<Argument>[] argumentSets)
        {
            var response = await this.SendAsync(HttpMethod.Get, ns, resource, null, CancellationToken.None, argumentSets);
            return response;
        }

        public override async Task<Response> GetAsync(Namespace ns, ResourceName resourceName, CancellationToken token, params IEnumerable<Argument>[] argumentSets)
        {
            var response = await this.SendAsync(HttpMethod.Get, ns, resourceName, null, token, argumentSets);
            return response;
        }

        public override async Task<Response> PostAsync(Namespace ns, ResourceName resource, params IEnumerable<Argument>[] argumentSets)
        {
            var content = this.CreateStringContent(argumentSets);
            return await PostAsync(ns, resource, content, null);
        }

        public override async Task<Response> PostAsync(Namespace ns, ResourceName resource, HttpContent content, params IEnumerable<Argument>[] argumentSets)
        {
            var response = await this.SendAsync(HttpMethod.Post, ns, resource, content, CancellationToken.None, argumentSets);
            return response;
        }

        public override async Task<Response> DeleteAsync(Namespace ns, ResourceName resource, params IEnumerable<Argument>[] argumentSets)
        {
            var response = await this.SendAsync(HttpMethod.Delete, ns, resource, null, CancellationToken.None, argumentSets);
            return response;
        }

        #endregion

        #region Privates/internals

        static readonly Dictionary<string, string> requestResponse = new Dictionary<string, string>();
        static readonly string filePath;
        static readonly bool isEnabled;
        static readonly bool isRecording;

        string savedRequestContent;
        string savedRequestString;

        static MockContextRetired()
        {
            filePath = ConfigurationManager.AppSettings["MockContext.FilePath"];
            isEnabled = bool.Parse(ConfigurationManager.AppSettings["MockContext.IsEnabled"]);
            isRecording = bool.Parse(ConfigurationManager.AppSettings["MockContext.IsRecording"]);
        }

        async Task<Response> SendAsync(HttpMethod method, Namespace ns, ResourceName resource, HttpContent content, CancellationToken cancellationToken, IEnumerable<Argument>[] argumentSets)
        {
            this.savedRequestString = "";
            var serviceUri = this.CreateServiceUri(ns, resource, argumentSets);

            using (var request = new HttpRequestMessage(method, serviceUri) { Content = content })
            {
                if (this.SessionKey != null)
                {
                    request.Headers.Add("Authorization", string.Concat("Splunk ", this.SessionKey));
                }

                HttpResponseMessage response;

                if (!IsRecording)
                {
                    response = this.CreateMockResponse();
                }
                else
                {
                    HttpClient httpclient = new HttpClient();
                    response = await httpclient.SendAsync(request);

                    if (!requestResponse.ContainsKey(this.savedRequestString))
                    {
                        XmlDocument doc = new XmlDocument();

                        doc.Load(filePath);

                        XmlNode root = doc.LastChild;
                        StringBuilder innertext = new StringBuilder();
                        innertext.AppendLine(string.Format("<UserRequest>{0}+{1}</UserRequest>", this.savedRequestString, this.savedRequestContent));

                        using (MemoryStream stream = new MemoryStream())
                        {
                            await response.Content.CopyToAsync(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            StreamReader reader = new StreamReader(stream);
                            string str = reader.ReadToEnd();
                            str = str.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n", "");
                            innertext.AppendLine(string.Format("<UserResponse><StatusCode>{0}</StatusCode><ReasonPhrase>{1}</ReasonPhrase><Content>{2}</Content></UserResponse>", response.StatusCode, response.ReasonPhrase, str));
                        }

                        XmlElement ele = doc.CreateElement("RequestResponse");
                        ele.InnerXml = innertext.ToString();
                        doc.DocumentElement.AppendChild(ele);
                        doc.Save(filePath);
                    }
                }

                this.savedRequestContent = string.Empty;
                this.savedRequestString = string.Empty;

                return await Response.CreateAsync(response);
            }
        }

        Uri CreateServiceUri(Namespace ns, ResourceName name, params IEnumerable<Argument>[] argumentSets)
        {
            var builder = new StringBuilder(this.ToString());

            builder.Append("/");
            builder.Append(ns.ToUriString());
            builder.Append("/");
            builder.Append(name.ToUriString());

            if (argumentSets != null)
            {
                var query = string.Join("&",
                    from args in argumentSets
                    where args != null
                    from arg in args
                    select string.Join("=", Uri.EscapeDataString(arg.Name), Uri.EscapeDataString(arg.Value.ToString())));

                if (query.Length > 0)
                {
                    builder.Append('?');
                    builder.Append(query);
                }
            }

            this.savedRequestString = builder.ToString();
            this.savedRequestString = this.RemoveGuidPattern(this.savedRequestString);

            var uri = UriConverter.Instance.Convert(this.savedRequestString);
            return uri;
        }

        StringContent CreateStringContent(params IEnumerable<Argument>[] argumentSets)
        {
            this.savedRequestContent = "";

            if (argumentSets == null)
            {
                return new StringContent(string.Empty);
            }

            var body = string.Join("&",
                from args in argumentSets
                where args != null
                from arg in args
                select string.Join("=", Uri.EscapeDataString(arg.Name), Uri.EscapeDataString(arg.Value.ToString())));

            body = this.RemoveGuidPattern(body);
            var stringContent = new StringContent(body);

            //the below process is only for to save the request string to requestResponse.xml        
            this.savedRequestContent = this.RemoveSpecialChar(body);

            return stringContent;
        }

        string RemoveGuidPattern(string body)
        {
            // remove Guid pattern sent by the test so that we don't hit the request not found issue
            string regexp = @"[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}";
            if (Regex.IsMatch(body, regexp))
            {
                body = body.Replace(Regex.Match(body, regexp).Value, new Guid().ToString());
            }

            return body;
        }

        string RemoveSpecialChar(string body)
        {
            // remove '&' so that we can save the string into xml file
            body = body.Replace("&", ",");
            return body;
        }

        void LoadRequestResponseXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlElement root = doc.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("RequestResponse");
            foreach (XmlNode node in nodes)
            {
                string request = node.ChildNodes[0].InnerXml;
                string response = node.ChildNodes[1].InnerXml;

                if (!requestResponse.ContainsKey(request))
                {
                    requestResponse.Add(request, response);
                }
            }
        }

        HttpResponseMessage CreateMockResponse()
        {
            var response = new HttpResponseMessage();
            //this.savedRequestString = this.RemoveGuidPattern(this.savedRequestString);
            //this.savedRequestContent = this.RemoveSpecialChar(this.savedRequestContent);

            string requestKey = string.Format("{0}+{1}", this.savedRequestString, this.savedRequestContent);
            if (requestResponse.ContainsKey(requestKey))
            {
                string str = string.Format("<xml>{0}</xml>", requestResponse[requestKey]);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);

                XmlNode node = doc.DocumentElement;
                response.StatusCode = (System.Net.HttpStatusCode)Enum.Parse(typeof(System.Net.HttpStatusCode), node.FirstChild.InnerText);
                response.ReasonPhrase = node.FirstChild.NextSibling.InnerText;
                response.Content = new StringContent(node.FirstChild.NextSibling.NextSibling.InnerXml);
            }
            else
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                response.ReasonPhrase = "not created the user response yet in the file, looking for " + requestKey;
                response.Content = new StringContent("");
            }

            return response;
        }

        #endregion
    }
}
