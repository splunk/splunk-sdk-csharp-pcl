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
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an object representation of a Splunk saved search.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.Entity{Splunk.Client.Resource}"/>
    /// <seealso cref="T:Splunk.Client.ISavedSearch"/>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1632:DocumentationTextMustMeetMinimumCharacterLength", 
        Justification = "Reviewed.")
    ]
    public class SavedSearch : Entity<Resource>, ISavedSearch
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedSearch"/> class.
        /// </summary>
        /// <param name="service">
        /// An object representing a root Splunk service endpoint.
        /// </param>
        /// <param name="name">
        /// An object identifying a Splunk resource within
        /// <paramref name= "service"/>.<see cref="Namespace"/>.
        /// </param>
        protected internal SavedSearch(Service service, string name)
            : this(service.Context, service.Namespace, name)
        {
            Contract.Requires<ArgumentNullException>(service != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedSearch"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="feed">
        /// A Splunk response atom feed.
        /// </param>
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
        protected internal SavedSearch(Context context, Namespace ns, string name)
            : base(context, ns, SavedSearchCollection.ClassResourceName, name)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref= "SavedSearch"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not intended to
        /// be used directly from your code.
        /// </remarks>
        public SavedSearch()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public virtual ActionsAdapter Actions
        {
            get { return this.Content.GetValue("Action", ActionsAdapter.Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual AlertAdapter Alert
        {
            get { return this.Content.GetValue("Alert", AlertAdapter.Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual AutoSummarizeAdapter AutoSummarize
        {
            get { return this.Content.GetValue("AutoSummarize", AutoSummarizeAdapter.Converter.Instance); }
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
        public virtual DispatchAdapter Dispatch
        {
            get { return this.Content.GetValue("Dispatch", DispatchAdapter.Converter.Instance); }
        }

        /// <inheritdoc/>
        public virtual DisplayAdapter Display
        {
            get { return this.Content.GetValue("Display", DisplayAdapter.Converter.Instance); }
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
        public virtual bool RealTimeSchedule
        {
            get { return this.Content.GetValue("RealtimeSchedule", BooleanConverter.Instance); }
        }

        /// <inheritdoc/>
        public virtual RequestAdapter Request
        {
            get { return this.Content.GetValue("Request", RequestAdapter.Converter.Instance); }
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
        public virtual ReadOnlyCollection<DateTime> ScheduledTimes
        {
            get
            { 
                return this.Content.GetValue(
                    "ScheduledTimes",
                    ReadOnlyCollectionConverter<List<DateTime>, UnixDateTimeConverter, DateTime>.Instance);
            }
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

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, dispatchArgs, templateArgs).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.Created).ConfigureAwait(false);
                searchId = await response.XmlReader.ReadResponseElementAsync("sid").ConfigureAwait(false);
            }

            Job job = new Job(this.Context, this.Namespace, searchId);
            await job.GetAsync().ConfigureAwait(false);

            return job;
        }

        /// <inheritdoc/>
        public virtual async Task GetAsync(Filter criteria)
        {
            using (var response = await this.Context.GetAsync(this.Namespace, this.ResourceName, criteria).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
                await this.ReconstructSnapshotAsync(response).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<JobCollection> GetHistoryAsync()
        {
            var resourceName = new ResourceName(this.ResourceName, "history");

            using (var response = await this.Context.GetAsync(this.Namespace, resourceName).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);

                var feed = new AtomFeed();
                await feed.ReadXmlAsync(response.XmlReader).ConfigureAwait(false);
                var jobs = new JobCollection(this.Context, feed);

                return jobs;
            }
        }

        /// <inheritdoc/>
        public virtual async Task<ReadOnlyCollection<DateTime>> GetScheduledTimesAsync(string earliestTime,
            string latestTime)
        {
            var resourceName = new ResourceName(this.ResourceName, "scheduled_times");

            var args = new Argument[] 
            {
                new Argument("earliest_time", earliestTime),
                new Argument("latest_time", latestTime)
            };

            using (var response = await this.Context.GetAsync(this.Namespace, resourceName, args).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
                await this.ReconstructSnapshotAsync(response).ConfigureAwait(false);
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

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, args).ConfigureAwait(false))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public virtual async Task<bool> UpdateAsync(string search,
            SavedSearchAttributes attributes = null,
            SavedSearchDispatchArgs dispatchArgs = null,
            SavedSearchTemplateArgs templateArgs = null)
        {
            IEnumerable<Argument> arguments = Enumerable.Empty<Argument>();

            if (search != null)
            {
                arguments = arguments.Concat(new Argument[] { new Argument("search", search) });
            }

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

            return await this.UpdateAsync(arguments).ConfigureAwait(false);
        }

        #endregion

        #region Types

        /// <summary>
        /// Provides the arguments required for retrieving information about a
        /// <see cref="SavedSearch"/>
        /// </summary>
        /// <remarks>
        /// <para><b>References:</b></para>
        /// <list type="number">
        /// <item><description>
        ///     <a href="http://goo.gl/bKrRK0">REST API: GET saved/searches</a>
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <seealso cref="T:Splunk.Client.Args{Splunk.Client.SavedSearch.Filter}"/>
        public sealed class Filter : Args<Filter>
        {
            /// <summary>
            /// Gets or sets the lower bound of the time window for which saved search
            /// schedules should be returned.
            /// </summary>
            /// <remarks>
            /// This property specifies that all the scheduled times starting from this
            /// time (not just the next run time) should be returned.
            /// </remarks>
            /// <value>
            /// The earliest time.
            /// </value>
            [DataMember(Name = "earliest_time", EmitDefaultValue = false)]
            [DefaultValue(null)]
            public string EarliestTime
            { get; set; }

            /// <summary>
            /// Gets or sets the upper bound of the time window for which saved search
            /// schedules should be returned.
            /// </summary>
            /// <remarks>
            /// This property specifies that all the scheduled times ending with this
            /// time (not just the next run time) should be returned.
            /// </remarks>
            /// <value>
            /// The latest time.
            /// </value>
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
            /// <value>
            /// <c>true</c> if list default actions, <c>false</c> if not.
            /// </value>
            [DataMember(Name = "listDefaultActionArgs", EmitDefaultValue = false)]
            [DefaultValue(false)]
            public bool ListDefaultActions
            { get; set; }
        }

        #region Static types that map to the dynamic content of a saved search

        /// <summary>
        /// The actions adapter.
        /// </summary>
        /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.ActionsAdapter}"/>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
            "This is by design.")
        ]
        public sealed class ActionsAdapter : ExpandoAdapter<ActionsAdapter>
        {
            #region Properties

            /// <summary>
            /// Gets the e-mail.
            /// </summary>
            /// <value>
            /// The e-mail.
            /// </value>
            public EmailAdapter Email
            {
                get { return this.GetValue("Email", EmailAdapter.Converter.Instance); }
            }

            /// <summary>
            /// Gets the populate lookup.
            /// </summary>
            /// <value>
            /// The populate lookup.
            /// </value>
            public PopulateLookupAdapter PopulateLookup
            {
                get { return this.GetValue("PopulateLookup", PopulateLookupAdapter.Converter.Instance); }
            }

            /// <summary>
            /// Gets the RSS.
            /// </summary>
            /// <value>
            /// The RSS.
            /// </value>
            public RssAdapter Rss
            {
                get { return this.GetValue("Rss", RssAdapter.Converter.Instance); }
            }

            /// <summary>
            /// Gets the script.
            /// </summary>
            /// <value>
            /// The script.
            /// </value>
            public ScriptAdapter Script
            {
                get { return this.GetValue("Script", ScriptAdapter.Converter.Instance); }
            }

            /// <summary>
            /// Gets the zero-based index of the summary.
            /// </summary>
            /// <value>
            /// The summary index.
            /// </value>
            public SummaryIndexAdapter SummaryIndex
            {
                get { return this.GetValue("SummaryIndex", SummaryIndexAdapter.Converter.Instance); }
            }

            #endregion

            #region Types

            /// <summary>
            /// An action.
            /// </summary>
            /// <typeparam name="TAction">
            /// Type of the action.
            /// </typeparam>
            /// <seealso cref="T:Splunk.Client.ExpandoAdapter{TAction}"/>
            [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                "This is by design.")
            ]
            public class Action<TAction> : ExpandoAdapter<TAction> where TAction : Action<TAction>, new()
            {
                /// <summary>
                /// Gets the command.
                /// </summary>
                /// <value>
                /// The command.
                /// </value>
                public string Command
                {
                    get { return this.GetValue("Command", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets a value indicating whether this object is enabled.
                /// </summary>
                /// <value>
                /// <c>true</c> if this object is enabled, <c>false</c> if not.
                /// </value>
                public bool IsEnabled
                {
                    get { return this.GetValue("IsEnabled", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets the maximum results.
                /// </summary>
                /// <value>
                /// The maximum results.
                /// </value>
                public int MaxResults
                {
                    get { return this.GetValue("Maxresults", Int32Converter.Instance); }
                }

                /// <summary>
                /// Gets the maximum time.
                /// </summary>
                /// <value>
                /// The maximum time.
                /// </value>
                public string MaxTime
                {
                    get { return this.GetValue("Maxtime", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets a value indicating whether the track alert.
                /// </summary>
                /// <value>
                /// <c>true</c> if track alert, <c>false</c> if not.
                /// </value>
                public bool TrackAlert
                {
                    get { return this.GetValue("TrackAlert", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets the TTL.
                /// </summary>
                /// <value>
                /// The TTL.
                /// </value>
                public string Ttl
                {
                    get { return this.GetValue("Ttl", StringConverter.Instance); }
                }
            }

            /// <summary>
            /// An e-mail adapter.
            /// </summary>
            /// <seealso cref="T:Splunk.Client.SavedSearch.ActionsAdapter.Action{Splunk.Client.SavedSearch.ActionsAdapter.EmailAdapter}"/>
            [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                "This is by design.")
            ]
            public sealed class EmailAdapter : Action<EmailAdapter>
            {
                /// <summary>
                /// Gets the authentication password.
                /// </summary>
                /// <value>
                /// The authentication password.
                /// </value>
                public string AuthPassword
                {
                    get { return this.GetValue("AuthPassword", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets the authentication username.
                /// </summary>
                /// <value>
                /// The authentication username.
                /// </value>
                public string AuthUsername
                {
                    get { return this.GetValue("AuthUsername", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets the Bcc.
                /// </summary>
                /// <value>
                /// The Bcc.
                /// </value>
                public string Bcc
                {
                    get { return this.GetValue("Bcc", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets the CC e-mail address list to use.
                /// </summary>
                /// <value>
                /// The CC e-mail address list to use.
                /// </value>
                public string CC
                {
                    get { return this.GetValue("Cc", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets the format of text in the e-mail.
                /// </summary>
                /// <value>
                /// The format of text in the e-mail.
                /// </value>
                public EmailFormat Format
                {
                    get { return this.GetValue("Format", EnumConverter<EmailFormat>.Instance); }
                }

                /// <summary>
                /// Gets the e-mail address from which the e-mail action originates.
                /// </summary>
                /// <value>
                /// The e-mail address from which the e-mail action originates.
                /// </value>
                public string From
                {
                    get { return this.GetValue("From", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets a value that indicates whether the search results are 
                /// contained in the body of the e-mail.
                /// </summary>
                /// <value>
                /// <c>true</c> if inline, <c>false</c> if not.
                /// </value>
                public bool Inline
                {
                    get { return this.GetValue("Inline", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets the mail server.
                /// </summary>
                /// <value>
                /// The mail server.
                /// </value>
                public string MailServer
                {
                    get { return this.GetValue("Mailserver", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets a list of report CID fonts.
                /// </summary>
                /// <value>
                /// A List of report cid fonts.
                /// </value>
                public string ReportCidFontList
                {
                    get { return this.GetValue("ReportCIDFontList", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets a value indicating whether the report include splunk logo.
                /// </summary>
                /// <value>
                /// <c>true</c> if report include splunk logo, <c>false</c> if not.
                /// </value>
                public bool ReportIncludeSplunkLogo
                {
                    get { return this.GetValue("ReportIncludeSplunkLogo", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets the report paper orientation.
                /// </summary>
                /// <value>
                /// The report paper orientation.
                /// </value>
                public PaperOrientation ReportPaperOrientation
                {
                    get { return this.GetValue("ReportPaperOrientation", EnumConverter<PaperOrientation>.Instance); }
                }

                /// <summary>
                /// Gets the size of the report paper.
                /// </summary>
                /// <value>
                /// The size of the report paper.
                /// </value>
                public PaperSize ReportPaperSize
                {
                    get { return this.GetValue("ReportPaperSize", EnumConverter<PaperSize>.Instance); }
                }

                /// <summary>
                /// Gets a value indicating whether the report server is enabled.
                /// </summary>
                /// <value>
                /// <c>true</c> if report server enabled, <c>false</c> if not.
                /// </value>
                public bool ReportServerEnabled
                {
                    get { return this.GetValue("ReportServerEnabled", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets a value indicating whether the send PDF.
                /// </summary>
                /// <value>
                /// <c>true</c> if send PDF, <c>false</c> if not.
                /// </value>
                public bool SendPdf
                {
                    get { return this.GetValue("Sendpdf", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets a value indicating whether the send results.
                /// </summary>
                /// <value>
                /// <c>true</c> if send results, <c>false</c> if not.
                /// </value>
                public bool SendResults
                {
                    get { return this.GetValue("Sendresults", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets the subject.
                /// </summary>
                /// <value>
                /// The subject.
                /// </value>
                public string Subject
                {
                    get { return this.GetValue("Subject", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets the subject alert.
                /// </summary>
                /// <value>
                /// The subject alert.
                /// </value>
                public string SubjectAlert
                {
                    get { return this.GetValue("SubjectAlert", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets the subject report.
                /// </summary>
                /// <value>
                /// The subject report.
                /// </value>
                public string SubjectReport
                {
                    get { return this.GetValue("SubjectReport", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets the list of recipient e-mail addresses.
                /// </summary>
                /// <value>
                /// The list of recipient e-mail addresses.
                /// </value>
                public string To
                {
                    get { return this.GetValue("To", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets a value indicating whether this object use ssl.
                /// </summary>
                /// <value>
                /// <c>true</c> if use ssl, <c>false</c> if not.
                /// </value>
                public bool UseSsl
                {
                    get { return this.GetValue("UseSsl", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets a value indicating whether this object use TLS.
                /// </summary>
                /// <value>
                /// <c>true</c> if use tls, <c>false</c> if not.
                /// </value>
                public bool UseTls
                {
                    get { return this.GetValue("UseTls", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets a value indicating whether the width sort columns.
                /// </summary>
                /// <value>
                /// <c>true</c> if width sort columns, <c>false</c> if not.
                /// </value>
                public bool WidthSortColumns
                {
                    get { return this.GetValue("WidthSortColumns", BooleanConverter.Instance); }
                }
            }

            /// <summary>
            /// A populate lookup adapter.
            /// </summary>
            /// <seealso cref="T:Splunk.Client.SavedSearch.ActionsAdapter.Action{Splunk.Client.SavedSearch.ActionsAdapter.PopulateLookupAdapter}"/>
            [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                "This is by design.")
            ]
            public sealed class PopulateLookupAdapter : Action<PopulateLookupAdapter>
            {
                /// <summary>
                /// Gets the Destination for the.
                /// </summary>
                /// <value>
                /// The destination.
                /// </value>
                public string Destination
                {
                    get { return this.GetValue("Dest", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets the hostname.
                /// </summary>
                /// <value>
                /// The hostname.
                /// </value>
                public string Hostname
                {
                    get { return this.GetValue("Hostname", StringConverter.Instance); }
                }
            }

            /// <summary>
            /// The RSS adapter.
            /// </summary>
            /// <seealso cref="T:Splunk.Client.SavedSearch.ActionsAdapter.Action{Splunk.Client.SavedSearch.ActionsAdapter.RssAdapter}"/>
            [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                "This is by design.")
            ]
            public sealed class RssAdapter : Action<RssAdapter>
            { }

            /// <summary>
            /// A script adapter.
            /// </summary>
            /// <seealso cref="T:Splunk.Client.SavedSearch.ActionsAdapter.Action{Splunk.Client.SavedSearch.ActionsAdapter.ScriptAdapter}"/>
            [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                "This is by design.")
            ]
            public sealed class ScriptAdapter : Action<ScriptAdapter>
            {
                /// <summary>
                /// Gets the filename of the file.
                /// </summary>
                /// <value>
                /// The name of the file.
                /// </value>
                public string FileName
                {
                    get { return this.GetValue("Filename", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets the hostname.
                /// </summary>
                /// <value>
                /// The hostname.
                /// </value>
                public string Hostname
                {
                    get { return this.GetValue("Hostname", StringConverter.Instance); }
                }
            }

            /// <summary>
            /// A summary index adapter.
            /// </summary>
            /// <seealso cref="T:Splunk.Client.SavedSearch.ActionsAdapter.Action{Splunk.Client.SavedSearch.ActionsAdapter.SummaryIndexAdapter}"/>
            [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                "This is by design.")
            ]
            public sealed class SummaryIndexAdapter : Action<SummaryIndexAdapter>
            {
                /// <summary>
                /// Gets a value indicating whether the inline.
                /// </summary>
                /// <value>
                /// <c>true</c> if inline, <c>false</c> if not.
                /// </value>
                public bool Inline
                {
                    get { return this.GetValue("Inline", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets the name.
                /// </summary>
                /// <value>
                /// The name.
                /// </value>
                public string Name
                {
                    get { return this.GetValue("Name", StringConverter.Instance); }
                }
            }

            #endregion
        }

        /// <summary>
        /// An alert adapter.
        /// </summary>
        /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.AlertAdapter}"/>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
            "This is by design.")
        ]
        public sealed class AlertAdapter : ExpandoAdapter<AlertAdapter>
        {
            #region Properties

            /// <summary>
            /// Gets the type of the alert.
            /// </summary>
            /// <value>
            /// The type of the alert.
            /// </value>
            public AlertType AlertType
            {
                get { return this.GetValue("Type", EnumConverter<AlertType>.Instance); }
            }

            /// <summary>
            /// Gets the comparator.
            /// </summary>
            /// <value>
            /// The comparator.
            /// </value>
            public AlertComparator Comparator
            {
                get { return this.GetValue("Comparator", EnumConverter<AlertComparator>.Instance); }
            }

            /// <summary>
            /// Gets the condition.
            /// </summary>
            /// <value>
            /// The condition.
            /// </value>
            public string Condition
            {
                get { return this.GetValue("Comparator", StringConverter.Instance); }
            }

            /// <summary>
            /// Gets a value indicating whether the digest mode.
            /// </summary>
            /// <value>
            /// <c>true</c> if digest mode, <c>false</c> if not.
            /// </value>
            public bool DigestMode
            {
                get { return this.GetValue("DigestMode", BooleanConverter.Instance); }
            }

            /// <summary>
            /// Gets the expires.
            /// </summary>
            /// <value>
            /// The expires.
            /// </value>
            public string Expires
            {
                get { return this.GetValue("Expires", StringConverter.Instance); }
            }

            /// <summary>
            /// Gets the severity.
            /// </summary>
            /// <value>
            /// The severity.
            /// </value>
            public AlertSeverity Severity
            {
                get { return this.GetValue("Severity", EnumConverter<AlertSeverity>.Instance); }
            }

            /// <summary>
            /// Gets the suppress.
            /// </summary>
            /// <value>
            /// The suppress.
            /// </value>
            public SuppressAdapter Suppress
            {
                get { return this.GetValue("Suppress", SuppressAdapter.Converter.Instance); }
            }

            /// <summary>
            /// Gets the threshold.
            /// </summary>
            /// <value>
            /// The threshold.
            /// </value>
            public string Threshold
            {
                get { return this.GetValue("Threshold", StringConverter.Instance); }
            }

            /// <summary>
            /// Gets the track.
            /// </summary>
            /// <value>
            /// The track.
            /// </value>
            public AlertTrack Track
            {
                get { return this.GetValue("Track", EnumConverter<AlertTrack>.Instance); }
            }

            #endregion

            #region Types

            /// <summary>
            /// The suppress adapter.
            /// </summary>
            /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.AlertAdapter.SuppressAdapter}"/>
            [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                "This is by design.")
            ]
            public sealed class SuppressAdapter : ExpandoAdapter<SuppressAdapter>
            {
                /// <summary>
                /// Gets a value indicating whether this object is enabled.
                /// </summary>
                /// <value>
                /// <c>true</c> if this object is enabled, <c>false</c> if not.
                /// </value>
                public bool IsEnabled
                {
                    get { return this.GetValue("IsEnabled", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets the fields.
                /// </summary>
                /// <value>
                /// The fields.
                /// </value>
                public string Fields
                {
                    get { return this.GetValue("Fields", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets the period.
                /// </summary>
                /// <value>
                /// The period.
                /// </value>
                public string Period
                {
                    get { return this.GetValue("Period", StringConverter.Instance); }
                }
            }

            #endregion
        }

        /// <summary>
        /// An automatic summarize adapter.
        /// </summary>
        /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.AutoSummarizeAdapter}"/>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
            "This is by design.")
        ]
        public sealed class AutoSummarizeAdapter : ExpandoAdapter<AutoSummarizeAdapter>
        {
            #region Properties

            /// <summary>
            /// Gets the command.
            /// </summary>
            /// <value>
            /// The command.
            /// </value>
            public string Command
            {
                get { return this.GetValue("Command", StringConverter.Instance); }
            }

            /// <summary>
            /// Gets a value indicating whether this object is enabled.
            /// </summary>
            /// <value>
            /// <c>true</c> if this object is enabled, <c>false</c> if not.
            /// </value>
            public bool IsEnabled
            {
                get { return this.GetValue("IsEnabled", BooleanConverter.Instance); }
            }

            /// <summary>
            /// Gets the cron schedule.
            /// </summary>
            /// <value>
            /// The cron schedule.
            /// </value>
            public string CronSchedule
            {
                get { return this.GetValue("CronSchedule", StringConverter.Instance); }
            }

            /// <summary>
            /// Gets the dispatch.
            /// </summary>
            /// <value>
            /// The dispatch.
            /// </value>
            public DispatchAdapter Dispatch
            {
                get { return this.GetValue("Dispatch", DispatchAdapter.Converter.Instance); }
            }

            /// <summary>
            /// Gets the maximum disabled buckets.
            /// </summary>
            /// <value>
            /// The maximum disabled buckets.
            /// </value>
            public int MaxDisabledBuckets
            {
                get { return this.GetValue("MaxDisabledBuckets", Int32Converter.Instance); }
            }

            /// <summary>
            /// Gets the maximum summary ratio.
            /// </summary>
            /// <value>
            /// The maximum summary ratio.
            /// </value>
            public double MaxSummaryRatio
            {
                get { return this.GetValue("MaxSummaryRatio", DoubleConverter.Instance); }
            }

            /// <summary>
            /// Gets the size of the maximum summary.
            /// </summary>
            /// <value>
            /// The size of the maximum summary.
            /// </value>
            public int MaxSummarySize
            {
                get { return this.GetValue("MaxSummarySize", Int32Converter.Instance); }
            }

            /// <summary>
            /// Gets the maximum time.
            /// </summary>
            /// <value>
            /// The maximum time.
            /// </value>
            public int MaxTime
            {
                get { return this.GetValue("MaxTime", Int32Converter.Instance); }
            }

            /// <summary>
            /// Gets the suspend period.
            /// </summary>
            /// <value>
            /// The suspend period.
            /// </value>
            public string SuspendPeriod
            {
                get { return this.GetValue("SuspendPeriod", StringConverter.Instance); }
            }

            #endregion

            #region Types

            /// <summary>
            /// A dispatch adapter.
            /// </summary>
            /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.AutoSummarizeAdapter.DispatchAdapter}"/>
            [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                "This is by design.")
            ]
            public sealed class DispatchAdapter : ExpandoAdapter<DispatchAdapter>
            {
                /// <summary>
                /// Gets the earliest time.
                /// </summary>
                /// <value>
                /// The earliest time.
                /// </value>
                public string EarliestTime
                {
                    get { return this.GetValue("EarliestTime", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets the latest time.
                /// </summary>
                /// <value>
                /// The latest time.
                /// </value>
                public string LatestTime
                {
                    get { return this.GetValue("LatestTime", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets the time format.
                /// </summary>
                /// <value>
                /// The time format.
                /// </value>
                public string TimeFormat
                {
                    get { return this.GetValue("TimeFormat", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets the TTL.
                /// </summary>
                /// <value>
                /// The TTL.
                /// </value>
                public string Ttl
                {
                    get { return this.GetValue("Ttl", StringConverter.Instance); }
                }
            }

            #endregion
        }

        /// <summary>
        /// A dispatch adapter.
        /// </summary>
        /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.DispatchAdapter}"/>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
            "This is by design.")
        ]
        public sealed class DispatchAdapter : ExpandoAdapter<DispatchAdapter>
        {
            /// <summary>
            /// Gets the buckets.
            /// </summary>
            /// <value>
            /// The buckets.
            /// </value>
            public int Buckets
            {
                get { return this.GetValue("Buckets", Int32Converter.Instance); }
            }

            /// <summary>
            /// Gets the earliest time.
            /// </summary>
            /// <value>
            /// The earliest time.
            /// </value>
            public string EarliestTime
            {
                get { return this.GetValue("EarliestTime", StringConverter.Instance); }
            }

            /// <summary>
            /// Gets a value indicating whether the lookups.
            /// </summary>
            /// <value>
            /// <c>true</c> if lookups, <c>false</c> if not.
            /// </value>
            public bool Lookups
            {
                get { return this.GetValue("Lookups", BooleanConverter.Instance); }
            }

            /// <summary>
            /// Gets the number of maximums.
            /// </summary>
            /// <value>
            /// The number of maximums.
            /// </value>
            public int MaxCount
            {
                get { return this.GetValue("MaxCount", Int32Converter.Instance); }
            }

            /// <summary>
            /// Gets the maximum time.
            /// </summary>
            /// <value>
            /// The maximum time.
            /// </value>
            public int MaxTime
            {
                get { return this.GetValue("MaxTime", Int32Converter.Instance); }
            }

            /// <summary>
            /// Gets the reduce frequency.
            /// </summary>
            /// <value>
            /// The reduce frequency.
            /// </value>
            public int ReduceFreq
            {
                get { return this.GetValue("ReduceFreq", Int32Converter.Instance); }
            }

            /// <summary>
            /// Gets a value indicating whether the real time backfill.
            /// </summary>
            /// <value>
            /// <c>true</c> if real time backfill, <c>false</c> if not.
            /// </value>
            public bool RealTimeBackfill
            {
                get { return this.GetValue("RealtimeBackfill", BooleanConverter.Instance); }
            }

            /// <summary>
            /// Gets a value indicating whether the spawn process.
            /// </summary>
            /// <value>
            /// <c>true</c> if spawn process, <c>false</c> if not.
            /// </value>
            public bool SpawnProcess
            {
                get { return this.GetValue("SpawnProcess", BooleanConverter.Instance); }
            }

            /// <summary>
            /// Gets the time format.
            /// </summary>
            /// <value>
            /// The time format.
            /// </value>
            public string TimeFormat
            {
                get { return this.GetValue("TimeFormat", StringConverter.Instance); }
            }

            /// <summary>
            /// Gets the TTL.
            /// </summary>
            /// <value>
            /// The TTL.
            /// </value>
            public string Ttl
            {
                get { return this.GetValue("Ttl", StringConverter.Instance); }
            }
        }

        /// <summary>
        /// A display adapter.
        /// </summary>
        /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.DisplayAdapter}"/>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
            "This is by design.")
        ]
        public sealed class DisplayAdapter : ExpandoAdapter<DisplayAdapter>
        {
            #region Properties

            /// <summary>
            /// Gets the events.
            /// </summary>
            /// <value>
            /// The events.
            /// </value>
            public EventsAdapter Events
            {
                get { return this.GetValue("Events", EventsAdapter.Converter.Instance); }
            }

            /// <summary>
            /// Gets the general.
            /// </summary>
            /// <value>
            /// The general.
            /// </value>
            public GeneralAdapter General
            {
                get { return this.GetValue("General", GeneralAdapter.Converter.Instance); }
            }

            /// <summary>
            /// Gets the page.
            /// </summary>
            /// <value>
            /// The page.
            /// </value>
            public PageAdapter Page
            {
                get { return this.GetValue("Page", PageAdapter.Converter.Instance); }
            }

            /// <summary>
            /// Gets the statistics.
            /// </summary>
            /// <value>
            /// The statistics.
            /// </value>
            public StatisticsAdapter Statistics
            {
                get { return this.GetValue("Statistics", StatisticsAdapter.Converter.Instance); }
            }

            /// <summary>
            /// Gets the visualizations.
            /// </summary>
            /// <value>
            /// The visualizations.
            /// </value>
            public VisualizationsAdapter Visualizations
            {
                get { return this.GetValue("Visualizations", VisualizationsAdapter.Converter.Instance); }
            }

            #endregion

            #region Types

            /// <summary>
            /// The events adapter.
            /// </summary>
            /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.DisplayAdapter.EventsAdapter}"/>
            [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                "This is by design.")
            ]
            public sealed class EventsAdapter : ExpandoAdapter<EventsAdapter>
            {
                #region Properties

                /// <summary>
                /// Gets the fields.
                /// </summary>
                /// <value>
                /// The fields.
                /// </value>
                public string Fields // TODO: Deal sensibily with this Pythonic string format: @"["host","source","sourcetype"]"
                {
                    get { return this.GetValue("Fields", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets the list.
                /// </summary>
                /// <value>
                /// The list.
                /// </value>
                public ListAdapter List
                {
                    get { return this.GetValue("List", ListAdapter.Converter.Instance); }
                }

                /// <summary>
                /// Gets the maximum lines.
                /// </summary>
                /// <value>
                /// The maximum lines.
                /// </value>
                public int MaxLines // TODO: Verify this property is a bool
                {
                    get { return this.GetValue("MaxLines", Int32Converter.Instance); }
                }

                /// <summary>
                /// Gets the raw.
                /// </summary>
                /// <value>
                /// The raw.
                /// </value>
                public RawAdapter Raw
                {
                    get { return this.GetValue("Raw", RawAdapter.Converter.Instance); }
                }

                /// <summary>
                /// Gets a value indicating whether the row numbers.
                /// </summary>
                /// <value>
                /// <c>true</c> if row numbers, <c>false</c> if not.
                /// </value>
                public bool RowNumbers // TODO: Verify this property is a bool
                {
                    get { return this.GetValue("RowNumbers", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets the table.
                /// </summary>
                /// <value>
                /// The table.
                /// </value>
                public TableAdapter Table
                {
                    get { return this.GetValue("Table", TableAdapter.Converter.Instance); }
                }

                /// <summary>
                /// Gets the type of the events.
                /// </summary>
                /// <value>
                /// The type of the events.
                /// </value>
                public string EventsType
                {
                    get { return this.GetValue("Type", StringConverter.Instance); }
                }

                #endregion

                #region Types

                /// <summary>
                /// A list adapter.
                /// </summary>
                /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.DisplayAdapter.EventsAdapter.ListAdapter}"/>
                [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                    "This is by design.")
                ]
                public sealed class ListAdapter : ExpandoAdapter<ListAdapter>
                {
                    /// <summary>
                    /// Gets the drilldown.
                    /// </summary>
                    /// <value>
                    /// The drilldown.
                    /// </value>
                    public string Drilldown // TODO: Encode this property as an enumeration
                    {
                        get { return this.GetValue("Drilldown", StringConverter.Instance); }
                    }

                    /// <summary>
                    /// Gets a value indicating whether the wrap.
                    /// </summary>
                    /// <value>
                    /// <c>true</c> if wrap, <c>false</c> if not.
                    /// </value>
                    public bool Wrap // TODO: Verify this property is a bool
                    {
                        get { return this.GetValue("Wrap", BooleanConverter.Instance); }
                    }
                }

                /// <summary>
                /// A raw adapter.
                /// </summary>
                /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.DisplayAdapter.EventsAdapter.RawAdapter}"/>
                [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                    "This is by design.")
                ]
                public sealed class RawAdapter : ExpandoAdapter<RawAdapter>
                {
                    /// <summary>
                    /// Gets the drilldown.
                    /// </summary>
                    /// <value>
                    /// The drilldown.
                    /// </value>
                    public string Drilldown
                    {
                        get { return this.GetValue("Drilldown", StringConverter.Instance); }
                    }
                }

                /// <summary>
                /// A table adapter.
                /// </summary>
                /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.DisplayAdapter.EventsAdapter.TableAdapter}"/>
                [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                    "This is by design.")
                ]
                public sealed class TableAdapter : ExpandoAdapter<TableAdapter>
                {
                    /// <summary>
                    /// Gets a value indicating whether the drilldown.
                    /// </summary>
                    /// <value>
                    /// <c>true</c> if drilldown, <c>false</c> if not.
                    /// </value>
                    public bool Drilldown // TODO: Verify this property is a bool
                    {
                        get { return this.GetValue("Drilldown", BooleanConverter.Instance); }
                    }

                    /// <summary>
                    /// Gets a value indicating whether the wrap.
                    /// </summary>
                    /// <value>
                    /// <c>true</c> if wrap, <c>false</c> if not.
                    /// </value>
                    public bool Wrap // TODO: Verify this property is a bool
                    {
                        get { return this.GetValue("Wrap", BooleanConverter.Instance); }
                    }
                }

                #endregion
            }

            /// <summary>
            /// A general adapter.
            /// </summary>
            /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.DisplayAdapter.GeneralAdapter}"/>
            [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                "This is by design.")
            ]
            public sealed class GeneralAdapter : ExpandoAdapter<GeneralAdapter>
            {
                #region Properties

                /// <summary>
                /// Gets a value indicating whether the preview is enabled.
                /// </summary>
                /// <value>
                /// <c>true</c> if enable preview, <c>false</c> if not.
                /// </value>
                public bool EnablePreview
                {
                    get { return this.GetValue("EnablePreview", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets a value indicating whether the migrated from view state.
                /// </summary>
                /// <value>
                /// <c>true</c> if migrated from view state, <c>false</c> if not.
                /// </value>
                public bool MigratedFromViewState
                {
                    get { return this.GetValue("MigratedFromViewState", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets the time range picker.
                /// </summary>
                /// <value>
                /// The time range picker.
                /// </value>
                public TimeRangePickerAdapter TimeRangePicker
                {
                    get { return this.GetValue("TimeRangePicker", TimeRangePickerAdapter.Converter.Instance); }
                }

                /// <summary>
                /// Gets the type of the general.
                /// </summary>
                /// <value>
                /// The type of the general.
                /// </value>
                public string GeneralType
                {
                    get { return this.GetValue("Type", StringConverter.Instance); }
                }

                #endregion

                #region Types

                /// <summary>
                /// A time range picker adapter.
                /// </summary>
                /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.DisplayAdapter.GeneralAdapter.TimeRangePickerAdapter}"/>
                [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                    "This is by design.")
                ]
                public sealed class TimeRangePickerAdapter : ExpandoAdapter<TimeRangePickerAdapter>
                {
                    /// <summary>
                    /// Gets a value indicating whether this object is shown.
                    /// </summary>
                    /// <value>
                    /// <c>true</c> if show, <c>false</c> if not.
                    /// </value>
                    public bool Show
                    {
                        get { return this.GetValue("Show", BooleanConverter.Instance); }
                    }
                }

                #endregion
            }

            /// <summary>
            /// A page adapter.
            /// </summary>
            /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.DisplayAdapter.PageAdapter}"/>
            [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                "This is by design.")
            ]
            public sealed class PageAdapter : ExpandoAdapter<PageAdapter>
            {
                #region Properties

                /// <summary>
                /// Gets the pivot.
                /// </summary>
                /// <value>
                /// The pivot.
                /// </value>
                public PivotAdapter Pivot
                {
                    get { return this.GetValue("Pivot", PivotAdapter.Converter.Instance); }
                }

                /// <summary>
                /// Gets the search.
                /// </summary>
                /// <value>
                /// The search.
                /// </value>
                public SearchAdapter Search
                {
                    get { return this.GetValue("Search", SearchAdapter.Converter.Instance); }
                }

                #endregion

                #region Types

                /// <summary>
                /// A pivot adapter.
                /// </summary>
                /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.DisplayAdapter.PageAdapter.PivotAdapter}"/>
                [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                    "This is by design.")
                ]
                public sealed class PivotAdapter : ExpandoAdapter<PivotAdapter>
                { }

                /// <summary>
                /// A search adapter.
                /// </summary>
                /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.DisplayAdapter.PageAdapter.SearchAdapter}"/>
                [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                    "This is by design.")
                ]
                public sealed class SearchAdapter : ExpandoAdapter<SearchAdapter>
                {
                    #region Properties

                    /// <summary>
                    /// Gets the mode.
                    /// </summary>
                    /// <value>
                    /// The mode.
                    /// </value>
                    public string Mode // TODO: Encode as enumeration
                    {
                        get { return this.GetValue("Mode", StringConverter.Instance); }
                    }

                    /// <summary>
                    /// Gets a value indicating whether the fields is shown.
                    /// </summary>
                    /// <value>
                    /// <c>true</c> if show fields, <c>false</c> if not.
                    /// </value>
                    public bool ShowFields
                    {
                        get { return this.GetValue("ShowFields", BooleanConverter.Instance); }
                    }

                    /// <summary>
                    /// Gets the search.
                    /// </summary>
                    /// <value>
                    /// The search.
                    /// </value>
                    public TimelineAdapter Search
                    {
                        get { return this.GetValue("Search", TimelineAdapter.Converter.Instance); }
                    }

                    #endregion

                    #region Types

                    /// <summary>
                    /// A timeline adapter.
                    /// </summary>
                    /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.DisplayAdapter.PageAdapter.SearchAdapter.TimelineAdapter}"/>
                    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                        "This is by design.")
                    ]
                    public sealed class TimelineAdapter : ExpandoAdapter<TimelineAdapter>
                    {
                        /// <summary>
                        /// Gets the format to use.
                        /// </summary>
                        /// <value>
                        /// The format.
                        /// </value>
                        public string Format // TODO: Encode as enumeration
                        {
                            get { return this.GetValue("Format", StringConverter.Instance); }
                        }

                        /// <summary>
                        /// Gets the scale.
                        /// </summary>
                        /// <value>
                        /// The scale.
                        /// </value>
                        public string Scale // TODO: Encode as enumeration
                        {
                            get { return this.GetValue("Scale", StringConverter.Instance); }
                        }
                    }

                    #endregion
                }

                #endregion
            }

            /// <summary>
            /// The statistics adapter.
            /// </summary>
            /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.DisplayAdapter.StatisticsAdapter}"/>
            [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                "This is by design.")
            ]
            public sealed class StatisticsAdapter : ExpandoAdapter<StatisticsAdapter>
            {
                /// <summary>
                /// Gets the drilldown.
                /// </summary>
                /// <value>
                /// The drilldown.
                /// </value>
                public string Drilldown // TODO: Encode as enumeration
                {
                    get { return this.GetValue("Drilldown", StringConverter.Instance); }
                }

                /// <summary>
                /// Gets a value indicating whether the overlay.
                /// </summary>
                /// <value>
                /// <c>true</c> if overlay, <c>false</c> if not.
                /// </value>
                public bool Overlay
                {
                    get { return this.GetValue("Overlay", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets a value indicating whether the row numbers.
                /// </summary>
                /// <value>
                /// <c>true</c> if row numbers, <c>false</c> if not.
                /// </value>
                public bool RowNumbers
                {
                    get { return this.GetValue("RowNumbers", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets a value indicating whether the wrap.
                /// </summary>
                /// <value>
                /// <c>true</c> if wrap, <c>false</c> if not.
                /// </value>
                public bool Wrap
                {
                    get { return this.GetValue("Wrap", BooleanConverter.Instance); }
                }
            }

            /// <summary>
            /// The visualizations adapter.
            /// </summary>
            /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.DisplayAdapter.VisualizationsAdapter}"/>
            [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                "This is by design.")
            ]
            public sealed class VisualizationsAdapter : ExpandoAdapter<VisualizationsAdapter> // TODO: Fill in the remainder
            {
                #region Properties

                /// <summary>
                /// Gets the height of the chart.
                /// </summary>
                /// <value>
                /// The height of the chart.
                /// </value>
                public int ChartHeight
                {
                    get { return this.GetValue("ChartHeight", Int32Converter.Instance); }
                }

                /// <summary>
                /// Gets the charting.
                /// </summary>
                /// <value>
                /// The charting.
                /// </value>
                public ChartingAdapter Charting
                {
                    get { return this.GetValue("Charting", ChartingAdapter.Converter.Instance); }
                }

                /// <summary>
                /// Gets a value indicating whether this object is shown.
                /// </summary>
                /// <value>
                /// <c>true</c> if show, <c>false</c> if not.
                /// </value>
                public bool Show
                {
                    get { return this.GetValue("Show", BooleanConverter.Instance); }
                }

                /// <summary>
                /// Gets the type of the visualizations.
                /// </summary>
                /// <value>
                /// The type of the visualizations.
                /// </value>
                public string VisualizationsType
                {
                    get { return this.GetValue("Type", StringConverter.Instance); }
                }

                #endregion

                #region Types

                /// <summary>
                /// A charting adapter.
                /// </summary>
                /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.DisplayAdapter.VisualizationsAdapter.ChartingAdapter}"/>
                [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
                    "This is by design.")
                ]
                public sealed class ChartingAdapter : ExpandoAdapter<ChartingAdapter>
                {
                    /// <summary>
                    /// Gets the drilldown.
                    /// </summary>
                    /// <value>
                    /// The drilldown.
                    /// </value>
                    public string Drilldown
                    {
                        get { return this.GetValue("Drilldown", StringConverter.Instance); }
                    }
                }

                #endregion
            }

            #endregion
        }

        /// <summary>
        /// A request adapter.
        /// </summary>
        /// <seealso cref="T:Splunk.Client.ExpandoAdapter{Splunk.Client.SavedSearch.RequestAdapter}"/>
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification =
            "This is by design.")
        ]
        public sealed class RequestAdapter : ExpandoAdapter<RequestAdapter>
        {
            /// <summary>
            /// Gets the dispatch application.
            /// </summary>
            /// <value>
            /// The user interface dispatch application.
            /// </value>
            public string UIDispatchApp
            {
                get { return this.GetValue("UiDispatchApp", StringConverter.Instance); }
            }

            /// <summary>
            /// Gets the dispatch view.
            /// </summary>
            /// <value>
            /// The user interface dispatch view.
            /// </value>
            public string UIDispatchView
            {
                get { return this.GetValue("UiDispatchView", StringConverter.Instance); }
            }
        }

        #endregion

        #endregion
    }
}
