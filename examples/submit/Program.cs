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

namespace Splunk.Examples.Submit
{
    using Splunk.Client;
    using Splunk.Client.Helpers;
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// An example program to submit events into Splunk.
    /// </summary>
    public class Program
    {
        static Program()
        {
            //// TODO: Use WebRequestHandler.ServerCertificateValidationCallback instead
            //// 1. Instantiate a WebRequestHandler
            //// 2. Set its ServerCertificateValidationCallback
            //// 3. Instantiate a Splunk.Client.Context with the WebRequestHandler

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };
        }

        static void Main(string[] args)
        {
            using (var service = new Service(SDKHelper.UserConfigure.scheme, SDKHelper.UserConfigure.host, SDKHelper.UserConfigure.port, new Namespace(user: "nobody", app: "search")))
            {
                Run(service).Wait();
            }
        }

        /// <summary>
        /// The main program
        /// </summary>
        /// <param name="argv">The command line arguments</param>
        static async Task Run(Service service)
        {
            Console.WriteLine("Login as admin");
            string username = "admin";
            string password = "changeme";
            await service.LoginAsync(username, password);

            Console.WriteLine("create a  index");
            string indexName = "user-index";
            //string source = "*\\splunkd.log";
            //string sourceType = "splunkd";

            if (service.GetIndexesAsync().Result.Any(a => a.Name == indexName))
            {
                await service.RemoveIndexAsync(indexName);
            }

            Index index = await service.CreateIndexAsync(indexName);
            try
            {
                await index.EnableAsync();

                Receiver receiver = service.Receiver;
                ReceiverArgs args = new ReceiverArgs()
                {
                    Index = indexName,
                    ////Source = source,
                    ////SourceType = sourceType,
                };

                await receiver.SendAsync("Hello World.", args);
                await receiver.SendAsync("Goodbye world.", args);

                SearchResultStream results = service.SearchOneshotAsync(
                    string.Format(
                        "search index={0}",// source={2} sourcetype={3}",
                        indexName
                        ////source,
                        ////sourceType
                        )).Result;

                Console.WriteLine(results);              
            }
            finally
            {
                service.RemoveIndexAsync(indexName).Wait();
            }
        }
    }
}
