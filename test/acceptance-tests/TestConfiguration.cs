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
    public class ConfigurationsTest
    {
        /// <summary>
        /// Basic conf touch test
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Configuration")]
        [MockContext]
        [Fact]
        public async Task ConfigurationCollection()
        {
            using (var service = await SdkHelper.CreateService())
            {
                ConfigurationCollection confs = service.Configurations;
                await confs.GetAllAsync();

                //// Make sure the collection contains some of the expected entries

                Assert.NotNull(confs.SingleOrDefault(a => a.Title == "eventtypes"));
                Assert.NotNull(confs.SingleOrDefault(a => a.Title == "searchbnf"));
                Assert.NotNull(confs.SingleOrDefault(a => a.Title == "indexes"));
                Assert.NotNull(confs.SingleOrDefault(a => a.Title == "inputs"));
                Assert.NotNull(confs.SingleOrDefault(a => a.Title == "props"));
                Assert.NotNull(confs.SingleOrDefault(a => a.Title == "transforms"));
                Assert.NotNull(confs.SingleOrDefault(a => a.Title == "savedsearches"));

                //// Iterate over the confs just to make sure we can read them

                foreach (Configuration conf in confs)
                {
                    string dummyString;
                
                    dummyString = conf.Name;
                    dummyString = conf.Title;

                    foreach (ConfigurationStanza stanza in conf)
                    {
                        dummyString = stanza.Name;
                        dummyString = stanza.Title;
                    }
                }

                for (int i = 0; i < confs.Count; i++)
                {
                    Configuration conf = confs[i];
                    string dummyString;

                    dummyString = conf.Name;
                    dummyString = conf.Title;

                    foreach (ConfigurationStanza stanza in conf)
                    {
                        dummyString = stanza.Name;
                        dummyString = stanza.Title;
                    }
                }
            }
        }

        /// <summary>
        /// Tests config Create Read Update and Delete.
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Configuration")]
        [MockContext]
        [Fact]
        public async Task Configuration()
        {            
            const string app = "sdk-tests"; // Provides a removable jail for the configuration changes we'll make

            using (var service = await SdkHelper.CreateService())
            {
                await service.Applications.RecreateAsync(app);
                await service.Server.RestartAsync(2 * 60 * 1000);
            }

            using (var service = await SdkHelper.CreateService(new Namespace(user: "nobody", app: app)))
            {
                ConfigurationCollection confs = service.Configurations;
                await confs.GetAllAsync();

                //// If this assesrt fails, remove the file C:\Program Files\Splunk\etc\system\local\testconf.conf.
                Assert.False(confs.Any(a => a.Name == "testconf"));

                Configuration testconf = await confs.CreateAsync("testconf");
                testconf = await confs.GetOrNullAsync("testconf");
                Assert.NotNull(testconf);

                ConfigurationStanza stanza1 = await testconf.CreateAsync("stanza1");
                ConfigurationStanza stanza2 = await testconf.CreateAsync("stanza2");
                ConfigurationStanza stanza3 = await testconf.CreateAsync("stanza3");

                await testconf.GetAllAsync();
            
                Assert.Equal(4, testconf.Count);
                Assert.NotNull(testconf.SingleOrDefault(conf => conf.Name == "default"));
                Assert.NotNull(testconf.SingleOrDefault(conf => conf.Name == stanza1.Name));
                Assert.NotNull(testconf.SingleOrDefault(conf => conf.Name == stanza2.Name));
                Assert.NotNull(testconf.SingleOrDefault(conf => conf.Name == stanza3.Name));

                //// Grab one of the new stanzas and check its content

                stanza1 = await testconf.GetAsync("stanza1");

                // Add a couple of properties

                await stanza1.UpdateAsync(
                    new Argument("key1", "string"),
                    new Argument("key2", (byte)2),
                    new Argument("key3", 'c'),
                    new Argument("key4", 4.00M),
                    new Argument("key5", 5.0),
                    new Argument("key6", 6.0F),
                    new Argument("key7", 7),
                    new Argument("key8", 8L),
                    new Argument("key9", stanza1),
                    new Argument("key10", (sbyte)10),
                    new Argument("key11", (uint)11),
                    new Argument("key12", (ulong)12));

                stanza1 = await testconf.GetAsync("stanza1");

                Assert.NotNull(stanza1.SingleOrDefault(setting => setting.Title == "key1" && setting.Value == "string"));
                Assert.NotNull(stanza1.SingleOrDefault(setting => setting.Title == "key2" && setting.Value == ((byte)2).ToString()));
                Assert.NotNull(stanza1.SingleOrDefault(setting => setting.Title == "key3" && setting.Value == 'c'.ToString()));
                Assert.NotNull(stanza1.SingleOrDefault(setting => setting.Title == "key4" && setting.Value == 4.00M.ToString()));
                Assert.NotNull(stanza1.SingleOrDefault(setting => setting.Title == "key5" && setting.Value == 5.0.ToString()));
                Assert.NotNull(stanza1.SingleOrDefault(setting => setting.Title == "key6" && setting.Value == 6.0F.ToString()));
                Assert.NotNull(stanza1.SingleOrDefault(setting => setting.Title == "key7" && setting.Value == 7.ToString()));
                Assert.NotNull(stanza1.SingleOrDefault(setting => setting.Title == "key8" && setting.Value == 8L.ToString()));
                Assert.NotNull(stanza1.SingleOrDefault(setting => setting.Title == "key9" && setting.Value == stanza1.ToString()));
                Assert.NotNull(stanza1.SingleOrDefault(setting => setting.Title == "key10" && setting.Value == ((sbyte)10).ToString()));
                Assert.NotNull(stanza1.SingleOrDefault(setting => setting.Title == "key11" && setting.Value == ((uint)11).ToString()));
                Assert.NotNull(stanza1.SingleOrDefault(setting => setting.Title == "key12" && setting.Value == ((ulong)12).ToString()));

                string value;

                value = await stanza1.GetAsync("key1");
                Assert.Equal("string", value);

                value = await stanza1.GetAsync("key2");
                Assert.Equal(((byte)2).ToString(), value);

                value = await stanza1.GetAsync("key3");
                Assert.Equal('c'.ToString(), value);

                value = await stanza1.GetAsync("key4");
                Assert.Equal(4.00M.ToString(), value);
            
                value = await stanza1.GetAsync("key5");
                Assert.Equal(5.0.ToString(), value);

                value = await stanza1.GetAsync("key6");
                Assert.Equal(6.0F.ToString(), value);

                value = await stanza1.GetAsync("key7");
                Assert.Equal(7.ToString(), value);

                value = await stanza1.GetAsync("key8");
                Assert.Equal(8L.ToString(), value);

                value = await stanza1.GetAsync("key9");
                Assert.Equal(stanza1.ToString(), value);

                value = await stanza1.GetAsync("key10");
                Assert.Equal(((sbyte)10).ToString(), value);

                value = await stanza1.GetAsync("key11");
                Assert.Equal(((uint)11).ToString(), value);

                value = await stanza1.GetAsync("key12");
                Assert.Equal(((ulong)12).ToString(), value);

                //// Update an existing setting

                bool updatedSnapshot = await stanza1.UpdateAsync(new Argument("key1", "value2"));
                Assert.False(updatedSnapshot);
                await stanza1.GetAsync();

                Assert.NotNull(stanza1.SingleOrDefault(setting => setting.Title == "key1" && setting.Value == "value2"));
                value = await stanza1.GetAsync("key1");
                Assert.Equal("value2", value);
                value = await stanza1.GetAsync("key1");
                Assert.Equal("value2", value);

                //// Remove the stanzas

                // Stanza 1
                await stanza1.RemoveAsync();
            
                await testconf.GetAllAsync(); // because remove gives no data back
                Assert.Equal(3, testconf.Count);
                Assert.NotNull(testconf.SingleOrDefault(stanza => stanza.Name == "default"));
                Assert.Null(testconf.SingleOrDefault(stanza => stanza.Name == "stanza1"));
                Assert.NotNull(testconf.SingleOrDefault(stanza => stanza.Name == "stanza2"));
                Assert.NotNull(testconf.SingleOrDefault(stanza => stanza.Name == "stanza3"));

                // Stanza 2
                await stanza2.RemoveAsync();

                await testconf.GetAllAsync(); // because remove gives no data back
                Assert.Equal(2, testconf.Count);
                Assert.NotNull(testconf.SingleOrDefault(stanza => stanza.Name == "default"));
                Assert.Null(testconf.SingleOrDefault(stanza => stanza.Name == "stanza1"));
                Assert.Null(testconf.SingleOrDefault(stanza => stanza.Name == "stanza2"));
                Assert.NotNull(testconf.SingleOrDefault(stanza => stanza.Name == "stanza3"));

                // Stanza 3
                await stanza3.RemoveAsync();

                await testconf.GetAllAsync(); // because remove gives no data back

                Assert.Equal(1, testconf.Count);
                Assert.NotNull(testconf.SingleOrDefault(stanza => stanza.Name == "default"));
                Assert.Null(testconf.SingleOrDefault(stanza => stanza.Name == "stanza1"));
                Assert.Null(testconf.SingleOrDefault(stanza => stanza.Name == "stanza2"));
                Assert.Null(testconf.SingleOrDefault(stanza => stanza.Name == "stanza3"));
            }

            using (var service = await SdkHelper.CreateService())
            {
                Assert.True(await service.Applications.RemoveAsync(app));
                await service.Server.RestartAsync(2 * 60 * 1000);
            }
        }
    }
}
