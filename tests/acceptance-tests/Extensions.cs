namespace Splunk.Sdk.UnitTesting
{
    using Splunk.Sdk;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class Extensions
    {
        #region Authentication/authorization

        /// <summary>
        /// Retrieves the list of all Splunk system capabilities.
        /// </summary>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/kgTKvM">GET 
        /// authorization/capabilities</a> endpoint to construct a 
        /// of available capabilities.
        /// </remarks>
        public static dynamic GetCapabilities(this Service service)
        {
            return service.GetCapabilitiesAsync().Result;
        }

        /// <summary>
        /// Provides user authentication.
        /// </summary>
        /// <param name="username">
        /// Splunk account username.
        /// </param>
        /// <param name="password">
        /// Splunk account password for username.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/hdNhwA">POST 
        /// auth/login</a> endpoint. The session key this endpoint returns is 
        /// used for subsequent requests. It is accessible via the <see cref=
        /// "SessionKey"/> property.
        /// </remarks>
        public static void Login(this Service service, string username, string password)
        {
            service.LoginAsync(username, password).Wait();
        }

        #endregion

        #region Indexes

        public static Index CreateIndex(this Service service, string name, IndexArgs args, IndexAttributes attributes = null)
        {
            return service.CreateIndexAsync(name, args, attributes).Result;
        }

        public static Index GetIndex(this Service service, string name)
        {
            return service.GetIndexAsync(name).Result;
        }

        public static IndexCollection GetIndexes(this Service service, IndexCollectionArgs args = null)
        {
            return service.GetIndexesAsync(args).Result;
        }

        public static void RemoveIndex(this Service service, string name)
        {
            service.RemoveIndexAsync(name).Wait();
        }

        public static void UpdateIndex(this Service service, string name, IndexAttributes attributes)
        {
            service.UpdateIndexAsync(name, attributes).Wait();
        }

        #endregion
    }
}
