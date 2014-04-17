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

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using Xunit;

    public class TestJobCollectionArgs
    {
        [Trait("class", "Args")]
        [Fact]
        void CanConstruct()
        {
            string[] expectedString = new string[] {
                "count=30; offset=0; search=null; sort_dir=desc; sort_key=dispatch_time",
                "count=30; offset=0; search=some_unchecked_string; sort_dir=desc; sort_key=dispatch_time"
            };
            var expectedArguments = new List<Argument>[]
            {
                new List<Argument>() 
                { 
                },
                new List<Argument>() 
                { 
                    new Argument("search", "some_unchecked_string")
                }
            };

            JobCollectionArgs args;

            args = new JobCollectionArgs();
            Assert.Equal(expectedString[0], args.ToString());
            Assert.Equal(expectedArguments[0], args);
        }

        [Trait("class", "Args")]
        [Fact]
        void CanSetEveryValue()
        {
            var args = new JobCollectionArgs()
            {
                Count = 100,
                Offset = 100,
                Search = "some_unchecked_string",
                SortDirection = SortDirection.Ascending,
                SortKey = "some_unchecked_string"
            };

            Assert.Equal("count=100; offset=100; search=some_unchecked_string; sort_dir=asc; sort_key=some_unchecked_string", args.ToString());

            Assert.Equal(new List<Argument>()
                { 
                    new Argument("count", "100"),
                    new Argument("offset", "100"),
                    new Argument("search", "some_unchecked_string"),
                    new Argument("sort_dir", "asc"),
                    new Argument("sort_key", "some_unchecked_string"),
                },
                args);
        }
    }
}
