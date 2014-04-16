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
                "blockSignSize=null; " +
                "bucketRebuildMemoryHint=null; " +
                "coldToFrozenDir=null; " +
                "coldToFrozenScript=null; " +
                "enableOnlineBucketRepair=null; " +
                "frozenTimePeriodInSecs=null; " +
                "maxBloomBackfillBucketAge=null; " +
                "maxConcurrentOptimizes=null; " +
                "maxDataSize=null; " +
                "maxHotBuckets=null; " +
                "maxHotIdleSecs=null; " +
                "maxHotSpanSecs=null; " +
                "maxMemMB=null; " +
                "maxMetaEntries=null; " +
                "maxTimeUnreplicatedNoAcks=null; " +
                "maxTimeUnreplicatedWithAcks=null; " +
                "maxTotalDataSizeMB=null; " +
                "maxWarmDBCount=null; " +
                "minRawFileSyncSecs=null; " +
                "minStreamGroupQueueSize=null; " +
                "partialServiceMetaPeriod=null; " +
                "processTrackerServiceInterval=null; " +
                "quarantineFutureSecs=null; " +
                "quarantinePastSecs=null; " +
                "rawChunkSizeBytes=null; " +
                "repFactor=null; " +
                "rotatePeriodInSecs=null; " +
                "serviceMetaPeriod=null; " +
                "syncMeta=null; " +
                "throttleCheckPeriod=null; " +
                "tstatsHomePath=null; " +
                "warmToColdScript=null",
                attributes.ToString());

            Assert.Equal(0, ((IEnumerable<Argument>)attributes).Count());
        }

        [Trait("class", "IndexAttributes")]
        [Fact]
        void CanSetEveryValue()
        {
            var defaultAttributes = new IndexAttributes()
            {
                BlockSignSize = 0,
                BucketRebuildMemoryHint = "auto",
                ColdToFrozenDir = "",
                ColdToFrozenScript = "",
                EnableOnlineBucketRepair = true,
                FrozenTimePeriodInSecs = 188697600,
                MaxBloomBackfillBucketAge = "30d",
                MaxConcurrentOptimizes = 6,
                MaxDataSize = "auto",
                MaxHotBuckets = 3,
                MaxHotIdleSecs = 0,
                MaxHotSpanSecs = 7776000,
                MaxMemMB = 5,
                MaxMetaEntries = 1000000,
                MaxTimeUnreplicatedNoAcks = 300,
                MaxTimeUnreplicatedWithAcks = 60,
                MaxTotalDataSizeMB = 500000,
                MaxWarmDBCount = 300,
                MinRawFileSyncSecs = "disable",
                MinStreamGroupQueueSize = 2000,
                PartialServiceMetaPeriod = 0,
                ProcessTrackerServiceInterval = 1,
                QuarantineFutureSecs = 2592000,
                QuarantinePastSecs = 77760000,
                RawChunkSizeBytes = 131072,
                RepFactor = "0",
                RotatePeriodInSecs = 60,
                ServiceMetaPeriod = 25,
                SyncMeta = true,
                ThrottleCheckPeriod = 15,
                TStatsHomePath = "",
                WarmToColdScript = "",
            };

            Assert.Equal(new List<Argument>()
                {
                    new Argument("blockSignSize", "0"),
                    new Argument("bucketRebuildMemoryHint", "auto"),
                    new Argument("coldToFrozenDir", ""),
                    new Argument("coldToFrozenScript", ""),
                    new Argument("enableOnlineBucketRepair", "t"),
                    new Argument("frozenTimePeriodInSecs", "188697600"),
                    new Argument("maxBloomBackfillBucketAge", "30d"),
                    new Argument("maxConcurrentOptimizes", "6"),
                    new Argument("maxDataSize", "auto"),
                    new Argument("maxHotBuckets", "3"),
                    new Argument("maxHotIdleSecs", "0"),
                    new Argument("maxHotSpanSecs", "7776000"),
                    new Argument("maxMemMB", "5"),
                    new Argument("maxMetaEntries", "1000000"),
                    new Argument("maxTimeUnreplicatedNoAcks", "300"),
                    new Argument("maxTimeUnreplicatedWithAcks", "60"),
                    new Argument("maxTotalDataSizeMB", "500000"),
                    new Argument("maxWarmDBCount", "300"),
                    new Argument("minRawFileSyncSecs", "disable"),
                    new Argument("minStreamGroupQueueSize", "2000"),
                    new Argument("partialServiceMetaPeriod", "0"),
                    new Argument("processTrackerServiceInterval", "1"),
                    new Argument("quarantineFutureSecs", "2592000"),
                    new Argument("quarantinePastSecs", "77760000"),
                    new Argument("rawChunkSizeBytes", "131072"),
                    new Argument("repFactor", "0"),
                    new Argument("rotatePeriodInSecs", "60"),
                    new Argument("serviceMetaPeriod", "25"),
                    new Argument("syncMeta", "t"),
                    new Argument("throttleCheckPeriod", "15"),
                    new Argument("tstatsHomePath", ""),
                    new Argument("warmToColdScript", ""),
                },
                defaultAttributes);

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
