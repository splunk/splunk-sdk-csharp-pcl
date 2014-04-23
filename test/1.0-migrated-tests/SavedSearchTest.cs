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

namespace Splunk.Client.UnitTesting
{
    using System;
    using Splunk.Client;
    using System.Linq.Expressions;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Tests saved searches
    /// </summary>
    public class SavedSearchTest : TestHelper
    {
        /// <summary>
        /// The assert root string
        /// </summary>
        private string assertRoot = "Saved Search assert: ";

        /// <summary>
        /// Returns a value indicating whether a specific Job SID exists
        /// in the job history.
        /// </summary>
        /// <param name="history">The job history</param>
        /// <param name="sid">The SID</param>
        /// <returns>True or false</returns>
        private bool Contains(Job[] history, string sid)
        {
            for (int i = 0; i < history.Length; ++i)
            {
                if (history[i].Sid.Equals(sid))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Touch test properties
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public void SavedSearches()
        {
            Service service = this.Connect();

            SavedSearchCollection savedSearches = service.GetSavedSearchesAsync().Result;

            // Iterate saved searches and make sure we can read them.
            foreach (SavedSearch savedSearch in savedSearches)
            {
                string dummyString;
                bool dummyBool;
                DateTime dummyDateTime;
                int dummyInt;

                // Resource properties
                //dummyString = savedSearch.Name;
                dummyString = savedSearch.Name;
                //dummyString = savedSearch.Path;
                // SavedSearch properties get
                //dummyString = savedSearch.Actions.Email.AuthPassword;
                //dummyString = savedSearch.Actions.Email.AuthUsername;
                dummyBool = savedSearch.Actions.Email.SendResults;
                //dummyString = savedSearch.Actions.Email.Bcc;
                //dummyString = savedSearch.Actions.Email.Cc;
                dummyString = savedSearch.Actions.Email.Command;
                //dummyString = savedSearch.Actions.Email.Format;
                dummyBool = savedSearch.Actions.Email.Inline;
                //dummyString = savedSearch.Actions.Email.MailServer;
                //dummyInt = savedSearch.Actions.Email.MaxResults;
                dummyString = savedSearch.Actions.Email.MaxTime;
                //dummyString = savedSearch.Actions.Email.ReportPaperOrientation;
                //dummyString = savedSearch.Actions.Email.ReportPaperSize;
                dummyBool = savedSearch.Actions.Email.ReportServerEnabled;
                //dummyString = savedSearch.Actions.Email.ReportServerUrl;
                dummyBool = savedSearch.Actions.Email.SendPdf;
                dummyBool = savedSearch.Actions.Email.SendResults;
                dummyString = savedSearch.Actions.Email.Subject;
                //dummyString = savedSearch.Actions.Email.To;
                dummyBool = savedSearch.Actions.Email.TrackAlert;
                dummyString = savedSearch.Actions.Email.Ttl;
                dummyBool = savedSearch.Actions.Email.UseSsl;
                dummyBool = savedSearch.Actions.Email.UseTls;
                dummyBool = savedSearch.Actions.Email.WidthSortColumns;
                dummyString = savedSearch.Actions.PopulateLookup.Command;
                //dummyString = savedSearch.Actions.PopulateLookup.Dest;
                //dummyString = savedSearch.Actions.PopulateLookup.Hostname;
                //dummyInt = savedSearch.Actions.PopulateLookup.MaxResults;
                dummyString = savedSearch.Actions.PopulateLookup.MaxTime;
                dummyBool = savedSearch.Actions.PopulateLookup.TrackAlert;
                dummyString = savedSearch.Actions.PopulateLookup.Ttl;
                dummyString = savedSearch.Actions.Rss.Command;
                //dummyString = savedSearch.Actions.Rss.Hostname;
                //dummyInt = savedSearch.Actions.Rss.MaxResults;
                dummyString = savedSearch.Actions.Rss.MaxTime;
                dummyBool = savedSearch.Actions.Rss.TrackAlert;
                dummyString = savedSearch.Actions.Rss.Ttl;
                //dummyString = savedSearch.Actions.ScriptCommand;
                //dummyString = savedSearch.ActionScriptFilename;
                //dummyString = savedSearch.ActionScriptHostname;
                //dummyInt = savedSearch.ActionScriptMaxResults;
                //dummyString = savedSearch.ActionScriptMaxTime;
                //dummyBool = savedSearch.ActionScriptTrackAlert;
                //dummyString = savedSearch.ActionScriptTtl;
                dummyString = savedSearch.Actions.SummaryIndex.Name;
                dummyString = savedSearch.Actions.SummaryIndex.Command;
                //dummyString = savedSearch.Actions.SummaryIndex.Hostname;
                dummyBool = savedSearch.Actions.SummaryIndex.Inline;
                //dummyInt = savedSearch.Actions.SummaryIndex.MaxResults;
                dummyString = savedSearch.Actions.SummaryIndex.MaxTime;
                dummyBool = savedSearch.Actions.SummaryIndex.TrackAlert;
                dummyString = savedSearch.Actions.SummaryIndex.Ttl;
                dummyBool = savedSearch.Alert.DigestMode;
                dummyString = savedSearch.Alert.Expires;
                //dummyInt = savedSearch.Alert.Severity;
                //dummyBool = savedSearch.Alert.Suppress;
                //dummyString = savedSearch.Alert.SuppressFields;
                //dummyString = savedSearch.Alert.SuppressPeriod;
                //dummyString = savedSearch.Alert.Track;
                //dummyString = savedSearch.Alert.Comparator;
                //dummyString = savedSearch.Alert.Condition;
                //dummyString = savedSearch.Alert.Threshold;
                //dummyString = savedSearch.Alert.Type;
                //dummyString = savedSearch.CronSchedule;
                //dummyString = savedSearch.Description;
                dummyInt = savedSearch.Dispatch.Buckets;
                dummyString = savedSearch.Dispatch.EarliestTime;
                //dummyString = savedSearch.Dispatch.LatestTime;
                dummyBool = savedSearch.Dispatch.Lookups;
                dummyInt = savedSearch.Dispatch.MaxCount;
                //dummyString = savedSearch.Dispatch.MaxTime;
                dummyInt = savedSearch.Dispatch.ReduceFreq;
                dummyBool = savedSearch.Dispatch.RealtimeBackfill;
                dummyBool = savedSearch.Dispatch.SpawnProcess;
                dummyString = savedSearch.Dispatch.TimeFormat;
                dummyString = savedSearch.Dispatch.Ttl;
                //dummyString = savedSearch.DisplayView;
                dummyInt = savedSearch.MaxConcurrent;
                //dummyDateTime = savedSearch.NextScheduledTime;
                dummyString = savedSearch.QualifiedSearch;
                //dummyInt = savedSearch.RealtimeSchedule;
                //dummyString = savedSearch.RequestUiDispatchApp;
                //dummyString = savedSearch.RequestUiDispatchView;
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

        /// <summary>
        /// Test Saved Search Create Read Update and Delete.
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public void SavedSearchesCRUD()
        {
            Service service = this.Connect();
            string savedSearchTitle = "sdk-test1";

            SavedSearchCollection savedSearches = service.GetSavedSearchesAsync().Result;

            // Ensure test starts in a known good state
            if (savedSearches.Where(a => a.Name == savedSearchTitle).Count() > 0)
            {
                service.RemoveSavedSearchAsync(savedSearchTitle).Wait();
                savedSearches.GetAsync().Wait();
            }

            Assert.False(savedSearches.Where(a => a.Name == savedSearchTitle).Count() > 0);

            SavedSearch savedSearch;
            string search = "search index=sdk-tests * earliest=-1m";

            // Create a saved search
            //savedSearches.Create("sdk-test1", search);
            SavedSearchAttributes attrs = new SavedSearchAttributes() { Search = search };
            service.CreateSavedSearchAsync(savedSearchTitle, attrs).Wait();
            savedSearches.GetAsync().Wait();
            Assert.True(savedSearches.Where(a => a.Name == savedSearchTitle).Count() > 0);

            // Read the saved search           
            //savedSearch = savedSearches.Get("sdk-test1");           
            savedSearch = savedSearches.Where(a => a.Name == savedSearchTitle).SingleOrDefault();
            Assert.True(savedSearch.IsVisible);

            // CONSIDER: Test some additinal default property values.

            // Update search properties, but don't specify required args to test
            // pulling them from the existing object
            savedSearch.UpdateAsync(new SavedSearchAttributes() { IsVisible = false }, null, null).Wait();
            //savedSearch.Refresh();
            Assert.False(savedSearch.IsVisible);

            // Delete the saved search            
            //savedSearches.("sdk-test1");
            service.RemoveSavedSearchAsync(savedSearchTitle).Wait();
            savedSearches.GetAsync().Wait();
            Assert.False(savedSearches.Where(a => a.Name == savedSearchTitle).Count() > 0);

            // Create a saved search with some additional arguments
            //savedSearch = savedSearches.Create("sdk-test1", search, new Args("is_visible", false));
            savedSearch = service.CreateSavedSearchAsync(savedSearchTitle, new SavedSearchAttributes() { Search = search, IsVisible = false }).Result;
            Assert.False(savedSearch.IsVisible);

            // set email params
            attrs = new SavedSearchAttributes();
            //attrs.ActionEmailAuthPassword = "sdk-password";
            //attrs.ActionEmailAuthUsername = "sdk-username";
            //attrs.ActionEmailBcc = "sdk-bcc@splunk.com";
            //attrs.ActionEmailCc = "sdk-cc@splunk.com";
            attrs.ActionEmailCommand = "$name1$";
            attrs.ActionEmailFormat = EmailFormat.Plain;
            attrs.ActionEmailFrom = "sdk@splunk.com";
            //attrs.ActionEmailHostname = "dummy1.host.com";
            attrs.ActionEmailInline = "true";
            attrs.ActionEmailMailServer = "splunk.com";
            attrs.ActionEmailMaxResults = 101;
            attrs.ActionEmailMaxTime = "10s";
            //attrs.ActionEmailPdfView = "dummy";
            //attrs.ActionEmailPreProcessResults = "*";
            attrs.ActionEmailReportPaperOrientation = PaperOrientation.Landscape;
            attrs.ActionEmailReportPaperSize = PaperSize.Letter;
            attrs.ActionEmailReportServerEnabled = false;
            //attrs.ActionEmailReportServerUrl = "splunk.com";
            attrs.ActionEmailSendPdf = false;
            attrs.ActionEmailSendResults = false;
            attrs.ActionEmailSubject = "sdk-subject";
            //attrs.ActionEmailTo = "sdk-to@splunk.com";
            attrs.ActionEmailTrackAlert = false;
            attrs.ActionEmailTtl = "61";
            attrs.ActionEmailUseSsl = false;
            attrs.ActionEmailUseTls = false;
            attrs.ActionEmailWidthSortColumns = false;
            attrs.ActionPopulateLookupCommand = "$name2$";
            //attrs.ActionPopulateLookupDest = "dummypath";
            //attrs.ActionPopulateLookupHostname = "dummy2.host.com";
            attrs.ActionPopulateLookupMaxResults = 102;
            attrs.ActionPopulateLookupMaxTime = "20s";
            attrs.ActionPopulateLookupTrackAlert = false;
            attrs.ActionPopulateLookupTtl = "62";
            attrs.ActionRssCommand = "$name3$";
            //attrs.ActionRssHostname = "dummy3.host.com";
            attrs.ActionRssMaxResults = 103;
            attrs.ActionRssMaxTime = "30s";
            attrs.ActionRssTrackAlert = "false";
            attrs.ActionRssTtl = "63";
            attrs.ActionScriptCommand = "$name4$";

            const string ActionScriptFilename = "action_script_filename";
            //attrs.ActionScriptFilename = ActionScriptFilename;
            //attrs.ActionScriptHostname = "dummy4.host.com";
            attrs.ActionScriptMaxResults = 104;
            attrs.ActionScriptMaxTime = "40s";
            attrs.ActionScriptTrackAlert = false;
            attrs.ActionScriptTtl = "64";
            attrs.ActionSummaryIndexName = "default";
            attrs.ActionSummaryIndexCommand = "$name5$";
            //attrs.ActionSummaryIndexHostname = "dummy5.host.com";
            attrs.ActionSummaryIndexInline = false;
            attrs.ActionSummaryIndexMaxResults = 105;
            attrs.ActionSummaryIndexMaxTime = "50s";
            attrs.ActionSummaryIndexTrackAlert = false;
            attrs.ActionSummaryIndexTtl = "65";
            attrs.Actions = "rss,email,populate_lookup,script,summary_index";
            attrs.Search = search;

            savedSearch.UpdateAsync(attrs, null, null).Wait();

            // check
            //Assert.True(savedSearch.IsActionEmail);
            //Assert.True(savedSearch.IsActionPopulateLookup);
            //Assert.True(savedSearch.IsActionRss);
            //Assert.True(savedSearch.IsActionScript);
            //Assert.True(savedSearch.IsActionsSummaryIndex);

            //Assert.Equal("sdk-password", savedSearch.Actions.Email.AuthPassword);
            //Assert.Equal("sdk-username", savedSearch.Actions.Email.AuthUsername);
            //Assert.Equal("sdk-bcc@splunk.com", savedSearch.Actions.Email.Bcc);
            //Assert.Equal("sdk-cc@splunk.com", savedSearch.Actions.Email.Cc);
            Assert.Equal("$name1$", savedSearch.Actions.Email.Command);
            Assert.Equal(EmailFormat.Plain, savedSearch.Actions.Email.Format);
            Assert.Equal("sdk@splunk.com", savedSearch.Actions.Email.From);
            //Assert.Equal("dummy1.host.com", savedSearch.Actions.Email.Hostname);
            Assert.True(savedSearch.Actions.Email.Inline);
            //Assert.Equal("splunk.com", savedSearch.Actions.Email.MailServer);
            Assert.Equal(101, savedSearch.Actions.Email.MaxResults);
            Assert.Equal("10s", savedSearch.Actions.Email.MaxTime);
            //Assert.Equal("dummy", savedSearch.Actions.Email.PdfView);
            //Assert.Equal("*", savedSearch.Actions.Email.PreProcessResults);
            Assert.Equal(PaperOrientation.Landscape, savedSearch.Actions.Email.ReportPaperOrientation);
            Assert.Equal(PaperSize.Letter, savedSearch.Actions.Email.ReportPaperSize);
            Assert.False(savedSearch.Actions.Email.ReportServerEnabled);
            //Assert.Equal("splunk.com", savedSearch.Actions.Email.ReportServerUrl);
            Assert.False(savedSearch.Actions.Email.SendPdf);
            Assert.False(savedSearch.Actions.Email.SendResults);
            Assert.Equal("sdk-subject", savedSearch.Actions.Email.Subject);
            //Assert.Equal("sdk-to@splunk.com", savedSearch.Actions.Email.To);
            Assert.False(savedSearch.Actions.Email.TrackAlert);
            Assert.Equal("61", savedSearch.Actions.Email.Ttl);
            Assert.False(savedSearch.Actions.Email.UseSsl);
            Assert.False(savedSearch.Actions.Email.UseTls);
            Assert.False(savedSearch.Actions.Email.WidthSortColumns);
            Assert.Equal("$name2$", savedSearch.Actions.PopulateLookup.Command);
            //Assert.Equal("dummypath", savedSearch.Actions.PopulateLookup.Dest);
            //Assert.Equal("dummy2.host.com", savedSearch.Actions.PopulateLookup.Hostname);
            Assert.Equal(102, savedSearch.Actions.PopulateLookup.MaxResults);
            Assert.Equal("20s", savedSearch.Actions.PopulateLookup.MaxTime);
            Assert.False(savedSearch.Actions.PopulateLookup.TrackAlert);
            Assert.Equal("62", savedSearch.Actions.PopulateLookup.Ttl);
            Assert.Equal("$name3$", savedSearch.Actions.Rss.Command);
            //Assert.Equal("dummy3.host.com", savedSearch.Actions.Rss.Hostname);
            Assert.Equal(103, savedSearch.Actions.Rss.MaxResults);
            Assert.Equal("30s", savedSearch.Actions.Rss.MaxTime);
            Assert.False(savedSearch.Actions.Rss.TrackAlert);
            Assert.Equal("63", savedSearch.Actions.Rss.Ttl);
            //Assert.Equal("$name4$", savedSearch.ActionScriptCommand);

            //Assert.Equal(ActionScriptFilename, savedSearch.ActionScriptFilename);

            //Assert.Equal("dummy4.host.com", savedSearch.ActionScriptHostname);
            //Assert.Equal(104, savedSearch.ActionScriptMaxResults);
            //Assert.Equal("40s", savedSearch.ActionScriptMaxTime);
            //Assert.False(savedSearch.ActionScriptTrackAlert);
            //Assert.Equal("64", savedSearch.ActionScriptTtl);
            Assert.Equal("default", savedSearch.Actions.SummaryIndex.Name);
            Assert.Equal("$name5$", savedSearch.Actions.SummaryIndex.Command);
            //Assert.Equal("dummy5.host.com", savedSearch.Actions.SummaryIndex.Hostname);
            Assert.False(savedSearch.Actions.SummaryIndex.Inline);
            Assert.Equal(105, savedSearch.Actions.SummaryIndex.MaxResults);
            Assert.Equal("50s", savedSearch.Actions.SummaryIndex.MaxTime);
            Assert.False(savedSearch.Actions.SummaryIndex.TrackAlert);
            Assert.Equal("65", savedSearch.Actions.SummaryIndex.Ttl);

            // Delete the saved search - using alternative method
            //savedSearch.Remove();
            //savedSearches.Refresh();
            service.RemoveSavedSearchAsync(savedSearchTitle).Wait();
            savedSearches.GetAsync().Wait();
            Assert.False(savedSearches.Where(a => a.Name == savedSearchTitle).Count() > 0);
        }

        /// <summary>
        /// Test saved search dispatch
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public void SavedSearchDispatch()
        {
            Service service = this.Connect();
            string savedSearchTitle = "sdk-test1";
            SavedSearchCollection savedSearches = service.GetSavedSearchesAsync().Result;

            // Ensure test starts in a known good state
            if (savedSearches.Where(a => a.Name == savedSearchTitle).Count() > 0)
            {
                service.RemoveSavedSearchAsync(savedSearchTitle).Wait();
                savedSearches.GetAsync().Wait();
            }

            Assert.False(savedSearches.Where(a => a.Name == savedSearchTitle).Count() > 0);

            // Create a saved search
            string search = "search index=sdk-tests * earliest=-1m";
            SavedSearch savedSearch = service.CreateSavedSearchAsync(savedSearchTitle, new SavedSearchAttributes() { Search = search }).Result;
            
            // Dispatch the saved search and wait for results.
            Job job = savedSearch.DispatchAsync().Result;
            //job.Cancel();

            // Dispatch with some additional search options
            job = savedSearch.DispatchAsync(new SavedSearchDispatchArgs(){DispatchBuckets= 100}).Result;
            this.Wait(job);
            //job.Timeline().Close();
            //job.Cancel();
            job.CancelAsync().Wait();

            // Dispatch with some additional search options
            job = savedSearch.DispatchAsync(new SavedSearchDispatchArgs() { DispatchEarliestTime = "aaaa" }).Result;
            this.Wait(job);
            //job.Timeline().Close();
            job.CancelAsync().Wait();
                        
            var savedSearchDispatchArgs = new SavedSearchTemplateArgs();
            if (this.VersionCompare(service, "6.0") < 0)
            {
                
                //savedSearchDispatchArgs.Email.AuthPassword = "sdk-password";
                //savedSearchDispatchArgs.Actions.Email.AuthUsername = "sdk-username";
                //savedSearchDispatchArgs.Actions.Email.Bcc = "sdk-bcc@splunk.com";
                //savedSearchDispatchArgs.Actions.Email.Cc = "sdk-cc@splunk.com";
                //savedSearchDispatchArgs.Actions.Email.Command = "$name1$";
                //savedSearchDispatchArgs.Actions.Email.Format = "text";
                //savedSearchDispatchArgs.Actions.Email.From = "sdk@splunk.com";
                //savedSearchDispatchArgs.Actions.Email.Hostname = "dummy1.host.com";
                //savedSearchDispatchArgs.Actions.Email.Inline = true;
                //savedSearchDispatchArgs.Actions.Email.MailServer = "splunk.com";
                //savedSearchDispatchArgs.Actions.Email.MaxResults = 101;
                //savedSearchDispatchArgs.Actions.Email.MaxTime = "10s";
                //savedSearchDispatchArgs.Actions.Email.PdfView = "dummy";
                //savedSearchDispatchArgs.Actions.Email.ReportPaperOrientation = "landscape";
                //savedSearchDispatchArgs.Actions.Email.ReportPaperSize = "letter";
                //savedSearchDispatchArgs.Actions.Email.ReportServerEnabled = false;
                //savedSearchDispatchArgs.Actions.Email.ReportServerUrl = "splunk.com";
                //savedSearchDispatchArgs.Actions.Email.SendPdf = false;
                //savedSearchDispatchArgs.Actions.Email.SendResults = false;
                //savedSearchDispatchArgs.Actions.Email.Subject = "sdk-subject";
                //savedSearchDispatchArgs.Actions.Email.To = "sdk-to@splunk.com";
                //savedSearchDispatchArgs.Actions.Email.TrackAlert = false;
                //savedSearchDispatchArgs.Actions.Email.Ttl = "61";
                //savedSearchDispatchArgs.Actions.Email.UseSsl = false;
                //savedSearchDispatchArgs.Actions.Email.UseTls = false;
                //savedSearchDispatchArgs.Actions.Email.WidthSortColumns = false;
                //savedSearchDispatchArgs.Actions.PopulateLookup.Command = "$name2$";
                //savedSearchDispatchArgs.Actions.PopulateLookup.Dest = "dummypath";
                //savedSearchDispatchArgs.Actions.PopulateLookup.Hostname = "dummy2.host.com";
                //savedSearchDispatchArgs.Actions.PopulateLookup.MaxResults = 102;
                //savedSearchDispatchArgs.Actions.PopulateLookup.MaxTime = "20s";
                //savedSearchDispatchArgs.Actions.PopulateLookup.TrackAlert = false;
                //savedSearchDispatchArgs.Actions.PopulateLookup.Ttl = "62";
                //savedSearchDispatchArgs.Actions.Rss.Command = "$name3$";
                //savedSearchDispatchArgs.Actions.Rss.Hostname = "dummy3.host.com";
                //savedSearchDispatchArgs.Actions.Rss.MaxResults = 103;
                //savedSearchDispatchArgs.Actions.Rss.MaxTime = "30s";
                //savedSearchDispatchArgs.Actions.Rss.TrackAlert = false;
                //savedSearchDispatchArgs.Actions.Rss.Ttl = "63";
                //savedSearchDispatchArgs.ActionScriptCommand = "$name4$";
                //savedSearchDispatchArgs.ActionScriptFilename = "action_script_filename";
                //savedSearchDispatchArgs.ActionScriptHostname = "dummy4.host.com";
                //savedSearchDispatchArgs.ActionScriptMaxResults = 104;
                //savedSearchDispatchArgs.ActionScriptMaxTime = "40s";
                //savedSearchDispatchArgs.ActionScriptTrackAlert = false;
                //savedSearchDispatchArgs.ActionScriptTtl = "64";
                //savedSearchDispatchArgs.Actions.SummaryIndex.Command = "$name5$";
                //savedSearchDispatchArgs.Actions.SummaryIndex.Hostname = "dummy5.host.com";
                //savedSearchDispatchArgs.Actions.SummaryIndex.Inline = false;
                //savedSearchDispatchArgs.Actions.SummaryIndex.MaxResults = 105;
                //savedSearchDispatchArgs.Actions.SummaryIndex.MaxTime = "50s";
                //savedSearchDispatchArgs.Actions.SummaryIndex.TrackAlert = false;
                //savedSearchDispatchArgs.Actions.SummaryIndex.Ttl = "65";
                //savedSearchDispatchArgs.Actions = "rss,email,populate_lookup,script,summary_index";
            }

            //// Same as the previous dispatch except using custom arg
            //job = savedSearch.DispatchAsync(savedSearchDispatchArgs);
            //this.Wait(job);
            //job.Timeline().Close();
            //job.Cancel();

            // Delete the saved search
            service.RemoveSavedSearchAsync(savedSearchTitle).Wait();            
            Assert.False(savedSearches.Where(a => a.Name == savedSearchTitle).Count() > 0);
        }

        /// <summary>
        /// Test saved search history
        /// </summary>
        [Trait("class", "Service")]
        [Fact]
        public void SavedSearchHistory()
        {
            Service service = this.Connect();
            string savedSearchTitle = "sdk-test1";
            SavedSearchCollection savedSearches = service.GetSavedSearchesAsync().Result;

            // Ensure test starts in a known good state
            if (savedSearches.Where(a => a.Name == savedSearchTitle).Count() > 0)
            {
                service.RemoveSavedSearchAsync(savedSearchTitle).Wait();
                savedSearches.GetAsync().Wait();
            }

            Assert.False(savedSearches.Where(a => a.Name == savedSearchTitle).Count() > 0);

            string search = "search index=sdk-tests * earliest=-1m";

            // Create a saved search
            SavedSearch savedSearch = service.CreateSavedSearchAsync(savedSearchTitle, new SavedSearchAttributes() { Search = search }).Result;
      
            // Clear the history - even though we have a newly create saved search
            // it's possible there was a previous saved search with the same name
            // that had a matching history.
            JobCollection history = savedSearch.GetHistoryAsync().Result;
            foreach (Job job in history)
            {
                job.CancelAsync().Wait();                
            }

            history = savedSearch.GetHistoryAsync().Result;
            Assert.Equal(0, history.Count);

            Job job1 = savedSearch.DispatchAsync().Result;
            this.Ready(job1);
            history = savedSearch.GetHistoryAsync().Result;
            Assert.Equal(1, history.Count);
            Assert.True(history.Where(a=>a.Sid==job1.Sid).Count()>0);// this.Contains(history, job1.Sid));

            Job job2 = savedSearch.DispatchAsync().Result;
            this.Ready(job2);
            history = savedSearch.GetHistoryAsync().Result;
            Assert.Equal(2, history.Count);
            Assert.True(history.Where(a=>a.Sid==job1.Sid).Count()>0);
            Assert.True(history.Where(a=>a.Sid==job2.Sid).Count()>0);

            job1.CancelAsync().Wait();
            history = savedSearch.GetHistoryAsync().Result;
            Assert.Equal(1, history.Count);
            Assert.True(history.Where(a=>a.Sid==job2.Sid).Count()>0);

            job2.CancelAsync().Wait();
            history = savedSearch.GetHistoryAsync().Result;
            Assert.Equal(0, history.Count);

            //// Delete the saved search
            //savedSearches.Remove("sdk test1");
            //Assert.False(savedSearches.ContainsKey("sdk test1"));
        }
    }
}