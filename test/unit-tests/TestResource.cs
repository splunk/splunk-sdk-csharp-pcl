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
        [Trait("unit-test", "class Entity")]
        [Fact]
        async Task CanConstructEntity()
        {
            var feed = await ReadAtomFeed(TestAtomFeed.Path);

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                dynamic entity = new Entity(context, feed);

                CheckCommonStaticProperties(entity);
                Assert.Equal("1392687998.313", entity.Name);
                Assert.NotNull(entity.Links);
                Assert.NotNull(entity.Messages);
                Assert.NotNull(entity.Resources);
                Assert.Equal(0, entity.Resources.Count);
                CheckDynamicProperties(entity);
            }
        }

        [Trait("unit-test", "class EntityCollection<Entity>")]
        [Fact]
        async Task CanConstructEntityCollectionOfEntity()
        {
            var feed = await ReadAtomFeed(TestAtomFeed.Path);

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                dynamic collection = new EntityCollection<Entity>(context, feed);

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
                CheckDynamicProperties(entity);
            }
        }

        [Trait("unit-test", "class EntityCollection<EntityCollection<Entity>")]
        [Fact]
        async Task CanConstructEntityCollectionOfEntityCollectionOfEntity()
        {
            var feed = await ReadAtomFeed(Path.Combine(TestAtomFeed.Directory, "ConfigurationCollection.GetAsync.xml"));

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                dynamic collection = new EntityCollection<EntityCollection<Entity>>(context, feed);

                Assert.DoesNotThrow(() => { var p = collection.Pagination; });
                CheckCommonStaticProperties(collection);
                Assert.Equal("properties", collection.Name);
                Assert.NotNull(collection.Links);
                Assert.NotNull(collection.Messages);
                Assert.Equal(83, collection.Count);

                foreach (var resource in collection)
                {
                    Assert.IsType(typeof(EntityCollection<Entity>), resource);
                    Assert.NotNull(resource.Links);
                    Assert.NotNull(resource.Messages);
                    Assert.NotNull(resource.Resources);
                    Assert.Equal(0, resource.Count);
                    Assert.Equal(resource.Pagination, Pagination.None);
                }
            }
        }

        [Trait("unit-test", "class EntityCollection<Resource>")]
        [Fact]
        async Task CanConstructEntityCollectionOfResource()
        {
            var feed = await ReadAtomFeed(TestAtomFeed.Path);

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
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
                CheckDynamicProperties(entity);
            }
        }

        [Trait("class", "Resource")]
        [Fact]
        async Task CanConstructResource()
        {
            var feed = await ReadAtomFeed(TestAtomFeed.Path);

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                dynamic collection = new Resource(context, feed);

                CheckCommonStaticProperties(collection);
                Assert.Equal("jobs", collection.Name);
                Assert.NotNull(collection.Links);
                Assert.NotNull(collection.Messages);
                Assert.NotNull(collection.Resources);
                Assert.Equal(1, collection.Resources.Count);

                dynamic entity = collection.Resources[0];

                CheckCommonStaticProperties(entity);
                Assert.Equal("1392687998.313", entity.Name);
                Assert.NotNull(entity.Links);
                Assert.NotNull(entity.Messages);
                Assert.NotNull(entity.Resources);
                Assert.Equal(0, entity.Resources.Count);
                CheckDynamicProperties(entity);
            }
        }

        #region Privates/internals

        void CheckDynamicProperties(dynamic job)
        {
            Assert.DoesNotThrow(() => { var p = job.CanSummarize; });
            Assert.DoesNotThrow(() => { var p = job.CursorTime; });
            Assert.DoesNotThrow(() => { var p = job.DefaultSaveTTL; });
            Assert.DoesNotThrow(() => { var p = job.DefaultTTL; });
            Assert.DoesNotThrow(() => { var p = job.DiskUsage; });
            Assert.DoesNotThrow(() => { var p = job.DispatchState; });
            Assert.DoesNotThrow(() => { var p = job.DoneProgress; });
            Assert.DoesNotThrow(() => { var p = job.DropCount; });
            Assert.DoesNotThrow(() => { var p = job.Eai; });
            Assert.DoesNotThrow(() => { var p = job.Eai.Acl; });
            Assert.DoesNotThrow(() => { var p = job.Eai.Acl.App; });
            Assert.DoesNotThrow(() => { var p = job.Eai.Acl.CanWrite; });
            Assert.DoesNotThrow(() => { var p = job.Eai.Acl.Modifiable; });
            Assert.DoesNotThrow(() => { var p = job.Eai.Acl.Owner; });
            Assert.DoesNotThrow(() => { var p = job.Eai.Acl.Perms; });
            Assert.DoesNotThrow(() => { var p = job.Eai.Acl.Perms.Read; });
            Assert.DoesNotThrow(() => { var p = job.Eai.Acl.Perms.Write; });
            Assert.DoesNotThrow(() => { var p = job.Eai.Acl.Sharing; });
            Assert.DoesNotThrow(() => { var p = job.Eai.Acl.Ttl; });
            Assert.DoesNotThrow(() => { var p = job.EarliestTime; });
            Assert.DoesNotThrow(() => { var p = job.EventAvailableCount; });
            Assert.DoesNotThrow(() => { var p = job.EventCount; });

            //// More...
        }

        void CheckCommonStaticProperties(Resource resource)
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
        }

        async Task<AtomFeed> ReadAtomFeed(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var reader = XmlReader.Create(stream, TestAtomFeed.XmlReaderSettings);
                var feed = new AtomFeed();

                await feed.ReadXmlAsync(reader);
                return feed;
            }
        }

        #endregion
    }
}
