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
//// [ ] Contracts
//// [ ] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the setup information for an <see cref="Application"/>.
    /// </summary>
    public sealed class ApplicationSetupInfo : Entity<ApplicationSetupInfo>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ApplicationSetupInfo class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="name">
        /// The name of a Splunk application.
        /// </param>
        internal ApplicationSetupInfo(Context context, Namespace ns, string name)
            : base(context, ns, new ResourceName(ApplicationCollection.ClassResourceName, name, "setup"))
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ApplicationSetupInfo"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. The <see cref=
        /// "ApplicationSetupInfo"/> class is an information object
        /// returned by these methods.
        /// <list type="table">
        /// <listheader>
        ///   <term>Method</term>
        ///   <description>Description</description>
        /// </listheader>
        /// <item>
        ///   <term><see cref="Application.GetSetupInfoAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves setup information for the current <see 
        ///   cref="Application"/>.
        /// </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.GetApplicationSetupInfoAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves setup information for an <see cref=
        ///   "Application"/> identified by name.
        ///   </description>
        /// </item>
        /// </list>
        /// </remarks>
        public ApplicationSetupInfo()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the 
        /// </summary>
        public Eai Eai
        {
            get { return this.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether to to reload the objects contained 
        /// in the locally installed application.
        /// </summary>
        public bool Refresh
        {
            get { return this.GetValue("Refresh", BooleanConverter.Instance); }
        }

        #endregion
    }
}
