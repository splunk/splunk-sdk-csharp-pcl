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

namespace Splunk.Sdk.UnitTesting
{
    using Splunk.Sdk;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    internal class MockHttpMessageHandler : HttpMessageHandler
    {
        public MockHttpMessageHandler(string contextName, bool isRecording)
        {
            this.ContextName = contextName;
            this.IsRecording = isRecording;
        }

        public string ContextName { get; set; }

        public bool IsRecording { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            System.Threading.CancellationToken cancellationToken)
        {
            Task<HttpResponseMessage> returnTask = null;

            if (request.RequestUri.AbsoluteUri.Contains("services/auth/login"))
            {
                returnTask = Task.FromResult(GetLoginResponseMessages());
                goto returnall;
            }
            
            switch (this.ContextName)
            {
                case "delayed":
                    returnTask = Task.Factory.StartNew<HttpResponseMessage>(GetDelayedResponseMessages);
                    break;

                case "emptyResponse":
                default:
                    returnTask = Task.FromResult(GetDelayedResponseMessages());
                    break;
            }

            returnall: return returnTask;

        }

        private HttpResponseMessage GetResponseMessages()
        {
            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                ReasonPhrase = this.ContextName,
                Content =
                    new StringContent(string.Format("<feed><title>{0}</title></feed>",
                        this.ContextName))
            };

            return response;
        }
        private HttpResponseMessage GetLoginResponseMessages()
        {
            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                ReasonPhrase = this.ContextName,
                Content =
                    new StringContent(string.Format("<response><sessionKey>{0}</sessionKey></response>",
                        this.ContextName))
            };

            return response;
        }

        private HttpResponseMessage GetDelayedResponseMessages()
        {
            System.Threading.Thread.Sleep(2000);
            return this.GetResponseMessages();
        }
    }
}
