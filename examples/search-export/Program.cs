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

namespace search_export
{
    using Splunk.Client;
    using Splunk.Client.Helpers;

    using System;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            using (var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search")))
            {
                Run(service).Wait();
            }

            Console.Write("Press return to exit: ");
            Console.ReadLine();
        }

        public static async Task Run(Service service)
        {
            await service.LoginAsync(SdkHelper.Splunk.Username, SdkHelper.Splunk.Password);

            //// Search : Export Previews

            using (SearchPreviewStream stream = await service.ExportSearchPreviewsAsync("search index=_internal | head 100"))
            {
                int previewNumber = 0;

                foreach (SearchPreview preview in stream)
                {
                    int resultNumber = 0;

                    Console.WriteLine("Preview {0:D8}: {1}", ++previewNumber, preview.IsFinal ? "final" : "partial");

                    foreach (var result in preview.Results)
                    {
                        Console.WriteLine(string.Format("{0:D8}: {1}", ++resultNumber, result));
                    }
                }
            }
        }
    }
}
