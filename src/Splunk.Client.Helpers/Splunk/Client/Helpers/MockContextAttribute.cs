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

namespace Splunk.Client.Helpers
{
    using Splunk.Client;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides a class for faking HTTP requests and responses from a Splunk server.
    /// </summary>
    public class MockContextAttribute : BeforeAfterTestAttribute
    {
        #region Methods

        public override void After(MethodInfo methodUnderTest)
        {
            MockContext.End();
        }

        public override void Before(MethodInfo methodUnderTest)
        {
            MockContext.Begin(string.Join(".", methodUnderTest.ReflectedType.FullName, methodUnderTest.Name));
        }

        #endregion
    }
}
