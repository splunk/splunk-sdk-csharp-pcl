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
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.IO;
    using System.Threading.Tasks;

    using Xunit;

    public class TestResource
    {
        [Trait("unit-test", "Splunk.Client.Resource")]
        [Fact]
        async Task CanConstructResource()
        {
            var feed = await TestAtomFeed.ReadFeed(Path.Combine(TestAtomFeed.Directory, "JobCollection.GetAsync.xml"));

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                dynamic collection = new ResourceCollection<Resource>(feed);
                CheckCommonStaticPropertiesOfResource(collection);

                //// Static property checks

                Assert.Equal("jobs", collection.Title);
                Assert.Throws<RuntimeBinderException>(() => { var p = collection.Links; });
                Assert.Throws<RuntimeBinderException>(() => { var p = collection.Messages; });

                Assert.DoesNotThrow(() =>
                {
                    Pagination p = collection.Pagination;
                    Assert.Equal(14, p.TotalResults);
                    Assert.Equal(0, p.StartIndex);
                    Assert.Equal(0, p.ItemsPerPage);
                });

                Assert.DoesNotThrow(() =>
                {
                    ReadOnlyCollection<Resource> p = collection.Entries;
                    Assert.Equal(14, p.Count);
                });

                foreach (var resource in collection.Entries)
                {
                    CheckCommonStaticPropertiesOfResource(resource);
                    CheckExistenceOfJobProperties(resource);
                }

                {   dynamic resource = collection.Entries[0];

                    CheckCommonStaticPropertiesOfResource(resource);

                    Assert.IsType(typeof(Uri), resource.Id);
                    Assert.Equal("https://localhost:8089/services/search/jobs/scheduler__admin__search__RMD50aa4c13eb03d1730_at_1401390000_866", resource.Id.ToString());

                    Assert.IsType(typeof(string), resource.Content.Sid);
                    Assert.Equal("scheduler__admin__search__RMD50aa4c13eb03d1730_at_1401390000_866", resource.Content.Sid);

                    Assert.IsType(typeof(string), resource.Title);
                    Assert.Equal("search search index=_internal | head 1000", resource.Title);

                    Assert.NotNull(resource.Links);
                    Assert.IsType(typeof(ReadOnlyDictionary<string, Uri>), resource.Links);
                    Assert.Equal(new string[] { "alternate", "search.log", "events", "results", "results_preview", "timeline", "summary", "control" }, resource.Links.Keys);

                    Assert.Throws<RuntimeBinderException>(() => resource.Resources);

                    CheckExistenceOfJobProperties(resource);
                }
            }
        }

        #region Privates/internals

        static void CheckCommonStaticPropertiesOfResource(BaseResource resource)
        {
            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource;
                Assert.True(o.Id.Equals(resource.Id));
            });

            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource;
                Assert.True(o.Title.Equals(resource.Title));
            });

        }

        internal static void CheckExistenceOfApplicationProperties(dynamic entity)
        {
            Assert.DoesNotThrow(() => { var p = entity.Content.CheckForUpdates; });
            Assert.DoesNotThrow(() => { var p = entity.Content.Configured; });
            Assert.DoesNotThrow(() => { var p = entity.Content.Disabled; });
            Assert.DoesNotThrow(() => { var p = entity.Content.Eai; });
            Assert.DoesNotThrow(() => { var p = entity.Content.Label; });
            Assert.DoesNotThrow(() => { var p = entity.Content.StateChangeRequiresRestart; });
            Assert.DoesNotThrow(() => { var p = entity.Content.Visible; });
        }

        internal static void CheckExistenceOfJobProperties(dynamic job)
        {
            Assert.DoesNotThrow(() => { var p = job.Published; });
            Assert.DoesNotThrow(() => { var p = job.Content.CanSummarize; });
            Assert.DoesNotThrow(() => { var p = job.Content.CursorTime; });
            Assert.DoesNotThrow(() => { var p = job.Content.DefaultSaveTTL; });
            Assert.DoesNotThrow(() => { var p = job.Content.DefaultTTL; });
            Assert.DoesNotThrow(() => { var p = job.Content.DiskUsage; });
            Assert.DoesNotThrow(() => { var p = job.Content.DispatchState; });
            Assert.DoesNotThrow(() => { var p = job.Content.DoneProgress; });
            Assert.DoesNotThrow(() => { var p = job.Content.DropCount; });
            Assert.DoesNotThrow(() => { var p = job.Content.Eai.Acl; });
            Assert.DoesNotThrow(() => { var p = job.Content.Eai.Acl.App; });
            Assert.DoesNotThrow(() => { var p = job.Content.Eai.Acl.CanWrite; });
            Assert.DoesNotThrow(() => { var p = job.Content.Eai.Acl.Modifiable; });
            Assert.DoesNotThrow(() => { var p = job.Content.Eai.Acl.Owner; });
            Assert.DoesNotThrow(() => { var p = job.Content.Eai.Acl.Perms; });
            Assert.DoesNotThrow(() => { var p = job.Content.Eai.Acl.Perms.Read; });
            Assert.DoesNotThrow(() => { var p = job.Content.Eai.Acl.Perms.Write; });
            Assert.DoesNotThrow(() => { var p = job.Content.Eai.Acl.Sharing; });
            Assert.DoesNotThrow(() => { var p = job.Content.Eai.Acl.Ttl; });
            Assert.DoesNotThrow(() => { var p = job.Content.EarliestTime; });
            Assert.DoesNotThrow(() => { var p = job.Content.EventAvailableCount; });
            Assert.DoesNotThrow(() => { var p = job.Content.EventCount; });

            //// More...

            Assert.DoesNotThrow(() => { var p = job.Content.Messages; });

            //// More...
        }
        #endregion
    }
}
