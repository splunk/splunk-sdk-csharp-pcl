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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TestHelper
    {
        /// <summary>
        /// Root Debug string for assertions
        /// </summary>
        private string assertRoot = "Test Support assert: ";

        /// <summary>
        /// The command object
        /// </summary>
        private Command command;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestHelper"/> class.
        /// </summary>
        public TestHelper()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };

            this.command = Command.Splunk();
        }

        /// <summary>
        /// Connect to splunk using the command line options (or .splunkrc)
        /// </summary>
        /// <returns>The service</returns>
        public Service Connect()
        {
            var service = new Service(Scheme.Https, this.command.Host, this.command.Port, Namespace.Default);
            service.LoginAsync(this.command.Username, this.command.Password).Wait();

            return service;
        }

        public Service Connect(Namespace ns)
        {
            var service = new Service(Scheme.Https, this.command.Host, this.command.Port, ns);
            service.LoginAsync(this.command.Username, this.command.Password).Wait();

            return service;
        }

        /// <summary>
        /// Returns a value dermining whether a string is in the
        /// non-ordered array of strings.
        /// </summary>
        /// <param name="array">The array to scan</param>
        /// <param name="value">The value to look for</param>
        /// <returns>True or false</returns>
        public bool Contains(string[] array, string value)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                if (array[i].Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Create a fresh test app with the given name, delete the existing
        /// test app and reboot Splunk.
        /// </summary>
        /// <param name="name">The app name</param>
        public void CreateApp(string name)
        {
            //EntityCollection<App> apps;

            Service service = this.Connect();

            ApplicationCollection apps = service.GetApplicationsAsync(new ApplicationCollectionArgs()).Result;

            if (apps.Any(a => a.ResourceName.Title == name))
            {
                service.RemoveApplicationAsync(name).Wait();
                this.SplunkRestart();
                service = this.Connect();
                apps = service.GetApplicationsAsync().Result;
            }

            Assert.False(apps.Any(a => a.ResourceName.Title == name), this.assertRoot + "#1");

            //apps.Create(name);
            service.CreateApplicationAsync(name, "sample_app").Wait();

            this.SplunkRestart();

            service = this.Connect();

            apps = service.GetApplicationsAsync().Result;
            Assert.True(apps.Any(a => a.Name == name), this.assertRoot + "#2");
        }

        /// <summary>
        /// Remove the given app and reboot Splunk if needed.
        /// </summary>
        /// <param name="name">The app name</param>
        public void RemoveApp(string name)
        {
            Service service = this.Connect();

            ApplicationCollection apps = service.GetApplicationsAsync().Result;
            if (apps.Any(a => a.Name == name))
            {
                service.RemoveApplicationAsync(name).Wait();
                this.SplunkRestart();
                service = this.Connect();
            }

            apps = service.GetApplicationsAsync().Result;
            Assert.False(apps.Any(a => a.Name == name), this.assertRoot + "#3");
        }

        /// <summary>
        /// Returns the command object, which picks up .splunkrc
        /// </summary>
        /// <returns>The command object</returns>
        public Command SetUp()
        {
            this.command = Command.Splunk();
            return this.command;
        }

        /// <summary>
        /// Restarts splunk with a default 3 minute restart time check.
        /// </summary>
        public void SplunkRestart()
        {
            Stopwatch watch = new Stopwatch();

            Service service = this.Connect();
            watch.Start();
            service.Server.RestartAsync().Wait();
            watch.Stop();

            Console.WriteLine("spend {0}s to restart server", watch.Elapsed.TotalSeconds);
        }

        /// <summary>
        /// Wait for a job to be queryable
        /// </summary>
        /// <param name="job">The job</param>
        /// <returns>The same job</returns>
        public Job Ready(Job job)
        {
            //while (!job.IsReady)
            //{
            //    Thread.Sleep(10);
            //}
            return job;
        }

        /// <summary>
        /// Wait for the given job to complete
        /// </summary>
        /// <param name="job">The job</param>
        /// <returns>The same job</returns>
        public Job Wait(Job job)
        {
            while (!job.IsDone)
            {
                Thread.Sleep(1000);
                job.GetAsync().Wait();

            }

            return job;
        }

        public int VersionCompare(Service service, string versionToCompare)
        {
            Version info = service.Server.GetInfoAsync().Result.Version;
            string version = info.ToString();
            //oat inputVersion = float.Parse(versionToCompare);

            return (string.Compare(version, versionToCompare, StringComparison.InvariantCulture));
            
        }
    }
}
