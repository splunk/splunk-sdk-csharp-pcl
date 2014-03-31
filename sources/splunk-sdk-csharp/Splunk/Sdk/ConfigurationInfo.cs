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
// [ ] Properties & Methods

namespace Splunk.Sdk
{
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a class for manipulating Splunk configuration files.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///     <a href="http://goo.gl/cTdaIH">REST API Reference: Accessing and 
    ///     updating Splunk configurations</a>
    /// </description></item>
    /// <item><description>
    ///     <a href="http://goo.gl/0ELhzV">REST API Reference: Configurations</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    public class ConfigurationInfo : Entity<ConfigurationInfo>
    {
        #region Constructors

        public ConfigurationInfo()
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Removes the named stanza from the current <see cref="Configuration"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the stanza to remove.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/79v7H3">DELETE 
        /// configs/conf-{file}/{name}</a> endpoint to remove the stanza
        /// identified by <see cref="name"/>.
        /// </remarks>
        public void RemoveStanza(string name)
        {
            this.RemoveStanzaAsync(name).Wait();
        }

        /// <summary>
        /// Removes the named stanza from the current <see cref="Configuration"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the stanza to remove.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/79v7H3">DELETE 
        /// configs/conf-{file}/{name}</a> endpoint to remove the stanza
        /// identified by <see cref="name"/>.
        /// </remarks>
        public async Task RemoveStanzaAsync(string name)
        {
            var resourceName = new ResourceName(ResourceName.Configs, string.Join("-", "conf", this.Title), name);

            using (var response = await this.Context.DeleteAsync(this.Namespace, resourceName))
            {
                if (response.Message.StatusCode != HttpStatusCode.OK)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
            }
        }
        
        #endregion
    }
}
