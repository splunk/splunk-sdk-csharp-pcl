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
//// [X] Properties & Methods
//// [ ] What HttpStatusCode does Splunk return when a session key expires?
//// [ ] What exception should Args throw when a required property is omitted: 
////     ArgumentException or SerializationException as it does right now?

namespace Splunk.Client.Refactored
{
    using Splunk.Client;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a Splunk storage password
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/wAcSfp">Splunk>Blogs: Storing Encrypted Credentials</a>
    /// </description></item>
    /// <item><description>
    ///   <a href="http://goo.gl/HTRSVu">REST API Reference: storage/passwords</a>
    /// </description></item>
    /// </list>
    /// </remarks>
    public class StoragePassword : Entity, IStoragePassword
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StoragePassword"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="name">
        /// Username associated with the <see cref="StoragePassword"/>.
        /// </param>
        /// <param name="realm">
        /// Realm associated with the <see cref="StoragePassword"/> or <c>
        /// null</c>. The default value is <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="ns"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        protected internal StoragePassword(Service service, string name, string realm = null)
            : this(service.Context, service.Namespace, name, realm)
        {
            Contract.Requires<ArgumentNullException>(service != null);
            Contract.Requires<ArgumentNullException>(name != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoragePassword"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="name">
        /// Username associated with the <see cref="StoragePassword"/>.
        /// </param>
        /// <param name="realm">
        /// Realm associated with the <see cref="StoragePassword"/> or <c>
        /// null</c>. The default value is <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="ns"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        internal StoragePassword(Context context, Namespace ns, string name, string realm = null)
            : base(context, ns, CreateResourceNameFromRealmAndUsername(realm ?? "", name))
        {
            Contract.Requires<ArgumentNullException>(name != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoragePassword"/>
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        protected internal StoragePassword(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "StoragePassword"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use one of these
        /// methods to obtain a <see cref="StoragePassword"/> instance:
        /// <list type="table">
        /// <listheader>
        ///   <term>Method</term>
        ///   <description>Description</description>
        /// </listheader>
        /// <item>
        ///   <term><see cref="Service.CreateStoragePasswordAsync"/></term>
        ///   <description>
        ///   Asynchronously creates a new <see cref="StoragePassword"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.GetStoragePasswordAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves an existing <see cref="StoragePassword"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.UpdateStoragePasswordAsync"/></term>
        ///   <description>
        ///   Asynchronously updates an existing <see cref="StoragePassword"/>.
        ///   </description>
        /// </item>
        /// </list>
        /// </remarks>
        public StoragePassword()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public virtual string ClearPassword
        {
            get { return this.GetValue("ClearPassword", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual Eai Eai
        {
            get { return this.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string EncryptedPassword
        {
            get { return this.GetValue("EncrPassword", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string Password
        {
            get { return this.GetValue("Password", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string Realm
        {
            get { return this.GetValue("Realm", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string Username
        {
            get { return this.GetValue("Username", StringConverter.Instance); }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async Task UpdateAsync(string password)
        {
            var attributes = new Argument[] { new Argument("password", password) };

            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, attributes))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.ReconstructSnapshotAsync(response);
            }
        }

        #endregion

        #region Privates/internals

        static ResourceName CreateResourceNameFromRealmAndUsername(string realm, string username)
        {
            var parts = new string[] { realm, username };
            var builder = new StringBuilder();

            foreach (var part in parts)
            {
                foreach (var c in part)
                {
                    if (c == ':')
                    {
                        builder.Append('\\');
                    }

                    builder.Append(c);
                }

                builder.Append(':');
            }

            return new ResourceName(StoragePasswordCollection.ClassResourceName, builder.ToString());
        }

        static void ParseRealmAndUsernameFromName(string name, out string realm, out string username)
        {
            var builder = new StringBuilder();
            var parts = new List<string>(3);
            int last = name.Length - 1;

            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] == ':')
                {
                    parts.Add(builder.ToString());
                    builder.Clear();
                    continue;
                }
                
                if (i < last && name[i] == '\\' && name[i + 1] == ':')
                {
                    builder.Append(':');
                    i++;
                    continue;
                }

                builder.Append(name[i]);
            }

            Debug.Assert(builder.Length == 0 && parts.Count == 2 && parts[1].Length > 0);
            realm = parts[0];
            username = parts[1];
        }

        #endregion
    }
}
