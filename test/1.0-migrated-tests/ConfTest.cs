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
    using Splunk.Client;
    using Xunit;

    /// <summary>
    /// Tests the configurations
    /// </summary>
    public class ConfTest : TestHelper
    {
        /// <summary>
        /// Assert root string
        /// </summary>
        private static string assertRoot = "Config assert: ";

        private bool ConfigurationContainKey(ConfigurationCollection confs, string key)
        {
            foreach (Configuration conf in confs)
            {
                if (conf.ResourceName.Title == key)
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Basic conf touch test
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public void Conf()
        {
            Service service = this.Connect();

            ConfigurationCollection confs = service.GetConfigurationsAsync().Result;

            // Make sure the collection contains some of the expected entries.
            Assert.True(this.ConfigurationContainKey(confs, "eventtypes"), assertRoot + "#1");
            Assert.True(this.ConfigurationContainKey(confs, "searchbnf"), assertRoot + "#2");
            Assert.True(this.ConfigurationContainKey(confs, "indexes"), assertRoot + "#3");
            Assert.True(this.ConfigurationContainKey(confs, "inputs"), assertRoot + "#4");
            Assert.True(this.ConfigurationContainKey(confs, "props"), assertRoot + "#5");
            Assert.True(this.ConfigurationContainKey(confs, "transforms"), assertRoot + "#6");
            Assert.True(this.ConfigurationContainKey(confs, "savedsearches"), assertRoot + "#7");

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
        [Trait("class", "Service")]
        [Fact]
        public void ConfCRUD()
        {
            Service service;
            // Create a fresh app to use as the container for confs that we will
            // create in this test. There is no way to delete a conf once it's
            // created so we make sure to create in the context of this test app
            // and then we delete the app when we are done to make everything go
            // away.
            string app = "sdk-tests";
            string owner = "nobody";
            this.CreateApp(app);
            service = this.Connect();
            var apps = service.GetApplicationsAsync().Result;
            Assert.True(apps.Any(a => a.ResourceName.Title == app), assertRoot + "#8");

            Namespace ns = new Namespace(owner, app);
            service = this.Connect(ns);

            ConfigurationCollection confs = service.GetConfigurationsAsync().Result;
            //if below failed, remove the file C:\Program Files\Splunk\etc\system\local\testconf.conf, serverInfo should provide home  path etc?            
            Assert.False(confs.Any(a => a.Name == "testconf"), assertRoot + "#9");

            Configuration testconf = service.CreateConfigurationAsync("testconf").Result;
            confs.GetAsync().Wait();
            Assert.True(confs.Any(a => a.Name == "testconf"), assertRoot + "#10");

            testconf = service.GetConfigurationAsync("testconf").Result;
            service.CreateConfigurationStanzaAsync("testconf", "stanza1").Wait();
            service.CreateConfigurationStanzaAsync("testconf", "stanza2").Wait();
            service.CreateConfigurationStanzaAsync("testconf", "stanza3").Wait();

            testconf.GetAsync().Wait();
            Assert.Equal(4, testconf.Count);
            Assert.NotNull(testconf.GetStanzaAsync("stanza1").Result.Name);
            Assert.NotNull(testconf.GetStanzaAsync("stanza2").Result.Name);
            Assert.NotNull(testconf.GetStanzaAsync("stanza3").Result.Name);

            //// Grab the new stanza and check its content
            ConfigurationStanza stanza1 = testconf.GetStanzaAsync("stanza1").Result;
            //Assert.False(stanza1.Any(), "Expected stanza1 to be non-empty");
            //Assert.Equal(5, stanza1.Count);//, "Expected stanza1 to have 5 elements");
            //Assert.Equal("nobody", stanza1.GetSettingAsync("eai:userName").Result.Name);
            //Assert.Equal(app, stanza1.GetSettingAsync("eai:appName").Result.Name);

            // Add a couple of properties
            Argument args = new Argument("key1", "value1");
            Argument args1 = new Argument("key2", "42");
            stanza1.UpdateAsync(args, args1).Wait();
            stanza1 = testconf.GetStanzaAsync("stanza1").Result;
            Assert.Equal("value1", stanza1.GetSettingAsync("key1").Result.Value);
            Assert.Equal("42", stanza1.GetSettingAsync("key2").Result.Value);
            //Assert.False(stanza1.ContainsKey("key3"), assertRoot + "#23");

            //// Update an existing property
            args = new Argument("key1", "value2");
            stanza1.UpdateAsync(args).Wait();
            Assert.Equal("value2", stanza1.GetSettingAsync("key1").Result.Value);
            Assert.Equal("42", stanza1.GetSettingAsync("key2").Result.Value);

            // Delete the stanzas
            testconf.RemoveStanzaAsync("stanza3").Wait();
            testconf.GetAsync().Wait(); // because remove gives no data back
            Assert.Equal(3, testconf.Count);
            Assert.NotNull(testconf.GetStanzaAsync("stanza1").Result.Name);
            Assert.NotNull(testconf.GetStanzaAsync("stanza2").Result.Name);
            //Assert.Null(testconf.GetStanzaAsync("stanza3").Result.Name);

            testconf.RemoveStanzaAsync("stanza2").Wait();
            testconf.GetAsync().Wait(); // because remove gives no data back
            Assert.Equal(2, testconf.Count);

            testconf.RemoveStanzaAsync("stanza1").Wait();
            testconf.GetAsync().Wait(); // because remove gives no data back
            Assert.Equal(1, testconf.Count);

            // Cleanup after ourselves
            this.RemoveApp(app);
        }
    }
}
