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

// TODO:
//
// [O] Contracts
//
// [O] Documentation
//
// [ ] Trace messages

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public sealed class SavedSearch : Entity<SavedSearch>
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <param name="namespace">
        /// </param>
        /// <param name="name">
        /// </param>
        internal SavedSearch(Context context, Namespace @namespace, string name)
            : base(context, @namespace, SavedSearchCollection.ClassResourceName, name)
        { }

        public SavedSearch()
        { }

        #endregion

        #region Properties

        public Action_t Actions
        {
            get { return this.Content.GetValue("Action", Action_t.Converter.Instance); }
        }

        public Alert_t Alert
        {
            get { return this.Content.GetValue("Alert", Alert_t.Converter.Instance); }
        }

        public AutoSummarize_t AutoSummarize
        {
            get { return this.Content.GetValue("AutoSummarize", AutoSummarize_t.Converter.Instance); }
        }

        public Dispatch_t Dispatch
        {
            get { return this.Content.GetValue("Dispatch", Dispatch_t.Converter.Instance); }
        }

        public Display_t Display
        {
            get { return this.Content.GetValue("Display", Display_t.Converter.Instance); }
        }

        public Eai Eai
        {
            get { return this.Content.GetValue("Eai", Eai.Converter.Instance); }
        }

        public bool IsDisabled
        {
            get { return this.Content.GetValue("IsDisabled", BooleanConverter.Instance); }
        }

        public bool IsScheduled
        {
            get { return this.Content.GetValue("IsScheduled", BooleanConverter.Instance); }
        }

        public bool IsVisible
        {
            get { return this.Content.GetValue("IsVisible", BooleanConverter.Instance); }
        }

        public int MaxConcurrent
        {
            get { return this.Content.GetValue("MaxConcurrent", Int32Converter.Instance); }
        }

        public bool RealtimeSchedule
        {
            get { return this.Content.GetValue("RealtimeSchedule", BooleanConverter.Instance); }
        }

        public Request_t Request
        {
            get { return this.Content.GetValue("Request", Request_t.Converter.Instance); }
        }

        public bool RestartOnSearchPeerAdd
        {
            get { return this.Content.GetValue("RestartOnSearchpeerAdd", BooleanConverter.Instance); }
        }

        public string QualifiedSearch
        {
            get { return this.Content.GetValue("QualifiedSearch", StringConverter.Instance); }
        }

        public bool RunOnStartup
        {
            get { return this.Content.GetValue("RunOnStartup", BooleanConverter.Instance); }
        }

        public string Search
        {
            get { return this.Content.GetValue("Search", StringConverter.Instance); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously creates a new saved search.
        /// </summary>
        /// <param name="searchName">
        /// Name of the saved search to be created.
        /// </param>
        /// <param name="attributes">
        /// Attributes of the saved search to be created.
        /// </param>
        /// <param name="dispatchArgs">
        /// Dispatch arguments for the saved search to be created.
        /// </param>
        /// <param name="templateArgs">
        /// Template arguments for the saved search to be created.
        /// </param>
        /// This method uses the <a href="http://goo.gl/EPQypw">POST 
        /// saved/searches</a> endpoint to create the <see cref="SavedSearch"/>
        /// represented by the current instance.
        /// </remarks>
        public async Task CreateAsync(SavedSearchAttributes attributes, SavedSearchDispatchArgs dispatchArgs = null, 
            SavedSearchTemplateArgs templateArgs = null)
        {
            var args = new Argument[] { new Argument("name", this.ResourceName.Title) };

            using (var response = await this.Context.PostAsync(this.Namespace, SavedSearchCollection.ClassResourceName, args, 
                attributes, dispatchArgs, templateArgs))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
                await this.UpdateDataAsync(response);
            }
        }

        /// <summary>
        /// Dispatches the current <see cref="SavedSearch"/> just like the 
        /// scheduler would.
        /// </summary>
        /// <param name="dispatchArgs">
        /// A set of template arguments to the saved search.
        /// </param>
        /// <param name="dispatchArgs">
        /// A set of arguments to the dispatcher.
        /// </param>
        /// <returns>
        /// An object representing the search job that was created by the 
        /// dispatcher.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/AfzBJO">POST 
        /// saved/searches/{name}/dispatch</a> endpoint to dispatch the 
        /// current <see cref="SavedSearch"/>.
        /// </remarks>
        public async Task<Job> DispatchAsync(SavedSearchDispatchArgs
            dispatchArgs = null, SavedSearchTemplateArgs templateArgs = null)
        {
            string searchId;

            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, templateArgs, 
                dispatchArgs))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                searchId = await response.XmlReader.ReadResponseElementAsync("sid");
            }

            Job job = new Job(this.Context, this.Namespace, searchId);
            await job.GetAsync();
            return job;
        }

        /// <summary>
        /// Asynchronously retrieves information about the current instance.
        /// </summary>
        /// <param name="args">
        /// Constrains the information returned about the current instance.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/L4JLwn">GET 
        /// saved/searches/{name}</a> endpoint to get information about the
        /// current <see cref="SavedSearch"/> instance.
        /// </remarks>
        public async Task GetAsync(SavedSearchFilterArgs args)
        {
            using (var response = await this.Context.GetAsync(this.Namespace, this.ResourceName, args))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.UpdateDataAsync(response);
            }
        }

        /// <summary>
        /// Asynchronously retrieves the collection of jobs created from the
        /// current instance.
        /// </summary>
        /// <returns>
        /// An object representing the collection of jobs created from the
        /// current instance.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/kv9L1l">GET 
        /// saved/searches/{name}/history</a> endpoint to construct the <see 
        /// cref="JobCollection"/> object it returns.
        /// </remarks>
        public async Task<JobCollection> GetHistoryAsync()
        {
            var jobs = new JobCollection(this.Context, this.Namespace, new ResourceName(this.ResourceName, "history"));
            await jobs.GetAsync();
            return jobs;
        }

        /// <summary>
        /// Asynchronously gets the scheduled times for the saved search 
        /// represented by the current instance.
        /// </summary>
        /// <param name="earliestTime">
        /// An absolute or relative time string specifying a lower-bound for
        /// the scheduled time range.
        /// </param>
        /// <param name="latestTime">
        /// An absolute or relative time string specifying a upper-bound for
        /// the scheduled time range.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/sn7qC5">DELETE 
        /// saved/searches/{name}</a> endpoint to remove the saved search
        /// represented by the current instance.
        /// </remarks>
        public async Task GetScheduledTimesAsync(string earliestTime, string latestTime)
        {
            using (var response = await this.Context.GetAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.UpdateDataAsync(response);
            }
        }

        /// <summary>
        /// Asynchronously removes the saved search represented by the current
        /// instance.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/sn7qC5">DELETE 
        /// saved/searches/{name}</a> endpoint to remove the saved search
        /// represented by the current instance.
        /// </remarks>
        public async Task RemoveAsync()
        {
            using (var response = await this.Context.DeleteAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Asynchronously reschedules the saved search represented by the 
        /// current instance.
        /// </summary>
        /// <param name="scheduleTime">
        /// A time string specifying the next time to run the search. This 
        /// value defaults to null indicating that the saved search should
        /// be run as soon as possible.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SJcEr5">POST 
        /// saved/searches/{name}/reschedule</a> endpoint to reschedule the 
        /// saved search represented by the current instance.
        /// </remarks>
        public async Task RescheduleAsync(string scheduleTime = null)
        {
            var resourceName = new ResourceName(this.ResourceName, "reschedule");
            
            Argument[] args = scheduleTime == null ? null : new Argument[] 
            { 
                new Argument("schedule_time", scheduleTime) 
            };

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, args) )
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Asynchronously updates the saved search represented by the current
        /// instance.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="SavedSearch"/> to be updated.
        /// </param>
        /// <param name="name">
        /// Name of the saved search to be updated.
        /// </param>
        /// <param name="attributes">
        /// New attributes for the saved search to be updated.
        /// </param>
        /// <param name="dispatchArgs">
        /// New dispatch arguments for the saved search to be updated.
        /// </param>
        /// <param name="templateArgs">
        /// New template arguments for the saved search to be updated.
        /// </param>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/aV9eiZ">POST 
        /// saved/searches{name}</a> endpoint to update the saved search
        /// represented by the current instance.
        /// </remarks>

        public async Task UpdateAsync(SavedSearchAttributes attributes, SavedSearchDispatchArgs dispatchArgs, 
            SavedSearchTemplateArgs templateArgs)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, attributes, 
                dispatchArgs, templateArgs))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.UpdateDataAsync(response);
            }
        }

        #endregion

        #region Privates/internals

        async Task UpdateDataAsync(Response response)
        {
            var feed = new AtomFeed();
            await feed.ReadXmlAsync(response.XmlReader);

            if (feed.Entries.Count != 1)
            {
                throw new InvalidDataException();  // TODO: Diagnostics
            }

            this.Data = new DataCache(feed.Entries[0]);
        }
        
        #endregion

        #region Types

        public class Action_t : ExpandoAdapter<Action_t>
        {
            #region Properties

            public Email_t Email
            {
                get { return this.GetValue("Email", Email_t.Converter.Instance); }
            }

            public PopulateLookup_t PopulateLookup
            {
                get { return this.GetValue("PopulateLookup", PopulateLookup_t.Converter.Instance); }
            }

            public Rss_t Rss
            {
                get { return this.GetValue("Rss", Rss_t.Converter.Instance); }
            }

            public SummaryIndex_t SummaryIndex
            {
                get { return this.GetValue("SummaryIndex", SummaryIndex_t.Converter.Instance); }
            }

            #endregion

            #region Types

            public class Action<TAction> : ExpandoAdapter<TAction> where TAction : Action<TAction>, new()
            {
                #region Properties

                public string Command
                {
                    get { return this.GetValue("Command", StringConverter.Instance); }
                }

                public bool IsEnabled
                {
                    get { return this.GetValue("IsEnabled", BooleanConverter.Instance); }
                }

                public string MaxResults
                {
                    get { return this.GetValue("Maxresults", StringConverter.Instance); }
                }

                public string MaxTime
                {
                    get { return this.GetValue("Maxtime", StringConverter.Instance); }
                }

                public bool TrackAlert
                {
                    get { return this.GetValue("TrackAlert", BooleanConverter.Instance); }
                }

                public string Ttl
                {
                    get { return this.GetValue("Ttl", StringConverter.Instance); }
                }

                #endregion
            }

            public class Email_t : Action<Email_t>
            {
                #region Properties

                public EmailFormat Format
                {
                    get { return this.GetValue("Format", EnumConverter<EmailFormat>.Instance); }
                }

                public string From
                {
                    get { return this.GetValue("From", StringConverter.Instance); }
                }

                public bool Inline
                {
                    get { return this.GetValue("Inline", BooleanConverter.Instance); }
                }

                public string Mailserver
                {
                    get { return this.GetValue("Mailserver", StringConverter.Instance); }
                }

                public string ReportCIDFontList
                {
                    get { return this.GetValue("ReportCIDFontList", StringConverter.Instance); }
                }

                public bool ReportIncludeSplunkLogo
                {
                    get { return this.GetValue("ReportIncludeSplunkLogo", BooleanConverter.Instance); }
                }

                public PaperOrientation ReportPaperOrientation
                {
                    get { return this.GetValue("ReportPaperOrientation", EnumConverter<PaperOrientation>.Instance); }
                }

                public PaperSize ReportPaperSize
                {
                    get { return this.GetValue("ReportPaperSize", EnumConverter<PaperSize>.Instance); }
                }

                public bool ReportServerEnabled
                {
                    get { return this.GetValue("ReportServerEnabled", BooleanConverter.Instance); }
                }

                public bool SendPdf
                {
                    get { return this.GetValue("Sendpdf", BooleanConverter.Instance); }
                }

                public bool SendResults
                {
                    get { return this.GetValue("Sendresults", BooleanConverter.Instance); }
                }

                public string Subject
                {
                    get { return this.GetValue("Subject", StringConverter.Instance); }
                }

                public bool UseSsl
                {
                    get { return this.GetValue("UseSsl", BooleanConverter.Instance); }
                }

                public bool UseTls
                {
                    get { return this.GetValue("UseTls", BooleanConverter.Instance); }
                }

                public bool WidthSortColumns
                {
                    get { return this.GetValue("WidthSortColumns", BooleanConverter.Instance); }
                }

                #endregion
            }

            public class PopulateLookup_t : Action<PopulateLookup_t>
            { }

            public class Rss_t : Action<Rss_t>
            { }

            public class SummaryIndex_t : Action<SummaryIndex_t>
            {
                #region Properties

                public bool Inline
                {
                    get { return this.GetValue("Inline", BooleanConverter.Instance); }
                }

                public string Name
                {
                    get { return this.GetValue("Name", StringConverter.Instance); }
                }

                #endregion
            }
            
            #endregion
        }

        public class Alert_t : ExpandoAdapter<Alert_t>
        {
            #region Properties

            public bool DigestMode
            {
                get { return this.GetValue("DigestMode", BooleanConverter.Instance); }
            }

            public string Expires
            {
                get { return this.GetValue("Expires", StringConverter.Instance); }
            }

            public AlertSeverity Severity
            {
                get { return this.GetValue("Severity", EnumConverter<AlertSeverity>.Instance); }
            }

            public bool Track
            {
                get { return this.GetValue("Track", BooleanConverter.Instance); }
            }

            public AlertTrigger Trigger
            {
                get { return this.GetValue("Track", EnumConverter<AlertTrigger>.Instance); }
            }

            #endregion
        }

        public class AutoSummarize_t : ExpandoAdapter<AutoSummarize_t>
        {
            #region Properties

            public string Command
            {
                get { return this.GetValue("Command", StringConverter.Instance); }
            }

            public bool IsEnabled
            {
                get { return this.GetValue("IsEnabled", BooleanConverter.Instance); }
            }

            public string CronSchedule
            {
                get { return this.GetValue("CronSchedule", StringConverter.Instance); }
            }

            public Dispatch_t Dispatch
            {
                get { return this.GetValue("Dispatch", Dispatch_t.Converter.Instance); }
            }

            public int MaxDisabledBuckets
            {
                get { return this.GetValue("MaxDisabledBuckets", Int32Converter.Instance); }
            }

            public double MaxSummaryRatio
            {
                get { return this.GetValue("MaxSummaryRatio", DoubleConverter.Instance); }
            }

            public int MaxSummarySize
            {
                get { return this.GetValue("MaxSummarySize", Int32Converter.Instance); }
            }

            public int MaxTime
            {
                get { return this.GetValue("MaxTime", Int32Converter.Instance); }
            }

            public string SuspendPeriod
            {
                get { return this.GetValue("SuspendPeriod", StringConverter.Instance); }
            }

            #endregion

            #region Types

            public class Dispatch_t : ExpandoAdapter<Dispatch_t>
            {
                public string EarliestTime
                {
                    get { return this.GetValue("EarliestTime", StringConverter.Instance); }
                }

                public string LatestTime
                {
                    get { return this.GetValue("LatestTime", StringConverter.Instance); }
                }

                public string TimeFormat
                {
                    get { return this.GetValue("TimeFormat", StringConverter.Instance); }
                }

                public string Ttl
                {
                    get { return this.GetValue("Ttl", StringConverter.Instance); }
                }
            }

            #endregion
        }

        public class Dispatch_t : ExpandoAdapter<Dispatch_t>
        {
            #region Properties

            public int Buckets
            {
                get { return this.GetValue("Buckets", Int32Converter.Instance); }
            }

            public string EarliestTime
            {
                get { return this.GetValue("EarliestTime", StringConverter.Instance); }
            }

            public bool Lookups
            {
                get { return this.GetValue("Lookups", BooleanConverter.Instance); }
            }

            public int MaxCount
            {
                get { return this.GetValue("MaxCount", Int32Converter.Instance); }
            }

            public int MaxTime
            {
                get { return this.GetValue("MaxTime", Int32Converter.Instance); }
            }

            public int ReduceFreq
            {
                get { return this.GetValue("ReduceFreq", Int32Converter.Instance); }
            }

            public bool RealtimeBackfill
            {
                get { return this.GetValue("RealtimeBackfill", BooleanConverter.Instance); }
            }

            public bool SpawnProcess
            {
                get { return this.GetValue("SpawnProcess", BooleanConverter.Instance); }
            }

            public string TimeFormat
            {
                get { return this.GetValue("TimeFormat", StringConverter.Instance); }
            }

            public string Ttl
            {
                get { return this.GetValue("Ttl", StringConverter.Instance); }
            }

            #endregion
        }

        public class Display_t : ExpandoAdapter<Display_t>
        {
            #region Properties

            public Events_t Events
            {
                get { return this.GetValue("Events", Events_t.Converter.Instance); }
            }

            public General_t General
            {
                get { return this.GetValue("General", General_t.Converter.Instance); }
            }

            public Page_t Page
            {
                get { return this.GetValue("Page", Page_t.Converter.Instance); }
            }

            public Statistics_t Statistics
            {
                get { return this.GetValue("Statistics", Statistics_t.Converter.Instance); }
            }

            public Visualizations_t Visualizations
            {
                get { return this.GetValue("Visualizations", Visualizations_t.Converter.Instance); }
            }

            #endregion

            #region Types

            public class Events_t : ExpandoAdapter<Events_t>
            {
                #region Properties

                public string Fields // TODO: Deal sensibily with this Pythonic string format: @"["host","source","sourcetype"]"
                {
                    get { return this.GetValue("Fields", StringConverter.Instance); }
                }

                public List_t List
                {
                    get { return this.GetValue("List", List_t.Converter.Instance); }
                }

                public int MaxLines // TODO: Verify this property is a bool
                {
                    get { return this.GetValue("MaxLines", Int32Converter.Instance); }
                }

                public Raw_t Raw
                {
                    get { return this.GetValue("Raw", Raw_t.Converter.Instance); }
                }

                public bool RowNumbers // TODO: Verify this property is a bool
                {
                    get { return this.GetValue("RowNumbers", BooleanConverter.Instance); }
                }

                public Table_t Table
                {
                    get { return this.GetValue("Table", Table_t.Converter.Instance); }
                }

                public string Type // TODO: Encode this property as an enumeration
                {
                    get { return this.GetValue("Type", StringConverter.Instance); }
                }

                #endregion

                #region Types

                public class List_t : ExpandoAdapter<List_t>
                {
                    public string Drilldown // TODO: Encode this property as an enumeration
                    {
                        get { return this.GetValue("Drilldown", StringConverter.Instance); }
                    }

                    public bool Wrap // TODO: Verify this property is a bool
                    {
                        get { return this.GetValue("Wrap", BooleanConverter.Instance); }
                    }
                }

                public class Raw_t : ExpandoAdapter<Raw_t>
                {
                    public string Drilldown // TODO: Encode this property as an enumeration
                    {
                        get { return this.GetValue("Drilldown", StringConverter.Instance); }
                    }
                }

                public class Table_t : ExpandoAdapter<Table_t>
                {
                    public bool Drilldown // TODO: Verify this property is a bool
                    {
                        get { return this.GetValue("Drilldown", BooleanConverter.Instance); }
                    }

                    public bool Wrap // TODO: Verify this property is a bool
                    {
                        get { return this.GetValue("Wrap", BooleanConverter.Instance); }
                    }
                }

                #endregion
            }

            public class General_t : ExpandoAdapter<General_t>
            {
                public bool EnablePreview
                {
                    get { return this.GetValue("EnablePreview", BooleanConverter.Instance); }
                }

                public bool MigratedFromViewState
                {
                    get { return this.GetValue("MigratedFromViewState", BooleanConverter.Instance); }
                }

                public TimeRangePickerAdapter TimeRangePicker
                {
                    get { return this.GetValue("TimeRangePicker", TimeRangePickerAdapter.Converter.Instance); }
                }

                public string Type // TODO: Encode as enumeration
                {
                    get { return this.GetValue("Type", StringConverter.Instance); }
                }

                #region Types

                public class TimeRangePickerAdapter : ExpandoAdapter<TimeRangePickerAdapter>
                {
                    public bool Show
                    {
                        get { return this.GetValue("Show", BooleanConverter.Instance); }
                    }
                }

                #endregion
            }

            public class Page_t : ExpandoAdapter<Page_t>
            {
                #region Properties

                public PivotAdapter Pivot
                {
                    get { return this.GetValue("Pivot", PivotAdapter.Converter.Instance); }
                }

                public SearchAdapter Search
                {
                    get { return this.GetValue("Search", SearchAdapter.Converter.Instance); }
                }

                #endregion

                #region Types

                public class PivotAdapter : ExpandoAdapter<PivotAdapter> // TODO: Implement properties (when do they show?)
                { }

                public class SearchAdapter : ExpandoAdapter<SearchAdapter>
                {
                    #region Properties

                    public string Mode // TODO: Encode as enumeration
                    {
                        get { return this.GetValue("Mode", StringConverter.Instance); }
                    }

                    public bool ShowFields
                    {
                        get { return this.GetValue("ShowFields", BooleanConverter.Instance); }
                    }

                    public TimelineAdapter Search
                    {
                        get { return this.GetValue("Search", TimelineAdapter.Converter.Instance); }
                    }

                    #endregion

                    #region Types

                    public class TimelineAdapter : ExpandoAdapter<TimelineAdapter>
                    {
                        public string Format // TODO: Encode as enumeration
                        {
                            get { return this.GetValue("Format", StringConverter.Instance); }
                        }

                        public string Scale // TODO: Encode as enumeration
                        {
                            get { return this.GetValue("Scale", StringConverter.Instance); }
                        }
                    }

                    #endregion
                }

                #endregion
            }

            public class Statistics_t : ExpandoAdapter<Statistics_t>
            {
                #region Properties

                public string Drilldown // TODO: Encode as enumeration
                {
                    get { return this.GetValue("Drilldown", StringConverter.Instance); }
                }

                public bool Overlay
                {
                    get { return this.GetValue("Overlay", BooleanConverter.Instance); }
                }

                public bool RowNumbers
                {
                    get { return this.GetValue("RowNumbers", BooleanConverter.Instance); }
                }

                public bool Wrap
                {
                    get { return this.GetValue("Wrap", BooleanConverter.Instance); }
                }

                #endregion
            }

            public class Visualizations_t : ExpandoAdapter<Visualizations_t> // TODO: Fill in the remainder
            {
                #region Properties

                public int ChartHeight
                {
                    get { return this.GetValue("ChartHeight", Int32Converter.Instance); }
                }

                public ChartingAdapter Charting
                {
                    get { return this.GetValue("Charting", ChartingAdapter.Converter.Instance); }
                }

                public bool Show
                {
                    get { return this.GetValue("Show", BooleanConverter.Instance); }
                }

                public string Type // TODO: Encode as enumeration
                {
                    get { return this.GetValue("Type", StringConverter.Instance); }
                }

                #endregion

                #region Types

                public class ChartingAdapter : ExpandoAdapter<ChartingAdapter>
                {
                    public string Drilldown // TODO: Encode as enumeration
                    {
                        get { return this.GetValue("Drilldown", StringConverter.Instance); }
                    }
                }

                #endregion
            }

            #endregion
        }

        public class Request_t : ExpandoAdapter<Request_t>
        {
            public string UIDispatchApp
            {
                get { return this.GetValue("UiDispatchApp", StringConverter.Instance); }
            }

            public string UIDispatchView
            {
                get { return this.GetValue("UiDispatchView", StringConverter.Instance); }
            }
        }

        #endregion
    }
}
