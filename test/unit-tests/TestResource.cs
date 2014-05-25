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

                CheckCommonStaticPropertiesOfResource(entity);
                Assert.Equal("1392687998.313", entity.Name);
                Assert.NotNull(entity.Links);
                Assert.NotNull(entity.Messages);
                Assert.NotNull(entity.Resources);
                Assert.Equal(0, entity.Resources.Count);
                CheckDynamicPropertiesOfJob(entity);
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
                CheckCommonStaticPropertiesOfResource(collection);
                Assert.Equal("jobs", collection.Name);
                Assert.NotNull(collection.Links);
                Assert.NotNull(collection.Messages);
                Assert.Equal(1, collection.Count);

                dynamic entity = collection.Resources[0];
                
                CheckCommonStaticPropertiesOfResource(entity);
                Assert.Equal("1392687998.313", entity.Name);
                Assert.NotNull(entity.Links);
                Assert.NotNull(entity.Messages);
                Assert.NotNull(entity.Resources);
                Assert.Equal(0, entity.Resources.Count);
                CheckDynamicPropertiesOfJob(entity);
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
                CheckCommonStaticPropertiesOfResource(collection);
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

        [Trait("class", "Resource")]
        [Fact]
        async Task CanConstructResource()
        {
            var feed = await ReadAtomFeed(TestAtomFeed.Path);

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                dynamic collection = new Resource(feed);

                CheckCommonStaticPropertiesOfResource(collection);
                
                //// Static property checks

                Assert.Equal("jobs", collection.Title);

                Assert.NotNull(collection.Links);
                Assert.IsType(typeof(ReadOnlyDictionary<string, Uri>), collection.Links);
                Assert.Throws<RuntimeBinderException>(() => collection.Links["foobar"] = new Uri("/services/search/jobs/1392687998.313/foobar", UriKind.Relative));
                Assert.Equal(0, collection.Links.Count);

                //// Dynamic property checks
                
                Assert.DoesNotThrow(() => { var p = collection.Messages; } );
                Assert.IsType(typeof(ReadOnlyCollection<Message>), collection.Messages);
                Assert.Equal(0, collection.Messages.Count);

                Assert.DoesNotThrow(() => { var p = collection.Pagination; });
                Assert.IsType(typeof(Pagination), collection.Pagination);

                Assert.DoesNotThrow(() => { var p = collection.Resources; });
                Assert.IsType(typeof(ReadOnlyCollection<Resource>), collection.Resources);
                Assert.Equal(1, collection.Resources.Count);

                dynamic entity = collection.Resources[0];

                CheckCommonStaticPropertiesOfResource(entity);
                
                Assert.IsType(typeof(Uri), entity.Id);
                Assert.Equal("https://localhost:8089/services/search/jobs/1392687998.313", entity.Id.ToString());
                
                Assert.IsType(typeof(string), entity.Sid);
                Assert.Equal("1392687998.313", entity.Sid);

                Assert.IsType(typeof(string), entity.Title);
                Assert.Equal("search *", entity.Title);

                Assert.NotNull(entity.Links);
                Assert.IsType(typeof(ReadOnlyDictionary<string, Uri>), entity.Links);
                Assert.Equal(new string[] { "alternate", "search.log", "events", "results", "results_preview", "timeline", "summary", "control" }, entity.Links.Keys);

                Assert.Throws<RuntimeBinderException>(() => entity.Resources);
                
                CheckDynamicPropertiesOfJob(entity);
            }
        }

        #region Privates/internals

        void CheckDynamicPropertiesOfJob(dynamic job)
        {
            Assert.DoesNotThrow(() => { var p = job.Published; });
            Assert.IsType(typeof(DateTime), job.Published);
            Assert.Equal("2014-02-17 17:46:39Z", job.Published.ToString("u"));
            Assert.DoesNotThrow(() => { var p = job.CanSummarize; });
            Assert.DoesNotThrow(() => { var p = job.CursorTime; });
            Assert.DoesNotThrow(() => { var p = job.DefaultSaveTTL; });
            Assert.DoesNotThrow(() => { var p = job.DefaultTTL; });
            Assert.DoesNotThrow(() => { var p = job.DiskUsage; });
            Assert.DoesNotThrow(() => { var p = job.DispatchState; });
            Assert.DoesNotThrow(() => { var p = job.DoneProgress; });
            Assert.DoesNotThrow(() => { var p = job.DropCount; });
            Assert.DoesNotThrow(() => { Assert.IsType(typeof(ExpandoObject), job.Eai); });
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

            Assert.DoesNotThrow(() => { Assert.IsType(typeof(ExpandoObject), job.Messages); });

            //// More...
        }

        void CheckCommonStaticPropertiesOfResource(Resource resource)
        {
            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource;
                Assert.True(o.Author.Equals(resource.Author));
            });

            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource;
                Assert.True(o.GeneratorVersion.Equals(resource.GeneratorVersion));
            });

            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource;
                Assert.True(o.Id.Equals(resource.Id));
            });

            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource;
                Assert.True(o.Links.Equals(resource.Links));
            });

            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource;
                Assert.True(o.Title.Equals(resource.Title));
            });

            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource;
                Assert.True(o.Updated.Equals(resource.Updated));
            });
        }

        void CheckCommonStaticPropertiesOfResourceEndpoint(ResourceEndpoint resource)
        {
            Assert.DoesNotThrow(() => 
            { 
                dynamic o = resource; 
                Assert.True(o.Context.Equals(resource.Context));
            });

            Assert.DoesNotThrow(() => 
            { 
                dynamic o = resource; 
                Assert.True(o.Namespace.Equals(resource.Namespace)); 
            });

            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource; 
                Assert.True(o.ResourceName.Equals(resource.ResourceName));
            });

            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource; 
                Assert.True(o.Author.Equals(resource.Author));
            });
            
            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource; 
                Assert.True(o.GeneratorVersion.Equals(resource.GeneratorVersion));
            });
            
            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource; 
                Assert.True(o.Id.Equals(resource.Id));
            });

            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource; 
                Assert.True(o.Links.Equals(resource.Links));
            });

            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource; 
                Assert.True(o.Title.Equals(resource.Title));
            });
            
            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource; 
                Assert.True(o.Updated.Equals(resource.Updated)); 
            });
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
