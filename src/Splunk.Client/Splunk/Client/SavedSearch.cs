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

//// TODO:
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a Splunk saved search.
    /// </summary>
    public class SavedSearch : Entity, ISavedSearch
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedSearch"/> class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// <param name="name">
        /// An object identifying a Splunk resource within <paramref name=
        /// "service"/>.<see cref="Namespace"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> or <paramref name="name"/> are <c>null</c>.
        /// </exception>
        protected internal SavedSearch(Service service, string name)
            : this(service.Context, service.Namespace, name)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedSearch"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="feed"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// <paramref name="feed"/> is in an invalid format.
        /// </exception>
        protected internal SavedSearch(Context context, AtomFeed feed)
        {
            this.Initialize(context, feed);
        }

        /// <summary>
        /// Initalizes a new instance of the <see cref="SavedSearch"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="name">
        /// Name of the <see cref="SavedSearch"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> or <paramref name="ns"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="ns"/> is not specific.
        /// </exception>
        protected internal SavedSearch(Context context, Namespace ns, string name)
            : base(context, ns, SavedSearchCollection.ClassResourceName, name)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "SavedSearch"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use one of these
        /// methods to obtain a <see cref="SavedSearch"/> instance:
        /// <list type="table">
        /// <listheader>
        ///   <term>Method</term>
        ///   <description>Description</description>
        /// </listheader>
        /// <item>
        ///   <term><see cref="Service.CreateSavedSearchAsync"/></term>
        ///   <description>
        ///   Asynchronously creates a new <see cref="SavedSearch"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.GetSavedSearchAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves an existing <see cref="SavedSearch"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.UpdateSavedSearchAsync"/></term>
        ///   <description>
        ///   Asynchronously updates an existing <see cref="SavedSearch"/>.
        ///   </description>
        /// </item>
        /// </list>
        /// </remarks>
        public SavedSearch()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public virtual Action_t Actions
        {
            get { return this.Content.GetValue("Action", Action_t.Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual Alert_t Alert
        {
            get { return this.Content.GetValue("Alert", Alert_t.Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual AutoSummarize_t AutoSummarize
        {
            get { return this.Content.GetValue("AutoSummarize", AutoSummarize_t.Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string CronSchedule
        {
            get { return this.Content.GetValue("CronSchedule", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string Description
        {
            get { return this.Content.GetValue("Description", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual Dispatch_t Dispatch
        {
            get { return this.Content.GetValue("Dispatch", Dispatch_t.Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual Display_t Display
        {
            get { return this.Content.GetValue("Display", Display_t.Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual Eai Eai
        {
            get { return this.Content.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual bool IsDisabled
        {
            get { return this.Content.GetValue("IsDisabled", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual bool IsScheduled
        {
            get { return this.Content.GetValue("IsScheduled", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual bool IsVisible
        {
            get { return this.Content.GetValue("IsVisible", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual int MaxConcurrent
        {
            get { return this.Content.GetValue("MaxConcurrent", Int32Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual DateTime NextScheduledTime
        {
            get { return this.Content.GetValue("NextScheduledTime", DateTimeConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual bool RealtimeSchedule
        {
            get { return this.Content.GetValue("RealtimeSchedule", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual Request_t Request
        {
            get { return this.Content.GetValue("Request", Request_t.Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual bool RestartOnSearchPeerAdd
        {
            get { return this.Content.GetValue("RestartOnSearchpeerAdd", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual string QualifiedSearch
        {
            get { return this.Content.GetValue("QualifiedSearch", StringConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual bool RunOnStartup
        {
            get { return this.Content.GetValue("RunOnStartup", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual IReadOnlyList<DateTime> ScheduledTimes
        {
            get { return this.Content.GetValue("ScheduledTimes", CollectionConverter<DateTime, List<DateTime>, UnixDateTimeConverter>.Instance); }
        }

        /// <inheritdoc/>
        public virtual string Search
        {
            get { return this.Content.GetValue("Search", StringConverter.Instance); }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async Task<Job> DispatchAsync(SavedSearchDispatchArgs dispatchArgs = null, 
            SavedSearchTemplateArgs templateArgs = null)
        {
            var resourceName = new ResourceName(this.ResourceName, "dispatch");
            string searchId;

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, dispatchArgs, templateArgs))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created);
                searchId = await response.XmlReader.ReadResponseElementAsync("sid");
            }

            Job job = new Job(this.Context, this.Namespace, searchId);
            await job.GetAsync();

            return job;
        }

        /// <inheritdoc/>
        public virtual async Task GetAsync(Filter criteria)
        {
            using (var response = await this.Context.GetAsync(this.Namespace, this.ResourceName, criteria))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.ReconstructSnapshotAsync(response);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<JobCollection> GetHistoryAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "history");

            using (var response = await this.Context.GetAsync(this.Namespace, resourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                
                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader);
                var jobs = new JobCollection(this.Context, feed);

                return jobs;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<IReadOnlyList<DateTime>> GetScheduledTimesAsync(DateTime earliestTime, 
            DateTime latestTime)
        {
            var resourceName = new ResourceName(this.ResourceName, "scheduled_times");
            var dateTime = DateTime.Now;
            
            var min = (long)(earliestTime - dateTime).TotalSeconds;
            var max = (long)(latestTime - dateTime).TotalSeconds;

            var args = new Argument[] 
            {
                new Argument("earliest_time", min.ToString("+#;-#;0")),
                new Argument("latest_time", max.ToString("+#;-#;0"))
            };

            using (var response = await this.Context.GetAsync(this.Namespace, resourceName, args))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                await this.ReconstructSnapshotAsync(response);
            }

            return this.ScheduledTimes;
        }

        /// <inheritdoc/>
        public virtual async Task ScheduleAsync(DateTime? scheduleTime = null)
        {
            var resourceName = new ResourceName(this.ResourceName, "reschedule");
            
            Argument[] args = scheduleTime == null ? null : new Argument[] 
            { 
                new Argument("schedule_time", scheduleTime.Value.ToString("u")) //string.Format("{0:s}Z", scheduleTime.Value.ToUniversalTime()))
            };

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, args) )
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <inheritdoc/>
        public virtual async Task UpdateAsync(SavedSearchAttributes attributes = null, 
            SavedSearchDispatchArgs dispatchArgs = null, 
            SavedSearchTemplateArgs templateArgs = null)
        {
            IEnumerable<Argument> arguments = Enumerable.Empty<Argument>();

            if (attributes != null)
            {
                arguments = arguments.Concat(attributes);
            }

            if (dispatchArgs != null)
            {
                arguments = arguments.Concat(dispatchArgs);
            }

            if (templateArgs != null)
            {
                arguments = arguments.Concat(templateArgs);
            }

            await this.UpdateAsync(arguments);
        }

        #endregion

        #region Types

        /// <summary>
        /// Provides the arguments required for retrieving information about
        /// a <see cref="SavedSearch"/>
        /// </summary>
        /// <remarks>
        /// <para><b>References:</b></para>
        /// <list type="number">
        /// <item><description>
        ///     <a href="http://goo.gl/bKrRK0">REST API: GET saved/searches</a>
        /// </description></item>
        /// </list>
        /// </remarks>
        public sealed class Filter : Args<Filter>
        {
            /// <summary>
            /// Gets or sets the lower bound of the time window for which saved 
            /// search schedules should be returned.
            /// </summary>
            /// <remarks>
            /// This property specifies that all the scheduled times starting from 
            /// this time (not just the next run time) should be returned.
            /// </remarks>
            [DataMember(Name = "earliest_time", EmitDefaultValue = false)]
            [DefaultValue(null)]
            public string EarliestTime
            { get; set; }

            /// <summary>
            /// Gets or sets the upper bound of the time window for which saved 
            /// search schedules should be returned.
            /// </summary>
            /// <remarks>
            /// This property specifies that all the scheduled times ending with 
            /// this time (not just the next run time) should be returned.
            /// </remarks>
            [DataMember(Name = "latest_time", EmitDefaultValue = false)]
            [DefaultValue(null)]
            public string LatestTime
            { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to list default actions for
            /// <see cref="SavedSearch"/> entries.
            /// </summary>
            /// <remarks>
            /// The default value is <c>false</c>.
            /// </remarks>
            [DataMember(Name = "listDefaultActionArgs", EmitDefaultValue = false)]
            [DefaultValue(false)]
            public bool ListDefaultActions
            { get; set; }
        }

        #region Static types that map to the dynamic content of a saved search

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

            public Script_t Script
            {
                get { return this.GetValue("Script", Script_t.Converter.Instance); }
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

                public int MaxResults
                {
                    get { return this.GetValue("Maxresults", Int32Converter.Instance); }
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

                public string AuthPassword
                {
                    get { return this.GetValue("AuthPassword", StringConverter.Instance); }
                }

                public string AuthUsername
                {
                    get { return this.GetValue("AuthUsername", StringConverter.Instance); }
                }

                public string Bcc
                {
                    get { return this.GetValue("Bcc", StringConverter.Instance); }
                }

                public string CC
                {
                    get { return this.GetValue("Cc", StringConverter.Instance); }
                }

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

                public string SubjectAlert
                {
                    get { return this.GetValue("SubjectAlert", StringConverter.Instance); }
                }

                public string SubjectReport
                {
                    get { return this.GetValue("SubjectReport", StringConverter.Instance); }
                }

                public string To
                {
                    get { return this.GetValue("To", StringConverter.Instance); }
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
            {
                #region Properties

                public string Destination
                {
                    get { return this.GetValue("Dest", StringConverter.Instance); }
                }

                public string Hostname
                {
                    get { return this.GetValue("Hostname", StringConverter.Instance); }
                }

                #endregion
            }

            public class Rss_t : Action<Rss_t>
            { }

            public class Script_t : Action<Script_t>
            {
                #region Properties

                public string FileName
                {
                    get { return this.GetValue("Filename", StringConverter.Instance); }
                }

                public string Hostname
                {
                    get { return this.GetValue("Hostname", StringConverter.Instance); }
                }

                #endregion
            }

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

            public AlertComparator Comparator
            {
                get { return this.GetValue("Comparator", EnumConverter<AlertComparator>.Instance); }
            }

            public string Condition
            {
                get { return this.GetValue("Comparator", StringConverter.Instance); }
            }

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

            public Suppress_t Suppress
            {
                get { return this.GetValue("Suppress", Suppress_t.Converter.Instance); }
            }

            public string Threshold
            {
                get { return this.GetValue("Threshold", StringConverter.Instance); }
            }

            public AlertTrack Track
            {
                get { return this.GetValue("Track", EnumConverter<AlertTrack>.Instance); }
            }

            public AlertType Type
            {
                get { return this.GetValue("Type", EnumConverter<AlertType>.Instance); }
            }

            #endregion

            #region Types

            public class Suppress_t : ExpandoAdapter<Suppress_t>
            {
                #region Properties

                public bool IsEnabled
                {
                    get { return this.GetValue("IsEnabled", BooleanConverter.Instance); }
                }

                public string Fields
                {
                    get { return this.GetValue("Fields", StringConverter.Instance); }
                }

                public string Period
                {
                    get { return this.GetValue("Period", StringConverter.Instance); }
                }

                #endregion
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

        #endregion
    }
}
