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

//// TODO: 
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// The expception that is thrown when a Splunk service request fails.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    public class RequestException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestException"/>
        /// class.
        /// </summary>
        /// <param name="message">
        /// An object representing an HTTP response message including the status
        /// code and data.
        /// </param>
        /// <param name="details">
        /// A sequence of <see cref="Message"/> instances detailing the cause
        /// of the <see cref="RequestException"/>.
        /// </param>
        protected internal RequestException(HttpResponseMessage message, IEnumerable<Message> details)
            : base(FormatMessageText(message, details))
        {
            Contract.Requires<ArgumentNullException>(message != null);

            this.Details = new List<Message>(details ?? Enumerable.Empty<Message>());
            this.StatusCode = message.StatusCode;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of Splunk messages detailing the cause of the current
        /// <see cref="RequestException"/>.
        /// </summary>
        /// <remarks>
        /// This list may be empty. Splunk does not provide <c>Details</c> all
        /// of the time.
        /// </remarks>
        public IReadOnlyList<Message> Details
        { get; private set; }

        /// <summary>
        /// Gets the <see cref="HttpStatusCode"/> for the current <see cref=
        /// "RequestException"/>.
        /// </summary>
        public HttpStatusCode StatusCode
        { get; private set; }

        #endregion

        #region Privates/internals

        static string FormatMessageText(HttpResponseMessage message, IEnumerable<Message> details)
        {
            StringBuilder builder = new StringBuilder(1024);

            builder.Append((int)message.StatusCode);
            builder.Append(": ");
            builder.Append(message.ReasonPhrase);

            if (details != null)
            {
                foreach (var detail in details)
                {
                    builder.Append("\n  ");
                    builder.Append(detail);
                }
            }

            return builder.ToString();
        }

        #endregion
    }
}
