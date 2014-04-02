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
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class EntityCollection<TCollection, TEntity> : Resource<TCollection>, IReadOnlyList<TEntity> 
        where TCollection : EntityCollection<TCollection, TEntity>, new() 
        where TEntity : Resource<TEntity>, new()
    {
        #region Constructors

        internal EntityCollection(Context context, Namespace @namespace, ResourceName resource, IEnumerable<Argument> args = null)
            : base(context, @namespace, resource)
        {
            this.args = args;
        }

        public EntityCollection()
        { }

        #endregion

        #region Properties

        #region AtomFeed properties

        public override string Author
        {
            get { return this.data == null ? null : this.data.Author; }
        }

        public Version GeneratorVersion
        {
            get { return this.data == null ? null : this.data.GeneratorVersion; }
        }

        public override Uri Id
        {
            get { return this.data == null ? null : this.data.Id; }
        }

        public override IReadOnlyDictionary<string, Uri> Links
        {
            get { return this.data == null ? null : this.data.Links; }
        }

        public IReadOnlyList<Message> Messages
        {
            get { return this.data == null ? null : this.data.Messages; }
        }

        public Pagination Pagination
        {
            get { return this.data == null ? Pagination.Empty : this.data.Pagination; }
        }

        public override DateTime Updated
        {
            get { return this.data == null ? DateTime.MinValue : this.data.Updated; }
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

        #region Request-related methods

        public override void Initialize(Context context, Namespace @namespace, ResourceName resourceName, object atom)
        {
            var entry = atom as AtomEntry;

            Debug.Assert(entry != null, "Expected non-null entry of type AtomEntry");

            if (atom == null)
            {
                throw new ArgumentException("Expected non-null entry of type AtomEntry");
            }

            this.data = new Data(entry);
            base.Initialize(context, @namespace, new ResourceName(resourceName, entry.Title), atom);
        }

        /// <summary>
        /// 
        /// </summary>
        public override async Task UpdateAsync()
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

        #endregion

        #region Types

        class Data
        {
            #region Constructors

            public Data(AtomEntry entry)
            {
                this.author = entry.Author;
                this.id = entry.Id;
                this.generatorVersion = null; // TODO: figure out a way to inherit the enclosing feed's generator version or is it in each entry too?
                this.links = entry.Links;
                this.messages = null; // TODO: does an entry contain messages?
                this.pagination = Pagination.Empty;
                this.updated = entry.Updated;

                this.entities = new List<TEntity>();
            }

            public Data(Context context, Namespace @namespace, ResourceName resourceName, AtomFeed feed)
            {
                this.author = feed.Author;
                this.id = feed.Id;
                this.generatorVersion = feed.GeneratorVersion;
                this.links = feed.Links;
                this.messages = feed.Messages;
                this.pagination = Pagination.Empty;
                this.updated = feed.Updated;

                var entities = new List<TEntity>(feed.Entries.Count);

                foreach (var entry in feed.Entries)
                {
                    TEntity entity = new TEntity();

                    entity.Initialize(context, @namespace, resourceName, entry);
                    entities.Add(entity);
                }

                this.entities = entities;
            }

            #endregion

            #region Properties

            public string Author
            {
                get { return this.author; }
            }

            public Uri Id
            {
                get { return this.id; }
            }

            public IReadOnlyList<TEntity> Entities
            { 
                get { return this.entities; } 
            }

            public Version GeneratorVersion
            {
                get { return this.generatorVersion; }
            }

            public IReadOnlyDictionary<string, Uri> Links
            {
                get { return this.links; }
            }

            public IReadOnlyList<Message> Messages
            {
                get { return this.messages; }
            }

            public Pagination Pagination
            {
                get { return this.pagination; }
            }

            public DateTime Updated
            {
                get { return this.updated; }
            }

            #endregion

            #region Privates

            readonly IReadOnlyList<TEntity> entities;
            readonly string author;
            readonly Uri id;
            readonly Version generatorVersion;
            readonly IReadOnlyDictionary<string, Uri> links;
            readonly IReadOnlyList<Message> messages;
            readonly Pagination pagination;
            readonly DateTime updated;

            #endregion
        }

        #endregion
    }
}
