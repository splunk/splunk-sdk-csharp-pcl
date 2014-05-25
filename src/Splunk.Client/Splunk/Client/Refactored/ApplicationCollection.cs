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

namespace Splunk.Client.Refactored
{
    using Splunk.Client;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a collection of Splunk applications.
    /// </summary>
    public class ApplicationCollection : EntityCollection<Application>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCollection"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="args">
        /// </param>
        internal ApplicationCollection(Service service)
            : base(service, ClassResourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ApplicationCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use <see cref=
        /// "Service.GetApplicationsAsync"/> to asynchronously retrieve a 
        /// collection of installed Splunk applications.
        /// </remarks>
        public ApplicationCollection()
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a new Splunk application from a template.
        /// </summary>
        /// <param name="template">
        /// 
        /// </param>
        /// <param name="attributes">
        /// 
        /// </param>
        /// <returns>
        /// An object representing the Splunk application created.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SzKzNX">POST 
        /// apps/local</a> endpoint to create the current <see cref=
        /// "Application"/>.
        /// </remarks>
        public async Task<Applicaion> CreateAsync(string template, ApplicationAttributes attributes = null)
        {
            var resourceName = ClassResourceName;

            var args = new CreationArgs()
            {
                ExplicitApplicationName = this.Title,
                Filename = false,
                Name = this.Title,
                Template = template
            };

            await this.CreateAsync(attributes == null ? args : args.Concat(attributes));
        }

        /// <summary>
        /// Asynchronously retrieves select entities from the list of entites
        /// in the current <see cref="EntityCollection&lt;TEntity&gt;"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// Following completion of the operation the list of entities in the
        /// current <see cref="EntityCollection&lt;TEntity&gt;"/> will contain 
        /// all changes since the select entites were last retrieved.
        /// </remarks>
        public virtual async Task GetSliceAsync(SelectionCriteria criteria)
        {
            await this.GetSliceAsync(criteria.AsEnumerable());
        }

        /// <summary>
        /// Asynchronously installs an application from a Splunk application
        /// archive file.
        /// </summary>
        /// <param name="path">
        /// Specifies the location of a Splunk application archive file.
        /// </param>
        /// <param name="name">
        /// Optionally specifies an explicit name for the application. This
        /// value overrides the name of the application as specified in the
        /// archive file.
        /// </param>
        /// <param name="update">
        /// <c>true</c> if Splunk should allow the installation to update an
        /// existing application. The default value is <c>false</c>.
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SzKzNX">POST 
        /// apps/local</a> endpoint to install the application from the archive
        /// file on <paramref name="path"/>.
        /// </remarks>
        public async Task<Application> InstallAsync(string path, string name = null, bool update = false)
        {
            var resourceName = ClassResourceName;

            var args = new CreationArgs()
            {
                ExplicitApplicationName = this.Title,
                Filename = true,
                Name = path,
                Update = update
            };

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, args))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
                await this.CreateAsync(response);
            }
        }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("apps", "local");

        #endregion

        #region Types

        /// <summary>
        /// Provides selection criteria for retrieving a slice of an <see cref=
        /// "ApplicationCollection"/>.
        /// </summary>
        /// <remarks>
        /// <para><b>References:</b></para>
        /// <list type="number">
        /// <item><description>
        ///   <a href="http://goo.gl/pqZJco">REST API: GET apps/local</a>
        /// </description></item>
        /// </list>
        /// </remarks>
        public sealed class SelectionCriteria : Args<SelectionCriteria>
        {
            #region Properties

            /// <summary>
            /// Gets or sets a value specifying the maximum number of <see cref=
            /// "Application"/> entries to return.
            /// </summary>
            /// <remarks>
            /// If the value of <c>Count</c> is set to zero, then all <see cref=
            /// "Application"/> entries are returned. The default value is 30.
            /// </remarks>
            [DataMember(Name = "count", EmitDefaultValue = false)]
            [DefaultValue(30)]
            public int Count
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the first result (inclusive) from 
            /// which to begin returning entries.
            /// </summary>
            /// <remarks>
            /// The <c>Offset</c> property is zero-based and cannot be negative. 
            /// The default value is zero.
            /// </remarks>
            /// <remarks>
            /// This value is zero-based and cannot be negative. The default value
            /// is zero.
            /// </remarks>
            [DataMember(Name = "offset", EmitDefaultValue = false)]
            [DefaultValue(0)]
            public int Offset
            { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to scan for new <see cref=
            /// "Application"/> instances and reload any objects those new <see 
            /// cref="Application"/> instances contain.
            /// </summary>
            /// <remarks>
            /// The default is <c>false</c>.
            /// </remarks>
            [DataMember(Name = "refresh", EmitDefaultValue = false)]
            [DefaultValue(false)]
            public bool Refresh // TODO: Verify default value (it's not in the docs)
            { get; set; }

            /// <summary>
            /// Gets or sets a search expression to filter <see cref="Application"/> 
            /// entries. 
            /// </summary>
            /// <remarks>
            /// Use this expression to filter the entries returned based on <see
            /// cref="Application"/> properties.
            /// </remarks>
            [DataMember(Name = "search", EmitDefaultValue = false)]
            [DefaultValue(null)]
            public string Search // TODO: Good search example for App
            { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to sort returned <see cref=
            /// "Application"/>entries in ascending or descending order.
            /// </summary>
            /// <remarks>
            /// The default value is <see cref="SortDirection"/>.Ascending.
            /// </remarks>
            [DataMember(Name = "sort_dir", EmitDefaultValue = false)]
            [DefaultValue(SortDirection.Ascending)]
            public SortDirection SortDirection
            { get; set; }

            /// <summary>
            /// Gets or sets a value specifying the <see cref="SortMode"/> for <see
            /// cref="Application"/> entries.
            /// </summary>
            /// <remarks>
            /// The default value is <see cref="SortMode"/>.Automatic.
            /// </remarks>
            [DataMember(Name = "sort_mode", EmitDefaultValue = false)]
            [DefaultValue(SortMode.Automatic)]
            public SortMode SortMode
            { get; set; }

            #endregion
        }

        #endregion
    }
}
