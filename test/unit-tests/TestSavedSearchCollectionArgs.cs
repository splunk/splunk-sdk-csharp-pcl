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

    public class TestSavedSearchCollectionArgs
    {
        [Trait("class", "SavedSearchCollectionArgs")]
        [Fact]
        void CanConstruct()
        {
            string[] expectedString = new string[] {
                "count=30; earliest_time=null; latest_time=null; listDefaultActionArgs=f; offset=0; search=null; sort_dir=asc; sort_key=name; sort_mode=auto",
                "count=30; earliest_time=null; latest_time=null; listDefaultActionArgs=f; offset=0; search=some_unchecked_string; sort_dir=asc; sort_key=name; sort_mode=auto",
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

            SavedSearchCollectionArgs args;

            args = new SavedSearchCollectionArgs();
            Assert.Equal(expectedString[0], args.ToString());
            Assert.Equal(expectedArguments[0], args);
        }

        [Trait("class", "SavedSearchCollectionArgs")]
        [Fact]
        void CanSetEveryValue()
        {
            var args = new SavedSearchCollectionArgs()
            {
                Count = 100,
                EarliestTime = "some_unchecked_string",
                LatestTime = "some_unchecked_string",
                ListDefaultActions = true,
                Offset = 100,
                Search = "some_unchecked_string",
                SortDirection = SortDirection.Descending,
                SortKey = "some_unchecked_string",
                SortMode = SortMode.Alphabetic
            };

            Assert.Equal("count=100; earliest_time=some_unchecked_string; latest_time=some_unchecked_string; listDefaultActionArgs=t; offset=100; search=some_unchecked_string; sort_dir=desc; sort_key=some_unchecked_string; sort_mode=alpha", args.ToString());

            var list = new List<Argument>()
            { 
                new Argument("count", "100"),
                new Argument("earliest_time", "some_unchecked_string"),
                new Argument("latest_time", "some_unchecked_string"),
                new Argument("listDefaultActionArgs", "t"),
                new Argument("offset", "100"),
                new Argument("search", "some_unchecked_string"),
                new Argument("sort_dir", "desc"),
                new Argument("sort_key", "some_unchecked_string"),
                new Argument("sort_mode", "alpha")
            };

            Assert.Equal(list, args);
        }
    }
}
