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
//// [ ] Diagnostics

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
    [ContractClass(typeof(IConfigurationContract<>))]
    public interface IConfiguration<TConfigurationStanza> : IEntityCollection<TConfigurationStanza>
        where TConfigurationStanza : BaseEntity, IConfigurationStanza, new()
    {
        /// <summary>
        /// Asynchronously creates a new configuration stanza.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of the configuration stanza to create.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/jae44k">POST 
        /// properties/{file_name}</a> endpoint to create the configuration
        /// stanza identified by <paramref name="stanzaName"/>.
        /// </remarks>
        Task<TConfigurationStanza> CreateAsync(string stanzaName);

        /// <summary>
        /// Asynchronously retrieves a setting value from a configuration stanza.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of the configuration stanza.
        /// </param>
        /// <param name="keyName">
        /// Name of the setting to retrieve.
        /// </param>
        /// <returns>
        /// The value of the setting identified by <paramref name="keyName"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/cqT50u">GET 
        /// properties/{file_name}/{stanza_name}/{key_name}</a> endpoint to 
        /// construct the setting value it returns.
        /// </remarks>
        Task<string> GetSettingAsync(string stanzaName, string keyName);

        /// <summary>
        /// Asynchronously removes a configuration stanza.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of the configuration stanza to remove.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/uMzr3F">DELETE 
        /// configs/conf-{file}/{name}</a> endpoint to remove <paramref name=
        /// "stanzaName"/>.
        /// </remarks>
        Task RemoveAsync(string stanzaName);

        /// <summary>
        /// Asynchronously updates a configuration stanza with new or updated
        /// settings.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of the configuration stanza to update.
        /// </param>
        /// <param name="settings">
        /// The new or updated settings.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/dpbuhQ">DELETE
        /// configs/conf-{file}/{name}</a> endpoint to remove the configuration
        /// stanza identified by <paramref name="stanzaName"/>.
        /// </remarks>
        Task<TConfigurationStanza> UpdateAsync(string stanzaName, params Argument[] settings);

        /// <summary>
        /// Asynchronously updates a setting within a configuration stanza.
        /// </summary>
        /// <param name="stanzaName">
        /// Name of the configuration stanza.
        /// </param>
        /// <param name="keyName">
        /// Name of the setting in the named configuration stanza.
        /// </param>
        /// <param name="value">
        /// A value for <paramref name="keyName"/>.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/w742jw">POST 
        /// properties/{file_name}/{stanza_name}</a> endpoint to update the 
        /// setting identified by <paramref name="stanzaName"/> and <paramref 
        /// name="keyName"/>.
        /// </remarks>
        /// 
        Task UpdateSettingAsync(string stanzaName, string keyName, object value);

        /// <inheritdoc cref="UpdateSettingAsync"/>
        Task UpdateSettingAsync(string stanzaName, string keyName, string value);
    }

    [ContractClassFor(typeof(IConfiguration<>))]
    abstract class IConfigurationContract<TConfigurationStanza> : IConfiguration<TConfigurationStanza>
        where TConfigurationStanza : BaseEntity, IConfigurationStanza, new()
    {
        public abstract TConfigurationStanza this[int index] { get; }
        public abstract int Count { get; }

        public Task<TConfigurationStanza> CreateAsync(IEnumerable<Argument> arguments)
        {
            Contract.Requires<ArgumentNullException>(arguments != null);
            return default(Task<TConfigurationStanza>);
        }

        public Task<TConfigurationStanza> CreateAsync(string stanzaName)
        {
            Contract.Requires<ArgumentNullException>(stanzaName != null);
            return default(Task<TConfigurationStanza>);
        }

        public abstract Task<TConfigurationStanza> GetAsync(string name);

        public abstract Task GetAllAsync();

        public abstract IEnumerator<TConfigurationStanza> GetEnumerator();

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return default(IEnumerator);
        }

        public Task<string> GetSettingAsync(string stanzaName, string keyName)
        {
            Contract.Requires<ArgumentNullException>(stanzaName != null);
            Contract.Requires<ArgumentNullException>(keyName != null);
            return default(Task<string>);
        }

        public abstract Task GetSliceAsync(params Argument[] arguments);

        public abstract Task GetSliceAsync(IEnumerable<Argument> arguments);
        
        public abstract Task ReloadAsync();
        
        public Task RemoveAsync(string stanzaName)
        {
            Contract.Requires<ArgumentNullException>(stanzaName != null);
            return default(Task);
        }

        public Task<TConfigurationStanza> UpdateAsync(string stanzaName, params Argument[] settings)
        {
            Contract.Requires<ArgumentNullException>(stanzaName != null);
            return default(Task<TConfigurationStanza>);
        }

        public Task UpdateSettingAsync(string stanzaName, string keyName, object value)
        {
            Contract.Requires<ArgumentNullException>(stanzaName != null);
            Contract.Requires<ArgumentNullException>(keyName != null);
            Contract.Requires<ArgumentNullException>(value != null);
            return default(Task<string>);
        }

        public Task UpdateSettingAsync(string stanzaName, string keyName, string value)
        {
            Contract.Requires<ArgumentNullException>(stanzaName != null);
            Contract.Requires<ArgumentNullException>(keyName != null);
            Contract.Requires<ArgumentNullException>(value != null);
            return default(Task<string>);
        }
    }
}
