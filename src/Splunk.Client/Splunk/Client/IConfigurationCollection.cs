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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an operational interface over a collection of Splunk 
    /// configuration files.
    /// </summary>
    public interface IConfigurationCollection<TConfiguration, TConfigurationStanza> : IEntityCollection<TConfiguration>
        where TConfiguration : BaseEntity, IConfiguration<TConfigurationStanza>, new()
        where TConfigurationStanza : BaseEntity, IConfigurationStanza, new()
    {
        /// <summary>
        /// Asynchronously creates a configuration file.
        /// </summary>
        /// <param name="name">
        /// Name of the configuration file to create.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/CBWes7">POST 
        /// properties</a> endpoint to create the configuration stanza 
        /// represented by the object it returns.
        /// </remarks>
        Task<TConfiguration> CreateAsync(string fileName);

        /// <summary>
        /// Asynchronously retrieves a configuration stanza by name.
        /// </summary>
        /// <param name="fileName">
        /// The name of a configuration file.
        /// </param>
        /// <param name="stanzaName">
        /// The name of a stanza within the configuration file identifeied by
        /// <paramref name="fileName"/>.
        /// </param>
        /// <returns>
        /// An object representing the configuration stanza identified by 
        /// <paramref name="fileName"/> and <paramref name="stanzaName"/>.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/sM63fa">GET 
        /// properties/{file_name}/{stanza_name}</a> endpoint to construct the 
        /// <see cref="ConfigurationStanza"/> it returns.
        /// </remarks>
        Task<TConfigurationStanza> GetAsync(string fileName, string stanzaName);
    }
}
