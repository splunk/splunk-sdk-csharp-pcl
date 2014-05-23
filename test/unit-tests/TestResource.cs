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
    using Splunk.Client.Refactored;

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    using Xunit;

    public class TestResource
    {
        [Trait("unit-test", "class EntityCollection<Resource>")]
        [Fact]
        async Task CanConstructEntityCollection()
        {
            using (var stream = new FileStream(TestAtomFeed.Path, FileMode.Open, FileAccess.Read))
            {
                var reader = XmlReader.Create(stream, TestAtomFeed.XmlReaderSettings);
                var feed = new AtomFeed();

                await feed.ReadXmlAsync(reader);

                Context context = new Context(Scheme.Https, "localhost", 8089);

                Action<dynamic> CheckCommonStaticProperties = resource =>
                {
                    Assert.DoesNotThrow(() => { var p = resource.Author; });
                    Assert.DoesNotThrow(() => { var p = resource.Context; });
                    Assert.DoesNotThrow(() => { var p = resource.GeneratorVersion; });
                    Assert.DoesNotThrow(() => { var p = resource.Id; });
                    Assert.DoesNotThrow(() => { var p = resource.Links; });
                    Assert.DoesNotThrow(() => { var p = resource.Messages; });
                    Assert.DoesNotThrow(() => { var p = resource.Name; });
                    Assert.DoesNotThrow(() => { var p = resource.Namespace; });
                    Assert.DoesNotThrow(() => { var p = resource.Published; });
                    Assert.DoesNotThrow(() => { var p = resource.ResourceName; });
                    Assert.DoesNotThrow(() => { var p = resource.Resources; });
                    Assert.DoesNotThrow(() => { var p = resource.Updated; });
                };

                dynamic collection = new EntityCollection<Resource>(context, feed);
                Assert.DoesNotThrow(() => { var p = collection.Pagination; });
                CheckCommonStaticProperties(collection);
                Assert.Equal("jobs", collection.Name);
                Assert.NotNull(collection.Links);
                Assert.NotNull(collection.Messages);
                Assert.Equal(1, collection.Count);

                dynamic entity = collection.Resources[0];
                CheckCommonStaticProperties(entity);
                Assert.Equal("1392687998.313", entity.Name);
                Assert.NotNull(entity.Links);
                Assert.NotNull(entity.Messages);
                Assert.NotNull(entity.Resources);
                Assert.Equal(0, entity.Resources.Count);
                
                //// Check dynamic properties

                Assert.DoesNotThrow(() => { var p = entity.CanSummarize; });
                Assert.DoesNotThrow(() => { var p = entity.CursorTime; });
                Assert.DoesNotThrow(() => { var p = entity.DefaultSaveTTL; });
                Assert.DoesNotThrow(() => { var p = entity.DefaultTTL; });
                Assert.DoesNotThrow(() => { var p = entity.DiskUsage; });
                Assert.DoesNotThrow(() => { var p = entity.DispatchState; });
                Assert.DoesNotThrow(() => { var p = entity.DoneProgress; });
                Assert.DoesNotThrow(() => { var p = entity.DropCount; });
                Assert.DoesNotThrow(() => { var p = entity.Eai; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.App; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.CanWrite; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.Modifiable; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.Owner; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.Perms; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.Perms.Read; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.Perms.Write; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.Sharing; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.Ttl; });
                Assert.DoesNotThrow(() => { var p = entity.EarliestTime; });
                Assert.DoesNotThrow(() => { var p = entity.EventAvailableCount; });
                Assert.DoesNotThrow(() => { var p = entity.EventCount; });
                
                //// More...
            }
        }

        [Trait("class", "Resource")]
        [Fact]
        async Task CanConstructResource()
        {
            using (var stream = new FileStream(TestAtomFeed.Path, FileMode.Open, FileAccess.Read))
            {
                var reader = XmlReader.Create(stream, TestAtomFeed.XmlReaderSettings);
                var feed = new AtomFeed();

                await feed.ReadXmlAsync(reader);

                Context context = new Context(Scheme.Https, "localhost", 8089);

                Action<dynamic> CheckStaticProperties = resource =>
                {
                    Assert.DoesNotThrow(() => { var p = resource.Author; });
                    Assert.DoesNotThrow(() => { var p = resource.Context; });
                    Assert.DoesNotThrow(() => { var p = resource.GeneratorVersion; });
                    Assert.DoesNotThrow(() => { var p = resource.Id; });
                    Assert.DoesNotThrow(() => { var p = resource.Links; });
                    Assert.DoesNotThrow(() => { var p = resource.Messages; });
                    Assert.DoesNotThrow(() => { var p = resource.Name; });
                    Assert.DoesNotThrow(() => { var p = resource.Namespace; });
                    Assert.DoesNotThrow(() => { var p = resource.Published; });
                    Assert.DoesNotThrow(() => { var p = resource.ResourceName; });
                    Assert.DoesNotThrow(() => { var p = resource.Resources; });
                    Assert.DoesNotThrow(() => { var p = resource.Updated; });
                };

                dynamic collection = new Resource(context, feed);
                CheckStaticProperties(collection);
                Assert.Equal("jobs", collection.Name);
                Assert.NotNull(collection.Links);
                Assert.NotNull(collection.Messages);
                Assert.NotNull(collection.Resources);
                Assert.Equal(1, collection.Resources.Count);

                dynamic entity = collection.Resources[0];
                CheckStaticProperties(entity);
                Assert.Equal("1392687998.313", entity.Name);
                Assert.NotNull(entity.Links);
                Assert.NotNull(entity.Messages);
                Assert.NotNull(entity.Resources);
                Assert.Equal(0, entity.Resources.Count);

                //// Check dynamic properties

                Assert.DoesNotThrow(() => { var p = entity.CanSummarize; });
                Assert.DoesNotThrow(() => { var p = entity.CursorTime; });
                Assert.DoesNotThrow(() => { var p = entity.DefaultSaveTTL; });
                Assert.DoesNotThrow(() => { var p = entity.DefaultTTL; });
                Assert.DoesNotThrow(() => { var p = entity.DiskUsage; });
                Assert.DoesNotThrow(() => { var p = entity.DispatchState; });
                Assert.DoesNotThrow(() => { var p = entity.DoneProgress; });
                Assert.DoesNotThrow(() => { var p = entity.DropCount; });
                Assert.DoesNotThrow(() => { var p = entity.Eai; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.App; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.CanWrite; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.Modifiable; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.Owner; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.Perms; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.Perms.Read; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.Perms.Write; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.Sharing; });
                Assert.DoesNotThrow(() => { var p = entity.Eai.Acl.Ttl; });
                Assert.DoesNotThrow(() => { var p = entity.EarliestTime; });
                Assert.DoesNotThrow(() => { var p = entity.EventAvailableCount; });
                Assert.DoesNotThrow(() => { var p = entity.EventCount; });

                //// More...
            }
        }
    }
}
