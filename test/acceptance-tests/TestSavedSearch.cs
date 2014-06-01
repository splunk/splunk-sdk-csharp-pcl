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
    using Splunk.Client.Helpers;

    using System;
    using System.Linq;
    using System.Threading.Tasks;
    
    using Xunit;

    /// <summary>
    /// Tests saved searches
    /// </summary>
    public class SavedSearchTest
    {
        /// <summary>
        /// Test saved search dispatch
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.SavedSearch")]
        [Fact]
        public async Task SavedSearchDispatchProperties()
        {
            using (Service service = await SDKHelper.CreateService())
            {
                // Ensure correct start state

                SavedSearchCollection savedSearches = service.SavedSearches;
                string name = "sdk-test_DispatchProperties";
                SavedSearch testSearch = await savedSearches.GetOrNullAsync(name);

                if (testSearch != null)
                {
                    await testSearch.RemoveAsync();
                }

                await savedSearches.GetAllAsync();
                Assert.Null(savedSearches.SingleOrDefault(savedSearch => savedSearch.Name == name));

                // Create a saved search
                testSearch = await savedSearches.CreateAsync(name, "search index=sdk-tests * earliest=-1m");

                // Dispatch the saved search with default arguments
                Job job = await testSearch.DispatchAsync();
                await job.TransitionAsync(DispatchState.Done);
                await job.CancelAsync();

                // Dispatch with some additional search options
                job = await testSearch.DispatchAsync(new SavedSearchDispatchArgs() { DispatchBuckets = 100 });
                await job.TransitionAsync(DispatchState.Done);
                await job.CancelAsync();

                // Dispatch with some additional search options
                job = await testSearch.DispatchAsync(new SavedSearchDispatchArgs() { DispatchEarliestTime = "aaaa" });
                await job.TransitionAsync(DispatchState.Done);
                await job.CancelAsync();

                var savedSearchTemplateArgs = new SavedSearchTemplateArgs(
                    new Argument("action.email.authpassword ", "sdk-password"),
                    new Argument("action.email.authusername ", "sdk-username"),
                    new Argument("action.email.bcc ", "sdk-bcc@splunk.com"),
                    new Argument("action.email.cc ", "sdk-cc@splunk.com"),
                    new Argument("action.email.command ", "$name1$"),
                    new Argument("action.email.format ", "text"),
                    new Argument("action.email.from ", "sdk@splunk.com"),
                    new Argument("action.email.hostname ", "dummy1.host.com"),
                    new Argument("action.email.inline ", "true"),
                    new Argument("action.email.mailserver ", "splunk.com"),
                    new Argument("action.email.maxresults ", "101"),
                    new Argument("action.email.maxtime ", "10s"),
                    new Argument("action.email.pdfview ", "dummy"),
                    new Argument("action.email.reportpaperorientation ", "landscape"),
                    new Argument("action.email.reportpapersize ", "letter"),
                    new Argument("action.email.reportserverenabled ", "false"),
                    new Argument("action.email.reportserverurl ", "splunk.com"),
                    new Argument("action.email.sendpdf ", "false"),
                    new Argument("action.email.sendresults ", "false"),
                    new Argument("action.email.subject ", "sdk-subject"),
                    new Argument("action.email.to ", "sdk-to@splunk.com"),
                    new Argument("action.email.trackalert ", "false"),
                    new Argument("action.email.ttl ", "61"),
                    new Argument("action.email.usessl ", "false"),
                    new Argument("action.email.usetls ", "false"),
                    new Argument("action.email.widthsortcolumns ", "false"),
                    new Argument("actions.populatelookup.command ", "$name2$"),
                    new Argument("actions.populatelookup.dest ", "dummypath"),
                    new Argument("actions.populatelookup.hostname ", "dummy2.host.com"),
                    new Argument("actions.populatelookup.maxresults ", "102"),
                    new Argument("actions.populatelookup.maxtime ", "20s"),
                    new Argument("actions.populatelookup.trackalert ", "false"),
                    new Argument("actions.populatelookup.ttl ", "62"),
                    new Argument("actions.rss.command ", "$name3$"),
                    new Argument("actions.rss.hostname ", "dummy3.host.com"),
                    new Argument("actions.rss.maxresults ", "103"),
                    new Argument("actions.rss.maxtime ", "30s"),
                    new Argument("actions.rss.trackalert ", "false"),
                    new Argument("actions.rss.ttl ", "63"),
                    new Argument("actionscriptcommand ", "$name4$"),
                    new Argument("actionscriptfilename ", "action_script_filename"),
                    new Argument("actionscripthostname ", "dummy4.host.com"),
                    new Argument("actionscriptmaxresults ", "104"),
                    new Argument("actionscriptmaxtime ", "40s"),
                    new Argument("actionscripttrackalert ", "false"),
                    new Argument("actionscriptttl ", "64"),
                    new Argument("actions.summaryindex.command ", "$name5$"),
                    new Argument("actions.summaryindex.hostname ", "dummy5.host.com"),
                    new Argument("actions.summaryindex.inline ", "false"),
                    new Argument("actions.summaryindex.maxresults ", "105"),
                    new Argument("actions.summaryindex.maxtime ", "50s"),
                    new Argument("actions.summaryindex.trackalert ", "false"),
                    new Argument("actions.summaryindex.ttl ", "65"),
                    new Argument("actions ", "rss,email,populate_lookup,script,summary_index"));

                //// Same as the previous dispatch except using custom arg
                job = await testSearch.DispatchAsync(templateArgs: savedSearchTemplateArgs);
                await job.TransitionAsync(DispatchState.Done);
                await job.CancelAsync();

                // Delete the saved search
                await testSearch.RemoveAsync();

                testSearch = await savedSearches.GetOrNullAsync(name);
                Assert.Null(testSearch);
            }
        }

        /// <summary>
        /// Touch test properties
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.SavedSearch")]
        [Fact]
        public async Task SavedSearchesProperties()
        {
            using (Service service = await SDKHelper.CreateService())
            {
                SavedSearchCollection savedSearches = service.SavedSearches;
                await savedSearches.GetAllAsync();

                // Iterate saved searches and make sure we can read them

                foreach (SavedSearch savedSearch in savedSearches)
                {
                    string dummyString;
                    bool dummyBool;
                    //DateTime dummyDateTime;
                    int dummyInt;

                    // Resource properties
                    //dummyString = savedSearch.Name;
                    dummyString = savedSearch.Name;
                    //dummyString = savedSearch.Path;
                    // SavedSearch properties get
                    dummyString = savedSearch.Actions.Email.AuthPassword;
                    dummyString = savedSearch.Actions.Email.AuthUsername;
                    dummyBool = savedSearch.Actions.Email.SendResults;
                    dummyString = savedSearch.Actions.Email.Bcc;
                    dummyString = savedSearch.Actions.Email.CC;
                    dummyString = savedSearch.Actions.Email.Command;
                    EmailFormat emailFormat = savedSearch.Actions.Email.Format;
                    dummyBool = savedSearch.Actions.Email.Inline;
                    dummyString = savedSearch.Actions.Email.Mailserver;
                    dummyInt = savedSearch.Actions.Email.MaxResults;
                    dummyString = savedSearch.Actions.Email.MaxTime;
                    PaperOrientation paperOrientation = savedSearch.Actions.Email.ReportPaperOrientation;
                    PaperSize paperSize = savedSearch.Actions.Email.ReportPaperSize;
                    dummyBool = savedSearch.Actions.Email.ReportServerEnabled;
                    //dummyString = savedSearch.Actions.Email.ReportServerUrl;
                    dummyBool = savedSearch.Actions.Email.SendPdf;
                    dummyBool = savedSearch.Actions.Email.SendResults;
                    dummyString = savedSearch.Actions.Email.Subject;
                    dummyString = savedSearch.Actions.Email.To;
                    dummyBool = savedSearch.Actions.Email.TrackAlert;
                    dummyString = savedSearch.Actions.Email.Ttl;
                    dummyBool = savedSearch.Actions.Email.UseSsl;
                    dummyBool = savedSearch.Actions.Email.UseTls;
                    dummyBool = savedSearch.Actions.Email.WidthSortColumns;
                    dummyString = savedSearch.Actions.PopulateLookup.Command;
                    dummyString = savedSearch.Actions.PopulateLookup.Destination;
                    dummyString = savedSearch.Actions.PopulateLookup.Hostname;
                    dummyInt = savedSearch.Actions.PopulateLookup.MaxResults;
                    dummyString = savedSearch.Actions.PopulateLookup.MaxTime;
                    dummyBool = savedSearch.Actions.PopulateLookup.TrackAlert;
                    dummyString = savedSearch.Actions.PopulateLookup.Ttl;
                    dummyString = savedSearch.Actions.Rss.Command;
                    //dummyString = savedSearch.Actions.Rss.Hostname;
                    dummyInt = savedSearch.Actions.Rss.MaxResults;
                    dummyString = savedSearch.Actions.Rss.MaxTime;
                    dummyBool = savedSearch.Actions.Rss.TrackAlert;
                    dummyString = savedSearch.Actions.Rss.Ttl;
                    SavedSearch.Action_t.Script_t scriptT = savedSearch.Actions.Script;
                    dummyString = scriptT.FileName;
                    dummyString = scriptT.Hostname;
                    dummyInt = scriptT.MaxResults;
                    dummyString = scriptT.MaxTime;
                    dummyBool = scriptT.TrackAlert;
                    dummyString = scriptT.Ttl;
                    dummyString = savedSearch.Actions.SummaryIndex.Name;
                    dummyString = savedSearch.Actions.SummaryIndex.Command;
                    //dummyString = savedSearch.Actions.SummaryIndex.Hostname;
                    dummyBool = savedSearch.Actions.SummaryIndex.Inline;
                    dummyInt = savedSearch.Actions.SummaryIndex.MaxResults;
                    dummyString = savedSearch.Actions.SummaryIndex.MaxTime;
                    dummyBool = savedSearch.Actions.SummaryIndex.TrackAlert;
                    dummyString = savedSearch.Actions.SummaryIndex.Ttl;
                    dummyBool = savedSearch.Alert.DigestMode;
                    dummyString = savedSearch.Alert.Expires;
                    AlertSeverity alertSeverity = savedSearch.Alert.Severity;
                    SavedSearch.Alert_t.Suppress_t suppressT = savedSearch.Alert.Suppress;
                    dummyString = suppressT.Fields;
                    dummyString = suppressT.Period;
                    AlertTrack track = savedSearch.Alert.Track;
                    AlertComparator comparator = savedSearch.Alert.Comparator;
                    dummyString = savedSearch.Alert.Condition;
                    dummyString = savedSearch.Alert.Threshold;
                    AlertType alertType = savedSearch.Alert.Type;
                    dummyString = savedSearch.CronSchedule;
                    dummyString = savedSearch.Description;
                    dummyInt = savedSearch.Dispatch.Buckets;
                    dummyString = savedSearch.Dispatch.EarliestTime;
                    //dummyString = savedSearch.Dispatch.LatestTime;
                    dummyBool = savedSearch.Dispatch.Lookups;
                    dummyInt = savedSearch.Dispatch.MaxCount;
                    dummyInt = savedSearch.Dispatch.MaxTime;
                    dummyInt = savedSearch.Dispatch.ReduceFreq;
                    dummyBool = savedSearch.Dispatch.RealtimeBackfill;
                    dummyBool = savedSearch.Dispatch.SpawnProcess;
                    dummyString = savedSearch.Dispatch.TimeFormat;
                    dummyString = savedSearch.Dispatch.Ttl;
                    SavedSearch.Display_t displayT = savedSearch.Display;
                    dummyInt = savedSearch.MaxConcurrent;
                    //dummyDateTime = savedSearch.NextScheduledTime;
                    dummyString = savedSearch.QualifiedSearch;
                    dummyBool = savedSearch.RealtimeSchedule;
                    dummyString = savedSearch.Request.UIDispatchApp;
                    dummyString = savedSearch.Request.UIDispatchView;
                    dummyBool = savedSearch.RestartOnSearchPeerAdd;
                    dummyBool = savedSearch.RunOnStartup;
                    dummyString = savedSearch.Search;
                    //dummyString = savedSearch.Vsid;
                    dummyBool = savedSearch.Actions.Email.IsEnabled;
                    dummyBool = savedSearch.Actions.PopulateLookup.IsEnabled;
                    dummyBool = savedSearch.Actions.Rss.IsEnabled;
                    //dummyBool = savedSearch.IsActionScript;
                    dummyBool = savedSearch.Actions.SummaryIndex.IsEnabled;
                    dummyBool = savedSearch.IsDisabled;
                    dummyBool = savedSearch.IsScheduled;
                    dummyBool = savedSearch.IsVisible;
                }

                for (int i = 0; i < savedSearches.Count; i++)
                {
                    SavedSearch savedSearch = savedSearches[i];

                    string dummyString;
                    bool dummyBool;
                    //DateTime dummyDateTime;
                    int dummyInt;

                    // Resource properties
                    //dummyString = savedSearch.Name;
                    dummyString = savedSearch.Name;
                    //dummyString = savedSearch.Path;
                    // SavedSearch properties get
                    dummyString = savedSearch.Actions.Email.AuthPassword;
                    dummyString = savedSearch.Actions.Email.AuthUsername;
                    dummyBool = savedSearch.Actions.Email.SendResults;
                    dummyString = savedSearch.Actions.Email.Bcc;
                    dummyString = savedSearch.Actions.Email.CC;
                    dummyString = savedSearch.Actions.Email.Command;
                    EmailFormat emailFormat = savedSearch.Actions.Email.Format;
                    dummyBool = savedSearch.Actions.Email.Inline;
                    dummyString = savedSearch.Actions.Email.Mailserver;
                    dummyInt = savedSearch.Actions.Email.MaxResults;
                    dummyString = savedSearch.Actions.Email.MaxTime;
                    PaperOrientation paperOrientation = savedSearch.Actions.Email.ReportPaperOrientation;
                    PaperSize paperSize = savedSearch.Actions.Email.ReportPaperSize;
                    dummyBool = savedSearch.Actions.Email.ReportServerEnabled;
                    //dummyString = savedSearch.Actions.Email.ReportServerUrl;
                    dummyBool = savedSearch.Actions.Email.SendPdf;
                    dummyBool = savedSearch.Actions.Email.SendResults;
                    dummyString = savedSearch.Actions.Email.Subject;
                    dummyString = savedSearch.Actions.Email.To;
                    dummyBool = savedSearch.Actions.Email.TrackAlert;
                    dummyString = savedSearch.Actions.Email.Ttl;
                    dummyBool = savedSearch.Actions.Email.UseSsl;
                    dummyBool = savedSearch.Actions.Email.UseTls;
                    dummyBool = savedSearch.Actions.Email.WidthSortColumns;
                    dummyString = savedSearch.Actions.PopulateLookup.Command;
                    dummyString = savedSearch.Actions.PopulateLookup.Destination;
                    dummyString = savedSearch.Actions.PopulateLookup.Hostname;
                    dummyInt = savedSearch.Actions.PopulateLookup.MaxResults;
                    dummyString = savedSearch.Actions.PopulateLookup.MaxTime;
                    dummyBool = savedSearch.Actions.PopulateLookup.TrackAlert;
                    dummyString = savedSearch.Actions.PopulateLookup.Ttl;
                    dummyString = savedSearch.Actions.Rss.Command;
                    //dummyString = savedSearch.Actions.Rss.Hostname;
                    dummyInt = savedSearch.Actions.Rss.MaxResults;
                    dummyString = savedSearch.Actions.Rss.MaxTime;
                    dummyBool = savedSearch.Actions.Rss.TrackAlert;
                    dummyString = savedSearch.Actions.Rss.Ttl;
                    SavedSearch.Action_t.Script_t scriptT = savedSearch.Actions.Script;
                    dummyString = scriptT.FileName;
                    dummyString = scriptT.Hostname;
                    dummyInt = scriptT.MaxResults;
                    dummyString = scriptT.MaxTime;
                    dummyBool = scriptT.TrackAlert;
                    dummyString = scriptT.Ttl;
                    dummyString = savedSearch.Actions.SummaryIndex.Name;
                    dummyString = savedSearch.Actions.SummaryIndex.Command;
                    //dummyString = savedSearch.Actions.SummaryIndex.Hostname;
                    dummyBool = savedSearch.Actions.SummaryIndex.Inline;
                    dummyInt = savedSearch.Actions.SummaryIndex.MaxResults;
                    dummyString = savedSearch.Actions.SummaryIndex.MaxTime;
                    dummyBool = savedSearch.Actions.SummaryIndex.TrackAlert;
                    dummyString = savedSearch.Actions.SummaryIndex.Ttl;
                    dummyBool = savedSearch.Alert.DigestMode;
                    dummyString = savedSearch.Alert.Expires;
                    AlertSeverity alertSeverity = savedSearch.Alert.Severity;
                    SavedSearch.Alert_t.Suppress_t suppressT = savedSearch.Alert.Suppress;
                    dummyString = suppressT.Fields;
                    dummyString = suppressT.Period;
                    AlertTrack track = savedSearch.Alert.Track;
                    AlertComparator comparator = savedSearch.Alert.Comparator;
                    dummyString = savedSearch.Alert.Condition;
                    dummyString = savedSearch.Alert.Threshold;
                    AlertType alertType = savedSearch.Alert.Type;
                    dummyString = savedSearch.CronSchedule;
                    dummyString = savedSearch.Description;
                    dummyInt = savedSearch.Dispatch.Buckets;
                    dummyString = savedSearch.Dispatch.EarliestTime;
                    //dummyString = savedSearch.Dispatch.LatestTime;
                    dummyBool = savedSearch.Dispatch.Lookups;
                    dummyInt = savedSearch.Dispatch.MaxCount;
                    dummyInt = savedSearch.Dispatch.MaxTime;
                    dummyInt = savedSearch.Dispatch.ReduceFreq;
                    dummyBool = savedSearch.Dispatch.RealtimeBackfill;
                    dummyBool = savedSearch.Dispatch.SpawnProcess;
                    dummyString = savedSearch.Dispatch.TimeFormat;
                    dummyString = savedSearch.Dispatch.Ttl;
                    SavedSearch.Display_t displayT = savedSearch.Display;
                    dummyInt = savedSearch.MaxConcurrent;
                    //dummyDateTime = savedSearch.NextScheduledTime;
                    dummyString = savedSearch.QualifiedSearch;
                    dummyBool = savedSearch.RealtimeSchedule;
                    dummyString = savedSearch.Request.UIDispatchApp;
                    dummyString = savedSearch.Request.UIDispatchView;
                    dummyBool = savedSearch.RestartOnSearchPeerAdd;
                    dummyBool = savedSearch.RunOnStartup;
                    dummyString = savedSearch.Search;
                    //dummyString = savedSearch.Vsid;
                    dummyBool = savedSearch.Actions.Email.IsEnabled;
                    dummyBool = savedSearch.Actions.PopulateLookup.IsEnabled;
                    dummyBool = savedSearch.Actions.Rss.IsEnabled;
                    //dummyBool = savedSearch.IsActionScript;
                    dummyBool = savedSearch.Actions.SummaryIndex.IsEnabled;
                    dummyBool = savedSearch.IsDisabled;
                    dummyBool = savedSearch.IsScheduled;
                    dummyBool = savedSearch.IsVisible;
                }
            }
        }

        /// <summary>
        /// Test Saved Search Create Read Update and Delete.
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.SavedSearch")]
        [Fact]
        public async Task SavedSearchesUpdateProperties()
        {
            using (Service service = await SDKHelper.CreateService())
            {
                SavedSearchCollection savedSearches = service.SavedSearches;
                const string name = "sdk-test_UpdateProperties";
                const string search = "search index=sdk-tests * earliest=-1m";

                //// Ensure test starts in a known good state

                SavedSearch testSearch = await savedSearches.GetOrNullAsync(name);

                if (testSearch != null)
                {
                    await testSearch.RemoveAsync();
                }

                //// Create a saved search

                testSearch = await savedSearches.CreateAsync(name, search);
                testSearch = await savedSearches.GetOrNullAsync(name);
                Assert.NotNull(testSearch);

                //// Read the saved search

                await savedSearches.GetAllAsync();
                testSearch = savedSearches.SingleOrDefault(a => a.Name == name);
                Assert.True(testSearch.IsVisible);

                // CONSIDER: Test some additinal default property values.

                // Update search properties, but don't specify required args to test
                // pulling them from the existing object
                bool updatedSnapshot = await testSearch.UpdateAsync(new SavedSearchAttributes() { IsVisible = false });
                Assert.True(updatedSnapshot);
                Assert.False(testSearch.IsVisible);

                // Delete the saved search            
                await testSearch.RemoveAsync();
                testSearch = await savedSearches.GetOrNullAsync(testSearch.Name);
                Assert.Null(testSearch);

                // Create a saved search with some additional arguments
                testSearch = await savedSearches.CreateAsync(name, search, new SavedSearchAttributes() { IsVisible = false });
                Assert.False(testSearch.IsVisible);

                // Set email param attributes

                var attributes = new SavedSearchAttributes()
                {
                    ActionEmailAuthPassword = "sdk-password",
                    ActionEmailAuthUsername = "sdk-username",
                    ActionEmailBcc = "sdk-bcc@splunk.com",
                    ActionEmailCC = "sdk-cc@splunk.com",
                    ActionEmailCommand = "$name1$",
                    ActionEmailFormat = EmailFormat.Plain,
                    ActionEmailFrom = "sdk@splunk.com",
                    //attrs.ActionEmailHostname = "dummy1.host.com",
                    ActionEmailInline = "true",
                    ActionEmailMailServer = "splunk.com",
                    ActionEmailMaxResults = 101,
                    ActionEmailMaxTime = "10s",
                    ActionEmailSendPdf = true, //??ActionEmailPdfView = "dummy",
                    ActionEmailSendResults = true, //??ActionEmailPreProcessResults = "*",
                    ActionEmailReportPaperOrientation = PaperOrientation.Landscape,
                    ActionEmailReportPaperSize = PaperSize.Letter,
                    ActionEmailReportServerEnabled = false,
                    //attrs.ActionEmailReportServerUrl = "splunk.com",
                    ActionEmailSubject = "sdk-subject",
                    ActionEmailTo = "sdk-to@splunk.com",
                    ActionEmailTrackAlert = false,
                    ActionEmailTtl = "61",
                    ActionEmailUseSsl = false,
                    ActionEmailUseTls = false,
                    ActionEmailWidthSortColumns = false,
                    ActionPopulateLookupCommand = "$name2$",
                    ActionPopulateLookupDestination = "dummypath",
                    ActionPopulateLookupHostname = "dummy2.host.com",
                    ActionPopulateLookupMaxResults = 102,
                    ActionPopulateLookupMaxTime = "20s",
                    ActionPopulateLookupTrackAlert = false,
                    ActionPopulateLookupTtl = "62",
                    ActionRssCommand = "$name3$",
                    //attrs.ActionRssHostname = "dummy3.host.com",
                    ActionRssMaxResults = 103,
                    ActionRssMaxTime = "30s",
                    ActionRssTrackAlert = "false",
                    ActionRssTtl = "63",
                    ActionScriptCommand = "$name4$",
                    ActionScriptFileName = "action_script_filename",
                    ActionScriptHostname = "dummy4.host.com",
                    ActionScriptMaxResults = 104,
                    ActionScriptMaxTime = "40s",
                    ActionScriptTrackAlert = false,
                    ActionScriptTtl = "64",
                    ActionSummaryIndexName = "default",
                    ActionSummaryIndexCommand = "$name5$",
                    //attrs.ActionSummaryIndexHostname = "dummy5.host.com",
                    ActionSummaryIndexInline = false,
                    ActionSummaryIndexMaxResults = 105,
                    ActionSummaryIndexMaxTime = "50s",
                    ActionSummaryIndexTrackAlert = false,
                    ActionSummaryIndexTtl = "65",
                    Actions = "rss,email,populate_lookup,script,summary_index"
                };

                await testSearch.UpdateAsync(attributes);

                // check

                Assert.True(testSearch.Actions.Email != null); //IsActionEmail));
                Assert.True(testSearch.Actions.PopulateLookup != null);
                Assert.True(testSearch.Actions.Rss != null);
                Assert.True(testSearch.Actions.Script != null);
                Assert.True(testSearch.Actions.SummaryIndex != null);

                Assert.Equal("sdk-password", testSearch.Actions.Email.AuthPassword);
                Assert.Equal("sdk-username", testSearch.Actions.Email.AuthUsername);
                Assert.Equal("sdk-bcc@splunk.com", testSearch.Actions.Email.Bcc);
                Assert.Equal("sdk-cc@splunk.com", testSearch.Actions.Email.CC);
                Assert.Equal("$name1$", testSearch.Actions.Email.Command);
                Assert.Equal(EmailFormat.Plain, testSearch.Actions.Email.Format);
                Assert.Equal("sdk@splunk.com", testSearch.Actions.Email.From);
                //Assert.Equal("dummy1.host.com", savedSearch.Actions.Email.Hostname);
                Assert.True(testSearch.Actions.Email.Inline);
                //Assert.Equal("splunk.com", savedSearch.Actions.Email.MailServer);
                Assert.Equal(101, testSearch.Actions.Email.MaxResults);
                Assert.Equal("10s", testSearch.Actions.Email.MaxTime);
                //Assert.Equal("dummy", savedSearch.Actions.Email.PdfView);
                //Assert.Equal("*", savedSearch.Actions.Email.PreProcessResults);
                Assert.Equal(PaperOrientation.Landscape, testSearch.Actions.Email.ReportPaperOrientation);
                Assert.Equal(PaperSize.Letter, testSearch.Actions.Email.ReportPaperSize);
                Assert.False(testSearch.Actions.Email.ReportServerEnabled);
                //Assert.Equal("splunk.com", savedSearch.Actions.Email.ReportServerUrl);
                Assert.True(testSearch.Actions.Email.SendPdf);
                Assert.True(testSearch.Actions.Email.SendResults);
                Assert.Equal("sdk-subject", testSearch.Actions.Email.Subject);
                Assert.Equal("sdk-to@splunk.com", testSearch.Actions.Email.To);
                Assert.False(testSearch.Actions.Email.TrackAlert);
                Assert.Equal("61", testSearch.Actions.Email.Ttl);
                Assert.False(testSearch.Actions.Email.UseSsl);
                Assert.False(testSearch.Actions.Email.UseTls);
                Assert.False(testSearch.Actions.Email.WidthSortColumns);
                Assert.Equal("$name2$", testSearch.Actions.PopulateLookup.Command);
                Assert.Equal("dummypath", testSearch.Actions.PopulateLookup.Destination);
                Assert.Equal("dummy2.host.com", testSearch.Actions.PopulateLookup.Hostname);
                Assert.Equal(102, testSearch.Actions.PopulateLookup.MaxResults);
                Assert.Equal("20s", testSearch.Actions.PopulateLookup.MaxTime);
                Assert.False(testSearch.Actions.PopulateLookup.TrackAlert);
                Assert.Equal("62", testSearch.Actions.PopulateLookup.Ttl);
                Assert.Equal("$name3$", testSearch.Actions.Rss.Command);
                //Assert.Equal("dummy3.host.com", savedSearch.Actions.Rss.Hostname);
                Assert.Equal(103, testSearch.Actions.Rss.MaxResults);
                Assert.Equal("30s", testSearch.Actions.Rss.MaxTime);
                Assert.False(testSearch.Actions.Rss.TrackAlert);
                Assert.Equal("63", testSearch.Actions.Rss.Ttl);

                Assert.Equal("$name4$", testSearch.Actions.Script.Command);
                Assert.Equal("action_script_filename", testSearch.Actions.Script.FileName);
                Assert.Equal("dummy4.host.com", testSearch.Actions.Script.Hostname);
                Assert.Equal(104, testSearch.Actions.Script.MaxResults);
                Assert.Equal("40s", testSearch.Actions.Script.MaxTime);
                Assert.False(testSearch.Actions.Script.TrackAlert);
                Assert.Equal("64", testSearch.Actions.Script.Ttl);

                Assert.Equal("default", testSearch.Actions.SummaryIndex.Name);
                Assert.Equal("$name5$", testSearch.Actions.SummaryIndex.Command);
                //Assert.Equal("dummy5.host.com", savedSearch.Actions.SummaryIndex.Hostname);
                Assert.False(testSearch.Actions.SummaryIndex.Inline);
                Assert.Equal(105, testSearch.Actions.SummaryIndex.MaxResults);
                Assert.Equal("50s", testSearch.Actions.SummaryIndex.MaxTime);
                Assert.False(testSearch.Actions.SummaryIndex.TrackAlert);
                Assert.Equal("65", testSearch.Actions.SummaryIndex.Ttl);

                // Delete the saved search

                await testSearch.RemoveAsync();

                try
                {
                    await testSearch.GetAsync();
                    Assert.True(false);
                }
                catch (ResourceNotFoundException)
                { }

                testSearch = await savedSearches.GetOrNullAsync(testSearch.Name);
                Assert.Null(testSearch);
            }
        }

        /// <summary>
        /// Test saved search history
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.SavedSearch")]
        [Fact]
        public async Task SavedSearchHistory()
        {
            using (Service service = await SDKHelper.CreateService())
            {
                const string name = "sdk-test_SavedSearchHistory";
                const string search = "search index=sdk-tests * earliest=-1m";

                SavedSearchCollection savedSearches = service.SavedSearches;
                await savedSearches.GetSliceAsync(new SavedSearchCollection.Filter { Count = 0, SortDirection = SortDirection.Descending });

                SavedSearch savedSearch = savedSearches.SingleOrDefault(ss => ss.Name == name);
    
                if (savedSearch != null)
                {
                    await savedSearch.RemoveAsync();
                }

                // Create a saved search
                savedSearch = await savedSearches.CreateAsync(name, search);

                // Clear the history - even though we have a newly create saved search
                // it's possible there was a previous saved search with the same name
                // that had a matching history.

                JobCollection history = await savedSearch.GetHistoryAsync();

                foreach (Job job in history)
                {
                    await job.CancelAsync();
                }

                history = await savedSearch.GetHistoryAsync();
                Assert.Equal(0, history.Count);

                Job job1 = await savedSearch.DispatchAsync();
                history = await savedSearch.GetHistoryAsync();
                Assert.Equal(1, history.Count);
                Assert.True(history.Any(a => a.Sid == job1.Sid)); // this.Contains(history, job1.Sid));

                Job job2 = await savedSearch.DispatchAsync();
                history = await savedSearch.GetHistoryAsync();
                Assert.Equal(2, history.Count);
                Assert.True(history.Any(a => a.Sid == job1.Sid));
                Assert.True(history.Any(a => a.Sid == job2.Sid));

                await job1.CancelAsync();
                history = await savedSearch.GetHistoryAsync();
                Assert.Equal(1, history.Count);
                Assert.True(history.Any(a => a.Sid == job2.Sid));

                await job2.CancelAsync();
                history = await savedSearch.GetHistoryAsync();
                Assert.Equal(0, history.Count);

                //// Delete the saved search
                await savedSearches.GetSliceAsync(new SavedSearchCollection.Filter { Count = 0, SortDirection = SortDirection.Descending });
                savedSearch = savedSearches.SingleOrDefault(ss => ss.Name == name);
                Assert.NotNull(savedSearch);
                await savedSearch.RemoveAsync();
                savedSearch = await savedSearches.GetOrNullAsync(savedSearch.Name);
                Assert.Null(savedSearch);
            }
        }
    }
}
