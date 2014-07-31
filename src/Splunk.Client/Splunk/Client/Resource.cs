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
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Dynamic;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Provides a base class that represents a Splunk resource as an object.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.BaseResource"/>
    public class Resource : BaseResource
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="entry">
        /// An object representing a Splunk atom entry response.
        /// </param>
        /// <param name="generatorVersion">
        /// The version of the generator producing the <see cref="AtomFeed"/>
        /// feed containing <paramref name="entry"/>.
        /// </param>
        protected internal Resource(AtomEntry entry, Version generatorVersion)
            : base(entry, generatorVersion)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        protected internal Resource(AtomFeed feed)
        {
            this.Initialize(feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="expandObject">
        /// An object containing the dynamic members of the newly created
        /// <see cref="Resource"/>.
        /// </param>
        protected Resource(ExpandoObject expandObject)
            : base(expandObject)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref= "Resource"/>
        /// class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public Resource()
        { }

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected internal override void Initialize(AtomEntry entry, Version generatorVersion)
        {
            BaseResource.Initialize(this, entry, generatorVersion);
        }

        /// <summary>
        /// Infrastructure. Initializes the current uninitialized
        /// <see cref= "Resource"/>.
        /// </summary>
        /// <remarks>
        /// This method may be called once to initialize a <see cref="Resource"/>
        /// instantiated by the default constructor. Override this method to provide
        /// special initialization code. Call this base method before initialization
        /// is complete.
        /// <note type="note">
        /// This method supports the Splunk client infrastructure and is not intended
        /// to be used directly from your code.
        /// </note>
        /// </remarks>
        /// <exception cref="InvalidDataException">
        /// Thrown when an Invalid Data error condition occurs.
        /// </exception>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        /// <seealso cref="M:Splunk.Client.BaseResource.Initialize(AtomFeed)"/>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="feed"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The current <see cref="Resource"/> is already initialized.
        /// </exception>
        protected internal override void Initialize(AtomFeed feed)
        {
            if (feed.Entries.Count != 1)
            {
                var text = string.Format(CultureInfo.CurrentCulture, "feed.Entries.Count = {0}", feed.Entries.Count);
                throw new InvalidDataException(text);
            }

            this.Initialize(feed.Entries[0], feed.GeneratorVersion);
        }
        
        #endregion
    }
}
