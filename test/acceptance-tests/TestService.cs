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
    using Splunk.Client.UnitTests;
    
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Security;
    
    using Xunit;

    public class TestService
    {
        // TODO: Move to unit-tests project
        [Trait("acceptance-test", "Splunk.Client.Service")]
        [Fact]
        public void CanConstructService()
        {
            foreach (var ns in TestNamespaces)
            {
                using (var service = new Service(SdkHelper.Splunk.Scheme, SdkHelper.Splunk.Host, SdkHelper.Splunk.Port, ns))
                {
                    Assert.Equal(string.Format("{0}://{1}:{2}/{3}", 
                        SdkHelper.Splunk.Scheme.ToString().ToLower(), 
                        SdkHelper.Splunk.Host, 
                        SdkHelper.Splunk.Port,
                        ns),
                        service.ToString());

                    Assert.IsType(typeof(ApplicationCollection), service.Applications);
                    Assert.NotNull(service.Applications);

                    Assert.IsType(typeof(ConfigurationCollection), service.Configurations);
                    Assert.NotNull(service.Configurations);

                    Assert.IsType(typeof(IndexCollection), service.Indexes);
                    Assert.NotNull(service.Indexes);

                    Assert.IsType(typeof(JobCollection), service.Jobs);
                    Assert.NotNull(service.Jobs);

                    Assert.IsType(typeof(SavedSearchCollection), service.SavedSearches);
                    Assert.NotNull(service.SavedSearches);

                    Assert.IsType(typeof(Server), service.Server);
                    Assert.NotNull(service.Server);

                    Assert.IsType(typeof(StoragePasswordCollection), service.StoragePasswords);
                    Assert.NotNull(service.StoragePasswords);

                    Assert.IsType(typeof(Transmitter), service.Transmitter);
                    Assert.NotNull(service.Transmitter);
                }
            }
        }

        #region Access Control

        [Trait("acceptance-test", "Splunk.Client.StoragePassword")]
        [MockContext]
        [Fact]
        public async Task CanCrudStoragePassword()
        {
            foreach (var ns in TestNamespaces)
            {
                using (var service = await SdkHelper.CreateService(ns))
                {
                    StoragePasswordCollection sps = service.StoragePasswords;
                    await sps.GetAllAsync();

                    foreach (StoragePassword sp in sps)
                    {
                        if (sp.Username.Contains("delete-me-"))
                        {
                            await sp.RemoveAsync(); // TODO: FAILS BECAUSE OF MONO URI IMPLEMENTATION!
                        }
                    }

                    //// Create and change the password for 50 StoragePassword instances

                    var surname = SdkHelper.GetOrElse(string.Format("delete-me-{0}-", Guid.NewGuid().ToString("N")));
                    var realms = new string[] { null, "splunk.com", "splunk:com" };

                    for (int i = 0; i < realms.Length; i++)
                    {
                        var password = "foobar-foobar";
                        var username = surname + i;
                        var realm = realms[i];

                        //// Create

                        StoragePassword sp = await sps.CreateAsync(password, username, realm);

                        Assert.Equal(password, sp.ClearPassword);
                        Assert.Equal(username, sp.Username);
                        Assert.Equal(realm, sp.Realm);

                        //// Read

                        await sp.GetAsync();

                        Assert.Equal(password, sp.ClearPassword);
                        Assert.Equal(username, sp.Username);
                        Assert.Equal(realm, sp.Realm);

                        sp = await sps.GetAsync(username, realm);

                        Assert.Equal(password, sp.ClearPassword);
                        Assert.Equal(username, sp.Username);
                        Assert.Equal(realm, sp.Realm);

                        //// Update

                        password = SdkHelper.GetOrElse(Membership.GeneratePassword(15, 2));
                        await sp.UpdateAsync(password);

                        Assert.Equal(password, sp.ClearPassword);
                        Assert.Equal(username, sp.Username);
                        Assert.Equal(realm, sp.Realm);

                        //// Remove
                    
                        await sp.RemoveAsync();

                        try
                        {
                            await sp.GetAsync();
                            Assert.True(false);
                        }
                        catch (ResourceNotFoundException)
                        { }

                        try 
                        {
                            await sps.GetAsync(username, realm);
                            Assert.True(false);
                        }
                        catch (ResourceNotFoundException)
                        { }

                        sp = await sps.GetOrNullAsync(username, realm);
                        Assert.Null(sp);
                    }
                }
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service")]
        [MockContext]
        [Fact]
        public async Task CanGetCapabilities()
        {
            using (var service = await SdkHelper.CreateService())
            {
                IReadOnlyList<string> capabilities = await service.GetCapabilitiesAsync();
                var serverInfo = await service.Server.GetInfoAsync();

                if (serverInfo.OSName == "Windows")
                {
                    Assert.Equal(new ReadOnlyCollection<string>(new List<string>
                        {
                            "accelerate_datamodel",         // 0
                            "admin_all_objects",            // 1
                            "change_authentication",        // 2
                            "change_own_password",          // 3
                            "delete_by_keyword",            // 4
                            "edit_deployment_client",       // 5
                            "edit_deployment_server",       // 6
                            "edit_dist_peer",               // 7
                            "edit_forwarders",              // 8
                            "edit_httpauths",               // 9
                            "edit_input_defaults",          // 10
                            "edit_monitor",                 // 11
                            "edit_roles",                   // 12
                            "edit_scripted",                // 13
                            "edit_search_server",           // 14
                            "edit_server",                  // 15
                            "edit_splunktcp",               // 16
                            "edit_splunktcp_ssl",           // 17
                            "edit_tcp",                     // 18
                            "edit_udp",                     // 19
                            "edit_user",                    // 20
                            "edit_view_html",               // 21
                            "edit_web_settings",            // 22
                            "edit_win_admon",               // 23
                            "edit_win_eventlogs",           // 24
                            "edit_win_perfmon",             // 25
                            "edit_win_regmon",              // 26
                            "edit_win_wmiconf",             // 27
                            "get_diag",                     // 28
                            "get_metadata",                 // 29
                            "get_typeahead",                // 30
                            "indexes_edit",                 // 31
                            "input_file",                   // 32
                            "license_edit",                 // 33
                            "license_tab",                  // 34
                            "list_deployment_client",       // 35
                            "list_deployment_server",       // 36
                            "list_forwarders",              // 37
                            "list_httpauths",               // 38
                            "list_inputs",                  // 39
                            "list_pdfserver",               // 40
                            "list_win_localavailablelogs",  // 41
                            "output_file",                  // 42
                            "request_remote_tok",           // 43
                            "rest_apps_management",         // 44
                            "rest_apps_view",               // 45
                            "rest_properties_get",          // 46
                            "rest_properties_set",          // 47
                            "restart_splunkd",              // 48
                            "rtsearch",                     // 49
                            "run_debug_commands",           // 50
                            "schedule_rtsearch",            // 51
                            "schedule_search",              // 52
                            "search",                       // 53
                            "use_file_operator",            // 54
                            "write_pdfserver"               // 55
                        }),
                        capabilities);
                }
                else
                {
                    Assert.Equal(new ReadOnlyCollection<string>(new List<string> 
                        {
                            "accelerate_datamodel",
                            "admin_all_objects",
                            "change_authentication",
                            "change_own_password",
                            "delete_by_keyword",
                            "edit_deployment_client",
                            "edit_deployment_server",
                            "edit_dist_peer",
                            "edit_forwarders",
                            "edit_httpauths",
                            "edit_input_defaults",
                            "edit_monitor",
                            "edit_roles",
                            "edit_scripted",
                            "edit_search_server",
                            "edit_server",
                            "edit_splunktcp",
                            "edit_splunktcp_ssl",
                            "edit_tcp",
                            "edit_udp",
                            "edit_user",
                            "edit_view_html",
                            "edit_web_settings",
                            "get_diag",
                            "get_metadata",
                            "get_typeahead",
                            "indexes_edit",
                            "input_file",
                            "license_edit",
                            "license_tab",
                            "list_deployment_client",
                            "list_deployment_server",
                            "list_forwarders",
                            "list_httpauths",
                            "list_inputs",
                            "output_file",
                            "request_remote_tok",
                            "rest_apps_management",
                            "rest_apps_view",
                            "rest_properties_get",
                            "rest_properties_set",
                            "restart_splunkd",
                            "rtsearch",
                            "run_debug_commands",
                            "schedule_rtsearch",
                            "schedule_search",
                            "search",
                            "use_file_operator"
                        }),
                        capabilities);
                }

            }
        }

        [Trait("acceptance-test", "Splunk.Client.StoragePasswordCollection")]
        [MockContext]
        [Fact]
        public async Task CanGetStoragePasswords()
        {
            foreach (var ns in TestNamespaces)
            {
                using (var service = await SdkHelper.CreateService(ns))
                {
                    StoragePasswordCollection sps = service.StoragePasswords;
                    await sps.GetAllAsync();

                    if (sps.Count < 50)
                    {
                        //// Ensure we've got 50 passwords to enumerate

                        var surname = SdkHelper.GetOrElse(string.Format("delete-me-{0}-", Guid.NewGuid().ToString("N")));
                        var realms = new string[] { null, "splunk.com", "splunk:com" };

                        for (int i = 0; i < 50 - sps.Count; i++)
                        {
                            var username = surname + i;
                            var realm = realms[i % realms.Length];
                            var password = SdkHelper.GetOrElse(Membership.GeneratePassword(15, 2));

                            StoragePassword sp = await service.StoragePasswords.CreateAsync(password, username, realm);

                            Assert.Equal(password, sp.ClearPassword);
                            Assert.Equal(username, sp.Username);
                            Assert.Equal(realm, sp.Realm);
                        }

                        await service.StoragePasswords.GetAllAsync();
                    }

                    int count = 0;

                    foreach (var sp in sps)
                    {
                        count++;
                    }

                    Assert.Equal(sps.Count, count);

                    var spl = new List<StoragePassword>(sps.Count);

                    for (int i = 0; i < count; i++)
                    {
                        spl.Add(sps[i]);
                    }

                    Assert.True(sps.SequenceEqual(spl));
                }
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service")]
        [MockContext]
        [Fact]
        public async Task CanLoginAndLogoff()
        {
            using (var service = await SdkHelper.CreateService(Namespace.Default))
            {
                try
                {
                    await service.Applications.GetAllAsync();
                }
                catch (Exception e)
                {
                    Assert.True(false, string.Format("Expected: No exception, Actual: {1}", e.GetType().FullName));
                }

                await service.LogoffAsync();

                Assert.Null(service.SessionKey);

                try
                {
                    await service.Applications.GetAllAsync();
                    Assert.True(false, "Expected AuthenticationFailureException");
                }
                catch (AuthenticationFailureException e)
                {
                    Assert.Equal(HttpStatusCode.Unauthorized, e.StatusCode);
                }

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

        [Trait("acceptance-test", "Splunk.Client.Application")]
        [MockContext]
        [Fact]
        public async Task CanCrudApplication()
        {
            using (var service = await SdkHelper.CreateService())
            {
                //// Install, update, and remove the Splunk App for Twitter Data, version 2.3.1

                var twitterApp = await service.Applications.GetOrNullAsync("twitter2");

                if (twitterApp != null)
                {
                    await twitterApp.RemoveAsync();

                    try
                    {
                        await twitterApp.GetAsync();
                        Assert.False(true, "Expected ResourceNotFoundException");
                    }
                    catch (ResourceNotFoundException)
                    { }

                    twitterApp = await service.Applications.GetOrNullAsync("twitter2");
                    Assert.Null(twitterApp);
                }

                IPHostEntry splunkHostEntry = await Dns.GetHostEntryAsync(service.Context.Host);
                IPHostEntry localHostEntry = await Dns.GetHostEntryAsync("localhost");

                if (splunkHostEntry.HostName == localHostEntry.HostName)
                {
                    var path = Path.Combine(Environment.CurrentDirectory, "Data", "app-for-twitter-data_230.spl");
                    Assert.True(File.Exists(path));

                    twitterApp = await service.Applications.InstallAsync(path, update: true);

                    //// Other asserts on the contents of the update

                    Assert.Equal("Splunk", twitterApp.ApplicationAuthor);
                    Assert.Equal(true, twitterApp.CheckForUpdates);
                    Assert.Equal(false, twitterApp.Configured);
                    Assert.Equal("This application indexes Twitter's sample stream.", twitterApp.Description);
                    Assert.Equal("Splunk-Twitter Connector", twitterApp.Label);
                    Assert.Equal(false, twitterApp.Refresh);
                    Assert.Equal(false, twitterApp.StateChangeRequiresRestart);
                    Assert.Equal("2.3.0", twitterApp.Version);
                    Assert.Equal(true, twitterApp.Visible);

                    //// TODO: Check ApplicationSetupInfo and ApplicationUpdateInfo
                    //// We might check that there is no update info for 2.3.1:
                    ////    Assert.Null(twitterApplicationUpdateInfo.Update);
                    //// Then change the version number to 2.3.0:
                    ////    await twitterApplication.UpdateAsync(new ApplicationAttributes() { Version = "2.3.0" });
                    //// Finally:
                    //// ApplicationUpdateInfo twitterApplicationUpdateInfo = await twitterApplication.GetUpdateInfoAsync();
                    //// Assert.NotNull(twitterApplicationUpdateInfo.Update);
                    //// Assert.True(string.Compare(twitterApplicationUpdateInfo.Update.Version, "2.3.0") == 1, "expect the newer twitter app info");
                    //// Assert.Equal("41ceb202053794cfec54b8d28f78d83c", twitterApplicationUpdateInfo.Update.Checksum);

                    var setupInfo = await twitterApp.GetSetupInfoAsync();
                    var updateInfo = await twitterApp.GetUpdateInfoAsync();

                    await twitterApp.RemoveAsync();

                    try
                    {
                        await twitterApp.GetAsync();
                        Assert.False(true, "Expected ResourceNotFoundException");
                    }
                    catch (ResourceNotFoundException)
                    { }

                    twitterApp = await service.Applications.GetOrNullAsync("twitter2");
                    Assert.Null(twitterApp);
                }

                //// Create an app from one of the built-in templates

                var name = SdkHelper.GetOrElse(string.Format("delete-me-{0}", Guid.NewGuid()));

                var creationAttributes = new ApplicationAttributes()
                {
                    ApplicationAuthor = "Splunk",
                    Configured = true,
                    Description = "This app confirms that an app can be created from a template",
                    Label = name,
                    Version = "2.0.0",
                    Visible = true
                };

                var templatedApp = await service.Applications.CreateAsync(name, "barebones", creationAttributes);

                Assert.Equal(creationAttributes.ApplicationAuthor, templatedApp.ApplicationAuthor);
                Assert.Equal(true, templatedApp.CheckForUpdates);
                Assert.Equal(creationAttributes.Configured, templatedApp.Configured);
                Assert.Equal(creationAttributes.Description, templatedApp.Description);
                Assert.Equal(creationAttributes.Label, templatedApp.Label);
                Assert.Equal(false, templatedApp.Refresh);
                Assert.Equal(false, templatedApp.StateChangeRequiresRestart);
                Assert.Equal(creationAttributes.Version, templatedApp.Version);
                Assert.Equal(creationAttributes.Visible, templatedApp.Visible);

                var updateAttributes = new ApplicationAttributes()
                {
                    ApplicationAuthor = "Splunk, Inc.",
                    Configured = true,
                    Description = "This app update confirms that an app can be updated from a template",
                    Label = name,
                    Version = "2.0.1",
                    Visible = true
                };

                bool updatedSnapshot = await templatedApp.UpdateAsync(updateAttributes, checkForUpdates: true);
                Assert.True(updatedSnapshot);
                await templatedApp.GetAsync(); // Because UpdateAsync doesn't produce an updated snapshot

                Assert.Equal(updateAttributes.ApplicationAuthor, templatedApp.ApplicationAuthor);
                Assert.Equal(true, templatedApp.CheckForUpdates);
                Assert.Equal(updateAttributes.Configured, templatedApp.Configured);
                Assert.Equal(updateAttributes.Description, templatedApp.Description);
                Assert.Equal(updateAttributes.Label, templatedApp.Label);
                Assert.Equal(false, templatedApp.Refresh);
                Assert.Equal(false, templatedApp.StateChangeRequiresRestart);
                Assert.Equal(updateAttributes.Version, templatedApp.Version);
                Assert.Equal(updateAttributes.Visible, templatedApp.Visible);

                Assert.False(templatedApp.Disabled);

                await templatedApp.DisableAsync();
                await templatedApp.GetAsync(); // Because POST apps/local/{name} does not return an updated snapshot
                Assert.True(templatedApp.Disabled);

                await templatedApp.EnableAsync();
                await templatedApp.GetAsync(); // Because POST apps/local/{name} does not return an updated snapshot
                Assert.False(templatedApp.Disabled);

                var archiveInfo = await templatedApp.PackageAsync();

                if (splunkHostEntry.HostName == localHostEntry.HostName)
                {
                    File.Delete(archiveInfo.Path);
                }

                await templatedApp.RemoveAsync();

                try
                {
                    await templatedApp.GetAsync();
                    Assert.False(true, "Expected ResourceNotFoundException");
                }
                catch (ResourceNotFoundException)
                { }

                templatedApp = await service.Applications.GetOrNullAsync(templatedApp.Name);
                Assert.Null(templatedApp);
            }
        }

        [Trait("acceptance-test", "Splunk.Client.ApplicationCollection")]
        [MockContext]
        [Fact]
        public async Task CanGetApplications()
        {
            foreach (var ns in TestNamespaces)
            {
                using (var service = await SdkHelper.CreateService(ns))
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
                            string value = null;

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
                        }

                        args.Offset += service.Applications.Pagination.ItemsPerPage;
                    }
                    while (args.Offset < service.Applications.Pagination.TotalResults);
                }
            }
        }

        #endregion

        #region Configuration

        [Trait("acceptance-test", "Splunk.Client.Configuration")]
        [MockContext]
        [Fact]
        public async Task CanCrudConfiguration() // no delete operation is available
        {
            const string testApplicationName = "acceptance-test_Splunk.Client.Configuration";

            using (var service = await SdkHelper.CreateService())
            {
                Application application = await service.Applications.GetOrNullAsync(testApplicationName);

                if (application != null)
                {
                    await application.RemoveAsync();

                    application = await service.Applications.GetOrNullAsync(testApplicationName);
                    Assert.Null(application);
                }

                await service.Applications.CreateAsync(testApplicationName, "barebones");
                application = await service.Applications.GetOrNullAsync(testApplicationName);
                Assert.NotNull(application);

                await service.Server.RestartAsync(2 * 60 * 1000);
            }

            using (var service = await(SdkHelper.CreateService(new Namespace("nobody", testApplicationName))))
            {
                var fileName = SdkHelper.GetOrElse(string.Format("delete-me-{0}", Guid.NewGuid()));

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

            using (var service = await SdkHelper.CreateService())
            {
                Application application = await service.Applications.GetAsync(testApplicationName);
                await application.RemoveAsync();
                await service.Server.RestartAsync(2 * 60 * 1000);
            }
        }

        [Trait("acceptance-test", "Splunk.Client.ConfigurationCollection")]
        [MockContext]
        [Fact]
        public async Task CanGetConfigurations()
        {
            foreach (Namespace ns in TestNamespaces)
            {
                using (var service = await SdkHelper.CreateService())
                {
                    var inputsConfiguration = await service.Configurations.GetAsync("inputs");

                    foreach (var stanza in inputsConfiguration)  // TODO: FAILS BECAUSE OF MONO URI IMPLEMENTATION!
                    {
                        await stanza.GetAsync();
                    }

                    await service.Configurations.GetAllAsync();

                    foreach (Configuration configuration in service.Configurations)
                    {
                        await configuration.GetAllAsync();

                        foreach (ConfigurationStanza stanza in configuration)
                        {
                            await stanza.GetAsync();
                        }
                    }

                    var configurationList = new List<Configuration>(service.Configurations.Count);

                    for (int i = 0; i < service.Configurations.Count; i++)
                    {
                        Configuration configuration = service.Configurations[i];
                        configurationList.Add(configuration);

                        await configuration.GetAllAsync();
                        var stanzaList = new List<ConfigurationStanza>(configuration.Count);

                        for (int j = 0; j < configuration.Count; j++)
                        {
                            ConfigurationStanza stanza = configuration[j];
                            stanzaList.Add(stanza);
                            await stanza.GetAsync();
                        }

                        Assert.Equal(configuration.ToList(), stanzaList);
                    }

                    Assert.Equal(service.Configurations.ToList(), configurationList);
                }
            }
        }

        #endregion

        #region Indexes

        [Trait("acceptance-test", "Splunk.Client.Index")]
        [MockContext]
        [Fact]
        public async Task CanCrudIndex()
        {
            var ns = new Namespace("nobody", "search");

            using (var service = await SdkHelper.CreateService(ns))
            {
                var indexName = SdkHelper.GetOrElse(string.Format("delete-me-{0}", Guid.NewGuid()));
                Index index;
                
                //// Create

                index = await service.Indexes.CreateAsync(indexName);
                Assert.Equal(true, index.EnableOnlineBucketRepair);

                //// Read

                index = await service.Indexes.GetAsync(indexName);

                //// Update

                Exception updateException = null;

                try
                {
                    var attributes = new IndexAttributes()
                    {
                        EnableOnlineBucketRepair = false
                    };

                    await index.UpdateAsync(attributes);
                    Assert.Equal(attributes.EnableOnlineBucketRepair, index.EnableOnlineBucketRepair);
                    Assert.False(index.Disabled);

                    await index.DisableAsync();
                    Assert.True(index.Disabled);

                    await service.Server.RestartAsync(2 * 60 * 1000);
                    await service.LoginAsync();

                    await index.EnableAsync();
                    Assert.False(index.Disabled);

                }
                catch (Exception e)
                {
                    updateException = e;
                }

                //// Delete

                try 
                {
                    await index.RemoveAsync();
                }
                catch (Exception removeException)
                {
                    if (updateException != null)
                    {
                        var text = string.Format("Update/remove failed:\nUpdate failure: {0}\n\n{1}\nRemove failure: {2}\n{3}",
                            updateException.Message, updateException.StackTrace, 
                            removeException.Message, removeException.StackTrace);
                        Assert.True(false, text);
                    }
                    throw;
                }

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

        [Trait("acceptance-test", "Splunk.Client.IndexCollection")]
        [MockContext]
        [Fact]
        public async Task CanGetIndexes()
        {
            using (var service = await SdkHelper.CreateService(new Namespace(user: "nobody", app: "search")))
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

        #endregion

        #region Inputs

        [Trait("acceptance-test", "Splunk.Client.Transmitter")]
        [MockContext]
        [Fact]
        public async Task CanSendEvents()
        {
            using (var service = await SdkHelper.CreateService())
            {
                // default index

                Index index = await service.Indexes.GetAsync("main");
                Assert.NotNull(index);
                Assert.False(index.Disabled);

                var receiver = service.Transmitter;

                long currentEventCount = index.TotalEventCount;
                int sendEventCount = 10;

                for (int i = 0; i < sendEventCount; i++)
                {
                    var result = await receiver.SendAsync(
                        SdkHelper.GetOrElse(string.Format("{0:D6} {1} Simple event", i, DateTime.Now)));
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
                            writer.Write(
                                SdkHelper.GetOrElse(string.Format("{0:D6}, {1}, Stream event\r\n", i, DateTime.Now)));
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
       
        #region Search

        [Trait("acceptance-test", "Splunk.Client.Job")]
        [MockContext]
        [Fact]
        public async Task CanCrudJob()
        {
            using (var service = await SdkHelper.CreateService())
            {
                Job job1 = null, job2 = null;

                //// Create

                job1 = await service.Jobs.CreateAsync("search index=_internal | head 100");

                using (SearchResultStream stream = await job1.GetSearchEventsAsync())
                {
                }

                using (SearchResultStream stream = await job1.GetSearchPreviewAsync())
                {
                }

                using (SearchResultStream stream = await job1.GetSearchResultsAsync())
                {
                }

                //// Read

                job2 = await service.Jobs.GetAsync(job1.ResourceName.Title);

                Assert.Equal(job1.ResourceName.Title, job2.ResourceName.Title);
                Assert.Equal(job1.Name, job1.ResourceName.Title);
                Assert.Equal(job1.Name, job2.Name);
                Assert.Equal(job1.Sid, job1.Name);
                Assert.Equal(job1.Sid, job2.Sid);
                Assert.Equal(job1.Id, job2.Id);
                //Assert.Equal(new SortedDictionary<string, Uri>().Concat(job1.Links), new SortedDictionary<string, Uri>().Concat(job2.Links));

                //// Update

                bool updatedSnapshot = await job1.UpdateAsync(new CustomJobArgs
                    {
                        new Argument("foo", 1),
                        new Argument("bar", 2)
                    });

                Assert.True(updatedSnapshot);

                //// Delete

                await job1.RemoveAsync();

                try
                {
                    await job1.GetAsync();
                    Assert.True(false);
                }
                catch (ResourceNotFoundException)
                { }

                try
                {
                    await service.Jobs.GetAsync(job1.Name);
                    Assert.True(false);
                }
                catch (ResourceNotFoundException)
                { }

                job2 = await service.Jobs.GetOrNullAsync(job1.Name);
                Assert.Null(job2);
            }
        }

        [Trait("acceptance-test", "Splunk.Client.SavedSearch")]
        [MockContext]
        [Fact]
        public async Task CanCrudSavedSearch()
        {
            using (var service = await SdkHelper.CreateService())
            {
                //// Create

                var name = SdkHelper.GetOrElse(string.Format("delete-me-{0}", Guid.NewGuid()));
                var search = "search index=_internal | head 1000";

                var originalAttributes = new SavedSearchAttributes
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

                //// Read history

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

                //// Read schedule

                var dateTime = SdkHelper.GetOrElse(DateTime.Now);
                var schedule = await savedSearch.GetScheduledTimesAsync(dateTime, dateTime.AddDays(2));

                Assert.Equal(48, schedule.Count);

                var expected = dateTime.AddMinutes(60);
                expected = expected.Date.AddHours(expected.Hour);

                Assert.Equal(expected, schedule[0]);

                //// Update

                search = "search index=_internal * earliest=-1m";

                var updatedAttributes = new SavedSearchAttributes
                {
                    ActionEmailBcc = "user1@splunk.com",
                    ActionEmailCC = "user2@splunk.com",
                    ActionEmailFrom = "user3@splunk.com",
                    ActionEmailTo = "user4@splunk.com, user5@splunk.com",
                    IsVisible = true
                };

                await savedSearch.UpdateAsync(search, updatedAttributes);

                Assert.Equal(search, savedSearch.Search);
                Assert.Equal(updatedAttributes.ActionEmailBcc, savedSearch.Actions.Email.Bcc);
                Assert.Equal(updatedAttributes.ActionEmailCC, savedSearch.Actions.Email.CC);
                Assert.Equal(updatedAttributes.ActionEmailFrom, savedSearch.Actions.Email.From);
                Assert.Equal(updatedAttributes.ActionEmailTo, savedSearch.Actions.Email.To);
                Assert.Equal(updatedAttributes.IsVisible, savedSearch.IsVisible);

                //// Update schedule

                dateTime = SdkHelper.GetOrElse(DateTime.Now);

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

        [Trait("acceptance-test", "Splunk.Client.Service")]
        [MockContext]
        [Fact]
        public async Task CanDispatchSavedSearch()
        {
            using (var service = await SdkHelper.CreateService())
            {
                Job job = await service.DispatchSavedSearchAsync("Splunk errors last 24 hours");
                
                using (SearchResultStream stream = await job.GetSearchResultsAsync())
                {
                    var results = new List<SearchResult>();

                    foreach (SearchResult result in stream)
                    {
                        results.Add(result);
                    }

                    Assert.NotEmpty(results);
                }
            }
        }
            
        [Trait("acceptance-test", "Splunk.Client.JobCollection")]
        [MockContext]
        [Fact]
        public async Task CanGetJobs()
        {
            using (var service = await SdkHelper.CreateService())
            {
                var jobs = new Job[]
                {
                    await service.Jobs.CreateAsync("search index=_internal | head 10"),
                    await service.Jobs.CreateAsync("search index=_internal | head 10"),
                    await service.Jobs.CreateAsync("search index=_internal | head 10"),
                    await service.Jobs.CreateAsync("search index=_internal | head 10"),
                    await service.Jobs.CreateAsync("search index=_internal | head 10"),
                };
                
                await service.Jobs.GetAllAsync();
                Assert.True(service.Jobs.Count >= jobs.Length);
                
                foreach (var job in jobs)
                {
                    Assert.Contains(job, service.Jobs);
                }

                var sequence = new List<Job>(service.Jobs.Count);

                for (int i = 0; i < service.Jobs.Count; i++)
                {
                    sequence.Add(service.Jobs[i]);
                }

                Assert.Equal(service.Jobs.ToList(), sequence);
            }
        }

        [Trait("acceptance-test", "Splunk.Client.SavedSearchCollection")]
        [MockContext]
        [Fact]
        public async Task CanGetSavedSearches()
        {
            using (var service = await SdkHelper.CreateService())
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

        [Trait("acceptance-test", "Splunk.Client.Service")]
        [MockContext]
        [Fact]
        public async Task CanExportSearchPreviewsToEnumerable()
        {
            using (var service = await SdkHelper.CreateService())
            {
                const string search = "search index=_internal | tail 100";
                var args = new SearchExportArgs() { Count = 0 };

                using (SearchPreviewStream previewStream = await service.ExportSearchPreviewsAsync(search, args))
                {
                    var results = new List<SearchResult>();

                    foreach (var preview in previewStream)
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
                    }

                    Assert.Equal(100, results.Count);
                }

                await service.LogoffAsync();
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service")]
        [MockContext]
        [Fact]
        public async Task CanExportSearchPreviewsToObservable()
        {
            using (var service = await SdkHelper.CreateService())
            {
                const string search = "search index=_internal | tail 100";
                var args = new SearchExportArgs() { Count = 0 };

                using (SearchPreviewStream stream = await service.ExportSearchPreviewsAsync(search, args))
                {
                    var manualResetEvent = new ManualResetEvent(true);
                    var results = new List<SearchResult>();
                    var exception = (Exception)null;

                    stream.Subscribe(new Observer<SearchPreview>(
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
                        onCompleted: () =>
                        {
                            manualResetEvent.Set();
                        },
                        onError: (e) =>
                        {
                            exception = new ApplicationException("SearchPreviewStream error: " + e.Message, e);
                            manualResetEvent.Set();
                        }));

                    manualResetEvent.Reset();
                    manualResetEvent.WaitOne();

                    Assert.Null(exception);
                    Assert.Equal(100, results.Count);
                }

                await service.LogoffAsync();
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service")]
        [MockContext]
        [Fact]
        public async Task CanExportSearchResultsToEnumerable()
        {
            using (var service = await SdkHelper.CreateService())
            {
                var args = new SearchExportArgs { Count = 0 };

                using (SearchResultStream stream = await service.ExportSearchResultsAsync("search index=_internal | tail 100", args))
                {
                    var results = new List<SearchResult>();

                    foreach (SearchResult result in stream)
                    {
                        results.Add(result);
                    }
                }

                await service.LogoffAsync();
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service")]
        [MockContext]
        [Fact]
        public async Task CanExportSearchResultsToObservable()
        {
            using (var service = await SdkHelper.CreateService())
            {
                var args = new SearchExportArgs { Count = 0 };

                using (SearchResultStream stream = await service.ExportSearchResultsAsync("search index=_internal | tail 100", args))
                {
                    var manualResetEvent = new ManualResetEvent(true);
                    var results = new List<SearchResult>();
                    var exception = (Exception)null;

                    stream.Subscribe(new Observer<SearchResult>(
                        onNext: (result) =>
                        {
                            var count = stream.FieldNames.Intersect(result.Keys).Count();
                            Assert.Equal(count, result.Count);

                            if (stream.IsFinal)
                            {
                                results.Add(result);
                            }
                        },
                        onCompleted: () =>
                        {
                            manualResetEvent.Set();
                        },
                        onError: (e) =>
                        {
                            exception = new ApplicationException("SearchPreviewStream error: " + e.Message, e);
                            manualResetEvent.Set();
                        }));

                    manualResetEvent.Reset();
                    manualResetEvent.WaitOne();

                    Assert.Null(exception);
                    Assert.Equal(100, results.Count);
                }

                await service.LogoffAsync();
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Service")]
        [MockContext]
        [Fact]
        public async Task CanSearchOneshot()
        {
            using (var service = await SdkHelper.CreateService())
            {
                var indexName = SdkHelper.GetOrElse(string.Format("delete-me-{0}", Guid.NewGuid().ToString("N")));

                var searches = new[] 
                {
                    new 
                    { 
                        Command = string.Format("search index={0} * | delete", indexName),
                        ResultCount = 0
                    },
                    new 
                    { 
                        Command = "search index=_internal | head 100",
                        ResultCount = 100
                    }
                };

                await service.Indexes.CreateAsync(indexName);

                foreach (var search in searches)
                {
                    var args = new JobArgs { MaxCount = 100000 };

                    using (SearchResultStream stream = await service.SearchOneshotAsync(search.Command, args))
                    {
                        var list = new List<SearchResult>();

                        foreach (SearchResult result in stream) 
                        {
                            list.Add(result);
                        }

                        Assert.Equal(search.ResultCount, stream.ReadCount);
                        Assert.Equal(search.ResultCount, list.Count);
                    }
                }
            }
        }

        #endregion

        #region System

        [Trait("acceptance-test", "Splunk.Client.ServerMessage")]
        [MockContext]
        [Fact]
        public async Task CanCrudServerMessage()
        {
            using (var service = await SdkHelper.CreateService())
            {
                //// Create

                var name = SdkHelper.GetOrElse(string.Format("delete-me-{0}", Guid.NewGuid()));
                ServerMessageCollection messages = service.Server.Messages;

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

        [Trait("acceptance-test", "Splunk.Client.ServerSettings")]
        [MockContext]
        [Fact]
        public async Task CanCrudServerSettings()
        {
            using (var service = await SdkHelper.CreateService())
            {
                //// Get

                var originalSettings = await service.Server.GetSettingsAsync();

                ServerSettingValues originalValues = new ServerSettingValues
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

                //// Update

                try
                {
                    var updatedValues = new ServerSettingValues
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

                    ServerSettings updatedSettings = await service.Server.UpdateSettingsAsync(updatedValues);

                    Assert.Equal(updatedValues.EnableSplunkWebSsl, updatedSettings.EnableSplunkWebSsl);
                    Assert.Equal(updatedValues.Host, updatedSettings.Host);
                    Assert.Equal(updatedValues.HttpPort, updatedSettings.HttpPort);
                    Assert.Equal(updatedValues.ManagementHostPort, updatedSettings.ManagementHostPort);
                    Assert.Equal(updatedValues.MinFreeSpace, updatedSettings.MinFreeSpace);
                    Assert.Equal(updatedValues.Pass4SymmetricKey, updatedSettings.Pass4SymmetricKey);
                    Assert.Equal(updatedValues.ServerName, updatedSettings.ServerName);
                    Assert.Equal(updatedValues.SessionTimeout, updatedSettings.SessionTimeout);
                    Assert.Equal(updatedValues.SplunkDB, updatedSettings.SplunkDB);
                    Assert.Equal(updatedValues.StartWebServer, updatedSettings.StartWebServer);
                    Assert.Equal(updatedValues.TrustedIP, updatedSettings.TrustedIP);

                    //// Restart the server because it's required following a server settings update

                    await service.Server.RestartAsync(2 * 60 * 1000);
                    await service.LoginAsync();

                }
                catch (Exception e1)
                {
                    try
                    {
                        service.Server.UpdateSettingsAsync(originalValues).Wait(); // because you can't await in catch block
                    }
                    catch (Exception e2)
                    {
                        throw new AggregateException(e1, e2);
                    }

                    throw;
                }

                //// Restore

                originalSettings = await service.Server.UpdateSettingsAsync(originalValues);

                Assert.Equal(originalValues.EnableSplunkWebSsl, originalSettings.EnableSplunkWebSsl);
                Assert.Equal(originalValues.Host, originalSettings.Host);
                Assert.Equal(originalValues.HttpPort, originalSettings.HttpPort);
                Assert.Equal(originalValues.ManagementHostPort, originalSettings.ManagementHostPort);
                Assert.Equal(originalValues.MinFreeSpace, originalSettings.MinFreeSpace);
                Assert.Equal(originalValues.Pass4SymmetricKey, originalSettings.Pass4SymmetricKey);
                Assert.Equal(originalValues.ServerName, originalSettings.ServerName);
                Assert.Equal(originalValues.SessionTimeout, originalSettings.SessionTimeout);
                Assert.Equal(originalValues.SplunkDB, originalSettings.SplunkDB);
                Assert.Equal(originalValues.StartWebServer, originalSettings.StartWebServer);
                Assert.Equal(originalValues.TrustedIP, originalSettings.TrustedIP);

                //// Restart the server because it's required following a settings update
                await service.Server.RestartAsync(2 * 60 * 1000);
            }
        }

        [Trait("acceptance-test", "Splunk.Client.Server")]
        [MockContext]
        [Fact]
        public async Task CanGetServerInfo()
        {
            using (var service = await SdkHelper.CreateService())
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
        [MockContext]
        [Fact]
        public async Task CanRestartServer()
        {
            Stopwatch watch = Stopwatch.StartNew();            

            using (var service = await SdkHelper.CreateService())
            {
                try
                {
                    await service.Server.RestartAsync(2 * 60 * 1000);
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine("----------------------------------------------------------------------------------------");
                    Console.WriteLine("{1}, spend {0}s to restart server failed:", watch.Elapsed.TotalSeconds, DateTime.Now);
                    Console.WriteLine(e);
                    Console.WriteLine("----------------------------------------------------------------------------------------");
                }

                Assert.Null(service.SessionKey);
                await service.LoginAsync();
            }
        }

        #endregion

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
