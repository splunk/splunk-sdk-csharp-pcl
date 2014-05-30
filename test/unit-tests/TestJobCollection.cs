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

namespace Splunk.Client.UnitTests
{
    using Microsoft.CSharp.RuntimeBinder;

    using Splunk.Client;
    using Splunk.Client.Refactored;
    using Job = Splunk.Client.Refactored.Job;

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    using Xunit;

    public class TestJobCollection
    {
        [Trait("unit-test", "class Job")]
        [Fact]
        async Task CanConstructJob()
        {
            var entry = await TestAtomFeed.ReadEntry(Path.Combine(TestAtomFeed.Directory, "Job.GetAsync.xml"));

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                var job = new Job(context, entry);
                CheckJob(entry, job);
            }
        }

        [Trait("unit-test", "class JobCollection")]
        [Fact]
        async Task CanConstructJobCollection()
        {
            var feed = await TestAtomFeed.ReadFeed(Path.Combine(TestAtomFeed.Directory, "JobCollection.GetAsync.xml"));

            using (var context = new Context(Scheme.Https, "localhost", 8089))
            {
                var expectedNames = new string[]
                {
                    "scheduler__admin__search__RMD50aa4c13eb03d1730_at_1401390000_866",
                    "scheduler__admin__search__RMD54d0063ad31759fca_at_1401390000_867",
                    "scheduler__admin__search__RMD581bb7159c0bb0bbb_at_1401390000_862",
                    "scheduler__admin__search__RMD58306e622619a7dcd_at_1401390000_863",
                    "scheduler__admin__search__RMD5a7321db12d7631bf_at_1401390000_865",
                    "scheduler__admin__search__RMD5e658dbdf77ae86f8_at_1401390000_864",
                    "scheduler__admin__search__RMD5f8965a6a2fa31c5d_at_1401390000_861",
                    "scheduler__admin__search__RMD50aa4c13eb03d1730_at_1401386400_859",
                    "scheduler__admin__search__RMD54d0063ad31759fca_at_1401386400_860",
                    "scheduler__admin__search__RMD581bb7159c0bb0bbb_at_1401386400_855",
                    "scheduler__admin__search__RMD58306e622619a7dcd_at_1401386400_856",
                    "scheduler__admin__search__RMD5a7321db12d7631bf_at_1401386400_858",
                    "scheduler__admin__search__RMD5e658dbdf77ae86f8_at_1401386400_857",
                    "scheduler__admin__search__RMD5f8965a6a2fa31c5d_at_1401386400_854"
                };

                var jobs = new Refactored.JobCollection(context, feed);

                Assert.Equal(expectedNames.Length, jobs.Count);
                var names = from job in jobs select job.Name;
                Assert.Equal(expectedNames, names);
                CheckCommonProperties("jobs", jobs);

                for (int i = 0; i < jobs.Count; i++)
                {
                    var entry = feed.Entries[i];
                    var job = jobs[i];
                    CheckJob(entry, job);
                }
            }
        }

        void CheckCommonProperties(string expectedName, ResourceEndpoint resourceEndpoint)
        {
            Assert.Equal(expectedName, resourceEndpoint.Title);

            //// Properties common to all resources

            Assert.DoesNotThrow(() =>
            {
                Version value = resourceEndpoint.GeneratorVersion;
                Assert.NotNull(value);
            });

            Assert.DoesNotThrow(() =>
            {
                Uri value = resourceEndpoint.Id;
                Assert.NotNull(value);
            });

            Assert.DoesNotThrow(() =>
            {
                string value = resourceEndpoint.Title;
                Assert.NotNull(value);
            });

            Assert.DoesNotThrow(() =>
            {
                DateTime value = resourceEndpoint.Updated;
                Assert.NotEqual(DateTime.MinValue, value);
            });
        }

        void CheckEai(Job password)
        {
            Assert.DoesNotThrow(() =>
            {
                bool canList = password.Eai.Acl.CanList;
                string app = password.Eai.Acl.App;
                dynamic eai = password.Eai;
                Assert.Equal(app, eai.Acl.App);
                Assert.Equal(canList, eai.Acl.CanList);
            });
        }

