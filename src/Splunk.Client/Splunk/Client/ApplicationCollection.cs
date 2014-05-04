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

namespace Splunk.Client
{
    /// <summary>
    /// Represents a collection of Splunk applications.
    /// </summary>
    public class ApplicationCollection : EntityCollection<ApplicationCollection, Application>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCollection"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="namespace">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="args">
        /// </param>
        internal ApplicationCollection(Context context, Namespace @namespace, ApplicationCollectionArgs args = null)
            : base(context, @namespace, ClassResourceName, args)
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

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("apps", "local");

        #endregion
    }
}
