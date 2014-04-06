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
// [ ] Port DispatchTimeFormat and use it wherever appropriate. Can we create
//     a relative as well as an absolute time formatter?
// [O] Documentation

namespace Splunk.Sdk
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments for creating a <see cref="SavedSearch"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item>
    ///     <description>
    ///     <a href="http://goo.gl/khXMdf">REST API: POST saved/searches.</a>
    ///     </description>
    /// </item>
    /// </list>
    /// </remarks>
    public sealed class SavedSearchAttributes : Args<SavedSearchAttributes>
    {
        #region Constructors

        public SavedSearchAttributes()
        { }

        public SavedSearchAttributes(string search)
        {
            this.Search = search;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the search command for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This value is required.
        /// </remarks>
        [DataMember(Name = "search", IsRequired = true)]
        public string Search
        { get; set; }

        /// <summary>
        /// Gets or sets a comma-separated list of actions to enable for a
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// No actions are enabled by default.
        /// </remarks>
        [DataMember(Name = "actions", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string Actions
        { get; set; }

        /// <summary>
        /// Gets or sets the search command which is responsible for executing 
        /// the e-mail action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Generally the command is a template search pipeline which is 
        /// realized with values from the <see cref="SavedSearch"/>. Reference
        /// template arguments by enclosing their names in dollar signs ($). 
        /// For example, to reference <see cref="Name"/> use <c>$name$</c>. To
        /// reference <see cref="Search"/> use <c>$search$</c>.
        /// </remarks>
        [DataMember(Name = "action.email.command", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string ActionEmailCommand
        { get; set; }

        /// <summary>
        /// Gets or sets the format of the e-mail action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This value also applies to any attachments.
        /// </remarks>
        [DataMember(Name = "action.email.format", EmitDefaultValue = false)]
        [DefaultValue(EmailFormat.None)]
        public EmailFormat ActionEmailFormat
        { get; set; }

        /// <summary>
        /// Gets or sets the e-mail address from which the e-mail action for a 
        /// <see cref="SavedSearch"/> originates.
        /// </summary>
        /// <remarks>
        /// This vaulue defaults to <c>splunk@$LOCALHOST</c> or whatever value 
        /// is set in <a href="http://goo.gl/odNige">alert_actions.conf</a>.
        /// </remarks>
        [DataMember(Name = "action.email.from", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string ActionEmailFrom
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether search results should be 
        /// contained in the body of the action e-mail for a <see cref=
        /// "SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Search results can be either inline or attached. See <see cref=
        /// "ActionEmailSendResults"/>.
        /// </remarks>
        [DataMember(Name = "action.email.inline", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string ActionEmailInline
        { get; set; }

        /// <summary>
        /// Gets or sets the address of the MTA server to be used to send the 
        /// action e-mail for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This value defaults to localhost or whatever value is set in <a 
        /// href="http://goo.gl/odNige">alert_actions.conf</a>.
        /// </remarks>
        [DataMember(Name = "action.email.mailserver", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string ActionEmailMailServer
        { get; set; }

        /// <summary>
        /// Gets or sets the global maximum number of search results to send 
        /// when the e-mail for a <see cref="SavedSearch"/> is enabled.
        /// </summary>
        /// <remarks>
        /// The default value is <c>100</c>.
        /// </remarks>
        [DataMember(Name = "action.email.maxresults", EmitDefaultValue = false)]
        [DefaultValue(100)]
        public int ActionEmailMaxResults
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum period of time the e-mail action for a
        /// <see cref="SearchCommand"/> may execute before it is aborted.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c><![CDATA[
        /// <integer>("s"|"m"|"h"|"d")]]></c>. The default is <c>"5m"</c>.
        /// </remarks>
        [DataMember(Name = "action.email.maxtime", EmitDefaultValue = false)]
        [DefaultValue("5m")]
        public string ActionEmailMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets the set and load order of CID fonts for handling 
        /// Simplified Chinese (gb), Traditional Chinese (cns), Japanese (jp), 
        /// and Korean (kor) in Integrated PDF Rendering.
        /// </summary>
        /// <remarks>
        /// The default value is <c>"gb cns jp kor"</c>.
        /// </remarks>
        [DataMember(Name = "action.email.reportCIDFontList", EmitDefaultValue = false)]
        [DefaultValue("gb cns jp kor")]
        public string ActionEmailReportCidFontList
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include the Splunk logo 
        /// with the report on an e-mail action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "action.email.reportIncludeSplunkLogo", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ActionEmailReportIncludeSplunkLogo
        { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PaperOrientation"/> for the report on 
        /// an e-mail action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default is <see cref="PaperOrientation.Portrait"/>.
        /// </remarks>
        [DataMember(Name = "action.email.reportPaperOrientation", EmitDefaultValue = false)]
        [DefaultValue(PaperOrientation.Portrait)]
        public PaperOrientation ActionEmailReportPaperOrientation
        { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PaperSize"/> for the report on an 
        /// e-mail action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default is <see cref="PaperSize.Letter"/>.
        /// </remarks>
        [DataMember(Name = "action.email.reportPaperSize", EmitDefaultValue = false)]
        [DefaultValue(PaperSize.Letter)]
        public PaperSize ActionEmailReportPaperSize
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the PDF server is 
        /// enabled on the e-mail action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "action.email.reportServerEnabled", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ActionEmailReportServerEnabled
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to create and send the 
        /// results of the e-mail action for a <see cref="SavedSearch"/> as a 
        /// PDF report.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>
        /// </remarks>
        [DataMember(Name = "action.email.sendpdf", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ActionEmailSendPdf
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether search results should
        /// be included in the action e-mail for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Results can be either attached or inline. See <see cref=
        /// "ActionEmailInline"/>. The default value is <c>false</c>
        /// </remarks>
        [DataMember(Name = "action.email.sendresults", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ActionEmailSendResults
        { get; set; }

        /// <summary>
        /// Gets or sets an alternate subject for the action e-mail on a 
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>"SplunkAlert-"<saved-search-name></c>.
        /// </remarks>
        [DataMember(Name = "action.email.subject", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string ActionEmailSubject
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the e-mail action of a
        /// <see cref="SavedSearch"/> signifies a trackable alert.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "action.email.track_alert", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ActionEmailTrackAlert
        { get; set; }

        /// <summary>
        /// Gets or sets the minimum time-to-live for artifacts of the e-mail
        /// action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c><![CDATA[
        /// <integer>["p"] ]]></c>. If <c>p</c> follows <![CDATA[<integer>]]>, 
        /// the unit of time is the number of scheduled periods. Otherwise, the
        /// unit of time is seconds. The default is 86,400 seconds; equivalent 
        /// to 24 hours.
        /// </remarks>
        [DataMember(Name = "action.email.ttl", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string ActionEmailTtl
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use SSL when communicating 
        /// with the SMTP server.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "action.email.use_ssl", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ActionEmailUseSsl
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use TLS when communicating
        /// with the SMTP server.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "action.email.use_tls", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ActionEmailUseTls
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether columns should be sorted 
        /// from least wide to most wide, left to right.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "action.email.width_sort_columns", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ActionEmailWidthSortColumns
        { get; set; }

        /// <summary>
        /// Gets or sets the search command which is responsible for executing 
        /// the populate lookup action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Generally the command is a template search pipeline which is 
        /// realized with values from the <see cref="SavedSearch"/>. Reference
        /// template arguments by enclosing their names in dollar signs ($). 
        /// For example, to reference <see cref="Name"/> use <c>$name$</c>. To
        /// reference <see cref="Search"/> use <c>$search$</c>.
        /// </remarks>
        [DataMember(Name = "action.populate_lookup.command", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string ActionPopulateLookupCommand
        { get; set; }

        /// <summary>
        /// Gets or sets the global maximum number of search results to send 
        /// when the populate lookup action for a <see cref="SavedSearch"/> is 
        /// enabled.
        /// </summary>
        /// <remarks>
        /// The default value is <c>100</c>.
        /// </remarks>
        [DataMember(Name = "action.populate_lookup.maxresults", EmitDefaultValue = false)]
        [DefaultValue(100)]
        public int ActionPopulateLookupMaxResults
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum period of time the populate lookup action
        /// for a <see cref="SearchCommand"/> may execute before it is aborted.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c><![CDATA[
        /// <integer>("s"|"m"|"h"|"d")]]></c>. The default is <c>"5m"</c>.
        /// </remarks>
        [DataMember(Name = "action.populate_lookup.maxtime", EmitDefaultValue = false)]
        [DefaultValue("5m")]
        public string ActionPopulateLookupMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the populate lookup
        /// action for a <see cref="SavedSearch"/> signifies a trackable alert.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "action.populate_lookup.track_alert", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ActionPopulateLookupTrackAlert
        { get; set; }

        /// <summary>
        /// Gets or sets the minimum time-to-live for artifacts of the populate
        /// lookup action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c><![CDATA[
        /// <integer>["p"] ]]></c>. If <c>p</c> follows <![CDATA[<integer>]]>, 
        /// the unit of time is the number of scheduled periods. Otherwise, the
        /// unit of time is seconds. The default is 86,400 seconds; equivalent
        /// to 24 hours.
        /// </remarks>
        [DataMember(Name = "action.populate_lookup.ttl", EmitDefaultValue = false)]
        [DefaultValue("86400")]
        public string ActionPopulateLookupTtl
        { get; set; }

        /// <summary>
        /// Gets or sets the search command which is responsible for executing 
        /// the RSS action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Generally the command is a template search pipeline which is 
        /// realized with values from the <see cref="SavedSearch"/>. Reference
        /// template arguments by enclosing their names in dollar signs ($). 
        /// For example, to reference <see cref="Name"/> use <c>$name$</c>. To
        /// reference <see cref="Search"/> use <c>$search$</c>.
        /// </remarks>
        [DataMember(Name = "action.rss.command", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string ActionRssCommand
        { get; set; }

        /// <summary>
        /// Gets or sets the global maximum number of search results to send 
        /// when the RSS action for a <see cref="SavedSearch"/> is enabled.
        /// </summary>
        /// <remarks>
        /// The default value is <c>100</c>.
        /// </remarks>
        [DataMember(Name = "action.rss.maxresults", EmitDefaultValue = false)]
        [DefaultValue(100)]
        public int ActionRssMaxResults
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum period of time the RSS action for a 
        /// <see cref="SearchCommand"/> may execute before it is aborted.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c><![CDATA[
        /// <integer>("s"|"m"|"h"|"d")]]></c>. The default is <c>"1m"</c>.
        /// </remarks>
        [DataMember(Name = "action.rss.maxtime", EmitDefaultValue = false)]
        [DefaultValue("1m")]
        public string ActionRssMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the RSS action of a
        /// <see cref="SavedSearch"/> signifies a trackable alert.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "action.rss.track_alert", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string ActionRssTrackAlert
        { get; set; }

        /// <summary>
        /// Gets or sets the minimum time-to-live for artifacts of the RSS
        /// action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c><![CDATA[
        /// <integer>["p"] ]]></c>. If <c>p</c> follows <![CDATA[<integer>]]>, 
        /// the unit of time is the number of scheduled periods. Otherwise, the
        /// unit of time is seconds. The default is 86,400 seconds; equivalent 
        /// to 24 hours.
        /// </remarks>
        [DataMember(Name = "action.rss.ttl", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string ActionRssTtl
        { get; set; }

        /// <summary>
        /// Gets or sets the search command which is responsible for executing 
        /// the RSS action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Generally the command is a template search pipeline which is 
        /// realized with values from the <see cref="SavedSearch"/>. Reference
        /// template arguments by enclosing their names in dollar signs ($). 
        /// For example, to reference <see cref="Name"/> use <c>$name$</c>. To
        /// reference <see cref="Search"/> use <c>$search$</c>.
        /// </remarks>
        [DataMember(Name = "action.script.command", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string ActionScriptCommand
        { get; set; }

        /// <summary>
        /// Gets or sets the global maximum number of search results to send 
        /// when the RSS action for a <see cref="SavedSearch"/> is enabled.
        /// </summary>
        /// <remarks>
        /// The default value is <c>100</c>.
        /// </remarks>
        [DataMember(Name = "action.script.maxresults", EmitDefaultValue = false)]
        [DefaultValue(100)]
        public int ActionScriptMaxResults
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum period of time the RSS action for a 
        /// <see cref="SearchCommand"/> may execute before it is aborted.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c><![CDATA[
        /// <integer>("s"|"m"|"h"|"d")]]></c>. The default is <c>"5m"</c>.
        /// </remarks>
        [DataMember(Name = "action.script.maxtime", EmitDefaultValue = false)]
        [DefaultValue("5m")]
        public string ActionScriptMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the script action of a
        /// <see cref="SavedSearch"/> signifies a trackable alert.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "action.script.track_alert", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ActionScriptTrackAlert
        { get; set; }

        /// <summary>
        /// Gets or sets the minimum time-to-live for artifacts of the script
        /// action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c><![CDATA[
        /// <integer>["p"] ]]></c>. If <c>p</c> follows <![CDATA[<integer>]]>, 
        /// the unit of time is the number of scheduled periods. Otherwise, the
        /// unit of time is seconds. The default is 600 seconds; equivalent 
        /// to 10 minutes.
        /// </remarks>
        [DataMember(Name = "action.script.ttl", EmitDefaultValue = false)]
        [DefaultValue("600")]
        public string ActionScriptTtl
        { get; set; }

        /// <summary>
        /// Gets or sets the search command which is responsible for executing 
        /// the summary index action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Generally the command is a template search pipeline which is 
        /// realized with values from the <see cref="SavedSearch"/>. Reference
        /// template arguments by enclosing their names in dollar signs ($). 
        /// For example, to reference <see cref="Name"/> use <c>$name$</c>. To
        /// reference <see cref="Search"/> use <c>$search$</c>.
        /// </remarks>
        [DataMember(Name = "action.summary_index.command", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string ActionSummaryIndexCommand
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to execute the summary 
        /// index action of a <see cref="SavedSearch"/> as part of the 
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This option is considered only if the summary index action is 
        /// enabled and is always executed. The default value is <c>true</c>.
        /// </remarks>
        [DataMember(Name = "action.summary_index.inline", EmitDefaultValue = false)]
        [DefaultValue(true)]
        public bool ActionSummaryIndexInline
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of search results sent via alerts
        /// for the index action of a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>100</c>.
        /// </remarks>
        [DataMember(Name = "action.summary_index.maxresults", EmitDefaultValue = false)]
        [DefaultValue(100)]
        public int ActionSummaryIndexMaxResults
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum period of time the summary index action 
        /// for a <see cref="SearchCommand"/> may execute before it is aborted.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c><![CDATA[
        /// <integer>("s"|"m"|"h"|"d")]]></c>. The default is <c>"5m"</c>.
        /// </remarks>
        [DataMember(Name = "action.summary_index.maxtime", EmitDefaultValue = false)]
        [DefaultValue("5m")]
        public string ActionSummaryIndexMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets the name of the summary index where the results of 
        /// the <see cref="SavedSearch"/> are stored.
        /// </summary>
        /// <remarks>
        /// The default summary index name is <c>"summary"</c>.
        /// </remarks>
        [DataMember(Name = "action.summary_index.name", EmitDefaultValue = false)]
        [DefaultValue("summary")]
        public string ActionSummaryIndexName
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the summary index 
        /// action of a <see cref="SavedSearch"/> signifies a trackable alert.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "action.summary_index.track_alert", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool ActionSummaryIndexTrackAlert
        { get; set; }

        /// <summary>
        /// Gets or sets the minimum time-to-live for artifacts of the summary
        /// index action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c><![CDATA[
        /// <integer>["p"] ]]></c>. If <c>p</c> follows <![CDATA[<integer>]]>, 
        /// the unit of time is the number of scheduled periods. Otherwise, the
        /// unit of time is seconds. The default is ten scheduled periods. 
        /// </remarks>
        [DataMember(Name = "action.summary_index.ttl", EmitDefaultValue = false)]
        [DefaultValue("10p")]
        public string ActionSummaryIndexTtl
        { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [DataMember(Name = "alert.digest_mode", EmitDefaultValue = false)]
        [DefaultValue(true)]
        public bool AlertDigestMode
        { get; set; }

        /// <summary>
        /// Gets or sets the period of time to show the alert in the dashboard. 
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c><![CDATA[
        /// <integer>("s"|"m"|"h"|"d")]]></c>. The default is <c>"24h"</c>.
        /// </remarks>
        [DataMember(Name = "alert.expires", EmitDefaultValue = false)]
        [DefaultValue("24h")]
        public string AlertExpires
        { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [DataMember(Name = "alert.severity", EmitDefaultValue = false)]
        [DefaultValue(AlertSeverity.Warning)]
        public AlertSeverity AlertSeverity
        { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [DataMember(Name = "alert.track", EmitDefaultValue = false)]
        [DefaultValue(AlertTrack.Automatic)]
        public AlertTrack AlertTrack
        { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [DataMember(Name = "alert.type", EmitDefaultValue = false)]
        [DefaultValue(AlertTrigger.None)]
        public AlertTrigger AlertTrigger
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the scheduler should 
        /// ensure that the data for this search is automatically summarized.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "auto_summarize", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool AutoSummarize
        { get; set; }

        /// <summary>
        /// Gets or sets a search command template that constructs the auto 
        /// summarization for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This is an advanced feature. Do not set this value unless you 
        /// understand the architecture of auto summarization of saved 
        /// searches. The default value is:
        /// <code>
        ///     summarize override=partial 
        ///     timespan=$auto_summarize.timespan$ 
        ///     max_summary_size=$auto_summarize.max_summary_size$ 
        ///     max_summary_ratio=$auto_summarize.max_summary_ratio$ 
        ///     max_disabled_buckets=$auto_summarize.max_disabled_buckets$ 
        ///     max_time=$auto_summarize.max_time$ 
        ///     [ $search$ ]
        /// </code>
        /// </remarks>
        [DataMember(Name = "auto_summarize.command", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string AutoSummarizeCommand
        { get; set; }

        /// <summary>
        /// Gets or sets a CRON schedule for auto-summarization of a <see cref=
        /// "SaveSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>"*/10 * * * *"</c> which schedules 
        /// auto-summarization on a ten hour schedule.
        /// </remarks>
        [DataMember(Name = "auto_summarize.cron_schedule", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string AutoSummarizeCronSchedule
        { get; set; }

        /// <summary>
        /// Gets or sets 
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [DataMember(Name = "auto_summarize.dispatch.time_format", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string AutoSummarizeDispatchTimeFormat
        { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [DataMember(Name = "auto_summarize.dispatch.ttl", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string AutoSummarizeDispatchTtl
        { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [DataMember(Name = "auto_summarize.max_disabled_buckets", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string AutoSummarizeMaxDisabledBuckets
        { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [DataMember(Name = "auto_summarize.max_summary_ratio", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string AutoSummarizeMaxSummaryRatio
        { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [DataMember(Name = "auto_summarize.max_summary_size", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string AutoSummarizeMaxSummarySize
        { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [DataMember(Name = "auto_summarize.max_time", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string AutoSummarizeMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [DataMember(Name = "auto_summarize.suspend_period", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string AutoSummarizeSuspendPeriod
        { get; set; }

        /// <summary>
        /// Gets or sets
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [DataMember(Name = "disabled", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool Disabled
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of timeline buckets for a search.
        /// </summary>
        /// <remarks>
        /// This default value is <c>0</c>.
        /// </remarks>        
        [DataMember(Name = "dispatch.buckets", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int DispatchBuckets
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the earliest time to begin a search.
        /// </summary>
        /// <remarks>
        /// The value can be a relative or absolute time. If it is an absolute 
        /// time, use the <see cref="DispatchTimeFormat"/> to format it.
        /// </remarks>
        [DataMember(Name = "dispatch.earliest_time", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string DispatchEarliestTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the latest time to begin a search.
        /// </summary>
        /// <remarks>
        /// The value can be a relative or absolute time. If it is an absolute
        /// time, use the <see cref="DispatchTimeFormat"/> to format it.
        /// </remarks>
        [DataMember(Name = "dispatch.latest_time", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string DispatchLatestTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether lookups are enabled.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>.
        /// </remarks>
        [DataMember(Name = "dispatch.lookups", EmitDefaultValue = false)]
        [DefaultValue(true)]
        public bool DispatchLookups
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of results to produce before 
        /// finalizing a search.
        /// </summary>
        [DataMember(Name = "dispatch.max_count", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int DispatchMaxCount
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of time to run before finalizing a
        /// search.
        /// </summary>
        [DataMember(Name = "dispatch.max_time", EmitDefaultValue = false)]
        [DefaultValue(0)]
        public int DispatchMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying how frequently Splunk runs the 
        /// map/reduce phase on accumulated map values.
        /// </summary>
        /// <remarks>
        /// The default value is <c>10</c>.
        /// </para>
        /// </remarks>
        [DataMember(Name = "dispatch.reduce_freq", EmitDefaultValue = false)]
        [DefaultValue(10)]
        public int DispatchReduceFrequency
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to back fill the real-time
        /// window for a search.
        /// </summary>
        /// <remarks>
        /// This value only applies to real-time searches.
        /// </remarks>
        [DataMember(Name = "dispatch.rt_backfill", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool DispatchRealTimeBackfill
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the search should run in its
        /// own process.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>. A searches against an index must 
        /// run in its own process.
        /// </remarks>
        [DataMember(Name = "dispatch.spawn_process", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool DispatchSpawnProcess
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the time format Splunk should use to
        /// for the earliest and latest times of a search.
        /// </summary>
        /// <remarks>
        /// The default value is <c>"%FT%T.%Q%:z"</c>.
        /// </remarks>
        [DataMember(Name = "dispatch.time_format", EmitDefaultValue = false)]
        [DefaultValue("%FT%T.%Q%:z")]
        public string DispatchTimeFormat
        { get; set; }

        /// <summary>
        /// Gets or sets the time to live (in seconds) for the artifacts of a
        /// search, if no actions are triggered.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <c>"2p"</c> (two scheduled search periods).
        /// </para><para>
        /// If an action is triggered, Splunk changes the time to live (TTL) to
        /// that action's TTL. If multiple actions are triggered, Splunk applies
        /// the maximum TTL to the artifacts. To set an action's TTL, refer to
        /// <a href="http://goo.gl/6YNR2X">alert_actions.conf.spec</a> in the
        /// Splunk Admin Manual.</para>
        /// </remarks>
        [DataMember(Name = "dispatch.time_format", EmitDefaultValue = false)]
        [DefaultValue("2p")]
        public string DispatchTtl
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating Whether a <see cref="SavedSearch"/>
        /// is to be run on a schedule.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        [DataMember(Name = "is_scheduled", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool IsScheduled
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="SavedSearch"/> 
        /// should be listed in the visible saved search list.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>.
        /// </remarks>
        [DataMember(Name = "is_visible", EmitDefaultValue = false)]
        [DefaultValue(true)]
        public bool IsVisible
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of concurrent instances of a 
        /// <see cref="SavedSearch"/> the scheduler is allowed to run.
        /// </summary>
        /// <remarks>
        /// The default value is one.
        /// </remarks>
        [DataMember(Name = "max_concurrent", EmitDefaultValue = false)]
        [DefaultValue(1)]
        public int MaxConcurrent
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="SavedSearch"/> 
        /// is to be run on a realtime schedule.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>. The scheduler bases its 
        /// determination of the next scheduled <see cref="SavedSearch"/> run
        /// time on the current time. If this value is <c>false</c>, the 
        /// scheduler bases its determination of the next scheduled <see cref=
        /// "SavedSearch"/> on the last run time. This is called continuous 
        /// scheduling. 
        /// <para>
        /// The scheduler never skips scheduled execution periods when a
        /// <see cref="SavedSearch"/> is configured for continuous scheduling. 
        /// However, the execution of the <see cref="SavedSearch"/> might fall 
        /// behind depending on the scheduler's load. Use continuous scheduling 
        /// whenever you enable the summary index option.</para>
        /// <para>
        /// When a <see cref="SavedSearch"/> is configured for realtime 
        /// scheduling, the scheduler might skip some execution periods to 
        /// make sure that the scheduler is executing the searches running 
        /// over the most recent time range. The scheduler tries to execute 
        /// searches configured for realtime scheduling before it executes 
        /// searches that have continuous scheduling. 
        /// </remarks>
        [DataMember(Name = "realtime_schedule", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string RealtimeSchedule
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to restart a realtime 
        /// <see cref="SavedSearch"/> managed by the scheduler when a new 
        /// search peer becomes available for the <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>.
        /// </remarks>
        [DataMember(Name = "restart_on_searchpeer_add", EmitDefaultValue = false)]
        [DefaultValue(null)]
        public string RestartOnSearchPeerAdd
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="SavedSearch"/>
        /// runs when Splunk starts.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c> indicating that after Splunk 
        /// starts, the <see cref="SavedSearch"/> runs at its next scheduled 
        /// time.
        /// </remarks>
        [DataMember(Name = "run_on_startup", EmitDefaultValue = false)]
        [DefaultValue(false)]
        public bool RunOnStartup
        { get; set; }

        #endregion
    }
}