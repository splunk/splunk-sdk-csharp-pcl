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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an operational interface for accessing and updating Splunk
    /// configuration files.
    /// </summary>
    /// <typeparam name="TConfigurationStanza">
    /// Type of the configuration stanza.
    /// </typeparam>
    /// <seealso cref="T:IEntityCollection{TConfigurationStanza"/>
    [ContractClass(typeof(IConfigurationContract<>))]
    public interface IConfiguration<TConfigurationStanza> : IEntityCollection<TConfigurationStanza, Resource>
        where TConfigurationStanza : BaseEntity<Resource>, IConfigurationStanza, new()
    {
        /// <summary>
        /// Asynchronously creates a new configuration stanza.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/jae44k">POST
        /// properties/{file_name}</a> endpoint to create the configuration stanza
        /// identified by <paramref name="stanzaName"/>.
        /// </remarks>
        /// <param name="stanzaName">
        /// Name of the configuration stanza to create.
        /// </param>
        /// <returns>
        /// The new asynchronous.
        /// </returns>
        Task<TConfigurationStanza> CreateAsync(string stanzaName);

        /// <summary>
        /// Asynchronously retrieves a setting value from a configuration stanza.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/cqT50u">GET
        /// properties/{file_name}/{stanza_name}/{key_name}</a> endpoint to construct
        /// the setting value it returns.
        /// </remarks>
        /// <param name="stanzaName">
        /// Name of the configuration stanza.
        /// </param>
        /// <param name="keyName">
        /// Name of the setting to retrieve.
        /// </param>
        /// <returns>
        /// The value of the setting identified by <paramref name="keyName"/>.
        /// </returns>
        Task<string> GetSettingAsync(string stanzaName, string keyName);

        /// <summary>
        /// Asynchronously removes a configuration stanza.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/uMzr3F">DELETE configs/conf-
        /// {file}/{name}</a> endpoint to remove <paramref name= "stanzaName"/>.
        /// </remarks>
        /// <param name="stanzaName">
        /// Name of the configuration stanza to remove.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task RemoveAsync(string stanzaName);

        /// <summary>
        /// Asynchronously updates a configuration stanza with new or revised
        /// settings.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/dpbuhQ">DELETE configs/conf-
        /// {file}/{name}</a> endpoint to remove the configuration stanza identified
        /// by <paramref name="stanzaName"/>.
        /// </remarks>
        /// <param name="stanzaName">
        /// Name of the configuration stanza to update.
        /// </param>
        /// <param name="settings">
        /// The new or updated settings.
        /// </param>
        /// <returns>
        /// An object representing the updated configuration stanza.
        /// </returns>
        Task<TConfigurationStanza> UpdateAsync(string stanzaName, params Argument[] settings);

        /// <summary>
        /// Asynchronously updates a setting within a configuration stanza.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/w742jw">POST
        /// properties/{file_name}/{stanza_name}</a> endpoint to update the setting
        /// identified by <paramref name="stanzaName"/> and
        /// <paramref name="keyName"/>.
        /// </remarks>
        /// <param name="stanzaName">
        /// Name of the configuration stanza containing the setting.
        /// </param>
        /// <param name="keyName">
        /// Name of the setting.
        /// </param>
        /// <param name="value">
        /// A value for <paramref name="keyName"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task UpdateSettingAsync(string stanzaName, string keyName, object value);

        /// <inheritdoc cref="UpdateSettingAsync(string,string,object)"/>
        Task UpdateSettingAsync(string stanzaName, string keyName, string value);
    }

    [ContractClassFor(typeof(IConfiguration<>))]
    abstract class IConfigurationContract<TConfigurationStanza> : IConfiguration<TConfigurationStanza>
        where TConfigurationStanza : BaseEntity<Resource>, IConfigurationStanza, new()
    {
        /// <summary>
        /// Indexer to get items within this collection using array index syntax.
        /// </summary>
        /// <param name="index">
        /// Zero-based index of the entry to access.
        /// </param>
        /// <returns>
        /// The indexed item.
        /// </returns>
        public abstract TConfigurationStanza this[int index] { get; }

        /// <summary>
        /// Gets the number of. 
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public abstract int Count { get; }

        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The new asynchronous.
        /// </returns>
        public Task<TConfigurationStanza> CreateAsync(IEnumerable<Argument> arguments)
        {
            Contract.Requires<ArgumentNullException>(arguments != null);
            return default(Task<TConfigurationStanza>);
        }

        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of the stanza.
        /// </param>
        /// <returns>
        /// The new asynchronous.
        /// </returns>
        public Task<TConfigurationStanza> CreateAsync(string stanzaName)
        {
            Contract.Requires<ArgumentNullException>(stanzaName != null);
            return default(Task<TConfigurationStanza>);
        }

        /// <summary>
        /// Asynchronously retrieves a configuration stanza by name.
        /// </summary>
        /// <param name="name">
        /// Name of the configuration stanza to retrieve.
        /// </param>
        /// <returns>
        /// The configuration stanza retrieved.
        /// </returns>
        public abstract Task<TConfigurationStanza> GetAsync(string name);

        /// <summary>
        /// Gets all asynchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public abstract Task GetAllAsync();

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public abstract IEnumerator<TConfigurationStanza> GetEnumerator();

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return default(IEnumerator);
        }

        /// <summary>
        /// Gets setting asynchronous.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of the stanza.
        /// </param>
        /// <param name="keyName">
        /// Name of the key.
        /// </param>
        /// <returns>
        /// The setting asynchronous.
        /// </returns>
        public Task<string> GetSettingAsync(string stanzaName, string keyName)
        {
            Contract.Requires<ArgumentNullException>(stanzaName != null);
            Contract.Requires<ArgumentNullException>(keyName != null);
            return default(Task<string>);
        }

        /// <summary>
        /// Gets slice asynchronous.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The slice asynchronous.
        /// </returns>
        public abstract Task GetSliceAsync(params Argument[] arguments);

        /// <summary>
        /// Gets slice asynchronous.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The slice asynchronous.
        /// </returns>
        public abstract Task GetSliceAsync(IEnumerable<Argument> arguments);

        /// <summary>
        /// Reload asynchronous.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public abstract Task ReloadAsync();

        /// <summary>
        /// Removes the asynchronous described by stanzaName.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of the stanza.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public Task RemoveAsync(string stanzaName)
        {
            Contract.Requires<ArgumentNullException>(stanzaName != null);
            return default(Task);
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of the stanza.
        /// </param>
        /// <param name="settings">
        /// A variable-length parameters list containing settings.
        /// </param>
        /// <returns>
        /// An object representing the updated configuration stanza.
        /// </returns>
        public Task<TConfigurationStanza> UpdateAsync(string stanzaName, params Argument[] settings)
        {
            Contract.Requires<ArgumentNullException>(stanzaName != null);
            return default(Task<TConfigurationStanza>);
        }

        /// <summary>
        /// Updates the setting asynchronous.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of the stanza.
        /// </param>
        /// <param name="keyName">
        /// Name of the key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public Task UpdateSettingAsync(string stanzaName, string keyName, object value)
        {
            Contract.Requires<ArgumentNullException>(stanzaName != null);
            Contract.Requires<ArgumentNullException>(keyName != null);
            Contract.Requires<ArgumentNullException>(value != null);
            return default(Task<string>);
        }

        /// <summary>
        /// Updates the setting asynchronous.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of the stanza.
        /// </param>
        /// <param name="keyName">
        /// Name of the key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public Task UpdateSettingAsync(string stanzaName, string keyName, string value)
        {
            Contract.Requires<ArgumentNullException>(stanzaName != null);
            Contract.Requires<ArgumentNullException>(keyName != null);
            Contract.Requires<ArgumentNullException>(value != null);
            return default(Task<string>);
        }
    }
}
