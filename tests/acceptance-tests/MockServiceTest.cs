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
    using Splunk.Sdk;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Xunit;
    using SplunkSDKHelper;
    public class MockServiceTest : IUseFixture<AcceptanceTestingSetup>
    {
        public void SetFixture(AcceptanceTestingSetup data)
        { }

        [Trait("class", "Service")]
        [Fact]
        public void MockeserviceTest1()
        {
            //var service = new Service(Scheme.Https, "localhost", 8089, Namespace.Default);

            Command command = Command.Splunk();
     
            MockContext context = new MockContext(Scheme.Https, command.Host, command.Port);
            var service = new Service(context);

            service.LoginAsync(command.Username, command.Password).Wait();

            try
            {
                AppCollection collection = service.GetAppsAsync().Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            Assert.Equal(service.ToString(), "https://localhost:8089/services");
        }

    }
}
