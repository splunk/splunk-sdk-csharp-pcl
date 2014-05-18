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
//// [ ] Construct from AtomFeed and update AtomEntry constructor to include
////     properties from AtomFeed that aren't in AtomEntry.
using PUrify;

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;

    /// <summary>
    /// Represents information about an entity at a point in time.
    /// </summary>
    public sealed class EntitySnapshot
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySnapshot"/> class.
        /// </summary>
        /// <param name="entry">
        /// An object representing a Splunk entity atom entry.
        /// </param>
        public EntitySnapshot(AtomEntry entry)
        {
            Contract.Requires<ArgumentNullException>(entry != null);

            if (entry.Content == null)
            {
                this.adapter = ExpandoAdapter.Empty;
            }
            else
            {
                dynamic content = entry.Content as ExpandoObject;

                if (content == null)
                {
                    content = new ExpandoObject();
                    content.Value = entry.Content;
                }

                this.adapter = new ExpandoAdapter(content);
            }

            this.author = entry.Author;
            this.id = entry.Id;
            this.links = entry.Links;
            this.published = entry.Published;
            this.title = entry.Title;
            this.updated = entry.Updated;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySnapshot"/> class.
        /// </summary>
        /// <param name="other">
        /// </param>
        /// <param name="content">
        /// </param>
        public EntitySnapshot(EntitySnapshot other, dynamic content)
        {
            Contract.Requires<ArgumentNullException>(other != null);

            if (content == null)
            {
                this.adapter = ExpandoAdapter.Empty;
            }
            else if (content is ExpandoObject)
            {
                this.adapter = new ExpandoAdapter(content);
            }
            else
            {
                dynamic expandoObject = new ExpandoObject();
                expandoObject.Value = content;
                this.adapter = new ExpandoAdapter(expandoObject);
            }

            this.author = other.Author;
            this.id = other.Id;
            this.links = other.Links;
            this.published = other.Published;
            this.title = other.Title;
            this.updated = other.Updated;
        }

        EntitySnapshot()
        {
            this.adapter = ExpandoAdapter.Empty;
            this.author = null;
            this.id = null;
            this.links = null;
            this.published = DateTime.MinValue;
            this.title = null;
            this.updated = DateTime.MinValue;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        public static readonly EntitySnapshot Missing = new EntitySnapshot();

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public ExpandoAdapter Adapter
        {
            get { return this.adapter; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Author
        {
            get { return this.author; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Uri Id
        {
            get { return this.id; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyDictionary<string, Uri> Links
        {
            get { return this.links; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Published
        {
            get { return this.published; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get { return this.title; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Updated
        {
            get { return this.updated; }
        }

        #endregion

        #region Privates/internals

        readonly ExpandoAdapter adapter;
        readonly string author;
        readonly Uri id;
        readonly IReadOnlyDictionary<string, Uri> links;
        readonly DateTime published;
        readonly string title;
        readonly DateTime updated;

        #endregion
    }
}
