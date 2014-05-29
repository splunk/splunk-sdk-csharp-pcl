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

    public class TestStoragePasswordCollection
    {
        [Trait("unit-test", "class StoragePassword")]
        [Fact]
        async Task CanConstructStoragePassword()
        {
            var feed = await TestAtomFeed.Read(Path.Combine(TestAtomFeed.Directory, "StoragePassword.GetAsync.xml"));

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                var storagePassword = new Refactored.Index(context, feed);
                CheckCommonProperties("_audit", storagePassword);
                
                Assert.DoesNotThrow(() =>
                {
                    bool canList = storagePassword.Eai.Acl.CanList;
                    string app = storagePassword.Eai.Acl.App;
                    dynamic eai = storagePassword.Eai;
                    Assert.Equal(app, eai.Acl.App);
                    Assert.Equal(canList, eai.Acl.CanList);
                });
            }
        }

        [Trait("unit-test", "class StoragePasswordCollection")]
        [Fact]
        async Task CanConstructStoragePasswordCollection()
        {
            var feed = await TestAtomFeed.Read(Path.Combine(TestAtomFeed.Directory, "StoragePasswordCollection.GetAsync.xml"));

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

                var storagePasswords = new Refactored.StoragePasswordCollection(context, feed);

                Assert.Equal(expectedNames, from index in storagePasswords select index.Title);
                Assert.Equal(expectedNames.Length, storagePasswords.Count);
                CheckCommonProperties("indexes", storagePasswords);

                for (int i = 0; i < storagePasswords.Count; i++)
                {
                    CheckCommonProperties(expectedNames[i], storagePasswords[i]);
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
