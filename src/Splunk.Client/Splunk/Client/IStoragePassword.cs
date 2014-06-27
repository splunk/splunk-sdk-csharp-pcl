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
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for storage password.
    /// </summary>
    /// <seealso cref="T:IEntity"/>
    [ContractClass(typeof(IStoragePasswordContract))]
    public interface IStoragePassword : IEntity
    {
        #region Properties

        /// <summary>
        /// Gets the plain text version of the current <see cref= "StoragePassword"/>.
        /// </summary>
        /// <value>
        /// The clear password.
        /// </value>
        string ClearPassword
        { get; }

        /// <summary>
        /// Gets the extensible administration interface properties for the current
        /// <see cref= "StoragePassword"/>.
        /// </summary>
        /// <value>
        /// The extensible administration interface properties.
        /// </value>
        Eai Eai
        { get; }

        /// <summary>
        /// Gets an encrypted version of the current <see cref= "StoragePassword"/>.
        /// </summary>
        /// <value>
        /// The encrypted password.
        /// </value>
        string EncryptedPassword
        { get; }

        /// <summary>
        /// Gets the masked version of the current <see cref="StoragePassword"/>.
        /// </summary>
        /// <remarks>
        /// This is always stored as <c>"********"</c>.
        /// </remarks>
        /// <value>
        /// The password.
        /// </value>
        string Password
        { get; }

        /// <summary>
        /// Gets the realm in which the current <see cref="StoragePassword"/>
        /// is valid.
        /// </summary>
        /// <value>
        /// The realm.
        /// </value>
        string Realm
        { get; }

        /// <summary>
        /// Gets the Splunk username associated with the current
        /// <see cref= "StoragePassword"/>.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        string Username
        { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously updates the storage password represented by the current
        /// instance.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/s0Bw7H">POST
        /// storage/passwords/{name}</a> endpoint to update the storage password
        /// represented by the current instance.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// <paramref name="password"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="RequestException">
        /// 
        /// </exception>
        /// <exception cref="ResourceNotFoundException">
        /// 
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// 
        /// </exception>
        /// <param name="password">
        /// New storage password.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task UpdateAsync(string password);

        #endregion
    }

    /// <summary>
    /// A storage password contract.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.IStoragePassword"/>
    [ContractClassFor(typeof(IStoragePassword))]
    abstract class IStoragePasswordContract : IStoragePassword
    {
        #region Properties

        /// <summary>
        /// Gets the plain text version of the current <see cref= "StoragePassword"/>.
        /// </summary>
        /// <value>
        /// The clear password.
        /// </value>
        /// <seealso cref="P:Splunk.Client.IStoragePassword.ClearPassword"/>
        public abstract string ClearPassword { get; }

        /// <summary>
        /// Gets the extensible administration interface properties for the current
        /// <see cref= "StoragePassword"/>.
        /// </summary>
        /// <value>
        /// The extensible administration interface properties.
        /// </value>
        /// <seealso cref="P:Splunk.Client.IStoragePassword.Eai"/>
        public abstract Eai Eai { get; }

        /// <summary>
        /// Gets an encrypted version of the current <see cref= "StoragePassword"/>.
        /// </summary>
        /// <value>
        /// The encrypted password.
        /// </value>
        /// <seealso cref="P:Splunk.Client.IStoragePassword.EncryptedPassword"/>
        public abstract string EncryptedPassword { get; }

        /// <summary>
        /// Gets the masked version of the current <see cref="StoragePassword"/>.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        /// <seealso cref="P:Splunk.Client.IStoragePassword.Password"/>
        public abstract string Password { get; }

        /// <summary>
        /// Gets the realm in which the current <see cref="StoragePassword"/>
        /// is valid.
        /// </summary>
        /// <value>
        /// The realm.
        /// </value>
        /// <seealso cref="P:Splunk.Client.IStoragePassword.Realm"/>
        public abstract string Realm { get; }

        /// <summary>
        /// Gets the Splunk username associated with the current
        /// <see cref= "StoragePassword"/>.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        /// <seealso cref="P:Splunk.Client.IStoragePassword.Username"/>
        public abstract string Username { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously retrieves a fresh copy of the current <see cref=
        /// "StoragePassword"/> that contains all changes to it since it was 
        /// last retrieved.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public abstract Task GetAsync();

        /// <summary>
        /// Removes the asynchronous.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public abstract Task RemoveAsync();

        /// <summary>
        /// Asynchronously updates the storage password represented by the current
        /// instance.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <see cref="CurrentSnapshot"/> was also updated.
        /// </returns>
        public abstract Task<bool> UpdateAsync(params Argument[] arguments);

        /// <summary>
        /// Asynchronously updates the storage password represented by the current
        /// instance.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <see cref="CurrentSnapshot"/> was also updated.
        /// </returns>
        public abstract Task<bool> UpdateAsync(IEnumerable<Argument> arguments);

        /// <summary>
        /// Asynchronously updates the storage password represented by the current
        /// instance.
        /// </summary>
        /// <param name="password">
        /// New storage password.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <seealso cref="M:Splunk.Client.IStoragePassword.UpdateAsync(string)"/>
        ///
        /// <exception cref="ArgumentNullException">
        /// <paramref name="password"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="RequestException">
        /// 
        /// </exception>
        /// <exception cref="ResourceNotFoundException">
        /// 
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// 
        /// </exception>
        public Task UpdateAsync(string password)
        {
            Contract.Requires<ArgumentNullException>(password != null);
            return default(Task);
        }

        #endregion
    }
}
