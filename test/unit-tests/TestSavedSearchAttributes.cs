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
    using Splunk.Client;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using Xunit;

    public class TestSavedSearchAttributes
    {
        [Trait("unit-test", "Splunk.Client.SavedSearchAttributes")]
        [Fact]
        void CanConstructSavedSearchAttributes()
        {
            var attributes = new SavedSearchAttributes();


            Assert.Equal(attributes.ToString().Length, 2642);
            Assert.Equal("action.email.auth_password=null; " +
                "action.email.auth_username=null; " +
                "action.email.bcc=null; " +
                "action.email.cc=null; " +
                "action.email.command=null; " +
                "action.email.format=null; " +
                "action.email.from=null; " +
                "action.email.inline=null; " +
                "action.email.mailserver=null; " +
                "action.email.maxresults=null; " +
                "action.email.maxtime=null; " +
                "action.email.reportCIDFontList=null; " +
                "action.email.reportIncludeSplunkLogo=null; " +
                "action.email.reportPaperOrientation=null; " +
                "action.email.reportPaperSize=null; " +
                "action.email.reportServerEnabled=null; " +
                "action.email.sendpdf=null; " +
                "action.email.sendresults=null; " +
                "action.email.subject=null; " +
                "action.email.to=null; " +
                "action.email.track_alert=null; " +
                "action.email.ttl=null; " +
                "action.email.use_ssl=null; " +
                "action.email.use_tls=null; " +
                "action.logevent.command = null; " +
                "action.logevent.description = null; " +
                "action.logevent.hostname = null; " +
                "action.logevent.icon_path = null; " +
                "action.logevent.is_custom = null; " +
                "action.logevent.label = null; " +
                "action.logevent.maxresults = null; " +
                "action.logevent.maxtime = null; " +
                "action.logevent.param.host = null; " +
                "action.logevent.param.index = null; " +
                "action.logevent.param.source = null; " +
                "action.logevent.param.sourcetype = null; " +
                "action.logevent.payload_format = null; " +
                "action.logevent.track_alert = null; " +
                "action.logevent.ttl = null; " +
                "action.lookup.append = null; " +
                "action.lookup.command = null; " +
                "action.lookup.description = null; " +
                "action.lookup.filename = null; " +
                "action.lookup.hostname = null; " +
                "action.lookup.icon_path = null; " +
                "action.lookup.label = null; " +
                "action.lookup.maxresults = null; " +
                "action.lookup.maxtime = null; " +
                "action.lookup.track_alert = null; " +
                "action.lookup.ttl = null; " +
                "action.email.width_sort_columns=null; " +
                "action.populate_lookup.command=null; " +
                "action.populate_lookup.dest=null; " +
                "action.populate_lookup.hostname=null; " +
                "action.populate_lookup.maxresults=null; " +
                "action.populate_lookup.maxtime=null; " +
                "action.populate_lookup.track_alert=null; " +
                "action.populate_lookup.ttl=null; " +
                "action.rss.command=null; " +
                "action.rss.maxresults=null; " +
                "action.rss.maxtime=null; " +
                "action.rss.track_alert=null; " +
                "action.rss.ttl=null; " +
                "action.script.command=null; " +
                "action.script.filename=null; " +
                "action.script.hostname=null; " +
                "action.script.maxresults=null; " +
                "action.script.maxtime=null; " +
                "action.script.track_alert=null; " +
                "action.script.ttl=null; " +
                "action.summary_index.command=null; " +
                "action.summary_index.inline=null; " +
                "action.summary_index.maxresults=null; " +
                "action.summary_index.maxtime=null; " +
                "action.summary_index.name=null; " +
                "action.summary_index.track_alert=null; " +
                "action.summary_index.ttl=null; " +
                "action.webhook.description = null; " +
                "action.webhook.hostname = null; " +
                "action.webhook.icon_path = null; " +
                "action.webhook.is_custom = null; " +
                "action.webhook.label = null; " +
                "action.webhook.maxresults = null; " +
                "action.webhook.maxtime = null; " +
                "action.webhook.param.user_agent = null; " +
                "action.webhook.payload_format = null; " +
                "action.webhook.track_alert = null; " +
                "action.webhook.ttl = null; " + 
                "action.webook.command = null; " +
                "actions=null; " +
                "alert.digest_mode=null; " +
                "alert.expires=null; " +
                "alert.severity=null; " +
                "alert.track=null; " +
                "alert_comparator=null; " +
                "alert_condition=null; " +
                "alert_threshold=null; " +
                "alert_type=null; " +
                "auto_summarize=null; " +
                "auto_summarize.command=null; " +
                "auto_summarize.cron_schedule=null; " +
                "auto_summarize.dispatch.earliest_time=null; " +
                "auto_summarize.dispatch.latest_time=null; " +
                "auto_summarize.dispatch.time_format=null; " +
                "auto_summarize.dispatch.ttl=null; " +
                "auto_summarize.max_disabled_buckets=null; " +
                "auto_summarize.max_summary_ratio=null; " +
                "auto_summarize.max_summary_size=null; " +
                "auto_summarize.max_time=null; " +
                "auto_summarize.suspend_period=null; " +
                "cron_schedule=null; " +
                "description=null; " +
                "disabled=null; " +
                "dispatch.buckets=null; " +
                "dispatch.earliest_time=null; " +
                "dispatch.latest_time=null; " +
                "dispatch.lookups=null; " +
                "dispatch.max_count=null; " +
                "dispatch.max_time=null; " +
                "dispatch.reduce_freq=null; " +
                "dispatch.rt_backfill=null; " +
                "dispatch.spawn_process=null; " +
                "dispatch.time_format=null; " +
                "is_scheduled=null; " +
                "is_visible=null; " +
                "max_concurrent=null; " +
                "realtime_schedule=null; " +
                "restart_on_searchpeer_add=null; " +
                "run_on_startup=null",
                attributes.ToString());

            Assert.Equal(new List<Argument>(), attributes);
        }

        [Trait("unit-test", "Splunk.Client.SavedSearchAttributes")]
        [Fact]
        void CanSetEverySavedSearchAttribute()
        {
            var attributes = new SavedSearchAttributes()
            {
                ActionEmailAuthPassword = "some-unchecked-string",
                ActionEmailAuthUsername = "some-unchecked-string",
                ActionEmailBcc = "some-unchecked-string",
                ActionEmailCC = "some-unchecked-string",
                ActionEmailCommand = "some-unchecked-string",
                ActionEmailFormat = EmailFormat.Html,
                ActionEmailFrom = "some-unchecked-string",
                ActionEmailInline = "some-unchecked-string",
                ActionEmailMailServer = "some-unchecked-string",
                ActionEmailMaxResults = 99,
                ActionEmailMaxTime = "some-unchecked-string",
                ActionEmailReportCidFontList = "some-unchecked-string",
                ActionEmailReportIncludeSplunkLogo = true,
                ActionEmailReportPaperOrientation = PaperOrientation.Landscape,
                ActionEmailReportPaperSize = PaperSize.Ledger,
                ActionEmailReportServerEnabled = true,
                ActionEmailSendPdf = true,
                ActionEmailSendResults = true,
                ActionEmailSubject = "some-unchecked-string",
                ActionEmailTo = "some-unchecked-string",
                ActionEmailTrackAlert = true,
                ActionEmailTtl = "some-unchecked-string",
                ActionEmailUseSsl = true,
                ActionEmailUseTls = true,
                ActionEmailWidthSortColumns = true,
                ActionPopulateLookupCommand = "some-unchecked-string",
                ActionPopulateLookupDestination = "some-unchecked-string",
                ActionPopulateLookupHostName = "some-unchecked-string",
                ActionPopulateLookupMaxResults = 99,
                ActionPopulateLookupMaxTime = "some-unchecked-string",
                ActionPopulateLookupTrackAlert = true,
                ActionPopulateLookupTtl = "some-unchecked-string",
                ActionRssCommand = "some-unchecked-string",
                ActionRssMaxResults = 99,
                ActionRssMaxTime = "some-unchecked-string",
                ActionRssTrackAlert = "some-unchecked-string",
                ActionRssTtl = "some-unchecked-string",
                Actions = "some-unchecked-string",
                ActionScriptCommand = "some-unchecked-string",
                ActionScriptFileName = "some-unchecked-string",
                ActionScriptHostName = "some-unchecked-string",
                ActionScriptMaxResults = 99,
                ActionScriptMaxTime = "some-unchecked-string",
                ActionScriptTrackAlert = true,
                ActionScriptTtl = "some-unchecked-string",
                ActionSummaryIndexCommand = "some-unchecked-string",
                ActionSummaryIndexInline = true,
                ActionSummaryIndexMaxResults = 99,
                ActionSummaryIndexMaxTime = "some-unchecked-string",
                ActionSummaryIndexName = "some-unchecked-string",
                ActionSummaryIndexTrackAlert = true,
                ActionSummaryIndexTtl = "some-unchecked-string",
                AlertComparator = AlertComparator.GreaterThan,
                AlertCondition = "some-unchecked-string",
                AlertDigestMode = true,
                AlertExpires = "some-unchecked-string",
                AlertSeverity = AlertSeverity.Warning,
                AlertThreshold = "some-unchecked-string",
                AlertTrack = AlertTrack.Automatic,
                AlertType = AlertType.Always,
                AutoSummarize = true,
                AutoSummarizeCommand = "some-unchecked-string",
                AutoSummarizeCronSchedule = "some-unchecked-string",
                AutoSummarizeDispatchEarliestTime = "some-unchecked-string",
                AutoSummarizeDispatchLatestTime = "some-unchecked-string",
                AutoSummarizeDispatchTimeFormat = "some-unchecked-string",
                AutoSummarizeDispatchTtl = "some-unchecked-string",
                AutoSummarizeMaxDisabledBuckets = 2,
                AutoSummarizeMaxSummaryRatio = 0.1,
                AutoSummarizeMaxSummarySize = 52428800L,
                AutoSummarizeMaxTime = 3600,
                AutoSummarizeSuspendPeriod = "some-unchecked-string",
                CronSchedule = "some-unchecked-string",
                Description = "some-unchecked-string",
                Disabled = true,
                DispatchBuckets = 99,
                DispatchEarliestTime = "some-unchecked-string",
                DispatchLatestTime = "some-unchecked-string",
                DispatchLookups = true,
                DispatchMaxCount = 99,
                DispatchMaxTime = 99,
                DispatchRealTimeBackfill = true,
                DispatchReduceFrequency = 99,
                DispatchSpawnProcess = true,
                DispatchTimeFormat = "some-unchecked-string",
                DispatchTtl = "some-unchecked-string",
                IsScheduled = true,
                IsVisible = true,
                MaxConcurrent = 99,
                RealTimeSchedule = "some-unchecked-string",
                RestartOnSearchPeerAdd = "some-unchecked-string",
                RunOnStartup = true,
            };

            Assert.Equal(
                "action.email.auth_password=some-unchecked-string; " +
                "action.email.auth_username=some-unchecked-string; " +
                "action.email.bcc=some-unchecked-string; " +
                "action.email.cc=some-unchecked-string; " +
                "action.email.command=some-unchecked-string; " +
                "action.email.format=html; " +
                "action.email.from=some-unchecked-string; " +
                "action.email.inline=some-unchecked-string; " +
                "action.email.mailserver=some-unchecked-string; " +
                "action.email.maxresults=99; " +
                "action.email.maxtime=some-unchecked-string; " +
                "action.email.reportCIDFontList=some-unchecked-string; " +
                "action.email.reportIncludeSplunkLogo=1; " +
                "action.email.reportPaperOrientation=landscape; " +
                "action.email.reportPaperSize=ledger; " +
                "action.email.reportServerEnabled=1; " +
                "action.email.sendpdf=1; " +
                "action.email.sendresults=1; " +
                "action.email.subject=some-unchecked-string; " +
                "action.email.to=some-unchecked-string; " +
                "action.email.track_alert=1; " +
                "action.email.ttl=some-unchecked-string; " +
                "action.email.use_ssl=1; " +
                "action.email.use_tls=1; " +
                "action.email.width_sort_columns=1; " +
                "action.populate_lookup.command=some-unchecked-string; " +
                "action.populate_lookup.dest=some-unchecked-string; " +
                "action.populate_lookup.hostname=some-unchecked-string; " +
                "action.populate_lookup.maxresults=99; " +
                "action.populate_lookup.maxtime=some-unchecked-string; " +
                "action.populate_lookup.track_alert=1; " +
                "action.populate_lookup.ttl=some-unchecked-string; " +
                "action.rss.command=some-unchecked-string; " +
                "action.rss.maxresults=99; " +
                "action.rss.maxtime=some-unchecked-string; " +
                "action.rss.track_alert=some-unchecked-string; " +
                "action.rss.ttl=some-unchecked-string; " +
                "action.script.command=some-unchecked-string; " +
                "action.script.filename=some-unchecked-string; " +
                "action.script.hostname=some-unchecked-string; " +
                "action.script.maxresults=99; " +
                "action.script.maxtime=some-unchecked-string; " +
                "action.script.track_alert=1; " +
                "action.script.ttl=some-unchecked-string; " +
                "action.summary_index.command=some-unchecked-string; " +
                "action.summary_index.inline=1; " +
                "action.summary_index.maxresults=99; " +
                "action.summary_index.maxtime=some-unchecked-string; " +
                "action.summary_index.name=some-unchecked-string; " +
                "action.summary_index.track_alert=1; " +
                "action.summary_index.ttl=some-unchecked-string; " +
                "actions=some-unchecked-string; " +
                "alert.digest_mode=1; " +
                "alert.expires=some-unchecked-string; " +
                "alert.severity=3; " +
                "alert.track=auto; " +
                "alert_comparator=greater than; " +
                "alert_condition=some-unchecked-string; " +
                "alert_threshold=some-unchecked-string; " +
                "alert_type=always; " +
                "auto_summarize=1; " +
                "auto_summarize.command=some-unchecked-string; " +
                "auto_summarize.cron_schedule=some-unchecked-string; " +
                "auto_summarize.dispatch.earliest_time=some-unchecked-string; " +
                "auto_summarize.dispatch.latest_time=some-unchecked-string; " +
                "auto_summarize.dispatch.time_format=some-unchecked-string; " +
                "auto_summarize.dispatch.ttl=some-unchecked-string; " +
                "auto_summarize.max_disabled_buckets=2; " +
                "auto_summarize.max_summary_ratio=0.1; " +
                "auto_summarize.max_summary_size=52428800; " +
                "auto_summarize.max_time=3600; " +
                "auto_summarize.suspend_period=some-unchecked-string; " +
                "cron_schedule=some-unchecked-string; " +
                "description=some-unchecked-string; " +
                "disabled=1; " +
                "dispatch.buckets=99; " +
                "dispatch.earliest_time=some-unchecked-string; " +
                "dispatch.latest_time=some-unchecked-string; " +
                "dispatch.lookups=1; " +
                "dispatch.max_count=99; " +
                "dispatch.max_time=99; " +
                "dispatch.reduce_freq=99; " +
                "dispatch.rt_backfill=1; " +
                "dispatch.spawn_process=1; " +
                "dispatch.time_format=some-unchecked-string; " +
                "is_scheduled=1; " +
                "is_visible=1; " +
                "max_concurrent=99; " +
                "realtime_schedule=some-unchecked-string; " +
                "restart_on_searchpeer_add=some-unchecked-string; " +
                "run_on_startup=1",
                attributes.ToString());

            Assert.Equal(new List<Argument> 
                {
                    new Argument("action.email.auth_password", "some-unchecked-string"),
                    new Argument("action.email.auth_username", "some-unchecked-string"),
                    new Argument("action.email.bcc", "some-unchecked-string"),
                    new Argument("action.email.cc", "some-unchecked-string"),
                    new Argument("action.email.command", "some-unchecked-string"),
                    new Argument("action.email.format", "html"),
                    new Argument("action.email.from", "some-unchecked-string"),
                    new Argument("action.email.inline", "some-unchecked-string"),
                    new Argument("action.email.mailserver", "some-unchecked-string"),
                    new Argument("action.email.maxresults", "99"),
                    new Argument("action.email.maxtime", "some-unchecked-string"),
                    new Argument("action.email.reportCIDFontList", "some-unchecked-string"),
                    new Argument("action.email.reportIncludeSplunkLogo", 1),
                    new Argument("action.email.reportPaperOrientation", "landscape"),
                    new Argument("action.email.reportPaperSize", "ledger"),
                    new Argument("action.email.reportServerEnabled", 1),
                    new Argument("action.email.sendpdf", 1),
                    new Argument("action.email.sendresults", 1),
                    new Argument("action.email.subject", "some-unchecked-string"),
                    new Argument("action.email.to", "some-unchecked-string"),
                    new Argument("action.email.track_alert", 1),
                    new Argument("action.email.ttl", "some-unchecked-string"),
                    new Argument("action.email.use_ssl", 1),
                    new Argument("action.email.use_tls", 1),
                    new Argument("action.email.width_sort_columns", 1),
                    new Argument("action.populate_lookup.command", "some-unchecked-string"),
                    new Argument("action.populate_lookup.dest", "some-unchecked-string"),
                    new Argument("action.populate_lookup.hostname", "some-unchecked-string"),
                    new Argument("action.populate_lookup.maxresults", "99"),
                    new Argument("action.populate_lookup.maxtime", "some-unchecked-string"),
                    new Argument("action.populate_lookup.track_alert", 1),
                    new Argument("action.populate_lookup.ttl", "some-unchecked-string"),
                    new Argument("action.rss.command", "some-unchecked-string"),
                    new Argument("action.rss.maxresults", "99"),
                    new Argument("action.rss.maxtime", "some-unchecked-string"),
                    new Argument("action.rss.track_alert", "some-unchecked-string"),
                    new Argument("action.rss.ttl", "some-unchecked-string"),
                    new Argument("action.script.command", "some-unchecked-string"),
                    new Argument("action.script.filename", "some-unchecked-string"),
                    new Argument("action.script.hostname", "some-unchecked-string"),
                    new Argument("action.script.maxresults", "99"),
                    new Argument("action.script.maxtime", "some-unchecked-string"),
                    new Argument("action.script.track_alert", 1),
                    new Argument("action.script.ttl", "some-unchecked-string"),
                    new Argument("action.summary_index.command", "some-unchecked-string"),
                    new Argument("action.summary_index.inline", 1),
                    new Argument("action.summary_index.maxresults", "99"),
                    new Argument("action.summary_index.maxtime", "some-unchecked-string"),
                    new Argument("action.summary_index.name", "some-unchecked-string"),
                    new Argument("action.summary_index.track_alert", 1),
                    new Argument("action.summary_index.ttl", "some-unchecked-string"),
                    new Argument("actions", "some-unchecked-string"),
                    new Argument("alert.digest_mode", 1),
                    new Argument("alert.expires", "some-unchecked-string"),
                    new Argument("alert.severity", "3"),
                    new Argument("alert.track", "auto"),
                    new Argument("alert_comparator", "greater than"),
                    new Argument("alert_condition", "some-unchecked-string"),
                    new Argument("alert_threshold", "some-unchecked-string"),
                    new Argument("alert_type", "always"),
                    new Argument("auto_summarize", 1),
                    new Argument("auto_summarize.command", "some-unchecked-string"),
                    new Argument("auto_summarize.cron_schedule", "some-unchecked-string"),
                    new Argument("auto_summarize.dispatch.earliest_time", "some-unchecked-string"),
                    new Argument("auto_summarize.dispatch.latest_time", "some-unchecked-string"),
                    new Argument("auto_summarize.dispatch.time_format", "some-unchecked-string"),
                    new Argument("auto_summarize.dispatch.ttl", "some-unchecked-string"),
                    new Argument("auto_summarize.max_disabled_buckets", "2"),
                    new Argument("auto_summarize.max_summary_ratio", "0.1"),
                    new Argument("auto_summarize.max_summary_size", "52428800"),
                    new Argument("auto_summarize.max_time", "3600"),
                    new Argument("auto_summarize.suspend_period", "some-unchecked-string"),
                    new Argument("cron_schedule", "some-unchecked-string"),
                    new Argument("description", "some-unchecked-string"),
                    new Argument("disabled", 1),
                    new Argument("dispatch.buckets", "99"),
                    new Argument("dispatch.earliest_time", "some-unchecked-string"),
                    new Argument("dispatch.latest_time", "some-unchecked-string"),
                    new Argument("dispatch.lookups", 1),
                    new Argument("dispatch.max_count", "99"),
                    new Argument("dispatch.max_time", "99"),
                    new Argument("dispatch.reduce_freq", "99"),
                    new Argument("dispatch.rt_backfill", 1),
                    new Argument("dispatch.spawn_process", 1),
                    new Argument("dispatch.time_format", "some-unchecked-string"),
                    new Argument("is_scheduled", 1),
                    new Argument("is_visible", 1),
                    new Argument("max_concurrent", "99"),
                    new Argument("realtime_schedule", "some-unchecked-string"),
                    new Argument("restart_on_searchpeer_add", "some-unchecked-string"),
                    new Argument("run_on_startup", 1)
                },
                attributes);
        }
    }
}
