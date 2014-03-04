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
// [ ] Pick up standard properties from AtomEntry on Update, not just AtomEntry.Content
//     See [Splunk responses to REST operations](http://goo.gl/tyXDfs).
// [ ] Check for HTTP Status Code 204 (No Content) and empty atoms in 
//     Entity<TEntity>.UpdateAsync.
// [O] Contracts
//
// [ ] Documentation

namespace Splunk.Sdk
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class Entity<TEntity> where TEntity : Entity<TEntity>, new()
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public Entity()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="namespace"></param>
        /// <param name="collection"></param>
        /// <param name="name"></param>
        protected Entity(Context context, Namespace @namespace, ResourceName collection, string name)
        {
            Contract.Requires<ArgumentNullException>(@namespace != null, "namespace");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name), "name");
            Contract.Requires<ArgumentException>(collection != null, "collection");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires(@namespace.IsSpecific);

            this.Context = context;
            this.name = name;
            this.@namespace = @namespace;
            this.Collection = collection;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the path to the collection containing this <see cref="Entity"/>.
        /// </summary>
        public ResourceName Collection
        { get; internal set; }

        /// <summary>
        /// Gets the <see cref="Context"/> instance for this <see cref="Entity"/>.
        /// </summary>
        public Context Context
        { get; internal set; }

        /// <summary>
        /// Gets the name of this <see cref="Entity"/>.
        /// </summary>
        public string Name
        { 
            get
            {
                if (this.name == null)
                {
                    if (this.record == null)
                    {
                        throw new InvalidOperationException(); // TODO: documentation
                    }
                    this.name = this.GetName(record);
                }
                return this.name;
            }
        }

        /// <summary>
        /// Gets the namespace containing this <see cref="Entity"/>.
        /// </summary>
        public Namespace Namespace
        {
            get
            {
                if (this.@namespace == null)
                {
                    if (this.record == null)
                    {
                        throw new InvalidOperationException(); // TODO: documentation
                    }
                    this.@namespace = new Namespace(record.Eai.Acl.Owner, record.Eai.Acl.App);
                }
                return this.@namespace;
            }
        }

        public ResourceName ResourceName
        {
            get
            {
                if (this.resourceName == null)
                {
                    this.resourceName = new ResourceName(this.Collection.Concat(new string[] { this.Name }));
                }
                return this.resourceName;
            }
        }

        /// <summary>
        /// Gets the state of this <see cref="Entity"/>.
        /// </summary>
        public virtual dynamic Record
        {
            get { return this.record; }

            internal set
            {
                this.record = value;
                this.Invalidate();
            }
        }

        #endregion

        #region Methods

        protected virtual string GetName(dynamic record)
        {
            return record.Title;
        }

        protected virtual void Invalidate()
        {
            this.resourceName = null;
            this.@namespace = null;
            this.name = null;
        }

        /// <summary>
        /// Refreshes the cached state of this <see cref="Entity"/>.
        /// </summary>
        public async Task UpdateAsync()
        {
            XDocument document = await this.Context.GetDocumentAsync(this.Namespace, this.ResourceName);

            // TODO: We need a response back to check these codes
            // Consider doing away with GetDocument* in favor of GetAsync.
#if false
            if response.code == 204 or response.body.nil?
            // This code is here primarily to handle the case of a job not yet being
            // ready, in which case you get back empty bodies.
            raise EntityNotReady.new((@resource + [name]).join("/"))
          end
#endif
            this.Record = new AtomEntry(document.Root).Content; // Gurantee: unique result because entities have specific namespaces
        }

        public override string ToString()
        {
            return string.Join("/", this.Context.ToString(), this.Namespace.ToString(), this.Collection.ToString(), this.Name);
        }

        #endregion

        #region Privates/internals

        ResourceName resourceName;
        Namespace @namespace;
        string name;
        dynamic record;

        internal static TEntity CreateEntity(Context context, ResourceName collection, AtomEntry entry)
        {
            Contract.Requires<ArgumentNullException>(collection != null, "collection");
            Contract.Requires<ArgumentNullException>(context != null, "context");
            Contract.Requires<ArgumentNullException>(entry != null, "entry");

            var entity = new TEntity()
            {
                Collection = collection,
                Context = context,
                Record = entry.Content
            };

            return entity;
        }

        #endregion
    }
}
