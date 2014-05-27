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

    public class TestResourceEndpoint
    {
        [Trait("unit-test", "class Entity")]
        [Fact]
        async Task CanConstructEntity()
        {
            var feed = await TestAtomFeed.Read(TestAtomFeed.Path);

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                var entity = new Entity(context, feed);
                CheckJob(entity);
            }
        }

        [Trait("unit-test", "class EntityCollection<Entity>")]
        [Fact]
        async Task CanConstructEntityCollectionOfEntity()
        {
            var feed = await TestAtomFeed.Read(TestAtomFeed.Path);

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                //// EntityCollection<TEntity> checks

                var collection = new EntityCollection<Entity>(context, feed);
                CheckCommonStaticPropertiesOfResourceEndpoint(collection);

                Assert.Equal("https://localhost:8089/services/search/jobs", collection.Id.ToString());
                Assert.Equal("6.0.1.187445", collection.GeneratorVersion.ToString());
                Assert.Equal("jobs", collection.Name);
                Assert.Equal(1, collection.Count);

                CheckJob(collection[0]);
            }
        }

        [Trait("unit-test", "class EntityCollection<EntityCollection<Entity>")]
        [Fact]
        async Task CanConstructEntityCollectionOfEntityCollectionOfEntity()
        {
            var feed = await TestAtomFeed.Read(Path.Combine(TestAtomFeed.Directory, "ConfigurationCollection.GetAsync.xml"));

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                var collection = new EntityCollection<EntityCollection<Entity>>(context, feed);

                CheckCommonStaticPropertiesOfResourceEndpoint(collection);
                Assert.Equal("properties", collection.Name);
                Assert.Equal(83, collection.Count);

                foreach (EntityCollection<Entity> entity in collection)
                {
                    Assert.Equal(0, entity.Count);
                }
            }
        }

        #region Privates/internals

        static void CheckCommonStaticPropertiesOfResourceEndpoint(ResourceEndpoint resource)
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
                Assert.True(o.Title.Equals(resource.Title));
            });
            
            Assert.DoesNotThrow(() =>
            {
                dynamic o = resource; 
                Assert.True(o.Updated.Equals(resource.Updated)); 
            });
        }

        static void CheckJob(Entity job)
        {
            CheckCommonStaticPropertiesOfResourceEndpoint(job);

            Assert.Equal("6.0.1.187445", job.GeneratorVersion.ToString());
            Assert.Equal("2014-02-17 17:46:39Z", job.Updated.ToString("u"));
            Assert.Equal("1392687998.313", job.Name);
            Assert.Equal("search *", job.Title);
            Assert.Equal("admin", job.Resource.Author);
            Assert.NotNull(job.Resource.Links);
            Assert.Equal(new string[] { "alternate", "search.log", "events", "results", "results_preview", "timeline", "summary", "control" }, job.Resource.Links.Keys);

            TestResource.CheckExistenceOfDynamicPropertiesOfJobResource(job.Resource);
        }

        #endregion
    }
}
