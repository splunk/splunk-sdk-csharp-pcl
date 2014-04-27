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

namespace Splunk.Client
{
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
    /// Provides a class that represents a Splunk storage password resource.
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
    public class StoragePassword : Entity<StoragePassword>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StoragePassword"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="namespace">
        /// An object identifying a Splunk service namespace.
        /// </param>
        /// <param name="username">
        /// Username associated with the <see cref="StoragePassword"/>.
        /// </param>
        /// <param name="username">
        /// Realm associated with the <see cref="StoragePassword"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <see cref="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <see cref="context"/> or <see cref="namespace"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="namespace"/> is not specific.
        /// </exception>
        internal StoragePassword(Context context, Namespace @namespace, string username, string realm = "")
            : base(context, @namespace, StoragePasswordCollection.ClassResourceName, CreateNameFromRealmAndUsername(
            realm ?? "", username))
        {
            Contract.Requires<ArgumentNullException>(username != null);
        }

        public StoragePassword()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the plain text version of the current <see cref=
        /// "StoragePassword"/>.
        /// </summary>
        public string ClearPassword
        {
            get { return this.Content.GetValue("ClearPassword", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the access control lists for the current <see cref=
        /// "StoragePassword"/>.
        /// </summary>
        public Eai Eai
        {
            get { return this.Content.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <summary>
        /// Gets an encrypted version of the current <see cref=
        /// "StoragePassword"/>.
        /// </summary>
        public string EncryptedPassword
        {
            get { return this.Content.GetValue("EncrPassword", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the masked version of the current <see cref="StoragePassword"/>.
        /// </summary>
        /// <remarks>
        ///  This is always stored as <c>"********"</c>.
        /// </remarks>
        public string Password
        {
            get { return this.Content.GetValue("Password", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the realm in which the current <see cref="StoragePassword"/> 
        /// is valid.
        /// </summary>
        public string Realm
        {
            get { return this.Content.GetValue("Realm", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the Splunk username associated with the current <see cref=
        /// "StoragePassword"/>. 
        /// </summary>
        public string Username
        {
            get { return this.Content.GetValue("Username", StringConverter.Instance); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates the storage password represented by the
        /// current instance.
        /// </summary>
        /// <param name="password">
        /// Storage password.
        /// </param>
        /// <param name="realm">
        /// Optional storage realm.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/JgyIeN">POST 
        /// storage/passwords</a> endpoint to create the <see cref=
        /// "StoragePassword"/> represented by the current instance.
        /// </remarks>
        public async Task CreateAsync(string password)
        {
            string realm, username;
            ParseRealmAndUsernameFromName(this.Name, out realm, out username);

            var attributes = new StoragePasswordAttributes()
            {
                Username = username,
                Password = password,
                Realm = realm
            };

            using (var response = await this.Context.PostAsync(
                this.Namespace, StoragePasswordCollection.ClassResourceName, attributes))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
                await this.UpdateDataAsync(response);
            }
        }

        /// <summary>
        /// Asynchronously removes the storage password represented by the 
        /// current instance.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/JGm0JP">DELETE 
        /// storage/passwords/{name}</a> endpoint to remove the storage 
        /// password represented by the current instance.
        /// </remarks>
        public async Task RemoveAsync()
        {
            using (var response = await this.Context.DeleteAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Asynchronously updates the storage password represented by the
        /// current instance.
        /// </summary>
        /// <param name="password">
        /// New storage password.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <see cref="password"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="RequestException">
        /// </exception>
        /// <exception cref="ResourceNotFoundException">
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// </exception>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/s0Bw7H">POST 
        /// storage/passwords/{name}</a> endpoint to update the storage 
        /// password represented by the current instance.
        /// </remarks>
        public async Task UpdateAsync(string password)
        {
            Contract.Requires<ArgumentException>(password != null);

            var attributes = new Argument[] { new Argument("password", password) };

            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, attributes))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.UpdateDataAsync(response);
            }
        }

        #endregion

        #region Privates/internals

        static string CreateNameFromRealmAndUsername(string realm, string username)
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

            return builder.ToString();
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

        async Task UpdateDataAsync(Response response)
        {
            var feed = new AtomFeed();
            await feed.ReadXmlAsync(response.XmlReader);

            if (feed.Entries.Count != 1)
            {
                throw new InvalidDataException();  // TODO: Diagnostics
            }

            this.Data = new DataCache(feed.Entries[0]);
        }

        #endregion
    }
}
