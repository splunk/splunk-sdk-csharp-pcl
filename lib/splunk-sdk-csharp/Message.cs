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

namespace Splunk.Sdk
{
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;

    public class Message
    {
        internal Message(XElement message)
        {
            Contract.Requires(message != null);
            Contract.Requires(message.Name == "msg");

            string type = message.Attribute("type").Value;

            switch (message.Attribute("type").Value)
            {
                case "DEBUG":
                    this.Type = MessageType.Debug;
                    break;
                case "INFO":
                    this.Type = MessageType.Information;
                    break;
                case "WARN":
                    this.Type = MessageType.Warning;
                    break;
                case "ERROR":
                    this.Type = MessageType.Error;
                    break;
                default:
                    Contract.Requires(false, string.Format("Unrecognized message type: {0}", type));
                    break;
            }
            
            this.Text = message.Value;
        }

        public string Text
        { get; private set; }

        public MessageType Type
        { get; private set; }
    }
}
