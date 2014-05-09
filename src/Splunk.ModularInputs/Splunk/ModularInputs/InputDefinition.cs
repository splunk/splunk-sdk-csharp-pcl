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

namespace Splunk.ModularInputs
{
    using System;
    using System.Collections.Generic;
    using Splunk.Client;

    public class InputDefinition
    {
        public string Name { get; set; }
        public IDictionary<string, Parameter> Parameters { get; set; }
        public string ServerHost { get; set; }
        public string ServerUri { get; set; }
        public string CheckpointDirectory { get; set; }
        public string SessionKey { get; set; }
        public Service Service
        {
            get
            {
                if (ServerUri == null)
                    throw new NullReferenceException("Cannot get a Service object without ServerUri");
                Uri parsedServerUri;
                if (Uri.TryCreate(ServerUri, UriKind.Absolute, out parsedServerUri))
                {
                    Splunk.Client.Scheme scheme;
                    if (parsedServerUri.Scheme.Equals("https"))
                        scheme = Splunk.Client.Scheme.Https;
                    else if (parsedServerUri.Scheme.Equals("http"))
                        scheme = Splunk.Client.Scheme.Http;
                    else
                        throw new FormatException("Invalid URI scheme: " + parsedServerUri.Scheme + "; expected http or https");
                    Service service = new Service(
                        scheme: scheme,
                        host: parsedServerUri.Host,
                        port: parsedServerUri.Port
                    );
                    service.SessionKey = SessionKey;
                    return service;
                }
                else
                {
                    throw new FormatException("Invalid server URI");
                }
            }
        }
    }
}
