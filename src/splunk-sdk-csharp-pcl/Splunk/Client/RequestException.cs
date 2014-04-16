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

// TODO: Documentation

namespace Splunk.Client
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;

    /// <summary>
    /// The expception that is thrown when a Splunk service request fails.
    /// </summary>
    public class RequestException : HttpRequestException
    {
        #region Constructors

        internal RequestException(HttpResponseMessage message, IEnumerable<Message> details)
            : base(string.Format("{0}: {1}", (int)message.StatusCode, message.ReasonPhrase))
        {
            this.Details = new List<Message>(details ?? Enumerable.Empty<Message>());
            this.StatusCode = message.StatusCode;
        }

        internal RequestException(HttpResponseMessage message, params Message[] messages)
            : this(message, (IEnumerable<Message>)messages)
        { }

        #endregion

        #region Properties

        public IReadOnlyList<Message> Details
        { get; private set; }

        public HttpStatusCode StatusCode
        { get; private set; }

        #endregion
    }
}
