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
    using System.Linq;
    using System.Threading.Tasks;

    using Xunit;

    /// <summary>
    /// Tests the configurations
    /// </summary>
    public class ConfTest
    {
        /// <summary>
        /// Basic conf touch test
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Service")]
        [Fact]
        public async Task Conf()
        {
            Service service = await SDKHelper.CreateService();

            ConfigurationCollection confs = service.GetConfigurationsAsync().Result;

            // Make sure the collection contains some of the expected entries.
            Assert.True(confs.Any(a => a.Title == "eventtypes"));
            Assert.True(confs.Any(a => a.Title == "searchbnf"));
            Assert.True(confs.Any(a => a.Title == "indexes"));
            Assert.True(confs.Any(a => a.Title == "inputs"));
            Assert.True(confs.Any(a => a.Title == "props"));
            Assert.True(confs.Any(a => a.Title == "transforms"));
            Assert.True(confs.Any(a => a.Title == "savedsearches"));

            // Iterate over the confs just to make sure we can read them
            foreach (Configuration conf in confs)
            {
                string dummyString;
                //dummyString = conf.Name;
                dummyString = conf.ResourceName.Title;
                //dummyString = conf.Path;

                foreach (ConfigurationStanza stanza in conf)
                {
                    try
                    {
                        //dummyString = stanza.Name;
                        dummyString = stanza.ResourceName.Title;
                        //dummyString = stanza.Path;
                    }
                    catch (Exception)
                    {
                        // IF the application is disabled, trying to get info
                        // on it will in fact give us a 404 exception.
                    }
                }
            }
        }

        /// <summary>
        /// Tests config Create Read Update and Delete.
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Service")]
        [Fact]
        public async Task ConfCRUD()
        {            
            // Create a fresh app to use as the container for confs that we will
            // create in this test. There is no way to delete a conf once it's
            // created so we make sure to create in the context of this test app
            // and then we delete the app when we are done to make everything go
            // away.
            string app = "sdk-tests";
            string owner = "nobody";
            await TestHelper.CreateApp(app);
            await TestHelper.CreateApp(app);
            Service service =await SDKHelper.CreateService();

            var apps = service.GetApplicationsAsync().Result;
            Assert.True(apps.Any(a => a.ResourceName.Title == app));

            Namespace ns = new Namespace(owner, app);
            service = await SDKHelper.CreateService(ns);

            ConfigurationCollection confs = service.GetConfigurationsAsync().Result;
            //if below failed, remove the file C:\Program Files\Splunk\etc\system\local\testconf.conf, serverInfo should provide home  path etc?            
            Assert.False(confs.Any(a => a.Name == "testconf"));

            Configuration testconf = service.CreateConfigurationAsync("testconf").Result;
            await confs.GetAsync();
            Assert.True(confs.Any(a => a.Name == "testconf"));

            testconf = service.GetConfigurationAsync("testconf").Result;
            await service.CreateConfigurationStanzaAsync("testconf", "stanza1");
            await service.CreateConfigurationStanzaAsync("testconf", "stanza2");
            await service.CreateConfigurationStanzaAsync("testconf", "stanza3");

            await testconf.GetAsync();
            Assert.Equal(4, testconf.Count);
            Assert.NotNull(testconf.GetStanzaAsync("stanza1").Result.Name);
            Assert.NotNull(testconf.GetStanzaAsync("stanza2").Result.Name);
            Assert.NotNull(testconf.GetStanzaAsync("stanza3").Result.Name);

            //// Grab the new stanza and check its content
            ConfigurationStanza stanza1 = testconf.GetStanzaAsync("stanza1").Result;

            // Add a couple of properties
            Argument args = new Argument("key1", "value1");
            Argument args1 = new Argument("key2", "42");
            await stanza1.UpdateAsync(args, args1);
            stanza1 = testconf.GetStanzaAsync("stanza1").Result;
            Assert.Equal("value1", stanza1.GetSettingAsync("key1").Result.Value);
            Assert.Equal("42", stanza1.GetSettingAsync("key2").Result.Value);

            //// Update an existing property
            args = new Argument("key1", "value2");
            await stanza1.UpdateAsync(args);
            Assert.Equal("value2", stanza1.GetSettingAsync("key1").Result.Value);
            Assert.Equal("42", stanza1.GetSettingAsync("key2").Result.Value);

            // Delete the stanzas
            await testconf.RemoveStanzaAsync("stanza3");
            await testconf.GetAsync(); // because remove gives no data back
            Assert.Equal(3, testconf.Count);
            Assert.NotNull(testconf.GetStanzaAsync("stanza1").Result.Name);
            Assert.NotNull(testconf.GetStanzaAsync("stanza2").Result.Name);

            await testconf.RemoveStanzaAsync("stanza2");
            await testconf.GetAsync(); // because remove gives no data back
            Assert.Equal(2, testconf.Count);

            await testconf.RemoveStanzaAsync("stanza1");
            await testconf.GetAsync(); // because remove gives no data back
            Assert.Equal(1, testconf.Count);

            // Cleanup after ourselves
            await TestHelper.RemoveApp(app);
        }
    }
}
