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
    using Splunk.Sdk;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
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

        [Trait("class", "Service: Applications")]
        [Fact]
        public void CanGetApps()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));

            Func<Task<AppCollection>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");

                var collection = await service.GetAppsAsync();
                return collection;
            };

            var result = Dispatch().Result;
        }

        [Trait("class", "Service: Authentication and Authorization")]
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
                Assert.Equal(requestException.Details[0], new Message(MessageType.Warning, "Login failed"));
            }
        }

        [Trait("class", "Service: Configuration")]
        [Fact]
        public void CanGetConfigurations()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));

            Func<Task<ConfigurationCollection>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");

                var collection = await service.GetConfigurationsAsync();
                return collection;
            };

            var result = Dispatch().Result;
        }

        [Trait("class", "Service: Configuration")]
        [Fact]
        public void CanManipulateConfiguration()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));
            var props = service.GetConfiguration("props");

            service.Login("admin", "changeme");
            props.GetStanzas();
        }

        [Trait("class", "Service: Saved Searches")]
        [Fact]
        public void CanCreateSavedSearch()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));

            Func<Task<SavedSearch>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");

                try
                {
                    service.RemoveSavedSearch("some_saved_search");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                var savedSearchCreationArgs = new SavedSearchCreationArgs("some_saved_search", "search index=_internal | head 1000");
                var savedSearch = await service.CreateSavedSearchAsync(savedSearchCreationArgs);

                return savedSearch;
            };

            var result = Dispatch().Result;
        }

        [Trait("class", "Service: Saved Searches")]
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

        [Trait("class", "Service: Saved Searches")]
        [Fact]
        public void CanGetSavedSearch()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));

            Func<Task<SavedSearch>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");

                var entity = await service.GetSavedSearchAsync("Errors in the last 24 hours");

                Assert.Equal("Errors in the last 24 hours", entity.Title);
                Assert.Equal(entity.Id.ToString(), entity.ToString());
                Assert.Equal("nobody", entity.Author);
                Assert.Equal(ResourceName.SavedSearches, entity.Collection);

                // TODO: Access each and every property of the saved search
                return entity;
            };

            var result = Dispatch().Result;
        }

        [Trait("class", "Service: Saved Searches")]
        [Fact]
        public void CanGetSavedSearches()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));

            Func<Task<SavedSearchCollection>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");

                var collection = await service.GetSavedSearchesAsync();
                return collection;
            };

            var result = Dispatch().Result;
        }

        [Trait("class", "Service: Search Jobs")]
        [Fact]
        public void CanGetJob()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));

            Func<Task<Job>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");
                var job1 = await service.StartJobAsync("search index=_internal | head 100");
                var job2 = await service.GetJobAsync(job1.Title);
                Assert.Equal(job1.Title, job2.Title);
                Assert.Equal(job1.Id, job2.Id);
                Assert.Equal(new SortedDictionary<string, Uri>().Concat(job1.Links), new SortedDictionary<string, Uri>().Concat(job2.Links));
                return job2;
            };

            var result = Dispatch().Result;
        }

        [Trait("class", "Service: Search Jobs")]
        [Fact]
        public void CanGetJobs()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));

            Func<Task<JobCollection>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");

                var collection = await service.GetJobsAsync();
                return collection;
            };

            var result = Dispatch().Result;
        }

        [Trait("class", "Service: Search Jobs")]
        [Fact]
        public void CanStartSearch()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);

            Func<Task<IEnumerable<Splunk.Sdk.Record>>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");

                Job job = await service.StartJobAsync("search index=_internal | head 10");
                SearchResults searchResults = await job.GetSearchResultsAsync();

                var records = new List<Splunk.Sdk.Record>(searchResults);
                return records;
            };
            
            var result = Dispatch().Result;
        }

        [Trait("class", "Service: Search Jobs")]
        [Fact]
        public void CanStartSearchExport()
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

        [Trait("class", "Service: Search Jobs")]
        [Fact]
        public void CanStartSearchOneshot()
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

        [Trait("class", "Service: Server")]
        [Fact]
        public void CanGetServerInfo()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            var serverInfo = service.Server.GetInfo();
            
            Acl acl = serverInfo.Eai.Acl;
            Permissions permissions = acl.Permissions;
            int build = serverInfo.Build;
            string cpuArchitecture = serverInfo.CpuArchitecture;
            Guid guid = serverInfo.Guid;
            bool isFree = serverInfo.IsFree;
            bool isRealtimeSearchEnabled = serverInfo.IsRealtimeSearchEnabled;
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

        [Trait("class", "Service: Server")]
        [Fact]
        public void CanRestartServer()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            service.LoginAsync("admin", "changeme").Wait();
            service.Server.Restart();
        }

        public void SetFixture(AcceptanceTestingSetup data)
        { }
    }
}
