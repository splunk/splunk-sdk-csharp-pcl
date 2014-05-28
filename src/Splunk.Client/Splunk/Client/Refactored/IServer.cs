namespace Splunk.Client.Refactored
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IServer
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        ServerMessageCollection Messages
        { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously gets <see cref="ServerInfo"/> from the Splunk server
        /// represented by the current instance.
        /// </summary>
        /// <returns>
        /// An object representing information about the Splunk server.
        /// </returns>
        Task<ServerInfo> GetInfoAsync();

        /// <summary>
        /// Asynchronously retrieves <see cref="ServerSettings"/> from the 
        /// Splunk server represented by the current instance.
        /// </summary>
        /// <returns>
        /// An object representing the server settings from the Splunk server
        /// represented by the current instance.
        /// </returns>
        Task<ServerSettings> GetSettingsAsync();

        /// <summary>
        /// Restarts the Splunk server represented by the current instance 
        /// and then optionally checks for a specified period of time for 
        /// server availability.
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
        Task RestartAsync(int millisecondsDelay, int retryInterval);

        /// <summary>
        /// Asynchronously updates <see cref="ServerSettings"/> on the Splunk 
        /// server represented by the current instance.
        /// </summary>
        /// <param name="values">
        /// An object representing the updated server setting values.
        /// </param>
        /// <returns>
        /// An object representing the updated server settings on the Splunk 
        /// server represented by the current instance.
        /// </returns>
        Task<ServerSettings> UpdateSettingsAsync(ServerSettingValues values);

        #endregion
    }
}
