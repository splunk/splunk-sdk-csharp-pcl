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

namespace Splunk.Examples.GetSplunkDataInputs
{
    using Splunk.Client;
    using Splunk.Client.Helpers;
    using System;
    using System.Dynamic;
    using System.Threading.Tasks;

    /// <summary>
    /// An example program to access the set of data inputs running on a Splunk instance.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            var ns = new Namespace(user: args[0], app: args[1]);

            using (var service = new Service(SdkHelper.Splunk.Scheme, SdkHelper.Splunk.Host, SdkHelper.Splunk.Port, ns))
            {
                Console.WriteLine("Splunk management port: {0}", service.Context);
                Console.WriteLine("Namespace: {0}", service.Namespace);
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
            try
            {
                await service.LogOnAsync(SdkHelper.Splunk.Username, SdkHelper.Splunk.Password);

                Console.WriteLine("Data inputs:");

                var collection = service.CreateEntityCollection("data", "inputs", "all");
                int count = 0;
                await collection.GetAllAsync();

                //// TODO:
                //// [X] 1. Make Entity<TResource>.Content public (It still must be assigned to a dynamic variable before use.)
                //// [X] 2. Write a convenience method on the Service object for getting an EntityCollection on some path: service.CreateEntityCollection("data", "inputs", "all")
                //// [X] 3. Write a convenience method on the Service object for getting an Entity on some path: something like service.CreateEntity("data", "inputs", "tcp", "ssl")
                //// [X] 4. Add service.CreateEntityCollectionCollection for completeness.
                //// [X] 5. Revert the access change to EntityCollection<Entity<TResource>, TResource>(service, new ResourceName("data", "inputs", "all")) because (2) makes it unnecessary.
                //// [X] 6. Revert the access change to Entity<TResource>(service, new ResourceName("data", "inputs", "all")) because (3) makes it unnecessary.
                //// [ ] 7. Enliven the example.
                //// [ ] 8. Fill holes in IService (holes are unrelated to this exercise)

                foreach (var entity in collection)
                {
                    Console.WriteLine("{0:D5}. {1}", ++count, entity.Id);
                    dynamic dataInput = entity.Content;

                    Console.WriteLine("       Disabled: {0}", dataInput.Disabled);
                    Console.WriteLine("          Index: {0}", dataInput.Index);
                    Console.WriteLine("           Type: {0}", dataInput.Eai.Type);

                    if (dataInput.Eai.Type == "example_hydra_scheduler")
                    {
                        // Restart...
                        await entity.InvokeAsync("disable");
                        await entity.InvokeAsync("enable");
                    }
                }

                await service.LogOffAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
