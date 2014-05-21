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

namespace Splunk.Client.Helpers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using Splunk.Client;
    using System.Diagnostics;
    using System.IO;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public class SDKHelper
    {
        /// <summary>
        /// Initializes the <see cref="SDKHelper" /> class.
        /// </summary>
        static SDKHelper()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };

            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            UserConfigure = new SplunkRC();
            LoadSplunkRC(Path.Combine(home, ".splunkrc"));
        }

        /// <summary>
        /// Gets the user configure.
        /// </summary>
        /// <value>
        /// The user configure.
        /// </value>
        public static SplunkRC UserConfigure { get; private set; }

        /// <summary>
        /// Create a Splunk 
        /// <see cref="Service" /> and login using the command
        /// line options (or .splunkrc)
        /// </summary>
        /// <param name="ns">The ns.</param>
        /// <returns>
        /// The service created.
        /// </returns>
        public static async Task<Service> CreateService(Namespace ns = null)
        {
            var service = new Service(UserConfigure.scheme, UserConfigure.host, UserConfigure.port, ns);
            await service.LoginAsync(UserConfigure.username, UserConfigure.password);
            return service;
        }

        /// <summary>
        /// Load a file of options and arguments
        /// </summary>
        /// <param name="path">The path to the .splunkrc file.</param>
        static void LoadSplunkRC(string path)
        {
            var reader = new StreamReader(path);

            List<string> argList = new List<string>(4);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();

                if (line.StartsWith("#", StringComparison.InvariantCulture))
                {
                    continue;
                }

                if (line.Length == 0)
                {
                    continue;
                }

                argList.Add(line);
            }

            SplunkRC splunkrc = new SplunkRC();

            foreach (string arg in argList)
            {
                string[] strs = arg.Split('=');

                switch (strs[0].ToLower().TrimStart().TrimEnd())
                {
                    case "username": UserConfigure.username = strs[1].TrimEnd().TrimStart(); break;
                    case "password": UserConfigure.password = strs[1].TrimEnd().TrimStart(); break;
                    case "scheme": UserConfigure.scheme = strs[1].TrimEnd().TrimStart() == "https" ? Scheme.Https : Scheme.Http; break;
                    case "port": UserConfigure.port = int.Parse(strs[1].TrimEnd().TrimStart()); break;
                    case "host": UserConfigure.host = strs[1].TrimEnd().TrimStart(); break;
                }
            }
        }

        /// <summary>
        /// represent .splunkrc file
        /// </summary>
        public class SplunkRC
        {
            /// <summary>
            /// The username
            /// </summary>
            public string username = "admin";
            /// <summary>
            /// The password
            /// </summary>
            public string password = "changeme";
            /// <summary>
            /// The scheme
            /// </summary>
            public Scheme scheme = Scheme.Https;
            /// <summary>
            /// The port
            /// </summary>
            public int port = 8089;
            /// <summary>
            /// The host
            /// </summary>
            public string host = "localhost";
        }
    }
}
