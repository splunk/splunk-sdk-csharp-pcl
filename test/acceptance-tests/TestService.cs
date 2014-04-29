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
    using Splunk.Client;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web.Security;
    using Xunit;

    public class TestService : IUseFixture<AcceptanceTestingSetup>
    {
        [Trait("class", "Service")]
        [Fact]
        public void CanConstruct()
        {
            using (var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default))
            {
                Assert.Equal(service.ToString(), "https://localhost:8089/services");
            }
        }

        #region Access Control

        [Trait("class", "Service: Saved Searches")]
        [Fact]
        public async Task CanCrudStoragePasswords()
        {
            foreach (var ns in TestNamespaces)
            {
                using (var service = new Service(Scheme.Https, "localhost", 8089, ns))
                {
                    await service.LoginAsync("admin", "changeme");
                    Assert.NotNull(service.SessionKey);

                    //// Create and change the password for 50 StoragePassword instances

                    var name = string.Format("delete-me-{0}-", Guid.NewGuid().ToString("N"));
                    var realm = new string[] { null, "splunk.com", "splunk:com" };
                    var storagePasswords = new List<StoragePassword>(50);

                    for (int i = 0; i < 50; i++)
                    {
                        var storagePassword = await service.CreateStoragePasswordAsync(name + i, "foobar", realm[i % realm.Length]);
                        var password = Membership.GeneratePassword(15, 2);
                        await storagePassword.UpdateAsync(password);

                        Assert.Equal(password, storagePassword.ClearPassword);

                        storagePasswords.Add(storagePassword);
                    }

                    //// Fetch the entire collection of StoragePassword instances

                    var collection = await service.GetStoragePasswordsAsync(new StoragePasswordCollectionArgs()
                    {
                        Count = 0
                    });

                    //// Verify and then remove each of the StoragePassword instances we created

                    for (int i = 0; i < 50; i++)
                    {
                        Assert.Contains(storagePasswords[i], collection);
                        await storagePasswords[i].RemoveAsync();
                    }
                }
            }
        }

        [Trait("class", "Service: Access Control")]
        [Fact]
        public async Task CanLoginAndLogoff()
        {
            using (var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default))
            {
                await service.LoginAsync("admin", "changeme");
                Assert.NotNull(service.SessionKey);

                await service.LogoffAsync();
                Assert.Null(service.SessionKey);

                try
                {
                    await service.LoginAsync("admin", "bad-password");
                    Assert.False(true, string.Format("Expected: {0}, Actual: {1}", typeof(AuthenticationFailureException).FullName, "no exception"));
                }
                catch (AuthenticationFailureException e)
                {
                    Assert.Equal(e.StatusCode, HttpStatusCode.Unauthorized);
                    Assert.Equal(e.Details.Count, 1);
                    Assert.Equal(e.Details[0], new Message(MessageType.Warning, "Login failed"));
                }
                catch (Exception e)
                {
                    Assert.True(false, string.Format("Expected: {0}, Actual: {1}", typeof(AuthenticationFailureException).FullName, e.GetType().FullName));
                }
            }
        }

        #endregion

        #region Applications

        [Trait("class", "Service: Applications")]
        [Fact]
        public async Task CanCrudApplications()
        {
            foreach (var ns in TestNamespaces)
            {
                using (var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search")))
                {
                    await service.LoginAsync("admin", "changeme");
                    Assert.NotNull(service.SessionKey);

                    var collection = await service.GetApplicationsAsync();

                    foreach (var application in collection)
                    {
                        string value;

                        Assert.DoesNotThrow(() => value = string.Format("ApplicationAuthor = {0}", application.ApplicationAuthor));
                        Assert.DoesNotThrow(() => value = string.Format("Author = {0}", application.Author));
                        Assert.DoesNotThrow(() => value = string.Format("CheckForUpdates = {0}", application.CheckForUpdates));
                        Assert.DoesNotThrow(() => value = string.Format("Configured = {0}", application.Configured));
                        Assert.DoesNotThrow(() => value = string.Format("Description = {0}", application.Description));
                        Assert.DoesNotThrow(() => value = string.Format("Disabled = {0}", application.Disabled));
                        Assert.DoesNotThrow(() => value = string.Format("Eai = {0}", application.Eai));
                        Assert.DoesNotThrow(() => value = string.Format("Id = {0}", application.Id));
                        Assert.DoesNotThrow(() => value = string.Format("Label = {0}", application.Label));
                        Assert.DoesNotThrow(() => value = string.Format("Links = {0}", application.Links));
                        Assert.DoesNotThrow(() => value = string.Format("Name = {0}", application.Name));
                        Assert.DoesNotThrow(() => value = string.Format("Namespace = {0}", application.Namespace));
                        Assert.DoesNotThrow(() => value = string.Format("Published = {0}", application.Published));
                        Assert.DoesNotThrow(() => value = string.Format("ResourceName = {0}", application.ResourceName));
                        Assert.DoesNotThrow(() => value = string.Format("StateChangeRequiresRestart = {0}", application.StateChangeRequiresRestart));
                        Assert.DoesNotThrow(() => value = string.Format("Updated = {0}", application.Updated));
                        Assert.DoesNotThrow(() => value = string.Format("Version = {0}", application.Version));
                        Assert.DoesNotThrow(() => value = string.Format("Visible = {0}", application.Visible));

                        if (application.Name == "twitter2")
                        {
                            await application.RemoveAsync();

                            try
                            {
                                await application.GetAsync();
                                Assert.False(true, "Expected ResourceNotFoundException");
                            }
                            catch (ResourceNotFoundException)
                            { }
                        }
                    }

                    //// Install, update, and remove the Splunk App for Twitter Data, version 2.3.1

                    var path = Path.Combine(Environment.CurrentDirectory, "Data", "app-for-twitter-data_231.spl");
                    Assert.True(File.Exists(path));

                    var twitterApplication = await service.InstallApplicationAsync("twitter2", path, update: true);
                    var md5sum = "41ceb202053794cfec54b8d28f78d83c";

                    //// TODO: Check ApplicationSetupInfo and ApplicationUpdateInfo noting that we must bump the
                    //// Splunk App for Twitter Data down to, say, 2.3.0 to ensure we get update info to verify
                    //// We might check that there is no update info for 2.3.1:
                    ////    Assert.Null(twitterApplicationUpdateInfo.Update);
                    //// Then change the version number to 2.3.0:
                    ////    await twitterApplication.UpdateAsync(new ApplicationAttributes() { Version = "2.3.0" });
                    //// Finally:
                    ////    await twitterApplicationUpdateInfo.GetAsync();
                    ////    Assert.NotNull(twitterApplicationUpdateInfo.Update);
                    ////    Assert.Equal("2.3.1", twitterApplicationUpdateInfo.Version);
                    ////    Assert.Equal(md5sum, twitterApplicationUpdateInfo.Checksum);
                    ////    // other asserts on the contents of the update
                    
                    Assert.Equal("Splunk", twitterApplication.ApplicationAuthor);
                    Assert.Equal(true, twitterApplication.CheckForUpdates);
                    Assert.Equal(false, twitterApplication.Configured);
                    Assert.Equal("This application indexes Twitter's sample stream.", twitterApplication.Description);
                    Assert.Equal("Splunk-Twitter Connector", twitterApplication.Label);
                    Assert.Equal(false, twitterApplication.Refresh);
                    Assert.Equal(false, twitterApplication.StateChangeRequiresRestart);
                    Assert.Equal("2.3.1", twitterApplication.Version);
                    Assert.Equal(true, twitterApplication.Visible);

                    await twitterApplication.GetAsync();
                    var twitterApplicationSetupInfo = await twitterApplication.GetSetupInfoAsync();
                    var twitterApplicationUpdateInfo = await twitterApplication.GetUpdateInfoAsync();

                    await twitterApplication.RemoveAsync();

                    try
                    {
                        await twitterApplication.GetAsync();
                        Assert.False(true, "Expected ResourceNotFoundException");
                    }
                    catch (ResourceNotFoundException)
                    { }

                    //// Create an app from one of the built-in templates

                    var name = string.Format("delete-me-{0:N}", Guid.NewGuid());

                    var attributes = new ApplicationAttributes()
                    {
                        ApplicationAuthor = "Splunk",
                        Configured = true,
                        Description = "This app confirms that an app can be created from a template",
                        Label = name,
                        Version = "2.0.0",
                        Visible = true
                    };

                    var templatedApplication = await service.CreateApplicationAsync(name, "barebones", attributes);

                    Assert.Equal(attributes.ApplicationAuthor, templatedApplication.ApplicationAuthor);
                    Assert.Equal(true, templatedApplication.CheckForUpdates);
                    Assert.Equal(attributes.Configured, templatedApplication.Configured);
                    Assert.Equal(attributes.Description, templatedApplication.Description);
                    Assert.Equal(attributes.Label, templatedApplication.Label);
                    Assert.Equal(false, templatedApplication.Refresh);
                    Assert.Equal(false, templatedApplication.StateChangeRequiresRestart);
                    Assert.Equal(attributes.Version, templatedApplication.Version);
                    Assert.Equal(attributes.Visible, templatedApplication.Visible);

                    await templatedApplication.RemoveAsync();
                }
            }
        }

        #endregion

        #region Configuration

        [Trait("class", "Service: Configuration")]
        [Fact]
        public async Task CanCrudConfiguration() // no delete operation is available
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));
            await service.LoginAsync("admin", "changeme");

            var fileName = string.Format("delete-me-{0:N}", Guid.NewGuid());

            //// Create

            var configuration = await service.CreateConfigurationAsync(fileName);

            //// Read

            configuration = await service.GetConfigurationAsync(fileName);

            //// Update the default stanza through a ConfigurationStanza object

            var defaultStanza = await configuration.UpdateStanzaAsync("default", new Argument("foo", "1"), new Argument("bar", "2"));
            await defaultStanza.UpdateAsync(new Argument("bar", "3"), new Argument("foobar", "4"));
            await defaultStanza.UpdateSettingAsync("foobar", "5");

            await defaultStanza.GetAsync(); // because the rest api does not return settings unless you ask for them
            Assert.Equal(3, defaultStanza.Count);
            List<ConfigurationSetting> settings;

            settings = defaultStanza.Select(setting => setting).Where(setting => setting.Name == "foo").ToList();
            Assert.Equal(1, settings.Count);
            Assert.Equal("1", settings[0].Value);

            settings = defaultStanza.Select(setting => setting).Where(setting => setting.Name == "bar").ToList();
            Assert.Equal(1, settings.Count);
            Assert.Equal("3", settings[0].Value);

            settings = defaultStanza.Select(setting => setting).Where(setting => setting.Name == "foobar").ToList();
            Assert.Equal(1, settings.Count);
            Assert.Equal("5", settings[0].Value);

            //// Create, read, update, and delete a stanza through the Service object

            await service.CreateConfigurationStanzaAsync(fileName, "stanza");

            await service.UpdateConfigurationSettingsAsync(fileName, "stanza", new Argument("foo", "1"), new Argument("bar", "2"));
            await service.UpdateConfigurationSettingAsync(fileName, "stanza", "bar", "3");

            var stanza = await service.GetConfigurationStanzaAsync(fileName, "stanza");

            settings = stanza.Select(setting => setting).Where(setting => setting.Name == "foo").ToList();
            Assert.Equal(1, settings.Count);
            Assert.Equal("1", settings[0].Value);

            settings = stanza.Select(setting => setting).Where(setting => setting.Name == "bar").ToList();
            Assert.Equal(1, settings.Count);
            Assert.Equal("3", settings[0].Value);

            await service.RemoveConfigurationStanzaAsync(fileName, "stanza");
        }

        [Trait("class", "Service: Configuration")]
        [Fact]
        public async Task CanGetConfigurations()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));
            await service.LoginAsync("admin", "changeme");

            var collection = await service.GetConfigurationsAsync();
        }

        [Trait("class", "Service: Configuration")]
        [Fact]
        public async Task CanReadConfigurations()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));
            await service.LoginAsync("admin", "changeme");

            //// Read the entire configuration system

            var configurations = await service.GetConfigurationsAsync();

            foreach (var configuration in configurations)
            {
                await configuration.GetAsync();

                foreach (ConfigurationStanza stanza in configuration)
                {
                    Assert.NotNull(stanza);
                    await stanza.GetAsync();
                }
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

        [Trait("class", "Service: Indexes")]
        [Fact]
        public async Task CanCrudIndex()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));
            await service.LoginAsync("admin", "changeme");

            var indexName = string.Format("delete-me-{0:N}", Guid.NewGuid());
            Index index;

            // Create

            index = await service.CreateIndexAsync(indexName, new IndexArgs());
            Assert.Equal(true, index.EnableOnlineBucketRepair);

            // Read

            index = await service.GetIndexAsync(indexName);

            // Update

            var indexAttributes = new IndexAttributes()
            {
                EnableOnlineBucketRepair = false
            };

            await index.UpdateAsync(indexAttributes);
            Assert.Equal(false, index.EnableOnlineBucketRepair);

            // Delete

            await service.RemoveIndexAsync(indexName);
        }

        #endregion

        #region Saved Searches

        [Trait("class", "Service: Saved Searches")]
        [Fact]
        public async Task CanCrudSavedSearch()
        {
            using (var service = new Service(Scheme.Https, "localhost", 8089/*, new Namespace(user: "nobody", app: "search")*/))
            {
                await service.LoginAsync("admin", "changeme");

                //// Create

                var name = string.Format("delete-me-{0:N}", Guid.NewGuid());

                var originalAttributes = new SavedSearchAttributes()
                {
                    Search = "search index=_internal | head 1000",
                    CronSchedule = "00 * * * *", // on the hour
                    IsScheduled = true,
                    IsVisible = false
                };

                var savedSearch = await service.CreateSavedSearchAsync(name, originalAttributes);

                Assert.Equal(originalAttributes.Search, savedSearch.Search);
                Assert.Equal(originalAttributes.CronSchedule, savedSearch.CronSchedule);
                Assert.Equal(originalAttributes.IsScheduled, savedSearch.IsScheduled);
                Assert.Equal(originalAttributes.IsVisible, savedSearch.IsVisible);

                //// Read

                savedSearch = await service.GetSavedSearchAsync(name);
                Assert.Equal(false, savedSearch.IsVisible);

                //// Read schedule

                var dateTime = DateTime.Now;
                var schedule = await savedSearch.GetScheduledTimesAsync(dateTime, dateTime.AddDays(2));

                Assert.Equal(48, schedule.Count);

                var expected = dateTime.AddMinutes(60);
                expected = expected.Date.AddHours(expected.Hour);

                Assert.Equal(expected, schedule[0]);

                //// Update

                var updatedAttributes = new SavedSearchAttributes()
                {
                    ActionEmailBcc = "user1@splunk.com",
                    ActionEmailCC = "user2@splunk.com",
                    ActionEmailFrom = "user3@splunk.com",
                    ActionEmailTo = "user4@splunk.com, user5@splunk.com",
                    IsVisible = true
                };

                savedSearch = await service.UpdateSavedSearchAsync(name, updatedAttributes);

                Assert.Equal(updatedAttributes.ActionEmailBcc, savedSearch.Actions.Email.Bcc);
                Assert.Equal(updatedAttributes.ActionEmailCC, savedSearch.Actions.Email.CC);
                Assert.Equal(updatedAttributes.ActionEmailFrom, savedSearch.Actions.Email.From);
                Assert.Equal(updatedAttributes.ActionEmailTo, savedSearch.Actions.Email.To);
                Assert.Equal(updatedAttributes.IsVisible, savedSearch.IsVisible);

                //// Update schedule

                dateTime = DateTime.Now;

                //// TODO: 
                //// Figure out why POST saved/searches/{name}/reschedule ignores schedule_time and runs the
                //// saved searches right away. Are we using the right time format?

                //// TODO: 
                //// Figure out how to parse or--more likely--complain that savedSearch.NextScheduledTime uses
                //// timezone names like "Pacific Daylight Time".

                await savedSearch.ScheduleAsync(dateTime.AddMinutes(15)); // Does not return anything but status
                await savedSearch.GetScheduledTimesAsync(dateTime, dateTime.AddDays(2));

                //// Delete

                await savedSearch.RemoveAsync();
            }
        }

        [Trait("class", "Service: Saved Searches")]
        [Fact]
        public async Task CanDispatchSavedSearch()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));
            await service.LoginAsync("admin", "changeme");

            Job job = await service.DispatchSavedSearchAsync("Splunk errors last 24 hours");
            SearchResults searchResults = await job.GetSearchResultsAsync();

            var records = new List<Splunk.Client.Result>(searchResults);
        }

        [Trait("class", "Service: Saved Searches")]
        [Fact]
        public async Task CanGetSavedSearchHistory()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));
            await service.LoginAsync("admin", "changeme");

            var attributes = new SavedSearchAttributes() { Search = "search index=_internal * earliest=-1m" };
            var name = string.Format("delete-me-{0:N}", Guid.NewGuid());
            var savedSearch = await service.CreateSavedSearchAsync(name, attributes);

            var jobHistory = await savedSearch.GetHistoryAsync();
            Assert.Equal(0, jobHistory.Count);

            Job job1 = await savedSearch.DispatchAsync();

            jobHistory = await savedSearch.GetHistoryAsync();
            Assert.Equal(1, jobHistory.Count);
            Assert.Equal(job1.Id, jobHistory[0].Id);
            Assert.Equal(job1.Name, jobHistory[0].Name);
            Assert.Equal(job1.Namespace, jobHistory[0].Namespace);
            Assert.Equal(job1.ResourceName, jobHistory[0].ResourceName);

            Job job2 = await savedSearch.DispatchAsync();

            jobHistory = await savedSearch.GetHistoryAsync();
            Assert.Equal(2, jobHistory.Count);
            Assert.Equal(1, jobHistory.Select(job => job).Where(job => job.Id.Equals(job1.Id)).Count());
            Assert.Equal(1, jobHistory.Select(job => job).Where(job => job.Id.Equals(job2.Id)).Count());

            await job1.CancelAsync();

            jobHistory = await savedSearch.GetHistoryAsync();
            Assert.Equal(1, jobHistory.Count);
            Assert.Equal(job2.Id, jobHistory[0].Id);

            await job2.CancelAsync();

            jobHistory = await savedSearch.GetHistoryAsync();
            Assert.Equal(0, jobHistory.Count);

            await savedSearch.RemoveAsync();
        }

        [Trait("class", "Service: Saved Searches")]
        [Fact]
        public async Task CanGetSavedSearches()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));
            await service.LoginAsync("admin", "changeme");

            var collection = await service.GetSavedSearchesAsync();
        }

        [Trait("class", "Service: Saved Searches")]
        [Fact]
        public async Task CanUpdateSavedSearch()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, new Namespace(user: "nobody", app: "search"));
            await service.LoginAsync("admin", "changeme");

            await service.UpdateSavedSearchAsync("Errors in the last 24 hours", new SavedSearchAttributes() { IsVisible = false });
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
            Assert.Equal(job1.Name, job1.ResourceName.Title);
            Assert.Equal(job1.Name, job2.Name);
            Assert.Equal(job1.Sid, job1.Name);
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

            Assert.Equal<IEnumerable<string>>(new List<string> 
                { 
                    "_bkt",
                    "_cd",
                    "_indextime",
                    "_raw",
                    "_serial",
                    "_si",
                    "_sourcetype",
                    "_subsecond",
                    "_time",
                    "host",
                    "index",
                    "linecount",
                    "source",
                    "sourcetype",
                    "splunk_server",
                 },
                 results.FieldNames);

            var records = new List<Result>(results);
            Assert.Equal(10, records.Count);
        }

        [Trait("class", "Service: Search Jobs")]
        [Fact]
        public async Task CanStartSearchExport()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            await service.LoginAsync("admin", "changeme");

            SearchResultsReader reader = await service.SearchExportAsync(new SearchExportArgs("search index=_internal | head 1000") { Count = 0 });
            var records = new List<Splunk.Client.Result>();

            foreach (var results in reader)
            {
                Assert.Equal<IEnumerable<string>>(new List<string> 
                    { 
                        "_bkt",
                        "_cd",
                        "_indextime",
                        "_raw",
                        "_serial",
                        "_si",
                        "_sourcetype",
                        "_subsecond",
                        "_time",
                        "host",
                        "index",
                        "linecount",
                        "source",
                        "sourcetype",
                        "splunk_server",
                    },
                    results.FieldNames);

                records.AddRange(results);
            }

            Assert.Equal(1000, records.Count);
        }

        [Trait("class", "Service: Search Jobs")]
        [Fact]
        public async Task CanStartSearchOneshot()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            await service.LoginAsync("admin", "changeme");

            SearchResults searchResults = await service.SearchOneshotAsync(new JobArgs("search index=_internal | head 100") { MaxCount = 100000 });
            var records = new List<Splunk.Client.Result>(searchResults);
        }

        #endregion

        #region System

        [Trait("class", "Service: Server")]
        [Fact]
        public async Task CanCrudServerMessages()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            await service.LoginAsync("admin", "changeme");

            //// Create

            var name = string.Format("delete-me-{0:N}", Guid.NewGuid());

            var messages = new ServerMessage[]
            {
                await service.Server.CreateMessageAsync(string.Format("{0}-{1}", name, ServerMessageSeverity.Information), ServerMessageSeverity.Information, "some message text"),
                await service.Server.CreateMessageAsync(string.Format("{0}-{1}", name, ServerMessageSeverity.Warning), ServerMessageSeverity.Warning, "some message text"),
                await service.Server.CreateMessageAsync(string.Format("{0}-{1}", name, ServerMessageSeverity.Error), ServerMessageSeverity.Error, "some message text"),
            };

            //// Read

            var messageCollection = await service.Server.GetMessagesAsync();

            foreach (var message in messages)
            {
                var messageCopy = await service.Server.GetMessageAsync(message.Name);
                Assert.Contains<ServerMessage>(message, messageCollection);
                await message.GetAsync();
            }

            //// Delete (there is no update)

            foreach (var message in messageCollection)
            {
                if (message.Name.StartsWith("delete-me-"))
                {
                    await message.RemoveAsync();
                }
            }

            //// Verify delete

            await messageCollection.GetAsync();

            foreach (var message in messageCollection)
            {
                Assert.False(message.Name.StartsWith("delete-me-"));
            }
        }

        [Trait("class", "Service: Server")]
        [Fact]
        public async Task CanCrudServerSettings()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            await service.LoginAsync("admin", "changeme");

            //// Get

            var originalSettings = await service.Server.GetSettingsAsync();

            //// Update

            var values = new ServerSettingValues()
            {
                EnableSplunkWebSsl = !originalSettings.EnableSplunkWebSsl,
                Host = originalSettings.Host,
                HttpPort = originalSettings.HttpPort + 1,
                ManagementHostPort = originalSettings.ManagementHostPort,
                MinFreeSpace = originalSettings.MinFreeSpace - 1,
                Pass4SymmetricKey = originalSettings.Pass4SymmetricKey + "-update",
                ServerName = originalSettings.ServerName,
                SessionTimeout = "2h",
                SplunkDB = originalSettings.SplunkDB,
                StartWebServer = !originalSettings.StartWebServer,
                TrustedIP = originalSettings.TrustedIP
            };

            var updatedSettings = await service.Server.UpdateSettingsAsync(values);

            Assert.Equal(values.EnableSplunkWebSsl, updatedSettings.EnableSplunkWebSsl);
            Assert.Equal(values.Host, updatedSettings.Host);
            Assert.Equal(values.HttpPort, updatedSettings.HttpPort);
            Assert.Equal(values.ManagementHostPort, updatedSettings.ManagementHostPort);
            Assert.Equal(values.MinFreeSpace, updatedSettings.MinFreeSpace);
            Assert.Equal(values.Pass4SymmetricKey, updatedSettings.Pass4SymmetricKey);
            Assert.Equal(values.ServerName, updatedSettings.ServerName);
            Assert.Equal(values.SessionTimeout, updatedSettings.SessionTimeout);
            Assert.Equal(values.SplunkDB, updatedSettings.SplunkDB);
            Assert.Equal(values.StartWebServer, updatedSettings.StartWebServer);
            Assert.Equal(values.TrustedIP, updatedSettings.TrustedIP);

            //// Restart the server because it's required following a settings update

            await service.Server.RestartAsync(60000);
            await service.LoginAsync("admin", "changeme");

            //// Restore

            values = new ServerSettingValues()
            {
                EnableSplunkWebSsl = originalSettings.EnableSplunkWebSsl,
                Host = originalSettings.Host,
                HttpPort = originalSettings.HttpPort,
                ManagementHostPort = originalSettings.ManagementHostPort,
                MinFreeSpace = originalSettings.MinFreeSpace,
                Pass4SymmetricKey = originalSettings.Pass4SymmetricKey,
                ServerName = originalSettings.ServerName,
                SessionTimeout = originalSettings.SessionTimeout,
                SplunkDB = originalSettings.SplunkDB,
                StartWebServer = originalSettings.StartWebServer,
                TrustedIP = originalSettings.TrustedIP
            };

            updatedSettings = await service.Server.UpdateSettingsAsync(values);

            Assert.Equal(values.EnableSplunkWebSsl, originalSettings.EnableSplunkWebSsl);
            Assert.Equal(values.Host, originalSettings.Host);
            Assert.Equal(values.HttpPort, originalSettings.HttpPort);
            Assert.Equal(values.ManagementHostPort, originalSettings.ManagementHostPort);
            Assert.Equal(values.MinFreeSpace, originalSettings.MinFreeSpace);
            Assert.Equal(values.Pass4SymmetricKey, originalSettings.Pass4SymmetricKey);
            Assert.Equal(values.ServerName, originalSettings.ServerName);
            Assert.Equal(values.SessionTimeout, originalSettings.SessionTimeout);
            Assert.Equal(values.SplunkDB, originalSettings.SplunkDB);
            Assert.Equal(values.StartWebServer, originalSettings.StartWebServer);
            Assert.Equal(values.TrustedIP, originalSettings.TrustedIP);

            //// Restart the server because it's required following a settings update

            await service.Server.RestartAsync(60000);
        }

        [Trait("class", "Service: System")]
        [Fact]
        public async Task CanGetServerInfo()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            var serverInfo = await service.Server.GetInfoAsync();

            EaiAcl acl = serverInfo.Eai.Acl;
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
        public async Task CanRestartServer()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            await service.LoginAsync("admin", "changeme");
            await service.Server.RestartAsync(millisecondsDelay: 60000);
            Assert.Null(service.SessionKey);
            await service.LoginAsync("admin", "changeme");
        }

        [Trait("class", "Service: System")]
        [Fact]
        public async Task CanSendEvents()
        {
            var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);
            await service.LoginAsync("admin", "changeme");

            var receiver = service.Receiver;

            for (int i = 0; i < 10; i++)
            {
                var result = await receiver.SendAsync(string.Format("{0:D6} {1} Hello world!", i, DateTime.Now));
            }

            using (var eventStream = new MemoryStream())
            {
                var writer = new StreamWriter(eventStream);

                for (int i = 0; i < 10; i++)
                {
                    writer.Write(string.Format("{0:D6} {1} Goodbye world!\r\n", i, DateTime.Now));
                }

                var task = receiver.SendAsync(eventStream);
                task.Wait();
            }
        }

        #endregion

        public void SetFixture(AcceptanceTestingSetup data)
        { }

        #region Privates/internals

        static readonly IReadOnlyList<Namespace> TestNamespaces = new Namespace[] 
        { 
            Namespace.Default, new Namespace("admin", "search"), new Namespace("nobody", "search") 
        };

        #endregion
    }
}
