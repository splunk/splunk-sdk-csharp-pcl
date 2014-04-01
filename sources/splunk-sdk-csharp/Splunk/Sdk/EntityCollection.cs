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


// TODO:
// [O] Contracts
// [O] Documentation
// [ ] Define and set addtional properties of the EntityCollection (the stuff we get from the atom feed)
//     See http://docs.splunk.com/Documentation/Splunk/6.0.1/RESTAPI/RESTatom.

namespace Splunk.Sdk
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class EntityCollection<TEntity> : IReadOnlyList<TEntity> where TEntity : Entity<TEntity>, new()
    {
        #region Constructors

        internal EntityCollection(Context context, Namespace @namespace, ResourceName name, IEnumerable<Argument> args = null)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(@namespace != null, "name");

            this.Context = context;
            this.Namespace = @namespace;
            this.ResourceName = name;
            
            this.args = args;
        }

        #endregion

        #region Properties

        #region Request related properties

        /// <summary>
        /// Gets the <see cref="Context"/> instance for this <see cref="EntityCollection"/>.
        /// </summary>
        public Context Context
        { get; private set; }

        /// <summary>
        /// Gets the <see cref="Namespace"/> containing this <see cref="EntityCollection"/>.
        /// </summary>
        public Namespace Namespace
        { get; private set; }

        /// <summary>
        /// Gets the resource name of this <see cref="EntityCollection"/>.
        /// </summary>
        public ResourceName ResourceName
        { get; private set; }

        #endregion

        #region Properties backed by AtomFeed

        public string Author
        {
            get { return this.AtomFeed == null ? null : this.AtomFeed.Author; }
        }

        public Version GeneratorVersion
        {
            get { return this.AtomFeed == null ? null : this.AtomFeed.GeneratorVersion; }
        }

        public Uri Id
        {
            get { return this.AtomFeed == null ? null : this.AtomFeed.Id; }
        }

        public IReadOnlyDictionary<string, Uri> Links
        {
            get { return this.AtomFeed == null ? null : this.AtomFeed.Links; }
        }

        public IReadOnlyList<Message> Messages
        {
            get { return this.AtomFeed == null ? null : this.AtomFeed.Messages; }
        }

        public Pagination Pagination
        {
            get { return this.AtomFeed == null ? Pagination.Empty : this.AtomFeed.Pagination; }
        }

        public string Title
        {
            get { return this.AtomFeed == null ? null : this.AtomFeed.Title; }
        }

        public DateTime Updated
        {
            get { return this.AtomFeed == null ? DateTime.MinValue : this.AtomFeed.Updated; }
        }

        #endregion

        #region IReadOnlyList<TEntity> properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TEntity this[int index]
        {
            get
            {
                if (this.data == null)
                {
                    throw new InvalidOperationException();
                }
                return this.data.Entities[index];
            }
        }

        /// <summary>
        /// Gets the number of entites in this <see cref="EntityCollection"/>.
        /// </summary>
        public int Count
        {
            get
            {
                if (this.data == null)
                {
                    throw new InvalidOperationException();
                }
                return this.data.Entities.Count;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Request related methods

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            this.UpdateAsync().Wait();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task UpdateAsync()
        {
            using (Response response = await this.Context.GetAsync(this.Namespace, this.ResourceName, this.args))
            {
                if (response.Message.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader);
                this.data = new Data(this.Context, this.Namespace, this.ResourceName, feed);
            }
        }

        #endregion

        #region IReadOnlyList<TEntity> methods

        public IEnumerator<TEntity> GetEnumerator()
        {
            if (this.data == null)
            {
                throw new InvalidOperationException();
            }
            return this.data.Entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #endregion

        #region Privates

        readonly IEnumerable<Argument> args;
        volatile Data data;

        AtomFeed AtomFeed
        {
            get { return this.data == null ? null : this.data.Feed; }
        }

        #endregion

        #region Types

        class Data
        {
            public Data(Context context, Namespace @namespace, ResourceName resourceName, AtomFeed feed)
            {
                var entities = new List<TEntity>(feed.Entries.Count);
                
                foreach (var entry in feed.Entries)
                    entities.Add(Entity<TEntity>.CreateEntity(context, @namespace, resourceName, entry));

                this.entities = entities;
                this.feed = feed;
            }

            public IReadOnlyList<TEntity> Entities
            { 
                get { return this.entities; } 
            }
            
            public AtomFeed Feed
            { 
                get { return this.feed; } 
            }

            readonly IReadOnlyList<TEntity> entities;
            readonly AtomFeed feed;
        }

        #endregion
    }
}
