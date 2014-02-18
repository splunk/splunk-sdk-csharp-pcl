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

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Security;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Splunk.Sdk;

    [TestClass]
    public class TestContext
    {
        static TestContext()
        {
            // TODO: Use WebRequestHandler.ServerCertificateValidationCallback instead
            // 1. Instantiate a WebRequestHandler
            // 2. Set its ServerCertificateValidationCallback
            // 3. Instantiate a Splunk.Sdk.Context with the WebRequestHandler

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };
        }

        [TestMethod]
        public void Construct()
        {
            client = new Context(Protocol.Https, "localhost", 8089);

            Assert.AreEqual(client.Protocol, Protocol.Https);
            Assert.AreEqual(client.Host, "localhost");
            Assert.AreEqual(client.Port, 8089);
            Assert.IsNull(client.SessionKey);

            Assert.AreEqual(client.ToString(), "https://localhost:8089");
        }

        [TestMethod]
        public void Login()
        {
            Task task;

            task = client.Login("admin", "changeme");
            task.Wait();

            Assert.AreEqual(task.Status, TaskStatus.RanToCompletion);
            Assert.IsNotNull(client.SessionKey);

            task = client.Login("admin", "bad-password");

            try
            {
                task.Wait();
            }
            catch (Exception)
            {
                Assert.AreEqual(task.Status, TaskStatus.Faulted);
                Assert.IsInstanceOfType(task.Exception, typeof(AggregateException));

                var aggregateException = (AggregateException)task.Exception;
                Assert.AreEqual(aggregateException.InnerExceptions.Count, 1);
                Assert.IsInstanceOfType(aggregateException.InnerExceptions[0], typeof(RequestException));
                
                var requestException = (RequestException)(aggregateException.InnerExceptions[0]);
                Assert.AreEqual(requestException.StatusCode, HttpStatusCode.Unauthorized);
                Assert.AreEqual(requestException.Details.Count, 1);
                Assert.AreEqual(requestException.Details[0], new Message(XElement.Parse(@"<msg type=""WARN"">Login failed</msg>")));
            }
        }

        static Context client;
    }
}
