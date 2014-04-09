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

        #region Access Control

        [Trait("class", "Service: Access Control")]
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

        #endregion

        #region Applications

        [Trait("class", "Service: Applications")]
        [Fact]
        public void CanGetApps()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));

            Func<Task<ApplicationCollection>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");

                var collection = await service.GetApplicationsAsync();
                return collection;
            };

            var result = Dispatch().Result;
        }

        #endregion

        #region Configuration

        [Trait("class", "Service: Configuration")]
        [Fact]
        public void CanCreateConfiguration()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));

            Func<Task<Configuration>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");

                await service.CreateConfigurationAsync("some_configuration");
                var entity = await service.GetConfigurationAsync("some_configuration");
                return entity;
            };

            var result = Dispatch().Result;
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
        public async Task CanManipulateConfiguration()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));
            await service.LoginAsync("admin", "changeme");

            ConfigurationCollection configurations = null;

            Assert.DoesNotThrow(() => configurations = service.GetConfigurationsAsync().Result);

            // Read the entire configuration system

            foreach (var configuration in configurations)
            {
                configuration.GetAsync().Wait();

                foreach (ConfigurationStanza stanza in configuration)
                {
                    Assert.NotNull(stanza);
                    stanza.GetAsync().Wait();
                }
            }

            // Get or create a custom configuration

            // TODO: create an app and add a custom configuration to it, then delete the app
            // Why? You can remove an app, including its configuration, but not a configuration

            try
            {
                var configuration = await service.GetConfigurationAsync("custom");
            }
            catch (AggregateException e)
            {
                Assert.Equal(1, e.InnerExceptions.Count);

                var requestException = e.InnerExceptions[0] as RequestException;

                Assert.NotNull(requestException);
                Assert.Equal(HttpStatusCode.NotFound, requestException.StatusCode);
                Assert.NotNull(requestException.Details);

                Configuration configuration;
                Assert.DoesNotThrow(() => service.CreateConfigurationAsync("custom").Wait());
                Assert.DoesNotThrow(() => configuration = service.GetConfigurationAsync("custom").Result);
            }
        }

        #endregion

        #region Indexes

        [Trait("class", "Service: Indexes")]
        [Fact]
        public async Task CanGetIndexes()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));
            await service.LoginAsync("admin", "changeme");
            var collection = service.GetIndexesAsync().Result;

            foreach (var entity in collection)
            {
                await entity.GetAsync();

                Assert.Equal(entity.ToString(), entity.Id.ToString());

                Assert.DoesNotThrow(() => { bool value = entity.AssureUTF8; });
                Assert.DoesNotThrow(() => { string value = entity.BlockSignatureDatabase; });
                Assert.DoesNotThrow(() => { int value = entity.BlockSignSize; });
                Assert.DoesNotThrow(() => { int value = entity.BloomFilterTotalSizeKB; });
                Assert.DoesNotThrow(() => { string value = entity.BucketRebuildMemoryHint; });
                Assert.DoesNotThrow(() => { string value = entity.ColdPath; });
                Assert.DoesNotThrow(() => { string value = entity.ColdPathExpanded; });
                Assert.DoesNotThrow(() => { string value = entity.ColdToFrozenDir; });
                Assert.DoesNotThrow(() => { string value = entity.ColdToFrozenScript; });
                Assert.DoesNotThrow(() => { int value = entity.CurrentDBSizeMB; });
                Assert.DoesNotThrow(() => { string value = entity.DefaultDatabase; });
                Assert.DoesNotThrow(() => { bool value = entity.Disabled; });
                Assert.DoesNotThrow(() => { Eai value = entity.Eai; });
                Assert.DoesNotThrow(() => { bool value = entity.EnableOnlineBucketRepair; });
                Assert.DoesNotThrow(() => { bool value = entity.EnableRealtimeSearch; });
                Assert.DoesNotThrow(() => { int value = entity.FrozenTimePeriodInSecs; });
                Assert.DoesNotThrow(() => { string value = entity.HomePath; });
                Assert.DoesNotThrow(() => { string value = entity.HomePathExpanded; });
                Assert.DoesNotThrow(() => { string value = entity.IndexThreads; });
                Assert.DoesNotThrow(() => { bool value = entity.IsInternal; });
                Assert.DoesNotThrow(() => { bool value = entity.IsReady; });
                Assert.DoesNotThrow(() => { bool value = entity.IsVirtual; });
                Assert.DoesNotThrow(() => { long value = entity.LastInitSequenceNumber; });
                Assert.DoesNotThrow(() => { long value = entity.LastInitTime; });
                Assert.DoesNotThrow(() => { string value = entity.MaxBloomBackfillBucketAge; });
                Assert.DoesNotThrow(() => { int value = entity.MaxBucketSizeCacheEntries; });
                Assert.DoesNotThrow(() => { int value = entity.MaxConcurrentOptimizes; });
                Assert.DoesNotThrow(() => { string value = entity.MaxDataSize; });
                Assert.DoesNotThrow(() => { int value = entity.MaxHotBuckets; });
                Assert.DoesNotThrow(() => { int value = entity.MaxHotIdleSecs; });
                Assert.DoesNotThrow(() => { int value = entity.MaxHotSpanSecs; });
                Assert.DoesNotThrow(() => { int value = entity.MaxMemMB; });
                Assert.DoesNotThrow(() => { int value = entity.MaxMetaEntries; });
                Assert.DoesNotThrow(() => { int value = entity.MaxRunningProcessGroups; });
                Assert.DoesNotThrow(() => { int value = entity.MaxRunningProcessGroupsLowPriority; });
                Assert.DoesNotThrow(() => { DateTime value = entity.MaxTime; });
                Assert.DoesNotThrow(() => { int value = entity.MaxTimeUnreplicatedNoAcks; });
                Assert.DoesNotThrow(() => { int value = entity.MaxTimeUnreplicatedWithAcks; });
                Assert.DoesNotThrow(() => { int value = entity.MaxTotalDataSizeMB; });
                Assert.DoesNotThrow(() => { int value = entity.MaxWarmDBCount; });
                Assert.DoesNotThrow(() => { string value = entity.MemPoolMB; });
                Assert.DoesNotThrow(() => { string value = entity.MinRawFileSyncSecs; });
                Assert.DoesNotThrow(() => { DateTime value = entity.MinTime; });
                Assert.DoesNotThrow(() => { int value = entity.PartialServiceMetaPeriod; });
                Assert.DoesNotThrow(() => { int value = entity.ProcessTrackerServiceInterval; });
                Assert.DoesNotThrow(() => { int value = entity.QuarantineFutureSecs; });
                Assert.DoesNotThrow(() => { int value = entity.QuarantinePastSecs; });
                Assert.DoesNotThrow(() => { int value = entity.RawChunkSizeBytes; });
                Assert.DoesNotThrow(() => { int value = entity.RepFactor; });
                Assert.DoesNotThrow(() => { int value = entity.RotatePeriodInSecs; });
                Assert.DoesNotThrow(() => { int value = entity.ServiceMetaPeriod; });
                Assert.DoesNotThrow(() => { bool value = entity.ServiceOnlyAsNeeded; });
                Assert.DoesNotThrow(() => { int value = entity.ServiceSubtaskTimingPeriod; });
                Assert.DoesNotThrow(() => { string value = entity.SummaryHomePathExpanded; });
                Assert.DoesNotThrow(() => { bool value = entity.Sync; });
                Assert.DoesNotThrow(() => { bool value = entity.SyncMeta; });
                Assert.DoesNotThrow(() => { string value = entity.ThawedPath; });
                Assert.DoesNotThrow(() => { string value = entity.ThawedPathExpanded; });
                Assert.DoesNotThrow(() => { int value = entity.ThrottleCheckPeriod; });
                Assert.DoesNotThrow(() => { long value = entity.TotalEventCount; });
                Assert.DoesNotThrow(() => { string value = entity.TStatsHomePath; });
                Assert.DoesNotThrow(() => { string value = entity.TStatsHomePathExpanded; });

                var sameEntity = await service.GetIndexAsync(entity.ResourceName.Title);

                Assert.Equal(entity.ResourceName, sameEntity.ResourceName);

                Assert.Equal(entity.AssureUTF8, sameEntity.AssureUTF8);
                Assert.Equal(entity.BlockSignatureDatabase, sameEntity.BlockSignatureDatabase);
                Assert.Equal(entity.BlockSignSize, sameEntity.BlockSignSize);
                Assert.Equal(entity.BloomFilterTotalSizeKB, sameEntity.BloomFilterTotalSizeKB);
                Assert.Equal(entity.BucketRebuildMemoryHint, sameEntity.BucketRebuildMemoryHint);
                Assert.Equal(entity.ColdPath, sameEntity.ColdPath);
                Assert.Equal(entity.ColdPathExpanded, sameEntity.ColdPathExpanded);
                Assert.Equal(entity.ColdToFrozenDir, sameEntity.ColdToFrozenDir);
                Assert.Equal(entity.ColdToFrozenScript, sameEntity.ColdToFrozenScript);
                Assert.Equal(entity.CurrentDBSizeMB, sameEntity.CurrentDBSizeMB);
                Assert.Equal(entity.DefaultDatabase, sameEntity.DefaultDatabase);
                Assert.Equal(entity.Disabled, sameEntity.Disabled);
                // Assert.Equal(entity.Eai, sameEntity.Eai); // TODO: verify this property setting (?)
                Assert.Equal(entity.EnableOnlineBucketRepair, sameEntity.EnableOnlineBucketRepair);
                Assert.Equal(entity.EnableRealtimeSearch, sameEntity.EnableRealtimeSearch);
                Assert.Equal(entity.FrozenTimePeriodInSecs, sameEntity.FrozenTimePeriodInSecs);
                Assert.Equal(entity.HomePath, sameEntity.HomePath);
                Assert.Equal(entity.HomePathExpanded, sameEntity.HomePathExpanded);
                Assert.Equal(entity.IndexThreads, sameEntity.IndexThreads);
                Assert.Equal(entity.IsInternal, sameEntity.IsInternal);
                Assert.Equal(entity.IsReady, sameEntity.IsReady);
                Assert.Equal(entity.IsVirtual, sameEntity.IsVirtual);
                Assert.Equal(entity.LastInitSequenceNumber, sameEntity.LastInitSequenceNumber);
                Assert.Equal(entity.LastInitTime, sameEntity.LastInitTime);
                Assert.Equal(entity.MaxBloomBackfillBucketAge, sameEntity.MaxBloomBackfillBucketAge);
                Assert.Equal(entity.MaxBucketSizeCacheEntries, sameEntity.MaxBucketSizeCacheEntries);
                Assert.Equal(entity.MaxConcurrentOptimizes, sameEntity.MaxConcurrentOptimizes);
                Assert.Equal(entity.MaxDataSize, sameEntity.MaxDataSize);
                Assert.Equal(entity.MaxHotBuckets, sameEntity.MaxHotBuckets);
                Assert.Equal(entity.MaxHotIdleSecs, sameEntity.MaxHotIdleSecs);
                Assert.Equal(entity.MaxHotSpanSecs, sameEntity.MaxHotSpanSecs);
                Assert.Equal(entity.MaxMemMB, sameEntity.MaxMemMB);
                Assert.Equal(entity.MaxMetaEntries, sameEntity.MaxMetaEntries);
                Assert.Equal(entity.MaxRunningProcessGroups, sameEntity.MaxRunningProcessGroups);
                Assert.Equal(entity.MaxRunningProcessGroupsLowPriority, sameEntity.MaxRunningProcessGroupsLowPriority);
                Assert.Equal(entity.MaxTime, sameEntity.MaxTime);
                Assert.Equal(entity.MaxTimeUnreplicatedNoAcks, sameEntity.MaxTimeUnreplicatedNoAcks);
                Assert.Equal(entity.MaxTimeUnreplicatedWithAcks, sameEntity.MaxTimeUnreplicatedWithAcks);
                Assert.Equal(entity.MaxTotalDataSizeMB, sameEntity.MaxTotalDataSizeMB);
                Assert.Equal(entity.MaxWarmDBCount, sameEntity.MaxWarmDBCount);
                Assert.Equal(entity.MemPoolMB, sameEntity.MemPoolMB);
                Assert.Equal(entity.MinRawFileSyncSecs, sameEntity.MinRawFileSyncSecs);
                Assert.Equal(entity.MinTime, sameEntity.MinTime);
                Assert.Equal(entity.PartialServiceMetaPeriod, sameEntity.PartialServiceMetaPeriod);
                Assert.Equal(entity.ProcessTrackerServiceInterval, sameEntity.ProcessTrackerServiceInterval);
                Assert.Equal(entity.QuarantineFutureSecs, sameEntity.QuarantineFutureSecs);
                Assert.Equal(entity.QuarantinePastSecs, sameEntity.QuarantinePastSecs);
                Assert.Equal(entity.RawChunkSizeBytes, sameEntity.RawChunkSizeBytes);
                Assert.Equal(entity.RepFactor, sameEntity.RepFactor);
                Assert.Equal(entity.RotatePeriodInSecs, sameEntity.RotatePeriodInSecs);
                Assert.Equal(entity.ServiceMetaPeriod, sameEntity.ServiceMetaPeriod);
                Assert.Equal(entity.ServiceOnlyAsNeeded, sameEntity.ServiceOnlyAsNeeded);
                Assert.Equal(entity.ServiceSubtaskTimingPeriod, sameEntity.ServiceSubtaskTimingPeriod);
                Assert.Equal(entity.SummaryHomePathExpanded, sameEntity.SummaryHomePathExpanded);
                Assert.Equal(entity.Sync, sameEntity.Sync);
                Assert.Equal(entity.SyncMeta, sameEntity.SyncMeta);
                Assert.Equal(entity.ThawedPath, sameEntity.ThawedPath);
                Assert.Equal(entity.ThawedPathExpanded, sameEntity.ThawedPathExpanded);
                Assert.Equal(entity.ThrottleCheckPeriod, sameEntity.ThrottleCheckPeriod);
                Assert.Equal(entity.TotalEventCount, sameEntity.TotalEventCount);
                Assert.Equal(entity.TStatsHomePath, sameEntity.TStatsHomePath);
                Assert.Equal(entity.TStatsHomePathExpanded, sameEntity.TStatsHomePathExpanded);
            }
        }

        #endregion

        #region Saved Searches

        [Trait("class", "Service: Saved Searches")]
        [Fact]
        public async Task CanCreateSavedSearch()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));
            await service.LoginAsync("admin", "changeme");

            try
            {
                await service.RemoveSavedSearchAsync("some_saved_search");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var attributes = new SavedSearchAttributes("search index=_internal | head 1000");
            var savedSearch = await service.CreateSavedSearchAsync("some_saved_search", attributes);
        }

        [Trait("class", "Service: Saved Searches")]
        [Fact]
        public void CanDispatchSavedSearch()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));

            Func<Task<IEnumerable<Splunk.Sdk.Result>>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");

                Job job = await service.DispatchSavedSearchAsync("Splunk errors last 24 hours");
                SearchResults searchResults = await job.GetSearchResultsAsync();

                var records = new List<Splunk.Sdk.Result>(searchResults);
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

                Assert.Equal("Errors in the last 24 hours", entity.ResourceName.Title);
                Assert.Equal(entity.Id.ToString(), entity.ToString());
                Assert.Equal("nobody", entity.Author);

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

        #endregion

        #region Search Jobs

        [Trait("class", "Service: Search Jobs")]
        [Fact]
        public async Task CanGetJob()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));
            await service.LoginAsync("admin", "changeme");
            Job job1 = null, job2 = null;

            job1 = await service.StartJobAsync("search index=_internal | head 100");
            await job1.GetSearchResultsAsync();
            await job1.GetSearchResultsEventsAsync();
            await job1.GetSearchResultsPreviewAsync();

            job2 = await service.GetJobAsync(job1.ResourceName.Title);
            Assert.Equal(job1.ResourceName.Title, job2.ResourceName.Title);
            Assert.Equal(job1.Title, job1.ResourceName.Title);
            Assert.Equal(job1.Title, job2.Title);
            Assert.Equal(job1.Sid, job1.Title);
            Assert.Equal(job1.Sid, job2.Sid);
            Assert.Equal(job1.Id, job2.Id);

            Assert.Equal(new SortedDictionary<string, Uri>().Concat(job1.Links), new SortedDictionary<string, Uri>().Concat(job2.Links));
        }

        [Trait("class", "Service: Search Jobs")]
        [Fact]
        public async Task CanGetJobs()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "admin", app: "search"));
            await service.LoginAsync("admin", "changeme");
            
            var jobs = new Job[]
            {
                await service.StartJobAsync("search index=_internal | head 1000"),
                await service.StartJobAsync("search index=_internal | head 1000"),
                await service.StartJobAsync("search index=_internal | head 1000"),
                await service.StartJobAsync("search index=_internal | head 1000"),
                await service.StartJobAsync("search index=_internal | head 1000"),
            };

            JobCollection collection = null;
            Assert.DoesNotThrow(() => collection = service.GetJobsAsync().Result);
            Assert.NotNull(collection);
            Assert.Equal(collection.ToString(), collection.Id.ToString());

            foreach (var job in jobs)
            {
                Assert.Contains(job, collection, EqualityComparer<Job>.Default);
            }
        }

        [Trait("class", "Service: Search Jobs")]
        [Fact]
        public async Task CanStartSearch()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            await service.LoginAsync("admin", "changeme");

            var job = await service.StartJobAsync("search index=_internal | head 10");
            Assert.NotNull(job);
            var results = await job.GetSearchResultsAsync();
            Assert.NotNull(results);
            var records = new List<Splunk.Sdk.Result>(results);
            Assert.Equal(10, records.Count);
        }

        [Trait("class", "Service: Search Jobs")]
        [Fact]
        public async Task CanStartSearchExport()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            await service.LoginAsync("admin", "changeme");

            SearchResultsReader reader = await service.SearchExportAsync(new SearchExportArgs("search index=_internal | head 1000") { Count = 0 });
            var records = new List<Splunk.Sdk.Result>();

            foreach (var searchResults in reader)
            {
                records.AddRange(searchResults);
            }
        }

        [Trait("class", "Service: Search Jobs")]
        [Fact]
        public void CanStartSearchOneshot()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);

            Func<Task<IEnumerable<Splunk.Sdk.Result>>> Dispatch = async () =>
            {
                await service.LoginAsync("admin", "changeme");

                SearchResults searchResults = await service.SearchOneshotAsync(new JobArgs("search index=_internal | head 100") { MaxCount = 100000 });
                var records = new List<Splunk.Sdk.Result>(searchResults);

                return records;
            };

            var result = Dispatch().Result;
        }

        #endregion

        #region System

        [Trait("class", "Service: System")]

        [Fact]
        public void CanGetServerInfo()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            var serverInfo = service.Server.GetInfoAsync().Result;

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
            service.Server.RestartAsync().Wait();
        }

        #endregion

        public void SetFixture(AcceptanceTestingSetup data)
        { }
    }
}
