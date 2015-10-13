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

using Splunk.Client.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Splunk.Client.UnitTests
{
    public class TestService
    {

        [Trait("unit-test", "Splunk.Client.Context")]
        [Fact]
        public async Task TestContextThrowsOnForbidden()
        {
            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                await SdkHelper.ThrowsAsync<AuthenticationFailureException>(async () =>
                {
                    var response = await context.GetAsync(new Namespace("nobody", "search"), new ResourceName(new[] { "search", "jobs" }));
                    await response.EnsureStatusCodeAsync(System.Net.HttpStatusCode.Forbidden);
                });
            }
        }

        [Trait("unit-test", "Splunk.Client.Context")]
        [Fact]
        public async Task TestContextThrowsOn404()
        {
            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                await SdkHelper.ThrowsAsync<ResourceNotFoundException>(async () =>
                {
                    using (var service = new Service(context))
                    {
                        await service.LogOnAsync(SdkHelper.Splunk.Username, SdkHelper.Splunk.Password);
                        var response = await service.Context.GetAsync(new Namespace("admin", "search"), new ResourceName(new[] { "abc", "def", "ghi" }));
                        await response.EnsureStatusCodeAsync(System.Net.HttpStatusCode.NotFound);
                    }
                });
            }
        }

        [Trait("unit-test", "Splunk.Client.Service")]
        [Fact]
        public void ServiceCanBeConstructedWithAUri()
        {
            var uri = new Uri("https://localhost:8000");
            var service = new Service(uri);
            var context = service.Context;
            Assert.Equal<string>("localhost", context.Host);
            Assert.Equal<int>(8000, context.Port);
            Assert.Equal<Scheme>(Scheme.Https, context.Scheme);
        }
    }
}
