/*
 * Copyright 2013 Splunk, Inc.
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

namespace Splunk.Examples.Authenticate
{
    using Splunk.Client;
    using Splunk.Client.Helpers;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// An example program to authenticate to the server and print the received
    /// token.
    /// </summary>
    public class Program
    {
        static Program()
        {
            // TODO: Use WebRequestHandler.ServerCertificateValidationCallback instead
            // 1. Instantiate a WebRequestHandler
            // 2. Set its ServerCertificateValidationCallback
            // 3. Instantiate a Splunk.Client.Context with the WebRequestHandler
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };
        }

        /// <summary>
        /// Mains function
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            using (var service = new Service(SDKHelper.UserConfigure.scheme, SDKHelper.UserConfigure.host, SDKHelper.UserConfigure.port))
            {
                Console.WriteLine("Connected to {0}:{1} ", service.Server.Context.Host, service.Server.Context.Port);
                Run(service).Wait();
            }
        }

        /// <summary>
        /// Runs the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>a task</returns>
        private static async Task Run(Service service)
        {
            try
            {
                await service.GetConfigurationsAsync();
            }
            catch (AuthenticationFailureException)
            {
                Console.WriteLine("Can't get service configuration without log in");
            }

            Console.WriteLine("Login as admin");
            string username = "admin";
            string password = "changeme";
            await service.LoginAsync(username, password);

            Console.WriteLine("List all configurations of the Splunk service:");
            ConfigurationCollection configs = service.GetConfigurationsAsync().Result;
            foreach (Configuration config in configs)
            {
                Console.WriteLine(config.Id);
            }

            Console.WriteLine("Log off");
            await service.LogoffAsync();
        }
    }
}