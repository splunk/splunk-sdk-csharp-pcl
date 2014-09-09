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
//// [ ] Port DispatchTimeFormat and use it wherever appropriate. Can we create
////     a relative as well as an absolute time formatter?

namespace Splunk.Client
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides the arguments for creating a <see cref="SavedSearch"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/khXMdf">REST API: POST saved/searches.</a>
    /// </description></item>
    /// <item><description>
    ///   <a href="http://goo.gl/csTpM2">REST API: POST saved/searches/{name}.</a>
    /// </description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="T:Splunk.Client.Args{Splunk.Client.SavedSearchAttributes}"/>
    public sealed class SavedSearchAttributes : Args<SavedSearchAttributes>
    {
        /// <summary>
        /// Gets or sets a comma-separated list of actions to enable for a
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// No actions are enabled by default.
        /// </remarks>
        /// <value>
        /// A comma-separated list of actions to enable for a
        /// <see cref= "SavedSearch"/>.
        /// </value>
        [DataMember(Name = "actions", EmitDefaultValue = false)]
        public string Actions
        { get; set; }

        /// <summary>
        /// Gets or sets a password for authenticating with the SMTP server for the e-
        /// mail action of a <see cref="SavedSearch"/>.
        /// </summary>
        /// <value>
        /// A password for authenticating with the SMTP server for the e-mail action
        /// of a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.email.auth_password", EmitDefaultValue = false)]
        public string ActionEmailAuthPassword
        { get; set; }

        /// <summary>
        /// Gets or sets a username for authenticating with the SMTP server for the e-
        /// mail action of a <see cref="SavedSearch"/>.
        /// </summary>
        /// <value>
        /// A username for authenticating with the SMTP server for the e-mail action
        /// of a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.email.auth_username", EmitDefaultValue = false)]
        public string ActionEmailAuthUsername
        { get; set; }

        /// <summary>
        /// Gets or sets the e-mail addresses of the blind carbon copy (BCC)
        /// recipients for the e-mail action of a <see cref="SavedSearch"/>.
        /// </summary>
        /// <value>
        /// The e-mail addresses of the blind carbon copy (BCC) recipients for the e-
        /// mail action of a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.email.bcc", EmitDefaultValue = false)]
        public string ActionEmailBcc
        { get; set; }

        /// <summary>
        /// Gets or sets the e-mail addresses of the carbon copy (CC)
        /// recipients for the e-mail action of a <see cref="SavedSearch"/>.
        /// </summary>
        /// <value>
        /// The e-mail addresses of the carbon copy (CC) recipients for the e-mail
        /// action of a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.email.cc", EmitDefaultValue = false)]
        public string ActionEmailCC
        { get; set; }

        /// <summary>
        /// Gets or sets the search command which is responsible for executing the e-
        /// mail action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Generally the command is a template search pipeline which is realized
        /// with values from the <see cref="SavedSearch"/>. Reference template
        /// arguments by enclosing their names in dollar signs ($). For example, to
        /// reference <see cref="SavedSearch"/>.Name use
        /// <c>$name$</c>. To reference <see cref="SavedSearch"/>.Search use
        /// <c>$search$</c>.
        /// </remarks>
        /// <value>
        /// The search command which is responsible for executing the e-mail action
        /// for a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.email.command", EmitDefaultValue = false)]
        public string ActionEmailCommand
        { get; set; }

        /// <summary>
        /// Gets or sets the format of the e-mail action for a
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This value also applies to any attachments.
        /// </remarks>
        /// <value>
        /// The format of the e-mail action for a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.email.format", EmitDefaultValue = false)]
        public EmailFormat? ActionEmailFormat
        { get; set; }

        /// <summary>
        /// Gets or sets the e-mail address from which the e-mail action for a
        /// <see cref="SavedSearch"/> originates.
        /// </summary>
        /// <remarks>
        /// This vaulue defaults to <c>splunk@$LOCALHOST</c> or whatever value is set
        /// in <a href="http://goo.gl/odNige">alert_actions.conf</a>.
        /// </remarks>
        /// <value>
        /// The e-mail address from which the e-mail action for a
        /// <see cref= "SavedSearch"/> originates.
        /// </value>
        [DataMember(Name = "action.email.from", EmitDefaultValue = false)]
        public string ActionEmailFrom
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether search results should be
        /// contained in the body of the action e-mail for a
        /// <see cref= "SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Search results can be either inline or attached. See
        /// <see cref= "ActionEmailSendResults"/>.
        /// </remarks>
        /// <value>
        /// A value indicating whether search results should be contained in the body
        /// of the action e-mail for a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.email.inline", EmitDefaultValue = false)]
        public string ActionEmailInline
        { get; set; }

        /// <summary>
        /// Gets or sets the address of the MTA server to be used to send the action
        /// e-mail for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This value defaults to localhost or whatever value is set in
        /// <a href="http://goo.gl/odNige">alert_actions.conf</a>.
        /// </remarks>
        /// <value>
        /// The address of the MTA server to be used to send the action e-mail for a
        /// <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.email.mailserver", EmitDefaultValue = false)]
        public string ActionEmailMailServer
        { get; set; }

        /// <summary>
        /// Gets or sets the global maximum number of search results to send when the
        /// e-mail for a <see cref="SavedSearch"/> is enabled.
        /// </summary>
        /// <remarks>
        /// The default value is <c>100</c>.
        /// </remarks>
        /// <value>
        /// The global maximum number of search results to send when the e-mail for a
        /// <see cref="SavedSearch"/> is enabled.
        /// </value>
        [DataMember(Name = "action.email.maxresults", EmitDefaultValue = false)]
        public int? ActionEmailMaxResults
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum period of time the e-mail action for a
        /// <see cref="SavedSearch"/> may execute before it is aborted.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c>&lt;![CDATA[ &lt;integer&gt;
        /// ("s"|"m"|"h"|"d")]]&gt;</c>. The default is <c>"5m"</c>.
        /// </remarks>
        /// <value>
        /// The maximum period of time the e-mail action for a
        /// <see cref= "SavedSearch"/> may execute before it is aborted.
        /// </value>
        [DataMember(Name = "action.email.maxtime", EmitDefaultValue = false)]
        public string ActionEmailMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets the set and load order of CID fonts for handling Simplified
        /// Chinese (gb), Traditional Chinese (cns), Japanese (jp), and Korean (kor)
        /// in Integrated PDF Rendering.
        /// </summary>
        /// <remarks>
        /// The default value is <c>"gb cns jp kor"</c>.
        /// </remarks>
        /// <value>
        /// The set and load order of CID fonts for handling Simplified Chinese (gb),
        /// Traditional Chinese (cns), Japanese (jp), and Korean (kor) in Integrated
        /// PDF Rendering.
        /// </value>
        [DataMember(Name = "action.email.reportCIDFontList", EmitDefaultValue = false)]
        public string ActionEmailReportCidFontList
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include the Splunk logo with
        /// the report on an e-mail action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the report on an e-mail action for a <see cref=
        /// "SavedSearch"/> should include the Splunk logo; otherwise
        /// <c>false</c>.
        /// </value>
        [DataMember(Name = "action.email.reportIncludeSplunkLogo", EmitDefaultValue = false)]
        public bool? ActionEmailReportIncludeSplunkLogo
        { get; set; }

        /// <summary>
        /// Gets or sets the paper orientation for the report on an e-mail action for
        /// a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default is <see cref="PaperOrientation"/><c>.Portrait</c>.
        /// </remarks>
        /// <value>
        /// The paper orientation for the report on an e-mail action for a
        /// <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.email.reportPaperOrientation", EmitDefaultValue = false)]
        public PaperOrientation? ActionEmailReportPaperOrientation
        { get; set; }

        /// <summary>
        /// Gets or sets the paper size for the report on an e-mail action for a
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default is <see cref="PaperSize"/><c>.Letter</c>.
        /// </remarks>
        /// <value>
        /// The paper size for the report on an e-mail action for a
        /// <see cref= "SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.email.reportPaperSize", EmitDefaultValue = false)]
        public PaperSize? ActionEmailReportPaperSize
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the PDF server is enabled on
        /// the e-mail action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the PDF server is enabled on the e-mail action for a
        /// <see cref="SavedSearch"/>; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "action.email.reportServerEnabled", EmitDefaultValue = false)]
        public bool? ActionEmailReportServerEnabled
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to create and send the
        /// results of the e-mail action for a <see cref="SavedSearch"/> as a PDF
        /// report.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the results of the e-mail action for a
        /// <see cref= "SavedSearch"/> should be created and sent as a PDF report;
        /// otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "action.email.sendpdf", EmitDefaultValue = false)]
        public bool? ActionEmailSendPdf
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether search results should be
        /// included in the action e-mail for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Results can be either attached or inline. See
        /// <see cref= "ActionEmailInline"/>. The default value is <c>false</c>
        /// </remarks>
        /// <value>
        /// <c>true</c>, if search results should be included in the action e-mail
        /// for a <see cref="SavedSearch"/>; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "action.email.sendresults", EmitDefaultValue = false)]
        public bool? ActionEmailSendResults
        { get; set; }

        /// <summary>
        /// Gets or sets an alternate subject for the action e-mail on a
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>"SplunkAlert-"&lt;saved-search-name&gt;</c>.
        /// </remarks>
        /// <value>
        /// An alternate subject for the action e-mail on a
        /// <see cref= "SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.email.subject", EmitDefaultValue = false)]
        public string ActionEmailSubject
        { get; set; }

        /// <summary>
        /// Gets or sets the e-mail addresses of the recipients for the e-mail action
        /// of a <see cref="SavedSearch"/>.
        /// </summary>
        /// <value>
        /// Ehe e-mail addresses of the recipients for the e-mail action of a
        /// <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.email.to", EmitDefaultValue = false)]
        public string ActionEmailTo
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the e-mail action of a
        /// <see cref="SavedSearch"/> signifies a trackable alert.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the e-mail action of a <see cref="SavedSearch"/>
        /// signifies a trackable alert; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "action.email.track_alert", EmitDefaultValue = false)]
        public bool? ActionEmailTrackAlert
        { get; set; }

        /// <summary>
        /// Gets or sets the minimum time-to-live for artifacts of the e-mail action
        /// for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c>&lt;![CDATA[ &lt;integer&gt;
        /// ["p"] ]]&gt;</c>. If <c>p</c> follows &lt;![CDATA[&lt;integer&gt;]]&gt;,
        /// the unit of time is the number of scheduled periods. Otherwise, the unit
        /// of time is seconds. The default is 86,400 seconds; equivalent to 24 hours.
        /// </remarks>
        /// <value>
        /// The minimum time-to-live for artifacts of the e-mail action for a
        /// <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.email.ttl", EmitDefaultValue = false)]
        public string ActionEmailTtl
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use SSL when communicating
        /// with the SMTP server.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if SSL should be used when communicatin with the SMTP server;
        /// otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "action.email.use_ssl", EmitDefaultValue = false)]
        public bool? ActionEmailUseSsl
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use TLS when communicating
        /// with the SMTP server.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if TLS should be used when communication with the SMTP
        /// server.
        /// </value>
        [DataMember(Name = "action.email.use_tls", EmitDefaultValue = false)]
        public bool? ActionEmailUseTls
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether columns should be sorted from
        /// least wide to most wide, left to right.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if columns should be sorted from least wide to most wide,
        /// left to right; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "action.email.width_sort_columns", EmitDefaultValue = false)]
        public bool? ActionEmailWidthSortColumns
        { get; set; }

        /// <summary>
        /// Gets or sets the search command which is responsible for executing the
        /// populate lookup action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Generally the command is a template search pipeline which is realized
        /// with values from the <see cref="SavedSearch"/>. Reference template
        /// arguments by enclosing their names in dollar signs ($). For example, to
        /// reference <see cref="SavedSearch"/>.Name use
        /// <c>$name$</c>. To reference <see cref="SavedSearch"/>.Search use
        /// <c>$search$</c>.
        /// </remarks>
        /// <value>
        /// The search command which is responsible for executing the populate lookup
        /// action for a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.populate_lookup.command", EmitDefaultValue = false)]
        public string ActionPopulateLookupCommand
        { get; set; }

        /// <summary>
        /// Gets or sets the name of the lookup table or lookup path to fill in the
        /// lookup action of a <see cref="SavedSearch"/>.
        /// </summary>
        /// <value>
        /// The name of the lookup table or lookup path to fill in the lookup action
        /// of a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.populate_lookup.dest", EmitDefaultValue = false)]
        public string ActionPopulateLookupDestination
        { get; set; }

        /// <summary>
        /// Gets or sets the hostname used in the web link (URI) that is sent in the
        /// populate lookup action of a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Valid forms are <c>"hostname"</c> and <c>"protocol://hostname:port"</c>.
        /// </remarks>
        /// <value>
        /// The hostname used in the web link (URI) that is sent in the populate
        /// lookup action of a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.populate_lookup.hostname", EmitDefaultValue = false)]
        public string ActionPopulateLookupHostName
        { get; set; }

        /// <summary>
        /// Gets or sets the global maximum number of search results to send when the
        /// populate lookup action for a <see cref="SavedSearch"/> is enabled.
        /// </summary>
        /// <remarks>
        /// The default value is <c>100</c>.
        /// </remarks>
        /// <value>
        /// The global maximum number of search results to send when the populate
        /// lookup action for a <see cref="SavedSearch"/> is enabled.
        /// </value>
        [DataMember(Name = "action.populate_lookup.maxresults", EmitDefaultValue = false)]
        public int? ActionPopulateLookupMaxResults
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum period of time the populate lookup action for a
        /// <see cref="SavedSearch"/> may execute before it is aborted.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c>&lt;![CDATA[ &lt;integer&gt;
        /// ("s"|"m"|"h"|"d")]]&gt;</c>. The default is <c>"5m"</c>.
        /// </remarks>
        /// <value>
        /// The maximum period of time the populate lookup action for a
        /// <see cref="SavedSearch"/> may execute before it is aborted.
        /// </value>
        [DataMember(Name = "action.populate_lookup.maxtime", EmitDefaultValue = false)]
        public string ActionPopulateLookupMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the populate lookup action
        /// for a <see cref="SavedSearch"/> signifies a trackable alert.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the populate lookup action for a
        /// <see cref= "SavedSearch"/> signifies a trackable alert; <c>false</c>
        /// otherwise.
        /// </value>
        [DataMember(Name = "action.populate_lookup.track_alert", EmitDefaultValue = false)]
        public bool? ActionPopulateLookupTrackAlert
        { get; set; }

        /// <summary>
        /// Gets or sets the minimum time-to-live for artifacts of the populate
        /// lookup action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c>&lt;![CDATA[ &lt;integer&gt;
        /// ["p"] ]]&gt;</c>. If <c>p</c> follows &lt;![CDATA[&lt;integer&gt;]]&gt;,
        /// the unit of time is the number of scheduled periods. Otherwise, the unit
        /// of time is seconds. The default is 86,400 seconds; equivalent to 24 hours.
        /// </remarks>
        /// <value>
        /// The minimum time-to-live for artifacts of the populate lookup action for
        /// a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.populate_lookup.ttl", EmitDefaultValue = false)]
        public string ActionPopulateLookupTtl
        { get; set; }

        /// <summary>
        /// Gets or sets the search command which is responsible for executing the
        /// RSS action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Generally the command is a template search pipeline which is realized
        /// with values from the <see cref="SavedSearch"/>. Reference template
        /// arguments by enclosing their names in dollar signs ($). For example, to
        /// reference <see cref="SavedSearch"/>.Name use
        /// <c>$name$</c>. To reference <see cref="SavedSearch"/>.Search use
        /// <c>$search$</c>.
        /// </remarks>
        /// <value>
        /// The search command which is responsible for executing the RSS action for
        /// a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.rss.command", EmitDefaultValue = false)]
        public string ActionRssCommand
        { get; set; }

        /// <summary>
        /// Gets or sets the global maximum number of search results to send when the
        /// RSS action for a <see cref="SavedSearch"/> is enabled.
        /// </summary>
        /// <remarks>
        /// The default value is <c>100</c>.
        /// </remarks>
        /// <value>
        /// The global maximum number of search results to send when the RSS action
        /// for a <see cref="SavedSearch"/> is enabled.
        /// </value>
        [DataMember(Name = "action.rss.maxresults", EmitDefaultValue = false)]
        public int? ActionRssMaxResults
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum period of time the RSS action for a
        /// <see cref="SavedSearch"/> may execute before it is aborted.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c>&lt;![CDATA[ &lt;integer&gt;
        /// ("s"|"m"|"h"|"d")]]&gt;</c>. The default is <c>"1m"</c>.
        /// </remarks>
        /// <value>
        /// The maximum period of time the RSS action for a
        /// <see cref= "SavedSearch"/> may execute before it is aborted.
        /// </value>
        [DataMember(Name = "action.rss.maxtime", EmitDefaultValue = false)]
        public string ActionRssMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the RSS action of a
        /// <see cref="SavedSearch"/> signifies a trackable alert.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the RSS action of a <see cref="SavedSearch"/>
        /// signifies a trackable alert; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "action.rss.track_alert", EmitDefaultValue = false)]
        public string ActionRssTrackAlert
        { get; set; }

        /// <summary>
        /// Gets or sets the minimum time-to-live for artifacts of the RSS action for
        /// a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c>&lt;![CDATA[ &lt;integer&gt;
        /// ["p"] ]]&gt;</c>. If <c>p</c> follows &lt;![CDATA[&lt;integer&gt;]]&gt;,
        /// the unit of time is the number of scheduled periods. Otherwise, the unit
        /// of time is seconds. The default is 86,400 seconds; equivalent to 24 hours.
        /// </remarks>
        /// <value>
        /// The minimum time-to-live for artifacts of the RSS action for a
        /// <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.rss.ttl", EmitDefaultValue = false)]
        public string ActionRssTtl
        { get; set; }

        /// <summary>
        /// Gets or sets the search command which is responsible for executing the
        /// script action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Generally the command is a template search pipeline which is realized
        /// with values from the <see cref="SavedSearch"/>. Reference template
        /// arguments by enclosing their names in dollar signs ($). For example, to
        /// reference <see cref="SavedSearch"/>.Name use
        /// <c>$name$</c>. To reference <see cref="SavedSearch"/>.Search use
        /// <c>$search$</c>.
        /// </remarks>
        /// <value>
        /// The search command which is responsible for executing the script action
        /// for a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.script.command", EmitDefaultValue = false)]
        public string ActionScriptCommand
        { get; set; }

        /// <summary>
        /// Gets or sets the file name of the script to invoke when the script action
        /// of a <see cref="SavedSearch"/> is enabled.
        /// </summary>
        /// <remarks>
        /// A value is required when the script action of a
        /// <see cref= "SavedSearch"/> is enabled.
        /// </remarks>
        /// <value>
        /// The file name of the script to invoke when the script action of a
        /// <see cref="SavedSearch"/> is enabled.
        /// </value>
        [DataMember(Name = "action.script.filename", EmitDefaultValue = false)]
        public string ActionScriptFileName
        { get; set; }

        /// <summary>
        /// Gets or sets the hostname used in the web link (URI) that is sent in the
        /// script action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Valid forms are <c>"hostname"</c> and <c>"protocol://hostname:port"</c>.
        /// </remarks>
        /// <value>
        /// The hostname used in the web link (URI) that is sent in the script action
        /// for a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.script.hostname", EmitDefaultValue = false)]
        public string ActionScriptHostName
        { get; set; }

        /// <summary>
        /// Gets or sets the global maximum number of search results to send when the
        /// script action for a <see cref="SavedSearch"/> is enabled.
        /// </summary>
        /// <remarks>
        /// The default value is <c>100</c>.
        /// </remarks>
        /// <value>
        /// The global maximum number of search results to send when the script
        /// action for a <see cref="SavedSearch"/> is enabled.
        /// </value>
        [DataMember(Name = "action.script.maxresults", EmitDefaultValue = false)]
        public int? ActionScriptMaxResults
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum period of time the script action for a
        /// <see cref="SavedSearch"/> may execute before it is aborted.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c>&lt;![CDATA[ &lt;integer&gt;
        /// ("s"|"m"|"h"|"d")]]&gt;</c>. The default is <c>"5m"</c>.
        /// </remarks>
        /// <value>
        /// The maximum period of time the script action for a
        /// <see cref= "SavedSearch"/> may execute before it is aborted.
        /// </value>
        [DataMember(Name = "action.script.maxtime", EmitDefaultValue = false)]
        public string ActionScriptMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the script action of a
        /// <see cref="SavedSearch"/> signifies a trackable alert.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the script action of a <see cref="SavedSearch"/>
        /// signifies a trackable alert; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "action.script.track_alert", EmitDefaultValue = false)]
        public bool? ActionScriptTrackAlert
        { get; set; }

        /// <summary>
        /// Gets or sets the minimum time-to-live for artifacts of the script action
        /// for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c>&lt;![CDATA[ &lt;integer&gt;
        /// ["p"] ]]&gt;</c>. If <c>p</c> follows &lt;![CDATA[&lt;integer&gt;]]&gt;,
        /// the unit of time is the number of scheduled periods. Otherwise, the unit
        /// of time is seconds. The default is 600 seconds; equivalent to 10 minutes.
        /// </remarks>
        /// <value>
        /// The minimum time-to-live for artifacts of the script action for a
        /// <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.script.ttl", EmitDefaultValue = false)]
        public string ActionScriptTtl
        { get; set; }

        /// <summary>
        /// Gets or sets the search command which is responsible for executing the
        /// summary index action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Generally the command is a template search pipeline which is realized
        /// with values from the <see cref="SavedSearch"/>. Reference template
        /// arguments by enclosing their names in dollar signs ($). For example, to
        /// reference <see cref="SavedSearch"/>.Name use
        /// <c>$name$</c>. To reference <see cref="SavedSearch"/>.Search use
        /// <c>$search$</c>.
        /// </remarks>
        /// <value>
        /// The search command which is responsible for executing the summary index
        /// action for a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.summary_index.command", EmitDefaultValue = false)]
        public string ActionSummaryIndexCommand
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to execute the summary index
        /// action of a <see cref="SavedSearch"/> as part of the
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This option is considered only if the summary index action is enabled and
        /// is always executed. The default value is <c>true</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the index action of a <see cref="SavedSearch"/>
        /// should execute the summary index action as part of the
        /// <see cref= "SavedSearch"/>; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "action.summary_index.inline", EmitDefaultValue = false)]
        public bool? ActionSummaryIndexInline
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of search results sent via alerts for the
        /// index action of a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>100</c>.
        /// </remarks>
        /// <value>
        /// The maximum number of search results sent via alerts for the index action
        /// of a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.summary_index.maxresults", EmitDefaultValue = false)]
        public int? ActionSummaryIndexMaxResults
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum period of time the summary index action for a
        /// <see cref="SavedSearch"/> may execute before it is aborted.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c>&lt;![CDATA[ &lt;integer&gt;
        /// ("s"|"m"|"h"|"d")]]&gt;</c>. The default is <c>"5m"</c>.
        /// </remarks>
        /// <value>
        /// The maximum period of time the summary index action for a
        /// <see cref="SavedSearch"/> may execute before it is aborted.
        /// </value>
        [DataMember(Name = "action.summary_index.maxtime", EmitDefaultValue = false)]
        public string ActionSummaryIndexMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets the name of the summary index where the results of the
        /// <see cref="SavedSearch"/> are stored.
        /// </summary>
        /// <remarks>
        /// The default summary index name is <c>"summary"</c>.
        /// </remarks>
        /// <value>
        /// The name of the summary index where the results of the
        /// <see cref= "SavedSearch"/> are stored.
        /// </value>
        [DataMember(Name = "action.summary_index.name", EmitDefaultValue = false)]
        public string ActionSummaryIndexName
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the summary index action of a
        /// <see cref="SavedSearch"/> signifies a trackable alert.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the summary index action of a <see cref= "SavedSearch"/>
        /// signifies a trackable alert; <c>false</c>
        /// otherwise.
        /// </value>
        [DataMember(Name = "action.summary_index.track_alert", EmitDefaultValue = false)]
        public bool? ActionSummaryIndexTrackAlert
        { get; set; }

        /// <summary>
        /// Gets or sets the minimum time-to-live for artifacts of the summary index
        /// action for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c>&lt;![CDATA[ &lt;integer&gt;
        /// ["p"] ]]&gt;</c>. If <c>p</c> follows &lt;![CDATA[&lt;integer&gt;]]&gt;,
        /// the unit of time is the number of scheduled periods. Otherwise, the unit
        /// of time is seconds. The default is ten scheduled periods.
        /// </remarks>
        /// <value>
        /// The minimum time-to-live for artifacts of the summary index action for a
        /// <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "action.summary_index.ttl", EmitDefaultValue = false)]
        public string ActionSummaryIndexTtl
        { get; set; }

        /// <summary>
        /// Gets or sets the comparator used with <see cref="AlertThreshold"/>
        /// to trigger alert actions.
        /// </summary>
        /// <value>
        /// The comparator used with <see cref="AlertThreshold"/> to trigger alert
        /// actions.
        /// </value>
        [DataMember(Name = "alert_comparator", EmitDefaultValue = false)]
        public AlertComparator? AlertComparator
        { get; set; }

        /// <summary>
        /// Gets or sets a conditional search that is evaluated against the results
        /// of a saved search.
        /// </summary>
        /// <remarks>
        /// Alerts are triggered if the search yields a non-empty result. If you
        /// specify a value, do not set <c>counttype</c>, <c>relation</c>, or
        /// <c>quantity</c>.
        /// </remarks>
        /// <value>
        /// A conditional search that is evaluated against the results of a
        /// <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "alert_condition", EmitDefaultValue = false)]
        public string AlertCondition
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Splunk applies the alert actions
        /// to the entire result set or on each individual result.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if Splunk should apply the alert actions to the entire
        /// result set or on each individual result; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "alert.digest_mode", EmitDefaultValue = false)]
        public bool? AlertDigestMode
        { get; set; }

        /// <summary>
        /// Gets or sets the period of time to show the alert in the dashboard.
        /// </summary>
        /// <remarks>
        /// Express this value as a string of the form <c>&lt;![CDATA[ &lt;integer&gt;
        /// ("s"|"m"|"h"|"d")]]&gt;</c>. The default is <c>"24h"</c>.
        /// </remarks>
        /// <value>
        /// The period of time to show the alert in the dashboard.
        /// </value>
        [DataMember(Name = "alert.expires", EmitDefaultValue = false)]
        public string AlertExpires
        { get; set; }

        /// <summary>
        /// Gets or sets the alert severity level for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="AlertSeverity"/>.Warning.
        /// </remarks>
        /// <value>
        /// The alert severity level for a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "alert.severity", EmitDefaultValue = false)]
        public AlertSeverity? AlertSeverity
        { get; set; }

        /// <summary>
        /// Gets or sets a string that specifies an absolute or percentage value to
        /// compare using <see cref="AlertComparator"/> before triggering the alert
        /// actions for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This value is specified as a string of the form &lt;![CDATA[&lt;
        /// integer&gt; *1"%"]]&gt;.
        /// </remarks>
        /// <value>
        /// A string that specifies an absolute or percentage value to compare using
        /// <see cref="AlertComparator"/> before triggering the alert actions for a
        /// <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "alert_threshold", EmitDefaultValue = false)]
        public string AlertThreshold
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to track the actions triggered by
        /// a scheduled <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="AlertTrack"/>.Automatic.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the actions triggered by a scheduled
        /// <see cref= "SavedSearch"/> should be tracked; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "alert.track", EmitDefaultValue = false)]
        public AlertTrack? AlertTrack
        { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the basis of an alert.
        /// </summary>
        /// <remarks>
        /// This value is overriden by <see cref="AlertCondition"/> if it is also
        /// specified.
        /// </remarks>
        /// <value>
        /// A value that specifies the basis of an alert.
        /// </value>
        [DataMember(Name = "alert_type", EmitDefaultValue = false)]
        public AlertType? AlertType
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the scheduler should ensure
        /// that the data for a <see cref="SavedSearch"/> is automatically summarized.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the scheduler should ensure that the data for a
        /// <see cref="SavedSearch"/> is automatically summarized;
        /// otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "auto_summarize", EmitDefaultValue = false)]
        public bool? AutoSummarize
        { get; set; }

        /// <summary>
        /// Gets or sets a search command template that constructs the auto
        /// summarization for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This is an advanced feature. Do not set this value unless you understand
        /// the architecture of auto summarization of saved searches. The default
        /// value is:
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
        /// <value>
        /// A search command template that constructs the auto summarization for a
        /// <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "auto_summarize.command", EmitDefaultValue = false)]
        public string AutoSummarizeCommand
        { get; set; }

        /// <summary>
        /// Gets or sets a Cron schedule for auto-summarization of a
        /// <see cref= "SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>"*/10 * * * *"</c> which schedules auto-
        /// summarization on a ten hour schedule.
        /// </remarks>
        /// <value>
        /// A Cron schedule for auto-summarization of a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "auto_summarize.cron_schedule", EmitDefaultValue = false)]
        public string AutoSummarizeCronSchedule
        { get; set; }

        /// <summary>
        /// Gets or sets a time string that specifies the earliest time for
        /// summarizing a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This value can be specified as a relative or absolute time string. You
        /// must use the <see cref="AutoSummarizeDispatchTimeFormat"/> to specify an
        /// absolute time string.
        /// </remarks>
        /// <value>
        /// A time string that specifies the earliest time for summarizing a
        /// <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "auto_summarize.dispatch.earliest_time", EmitDefaultValue = false)]
        public string AutoSummarizeDispatchEarliestTime
        { get; set; }

        /// <summary>
        /// Gets or sets a time string that specifies the latest time for summarizing
        /// a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This value can be specified as a relative or absolute time string. You
        /// must use the <see cref="AutoSummarizeDispatchTimeFormat"/> to specify an
        /// absolute time string.
        /// </remarks>
        /// <value>
        /// A time string that specifies the latest time for summarizing a
        /// <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "auto_summarize.dispatch.latest_time", EmitDefaultValue = false)]
        public string AutoSummarizeDispatchLatestTime
        { get; set; }

        /// <summary>
        /// Gets or sets the time format that Splunk uses to parse the values of
        /// <see cref="AutoSummarizeDispatchEarliestTime"/> and
        /// <see cref="AutoSummarizeDispatchLatestTime"/> when they are
        /// specified as absolute times.
        /// </summary>
        /// <remarks>
        /// The default value is <c>"%FT%T.%Q%:z"</c>.
        /// </remarks>
        /// <value>
        /// The time format that Splunk uses to parse the values of
        /// <see cref= "AutoSummarizeDispatchEarliestTime"/> and
        /// <see cref= "AutoSummarizeDispatchLatestTime"/> when they are specified as
        /// absolute times.
        /// </value>
        [DataMember(Name = "auto_summarize.dispatch.time_format", EmitDefaultValue = false)]
        public string AutoSummarizeDispatchTimeFormat
        { get; set; }

        /// <summary>
        /// Gets or sets a string that specifies the time to live in seconds or
        /// periods for the artifacts of the summarization of a scheduled
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default value is <c>"60"</c>, equivalent to one minute.
        /// </remarks>
        /// <value>
        /// A string that specifies the time to live in seconds or periods for the
        /// artifacts of the summarization of a scheduled <see cref= "SavedSearch"/>.
        /// </value>
        [DataMember(Name = "auto_summarize.dispatch.ttl", EmitDefaultValue = false)]
        public string AutoSummarizeDispatchTtl
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of buckets with suspended summarization
        /// before a summarization search is completely stopped.
        /// </summary>
        /// <remarks>
        /// The summarization of the search is suspended for
        /// <see cref= "AutoSummarizeSuspendPeriod"/> when this value is reached. The
        /// default value is <c>2</c>.
        /// </remarks>
        /// <value>
        /// The maximum number of buckets with suspended summarization before a
        /// summarization search is completely stopped.
        /// </value>
        [DataMember(Name = "auto_summarize.max_disabled_buckets", EmitDefaultValue = false)]
        public int? AutoSummarizeMaxDisabledBuckets
        { get; set; }

        /// <summary>
        /// Gets or sets the ratio of summary_size/bucket_size which specifies when
        /// to stop summarization and deem it unhelpful for a bucket.
        /// </summary>
        /// <remarks>
        /// This value is only checked when the summary_size is larger than
        /// <see cref="AutoSummarizeMaxSummarySize"/>. The default value is
        /// <c>0.1</c>.
        /// </remarks>
        /// <value>
        /// The ratio of summary_size/bucket_size which specifies when to stop
        /// summarization and deem it unhelpful for a bucket.
        /// </value>
        [DataMember(Name = "auto_summarize.max_summary_ratio", EmitDefaultValue = false)]
        public double? AutoSummarizeMaxSummaryRatio
        { get; set; }

        /// <summary>
        /// Gets or sets the minimum summary size, in bytes, before testing whether
        /// the summarization is helpful.
        /// </summary>
        /// <remarks>
        /// The default value is <c>52428800</c>, equivalent to 5MB.
        /// </remarks>
        /// <value>
        /// The minimum summary size, in bytes, before testing whether the
        /// summarization is helpful.
        /// </value>
        [DataMember(Name = "auto_summarize.max_summary_size", EmitDefaultValue = false)]
        public long? AutoSummarizeMaxSummarySize
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum time in seconds that a summary search is allowed
        /// to run.
        /// </summary>
        /// <remarks>
        /// The default value is <c>3600</c>, equivalent to 60 minutes. This is an
        /// approximate time as the summary search stops at clean bucket boundaries.
        /// </remarks>
        /// <value>
        /// The maximum time in seconds that a summary search is allowed to run.
        /// </value>
        [DataMember(Name = "auto_summarize.max_time", EmitDefaultValue = false)]
        public long? AutoSummarizeMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets a time specfier that tells Splunk when to suspend
        /// summarization of a <see cref="SavedSearch"/>, if the summarization is
        /// deemed unhelpful.
        /// </summary>
        /// <remarks>
        /// The default time is <c>"24h"</c>.
        /// </remarks>
        /// <value>
        /// A time specfier that tells Splunk when to suspend summarization of a
        /// <see cref="SavedSearch"/>, if the summarization is deemed unhelpful.
        /// </value>
        [DataMember(Name = "auto_summarize.suspend_period", EmitDefaultValue = false)]
        public string AutoSummarizeSuspendPeriod
        { get; set; }

        /// <summary>
        /// Gets or sets the Cron schedule to execute a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// Use standard Cron notation to define the scheduled search interval.
        /// Splunk recommends that you schedule your searches so that they are
        /// staggered over time. This reduces system load. Running all of them every
        /// 20 minutes (*/20) means they would all launch at hh:00, hh:20, and hh:40
        /// and this might slow your system every twenty minutes.
        /// </remarks>
        /// <value>
        /// The Cron schedule to execute a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "cron_schedule", EmitDefaultValue = false)]
        public string CronSchedule
        { get; set; }

        /// <summary>
        /// Gets or sets the human-readable description of a
        /// <see cref= "SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The default value is an empty string.
        /// </remarks>
        /// <value>
        /// The human-readable description of a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if a <see cref="SavedSearch"/>
        /// should be disabled.
        /// </summary>
        /// <remarks>
        /// Disabled saved searches are not visible in Splunk Web.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the <see cref="SavedSearch"/> should be disabled;
        /// otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "disabled", EmitDefaultValue = false)]
        public bool? Disabled
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of timeline buckets for a
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This default value is <c>0</c>.
        /// </remarks>
        /// <value>
        /// The maximum number of timeline buckets for a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "dispatch.buckets", EmitDefaultValue = false)]
        public int? DispatchBuckets
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the earliest time to begin a
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The value can be a relative or absolute time. If it is an absolute time,
        /// use the <see cref="DispatchTimeFormat"/> to format it.
        /// </remarks>
        /// <value>
        /// A value specifying the earliest time to begin a
        /// <see cref= "SavedSearch"/>.
        /// </value>
        [DataMember(Name = "dispatch.earliest_time", EmitDefaultValue = false)]
        public string DispatchEarliestTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the latest time to begin a
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// The value can be a relative or absolute time. If it is an absolute time,
        /// use the <see cref="DispatchTimeFormat"/> to format it.
        /// </remarks>
        /// <value>
        /// A value specifying the latest time to begin a <see cref= "SavedSearch"/>.
        /// </value>
        [DataMember(Name = "dispatch.latest_time", EmitDefaultValue = false)]
        public string DispatchLatestTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether lookups are enabled.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if lookups are enabled; <c>false</c> othwerwise.
        /// </value>
        [DataMember(Name = "dispatch.lookups", EmitDefaultValue = false)]
        public bool? DispatchLookups
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of results to produce before finalizing a
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <value>
        /// The maximum number of results to produce before finalizing a
        /// <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "dispatch.max_count", EmitDefaultValue = false)]
        public int? DispatchMaxCount
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of time to run before finalizing a
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <value>
        /// The maximum length of time to run before finalizing a
        /// <see cref= "SavedSearch"/>.
        /// </value>
        [DataMember(Name = "dispatch.max_time", EmitDefaultValue = false)]
        public int? DispatchMaxTime
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying how frequently Splunk runs the map/reduce
        /// phase on accumulated map values.
        /// </summary>
        /// <remarks>
        /// The default value is <c>10</c>.
        /// </remarks>
        /// <value>
        /// A value specifying how frequently Splunk runs the map/reduce phase on
        /// accumulated map values.
        /// </value>
        [DataMember(Name = "dispatch.reduce_freq", EmitDefaultValue = false)]
        public int? DispatchReduceFrequency
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to back fill the real-time window
        /// for a <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This value only applies to real-time searches.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if the real-time window for a <see cref="SavedSearch"/>
        /// should be back filled; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "dispatch.rt_backfill", EmitDefaultValue = false)]
        public bool? DispatchRealTimeBackfill
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="SavedSearch"/>
        /// should run in its own process.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>. A searches against an index must run
        /// in its own process.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if a <see cref="SavedSearch"/> should run in its own process;
        /// otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "dispatch.spawn_process", EmitDefaultValue = false)]
        public bool? DispatchSpawnProcess
        { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the time format Splunk should use for the
        /// earliest and latest times of a search.
        /// </summary>
        /// <remarks>
        /// The default value is <c>"%FT%T.%Q%:z"</c>.
        /// </remarks>
        /// <value>
        /// A value specifying the time format Splunk should use for the earliest and
        /// latest times of a <see cref="SavedSearch"/>.
        /// </value>
        [DataMember(Name = "dispatch.time_format", EmitDefaultValue = false)]
        public string DispatchTimeFormat
        { get; set; }

        /// <summary>
        /// Gets or sets the time to live (in seconds) for the artifacts of a
        /// <see cref="SavedSearch"/>, if no actions are triggered.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <c>"2p"</c> (two scheduled search periods).
        /// </para><para>
        /// If an action is triggered, Splunk changes the time to live (TTL) to that
        /// action's TTL. If multiple actions are triggered, Splunk applies the
        /// maximum TTL to the artifacts. To set an action's TTL, refer to
        /// <a href="http://goo.gl/6YNR2X">alert_actions.conf.spec</a> in the
        /// Splunk Admin Manual.</para>
        /// </remarks>
        /// <value>
        /// The time to live (in seconds) for the artifacts of a
        /// <see cref= "SavedSearch"/>, if no actions are triggered.
        /// </value>
        [DataMember(Name = "dispatch.time_format", EmitDefaultValue = false)]
        public string DispatchTtl
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="SavedSearch"/>
        /// is to be run on a schedule.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if a <see cref="SavedSearch"/> is to be run on a schedule;
        /// otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "is_scheduled", EmitDefaultValue = false)]
        public bool? IsScheduled
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="SavedSearch"/>
        /// should appear in the list of saved searches on Splunk Web.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if a <see cref="SavedSearch"/> should appear in the list of
        /// saved searches on Splunk Web; <c>false</c> othwerwise.
        /// </value>
        [DataMember(Name = "is_visible", EmitDefaultValue = false)]
        public bool? IsVisible
        { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of concurrent instances of a
        /// <see cref="SavedSearch"/> the scheduler is allowed to run.
        /// </summary>
        /// <remarks>
        /// The default value is <c>1</c>.
        /// </remarks>
        /// <value>
        /// The maximum number of concurrent instances of a
        /// <see cref= "SavedSearch"/> the scheduler is allowed to run.
        /// </value>
        [DataMember(Name = "max_concurrent", EmitDefaultValue = false)]
        public int? MaxConcurrent
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="SavedSearch"/>
        /// is to be run on a realtime schedule.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>. The scheduler bases its determination
        /// of the next scheduled <see cref="SavedSearch"/> run time or the current
        /// time. If this value is <c>false</c>, the scheduler bases its
        /// determination of the next scheduled <see cref= "SavedSearch"/> on the
        /// last run time. This is called continuous scheduling.
        /// <para>
        /// The scheduler never skips scheduled execution periods when a
        /// <see cref="SavedSearch"/> is configured for continuous scheduling.
        /// However, the execution of the <see cref="SavedSearch"/> might fall behind
        /// depending on the scheduler's load. Use continuous scheduling whenever you
        /// enable the summary index option.</para>
        /// <para>
        /// When a <see cref="SavedSearch"/> is configured for realtime scheduling,
        /// the scheduler might skip some execution periods to make sure that the
        /// scheduler is executing the searches running over the most recent time
        /// range. The scheduler tries to execute searches configured for realtime
        /// scheduling before it executes searches that have continuous
        /// scheduling.</para>
        /// </remarks>
        /// <value>
        /// <c>true</c>, if a <see cref="SavedSearch"/> is to be run on a realtime
        /// schedule; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "realtime_schedule", EmitDefaultValue = false)]
        public string RealTimeSchedule
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to restart a realtime
        /// <see cref="SavedSearch"/> managed by the scheduler when a new
        /// search peer becomes available.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if a realtime <see cref="SavedSearch"/> managed by the
        /// scheduler should be restarted when a new search peer becomes available;
        /// otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "restart_on_searchpeer_add", EmitDefaultValue = false)]
        public string RestartOnSearchPeerAdd
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="SavedSearch"/>
        /// runs when Splunk starts.
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c> indicating that after Splunk starts,
        /// the <see cref="SavedSearch"/> runs at its next scheduled time.
        /// </remarks>
        /// <value>
        /// <c>true</c>, if a <see cref="SavedSearch"/> runs when Splunk starts;
        /// otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "run_on_startup", EmitDefaultValue = false)]
        public bool? RunOnStartup
        { get; set; }
    }
}