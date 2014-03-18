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
// [ ] Contracts
// [ ] Documentation

namespace Splunk.Sdk
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class EntityCollection<TEntity> : IReadOnlyList<TEntity> where TEntity : Entity<TEntity>, new()
    {
        #region Constructors

        internal EntityCollection(Context context, Namespace @namespace, ResourceName name, IEnumerable<KeyValuePair<string, object>> args)
        {
            this.Context = context;
            this.Namespace = @namespace;
            this.Name = name;

            if (args != null)
            {
                this.Arguments = args.ToDictionary(pair => pair.Key, pair => pair.Value);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TEntity this[int index]
        {
            get
            {
                if (this.feed == null)
                {
                    throw new InvalidOperationException();
                }
                return this.entities[index];
            }
        }

        /// <summary>
        /// Gets the <see cref="Context"/> instance for this <see cref="EntityCollection"/>.
        /// </summary>
        public Context Context
        { get; private set; }

        /// <summary>
        /// Gets the number of entites in this <see cref="EntityCollection"/>.
        /// </summary>
        public int Count
        {
            get
            {
                if (this.feed == null)
                {
                    throw new InvalidOperationException();
                }
                return this.entities.Count;
            }
        }

        /// <summary>
        /// Gets the resource name of this <see cref="EntityCollection"/>.
        /// </summary>
        public ResourceName Name
        { get; private set; }

        /// <summary>
        /// Gets the <see cref="Namespace"/> containing this <see cref="EntityCollection"/>.
        /// </summary>
        public Namespace Namespace
        { get; private set; }

        /// <summary>
        /// Gets the parameters for this <see cref="EntityCollection"/>.
        /// </summary>
        public IReadOnlyDictionary<string, object> Arguments
        { get; private set; }

        #endregion

        #region Methods

        public IEnumerator<TEntity> GetEnumerator()
        {
            if (this.feed == null)
            {
                throw new InvalidOperationException();
            }
            return this.entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (this.feed == null)
            {
                throw new InvalidOperationException();
            }
            return ((IEnumerable)this.entities).GetEnumerator();
        }

        public async Task UpdateAsync()
        {
#if false
            var document = await this.Context.GetDocumentAsync(this.Namespace, this.Name, this.Arguments);

            // TODO: Define and set addtional properties of the EntityCollection (the stuff we get from the atom feed)
            // See http://docs.splunk.com/Documentation/Splunk/6.0.1/RESTAPI/RESTatom
            
            var feed = new AtomFeed(document.Root);
            var entities = new List<TEntity>(this.feed.Entries.Count);
            entities.AddRange(from entry in this.feed.Entries select Entity<TEntity>.CreateEntity(this.Context, this.Name, entry));

            this.entities = entities;
            this.feed = feed;
#endif
            throw new NotImplementedException();
        }

        #endregion

        #region Privates

        IReadOnlyList<TEntity> entities;
        AtomFeed feed;

        #endregion
    }
}
