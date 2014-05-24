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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a collection of Splunk configuration files.
    /// </summary>
    public class ConfigurationCollection : EntityCollection<Configuration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationCollection"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        internal ConfigurationCollection(Service service)
            : base(service, ClassResourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ConfigurationCollection"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use <see cref=
        /// "Service.GetConfigurationsAsync"/> to asynchronously retrieve the 
        /// collection of all configuration files known to Splunk.
        /// </remarks>
        public ConfigurationCollection()
        { }

        /// <summary>
        /// Unsupported. This method is not supported by the <see cref=
        /// "ConfigurationCollection"/> class because it is not supported by 
        /// the <a href="http://goo.gl/Unj6fs"> Splunk properties endpoint</a>.
        /// </summary>
        /// <returns></returns>
        public override async Task GetSliceAsync(params Argument[] arguments)
        {
            await this.GetSliceAsync(arguments.AsEnumerable());
        }

        /// <summary>
        /// Unsupported. This method is not supported by the <see cref=
        /// "ConfigurationCollection"/> class because it is not supported by 
        /// the <a href="http://goo.gl/Unj6fs"> Splunk properties endpoint</a>.
        /// </summary>
        /// <returns></returns>
        public override Task GetSliceAsync(IEnumerable<Argument> arguments)
        {
            throw new NotSupportedException("The Splunk properties endpoint can only return the full list of configuration files.")
            {
                HelpLink = "http://docs.splunk.com/Documentation/Splunk/latest/RESTAPI/RESTconfig#GET_properties"
            };
        }

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("properties");
        
        #endregion
    }
}
