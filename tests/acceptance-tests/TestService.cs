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

namespace Splunk.Sdk.UnitTesting
{
    using System;
    using System.Net;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Splunk.Sdk;
    using Xunit;

    public class TestService : IUseFixture<AcceptanceTestingSetup>
    {
        [Trait("class", "Service")]
        [Fact]
        public void CanConstruct()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            Assert.Equal(service.ToString(), "https://localhost:8089/services");
        }

        [Trait("class", "Service")]
        [Fact]
        public void CanLogin()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            Task task;

            task = service.LoginAsync("admin", "changeme");
            task.Wait();

            Assert.Equal(task.Status, TaskStatus.RanToCompletion);
            Assert.NotNull(service.SessionKey);

            try
            {
                task = service.LoginAsync("admin", "bad-password");
                task.Wait();
            }
            catch (Exception)
            {
                Assert.Equal(task.Status, TaskStatus.Faulted);
                Assert.IsType(typeof(AggregateException), task.Exception);

                var aggregateException = (AggregateException)task.Exception;
                Assert.Equal(aggregateException.InnerExceptions.Count, 1);
                Assert.IsType(typeof(RequestException), aggregateException.InnerExceptions[0]);

                var requestException = (RequestException)(aggregateException.InnerExceptions[0]);
                Assert.Equal(requestException.StatusCode, HttpStatusCode.Unauthorized);
                Assert.Equal(requestException.Details.Count, 1);
                Assert.Equal(requestException.Details[0], new Message(XElement.Parse(@"<msg type=""WARN"">Login failed</msg>")));
            }
        }

        [Trait("class", "Service")]
        [Fact]
        public void CanDispatchSavedSearch()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));

            Func<Task<IEnumerable<Splunk.Sdk.Record>>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");
                
                Job job = await service.DispatchSavedSearchAsync("Splunk errors last 24 hours");
                SearchResults searchResults = await job.GetSearchResultsAsync();
                
                var records = new List<Splunk.Sdk.Record>(searchResults);
                return records;
            };

            var result = Dispatch().Result;
        }

        [Trait("class", "Service")]
        [Fact]
        public void CanDispatchSearch()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);

            Func<Task<IEnumerable<Splunk.Sdk.Record>>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");

                Job job = await service.SearchAsync("search index=_internal | head 10");
                SearchResults searchResults = await job.GetSearchResultsAsync();

                var records = new List<Splunk.Sdk.Record>(searchResults);
                return records;
            };
            
            var result = Dispatch().Result;
        }

        [Trait("class", "Service")]
        [Fact]
        public void CanDispatchSearchExport()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);

            Func<Task<IEnumerable<Splunk.Sdk.Record>>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");

                SearchResultsReader reader = await service.SearchExportAsync(new SearchExportArgs("search index=_internal | head 1000") { Count = 0 });
                var records = new List<Splunk.Sdk.Record>();

                foreach (var searchResults in reader)
                {
                    records.AddRange(searchResults);
                }
                return records;
            };

            var result = Dispatch().Result;
        }

        [Trait("class", "Service")]
        [Fact]
        public void CanDispatchSearchOneshot()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);

            Func<Task<IEnumerable<Splunk.Sdk.Record>>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");

                SearchResults searchResults = await service.SearchOneshotAsync(new JobArgs("search index=_internal | head 100") { MaxCount = 100000 });
                var records = new List<Splunk.Sdk.Record>(searchResults);

                return records;
            };

            var result = Dispatch().Result;
        }

        [Trait("class", "Service")]
        [Fact]
        public void CanGetServerInfo()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            var serverInfo = service.Server.GetInfo();
            
            Acl acl = serverInfo.Acl;
            int build = serverInfo.Build;
            string cpuArchitecture = serverInfo.CpuArchitecture;
            Guid guid = serverInfo.Guid;
            bool isFree = serverInfo.IsFree;
            bool isRealtimeSearchEnabled = serverInfo.IsRealTimeSearchEnabled;
            bool isTrial = serverInfo.IsTrial;
            IReadOnlyList<string> licenseKeys = serverInfo.LicenseKeys;
            IReadOnlyList<string> licenseLabels = serverInfo.LicenseLabels;
            string licenseSignature = serverInfo.LicenseSignature;
            LicenseState licenseState = serverInfo.LicenseState;
            Guid masterGuid = serverInfo.MasterGuid;
            ServerMode mode = serverInfo.Mode;
            string osBuild = serverInfo.OSBuild;
            string osName = serverInfo.OSName;
            string osVersion = serverInfo.OSVersion;
            string serverName = serverInfo.ServerName;
            Version version = serverInfo.Version;
        }

        public void SetFixture(AcceptanceTestingSetup data)
        { }
    }
}
