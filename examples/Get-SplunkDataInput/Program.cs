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
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
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
            var ns = new Namespace(user: args[0], app: args[1]);

            using (var service = new Service(SdkHelper.Splunk.Scheme, SdkHelper.Splunk.Host, SdkHelper.Splunk.Port, ns))
            {
                Console.WriteLine("Connected to {0}", service);
                Run(service).Wait();
            }

            Console.Write("Press return to exit: ");
            Console.ReadLine();
        }

        /// <summary>
        /// Runs the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>a task</returns>
        static async Task Run(Service service)
        {
            await service.LogOnAsync(SdkHelper.Splunk.Username, SdkHelper.Splunk.Password);

            Console.WriteLine("List all configurations of the Splunk service:");
            await service.Configurations.GetAllAsync();

            foreach (Configuration config in service.Configurations)
            {
                Console.WriteLine(config.Id);
            }

            Console.WriteLine("Log off");
            await service.LogOffAsync();
        }
    }
}
