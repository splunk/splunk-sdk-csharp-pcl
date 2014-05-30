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
//// [O] contracts
//// [O] Documentation

namespace Splunk.Client.Refactored
{
    using Splunk.Client;

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a collection of saved searches.
    /// </summary>
    public class SavedSearchCollection : EntityCollection<SavedSearch>, ISavedSearchCollection
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedSearchCollection"/>
        /// class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is <c>null</c>.
        /// </exception>
        protected internal SavedSearchCollection(Service service)
            : base(service, ClassResourceName)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedSearchCollection"/> 
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <see cref="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        protected internal SavedSearchCollection(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedSearchCollection"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="args">
        /// Arguments for retrieving the <see cref="SavedSearchCollection"/>.
        /// </param>
        protected internal SavedSearchCollection(Context context, Namespace ns)
            : base(context, ns, ClassResourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "SavedSearchCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use <see cref=
        /// "Service.GetSavedSearchesAsync"/> to asynchronously retrieve a 
        /// collection of saved searches from Splunk.
        /// </remarks>
        public SavedSearchCollection()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public virtual IReadOnlyList<Message> Messages
        {
            get { return this.Snapshot.GetValue("Messages") ?? NoMessages; }
        }

        /// <inheritdoc/>
        public virtual Pagination Pagination
        {
            get { return this.Snapshot.GetValue("Pagination") ?? Pagination.None; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a new saved search.
        /// </summary>
        /// <param name="name">
        /// Name of the saved search to be created.
        /// </param>
        /// <param name="search">
        /// A Splunk search command.
        /// </param>
        /// <param name="attributes">
        /// Attributes of the saved search to be created.
        /// </param>
        /// <param name="dispatchArgs">
        /// Dispatch arguments for the saved search to be created.
        /// </param>
        /// <param name="templateArgs">
        /// Template arguments for the saved search to be created.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/EPQypw">POST 
        /// saved/searches</a> endpoint to create the <see cref="SavedSearch"/>
        /// represented by the current instance.
        /// </remarks>
        public virtual async Task<SavedSearch> CreateAsync(string name, string search, SavedSearchAttributes attributes, 
            SavedSearchDispatchArgs dispatchArgs = null, SavedSearchTemplateArgs templateArgs = null)
        {
            Contract.Requires<ArgumentNullException>(search != null);
            Contract.Requires<ArgumentNullException>(name != null);

            var args = new Argument[]
            { 
                new Argument("name", this.Name), 
                new Argument("search", search) 
            }
            .AsEnumerable();
            
            if (attributes != null)
            {
                args = args.Concat(attributes);
            }

            if (dispatchArgs != null)
            {
                args = args.Concat(dispatchArgs);
            }

            if (templateArgs != null)
            {
                args = args.Concat(templateArgs);
            }

            var savedSearch = await this.CreateAsync(args);
            return savedSearch;
        }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("saved", "searches");

        #endregion

        #region Types

        #endregion
    }
}
