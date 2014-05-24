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
//// [O] Documentation (check all links!)
//// [ ] Consider schema validation from schemas stored as resources.
////     See [XmlReaderSettings.Schemas Property](http://goo.gl/Syvj4V)
//// [ ] Strong type and tests for Service.GetCapabilities


namespace Splunk.Client.Refactored
{
    using Splunk.Client;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public class Service : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        /// <param name="context">
        /// The context for requests by the new <see cref="Service"/>.
        /// </param>
        /// <param name="ns">
        /// The namespace for requests by the new <see cref="Service"/>. The
        /// default value is <c>null</c> indicating that <see cref=
        /// "Namespace"/>.Default should be used.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public Service(Context context, Namespace ns = null)
        {
            Contract.Requires<ArgumentNullException>(context != null, "context");

            this.context = context;
            this.ns = ns ?? Namespace.Default;

            this.configurations = new ConfigurationCollection(this);
            this.applications = new ApplicationCollection(this);
            this.receiver = new Receiver(this);
            this.server = new Server(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        /// <param name="scheme">
        /// The scheme for the new <see cref="Service"/>.
        /// </param>
        /// <param name="host">
        /// The host for the new <see cref="Service"/>.
        /// </param>
        /// <param name="port">
        /// The port for the new <see cref="Service"/>.
        /// </param>
        /// <param name="ns">
        /// The namespace for requests issue by the new <see cref="Service"/>.
        /// </param>
        /// <exception name="ArgumentException">
        /// <paramref name="scheme"/> is invalid, <paramref name="host"/> is
        /// <c>null</c> or empty, or <paramref name="port"/> is less than zero
        /// or greater than <c>65535</c>.
        /// </exception>
        public Service(Scheme scheme, string host, int port, Namespace ns = null)
            : this(new Context(scheme, host, port), ns)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="Context"/> instance for this <see cref="Service"/>.
        /// </summary>
        protected internal Context Context
        {
            get { return this.context; }
        }

        /// <summary>
        /// Gets the <see cref="Namespace"/> used by this <see cref="Service"/>.
        /// </summary>
        public Namespace Namespace
        {
            get { return this.ns; }
        }

        /// <summary>
        /// 
        /// </summary>

        public ApplicationCollection Applications
        {
            get { return this.applications; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Receiver Receiver
        {
            get { return this.receiver; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Server Server
        {
            get { return this.server; }
        }

        /// <summary>
        /// Gets or sets the session key used by this <see cref="Service"/>.
        /// </summary>
        public string SessionKey
        {
            get { return this.Context.SessionKey; }
            set { this.Context.SessionKey = value; }
        }

        #endregion

        #region Methods

        #region Access Control

        /// <summary>
        /// Asynchronously creates a new storage password.
        /// </summary>
        /// <param name="password">
        /// Storage password.
        /// </param>
        /// <param name="name">
        /// Storage password name.
        /// </param>
        /// <param name="realm">
        /// Storage password realm.
        /// </param>
        /// <returns>
        /// An object representing the storage password created.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/JgyIeN">POST 
        /// storage/passwords</a> endpoint to construct the <see cref=
        /// "StoragePassword"/> it returns.
        /// </remarks>
        public async Task<StoragePassword> CreateStoragePasswordAsync(string password, string name, string realm = null)
        {
            var resource = new StoragePassword(this.Context, this.Namespace, name, realm);
            await resource.CreateAsync(password);
            return resource;
        }

        /// <summary>
        /// Asynchronously retrieves the list of all Splunk system capabilities.
        /// </summary>
        /// <returns>
        /// An object representing the list of all Splunk system capabilities.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/kgTKvM">GET 
        /// authorization/capabilities</a> endpoint to construct a list of all 
        /// Splunk system capabilities.
        /// </remarks>
        public async Task<dynamic> GetCapabilitiesAsync()
        {
            using (var response = await this.Context.GetAsync(this.Namespace, AuthorizationCapabilities))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);

                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader);

                if (feed.Entries.Count != 1)
                {
                    throw new InvalidDataException(); // TODO: Diagnostics : cardinality violation
                }

                var entry = feed.Entries[0];
                dynamic capabilities = entry.Content.Capabilities; // TODO: Static type (?)

                return capabilities;
            }
        }

        /// <summary>
        /// Asynchronously retrieves a storage password.
        /// </summary>
        /// <param name="name">
        /// Storage password name.
        /// </param>
        /// <param name="realm">
        /// Storage password realm.
        /// </param>
        /// <returns>
        /// An object representing the storage password retrieved.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/HL3c0T">GET 
        /// storage/passwords/{name}</a> endpoint to construct the <see cref=
        /// "StoragePassword"/> it returns.
        /// </remarks>
        public async Task<StoragePassword> GetStoragePasswordAsync(string name, string realm = null)
        {
            var resource = new StoragePassword(this.Context, this.Namespace, name, realm);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously retrieves a collection of storage passwords.
        /// </summary>
        /// <returns>
        /// An object representing the collection of storage passwords retrieved.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/XhebMI">GET 
        /// storage/passwords</a> endpoint to construct the <see cref=
        /// "StoragePasswordCollection"/> it returns.
        /// </remarks>
        public async Task<StoragePasswordCollection> GetStoragePasswordsAsync(StoragePasswordCollectionArgs args = null)
        {
            var resource = new StoragePasswordCollection(this.Context, this.Namespace, args);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Provides user authentication asynchronously.
        /// </summary>
        /// <param name="username">
        /// Splunk account username.
        /// </param>
        /// <param name="password">
        /// Splunk account password for username.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/hdNhwA">POST 
        /// auth/login</a> endpoint. The session key this endpoint returns is 
        /// used for subsequent requests. It is accessible via the <see cref=
        /// "SessionKey"/> property.
        /// </remarks>
        public async Task LoginAsync(string username, string password)
        {
            Contract.Requires<ArgumentNullException>(username != null);
            Contract.Requires<ArgumentNullException>(password != null);

            using (var response = await this.Context.PostAsync(Namespace.Default, AuthLogin, new Argument[]
            {
                new Argument("username", username),
                new Argument("password", password)
            }))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                this.SessionKey = await response.XmlReader.ReadResponseElementAsync("sessionKey");
            }
        }

        /// <summary>
        /// Ends the session by associated with the current instance.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/hdNhwA">DELETE 
        /// authentication/httpauth-tokens/{name}</a> endpoint to end the
        /// the session by removing <see cref="SessionKey"/>.
        /// </remarks>
        public async Task LogoffAsync()
        {
            Contract.Requires<InvalidOperationException>(this.SessionKey != null);

            var resourceName = new ResourceName(AuthenticationHttpAuthTokens, this.SessionKey);

            using (var response = await this.Context.DeleteAsync(Namespace.Default, resourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                this.SessionKey = null;
            }
        }

        /// <summary>
        /// Asynchronously removes a storage password.
        /// </summary>
        /// <param name="name">
        /// Storage password name.
        /// </param>
        /// <param name="realm">
        /// Storage password realm.
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/JGm0JP">DELETE 
        /// storage/passwords/{name}</a> endpoint to remove the <see cref=
        /// "StoragePassword"/> identified by <paramref name="name"/>.
        /// </remarks>
        public async Task RemoveStoragePasswordAsync(string name, string realm = null)
        {
            var resource = new StoragePassword(this.Context, this.Namespace, name, realm);
            await resource.RemoveAsync();
        }

        /// <summary>
        /// Asynchronously updates a storage password.
        /// </summary>
        /// <param name="password">
        /// Storage password.
        /// </param>
        /// <param name="name">
        /// Storage password name.
        /// </param>
        /// <param name="realm">
        /// Storage password realm.
        /// </param>
        /// <returns>
        /// An object representing the updated storage password.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/HL3c0T">POST 
        /// storage/passwords/{name}</a> endpoint to update the storage
        /// password identified by <paramref name="name"/>.
        /// </remarks>
        public async Task<StoragePassword> UpdateStoragePasswordAsync(string password, string name, string realm = null)
        {
            var resource = new StoragePassword(this.Context, this.Namespace, name, realm);
            await resource.UpdateAsync(password);
            return resource;
        }

        #endregion

        #region Applications

        /// <summary>
        /// Asynchronously creates an application from an application template.
        /// </summary>
        /// <param name="name">
        /// Name of the application to create.
        /// </param>
        /// <param name="template">
        /// 
        /// </param>
        /// <param name="attributes">
        /// 
        /// </param>
        /// <returns>
        /// An object representing the application created.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SzKzNX">POST 
        /// apps/local</a> endpoint to construct the <see cref=
        /// "Application"/> object it returns.
        /// </remarks>
        public async Task<Application> CreateApplicationAsync(string name, string template, 
            ApplicationAttributes attributes = null)
        {
            var resource = new Application(this.Context, this.Namespace, name);
            await resource.CreateAsync(template, attributes);
            return resource;
        }

        /// <summary>
        /// Asynchronously retrieves an <see cref="Application"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the application to retrieve.
        /// </param>
        /// <returns>
        /// An object representing the application retrieved.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SzKzNX">GET 
        /// apps/local/{name}</a> endpoint to construct the <see cref=
        /// "Application"/> object it returns.
        /// </remarks>
        public async Task<Application> GetApplicationAsync(string name)
        {
            var resource = new Application(this.Context, this.Namespace, name);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously retrieves setup information for an <see cref=
        /// "Application"/> identified by name.
        /// </summary>
        /// <param name="name">
        /// Name of the application for which to retrieve setup information.
        /// </param>
        /// <returns>
        /// An object representing the setup information retrieved.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SzKzNX">GET 
        /// apps/local/{name}/setup</a> endpoint to construct the <see cref=
        /// "ApplicationSetupInfo"/> object it returns.
        /// </remarks>
        public async Task<ApplicationSetupInfo> GetApplicationSetupInfoAsync(string name)
        {
            var resource = new ApplicationSetupInfo(this.Context, this.Namespace, name);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously retrieves update information for an <see cref=
        /// "Application"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the application for which to retrieve update information.
        /// </param>
        /// <returns>
        /// An object representing the update information retrieved.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SzKzNX">GET 
        /// apps/local/{name}/setup</a> endpoint to construct the <see cref=
        /// "ApplicationUpdateInfo"/> object it returns.
        /// </remarks>
        public async Task<ApplicationUpdateInfo> GetApplicationUpdateInfoAsync(string name)
        {
            var resource = new ApplicationUpdateInfo(this.Context, this.Namespace, name);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously creates an application from an application package.
        /// </summary>
        /// <param name="name">
        /// Name of the application to create.
        /// </param>
        /// <param name="path">
        /// 
        /// </param>
        /// <param name="update">
        /// 
        /// </param>
        /// <returns>
        /// An object representing the application created.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SzKzNX">POST 
        /// apps/local</a> endpoint to construct the <see cref="Application"/>
        /// object it returns.
        /// </remarks>
        public async Task<Application> InstallApplicationAsync(string name, string path, bool update = false)
        {
            var resource = new Application(this.Context, this.Namespace, name);
            await resource.InstallAsync(path, update);
            return resource;
        }

        /// <summary>
        /// Asynchronously creates an application archive.
        /// </summary>
        /// <param name="name">
        /// Name of the application to be archived.
        /// </param>
        /// <returns>
        /// An object containing information about the archive created.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SzKzNX">GET 
        /// apps/local/{name}/package</a> endpoint to create an archive of the
        /// application identified by <paramref name="name"/> and construct the <see 
        /// cref="ApplicationArchiveInfo"/> object it returns.
        /// </remarks>
        public async Task<ApplicationArchiveInfo> PackageApplicationAsync(string name)
        {
            var resource = new ApplicationArchiveInfo(this.Context, this.Namespace, name);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously removes an <see cref="Application"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the application to remove.
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SzKzNX">DELETE 
        /// apps/local/{name}</a> endpoint to remove the <see cref=
        /// "Application"/> object identified by <paramref name="name"/>.
        /// </remarks>
        public async Task RemoveApplicationAsync(string name)
        {
            var resource = new Application(this.Context, this.Namespace, name);
            await resource.RemoveAsync();
        }

        /// <summary>
        /// Asynchronously updates the attributes of an <see cref="Application"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the application to update.
        /// </param>
        /// <param name="attributes">
        /// New attributes for the application identified by <paramref name=
        /// "name"/>.
        /// </param>
        /// <returns>
        /// An object representing the application that was updated.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SzKzNX">POST 
        /// apps/local/{name}</a> endpoint to update the <see cref=
        /// "Application"/> object identified by <paramref name="name"/>.
        /// </remarks>
        public async Task<Application> UpdateApplicationAsync(string name, ApplicationAttributes attributes)
        {
            var resource = new Application(this.Context, this.Namespace, name);
            await resource.RemoveAsync();
            return resource;
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Asynchronously creates a new configuration file.
        /// </summary>
        /// <param name="name">
        /// Name of the configuration file to create.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/CBWes7">POST 
        /// properties</a> endpoint to create the <see cref="Configuration"/>
        /// identified by <paramref name="name"/>.
        /// </remarks>
        public async Task<Configuration> CreateConfigurationAsync(string name)
        {
            var entity = new Configuration(this.Context, this.Namespace, name);
            await entity.CreateAsync();
            return entity;
        }

        /// <summary>
        /// Asynchronously creates a new stanza in a configuration file.
        /// </summary>
        /// <param name="fileName">
        /// Name of a configuration file.
        /// </param>
        /// <param name="stanzaName">
        /// Name of the configuration stanza to create.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/jae44k">POST 
        /// properties/{file_name></a> endpoint to create the <see cref=
        /// "ConfigurationStanza"/> identified by <paramref name="fileName"/> and
        /// <paramref name="stanzaName"/>.
        /// </remarks>
        public async Task<ConfigurationStanza> CreateConfigurationStanzaAsync(string fileName, string stanzaName)
        {
            var entity = new ConfigurationStanza(this.Context, this.Namespace, fileName, stanzaName);
            await entity.CreateAsync();
            return entity;
        }

        /// <summary>
        /// Asynchronously retrieves a single configuration setting.
        /// </summary>
        /// <returns>
        /// An object representing the configuration setting.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/1jeyog">GET 
        /// properties/{file_name}/{stanza_name}/{key_name}</a> endpoint/> to 
        /// construct the <see cref="ConfigurationSetting"/> it returns.
        /// </remarks>
        public async Task<ConfigurationSetting> GetConfigurationSettingAsync(string fileName, string stanzaName, string keyName)
        {
            var entity = new ConfigurationSetting(this.Context, this.Namespace, fileName, stanzaName, keyName);
            await entity.GetAsync();
            return entity;
        }

        /// <summary>
        /// Asynchronously retrieves a configuration stanza.
        /// </summary>
        /// <returns>
        /// An object representing the configuration stanza.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/sM63fa">GET 
        /// properties/{file_name}/{stanza_name}</a> endpoint/> to construct
        /// the <see cref="ConfigurationStanza"/> it returns.
        /// </remarks>
        public async Task<ConfigurationStanza> GetConfigurationStanzaAsync(string fileName, string stanzaName)
        {
            var collection = new ConfigurationStanza(this.Context, this.Namespace, fileName, stanzaName);
            await collection.GetAsync();
            return collection;
        }

        /// <summary>
        /// Asynchronously removes a configuration stanza.
        /// </summary>
        /// <param name="fileName">
        /// Name of a configuration file.
        /// </param>
        /// <param name="stanzaName">
        /// Name of a configuration stanza in <paramref name="fileName"/> to be
        /// removed.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/dpbuhQ">DELETE
        /// configs/conf-{file}/{name}</a> endpoint to remove the configuration
        /// identified by <paramref name="stanzaName"/>.
        /// </remarks>
        public async Task RemoveConfigurationStanzaAsync(string fileName, string stanzaName)
        {
            var entity = new ConfigurationStanza(this.Context, this.Namespace, fileName, stanzaName);
            await entity.RemoveAsync();
        }

        /// <summary>
        /// Asynchronously updates an existing configuration setting.
        /// </summary>
        /// <param name="fileName">
        /// Name of a configuration file.
        /// </param>
        /// <param name="stanzaName">
        /// Name of a configuration stanza.
        /// </param>
        /// <param name="keyName">
        /// Name of the configuration setting to update.
        /// </param>
        /// <param name="value">
        /// A new value for the configuration setting identified by <paramref 
        /// name="fileName"/>, <paramref name="stanzaName"/>, and <paramref 
        /// name="keyName"/>.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/sSzcMy">POST 
        /// properties/{file_name}/{stanza_name}/{key_Name}</a> endpoint to 
        /// update the configuration setting identified by <paramref name=
        /// "fileName"/>, <paramref name="stanzaName"/>, and <paramref name="keyName"/>.
        /// </remarks>
        public async Task<ConfigurationSetting> UpdateConfigurationSettingAsync(string fileName, string stanzaName, 
            string keyName, string value)
        {
            ConfigurationSetting resource = new ConfigurationSetting(this.Context, this.Namespace, fileName, 
                stanzaName, keyName);
            await resource.UpdateAsync(value);
            return resource;
        }

        /// <summary>
        /// Adds or updates a list of settings in a configuration stanza.
        /// </summary>
        /// <param name="fileName">
        /// Name of a configuration file.
        /// </param>
        /// <param name="stanzaName">
        /// Name of a stanza within the configuration file identified by <paramref 
        /// name="fileName"/>.
        /// </param>
        /// <param name="settings">
        /// A variable-length list of objects representing the settings to be
        /// added or updated.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/w742jw">POST 
        /// properties/{file_name}/{stanza_name}</a> endpoint to update 
        /// <paramref name="settings"/> in the stanza identified by <paramref 
        /// name="fileName"/> and <paramref name="stanzaName"/>.
        /// </remarks>
        public async Task UpdateConfigurationSettingsAsync(string fileName, string stanzaName, params Argument[] settings)
        {
            ConfigurationStanza stanza = new ConfigurationStanza(this.Context, this.Namespace, fileName, stanzaName);
            await stanza.UpdateAsync(settings);
        }

        #endregion

        #region Indexes

        /// <summary>
        /// Asynchronously creates a new index.
        /// </summary>
        /// <param name="name">
        /// Name of the index to create.
        /// </param>
        /// <param name="coldPath">
        /// Location for storing the cold databases for the index identified by
        /// <paramref name="name"/>. A value of <c>null</c> or <c>""</c> specifies 
        /// that the cold databases should be stored at the default location.
        /// </param>
        /// <param name="homePath">
        /// Location for storing the hot and warm buckets for the index 
        /// identified by <paramref name="name"/>. A value of <c>null</c> or <c>""
        /// </c> specifies that the hot and warm buckets should be stored at 
        /// the default location.
        /// </param>
        /// <param name="thawedPath">
        /// Specifies the absolute path for storing the resurrected databases 
        /// for the index identified by <paramref name="name"/>. A value of <c>null
        /// </c> or <c>""</c> specifies that the resurrected databases should 
        /// be stored at the default location.
        /// </param>
        /// <param name="attributes">
        /// Attributes to set on the newly created index.
        /// </param>
        /// <returns>
        /// An object representing the newly created index.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/yDfQ4T">POST
        /// data/indexes</a> endpoint to create the <see cref="Index"/> object
        /// it returns.
        /// </remarks>
        public async Task<Index> CreateIndexAsync(string name, string coldPath = null, string homePath = null, 
            string thawedPath = null, IndexAttributes attributes = null)
        {
            var entity = new Index(this.Context, this.Namespace, name);
            await entity.CreateAsync(coldPath, homePath, thawedPath, attributes);
            return entity;
        }

        /// <summary>
        /// Asynchronously retrieves an <see cref="Index"/> by name.
        /// </summary>
        /// <param name="name">
        /// Name of the index to retrieve.
        /// </param>
        /// <returns>
        /// An object representing the named index.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/xljxjD">GET
        /// data/indexes/{name}</a> endpoint to construct the <see cref=
        /// "Index"/> object it returns.
        /// </remarks>
        public async Task<Index> GetIndexAsync(string name)
        {
            var entity = new Index(this.Context, this.Namespace, name);
            await entity.GetAsync();
            return entity;
        }

        /// <summary>
        /// Asynchronously retrieves a collection of indexes.
        /// </summary>
        /// <param name="args">
        /// Specification of the collection of indexes to retrieve.
        /// </param>
        /// <returns>
        /// An object representing the collection of indexes retrieved.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/qVZ6wJ">GET
        /// data/indexes</a> endpoint to construct the <see cref=
        /// "IndexCollection"/> object it returns.
        /// </remarks>
        public async Task<IndexCollection> GetIndexesAsync(IndexCollectionArgs args = null)
        {
            var collection = new IndexCollection(this.Context, this.Namespace);
            await collection.GetAsync();
            return collection;
        }

        /// <summary>
        /// Asynchronously removes an <see cref="Index"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the index to remove.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/hCc1xe">DELETE
        /// data/indexes/{name}</a> endpoint to remove the <see cref=
        /// "Index"/> identified by <paramref name="name"/>.
        /// </remarks>
        public async Task RemoveIndexAsync(string name)
        {
            var entity = new Index(this.Context, this.Namespace, name);
            await entity.RemoveAsync();
        }

        /// <summary>
        /// Asynchronously updates an index.
        /// </summary>
        /// <param name="name">
        /// Name of the index to update.
        /// </param>
        /// <param name="attributes">
        /// Attributes to set on the index.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/n3S22S">POST
        /// data/indexes/{name}</a> endpoint to update the <see cref=
        /// "Index"/> identified by <paramref name="name"/> with a new set of <paramref 
        /// name="attributes"/>.
        /// </remarks>
        public async Task UpdateIndexAsync(string name, IndexAttributes attributes)
        {
            var entity = new Index(this.Context, this.Namespace, name);
            await entity.UpdateAsync(attributes);
        }

        #endregion

        #region Saved searches

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
        /// <returns>
        /// An object representing the saved search that was created.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/EPQypw">POST 
        /// saved/searches</a> endpoint to create the <see cref="SavedSearch"/>
        /// object it returns.
        /// </remarks>
        public async Task<SavedSearch> CreateSavedSearchAsync(string name, string search, 
            SavedSearchAttributes attributes = null, SavedSearchDispatchArgs dispatchArgs = null, 
            SavedSearchTemplateArgs templateArgs = null)
        {
            var resource = new SavedSearch(this.Context, this.Namespace, name);
            await resource.CreateAsync(search, attributes, dispatchArgs, templateArgs);
            return resource;
        }

        /// <summary>
        /// Asynchronously dispatches a <see cref="SavedSearch"/> just like the
        /// scheduler would.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="SavedSearch"/> to dispatch.
        /// </param>
        /// <param name="dispatchArgs">
        /// A set of arguments to the dispatcher.
        /// </param>
        /// <param name="templateArgs">
        /// A set of template arguments to the <see cref="SavedSearch"/>.
        /// </param>
        /// <returns>
        /// The search <see cref="Job"/> that was dispatched.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/AfzBJO">POST 
        /// saved/searches/{name}/dispatch</a> endpoint to dispatch the <see 
        /// cref="SavedSearch"/> identified by <paramref name="name"/>.
        /// </remarks>
        public async Task<Job> DispatchSavedSearchAsync(string name, SavedSearchDispatchArgs dispatchArgs = null,
            SavedSearchTemplateArgs templateArgs = null)
        {
            var savedSearch = new SavedSearch(this.Context, this.Namespace, name);
            var job = await savedSearch.DispatchAsync(dispatchArgs, templateArgs);
            return job;
        }

        /// <summary>
        /// Asynchronously retrieves the named <see cref="SavedSearch"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="SavedSearch"/> to be retrieved.
        /// </param>
        /// <param name="args">
        /// Constrains the information returned about the <see cref=
        /// "SavedSearch"/>.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/L4JLwn">GET 
        /// saved/searches/{name}</a> endpoint to get the <see cref=
        /// "SavedSearch"/> identified by <paramref name="name"/>.
        /// </remarks>
        public async Task<SavedSearch> GetSavedSearchAsync(string name, SavedSearchFilterArgs args = null)
        {
            var resource = new SavedSearch(this.Context, this.Namespace, name);
            await resource.GetAsync(args);
            return resource;
        }

        /// <summary>
        /// Asynchronously retrieves a collection of saved searches.
        /// </summary>
        /// <param name="args">
        /// Arguments identifying the collection of <see cref="SavedSearch"/>
        /// entries to retrieve.
        /// </param>
        /// <returns>
        /// A new <see cref="SavedSearchCollection"/> containing the <see cref=
        /// "SavedSearch"/> entries identified by <paramref name="args"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/bKrRK0">GET 
        /// saved/searches</a> endpoint to retrieve a new <see cref=
        /// "SavedSearchCollection"/> containing the <see cref="SavedSearch"/> 
        /// entries identified by <paramref name="args"/>.
        /// </remarks>
        public async Task<SavedSearchCollection> GetSavedSearchesAsync(SavedSearchCollectionArgs args = null)
        {
            var resource = new SavedSearchCollection(this.Context, this.Namespace, args);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously removes a saved search.
        /// </summary>
        /// <param name="name">
        /// Name of the saved search to be removed.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/sn7qC5">DELETE 
        /// saved/searches/{name}</a> endpoint to remove the saved search
        /// identified by <paramref name="name"/>.
        /// </remarks>
        public async Task RemoveSavedSearchAsync(string name)
        {
            var resource = new SavedSearch(this.Context, this.Namespace, name);
            await resource.RemoveAsync();
        }

        /// <summary>
        /// Asynchronously updates a saved search.
        /// </summary>
        /// <param name="name">
        /// Name of the saved search to be updated.
        /// </param>
        /// <param name="attributes">
        /// New attributes for the saved search to be updated.
        /// </param>
        /// <param name="dispatchArgs">
        /// New dispatch arguments for the saved search to be updated.
        /// </param>
        /// <param name="templateArgs">
        /// New template arguments for the saved search to be updated.
        /// </param>
        /// <returns>
        /// An object representing the saved search that was updated.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/aV9eiZ">POST 
        /// saved/searches/{name}</a> endpoint to update the saved search
        /// identified by <paramref name="name"/>.
        /// </remarks>
        public async Task<SavedSearch> UpdateSavedSearchAsync(string name, SavedSearchAttributes attributes = null, 
            SavedSearchDispatchArgs dispatchArgs = null, SavedSearchTemplateArgs templateArgs = null)
        {
            var resource = new SavedSearch(this.Context, this.Namespace, name);
            await resource.UpdateAsync(attributes, dispatchArgs, templateArgs);
            return resource;
        }

        /// <summary>
        /// Asynchronously retrieves the collection of jobs created from a
        /// saved search.
        /// </summary>
        /// <param name="name">
        /// Name of a saved search.
        /// </param>
        /// <returns>
        /// An object representing the collection of jobs created from the
        /// saved search identified by <paramref name="name"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/kv9L1l">GET 
        /// saved/searches/{name}/history</a> endpoint to get the collection of
        /// jobs created from the <see cref="SavedSearch"/> identified by <paramref 
        /// name="name"/>.
        /// </remarks>
        public async Task<JobCollection> GetSavedSearchHistoryAsync(string name)
        {
            var savedSearch = new SavedSearch(this.Context, this.Namespace, name);
            var jobs = await savedSearch.GetHistoryAsync();
            return jobs;
        }

        #endregion

        #region Search jobs

        /// <summary>
        /// Asynchronously creates a new search <see cref="Job"/>.
        /// </summary>
        /// <param name="search">
        /// Search string.
        /// </param>
        /// <param name="args">
        /// Optional search arguments.
        /// </param>
        /// <param name="customArgs">
        /// 
        /// </param>
        /// <param name="requiredState">
        /// 
        /// </param>
        /// <returns>
        /// An object representing the search job that was created.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/JZcPEb">POST 
        /// search/jobs</a> endpoint to start a new search <see cref="Job"/> as
        /// specified by <paramref name="args"/>.
        /// </remarks>
        public async Task<Job> CreateJobAsync(string search, JobArgs args = null, CustomJobArgs customArgs = null,
            DispatchState requiredState = DispatchState.Running)
        {
            Contract.Requires<ArgumentNullException>(search != null);
            Contract.Requires<ArgumentOutOfRangeException>(args == null || args.ExecutionMode != ExecutionMode.Oneshot);

            //// FJR: Also check that it's not export, which also won't return a job.
            //// DSN: JobArgs does not include SearchExportArgs

            var resourceName = JobCollection.ClassResourceName;

            var command = new Argument[] 
            {
               new Argument("search", search)
            };

            string searchId;

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, command, args, customArgs))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
                searchId = await response.XmlReader.ReadResponseElementAsync("sid");
            }

            //// FJR: Jobs need to be handled a little more delicately. Let's talk about the patterns here.
            //// In the other SDKs, we've been doing functions to wait for ready and for done. Async means
            //// that we can probably make that a little slicker, but let's talk about how.

            Job job = new Job(this.Context, this.Namespace, name: searchId);

            await job.GetAsync();
            await job.TransitionAsync(requiredState);

            return job;
        }

        /// <summary>
        /// Asynchronously retrieves information about a search job.
        /// </summary>
        /// <param name="searchId">
        /// ID of a search job.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SFqSPI">GET 
        /// search/jobs/{search_id}</a> endpoint to get the <see cref="Job"/> 
        /// identified by <paramref name="searchId"/>.
        /// </remarks>
        public async Task<Job> GetJobAsync(string searchId)
        {
            var resource = new Job(this.Context, this.Namespace, searchId);
            await resource.GetAsync();
            return resource;
        }

        /// <summary>
        /// Asynchronously retrieves a collection of running search jobs.
        /// </summary>
        /// <param name="args">
        /// Specification of the collection of running search jobs to retrieve.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/ja2Sev">GET 
        /// search/jobs</a> endpoint to get the <see cref="JobCollection"/>
        /// specified by <paramref name="args"/>.
        /// </remarks>
        public async Task<JobCollection> GetJobsAsync(JobCollectionArgs args = null)
        {
            var jobs = new JobCollection(this.Context, this.Namespace, args);
            await jobs.GetAsync();
            return jobs;
        }

        /// <summary>
        /// Asynchronously removes a search <see cref="Job"/>.
        /// </summary>
        /// <param name="searchId">
        /// ID of a search job.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/TUqQUc">DELETE 
        /// search/jobs/{search_id}</a> endpoint to remove the <see cref="Job"/> 
        /// identified by <paramref name="searchId"/>.
        /// </remarks>
        public async Task RemoveJobAsync(string searchId)
        {
            var resource = new Job(this.Context, this.Namespace, searchId);
            await resource.RemoveAsync();
        }

        /// <summary>
        /// Executes a oneshot search.
        /// </summary>
        /// <param name="search">
        /// Search string.
        /// </param>
        /// <param name="args">
        /// Optional job arguments.
        /// </param>
        /// <returns>
        /// An object representing the stream search results.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/b02g1d">POST 
        /// search/jobs</a> endpoint to execute a oneshot search.
        /// </remarks>
        public async Task<SearchResultStream> SearchOneshotAsync(string search, JobArgs args = null)
        {
            Contract.Requires<ArgumentNullException>(search != null);

            var resourceName = JobCollection.ClassResourceName;

            var command = new Argument[]
            {
                new Argument("search", search),
                new Argument("exec_mode", "oneshot")
            };

            if (args != null)
            {
                args.ExecutionMode = null;
            }

            Response response = await this.Context.PostAsync(this.Namespace, resourceName, command, args);

            try
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                var stream = await SearchResultStream.CreateAsync(response); // Transfers response ownership
                return stream;
            }
            catch
            {
                response.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Asynchronously exports an observable sequence of search result previews.
        /// </summary>
        /// <param name="search">
        /// Splunk search command.
        /// </param>
        /// <param name="args">
        /// Optional export arguments.
        /// </param>
        /// <returns>
        /// An object representing an observable sequence of search result 
        /// previews.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/vJvIXv">GET 
        /// search/jobs/export</a> endpoint to start the 
        /// </remarks>
        public async Task<SearchPreviewStream> ExportSearchPreviewsAsync(string search, SearchExportArgs args = null)
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");

            var command = new Argument[] 
            { 
                new Argument("search", search) 
            };

            var response = await this.Context.GetAsync(this.Namespace, SearchJobsExport, command, args);

            try
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                return new SearchPreviewStream(response); // Transfers response ownership
            }
            catch
            {
                response.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Asynchronously exports an observable sequence of search results.
        /// </summary>
        /// <param name="search">
        /// Splunk search command.
        /// </param>
        /// <param name="args">
        /// Optional export arguments.
        /// </param>
        /// <returns>
        /// An object representing an observable sequence of search result 
        /// previews.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/vJvIXv">GET 
        /// search/jobs/export</a> endpoint to start the 
        /// </remarks>
        public async Task<SearchResultStream> ExportSearchResultsAsync(string search, SearchExportArgs args = null)
        {
            Contract.Requires<ArgumentNullException>(args != null, "args");

            var command = new Argument[] 
            { 
                new Argument("search", search) 
            };

            var response = await this.Context.GetAsync(this.Namespace, SearchJobsExport, command, args);

            try
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                return await SearchResultStream.CreateAsync(response); // Transfers response ownership
            }
            catch
            {
                response.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Asynchronously updates custom arguments to a search <see cref=
        /// "Job"/>.
        /// </summary>
        /// <param name="searchId">
        /// ID of the search <see cref="Job"/> to be updated.
        /// </param>
        /// <param name="args">
        /// New custom arguments to the search job.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/bL4tFk">POST 
        /// search/jobs/{search_id}</a> to update the specificiation of
        /// search <see cref="Job"/> identified by <paramref name="searchId"/>.
        /// </remarks>
        public async Task UpdateJobAsync(string searchId, CustomJobArgs args)
        {
            var resource = new Job(this.Context, this.Namespace, searchId);
            await resource.UpdateAsync(args);
        }

        #endregion

        #region Other methods

        /// <summary>
        /// Releases all resources used by the <see cref="Service"/>.
        /// </summary>
        /// <remarks>
        /// Do not override this method. Override <see cref="Service.Dispose(bool)"/> 
        /// instead.
        /// </remarks>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases all resources used by the <see cref="Service"/>.
        /// </summary>
        /// <remarks>
        /// Subclasses should implement the disposable pattern as follows:
        /// <list type="bullet">
        /// <item><description>
        ///     Override this method and call it from the override.
        ///     </description></item>
        /// <item><description>
        ///     Provide a finalizer, if needed, and call this method from it.
        ///     </description></item>
        /// <item><description>
        ///     To help ensure that resources are always cleaned up 
        ///     appropriately, ensure that the override is callable multiple
        ///     times without throwing an exception.
        ///     </description></item>
        /// </list>
        /// There is no performance benefit in overriding this method on types
        /// that use only managed resources (such as arrays) because they are 
        /// automatically reclaimed by the garbage collector. See 
        /// <a href="http://goo.gl/VPIovn">Implementing a Dispose Method</a>.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.Context.Dispose();
                this.disposed = true;
            }
        }

        /// <summary>
        /// Gets the URI string for this <see cref="Service"/> instance. 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("/", this.Context.ToString(), this.Namespace.ToString());
        }

        #endregion

        #endregion

        #region Privates/internals

        static readonly ResourceName AuthLogin = new ResourceName("auth", "login");
        static readonly ResourceName AuthenticationHttpAuthTokens = new ResourceName("authentication", "httpauth-tokens");
        static readonly ResourceName AuthorizationCapabilities = new ResourceName("authorization", "capabilities");
        static readonly ResourceName SearchJobsExport = new ResourceName("search", "jobs", "export");
        
        readonly Context context;
        readonly Namespace ns;

        readonly ConfigurationCollection configurations;
        readonly ApplicationCollection applications;
        readonly Receiver receiver;
        readonly Server server;

        bool disposed;

        #endregion
    }
}
