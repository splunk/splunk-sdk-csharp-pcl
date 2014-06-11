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

namespace Splunk.Client.UnitTests
{
    using Splunk.Client;
    using Splunk.Client.Helpers;

    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    
    using Xunit;

    /// <summary>
    /// Test helper class
    /// </summary>
    public static class TestHelper
    {
        public static int VersionCompare(Service service, string versionToCompare)
        {
            Version info = service.Server.GetInfoAsync().Result.Version;
            string version = info.ToString();
            return string.Compare(version, versionToCompare, StringComparison.InvariantCulture);
        }

        public static async Task WaitIndexTotalEventCountUpdated(Index index, long expectedEventCount, int seconds = 60)
        {
            Stopwatch watch = Stopwatch.StartNew();

            while (watch.Elapsed < new TimeSpan(0, 0, seconds) && index.TotalEventCount != expectedEventCount)
            {
                await Task.Delay(1000);
                await index.GetAsync();
            }

            Console.WriteLine("Waited {0} seconds for index {1} event count to reach {2} and it reached {3}.", 
                watch.Elapsed, expectedEventCount, index.TotalEventCount, index.Name);

            Assert.Equal(expectedEventCount, index.TotalEventCount);
        }

       /// <summary>
        /// Create a fresh test app with the given name, delete the existing
        /// test app and reboot Splunk.
        /// </summary>
        /// <param name="name">The app name</param>
        public static async Task CreateApp(string name)
        {
            //EntityCollection<App> apps;

            using (var service = await SDKHelper.CreateService())
            {
                var app = await service.Applications.GetOrNullAsync(name);

                if (app != null)
                {
                    await app.RemoveAsync();
                    await service.Server.RestartAsync();
                    await service.LoginAsync(SDKHelper.UserConfigure.username, SDKHelper.UserConfigure.password);
                }

                await service.Applications.CreateAsync(name, "sample_app");
                await service.Server.RestartAsync();
                await service.LoginAsync(SDKHelper.UserConfigure.username, SDKHelper.UserConfigure.password);

                app = await service.Applications.GetOrNullAsync(name);
                Assert.NotNull(app);
            }
        }

        /// <summary>
        /// Remove the given app and reboot Splunk if needed.
        /// </summary>
        /// <param name="name">The app name</param>
        public static async Task<bool> RemoveApp(string name)
        {
            using (var service = await SDKHelper.CreateService())
            {
                Application app = await service.Applications.GetOrNullAsync(name);

                if (app == null)
                {
                    return false;
                }

                await app.RemoveAsync();
                await service.Server.RestartAsync();
                await service.LoginAsync(SDKHelper.UserConfigure.username, SDKHelper.UserConfigure.password);

                app = await service.Applications.GetOrNullAsync(name);
                Assert.Null(app);

                return true;
            }
        }
    }
}