        void CheckJob(AtomEntry entry, Job job)
        {
            CheckCommonProperties(entry.Title, job);
            CheckEai(job);

            dynamic content = entry.Content;

            Assert.Equal(BooleanConverter.Instance.Convert(content.CanSummarize), job.CanSummarize);
            Assert.Equal(DateTimeConverter.Instance.Convert(content.CursorTime), job.CursorTime);
            Assert.Equal(Int32Converter.Instance.Convert(content.DefaultSaveTTL), job.DefaultSaveTTL);
            Assert.Equal(Int32Converter.Instance.Convert(content.DefaultTTL), job.DefaultTTL);
            Assert.Equal(Int64Converter.Instance.Convert(content.DiskUsage), job.DiskUsage);
            Assert.Equal(EnumConverter<DispatchState>.Instance.Convert(content.DispatchState), job.DispatchState);
            Assert.Equal(DoubleConverter.Instance.Convert(content.DoneProgress), job.DoneProgress);
            Assert.Equal(Int64Converter.Instance.Convert(content.DropCount), job.DropCount);
            Assert.Equal(DateTimeConverter.Instance.Convert(content.EarliestTime), job.EarliestTime);
            Assert.Equal(Int64Converter.Instance.Convert(content.EventAvailableCount), job.EventAvailableCount);
            Assert.Equal(Int64Converter.Instance.Convert(content.EventCount), job.EventCount);
            Assert.Equal(Int32Converter.Instance.Convert(content.EventFieldCount), job.EventFieldCount);
            Assert.Equal(BooleanConverter.Instance.Convert(content.EventIsStreaming), job.EventIsStreaming);
            Assert.Equal(BooleanConverter.Instance.Convert(content.EventIsTruncated), job.EventIsTruncated);
            Assert.Equal(content.EventSearch, job.EventSearch);
            Assert.Equal(EnumConverter<SortDirection>.Instance.Convert(content.EventSorting), job.EventSorting);
            Assert.Equal(Int64Converter.Instance.Convert(content.IndexEarliestTime), job.IndexEarliestTime);
            Assert.Equal(Int64Converter.Instance.Convert(content.IndexLatestTime), job.IndexLatestTime);
            Assert.Equal(BooleanConverter.Instance.Convert(content.IsBatchModeSearch), job.IsBatchModeSearch);
            Assert.Equal(BooleanConverter.Instance.Convert(content.IsDone), job.IsDone);
            Assert.Equal(BooleanConverter.Instance.Convert(content.IsFailed), job.IsFailed);
            Assert.Equal(BooleanConverter.Instance.Convert(content.IsFinalized), job.IsFinalized);
            Assert.Equal(BooleanConverter.Instance.Convert(content.IsPaused), job.IsPaused);
            Assert.Equal(BooleanConverter.Instance.Convert(content.IsPreviewEnabled), job.IsPreviewEnabled);
            Assert.Equal(BooleanConverter.Instance.Convert(content.IsRealTimeSearch), job.IsRealTimeSearch);
            Assert.Equal(BooleanConverter.Instance.Convert(content.IsRemoteTimeline), job.IsRemoteTimeline);
            Assert.Equal(BooleanConverter.Instance.Convert(content.IsSaved), job.IsSaved);
            Assert.Equal(BooleanConverter.Instance.Convert(content.IsSavedSearch), job.IsSavedSearch);
            Assert.Equal(BooleanConverter.Instance.Convert(content.IsZombie), job.IsZombie);
            Assert.Equal(content.Keywords, job.Keywords);
            Assert.Equal(DateTimeConverter.Instance.Convert(content.LatestTime), job.LatestTime);
            Assert.Equal(content.NormalizedSearch, job.NormalizedSearch);
            Assert.Equal(Int32Converter.Instance.Convert(content.NumPreviews), job.NumPreviews);
            Assert.Equal(content.Performance.Command.Search.Calcfields.InputCount, job.Performance.Command.Search.Calcfields.InputCount);
            Assert.Equal(Int32Converter.Instance.Convert(content.Pid), job.Pid);
            Assert.Equal(Int32Converter.Instance.Convert(content.Priority), job.Priority);
            Assert.Equal(content.RemoteSearch, job.RemoteSearch);
            Assert.Equal(content.ReportSearch, job.ReportSearch);
            Assert.Equal(content.Request, job.Request);
            Assert.Equal(Int64Converter.Instance.Convert(content.ResultCount), job.ResultCount);
            Assert.Equal(BooleanConverter.Instance.Convert(content.ResultIsStreaming), job.ResultIsStreaming);
            Assert.Equal(Int64Converter.Instance.Convert(content.ResultPreviewCount), job.ResultPreviewCount);
            Assert.Equal(DoubleConverter.Instance.Convert(content.RunDuration), job.RunDuration);
            Assert.Null(job.Runtime);
            Assert.Equal(Int64Converter.Instance.Convert(content.ScanCount), job.ScanCount);
            Assert.Equal(job.Title, job.Search);
            Assert.Equal(DateTime.MinValue, job.SearchEarliestTime);
            Assert.Equal(UnixDateTimeConverter.Instance.Convert(content.SearchLatestTime), job.SearchLatestTime);
            Assert.Equal(content.SearchProviders, job.SearchProviders);
            Assert.Equal(content.Sid, job.Sid);
            Assert.Equal(Int32Converter.Instance.Convert(content.StatusBuckets), job.StatusBuckets);
            Assert.Equal(Int64Converter.Instance.Convert(content.Ttl), job.Ttl);
        }
    }
}
