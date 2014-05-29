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
////
//// [O] Contracts
////
//// [O] Documentation
////
//// [ ] Trace messages (e.g., when there are no observers)

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Provides an object representation of a Splunk search job.
    /// </summary>
    public sealed class Job : Entity<Job>, IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Job"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="name">
        /// Name of the search <see cref="Job"/>.
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
        internal Job(Context context, Namespace ns, string name)
            : base(context, ns, JobCollection.ClassResourceName, name)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "Job"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use one of these
        /// methods to obtain a <see cref="Job"/> instance:
        /// <list type="table">
        /// <listheader>
        ///   <term>Method</term>
        ///   <description>Description</description>
        /// </listheader>
        /// <item>
        ///   <term><see cref="SavedSearch.DispatchAsync"/></term>
        ///   <description>
        ///   Asynchronously dispatches the current <see cref="SavedSearch"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.CreateJobAsync"/></term>
        ///   <description>
        ///   Asynchronously creates a new search <see cref="Job"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.DispatchSavedSearchAsync"/></term>
        ///   <description>
        ///   Asynchronously dispatches a <see cref="SavedSearch"/> identified
        ///   by name.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><see cref="Service.GetJobAsync"/></term>
        ///   <description>
        ///   Asynchronously retrieves a <see cref="Job"/> identified by search
        ///   ID.
        ///   </description>
        /// </item>
        /// </list>
        /// </remarks>
        public Job()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public bool CanSummarize
        {
            get { return this.GetValue("CanSummarize", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the earliest time from which no events are later scanned.
        /// </summary>
        /// <remarks>
        /// This value can be used to indicate progress. See <see cref=
        /// "DoneProgress"/>. 
        /// </remarks>
        public DateTime CursorTime
        {
            get { return this.GetValue("CursorTime", DateTimeConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int DefaultSaveTTL
        {
            get { return this.GetValue("DefaultSaveTTL", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int DefaultTTL
        {
            get { return this.GetValue("DefaultTTL", Int32Converter.Instance); }
        }

        /// <summary>
        /// The total number of bytes of disk space used by the current <see 
        /// cref="Job"/>.
        /// </summary>
        public long DiskUsage
        {
            get { return this.GetValue("DiskUsage", Int64Converter.Instance); } // sample value: "86016"
        }

        /// <summary>
        /// Gets the <see cref="DispatchState"/> of the current <see cref="Job"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="DispatchState"/> value.
        /// </returns>
        public DispatchState DispatchState
        {
            get { return this.GetValue("DispatchState", EnumConverter<DispatchState>.Instance); }
        }

        /// <summary>
        /// Gets a number between 0 and 1.0 that indicates the approximate 
        /// progress of the current <see cref="Job"/>
        /// </summary>
        /// <remarks>
        /// This value is computed as (<see cref="LatestTime"/> - <see cref=
        /// "CursorTime"/>) / (<see cref="LatestTime"/> - <see cref=
        /// "EarliestTime"/>).
        /// </remarks>
        public double DoneProgress
        {
            get { return this.GetValue("DoneProgress", DoubleConverter.Instance); }
        }

        /// <summary>
        /// Gets the number of possible events that were dropped from the 
        /// current <see cref="Job"/> due to the realtime queue size.
        /// </summary>
        /// <remarks>
        /// This value only applies to realtime search jobs.
        /// </remarks>
        public long DropCount
        {
            get { return this.GetValue("DropCount", Int64Converter.Instance); }
        }

        /// <summary>
        /// Gets the access control lists for the current <see cref="Job"/>.
        /// </summary>
        public Eai Eai
        {
            get { return this.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <summary>
        /// Gets the earliest time the current <see cref="Job"/> is configured
        /// to start.
        /// </summary>
        /// <remarks>
        /// This value can be used to indicate progress. See <see cref=
        /// "DoneProgress"/>.
        /// </remarks>
        public DateTime EarliestTime
        {
            get { return this.GetValue("EarliestTime", DateTimeConverter.Instance); }
        }

        /// <summary>
        /// Gets the number of events that are available for export from the 
        /// current <see cref="Job"/>.
        /// </summary>
        public long EventAvailableCount
        {
            get { return this.GetValue("EventAvailableCount", Int64Converter.Instance); }
        }

        /// <summary>
        /// Gets the number of events found by the current <see cref="Job"/>.
        /// </summary>
        public long EventCount
        {
            get { return this.GetValue("EventCount", Int64Converter.Instance); }
        }

        /// <summary>
        /// Gets the number of fields found in the search results produced by 
        /// the current <see cref="Job"/>.
        /// </summary>
        public int EventFieldCount
        {
            get { return this.GetValue("EventFieldCount", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates if the events of the current <see cref=
        /// "Job"/> are being streamed.
        /// </summary>
        public bool EventIsStreaming
        {
            get { return this.GetValue("EventIsStreaming", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates if the events produced by the current 
        /// <see cref="Job"/> have not been stored, and thus are not available 
        /// from the events endpoint for the <see cref="Job"/>.
        /// </summary>
        public bool EventIsTruncated
        {
            get { return this.GetValue("EventIsTruncated", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets that subset of the search command that appears before any 
        /// transforming commands.
        /// </summary>
        /// <remarks>
        /// The <a href="http://goo.gl/P3x68V">timeline</a> and <a href=
        /// "http://goo.gl/7kNQSb">events</a> endpoints represent the results
        /// of this part of the search.
        /// </remarks>
        public string EventSearch
        {
            get { return this.GetValue("EventSearch", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the <see cref="SortDirection"/> of the current <see cref="Job"/>.
        /// </summary>
        public SortDirection EventSorting
        {
            get { return this.GetValue("EventSorting", EnumConverter<SortDirection>.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public long IndexEarliestTime
        {
            get { return this.GetValue("IndexEarliestTime", Int64Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public long IndexLatestTime
        {
            get { return this.GetValue("IndexLatestTime", Int64Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsBatchModeSearch
        {
            get { return this.GetValue("IsBatchModeSearch", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref="Job"/>
        /// has completed.
        /// </summary>
        public bool IsDone
        {
            get { return this.GetValue("IsDone", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref="Job"/>
        /// failed due to a fatal error.
        /// </summary>
        /// <remarks>
        /// A <see cref="Job"/> may fail, for example, because of syntax errors
        /// in the search command.
        /// </remarks>
        public bool IsFailed
        {
            get { return this.GetValue("IsFailed", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref="Job"/>
        /// was finalized before completion.
        /// </summary>
        public bool IsFinalized
        {
            get { return this.GetValue("IsFinalized", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref="Job"/>
        /// is paused.
        /// </summary>
        public bool IsPaused
        {
            get { return this.GetValue("IsPaused", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates whether previews are enabled for the 
        /// current <see cref="Job"/>.
        /// </summary>
        public bool IsPreviewEnabled
        {
            get { return this.GetValue("IsPreviewEnabled", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref="Job"/>
        /// is executing a realtime search.
        /// </summary>
        public bool IsRealTimeSearch
        {
            get { return this.GetValue("IsRealTimeSearch", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates whether the timeline feature is enabled
        /// for the current <see cref="Job"/>.
        /// </summary>
        public bool IsRemoteTimeline
        {
            get { return this.GetValue("IsRemoteTimeline", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref="Job"/>
        /// is saved.
        /// </summary>
        /// <remarks>
        /// A value of <c>true</c> indicates that the current <see cref="Job"/>
        /// is saved, storing search artifacts on disk for 7 days from the last 
        /// time that the job was viewed or otherwise touched. Set the value
        /// of <c>default_save_ttl</c> <a href="http://goo.gl/OpE4lR">
        /// limits.conf</a> to override the default value.
        /// </remarks>
        public bool IsSaved
        {
            get { return this.GetValue("IsSaved", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates if the current <see cref="Job"/> is a 
        /// saved search that was run by the Splunk scheduler.
        /// </summary>
        public bool IsSavedSearch
        {
            get { return this.GetValue("IsSavedSearch", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates if the process running the current <see 
        /// cref="Job"/> is dead, but with the search not finished.
        /// </summary>
        public bool IsZombie
        {
            get { return this.GetValue("IsZombie", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the positive keywords used by this search.
        /// </summary>
        /// <remarks>
        /// A positive keyword is a keyword that is not in a <c>NOT</c> clause.
        /// </remarks>
        public string Keywords
        {
            get { return this.GetValue("Keywords", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the latest time a search job is configured to start.
        /// </summary>
        /// <remarks>
        /// This value can be used to indicate progress. See <see cref=
        /// "DoneProgress"/>.
        /// </remarks>
        public DateTime LatestTime
        {
            get { return this.GetValue("LatestTime", DateTimeConverter.Instance); }
        }

        //// TODO: Messages	{System.Dynamic.ExpandoObject}	System.Dynamic.ExpandoObject

        /// <summary>
        /// 
        /// </summary>
        public string NormalizedSearch
        {
            get { return this.GetValue("NormalizedSearch", StringConverter.Instance); } 
        }

        /// <summary>
        /// Gets the number of previews that have been generated by the current
        /// <see cref="Job"/> so far.
        /// </summary>
        public int NumPreviews
        {
            get { return this.GetValue("NumPreviews", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets the executions costs of the current <see cref="Job"/>.
        /// </summary>
        public dynamic Performance
        {
            get { return this.GetValue("Performance"); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Pid
        {
            get { return this.GetValue("Pid", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets an integer value between <c>0</c> and <c>10</c> that indicates
        /// the priority of the current <see cref="Job"/>.
        /// </summary>
        /// <remarks>
        /// The priority is mapped to the OS process priority. The higher the 
        /// number the higher the priority. The priority can be changed using 
        /// <see cref="SetPriorityAsync"/>. 
        /// <note type="note">
        /// On *nix systems, non-privileged users can only reduce the priority 
        /// of a process.
        /// </note>
        /// </remarks>
        public int Priority
        {
            get { return this.GetValue("Priority", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets the search string that is sent to every search peer.
        /// </summary>
        public string RemoteSearch
        {
            get { return this.GetValue("RemoteSearch", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the subset of the entire search command that is composed of
        /// reporting commands.
        /// </summary>
        /// <remarks>
        /// A value of <c>indicates</c> that search command has no reporting
        /// commands.
        /// </remarks>
        public string ReportSearch
        {
            get { return this.GetValue("ReportSearch", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public dynamic Request
        {
            get { return this.GetValue("Request"); }
        }

        /// <summary>
        /// Gets the total number of results returned by the search.
        /// </summary>
        /// <remarks>
        /// This is the subset of scanned events (represented by the <see cref=
        /// "ScanCount"/> property) that actually matches the search terms.
        /// </remarks>
        public long ResultCount
        {
            get { return this.GetValue("ResultCount", Int64Converter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates if the final results of the search are
        /// available using streaming.
        /// </summary>
        public bool ResultIsStreaming
        {
            get { return this.GetValue("ResultIsStreaming", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the number of result rows in the latest preview results.
        /// </summary>
        public long ResultPreviewCount
        {
            get { return this.GetValue("ResultPreviewCount", Int64Converter.Instance); }
        }

        /// <summary>
        /// Gets the time in seconds that the current <see cref="Job"/> took
        /// to complete.
        /// </summary>
        public double RunDuration
        {
            get { return this.GetValue("RunDuration", DoubleConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Runtime_t Runtime
        {
            get { return this.GetValue("Runtime", Runtime_t.Converter.Instance); }
        }

        /// <summary>
        /// Gets the number of events that are scanned or read from disk.
        /// </summary>
        public long ScanCount
        {
            get { return this.GetValue("ScanCount", Int64Converter.Instance); }
        }

        /// <summary>
        /// Gets the full text of the search command for the current <see cref=
        /// "Job"/>.
        /// </summary>
        public string Search
        {
            get { return this.Snapshot.Title; }
        }

        /// <summary>
        /// Gets the earliest time for the current <see cref="Job"/> as 
        /// specified in the search command rather than the <see cref=
        /// "EarliestTime"/> parameter.
        /// </summary>
        /// <remarks>
        /// This value does not snap to the indexed data time bounds for 
        /// all-time searches as <see cref="EarliestTime"/> and <see cref=
        /// "LatestTime"/> do.
        /// </remarks>
        public DateTime SearchEarliestTime
        {
            get { return this.GetValue("SearchEarliestTime", DateTimeConverter.Instance); }
        }

        /// <summary>
        /// Gets the latest time for the current <see cref="Job"/> as specified
        /// the search command rather than the <see cref="LatestTime"/> 
        /// parameter.
        /// </summary>
        /// <remarks>
        /// This value does not snap to the indexed data time bounds for 
        /// all-time searches as <see cref="EarliestTime"/> and <see cref=
        /// "LatestTime"/> do.
        /// </remarks>
        public DateTime SearchLatestTime
        {
            get { return this.GetValue("LatestTime", DateTimeConverter.Instance); }
        }

        /// <summary>
        /// Gets the list of all search peers that were contacted by the current
        /// <see cref="Job"/>
        /// </summary>
        public IReadOnlyList<string> SearchProviders
        {
            get { return this.GetValue("SearchProviders", CollectionConverter<string, List<string>, StringConverter>.Instance); }
        }

        /// <summary>
        /// Gets the search ID for the current <see cref="Job"/>.
        /// </summary>
        /// <remarks>
        /// This property is a synonym for <see cref="Resource&lt;TResource&gt;.Name"/>.
        /// </remarks>
        public string Sid
        {
            get { return this.Name; }
        }

        /// <summary>
        /// Gets the maximum number of timeline buckets.
        /// </summary>
        public int StatusBuckets
        {
            get { return this.GetValue("StatusBuckets", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets the time in seconds before the current <see cref="Job"/> expires
        /// after it completes.
        /// </summary>
        public long Ttl
        {
            get { return this.GetValue("Ttl", Int64Converter.Instance); }
        }

        #endregion

        #region Methods for getting, removing, and updating

        /// <summary>
        /// Asynchronously retrieves a fresh copy of the current <see cref=
        /// "Job"/> that contains all changes to it since it was last 
        /// retrieved.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SFqSPI">GET 
        /// search/jobs/{search_id}</a> endpoint to update the cached state of 
        /// the current <see cref="Job"/>.
        /// </remarks>
        public override async Task GetAsync()
        {
            await GetAsync(DispatchState.Running);
        }

        /// <summary>
        /// Asynchronously retrieves a fresh copy of the current <see cref=
        /// "Job"/> that is in or has moved past a desired <see cref=
        /// "DispatchState"/> and contains all changes to it since it was last 
        /// retrieved.
        /// </summary>
        /// <param name="dispatchState">
        /// Desired dispatch state.
        /// </param>
        /// <param name="delay">
        /// Number of milliseconds to wait for the current <see cref="Job"/> to 
        /// move into the desired <paramref name="dispatchState"/>.
        /// </param>
        /// <param name="retryInterval">
        /// Number of milliseconds to wait between checks for the dispatch
        /// state of the current <see cref="Job"/>. This value is increased
        /// by 50% on each retry.
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// This method always retrieves a fresh copy of the current <see cref=
        /// "Job"/>.
        /// </remarks>
        public async Task GetAsync(DispatchState dispatchState, int delay = 30000, int retryInterval = 250)
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(delay);
                var token = cancellationTokenSource.Token;

                for (int i = 0; ; i++)
                {
                    using (var response = await this.Context.GetAsync(this.Namespace, this.ResourceName, token))
                    {
                        if (response.Message.StatusCode != HttpStatusCode.NoContent)
                        {
                            await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
                            await this.UpdateSnapshotAsync(response);

                            if (this.DispatchState >= dispatchState)
                            {
                                break;
                            }
                        }
                    }

                    await Task.Delay(retryInterval);
                    retryInterval += retryInterval / 2;
                }
            }
        }

        /// <summary>
        /// Removes the current <see cref="Job"/>.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SFqSPI">DELETE 
        /// search/jobs/{search_id}</a> endpoint to remove the current <see 
        /// cref="Job"/>.
        /// </remarks>
        public async Task RemoveAsync()
        {
            using (var response = await this.Context.DeleteAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        public void Dispose()
        {
            this.CancelAsync().Wait();
            this.RemoveAsync().Wait();
        }

        /// <summary>
        /// Asynchronously retrieves a fresh copy of the current <see cref=
        /// "Job"/> that is in or has moved past a desired <see cref=
        /// "DispatchState"/> and contains all changes to it since it was last 
        /// retrieved.
        /// </summary>
        /// <param name="dispatchState">
        /// Desired dispatch state.
        /// </param>
        /// <param name="delay">
        /// Number of milliseconds to wait for the current <see cref="Job"/> to 
        /// move into the desired <paramref name="dispatchState"/>.
        /// </param>
        /// <param name="retryInterval">
        /// Number of milliseconds to wait between checks for the dispatch
        /// state of the current <see cref="Job"/>. This value is increased
        /// by 50% on each retry.
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// This method returns immediately if <see cref="DispatchState"/> is 
        /// greater than or equal to <paramref name="dispatchState"/>.
        /// </remarks>
        public async Task TransitionAsync(DispatchState dispatchState, int delay = 30000, int retryInterval = 250)
        {
            if (this.DispatchState >= dispatchState)
                return;
            await this.GetAsync(dispatchState, delay, retryInterval);
        }

        /// <summary>
        /// Updates custom arguments to the current <see cref="Job"/>.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/bL4tFk">POST 
        /// search/jobs/{search_id}</a> endpoint to update custom arguments to
        /// the current <see cref="Job"/>.
        /// </remarks>
        public async Task UpdateAsync(CustomJobArgs args)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, args))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Infrastructure. Asynchronously brings the current <see cref=
        /// "Job"/> up to date with new metadata and content.
        /// </summary>
        /// <param name="response">
        /// An <see cref="AtomFeed"/> or <see cref="AtomEntry"/> response.
        /// </param>
        protected override async Task UpdateSnapshotAsync(Response response)
        {
            var reader = response.XmlReader;

            reader.Requires(await reader.MoveToDocumentElementAsync("feed", "entry"));
            AtomEntry entry;

            if (reader.Name == "feed")
            {
                AtomFeed feed = new AtomFeed();
                await feed.ReadXmlAsync(reader);

                if (feed.Entries.Count != 1)
                {
                    throw new InvalidDataException(); // TODO: Diagnostics : cardinality violation
                }

                entry = feed.Entries[0];
            }
            else
            {
                entry = new AtomEntry();
                await entry.ReadXmlAsync(reader);
            }

            this.Snapshot = new EntitySnapshot(entry);
        }

        #endregion

        #region Methods for retrieving search results

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<SearchResultStream> GetSearchResultsAsync(SearchResultsArgs args = null)
        {
            await this.TransitionAsync(DispatchState.Done);

            var searchResults = await this.GetSearchResultsAsync("results", args);
            return searchResults;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<SearchResultStream> GetSearchResultsEventsAsync(SearchEventArgs args = null)
        {
            await this.TransitionAsync(DispatchState.Done);

            var searchResults = await this.GetSearchResultsAsync("events", args);
            return searchResults;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<SearchResultStream> GetSearchResultsPreviewAsync(SearchResultsArgs args = null)
        {
            await this.TransitionAsync(DispatchState.Running);

            var searchResults = await this.GetSearchResultsAsync("results_preview", args);
            return searchResults;
        }

        #endregion

        #region Job Control methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task CancelAsync()
        {
            await this.TransitionAsync(DispatchState.Running);
            await this.PostControlCommandAsync(Cancel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task DisablePreviewAsync()
        {
            await this.TransitionAsync(DispatchState.Running);
            await this.PostControlCommandAsync(DisablePreview);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task EnablePreviewAsync()
        {
            await this.TransitionAsync(DispatchState.Running);
            await this.PostControlCommandAsync(EnablePreview);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task FinalizeAsync()
        {
            await this.TransitionAsync(DispatchState.Running);
            await this.PostControlCommandAsync(Finalize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task PauseAsync()
        {
            await this.TransitionAsync(DispatchState.Running);
            await this.PostControlCommandAsync(Pause);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await this.TransitionAsync(DispatchState.Running);
            await this.PostControlCommandAsync(Save);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public async Task SetPriorityAsync(int priority)
        {
            await this.TransitionAsync(DispatchState.Running);

            await this.PostControlCommandAsync(new Argument[] 
            { 
                new Argument("action", "priority"),
                new Argument("priority", priority.ToString())
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public async Task SetTtlAsync(int ttl)
        {
            await this.TransitionAsync(DispatchState.Running);

            await this.PostControlCommandAsync(new Argument[]
            { 
                new Argument("action", "setttl"),
                new Argument("ttl", ttl.ToString())
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public async Task TouchAsync(int ttl)
        {
            await this.TransitionAsync(DispatchState.Running);

            await this.PostControlCommandAsync(new Argument[]
            { 
                new Argument("action", "touch"),
                new Argument("ttl", ttl.ToString())
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task UnpauseAsync()
        {
            await this.TransitionAsync(DispatchState.Running);
            await this.PostControlCommandAsync(Unpause);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task UnsaveAsync()
        {
            await this.TransitionAsync(DispatchState.Running);
            await this.PostControlCommandAsync(Unsave);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// 
        /// </param>
        /// <returns></returns>
        public async Task UpdateJobArgs(JobArgs args)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, args))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        #endregion

        #region Methods used by our base class, Entity<TEntity>

        #endregion

        #region Privates

        static readonly Argument[] Cancel = new Argument[] 
        { 
            new Argument("action", "cancel")
        };

        static readonly Argument[] DisablePreview = new Argument[] 
        { 
            new Argument("action", "disable_preview") 
        };

        static readonly Argument[] EnablePreview = new Argument[] 
        { 
            new Argument("action", "enable_preview") 
        };

        static readonly Argument[] Finalize = new Argument[] 
        { 
            new Argument("action", "finalize") 
        };

        static readonly Argument[] Pause = new Argument[]
        { 
            new Argument("action", "pause") 
        };

        static readonly Argument[] Save = new Argument[]
        {
            new Argument("action", "save") 
        };

        static readonly Argument[] Unpause = new Argument[] 
        { 
            new Argument("action", "unpause") 
        };

        static readonly Argument[] Unsave = new Argument[] 
        { 
            new Argument("action", "unsave") 
        };

        async Task<SearchResultStream> GetSearchResultsAsync(string endpoint, IEnumerable<Argument> args)
        {
            var resourceName = new ResourceName(this.ResourceName, endpoint);
            var response = await this.Context.GetAsync(this.Namespace, resourceName, args);

            try
            {
                var searchResults = await SearchResultStream.CreateAsync(response);
                return searchResults;
            }
            catch 
            {
                response.Dispose();
                throw;
            }
        }

        async Task PostControlCommandAsync(IEnumerable<Argument> args)
        {
            var resourceName = new ResourceName(this.ResourceName, "control");

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, args))
            {
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        #endregion

        #region Types

        public class Runtime_t : ExpandoAdapter<Runtime_t>
        {
            public bool AutoCancel
            {
                get { return this.GetValue("AutoCancel", BooleanConverter.Instance); }
            }

            public bool AutoPause
            {
                get { return this.GetValue("AutoPause", BooleanConverter.Instance); }
            }
        }
        #endregion
    }
}
