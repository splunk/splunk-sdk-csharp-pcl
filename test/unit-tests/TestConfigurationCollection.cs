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
    using Microsoft.CSharp.RuntimeBinder;
    using Splunk.Client;
    using Splunk.Client.Refactored;

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    using Xunit;

    public class TestConfigurationCollection
    {
        [Trait("unit-test", "class Configuration")]
        [Fact]
        async Task CanConstructConfiguration()
        {
            var feed = await TestAtomFeed.ReadFeed(Path.Combine(TestAtomFeed.Directory, "Configuration.GetAsync.xml"));

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                var expectedConfigurationStanzaNames = new string[] 
                { 
                    "default",
                    "email",
                    "populate_lookup",
                    "rss",
                    "script",
                    "summary_index"
                };

                var configuration = new Refactored.Configuration(context, feed);
                
                Assert.Equal(expectedConfigurationStanzaNames, from configurationStanza in configuration select configurationStanza.Title);
                Assert.Equal(expectedConfigurationStanzaNames.Length, configuration.Count);

                for (int i = 0; i < configuration.Count; i++)
                {
                    Assert.Equal(expectedConfigurationStanzaNames[i], configuration[i].Title);

                    Assert.DoesNotThrow(() => { var value = configuration[i].GeneratorVersion; });
                    Assert.DoesNotThrow(() => { var value = configuration[i].Id; });
                    Assert.DoesNotThrow(() => { var value = configuration[i].Updated; });

                    Assert.Equal(0, configuration[i].Count);
                    Assert.Throws<ArgumentOutOfRangeException>(() => { var value = configuration[i][0]; });
                }
            }
        }

        [Trait("unit-test", "class ConfigurationCollection")]
        [Fact]
        async Task CanConstructConfigurationCollection()
        {
            var feed = await TestAtomFeed.ReadFeed(Path.Combine(TestAtomFeed.Directory, "ConfigurationCollection.GetAsync.xml"));

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                var expectedConfigurationNames = new string[] 
                { 
                    "alert_actions",
                    "app",
                    "audit",
                    "authentication",
                    "authorize",
                    "commands",
                    "conf",
                    "crawl",
                    "datatypesbnf",
                    "default-mode",
                    "delete-me-0af59d05d0124e4ab915398500a45262",
                    "delete-me-1c58e9c0851d435aba8b5c53b0c73959",
                    "delete-me-233bbea06392457c9f38b629a912baff",
                    "delete-me-24d778d67e7b4f4e9704c8118e9cbc75",
                    "delete-me-2a38f8d7bfa542c9a09b201ee29da2a8",
                    "delete-me-2f884142c52b4c0883399dfa30fff5bf",
                    "delete-me-3322c0339fb849d98605559cf119a369",
                    "delete-me-33bc32518a364599a08d4bfb226a0975",
                    "delete-me-35a847f36e8a491d962d04a037310dfd",
                    "delete-me-42cecdc8e72f4645b75616c94cd93399",
                    "delete-me-446ee1261b424ae3aec66b67e391df69",
                    "delete-me-4d5457f77b6d49479d518cc4204f300d",
                    "delete-me-59aa7746422549368e550e1bded488af",
                    "delete-me-6d374e8b93014515b506918ada66026e",
                    "delete-me-6d4d46fe88f0444cb330f2a6f022477a",
                    "delete-me-71e7fd0fc0fc420e9dd034a3fd6ae989",
                    "delete-me-7311a03780674f5dbff082fe5f09b458",
                    "delete-me-7a48db483d73412480efd9883af53001",
                    "delete-me-7a4ad0da232f45dab041343c2235a5b0",
                    "delete-me-980b1cb1b2f34884a972ed8bf0c680e2",
                    "delete-me-9940cceaeaf74309b415c43e648ff9eb",
                    "delete-me-9bd517dbec464d32a3b0b51f79bcdad9",
                    "delete-me-9d0b6101094141eab7a69d0daabdce76",
                    "delete-me-9fd99ef85a3641b09a2d5a512bc692c0",
                    "delete-me-a017f23857f14f9fa66e060fbdbd0b37",
                    "delete-me-a5999e197618423ab0c9afffd052c7a9",
                    "delete-me-b8b05d95822a4552bc9c67f863975f0f",
                    "delete-me-c5c6edfa1ccc470981e794d50f2940c6",
                    "delete-me-c6d66b4d921242608962431036860b50",
                    "delete-me-d7e1572f83644e658ccc926c1a736a7f",
                    "delete-me-d8dc0fb200ab4be9862d7e042e919b2c",
                    "delete-me-f8565b63b2af4c0993481440e81ee018",
                    "delete-me-fd46773243cc4b5a8b731483a13e55fb",
                    "distsearch",
                    "event_renderers",
                    "eventdiscoverer",
                    "eventtypes",
                    "fields",
                    "indexes",
                    "inputs",
                    "launcher",
                    "limits",
                    "literals",
                    "lookups",
                    "macros",
                    "manager",
                    "migration",
                    "multikv",
                    "nav",
                    "outputs",
                    "pdf_server",
                    "prefs",
                    "procmon-filters",
                    "props",
                    "quickstart",
                    "restmap",
                    "savedsearches",
                    "searchbnf",
                    "searchscripts",
                    "segmenters",
                    "server",
                    "serverclass",
                    "source-classifier",
                    "sourcetypes",
                    "times",
                    "transactiontypes",
                    "transforms",
                    "ui-prefs",
                    "user-prefs",
                    "views",
                    "viewstates",
                    "web",
                    "workflow_actions"
                };

                var configurations = new Refactored.ConfigurationCollection(context, feed);

                Assert.Equal(expectedConfigurationNames, from configuration in configurations select configuration.Title);
                Assert.Equal(expectedConfigurationNames.Length, configurations.Count);

                for (int i = 0; i < configurations.Count; i++)
                {
                    Assert.Equal(expectedConfigurationNames[i], configurations[i].Title);

                    Assert.DoesNotThrow(() => { var value = configurations[i].GeneratorVersion; });
                    Assert.DoesNotThrow(() => { var value = configurations[i].Id; });
                    Assert.DoesNotThrow(() => { var value = configurations[i].Updated; });
                }
            }
        }

        [Trait("unit-test", "class ConfigurationStanza")]
        [Fact]
        async Task CanConstructConfigurationStanza()
        {
            var feed = await TestAtomFeed.ReadFeed(Path.Combine(TestAtomFeed.Directory, "ConfigurationStanza.GetAsync.xml"));

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                var expectedConfigurationSettingNames = new string[] 
                { 
                    "_name", 
                    "command", 
                    "hostname", 
                    "inline", 
                    "maxresults", 
                    "maxtime", 
                    "track_alert", 
                    "ttl" 
                };

                var configurationStanza = new Refactored.ConfigurationStanza(context, feed);

                Assert.Equal(expectedConfigurationSettingNames, from setting in configurationStanza select setting.Title);
                Assert.Equal(expectedConfigurationSettingNames.Length, configurationStanza.Count);

                for (int i = 0; i < configurationStanza.Count; i++)
                {
                    Assert.Equal(expectedConfigurationSettingNames[i], configurationStanza[i].Title);

                    Assert.DoesNotThrow(() => { var value = configurationStanza[i].Id; });
                    Assert.DoesNotThrow(() => { var value = configurationStanza[i].Links; });
                    Assert.DoesNotThrow(() => { var value = configurationStanza[i].Updated; });
                    Assert.DoesNotThrow(() => { var value = configurationStanza[i].Value; });
                }
            }
        }
    }
}
