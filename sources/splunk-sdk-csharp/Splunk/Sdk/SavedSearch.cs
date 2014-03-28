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
        /// <param name="collection">
        /// </param>
        /// <param name="name">
        /// </param>
        internal SavedSearch(Context context, Namespace @namespace, ResourceName collection, string name)
            : base(context, @namespace, collection, name)
        { }

        public SavedSearch()
        { }

        #endregion

        #region Properties

        public ActionsAdapter Actions
        {
            get { return this.Content.GetValue("Action", ExpandoAdapter.Converter<ActionsAdapter>.Instance); }
        }

        public AlertAdapter Alert
        {
            get { return this.Content.GetValue("Alert", ExpandoAdapter.Converter<AlertAdapter>.Instance); }
        }

        public AutoSummarizeAdapter AutoSummarize
        {
            get { return this.Content.GetValue("AutoSummarize", ExpandoAdapter.Converter<AutoSummarizeAdapter>.Instance); }
        }

        public DispatchAdapter Dispatch
        {
            get { return this.Content.GetValue("Dispatch", ExpandoAdapter.Converter<DispatchAdapter>.Instance); }
        }

        public DisplayAdapter Display
        {
            get { return this.Content.GetValue("Display", ExpandoAdapter.Converter<DisplayAdapter>.Instance); }
        }

        public Eai Eai
        {
            get { return this.Content.GetValue("Eai", ExpandoAdapter.Converter<Eai>.Instance); }
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

        public RequestAdapter Request
        {
            get { return this.Content.GetValue("Request", ExpandoAdapter.Converter<RequestAdapter>.Instance); }
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

        #endregion

        #region Privates

        #endregion

        #region Types

        public class ActionsAdapter : ExpandoAdapter
        {
            #region Properties

            public EmailActionAdapter Email
            {
                get { return this.GetValue("Email", ExpandoAdapter.Converter<EmailActionAdapter>.Instance); }
            }

            public ActionAdapter PopulateLookup
            {
                get { return this.GetValue("PopulateLookup", ExpandoAdapter.Converter<ActionAdapter>.Instance); }
            }

            public ActionAdapter Rss
            {
                get { return this.GetValue("Rss", ExpandoAdapter.Converter<ActionAdapter>.Instance); }
            }

            public SummaryIndexAdapter SummaryIndex
            {
                get { return this.GetValue("SummaryIndex", ExpandoAdapter.Converter<SummaryIndexAdapter>.Instance); }
            }

            #endregion

            #region Types

            public class ActionAdapter : ExpandoAdapter
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

            public class EmailActionAdapter : ActionAdapter
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

            public class SummaryIndexAdapter : ActionAdapter
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

        public class AlertAdapter : ExpandoAdapter
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

        public class AutoSummarizeAdapter : ExpandoAdapter
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

            public DispatchAdapter Dispatch
            {
                get { return this.GetValue("Dispatch", ExpandoAdapter.Converter<DispatchAdapter>.Instance); }
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

            public class DispatchAdapter : ExpandoAdapter
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

        public class DispatchAdapter : ExpandoAdapter
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

        public class DisplayAdapter : ExpandoAdapter
        {
            #region Properties

            public EventsAdapter Events
            {
                get { return this.GetValue("Events", ExpandoAdapter.Converter<EventsAdapter>.Instance); }
            }

            public GeneralAdapter General
            {
                get { return this.GetValue("General", ExpandoAdapter.Converter<GeneralAdapter>.Instance); }
            }

            public PageAdapter Page
            {
                get { return this.GetValue("Page", ExpandoAdapter.Converter<PageAdapter>.Instance); }
            }

            public StatisticsAdapter Statistics
            {
                get { return this.GetValue("Statistics", ExpandoAdapter.Converter<StatisticsAdapter>.Instance); }
            }

            #endregion

            #region Types

            public VisualizationsAdapter Visualizations
            {
                get { return this.GetValue("Visualizations", ExpandoAdapter.Converter<VisualizationsAdapter>.Instance); }
            }

            public class EventsAdapter : ExpandoAdapter
            {
                #region Properties

                public string Fields // TODO: Deal sensibily with this Pythonic string format: @"["host","source","sourcetype"]"
                {
                    get { return this.GetValue("Fields", StringConverter.Instance); }
                }

                public ListAdapter List
                {
                    get { return this.GetValue("List", ExpandoAdapter.Converter<ListAdapter>.Instance); }
                }
                    
                public int MaxLines // TODO: Verify this property is a bool
                {
                    get { return this.GetValue("MaxLines", Int32Converter.Instance); }
                }

                public RawAdapter Raw
                {
                    get { return this.GetValue("Raw", ExpandoAdapter.Converter<RawAdapter>.Instance); }
                }

                public bool RowNumbers // TODO: Verify this property is a bool
                {
                    get { return this.GetValue("RowNumbers", BooleanConverter.Instance); }
                }

                public TableAdapter Table
                {
                    get { return this.GetValue("Table", ExpandoAdapter.Converter<TableAdapter>.Instance); }
                }

                public string Type // TODO: Encode this property as an enumeration
                {
                    get { return this.GetValue("Type", StringConverter.Instance); }
                }

                #endregion

                #region Types

                public class ListAdapter : ExpandoAdapter
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

                public class RawAdapter : ExpandoAdapter
                {
                    public string Drilldown // TODO: Encode this property as an enumeration
                    {
                        get { return this.GetValue("Drilldown", StringConverter.Instance); }
                    }
                }

                public class TableAdapter : ExpandoAdapter
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

            public class GeneralAdapter : ExpandoAdapter
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
                    get { return this.GetValue("TimeRangePicker", ExpandoAdapter.Converter<TimeRangePickerAdapter>.Instance); }
                }

                public string Type // TODO: Encode as enumeration
                {
                    get { return this.GetValue("Type", StringConverter.Instance); }
                }

                #region Types

                public class TimeRangePickerAdapter : ExpandoAdapter
                {
                    public bool Show
                    {
                        get { return this.GetValue("Show", BooleanConverter.Instance); }
                    }
                }

                #endregion
            }

            public class PageAdapter : ExpandoAdapter
            {
                #region Properties

                public PivotAdapter Pivot
                {
                    get { return this.GetValue("Pivot", ExpandoAdapter.Converter<PivotAdapter>.Instance); }
                }

                public SearchAdapter Search
                {
                    get { return this.GetValue("Search", ExpandoAdapter.Converter<SearchAdapter>.Instance); }
                }

                #endregion

                #region Types

                public class PivotAdapter : ExpandoAdapter // TODO: Implement properties (when do they show?)
                { }

                public class SearchAdapter : ExpandoAdapter
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
                        get { return this.GetValue("Search", ExpandoAdapter.Converter<TimelineAdapter>.Instance); }
                    }

                    #endregion

                    #region Types

                    public class TimelineAdapter : ExpandoAdapter
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

            public class StatisticsAdapter : ExpandoAdapter
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

            public class VisualizationsAdapter : ExpandoAdapter // TODO: Fill in the remainder
            {
                #region Properties

                public int ChartHeight
                {
                    get { return this.GetValue("ChartHeight", Int32Converter.Instance); }
                }

                public ChartingAdapter Charting
                {
                    get { return this.GetValue("Charting", ExpandoAdapter.Converter<ChartingAdapter>.Instance); }
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

                public class ChartingAdapter : ExpandoAdapter
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

        public class RequestAdapter : ExpandoAdapter
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
