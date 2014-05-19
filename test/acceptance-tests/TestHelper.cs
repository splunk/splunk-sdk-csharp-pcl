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

namespace Splunk.Client.UnitTesting
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    using Splunk.Client;
    using Splunk.Client.UnitTesting;

    using Xunit;
    using System.Diagnostics;
    using System.IO;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Test helper class
    /// </summary>
    static class TestHelper
    {
        static TestHelper()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };

            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            UserConfigure = new SplunkRC();
            LoadSplunkRC(Path.Combine(home, ".splunkrc"));
        }

        public static SplunkRC UserConfigure
        { get; private set; }

        /// <summary>
        /// Create a Splunk <see cref="Service"/> and login using the command 
        /// line options (or .splunkrc)
        /// </summary>
        /// <returns>
        /// The service created.
        /// </returns>
        public static async Task<Service> CreateService(Namespace ns = null)
        {
            var service = new Service(UserConfigure.scheme, UserConfigure.host, UserConfigure.port, ns);
            
            await service.LoginAsync(UserConfigure.username, UserConfigure.password);
            Assert.NotNull(service.SessionKey);

            return service;
        }

        public static int VersionCompare(Service service, string versionToCompare)
        {
            Version info = service.Server.GetInfoAsync().Result.Version;
            string version = info.ToString();
            return (string.Compare(version, versionToCompare, StringComparison.InvariantCulture));
        }

        public static async Task WaitIndexTotalEventCountUpdated(Index index, long expectEventCount, int seconds=60)
        {
            Stopwatch watch = Stopwatch.StartNew();
            while (watch.Elapsed < new TimeSpan(0, 0, seconds) && index.TotalEventCount != expectEventCount)
            {
                await Task.Delay(1000);
                await index.GetAsync();
            }

            Console.WriteLine("Sleep {0}s to wait index {2} 'TotalEventCount got updated, current index.TotalEventCount={1}", watch.Elapsed, index.TotalEventCount, index.Name);
            Assert.True(index.TotalEventCount == expectEventCount);
        }

        public static async Task RestartServer()
        {
            Stopwatch watch = Stopwatch.StartNew();

            Service service = await CreateService();
            try
            {
                await service.Server.RestartAsync();
                Console.WriteLine("{1},  spend {0}s to restart server successfully", watch.Elapsed.TotalSeconds, DateTime.Now);
            }
            catch (Exception e)
            {
                Console.WriteLine("----------------------------------------------------------------------------------------");
                Console.WriteLine("{1}, spend {0}s to restart server failed:", watch.Elapsed.TotalSeconds, DateTime.Now);
                Console.WriteLine(e);
                Console.WriteLine("----------------------------------------------------------------------------------------");
            }

            watch.Stop();
        }

        /// <summary>
        /// Create a fresh test app with the given name, delete the existing
        /// test app and reboot Splunk.
        /// </summary>
        /// <param name="name">The app name</param>
        public async static void CreateApp(string name)
        {
            //EntityCollection<App> apps;

            Service service = await CreateService();

            ApplicationCollection apps = service.GetApplicationsAsync(new ApplicationCollectionArgs()).Result;

            if (apps.Any(a => a.ResourceName.Title == name))
            {
                service.RemoveApplicationAsync(name).Wait();
                await RestartServer();
                service = await CreateService();
                apps = service.GetApplicationsAsync().Result;
            }

            Assert.False(apps.Any(a => a.ResourceName.Title == name));

            //apps.Create(name);
            service.CreateApplicationAsync(name, "sample_app").Wait();

            await RestartServer();

            service = await CreateService();

            apps = service.GetApplicationsAsync().Result;
            Assert.True(apps.Any(a => a.Name == name));
        }

        /// <summary>
        /// Remove the given app and reboot Splunk if needed.
        /// </summary>
        /// <param name="name">The app name</param>
        public static async void RemoveApp(string name)
        {
            Service service = await CreateService();

            ApplicationCollection apps = service.GetApplicationsAsync().Result;
            if (apps.Any(a => a.Name == name))
            {
                service.RemoveApplicationAsync(name).Wait();
                await RestartServer();
                service = await CreateService();
            }

            apps = service.GetApplicationsAsync().Result;
            Assert.False(apps.Any(a => a.Name == name));
        }

        /// <summary>
        /// Load a file of options and arguments
        /// </summary>
        /// <param name="path">
        /// The path to the .splunkrc file.
        /// </param>
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
 
        internal class SplunkRC
        {
            public string username = "admin";
            public string password = "changeme";
            public Scheme scheme = Scheme.Https;
            public int port = 8089;
            public string host = "localhost";
        }
    }
}
