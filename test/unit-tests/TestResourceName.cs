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

    public class TestResourceName
    {
        [Trait("class", "ResourceName")]
        [Fact]
        void CanConstruct()
        {
            ResourceName resourceName;

            resourceName = new ResourceName("fu", "man", "chu");
            Assert.Equal("fu/man/chu", resourceName.ToString());

            resourceName = new ResourceName(resourceName, "too");
            Assert.Equal("fu/man/chu/too", resourceName.ToString());

            Assert.Throws(typeof(ArgumentException), () => new ResourceName("fu", null));
            Assert.Throws(typeof(ArgumentNullException), () => new ResourceName((string[])null));
            Assert.Throws(typeof(ArgumentException), () => new ResourceName(resourceName, "fu", null, "chu"));
            Assert.Throws(typeof(ArgumentNullException), () => new ResourceName((ResourceName)null, "fu", "man", "chu"));
        }

        [Trait("class", "JobArgs")]
        [Fact]
        void CanCompare()
        {
            var resourceNames = new ResourceName[] 
            {
                new ResourceName("1", "2", "3"),
                new ResourceName("1", "2", "4"),
                new ResourceName("1", "3", "3"),
                new ResourceName("2", "2", "3"),
                new ResourceName("4", "5", "6"),
                new ResourceName("1", "2", "3", "4"),
                new ResourceName("1", "2")
            };

            Assert.True(resourceNames[0].Equals((object)resourceNames[0]));
            Assert.True(resourceNames[0].Equals(resourceNames[0]));
            Assert.False(resourceNames[0].Equals(resourceNames[1]));
            Assert.False(resourceNames[0].Equals(resourceNames[2]));
            Assert.False(resourceNames[0].Equals(resourceNames[3]));
            Assert.False(resourceNames[0].Equals(resourceNames[4]));
            Assert.False(resourceNames[0].Equals(resourceNames[5]));
            Assert.False(resourceNames[0].Equals(resourceNames[6]));

            Assert.True(resourceNames[0].CompareTo((object)resourceNames[0]) == 0);
            Assert.True(resourceNames[0].CompareTo(resourceNames[0]) == 0);
            
            Assert.True(resourceNames[0].CompareTo(resourceNames[1]) < 0);
            Assert.True(resourceNames[0].CompareTo(resourceNames[2]) < 0);
            Assert.True(resourceNames[0].CompareTo(resourceNames[3]) < 0);
            Assert.True(resourceNames[0].CompareTo(resourceNames[4]) < 0);
            Assert.True(resourceNames[0].CompareTo(resourceNames[5]) < 0);
            Assert.True(resourceNames[6].CompareTo(resourceNames[0]) < 0);
            
            Assert.True(resourceNames[1].CompareTo(resourceNames[0]) > 0);
            Assert.True(resourceNames[2].CompareTo(resourceNames[0]) > 0);
            Assert.True(resourceNames[3].CompareTo(resourceNames[0]) > 0);
            Assert.True(resourceNames[4].CompareTo(resourceNames[0]) > 0);
            Assert.True(resourceNames[5].CompareTo(resourceNames[0]) > 0);
            Assert.True(resourceNames[0].CompareTo(resourceNames[6]) > 0);

            Assert.True(resourceNames[0].CompareTo((ResourceName)null) > 0);
            Assert.True(resourceNames[0].CompareTo(new object()) > 0);
            Assert.True(resourceNames[0].CompareTo(null) > 0);
        }
    }
}
