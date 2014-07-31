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

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    using Xunit;

    public class TestServer
    {
        [Trait("unit-test", "Splunk.Client.Server")]
        [Fact]
        void CanConstructServer()
        {
            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                var server = new Server(context, Namespace.Default);

                Assert.Equal(Pagination.None, server.Messages.Pagination);
                Assert.Equal(0, server.Messages.Count);
            }
        }

        [Trait("unit-test", "Splunk.Client.ServerInfo")]
        [Fact]
        async Task CanConstructServerInfo()
        {
            var feed = await TestAtomFeed.ReadFeed(Path.Combine(TestAtomFeed.Directory, "ServerInfo.GetAsync.xml"));

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                var serverInfo = new ServerInfo(feed);

                //// TODO: Match contents of feed to contents of resource; in this case serverInfo

                Assert.DoesNotThrow(() =>
                {
                    bool canList = serverInfo.Eai.Acl.CanList;
                    string app = serverInfo.Eai.Acl.App;
                    dynamic eai = serverInfo.Eai;
                    Assert.Equal(app, eai.Acl.App);
                    Assert.Equal(canList, eai.Acl.CanList);
                });
            }
        }

        [Trait("unit-test", "Splunk.Client.ServerSettings")]
        [Fact]
        async Task CanConstructServerSettings()
        {
            var feed = await TestAtomFeed.ReadFeed(Path.Combine(TestAtomFeed.Directory, "ServerSettings.GetAsync.xml"));

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                var serverSettings = new ServerSettings(feed);

                //// TODO: Match contents of feed to contents of resource; in this case serverInfo

                Assert.DoesNotThrow(() =>
                {
                    bool canList = serverSettings.Eai.Acl.CanList;
                    string app = serverSettings.Eai.Acl.App;
                    dynamic eai = serverSettings.Eai;
                    Assert.Equal(app, eai.Acl.App);
                    Assert.Equal(canList, eai.Acl.CanList);
                });
            }
        }
    }
}
