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
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for configuration stanza.
    /// </summary>
    /// <seealso cref="T:IEntity"/>
    /// <seealso cref="T:IReadOnlyList{ConfigurationSetting}"/>
    [ContractClass(typeof(IConfigurationStanzaContract))]
    public interface IConfigurationStanza : IEntity, IReadOnlyList<ConfigurationSetting>
    {
        /// <summary>
        /// Gets the author of the current <see cref="ConfigurationStanza"/>.
        /// </summary>
        /// <value>
        /// The author.
        /// </value>
        string Author { get; }

        /// <summary>
        /// Asynchronously retrieves a configuration setting value from the current
        /// <see cref="ConfigurationStanza"/>
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/cqT50u">GET
        /// properties/{file_name}/{stanza_name}/{key_Name}</a> endpoint to construct
        /// the <see cref="ConfigurationSetting"/> identified by
        /// <paramref name="keyName"/>.
        /// </remarks>
        /// <param name="keyName">
        /// The name of a configuration setting.
        /// </param>
        /// <returns>
        /// The string value of <paramref name="keyName"/>.
        /// </returns>
        Task<string> GetAsync(string keyName);

        /// <summary>
        /// Asynchronously updates the value of an existing setting in the current
        /// <see cref="ConfigurationStanza"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/sSzcMy">POST
        /// properties/{file_name}/{stanza_name}/{key_Name}</a> endpoint to update
        /// the <see cref="ConfigurationSetting"/> identified by
        /// <paramref name="keyName"/>.
        /// </remarks>
        /// <param name="keyName">
        /// The name of a configuration setting in the current
        /// <see cref= "ConfigurationStanza"/>.
        /// </param>
        /// <param name="value">
        /// A new value for the setting identified by <paramref name="keyName"/>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task UpdateAsync(string keyName, object value);

        /// <inheritdoc/>
        Task UpdateAsync(string keyName, string value);
    }

    /// <summary>
    /// A configuration stanza contract.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.IConfigurationStanza"/>
    [ContractClassFor(typeof(IConfigurationStanza))]
    abstract class IConfigurationStanzaContract : IConfigurationStanza
    {
        #region Properties

        public abstract ConfigurationSetting this[int index] { get; }
        public abstract int Count { get; }
        public abstract string Author { get; }

        #endregion

        #region Methods

        public abstract Task GetAsync();

        public abstract Task<bool> UpdateAsync(params Argument[] arguments);

        public Task<string> GetAsync(string keyName)
        {
            Contract.Requires<ArgumentNullException>(keyName != null);
            return default(Task<string>);
        }

        public abstract IEnumerator<ConfigurationSetting> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return default(IEnumerator);
        }

        public abstract Task RemoveAsync();

        public abstract Task<bool> SendAsync(HttpMethod method, string action, params Argument[] arguments);

        public abstract Task<bool> UpdateAsync(IEnumerable<Argument> arguments);

        public Task UpdateAsync(string keyName, object value)
        {
            Contract.Requires<ArgumentNullException>(keyName != null);
            Contract.Requires<ArgumentNullException>(value != null);
            return default(Task);
        }

        public Task UpdateAsync(string keyName, string value)
        {
            Contract.Requires<ArgumentNullException>(keyName != null);
            Contract.Requires<ArgumentNullException>(value != null);
            return default(Task);
        }

        #endregion
    }
}
