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

    public class TestIndexAttributes
    {
        [Trait("class", "IndexAttributes")]
        [Fact]
        void CanConstruct()
        {
            var attributes = new IndexAttributes();

            Assert.DoesNotThrow(() => attributes.ToArray());

            Assert.Equal(
                "blockSignSize=0; " +
                "bucketRebuildMemoryHint=auto; " +
                "coldToFrozenDir=null; " +
                "coldToFrozenScript=null; " +
                "enableOnlineBucketRepair=t; " +
                "frozenTimePeriodInSecs=188697600; " +
                "maxBloomBackfillBucketAge=30d; " +
                "maxConcurrentOptimizes=6; " +
                "maxDataSize=auto; " +
                "maxHotBuckets=3; " +
                "maxHotIdleSecs=0; " +
                "maxHotSpanSecs=7776000; " +
                "maxMemMB=5; " +
                "maxMetaEntries=1000000; " +
                "maxTimeUnreplicatedNoAcks=300; " +
                "maxTimeUnreplicatedWithAcks=60; " +
                "maxTotalDataSizeMB=500000; " +
                "maxWarmDBCount=300; " +
                "minRawFileSyncSecs=disable; " +
                "minStreamGroupQueueSize=2000; " +
                "partialServiceMetaPeriod=0; " +
                "processTrackerServiceInterval=1; " +
                "quarantineFutureSecs=2592000; " +
                "quarantinePastSecs=77760000; " +
                "rawChunkSizeBytes=131072; " +
                "repFactor=0; " +
                "rotatePeriodInSecs=60; " +
                "serviceMetaPeriod=25; " +
                "syncMeta=t; " +
                "throttleCheckPeriod=15; " +
                "tstatsHomePath=null; " +
                "warmToColdScript=null",
                attributes.ToString());

            Assert.Equal(0, ((IEnumerable<Argument>)attributes).Count());
        }

        [Trait("class", "IndexAttributes")]
        [Fact]
        void CanSetEveryValue()
        {
            var attributes = new IndexAttributes()
            {
                BlockSignSize = 1 + 0,
                BucketRebuildMemoryHint = "some_uncheck_string_value",
                ColdToFrozenDir = "some_uncheck_string_value",
                ColdToFrozenScript = "some_uncheck_string_value",
                EnableOnlineBucketRepair = false,
                FrozenTimePeriodInSecs = int.MaxValue,
                MaxBloomBackfillBucketAge = "some_uncheck_string_value",
                MaxConcurrentOptimizes = 2,
                MaxDataSize = "some_uncheck_string_value",
                MaxHotBuckets = 3 * 3,
                MaxHotIdleSecs = 4,
                MaxHotSpanSecs = 7776000 * 5,
                MaxMemMB = 6,
                MaxMetaEntries = 7 * 1000000,
                MaxTimeUnreplicatedNoAcks = 8 * 300,
                MaxTimeUnreplicatedWithAcks = 9 * 60,
                MaxTotalDataSizeMB = 10 * 500000,
                MaxWarmDBCount = 11 * 300,
                MinRawFileSyncSecs = "some_uncheck_string_value",
                MinStreamGroupQueueSize = 12 * 2000,
                PartialServiceMetaPeriod = 13 + 0,
                ProcessTrackerServiceInterval = 14 + 1,
                QuarantineFutureSecs = 15 + 2592000,
                QuarantinePastSecs = 16 + 77760000,
                RawChunkSizeBytes = 17 + 131072,
                RepFactor = "some_uncheck_string_value",
                RotatePeriodInSecs = 19 + 60,
                ServiceMetaPeriod = 20 + 25,
                SyncMeta = false,
                ThrottleCheckPeriod = 21 + 15,
                TStatsHomePath = "some_uncheck_string_value",
                WarmToColdScript = "some_uncheck_string_value",
            };

            Assert.Equal(
                "blockSignSize=1; " +
                "bucketRebuildMemoryHint=some_uncheck_string_value; " +
                "coldToFrozenDir=some_uncheck_string_value; " +
                "coldToFrozenScript=some_uncheck_string_value; " +
                "enableOnlineBucketRepair=f; " +
                "frozenTimePeriodInSecs=2147483647; " +
                "maxBloomBackfillBucketAge=some_uncheck_string_value; " +
                "maxConcurrentOptimizes=2; " +
                "maxDataSize=some_uncheck_string_value; " +
                "maxHotBuckets=9; " +
                "maxHotIdleSecs=4; " +
                "maxHotSpanSecs=38880000; " +
                "maxMemMB=6; " +
                "maxMetaEntries=7000000; " +
                "maxTimeUnreplicatedNoAcks=2400; " +
                "maxTimeUnreplicatedWithAcks=540; " +
                "maxTotalDataSizeMB=5000000; " +
                "maxWarmDBCount=3300; " +
                "minRawFileSyncSecs=some_uncheck_string_value; " +
                "minStreamGroupQueueSize=24000; " +
                "partialServiceMetaPeriod=13; " +
                "processTrackerServiceInterval=15; " +
                "quarantineFutureSecs=2592015; " +
                "quarantinePastSecs=77760016; " +
                "rawChunkSizeBytes=131089; " +
                "repFactor=some_uncheck_string_value; " +
                "rotatePeriodInSecs=79; " +
                "serviceMetaPeriod=45; " +
                "syncMeta=f; " +
                "throttleCheckPeriod=36; " +
                "tstatsHomePath=some_uncheck_string_value; " +
                "warmToColdScript=some_uncheck_string_value",
                attributes.ToString());

            Assert.Equal(new List<Argument>()
                {
                    new Argument("blockSignSize", "1"),
                    new Argument("bucketRebuildMemoryHint", "some_uncheck_string_value"),
                    new Argument("coldToFrozenDir", "some_uncheck_string_value"),
                    new Argument("coldToFrozenScript", "some_uncheck_string_value"),
                    new Argument("enableOnlineBucketRepair", "f"),
                    new Argument("frozenTimePeriodInSecs", "2147483647"),
                    new Argument("maxBloomBackfillBucketAge", "some_uncheck_string_value"),
                    new Argument("maxConcurrentOptimizes", "2"),
                    new Argument("maxDataSize", "some_uncheck_string_value"),
                    new Argument("maxHotBuckets", "9"),
                    new Argument("maxHotIdleSecs", "4"),
                    new Argument("maxHotSpanSecs", "38880000"),
                    new Argument("maxMemMB", "6"),
                    new Argument("maxMetaEntries", "7000000"),
                    new Argument("maxTimeUnreplicatedNoAcks", "2400"),
                    new Argument("maxTimeUnreplicatedWithAcks", "540"),
                    new Argument("maxTotalDataSizeMB", "5000000"),
                    new Argument("maxWarmDBCount", "3300"),
                    new Argument("minRawFileSyncSecs", "some_uncheck_string_value"),
                    new Argument("minStreamGroupQueueSize", "24000"),
                    new Argument("partialServiceMetaPeriod", "13"),
                    new Argument("processTrackerServiceInterval", "15"),
                    new Argument("quarantineFutureSecs", "2592015"),
                    new Argument("quarantinePastSecs", "77760016"),
                    new Argument("rawChunkSizeBytes", "131089"),
                    new Argument("repFactor", "some_uncheck_string_value"),
                    new Argument("rotatePeriodInSecs", "79"),
                    new Argument("serviceMetaPeriod", "45"),
                    new Argument("syncMeta", "f"),
                    new Argument("throttleCheckPeriod", "36"),
                    new Argument("tstatsHomePath", "some_uncheck_string_value"),
                    new Argument("warmToColdScript", "some_uncheck_string_value")
                },
                attributes);
        }
    }
}
