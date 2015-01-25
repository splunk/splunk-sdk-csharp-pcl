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

namespace Splunk.Examples.GetSplunkDataInputs.Not
{
    using Splunk.Client;
    using Splunk.Client.Helpers;
    using System;
    using System.Dynamic;
    using System.Threading.Tasks;

    /// <summary>
    /// An example program to authenticate to the server and print the received
    /// token.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void NotMain(string[] args)
        {
            var ns = new Namespace(user: args[0], app: args[1]);

            using (var service = new Service(SdkHelper.Splunk.Scheme, SdkHelper.Splunk.Host, SdkHelper.Splunk.Port, ns))
            {
                Console.WriteLine("Splunk service endpoint: {0}", service);
                Run(service).Wait();
            }

            Console.Write("Press enter to exit: ");
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
            Console.WriteLine("Data inputs:");

            var collection = service.CreateEntityCollection("data", "inputs", "all");
            int count = 0;
            await collection.GetAllAsync();

            foreach (var entity in collection)
            {
                Console.WriteLine("{0:D5}. {1}", ++count, entity.Id);
                dynamic dataInput = entity.Dynamic.Content;

                Console.WriteLine("       Disabled: {0}", dataInput.Disabled);
                Console.WriteLine("          Index: {0}", dataInput.Index);
                Console.WriteLine("           Type: {0}", dataInput.Eai.Type);
            }

            await service.LogOffAsync();
        }
    }
}
