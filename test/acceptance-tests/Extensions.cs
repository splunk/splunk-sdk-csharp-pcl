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

namespace Splunk.Client.AcceptanceTests
{
    using Splunk.Client;
    using Splunk.Client.Helpers;

    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    
    using Xunit;

    public static class Extensions
    {
        /// <summary>
        /// Create a fresh test app with the given name, delete the existing
        /// test app and reboot Splunk.
        /// </summary>
        /// <param name="name">The app name</param>
        public static async Task<Application> RecreateAsync(this ApplicationCollection applications, string name)
        {
            var app = await applications.GetOrNullAsync(name);

            if (app != null)
            {
                await app.RemoveAsync();
            }

            await applications.CreateAsync(name, "sample_app");
            app = await applications.GetOrNullAsync(name);
            Assert.NotNull(app);

            return app;
        }

        /// <summary>
        /// Asynchronously removes an application by name, if it exists.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the application existed and was removed; otherwise, if 
        /// the application did not exist, <c>false</c>.
        /// </returns>
        /// <param name="applications">Applications.</param>
        /// <param name="name">Name.</param>
        public static async Task<bool> RemoveAsync(this ApplicationCollection applications, string name)
        {
            Application app = await applications.GetOrNullAsync(name);

            if (app == null)
            {
                return false;
            }

            await app.RemoveAsync();
            return true;
        }

        /// <summary>
        /// Waits for updated event count.
        /// </summary>
        /// <returns>The for updated event count.</returns>
        /// <param name="index">Index.</param>
        /// <param name="expectedEventCount">Expected event count.</param>
        /// <param name="seconds">Seconds.</param>
        public static async Task PollForUpdatedEventCount(this Index index, long expectedEventCount, int seconds = 60)
        {
            Stopwatch watch = Stopwatch.StartNew();

            while (watch.Elapsed < new TimeSpan(0, 0, seconds) && index.TotalEventCount != expectedEventCount)
            {
                await Task.Delay(1000);
                await index.GetAsync();
            }

            Assert.Equal(expectedEventCount, index.TotalEventCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">
        /// </param>
        public static async Task<Index> RecreateAsync(this IndexCollection indexes, string name)
        {
            Index index = await indexes.GetOrNullAsync(name);

            if (index != null)
            {
                await index.RemoveAsync();
            }

            await indexes.CreateAsync(name);

            index = await indexes.GetOrNullAsync(name);
            Assert.NotNull(index);

            return index;
        }

        /// <summary>
        /// Asynchronously removes an index by name, if it exists.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the index existed and was removed; otherwise, if 
        /// the application did not exist, <c>false</c>.
        /// </returns>
        /// <param name="applications">Applications.</param>
        /// <param name="name">Name.</param>
        public static async Task<bool> RemoveAsync(this IndexCollection indexes, string name)
        {
            Index index = await indexes.GetOrNullAsync(name);

            if (index == null)
            {
                return false;
            }

            await index.RemoveAsync();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static async Task LogOnAsync(this Service service)
        {
            await service.LogOnAsync(SdkHelper.Splunk.Username, SdkHelper.Splunk.Password);
        }
    }
}
