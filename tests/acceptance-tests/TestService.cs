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

namespace Splunk.Sdk.UnitTesting
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Xunit;

    public class TestService : IUseFixture<AcceptanceTestingSetup>
    {
        [Trait("class", "Service")]
        [Fact]
        public void CanConstruct()
        {
            var service = new Service(new Context(Scheme.Https, "localhost", 8089), Namespace.Default);
            Assert.Equal(service.ToString(), "https://localhost:8089/services");
        }

        [Trait("class", "Service")]
        [Fact]
        public void CanLogin()
        {
            var service = new Service(new Context(Scheme.Https, "localhost", 8089), Namespace.Default);
            Task task;
            
            task = service.LoginAsync("admin", "changeme");
            task.Wait();

            Assert.Equal(task.Status, TaskStatus.RanToCompletion);
            Assert.NotNull(service.Context.SessionKey);

            try
            {
                task = service.LoginAsync("admin", "bad-password");
                task.Wait();
            }
            catch (Exception)
            {
                Assert.Equal(task.Status, TaskStatus.Faulted);
                Assert.IsType(typeof(AggregateException), task.Exception);

                var aggregateException = (AggregateException)task.Exception;
                Assert.Equal(aggregateException.InnerExceptions.Count, 1);
                Assert.IsType(typeof(RequestException), aggregateException.InnerExceptions[0]);

                var requestException = (RequestException)(aggregateException.InnerExceptions[0]);
                Assert.Equal(requestException.StatusCode, HttpStatusCode.Unauthorized);
                Assert.Equal(requestException.Details.Count, 1);
                Assert.Equal(requestException.Details[0], new Message(XElement.Parse(@"<msg type=""WARN"">Login failed</msg>")));
            }
        }

        [Trait("class", "Service")]
        [Fact]
        public void CanSearch()
        {
            Service service = new Service(new Context(Scheme.Https, "localhost", 8089), Namespace.Default);

            Task<Task<Job>> task = service.LoginAsync("admin", "changeme").ContinueWith(loginTask => service.SearchAsync("index=_internal | head 10"));
            task.Wait();
        }

        public void SetFixture(AcceptanceTestingSetup data)
        {  }
    }
}
