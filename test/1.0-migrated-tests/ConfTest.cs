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
            string app = "sdk-tests";
            string owner = "nobody";
            // Create a fresh app to use as the container for confs that we will
            // create in this test. There is no way to delete a conf once it's
            // created so we make sure to create in the context of this test app
            // and then we delete the app when we are done to make everything go
            // away.
            this.CreateApp(app);
            service = this.Connect();
            var apps = service.GetApplicationsAsync().Result;

            bool foundApp = false;
            foreach (Application application in apps)
            {
                if (application.ResourceName.Title == app)
                {
                    foundApp = true;
                }
            }
            Assert.True(foundApp, assertRoot + "#8");

            //// Create an app specific service instance
            //Args args = new Args(this.SetUp().Opts);
            //args.Add("app", app);
            //args.Add("owner", owner);
            //service = Service.Connect(args);

            //Context context = new Context(Scheme.Https, this.SetUp().Host, this.SetUp().Port);
            // service = new Service(context);
            //service.LoginAsync(this.SetUp().Username, this.SetUp().Password).Wait();


            ConfigurationCollection confs = service.GetConfigurationsAsync().Result;
            Assert.False(this.ConfigurationContainKey(confs, "testconf"), assertRoot + "#9");

            Configuration testconf = service.CreateConfigurationAsync("testconf").Result;
            //confs.CreateAsync("testconf").Wait();

            Assert.True(this.ConfigurationContainKey(confs, "testconf"), assertRoot + "#10");

            //EntityCollection<Entity> stanzas = confs.Get("testconf");
            //Assert.Equal(0, stanzas.Size, assertRoot + "#11");

            //stanzas.Create("stanza1");
            //stanzas.Create("stanza2");
            //stanzas.Create("stanza3");
            //Assert.Equal(3, stanzas.Size, assertRoot + "#12");
            //Assert.True(stanzas.ContainsKey("stanza1"), assertRoot + "#13");
            //Assert.True(stanzas.ContainsKey("stanza2"), assertRoot + "#14");
            //Assert.True(stanzas.ContainsKey("stanza3"), assertRoot + "#15");

            //// Grab the new stanza and check its content
            //Entity stanza1 = stanzas.Get("stanza1");
            //Assert.False(stanza1.IsEmpty, "Expected stanza1 to be non-empty");
            //Assert.Equal(5, stanza1.Size, "Expected stanza1 to have 5 elements");
            //Assert.Equal("nobody", stanza1.Get("eai:userName"), assertRoot + "#16");
            //Assert.Equal(app, stanza1.Get("eai:appName"), assertRoot + "#17");
            //Assert.False(stanza1.ContainsKey("key1"), assertRoot + "#18");
            //Assert.False(stanza1.ContainsKey("key2"), assertRoot + "#19");
            //Assert.False(stanza1.ContainsKey("key3"), assertRoot + "#20");

            //// Add a couple of properties
            //args = new Args();
            //args.Add("key1", "value1");
            //args.Add("key2", 42);
            //stanza1.Update(args);

            //// Make sure the properties showed up
            //Assert.Equal("value1", stanza1.Get("key1"), assertRoot + "#21");
            //Assert.Equal("42", stanza1.Get("key2"), assertRoot + "#22");
            //Assert.False(stanza1.ContainsKey("key3"), assertRoot + "#23");

            //// Update an existing property
            //args = new Args();
            //args.Add("key1", "value2");
            //stanza1.Update(args);

            //// Make sure the updated property shows up (and no other changes).
            //Assert.Equal("value2", stanza1.Get("key1"), assertRoot + "#24");
            //Assert.Equal("42", stanza1.Get("key2"), assertRoot + "#25");
            //Assert.False(stanza1.ContainsKey("key3"), assertRoot + "#26");
            //Assert.True(stanza1.ContainsValue("value2"), "Expected stanza1 to contain the value value2");
            //Assert.True(stanza1.ContainsValue("42"), "Expected stanza1 to contain the value 42");

            //// Delete the stanzas
            //stanzas.Remove("stanza3");
            //Assert.Equal(2, stanzas.Size, assertRoot + "#27");
            //Assert.True(stanzas.ContainsKey("stanza1"), assertRoot + "#28");
            //Assert.True(stanzas.ContainsKey("stanza2"), assertRoot + "#29");
            //Assert.False(stanzas.ContainsKey("stanza3"), assertRoot + "#30");

            //stanzas.Remove("stanza2");
            //Assert.Equal(1, stanzas.Size, assertRoot + "#31");
            //Assert.True(stanzas.ContainsKey("stanza1"), assertRoot + "#32");
            //Assert.False(stanzas.ContainsKey("stanza2"), assertRoot + "#33");
            //Assert.False(stanzas.ContainsKey("stanza3"), assertRoot + "#34");

            //stanzas.Remove("stanza1");
            //Assert.Equal(0, stanzas.Size, assertRoot + "#35");
            //Assert.False(stanzas.ContainsKey("stanza1"), assertRoot + "#36");
            //Assert.False(stanzas.ContainsKey("stanza2"), assertRoot + "#37");
            //Assert.False(stanzas.ContainsKey("stanza3"), assertRoot + "#38");

            //// Cleanup after ourselves
            //this.RemoveApp(app);
        }
    }
}
