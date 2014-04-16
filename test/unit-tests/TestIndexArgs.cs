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
    using System.Linq;
    using System.Runtime.Serialization;

    using Xunit;

    public class TestIndexArgs
    {
        [Trait("class", "IndexArgs")]
        [Fact]
        void CanConstruct()
        {
            var args = new IndexArgs("coldPath", "homePath", "thawedPath");
            Assert.DoesNotThrow(() => args.ToArray());

            Assert.Equal("coldPath=coldPath; homePath=homePath; thawedPath=thawedPath", args.ToString());

            Assert.Equal(new Argument[] 
                {
                    new Argument("coldPath", "coldPath"),
                    new Argument("homePath", "homePath"),
                    new Argument("thawedPath", "thawedPath")
                },
                args);
        }
    }
}
