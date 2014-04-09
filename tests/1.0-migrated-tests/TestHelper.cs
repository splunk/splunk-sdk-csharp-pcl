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

namespace Splunk.Sdk.UnitTesting
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Splunk.Sdk;
    using Splunk.Sdk.UnitTesting;
    using SplunkSDKHelper;

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
            //var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            var service = new Service(Scheme.Https, this.command.Host, this.command.Port, Namespace.Default);
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

          
            //if (apps.GetAsync(name))
            //{
            //     apps.Remove(name);
            //    this.SplunkRestart();
            //    service = this.Connect();
            //}

            //apps = service.GetApplicationsAsync().Result;
            //Assert.IsFalse(apps.ContainsKey(name), this.assertRoot + "#1");

            //apps.Create(name);
            this.SplunkRestart();

            service = this.Connect();

            apps = service.GetApplicationsAsync().Result;
            //Assert.IsTrue(apps.ContainsKey(name), this.assertRoot + "#2");
        }

        /// <summary>
        /// Remove the given app and reboot Splunk if needed.
        /// </summary>
        /// <param name="name">The app name</param>
        public void RemoveApp(string name)
        {
            //EntityCollection<App> apps;

            //Service service = this.Connect();

            //apps = service.GetAppsAsync().Result;
            //if (apps.ContainsKey(name))
            //{
            //    apps.Remove(name);
            //    this.SplunkRestart();
            //    service = this.Connect();
            //}

            //apps = service.GetAppsAsync().Result;
            //Assert.IsFalse(apps.ContainsKey(name), this.assertRoot + "#3");
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
            // If not specified, use 3 minutes (in milliseconds) as default
            // restart timeout.
            this.SplunkRestart(3 * 60 * 1000);
        }

        /// <summary>
        /// Restarts splunk -- If the restart (i.e. splunk is back up) does
        /// not happen by the time the millisecond timeout counts down, an
        /// the assertion that the service is back up will throw an exception.
        /// </summary>
        /// <param name="millisecondTimeout">The number of milliseconds</param>
        public void SplunkRestart(int millisecondTimeout) 
        {
            bool restarted = false;

            Service service = this.Connect();

            service.Server.RestartAsync().Wait();

            // Sniff the management port. We expect the port to be up for a short
            // while, and then no conection
            int totalTime = 0;

            //// Server is back up, wait until socket no longer accepted.
            //while (totalTime < millisecondTimeout)
            //{
            //    try
            //    {
            //        Socket socket = service.Open(service.Port);
            //        socket.Close();
            //        Thread.Sleep(10);
            //        totalTime += 10;
            //    }
            //    catch (Exception)
            //    {
            //        break;
            //    }
            //}

            //// server down, wait until socket accepted.
            //while (totalTime < millisecondTimeout)
            //{
            //    try
            //    {
            //        Socket socket = service.Open(service.Port);
            //        socket.Close();
            //        break;
            //    }
            //    catch (Exception)
            //    {
            //        Thread.Sleep(10);
            //        totalTime += 10;
            //    }
            //}

            while (totalTime < millisecondTimeout)
            {
                try
                {
                    this.Connect();
                    restarted = true;
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                    totalTime += 100;
                }
            }

            Assert.IsTrue(restarted, this.assertRoot + "#5");
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
            }

            return job;
        }
    }
}
