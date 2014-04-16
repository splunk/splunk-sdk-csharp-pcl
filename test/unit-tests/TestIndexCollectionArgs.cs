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

    public class TestIndexCollectionArgs
    {
        [Trait("class", "IndexCollectionArgs")]
        [Fact]
        void CanConstruct()
        {
            // Checked against http://docs.splunk.com/Documentation/Splunk/latest/RESTAPI/RESTindex#GET_data.2Findexes

            IndexCollectionArgs args = new IndexCollectionArgs();
            Assert.Equal("count=30; offset=0; search=null; sort_dir=asc; sort_key=name; sort_mode=auto; summarize=f", args.ToString());
            Assert.Equal(0, ((IEnumerable<Argument>)args).Count());
        }

        [Trait("class", "IndexCollectionArgs")]
        [Fact]
        void CanSetEveryValue()
        {
            var args = new IndexCollectionArgs()
            {
                Count = 100,
                Offset = 100,
                Search = "some_unchecked_string",
                SortDirection = SortDirection.Descending,
                SortKey = "some_unchecked_string",
                SortMode = SortMode.Alphabetic,
                Summarize = true
            };

            Assert.Equal("count=100; offset=100; search=some_unchecked_string; sort_dir=desc; sort_key=some_unchecked_string; sort_mode=alpha; summarize=t", args.ToString());

            Assert.Equal(new List<Argument>()
                { 
                    new Argument("count", "100"),
                    new Argument("offset", "100"),
                    new Argument("search", "some_unchecked_string"),
                    new Argument("sort_dir", "desc"),
                    new Argument("sort_key", "some_unchecked_string"),
                    new Argument("sort_mode", "alpha"),
                    new Argument("summarize", "t")
                },
                args);
        }
    }
}
