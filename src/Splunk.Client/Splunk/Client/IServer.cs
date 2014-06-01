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

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [ContractClass(typeof(IServerContract))]
    public interface IServer
    {
        #region Properties

        /// <summary>
        /// Gets the server messages collection associated with the current
        /// <see cref="Server"/>.
        /// </summary>
        ServerMessageCollection Messages
        { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously retrieves information about the current <see cref="Server"/>.
        /// </summary>
        /// <returns>
        /// An object representing information about the Splunk server.
        /// </returns>
        Task<ServerInfo> GetInfoAsync();

        /// <summary>
        /// Asynchronously retrieves <see cref="ServerSettings"/> from the 
        /// current <see cref="Server"/> endpoint.
        /// </summary>
        /// <returns>
        /// An object representing the server settings from the Splunk server
        /// represented by the current instance.
        /// </returns>
        Task<ServerSettings> GetSettingsAsync();

        /// <summary>
        /// Asynchronously restarts the current <see cref="Server"/> and then
        /// optionally checks for a specified period of time for server
        /// availability.
        /// </summary>
        /// <param name="millisecondsDelay">
        /// The time to wait before canceling the check for server availability.
        /// The default value is <c>60000</c> specifying that the check for
        /// server avaialability will continue for up to 60 seconds. A value
        /// of <c>0</c> specifices that no check should be made. A value of 
        /// <c>-1</c> specifies an infinite wait time.
        /// </param>
        /// <param name="retryInterval">
        /// The time to wait between checks for server availability in 
        /// milliseconds. The default value is <c>250</c> specifying that the
        /// time between checks for server availability is one quarter of a 
        /// second.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="millisecondsDelay"/> is less than <c>-1</c>.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The restart operation failed.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// The check for server availability was canceled after <paramref 
        /// name="millisecondsDelay"/>.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Insufficient privileges to restart the current <see cref="Server"/>.
        /// </exception>
        Task RestartAsync(int millisecondsDelay = 60000, int retryInterval = 250);

        /// <summary>
        /// Asynchronously updates setting values on the current <see cref="Server"/>.
        /// </summary>
        /// <param name="values">
        /// An object representing the setting values to be changed.
        /// </param>
        /// <returns>
        /// An object representing the updated server settings.
        /// </returns>
        Task<ServerSettings> UpdateSettingsAsync(ServerSettingValues values);

        #endregion
    }

    [ContractClassFor(typeof(IServer))]
    abstract class IServerContract : IServer
    {
        public ServerMessageCollection Messages
        {
            get { return null; }
        }

        public Task<ServerInfo> GetInfoAsync()
        {
            return null;
        }

        public Task<ServerSettings> GetSettingsAsync()
        {
            return null;
        }

        public Task RestartAsync(int millisecondsDelay = 60000, int retryInterval = 250)
        {
            Contract.Requires<ArgumentOutOfRangeException>(millisecondsDelay >= -1);
            return null;
        }

        public Task<ServerSettings> UpdateSettingsAsync(ServerSettingValues values)
        {
            Contract.Requires(values != null);
            return null;
        }
    }
}
