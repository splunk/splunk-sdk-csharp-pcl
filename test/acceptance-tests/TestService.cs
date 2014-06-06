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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reactive.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Security;
    
    using Xunit;

    public class TestService : IUseFixture<AcceptanceTestingSetup>
    {
        [Trait("acceptance-test", "Splunk.Client.Service")]
        [Fact]
        public void CanConstructService()
        {
            using (var service = new Service(SDKHelper.UserConfigure.scheme, SDKHelper.UserConfigure.host, SDKHelper.UserConfigure.port, Namespace.Default))
            {
                Assert.Equal(service.ToString(), string.Format("{0}://{1}:{2}/services", SDKHelper.UserConfigure.scheme, SDKHelper.UserConfigure.host, SDKHelper.UserConfigure.port).ToLower());
            }
        }

        #region Access Control

        [Trait("acceptance-test", "Splunk.Client.Service: Access control")]
        [Fact]
        public async Task  CanCrudStoragePasswords()
        {
            foreach (var ns in TestNamespaces)
            {
                using (var service = await SDKHelper.CreateService(ns))
                {
                    StoragePasswordCollection sps = service.StoragePasswords;
                    await sps.GetAllAsync();

                    foreach (StoragePassword sp in sps)
                    {
                        if (sp.Username.Contains("delete-me-"))
                        {
                            await sp.RemoveAsync();
                        }
                    }

                    //// Create and change the password for 50 StoragePassword instances

                    var name = string.Format("delete-me-{0}-", Guid.NewGuid().ToString("N"));
                    var realm = new string[] { null, "splunk.com", "splunk:com" };
                    var storagePasswords = new List<StoragePassword>(50);

                    for (int i = 0; i < 50; i++)
                    {
                        StoragePassword storagePassword = await sps.CreateAsync(name + i, "foobar", realm[i % realm.Length]);
                        Console.WriteLine("print:"+storagePassword.Name + "," + storagePassword.Realm);
                        var password = Membership.GeneratePassword(15, 2);
                        Console.WriteLine("password:" + password);
                        await storagePassword.UpdateAsync(password);

                        Assert.Equal(password, storagePassword.ClearPassword);
                        storagePasswords.Add(storagePassword);
                    }

                    //// Fetch the entire collection of StoragePassword instances

                    await sps.GetAllAsync();

                    //// Verify and then remove each of the StoragePassword instances we created

                    for (int i = 0; i < 50; i++)
                    {
                        Assert.Contains(storagePasswords[i], sps);
                        await storagePasswords[i].RemoveAsync();
                    }
                }
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service: Access Control")]
        [Fact]
        public async Task  CanLoginAndLogoff()
        {
            using (var service = await SDKHelper.CreateService(Namespace.Default))
            {
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

        [Trait("acceptance-test", "Splunk.Client.Service: Applications")]
        [Fact]
        public async Task  CanCrudApplications()
        {
            foreach (var ns in TestNamespaces)
            {
                using (var service = await SDKHelper.CreateService(ns))
                {
                    var args = new ApplicationCollection.Filter()
                    {
                        Offset = 0,
                        Count = 10
                    };

                    do
                    {
                        await service.Applications.GetSliceAsync(args);
                        await service.Applications.ReloadAsync();

                        foreach (var application in service.Applications)
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
                            //Assert.DoesNotThrow(() => value = string.Format("Published = {0}", application.Published));
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

                        args.Offset += service.Applications.Pagination.ItemsPerPage;
                    }
                    while (args.Offset < service.Applications.Pagination.TotalResults);

                    //// Install, update, and remove the Splunk App for Twitter Data, version 2.3.1

                    IPHostEntry splunkHostEntry = await Dns.GetHostEntryAsync(service.Context.Host);
                    IPHostEntry localHostEntry = await Dns.GetHostEntryAsync("localhost");

                    if (splunkHostEntry.HostName == localHostEntry.HostName)
                    {
                        var path = Path.Combine(Environment.CurrentDirectory, "Data", "app-for-twitter-data_230.spl");
                        Assert.True(File.Exists(path));

                        var twitterApplication = await service.Applications.InstallAsync(path, update: true);

                        //// Other asserts on the contents of the update

                        Assert.Equal("Splunk", twitterApplication.ApplicationAuthor);
                        Assert.Equal(true, twitterApplication.CheckForUpdates);
                        Assert.Equal(false, twitterApplication.Configured);
                        Assert.Equal("This application indexes Twitter's sample stream.", twitterApplication.Description);
                        Assert.Equal("Splunk-Twitter Connector", twitterApplication.Label);
                        Assert.Equal(false, twitterApplication.Refresh);
                        Assert.Equal(false, twitterApplication.StateChangeRequiresRestart);
                        Assert.Equal("2.3.0", twitterApplication.Version);
                        Assert.Equal(true, twitterApplication.Visible);

                        //// TODO: Check ApplicationSetupInfo and ApplicationUpdateInfo noting that we must bump the
                        //// Splunk App for Twitter Data down to, say, 2.3.0 to ensure we get update info to verify
                        //// We might check that there is no update info for 2.3.1:
                        ////    Assert.Null(twitterApplicationUpdateInfo.Update);
                        //// Then change the version number to 2.3.0:
                        ////    await twitterApplication.UpdateAsync(new ApplicationAttributes() { Version = "2.3.0" });
                        //// Finally:
                        //// ApplicationUpdateInfo twitterApplicationUpdateInfo = await twitterApplication.GetUpdateInfoAsync();
                        //// Assert.NotNull(twitterApplicationUpdateInfo.Update);
                        //// Assert.True(string.Compare(twitterApplicationUpdateInfo.Update.Version, "2.3.0") == 1, "expect the newer twitter app info");
                        //// Assert.Equal("41ceb202053794cfec54b8d28f78d83c", twitterApplicationUpdateInfo.Update.Checksum);

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
                    }

                    //// Create an app from one of the built-in templates

                    var name = string.Format("delete-me-{0}", Guid.NewGuid());

                    var creationAttributes = new ApplicationAttributes()
                    {
                        ApplicationAuthor = "Splunk",
                        Configured = true,
                        Description = "This app confirms that an app can be created from a template",
                        Label = name,
                        Version = "2.0.0",
                        Visible = true
                    };

                    var templatedApplication = await service.Applications.CreateAsync(name, "barebones", creationAttributes);

                    Assert.Equal(creationAttributes.ApplicationAuthor, templatedApplication.ApplicationAuthor);
                    Assert.Equal(true, templatedApplication.CheckForUpdates);
                    Assert.Equal(creationAttributes.Configured, templatedApplication.Configured);
                    Assert.Equal(creationAttributes.Description, templatedApplication.Description);
                    Assert.Equal(creationAttributes.Label, templatedApplication.Label);
                    Assert.Equal(false, templatedApplication.Refresh);
                    Assert.Equal(false, templatedApplication.StateChangeRequiresRestart);
                    Assert.Equal(creationAttributes.Version, templatedApplication.Version);
                    Assert.Equal(creationAttributes.Visible, templatedApplication.Visible);

                    var updateAttributes = new ApplicationAttributes()
                    {
                        ApplicationAuthor = "Splunk, Inc.",
                        Configured = true,
                        Description = "This app update confirms that an app can be updated from a template",
                        Label = name,
                        Version = "2.0.1",
                        Visible = true
                    };

                    await templatedApplication.UpdateAsync(updateAttributes, checkForUpdates: true);
                    await templatedApplication.GetAsync(); // Because UpdateAsync doesn't return the updated entity

                    Assert.Equal(updateAttributes.ApplicationAuthor, templatedApplication.ApplicationAuthor);
                    Assert.Equal(true, templatedApplication.CheckForUpdates);
                    Assert.Equal(updateAttributes.Configured, templatedApplication.Configured);
                    Assert.Equal(updateAttributes.Description, templatedApplication.Description);
                    Assert.Equal(updateAttributes.Label, templatedApplication.Label);
                    Assert.Equal(false, templatedApplication.Refresh);
                    Assert.Equal(false, templatedApplication.StateChangeRequiresRestart);
                    Assert.Equal(updateAttributes.Version, templatedApplication.Version);
                    Assert.Equal(updateAttributes.Visible, templatedApplication.Visible);

                    Assert.False(templatedApplication.Disabled);

                    await templatedApplication.DisableAsync();
                    await templatedApplication.GetAsync(); // because POST apps/local/{name} does not return new data
                    Assert.True(templatedApplication.Disabled);

                    await templatedApplication.EnableAsync();
                    await templatedApplication.GetAsync(); // because POST apps/local/{name} does not return new data
                    Assert.False(templatedApplication.Disabled);

                    var templatedApplicationArchiveInfo = await templatedApplication.PackageAsync();

                    if (splunkHostEntry.HostName == localHostEntry.HostName)
                    {
                        Assert.True(File.Exists(templatedApplicationArchiveInfo.Path));
                        File.Delete(templatedApplicationArchiveInfo.Path);
                    }

                    await templatedApplication.RemoveAsync();
                }
            }
        }

        #endregion

        #region Configuration

        [Trait("acceptance-test", "Splunk.Client.Service: Configuration")]
        [Fact]
        public async Task  CanCrudConfiguration() // no delete operation is available
        {
            using (var service = await SDKHelper.CreateService())
            {
                var fileName = string.Format("delete-me-{0}", Guid.NewGuid());

                //// Create

                var configuration = await service.Configurations.CreateAsync(fileName);

                //// Read

                configuration = await service.Configurations.GetAsync(fileName);

                //// Update the default stanza through a ConfigurationStanza object

                var defaultStanza = await configuration.UpdateAsync("default", new Argument("foo", 1), new Argument("bar", 2));
                await defaultStanza.UpdateAsync(new Argument("bar", 3), new Argument("foobar", 4));
                await defaultStanza.UpdateAsync("non_existent_setting", "some_value");

                await defaultStanza.GetAsync(); // because the rest api does not return settings unless you ask for them
                Assert.Equal(4, defaultStanza.Count);

                ConfigurationSetting setting;

                setting = defaultStanza.SingleOrDefault(s => s.Title == "foo");
                Assert.NotNull(setting);
                Assert.Equal("1", setting.Value);

                setting = defaultStanza.SingleOrDefault(s => s.Title == "bar");
                Assert.NotNull(setting);
                Assert.Equal("3", setting.Value);

                setting = defaultStanza.SingleOrDefault(s => s.Title == "foobar");
                Assert.NotNull(setting);
                Assert.Equal("4", setting.Value);

                setting = defaultStanza.SingleOrDefault(s => s.Title == "non_existent_setting");
                Assert.NotNull(setting);
                Assert.Equal("some_value", setting.Value);

                //// Create, read, update, and delete a stanza through the Service object

                ConfigurationStanza configurationStanza = await configuration.CreateAsync("stanza");
                Assert.Equal(0, configurationStanza.Count);

                bool isUpdatedSnapshot = await configurationStanza.UpdateAsync(new Argument("foo", 5), new Argument("bar", 6));
                Assert.False(isUpdatedSnapshot);
                Assert.Equal(0, configurationStanza.Count);

                await configurationStanza.GetAsync();
                Assert.Equal(4, configurationStanza.Count); // because all stanzas inherit from the default stanza

                setting = configurationStanza.SingleOrDefault(s => s.Title == "foo");
                Assert.NotNull(setting);
                Assert.Equal("5", setting.Value);

                setting = configurationStanza.SingleOrDefault(s => s.Title == "bar");
                Assert.NotNull(setting);
                Assert.Equal("6", setting.Value);

                await configurationStanza.RemoveAsync();

                try
                {
                    await configurationStanza.GetAsync();
                    Assert.True(false);
                }
                catch (ResourceNotFoundException)
                { }

                configurationStanza = await configuration.GetOrNullAsync("stanza");
                Assert.Null(configurationStanza);
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service: Configuration")]
        [Fact]
        public async Task  CanGetConfigurations()
        {
            using (var service = await SDKHelper.CreateService())
            {
                await service.Configurations.GetAllAsync();
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service: Configuration")]
        [Fact]
        public async Task  CanReadConfigurations()
        {
            using (var service = await SDKHelper.CreateService())
            {
                //// Read the entire configuration system

                await service.Configurations.GetAllAsync();

                foreach (var configuration in service.Configurations)
                {
                    await configuration.GetAllAsync();

                    foreach (ConfigurationStanza stanza in configuration)
                    {
                        Assert.NotNull(stanza);
                        Console.WriteLine("stanza.Name:"+stanza.Name);
                        Console.WriteLine("stanza.Namespace:" + stanza.Namespace);
                        await stanza.GetAsync();
                    }
                }
            }
        }

        #endregion

        #region Indexes

        [Trait("acceptance-test", "Splunk.Client.Service: Indexes")]
        [Fact]
        public async Task  CanGetIndexes()
        {
            using (var service = await SDKHelper.CreateService(new Namespace(user: "nobody", app: "search")))
            {
                await service.Indexes.GetAllAsync();

                foreach (var entity in service.Indexes)
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
                    Assert.DoesNotThrow(() => { long value = entity.CurrentDBSizeMB; });
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

                    var sameEntity = await service.Indexes.GetAsync(entity.ResourceName.Title);

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
                    Assert.True((sameEntity.MaxTime - entity.MaxTime)<=new TimeSpan(0,0,20));//expect the test finish run within 20s which means new events comes only within 20s
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
        }

        [Trait("acceptance-test", "Splunk.Client.Service: Indexes")]
        [Fact]
        public async Task  CanCrudIndex()
        {
            using (var service = await SDKHelper.CreateService(new Namespace(user: "nobody", app: "search")))
            {
                var indexName = string.Format("delete-me-{0}", Guid.NewGuid());
                Index index;
                
                //// Create

                index = await service.Indexes.CreateAsync(indexName);
                Assert.Equal(true, index.EnableOnlineBucketRepair);

                //// Read

                index = await service.Indexes.GetAsync(indexName);

                //// Update

                var attributes = new IndexAttributes()
                {
                    EnableOnlineBucketRepair = false
                };

                await index.UpdateAsync(attributes);
                Assert.Equal(attributes.EnableOnlineBucketRepair, index.EnableOnlineBucketRepair);
                Assert.False(index.Disabled);

                await index.DisableAsync();
                Assert.True(index.Disabled);

                await index.EnableAsync();
                Assert.False(index.Disabled);

                //// Delete

                await index.RemoveAsync();

                try
                {
                    await index.GetAsync();
                    Assert.True(false);
                }
                catch (ResourceNotFoundException)
                { }

                index = await service.Indexes.GetOrNullAsync(indexName);
                Assert.Null(index);
            }
        }

        #endregion

        #region Saved Searches

        [Trait("acceptance-test", "Splunk.Client.Service: Saved Searches")]
        [Fact]
        public async Task  CanCrudSavedSearch()
        {
            using (var service = await SDKHelper.CreateService())
            {
                //// Create

                var name = string.Format("delete-me-{0}", Guid.NewGuid());
                var search = "search index=_internal | head 1000";

                var originalAttributes = new SavedSearchAttributes()
                {
                    CronSchedule = "00 * * * *", // on the hour
                    IsScheduled = true,
                    IsVisible = false
                };

                var savedSearch = await service.SavedSearches.CreateAsync(name, search, originalAttributes);

                Assert.Equal(search, savedSearch.Search);
                Assert.Equal(originalAttributes.CronSchedule, savedSearch.CronSchedule);
                Assert.Equal(originalAttributes.IsScheduled, savedSearch.IsScheduled);
                Assert.Equal(originalAttributes.IsVisible, savedSearch.IsVisible);

                //// Read

                savedSearch = await service.SavedSearches.GetAsync(name);
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

                await savedSearch.UpdateAsync(updatedAttributes);

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

        [Trait("acceptance-test", "Splunk.Client.Service: Saved Searches")]
        [Fact]
        public async Task  CanDispatchSavedSearch()
        {
            using (var service = await SDKHelper.CreateService())
            {
                Job job = await service.DispatchSavedSearchAsync("Splunk errors last 24 hours");
                SearchResultStream searchResults = await job.GetSearchResultsAsync();

                var records = new List<Splunk.Client.SearchResult>(searchResults.ToEnumerable());
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service: Saved Searches")]
        [Fact]
        public async Task  CanGetSavedSearchHistory()
        {
            using (var service = await SDKHelper.CreateService())
            {
                var name = string.Format("delete-me-{0}", Guid.NewGuid());
                var search = "search index=_internal * earliest=-1m";
                var savedSearch = await service.SavedSearches.CreateAsync(name, search);

                var jobHistory = await savedSearch.GetHistoryAsync();
                Assert.Equal(0, jobHistory.Count);

                Job job1 = await savedSearch.DispatchAsync();

                jobHistory = await savedSearch.GetHistoryAsync();
                Assert.Equal(1, jobHistory.Count);
                Assert.Equal(job1, jobHistory[0]);
                Assert.Equal(job1.Name, jobHistory[0].Name);
                Assert.Equal(job1.ResourceName, jobHistory[0].ResourceName);
                Assert.Equal(job1.Sid, jobHistory[0].Sid);

                Job job2 = await savedSearch.DispatchAsync();

                jobHistory = await savedSearch.GetHistoryAsync();
                Assert.Equal(2, jobHistory.Count);
                Assert.Equal(1, jobHistory.Select(job => job).Where(job => job.Equals(job1)).Count());
                Assert.Equal(1, jobHistory.Select(job => job).Where(job => job.Equals(job2)).Count());

                await job1.CancelAsync();

                jobHistory = await savedSearch.GetHistoryAsync();
                Assert.Equal(1, jobHistory.Count);
                Assert.Equal(job2, jobHistory[0]);
                Assert.Equal(job2.Name, jobHistory[0].Name);
                Assert.Equal(job2.ResourceName, jobHistory[0].ResourceName);
                Assert.Equal(job2.Sid, jobHistory[0].Sid);

                await job2.CancelAsync();

                jobHistory = await savedSearch.GetHistoryAsync();
                Assert.Equal(0, jobHistory.Count);

                await savedSearch.RemoveAsync();
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service: Saved Searches")]
        [Fact]
        public async Task  CanGetSavedSearches()
        {
            using (var service = await SDKHelper.CreateService())
            {
                var savedSearches = service.SavedSearches;
                await savedSearches.GetAllAsync();

                foreach (SavedSearch savedSearch in savedSearches)
                {
                }

                for (int i = 0; i < savedSearches.Count; i++)
                {
                    SavedSearch savedSearch = savedSearches[i];
                }
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service: Saved Searches")]
        [Fact]
        public async Task CanUpdateSavedSearch()
        {
            using (var service = await SDKHelper.CreateService())
            {
                SavedSearch savedSearch = await service.SavedSearches.GetAsync("Errors in the last 24 hours");
                bool isVisible = savedSearch.IsVisible;
                
                bool updatedSnapshot = await savedSearch.UpdateAsync(new SavedSearchAttributes() { IsVisible = !isVisible });
                Assert.True(updatedSnapshot);
                Assert.Equal(!isVisible, savedSearch.IsVisible);

                await savedSearch.UpdateAsync(new SavedSearchAttributes() { IsVisible = isVisible });
            }
        }

        #endregion

        #region Search Jobs

        [Trait("acceptance-test", "Splunk.Client.Service: Search Jobs")]
        [Fact]
        public async Task  CanGetJob()
        {
            using (var service = await SDKHelper.CreateService())
            {
                Job job1 = null, job2 = null;

                job1 = await service.Jobs.CreateAsync("search index=_internal | head 100");
                await job1.GetSearchResultsAsync();
                await job1.GetSearchResultsEventsAsync();
                await job1.GetSearchResultsPreviewAsync();

                job2 = await service.Jobs.GetAsync(job1.ResourceName.Title);
                Assert.Equal(job1.ResourceName.Title, job2.ResourceName.Title);
                Assert.Equal(job1.Name, job1.ResourceName.Title);
                Assert.Equal(job1.Name, job2.Name);
                Assert.Equal(job1.Sid, job1.Name);
                Assert.Equal(job1.Sid, job2.Sid);
                Assert.Equal(job1.Id, job2.Id);
                //Assert.Equal(new SortedDictionary<string, Uri>().Concat(job1.Links), new SortedDictionary<string, Uri>().Concat(job2.Links));
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service: Search Jobs")]
        [Fact]
        public async Task  CanGetJobs()
        {
            using (var service = await SDKHelper.CreateService())
            {
                var jobs = new Job[]
                {
                    await service.Jobs.CreateAsync("search index=_internal | head 1000"),
                    await service.Jobs.CreateAsync("search index=_internal | head 1000"),
                    await service.Jobs.CreateAsync("search index=_internal | head 1000"),
                    await service.Jobs.CreateAsync("search index=_internal | head 1000"),
                    await service.Jobs.CreateAsync("search index=_internal | head 1000"),
                };

                await service.Jobs.GetAllAsync();

                foreach (var job in jobs)
                {
                    Assert.Contains(job, service.Jobs);
                }
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service: Search Jobs")]
        [Fact]
        public async Task  CanCreateJobAndGetResults()
        {
            var expectedFieldNames = new List<string>
            {
                "_bkt",
                "_cd",
                "_confstr",
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
            };

            var searches = new[]
            {
                new
                {
                    Command = "search index=_internal",
                    JobArgs = new JobArgs
                    {
                        SearchMode = SearchMode.Realtime,
                        MaxCount = 10,
                        EarliestTime = "rt-5m",
                        LatestTime = "rt",
                        MaxTime = 10000
                    }
                },
                new
                {
                    Command = "search index=_internal | head 10",
                    JobArgs = new JobArgs()
                }
            };

            using (var service = await SDKHelper.CreateService())
            {
                foreach (var search in searches)
                {
                    var job = await service.Jobs.CreateAsync(search.Command, search.JobArgs);
                    Assert.NotNull(job);

                    SearchResultStream resultStream = null;

                    if (job.IsRealTimeSearch)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            resultStream = await job.GetSearchResultsPreviewAsync();

                            if (resultStream.FieldNames.Count > 0)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        resultStream = await job.GetSearchResultsAsync();
                    }

                    var count = resultStream.FieldNames.Intersect(expectedFieldNames).Count();
                    Assert.Equal(resultStream.FieldNames.Count, count);
                    List<SearchResult> results = null;

                    Assert.DoesNotThrow(() => results = new List<SearchResult>(resultStream.ToEnumerable()));
                }
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service: Search Jobs")]
        [Fact]
        public async Task CanExportSearchPreviews()
        {
            using (var service = await SDKHelper.CreateService())
            {
                SearchPreviewStream previewStream = await service.ExportSearchPreviewsAsync("search index=_internal | tail 100", new SearchExportArgs() { Count = 0 });

                var results = new List<Splunk.Client.SearchResult>();
                var exception = (Exception)null;

                var manualResetEvent = new ManualResetEvent(true);

                previewStream.Subscribe(
                    onNext: (preview) =>
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
                            preview.FieldNames);

                        if (preview.IsFinal)
                        {
                            results.AddRange(preview.Results);
                        }
                    },
                    onError: (e) =>
                    {
                        exception = new ApplicationException("SearchPreviewStream error: " + e.Message, e);
                        manualResetEvent.Set();

                    },
                    onCompleted: () =>
                    {
                        manualResetEvent.Set();
                    });

                manualResetEvent.Reset();
                manualResetEvent.WaitOne();

                Assert.Null(exception);
                Assert.Equal(100, results.Count);

                await service.LogoffAsync();
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service: Search Jobs")]
        [Fact]
        public async Task CanExportSearchResults()
        {
            using (var service = new Service(SDKHelper.UserConfigure.scheme, SDKHelper.UserConfigure.host, SDKHelper.UserConfigure.port))
            {
                await service.LoginAsync("admin", "changeme");

                SearchResultStream resultStream = await service.ExportSearchResultsAsync("search index=_internal | tail 100", new SearchExportArgs() { Count = 0 });
                var results = new List<Splunk.Client.SearchResult>();
                var exception = (Exception)null;

                var manualResetEvent = new ManualResetEvent(true);

                resultStream.Subscribe(
                    onNext: (result) =>
                    {
                        var count = resultStream.FieldNames.Intersect(result.Keys).Count();
                        Assert.Equal(count, result.Count);

                        if (resultStream.IsFinal)
                        {
                            results.Add(result);
                        }
                    },
                    onError: (e) =>
                    {
                        exception = new ApplicationException("SearchPreviewStream error: " + e.Message, e);
                        manualResetEvent.Set();
                    },
                    onCompleted: () =>
                    {
                        manualResetEvent.Set();
                    });

                manualResetEvent.Reset();
                manualResetEvent.WaitOne();

                Assert.Null(exception);
                Assert.Equal(100, results.Count);

                await service.LogoffAsync();
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service: Search Jobs")]
        [Fact]
        public async Task CanSearchOneshot()
        {
            using (var service = await SDKHelper.CreateService())
            {
                var indexName = string.Format("delete-me-{0}-", Guid.NewGuid().ToString("N"));

                var searchCommands = new string[] 
                {
                    string.Format("search index={0} * | delete", indexName),
                    "search index=_internal | head 100"
                };

                await service.Indexes.CreateAsync(indexName);

                foreach (var command in searchCommands)
                {
                    var searchResults = await service.SearchOneshotAsync(command, new JobArgs() { MaxCount = 100000 });
                    var results = new List<Splunk.Client.SearchResult>(searchResults.ToEnumerable());
                    Assert.NotEmpty(results);
                }
            }
        }

        #endregion

        #region System

        [Trait("acceptance-test", "Splunk.Client.Server")]
        [Fact]
        public async Task CanCrudServerMessages()
        {
            using (var service = await SDKHelper.CreateService())
            {
                //// Create

                ServerMessageCollection messages = service.Server.Messages;
                var name = string.Format("delete-me-{0}", Guid.NewGuid());

                var messageList = new ServerMessage[] 
                {
                    await messages.CreateAsync(string.Format("{0}-{1}", name, ServerMessageSeverity.Information), ServerMessageSeverity.Information, "some message text"),
                    await messages.CreateAsync(string.Format("{0}-{1}", name, ServerMessageSeverity.Warning), ServerMessageSeverity.Warning, "some message text"),
                    await messages.CreateAsync(string.Format("{0}-{1}", name, ServerMessageSeverity.Error), ServerMessageSeverity.Error, "some message text")
                };

                //// Read

                await messages.GetAllAsync();

                foreach (var message in messageList)
                {
                    Assert.NotNull(messages.SingleOrDefault(m => m.Name == message.Name));
                    var messageCopy = await messages.GetAsync(message.Name);
                    Assert.Equal(message, messageCopy);
                    await message.GetAsync();
                }

                //// Delete (there is no update)

                foreach (var message in messages)
                {
                    if (message.Name.StartsWith("delete-me-"))
                    {
                        await message.RemoveAsync();
                    }
                }

                //// Verify delete

                await messages.GetAllAsync();

                foreach (var message in messages)
                {
                    Assert.False(message.Name.StartsWith("delete-me-"));
                }
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Server")]
        [Fact]
        public async Task CanCrudServerSettings()
        {
            using (var service = await SDKHelper.CreateService())
            {
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

                await service.Server.RestartAsync(60000 * 2);
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


                //await service.Server.RestartAsync();
                await TestHelper.RestartServerAsync();
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Server")]
        [Fact]
        public async Task  CanGetServerInfo()
        {
            using (var service = await SDKHelper.CreateService())
            {
                var info = await service.Server.GetInfoAsync();

                EaiAcl acl = info.Eai.Acl;
                Permissions permissions = acl.Permissions;
                int build = info.Build;
                string cpuArchitecture = info.CpuArchitecture;
                Guid guid = info.Guid;
                bool isFree = info.IsFree;
                bool isRealtimeSearchEnabled = info.IsRealtimeSearchEnabled;
                bool isTrial = info.IsTrial;
                IReadOnlyList<string> licenseKeys = info.LicenseKeys;
                IReadOnlyList<string> licenseLabels = info.LicenseLabels;
                string licenseSignature = info.LicenseSignature;
                LicenseState licenseState = info.LicenseState;
                Guid masterGuid = info.MasterGuid;
                ServerMode mode = info.Mode;
                string osBuild = info.OSBuild;
                string osName = info.OSName;
                string osVersion = info.OSVersion;
                string serverName = info.ServerName;
                Version version = info.Version;
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Server")]
        [Fact]
        public async Task CanRestartServer()
        {
            Stopwatch watch = Stopwatch.StartNew();            

            using (var service = await SDKHelper.CreateService())
            {
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

                Assert.Null(service.SessionKey);
                await service.LoginAsync("admin", "changeme");
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Transmitter")]
        [Fact]
        public async Task  CanSendEvents()
        {
            using (var service = await SDKHelper.CreateService())
            {
                // default index

                Index index = await service.Indexes.GetAsync("main");
                Assert.NotNull(index);
                Assert.False(index.Disabled);

                var receiver = service.Transmitter;

                long currentEventCount = index.TotalEventCount;
                Console.WriteLine("Current Index TotalEventCount = {0} ", currentEventCount);
                int sendEventCount = 10;
                                
                for (int i = 0; i < sendEventCount; i++)
                {
                    var result = await receiver.SendAsync(string.Format("{0:D6} {1} CanSendEvents test send string event Hello !", i, DateTime.Now));
                    Assert.NotNull(result);
                }

                Stopwatch watch = Stopwatch.StartNew();                

                while (watch.Elapsed < new TimeSpan(0, 0, 120) && index.TotalEventCount != currentEventCount + sendEventCount)
                {
                    await Task.Delay(1000);
                    await index.GetAsync();
                }

                Console.WriteLine("After send {0} string events, Current Index TotalEventCount = {1} ", sendEventCount, index.TotalEventCount);
                Console.WriteLine("Sleep {0}s to wait index.TotalEventCount got updated", watch.Elapsed);
                Assert.True(index.TotalEventCount == currentEventCount + sendEventCount);

                // Test stream events

                currentEventCount = currentEventCount + sendEventCount;

                using (var eventStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(eventStream, Encoding.UTF8, 4096, leaveOpen: true))
                    {
                        for (int i = 0; i < sendEventCount; i++)
                        {
                            writer.Write(string.Format("{0:D6} {1} jly send stream event hello world!\r\n", i, DateTime.Now));
                        }
                    }

                    eventStream.Seek(0, SeekOrigin.Begin);
                    await receiver.SendAsync(eventStream);
                }

                watch.Restart();

                while (watch.Elapsed < new TimeSpan(0, 0, 120) && index.TotalEventCount != currentEventCount + sendEventCount)
                {
                    await Task.Delay(1000);
                    await index.GetAsync();
                }

                Console.WriteLine("After send {0} strem events, Current Index TotalEventCount = {1} ", sendEventCount, index.TotalEventCount);
                Console.WriteLine("Sleep {0}s to wait index.TotalEventCount got updated", watch.Elapsed);
                Assert.True(index.TotalEventCount == currentEventCount + sendEventCount);
            }
        }

        #endregion

        public void SetFixture(AcceptanceTestingSetup data)
        { } // TODO: Implement or remove

        #region Privates/internals

        static readonly IReadOnlyList<Namespace> TestNamespaces = new Namespace[] 
        { 
            Namespace.Default, 
            new Namespace("admin", "search"), 
            new Namespace("nobody", "search"),
        };

        #endregion
    }
}
