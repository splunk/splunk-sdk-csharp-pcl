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
//// [ ] What HttpStatusCode does Splunk return when a session key expires?

namespace Splunk.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a Splunk storage password.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/wAcSfp">Splunk&gt;Blogs: Storing Encrypted
    ///   Credentials</a>.
    /// </description></item>
    /// <item><description>
    ///   <a href="http://goo.gl/HTRSVu">REST API Reference:
    ///   storage/passwords</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="T:Splunk.Client.Entity{Splunk.Client.Resource}"/>
    /// <seealso cref="T:Splunk.Client.IStoragePassword"/>
    public class StoragePassword : Entity<Resource>, IStoragePassword
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StoragePassword"/> class.
        /// </summary>
        /// <param name="service">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="username">
        /// Username associated with the <see cref="StoragePassword"/>.
        /// </param>
        /// <param name="realm">
        /// Realm associated with the <see cref="StoragePassword"/> or <c>
        /// null</c>. The default value is <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="username"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> or <paramref name="username"/> are <c>null</c>.
        /// </exception>
        protected internal StoragePassword(Service service, string username, string realm = null)
            : this(service.Context, service.Namespace, username, realm)
        {
            Contract.Requires<ArgumentNullException>(service != null);
            Contract.Requires<ArgumentNullException>(username != null);
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
        /// <param name="username">
        /// Username associated with the <see cref="StoragePassword"/>.
        /// </param>
        /// <param name="realm">
        /// Realm associated with the <see cref="StoragePassword"/> or <c>
        /// null</c>. The default value is <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="username"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="ns"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        internal StoragePassword(Context context, Namespace ns, string username, string realm = null)
            : base(context, ns, CreateResourceNameFromRealmAndUsername(realm ?? "", username))
        {
            Contract.Requires<ArgumentNullException>(username != null);
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
        ///
        /// ### <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// ### <exception cref="InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        protected internal StoragePassword(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "StoragePassword"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public StoragePassword()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public virtual string ClearPassword
        {
            get { return this.Content.GetValue("ClearPassword", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual Eai Eai
        {
            get { return this.Content.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string EncryptedPassword
        {
            get { return this.Content.GetValue("EncrPassword", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string Password
        {
            get { return this.Content.GetValue("Password", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string Realm
        {
            get { return this.Content.GetValue("Realm", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string Username
        {
            get { return this.Content.GetValue("Username", StringConverter.Instance); }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async Task UpdateAsync(string password)
        {
            var attributes = new Argument[] { new Argument("password", password) };

            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, attributes).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
                await this.ReconstructSnapshotAsync(response).ConfigureAwait(false);
            }
        }

        #endregion

        #region Privates/internals

        /// <summary>
        /// Creates name from realm and username.
        /// </summary>
        /// <param name="realm">
        /// Realm associated with the <see cref="StoragePassword"/> or <c>null</c>.
        /// The default value is <c>null</c>.
        /// </param>
        /// <param name="username">
        /// Username associated with the <see cref="StoragePassword"/>.
        /// </param>
        /// <returns>
        /// The new name from realm and username.
        /// </returns>
        internal static string CreateNameFromRealmAndUsername(string realm, string username)
        {
            Contract.Requires<ArgumentNullException>(username != null);

            var parts = new string[] { realm ?? "", username };
            var builder = new StringBuilder();

            foreach (string part in parts)
            {
                foreach (char c in part)
                {
                    if (c == ':')
                    {
                        builder.Append('\\');
                    }

                    builder.Append(c);
                }

                builder.Append(':');
            }

            return builder.ToString();
        }

        static ResourceName CreateResourceNameFromRealmAndUsername(string realm, string username)
        {
            string name = CreateNameFromRealmAndUsername(realm, username);
            return new ResourceName(StoragePasswordCollection.ClassResourceName, name);
        }

        #endregion
    }
}
