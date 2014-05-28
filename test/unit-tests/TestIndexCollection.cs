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

    public class TestIndexCollection
    {
        [Trait("unit-test", "class Index")]
        [Fact]
        async Task CanConstructIndex()
        {
            var feed = await TestAtomFeed.Read(Path.Combine(TestAtomFeed.Directory, "Index.GetAsync.xml"));

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                var index = new Refactored.Index(context, feed);
                CheckCommonProperties("_audit", index);
                
                Assert.DoesNotThrow(() =>
                {
                    bool canList = index.Eai.Acl.CanList;
                    string app = index.Eai.Acl.App;
                    dynamic eai = index.Eai;

                    Assert.Equal(canList, eai.Acl.CanList);
                    Assert.Equal(app, eai.Acl.App);
                });
            }
        }

        [Trait("unit-test", "class IndexCollection")]
        [Fact]
        async Task CanConstructIndexCollection()
        {
            var feed = await TestAtomFeed.Read(Path.Combine(TestAtomFeed.Directory, "IndexCollection.GetAsync.xml"));

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                var expectedNames = new string[] 
                { 
                    "_audit",
                    "_blocksignature",
                    "_internal",
                    "_thefishbucket",
                    "history",
                    "main",
                    "splunklogger",
                    "summary"
                };

                var indexes = new Refactored.ConfigurationCollection(context, feed);

                Assert.Equal(expectedNames, from index in indexes select index.Title);
                Assert.Equal(expectedNames.Length, indexes.Count);
                CheckCommonProperties("indexes", indexes);

                for (int i = 0; i < indexes.Count; i++)
                {
                    CheckCommonProperties(expectedNames[i], indexes[i]);
                }
            }
        }

        void CheckCommonProperties(string expectedName, ResourceEndpoint resourceEndpoint)
        {
            Assert.Equal(expectedName, resourceEndpoint.Title);

            //// Properties common to all resources

            Assert.DoesNotThrow(() =>
            {
                Version value = resourceEndpoint.GeneratorVersion;
                Assert.NotNull(value);
            });

            Assert.DoesNotThrow(() =>
            {
                Uri value = resourceEndpoint.Id;
                Assert.NotNull(value);
            });

            Assert.DoesNotThrow(() =>
            {
                string value = resourceEndpoint.Title;
                Assert.NotNull(value);
            });

            Assert.DoesNotThrow(() =>
            {
                DateTime value = resourceEndpoint.Updated;
                Assert.NotEqual(DateTime.MinValue, value);
            });
        }
    }
}
