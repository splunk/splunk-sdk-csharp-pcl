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
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Xml.Linq;

    public class Entity
    {
        internal Entity(Context context, Namespace @namespace, IEnumerable<string> resource, string name, IDictionary<string, object> state = null)
        {
            Contract.Requires(@namespace == null || @namespace.IsSpecific);
            Contract.Requires(context != null);
            Contract.Requires(resource != null);
            Contract.Requires(!string.IsNullOrEmpty(name));

            this.Context = context;
            this.Namespace = @namespace;
            this.Resource = (IReadOnlyList<string>)resource;
            this.Name = name;
            this.State = (IReadOnlyDictionary<string, object>)state;

            this.resourceName = resource.Concat(new string[] { name }).ToArray();

            if (this.State == null)
                this.Refresh();
        }

        #region Properties

        /// <summary>
        /// Gets the <see cref="Context"/> instance for this <see cref="Entity"/>.
        /// </summary>
        public Context Context
        { get; private set; }

        /// <summary>
        /// Gets the name of this <see cref="Entity"/>.
        /// </summary>
        public string Name
        { get; private set; }

        /// <summary>
        /// Gets the namespace containing this <see cref="Entity"/>.
        /// </summary>
        public Namespace Namespace
        { get; private set; }

        /// <summary>
        /// Gets the path to the collection containing this <see cref="Entity"/>.
        /// </summary>
        public IReadOnlyList<string> Resource
        { get; private set; }

        /// <summary>
        /// Gets the state of this <see cref="Entity"/>.
        /// </summary>
        public IReadOnlyDictionary<string, object> State
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Refreshes the cached state of this <see cref="Entity"/>.
        /// </summary>
        public async void Refresh()
        {
            XDocument document = await this.Context.GetDocument(this.Namespace, this.resourceName);
        
#if false
            if response.code == 204 or response.body.nil?
            // This code is here primarily to handle the case of a job not yet being
            // ready, in which case you get back empty bodies.
            raise EntityNotReady.new((@resource + [name]).join("/"))
          end
#endif
          // Gurantee: unique result because entities have specific namespaces
          var feed = new AtomFeed(document);
          this.State = feed.Entries[0];
        }

        #endregion

        #region privates

        IReadOnlyList<string> resourceName;

        #endregion
    }
}
