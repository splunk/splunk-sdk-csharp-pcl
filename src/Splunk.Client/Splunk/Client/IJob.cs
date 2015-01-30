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

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an operational interface to a search job.
    /// </summary>
    /// <seealso cref="T:IEntity"/>
    [ContractClass(typeof(IJobContract))]
    public interface IJob : IEntity
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether we can summarize.
        /// </summary>
        /// <value>
        /// <c>true</c> if we can summarize, <c>false</c> if not.
        /// </value>
        bool CanSummarize
        { get; }

        /// <summary>
        /// Gets the earliest time from which no events are later scanned.
        /// </summary>
        /// <remarks>
        /// This value can be used to indicate progress. See <see cref="DoneProgress"/>.
        /// </remarks>
        /// <value>
        /// The earliest time from which no events are later scanned.
        /// </value>
        DateTime CursorTime
        { get; }

        /// <summary>
        /// Gets the default save TTL.
        /// </summary>
        /// <value>
        /// The default save TTL.
        /// </value>
        int DefaultSaveTtl
        { get; }

        /// <summary>
        /// Gets the default TTL.
        /// </summary>
        /// <value>
        /// The default TTL.
        /// </value>
        int DefaultTtl
        { get; }

        /// <summary>
        /// Gets the total number of bytes of disk space used by the current 
        /// search job.
        /// </summary>
        /// <value>
        /// The total number of bytes of disk space used by the current search
        /// job.
        /// </value>
        long DiskUsage
        { get; }

        /// <summary>
        /// Gets the <see cref="DispatchState"/> of the current search job.
        /// </summary>
        /// <value>
        /// The <see cref="DispatchState"/> of the current search job.
        /// </value>
        DispatchState DispatchState
        { get; }

        /// <summary>
        /// Gets a number between 0 and 1.0 that indicates the approximate 
        /// progress of the current search job.
        /// </summary>
        /// <remarks>
        /// This value is computed as (<see cref="LatestTime"/> - <see cref=
        /// "CursorTime"/>) / (<see cref="LatestTime"/> - <see cref=
        /// "EarliestTime"/>).
        /// </remarks>
        /// <value>
        /// The done progress.
        /// </value>
        double DoneProgress
        { get; }

        /// <summary>
        /// Gets the number of possible events that were dropped from the current
        /// search job due to the realtime queue size.
        /// </summary>
        /// <remarks>
        /// This value only applies to realtime search jobs.
        /// </remarks>
        /// <value>
        /// The number of drops.
        /// </value>
        long DropCount
        { get; }

        /// <summary>
        /// Gets the extensible administration interface properties for the
        /// current search job.
        /// </summary>
        /// <value>
        /// The extensible administration interface properties.
        /// </value>
        Eai Eai
        { get; }

        /// <summary>
        /// Gets the earliest time the current search job is configured to
        /// start.
        /// </summary>
        /// <remarks>
        /// This value can be used to indicate progress. See <see cref= "DoneProgress"/>.
        /// </remarks>
        /// <value>
        /// The earliest time.
        /// </value>
        DateTime EarliestTime
        { get; }

        /// <summary>
        /// Gets the number of events that are available for export from the 
        /// current search job.
        /// </summary>
        /// <value>
        /// The number of events that are available for export from the current
        /// search job.
        /// </value>
        long EventAvailableCount
        { get; }

        /// <summary>
        /// Gets the number of events found by the current search job.
        /// </summary>
        /// <value>
        /// The number of events.
        /// </value>
        long EventCount
        { get; }

        /// <summary>
        /// Gets the number of fields found in the search results produced by 
        /// the current search job.
        /// </summary>
        /// <value>
        /// The number of event fields.
        /// </value>
        int EventFieldCount
        { get; }

        /// <summary>
        /// Gets a value that indicates if the events of the current search job
        /// are being streamed.
        /// </summary>
        /// <value>
        /// <c>true</c> if event is streaming, <c>false</c> if not.
        /// </value>
        bool EventIsStreaming
        { get; }

        /// <summary>
        /// Gets a value that indicates if the events produced by the current
        /// search job have not been stored, and thus are not available from 
        /// the events endpoint for the job.
        /// </summary>
        /// <value>
        /// <c>true</c> if event is truncated, <c>false</c> if not.
        /// </value>
        bool EventIsTruncated
        { get; }

        /// <summary>
        /// Gets that subset of the search command that appears before any
        /// transforming commands.
        /// </summary>
        /// <remarks>
        /// The <a href="http://goo.gl/P3x68V">timeline</a> and
        /// <a href= "http://goo.gl/7kNQSb">events</a> endpoints represent the
        /// results of this part of the search.
        /// </remarks>
        /// <value>
        /// The event search.
        /// </value>
        string EventSearch
        { get; }

        /// <summary>
        /// Gets the <see cref="SortDirection"/> of the current search job.
        /// </summary>
        /// <value>
        /// The event sorting.
        /// </value>
        SortDirection EventSorting
        { get; }

        /// <summary>
        /// Gets the index earliest time.
        /// </summary>
        /// <value>
        /// The index earliest time.
        /// </value>
        long IndexEarliestTime
        { get; }

        /// <summary>
        /// Gets the index latest time.
        /// </summary>
        /// <value>
        /// The index latest time.
        /// </value>
        long IndexLatestTime
        { get; }

        /// <summary>
        /// Gets a value indicating whether this object is batch mode search.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is batch mode search, <c>false</c> if not.
        /// </value>
        bool IsBatchModeSearch
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current search job.
        /// has completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is done, <c>false</c> if not.
        /// </value>
        bool IsDone
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current search job failed
        /// due to a fatal error.
        /// </summary>
        /// <remarks>
        /// A search job may fail, for example, because of syntax errors in the
        /// search command.
        /// </remarks>
        /// <value>
        /// <c>true</c> if this object is failed, <c>false</c> if not.
        /// </value>
        bool IsFailed
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current search job was
        /// finalized before completion.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is finalized, <c>false</c> if not.
        /// </value>
        bool IsFinalized
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current search job is
        /// paused.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is paused, <c>false</c> if not.
        /// </value>
        bool IsPaused
        { get; }

        /// <summary>
        /// Gets a value that indicates whether previews are enabled for the 
        /// current search job.
        /// </summary>
        /// <value>
        /// <c>true</c> if a preview is enabled, <c>false</c> if not.
        /// </value>
        bool IsPreviewEnabled
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current search job is
        /// executing a realtime search.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is real time search, <c>false</c> if not.
        /// </value>
        bool IsRealTimeSearch
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the timeline feature is enabled 
        /// for the current search job.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is remote timeline, <c>false</c> if not.
        /// </value>
        bool IsRemoteTimeline
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current search job is saved.
        /// </summary>
        /// <remarks>
        /// A value of <c>true</c> indicates that the current <see cref="Job"/>
        /// is saved, storing search artifacts on disk for 7 days from the last time
        /// that the job was viewed or otherwise touched. Set the value of
        /// <c>default_save_ttl</c> <a href="http://goo.gl/OpE4lR">
        /// limits.conf</a> to override the default value.
        /// </remarks>
        /// <value>
        /// <c>true</c> if this object is saved, <c>false</c> if not.
        /// </value>
        bool IsSaved
        { get; }

        /// <summary>
        /// Gets a value that indicates if the current search job is a saved
        /// search that was run by the Splunk scheduler.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is saved search, <c>false</c> if not.
        /// </value>
        bool IsSavedSearch
        { get; }

        /// <summary>
        /// Gets a value that indicates if the process running the current
        /// search job is dead, but with the search not finished.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is zombie, <c>false</c> if not.
        /// </value>
        bool IsZombie
        { get; }

        /// <summary>
        /// Gets the positive keywords used by this search.
        /// </summary>
        /// <remarks>
        /// A positive keyword is a keyword that is not in a <c>NOT</c> clause.
        /// </remarks>
        /// <value>
        /// The keywords.
        /// </value>
        string Keywords
        { get; }

        /// <summary>
        /// Gets the latest time a search job is configured to start.
        /// </summary>
        /// <remarks>
        /// This value can be used to indicate progress. See
        /// <see cref= "DoneProgress"/>.
        /// </remarks>
        /// <value>
        /// The latest time.
        /// </value>
        DateTime LatestTime
        { get; }

        //// TODO: Messages	{System.Dynamic.ExpandoObject}	System.Dynamic.ExpandoObject

        /// <summary>
        /// Gets the normalized search.
        /// </summary>
        /// <value>
        /// The normalized search.
        /// </value>
        string NormalizedSearch
        { get; }

        /// <summary>
        /// Gets the number of previews that have been generated by the current
        /// search job so far.
        /// </summary>
        /// <value>
        /// The total number of previews.
        /// </value>
        int NumPreviews
        { get; }

        /// <summary>
        /// Gets the execution costs of the current search job.
        /// </summary>
        /// <value>
        /// The execution costs of the current search job.
        /// </value>
        dynamic Performance
        { get; }

        /// <summary>
        /// Gets the PID of the current search job.
        /// </summary>
        /// <value>
        /// PID of the current search job.
        /// </value>
        int Pid
        { get; }

        /// <summary>
        /// Gets an integer value between <c>0</c> and <c>10</c> that indicates the
        /// priority of the current search job.
        /// </summary>
        /// <remarks>
        /// The priority is mapped to the OS process priority. The higher the number
        /// the higher the priority. The priority can be changed using
        /// <see cref="SetPriorityAsync"/>.
        /// <note type="note">
        /// On *nix systems, non-privileged users can only reduce the priority of a
        /// process.
        /// </note>
        /// </remarks>
        /// <value>
        /// The priority.
        /// </value>
        int Priority
        { get; }

        /// <summary>
        /// Gets the search string that is sent to every search peer.
        /// </summary>
        /// <value>
        /// The search string that is sent to every search peer.
        /// </value>
        string RemoteSearch
        { get; }

        /// <summary>
        /// Gets the subset of the entire search command that is composed of
        /// reporting commands.
        /// </summary>
        /// <remarks>
        /// A value of <c>indicates</c> that search command has no reporting 
        /// commands.
        /// </remarks>
        /// <value>
        /// The report search.
        /// </value>
        string ReportSearch
        { get; }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>
        /// The request.
        /// </value>
        dynamic Request
        { get; }

        /// <summary>
        /// Gets the total number of results returned by the search.
        /// </summary>
        /// <remarks>
        /// This is the subset of scanned events (represented by the
        /// <see cref= "ScanCount"/> property) that actually matches the search
        /// terms.
        /// </remarks>
        /// <value>
        /// The total number of results returned by the search.
        /// </value>
        long ResultCount
        { get; }

        /// <summary>
        /// Gets a value that indicates if the final results of the search are
        /// available using streaming.
        /// </summary>
        /// <value>
        /// <c>true</c> if result is streaming, <c>false</c> if not.
        /// </value>
        bool ResultIsStreaming
        { get; }

        /// <summary>
        /// Gets the number of result rows in the latest preview results.
        /// </summary>
        /// <value>
        /// The number of result previews.
        /// </value>
        long ResultPreviewCount
        { get; }

        /// <summary>
        /// Gets the time in seconds that the current search job took to
        /// complete.
        /// </summary>
        /// <value>
        /// The run duration.
        /// </value>
        double RunDuration
        { get; }

        /// <summary>
        /// Gets the runtime.
        /// </summary>
        /// <value>
        /// The runtime.
        /// </value>
        Job.RuntimeAdapter Runtime
        { get; }

        /// <summary>
        /// Gets the number of events that are scanned or read from disk.
        /// </summary>
        /// <value>
        /// The number of scans.
        /// </value>
        long ScanCount
        { get; }

        /// <summary>
        /// Gets the full text of the search command for the current search job.
        /// </summary>
        /// <value>
        /// The full text of the search command for the current search job.
        /// </value>
        string Search
        { get; }

        /// <summary>
        /// Gets the earliest time for the current search job as specified in
        /// the search command rather than the <see cref= "EarliestTime"/>
        /// parameter.
        /// </summary>
        /// <remarks>
        /// This value does not snap to the indexed data time bounds for 
        /// all-time searches as <see cref="EarliestTime"/> and <see cref=
        /// "LatestTime"/> do.
        /// </remarks>
        /// <value>
        /// The search earliest time.
        /// </value>
        DateTime SearchEarliestTime
        { get; }

        /// <summary>
        /// Gets the latest time for the current search job as specified in the
        /// search command rather than the <see cref="LatestTime"/> parameter.
        /// </summary>
        /// <remarks>
        /// This value does not snap to the indexed data time bounds for 
        /// all-time searches as <see cref="EarliestTime"/> and <see cref=
        /// "LatestTime"/> do.
        /// </remarks>
        /// <value>
        /// The search latest time.
        /// </value>
        DateTime SearchLatestTime
        { get; }

        /// <summary>
        /// Gets the list of all search peers that were contacted by the current
        /// search job.
        /// </summary>
        /// <value>
        /// The search providers.
        /// </value>
        ReadOnlyCollection<string> SearchProviders
        { get; }

        /// <summary>
        /// Gets the ID of the current search job.
        /// </summary>
        /// <remarks>
        /// This property is a synonym for the job name.
        /// </remarks>
        /// <value>
        /// The search ID of the current <see cref="Job"/>.
        /// </value>
        string Sid
        { get; }

        /// <summary>
        /// Gets the maximum number of time line buckets.
        /// </summary>
        /// <value>
        /// The status buckets.
        /// </value>
        int StatusBuckets
        { get; }

        /// <summary>
        /// Gets the time in seconds before the current search job expires 
        /// after it completes.
        /// </summary>
        /// <value>
        /// The time in seconds before the current search job expires after it
        /// completes.
        /// </value>
        long Ttl
        { get; }

        #endregion

        #region Methods

        #region Getting, removing, and updating the current Job

        /// <summary>
        /// Asynchronously retrieves a fresh copy of the current search hob
        /// that is in or has moved past a desired <see cref= "DispatchState"/>
        /// and contains all changes to it since it was last retrieved.
        /// </summary>
        /// <remarks>
        /// This method always retrieves a fresh copy of the current
        /// <see cref= "Job"/>.
        /// </remarks>
        /// <param name="dispatchState">
        /// Desired dispatch state.
        /// </param>
        /// <param name="delay">
        /// Number of milliseconds to wait for the current <see cref="Job"/> to move
        /// into the desired <paramref name="dispatchState"/>.
        /// </param>
        /// <param name="retryInterval">
        /// Number of milliseconds to wait between checks for the dispatch state of
        /// the current <see cref="Job"/>. This value is increased by 50% on each
        /// retry.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task GetAsync(DispatchState dispatchState, int delay, int retryInterval);

        /// <summary>
        /// Asynchronously retrieves a fresh copy of the current search job
        /// that is in or has moved past a desired <see cref= "DispatchState"/>
        /// and contains all changes to it since it was last retrieved.
        /// </summary>
        /// <remarks>
        /// This method returns immediately if <see cref="DispatchState"/> is 
        /// greater than or equal to <paramref name="dispatchState"/>.
        /// </remarks>
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
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task TransitionAsync(DispatchState dispatchState, int delay, int retryInterval);

        /// <summary>
        /// Updates custom arguments to the current <see cref="Job"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/bL4tFk">POST
        /// search/jobs/{search_id}</a> endpoint to update custom arguments to
        /// the current <see cref="Job"/>.
        /// </remarks>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// <c>true</c> if the current snapshot was also updated.
        /// </returns>
        Task<bool> UpdateAsync(CustomJobArgs arguments);

        #endregion

        #region Retrieving search results

        /// <summary>
        /// Gets search events asynchronous.
        /// </summary>
        /// <param name="args">
        /// Optional search event arguments.
        /// </param>
        /// <returns>
        /// The search events asynchronous.
        /// </returns>
        Task<SearchResultStream> GetSearchEventsAsync(SearchEventArgs args);

        /// <summary>
        /// Gets search events asynchronous.
        /// </summary>
        /// <param name="count">
        /// Optional search results count, defaults to 0 for all results.
        /// </param>
        /// <returns>
        /// The search events asynchronous.
        /// </returns>
        Task<SearchResultStream> GetSearchEventsAsync(int count = 0);

        /// <summary>
        /// Gets search preview asynchronous.
        /// </summary>
        /// <param name="args">
        /// Optional search result arguments.
        /// </param>
        /// <returns>
        /// The search preview asynchronous.
        /// </returns>
        Task<SearchResultStream> GetSearchPreviewAsync(SearchResultArgs args);

        /// <summary>
        /// Gets search preview asynchronous.
        /// </summary>
        /// <param name="count">
        /// Optional search events count, defaults to 0 for all events.
        /// </param>
        /// <returns>
        /// The search preview asynchronous.
        /// </returns>
        Task<SearchResultStream> GetSearchPreviewAsync(int count = 0);

        /// <summary>
        /// Gets search results asynchronous.
        /// </summary>
        /// <param name="args">
        /// Optional search result arguments.
        /// </param>
        /// <returns>
        /// The search results asynchronous.
        /// </returns>
        Task<SearchResultStream> GetSearchResultsAsync(SearchResultArgs args);

        /// <summary>
        /// Gets search results asynchronous.
        /// </summary>
        /// <param name="count">
        /// Optional search results count, defaults to 0 for all results.
        /// </param>
        /// <returns>
        /// The search results asynchronous.
        /// </returns>
        Task<SearchResultStream> GetSearchResultsAsync(int count = 0);

        #endregion

        #region Job control

        /// <summary>
        /// Asynchronously stops the current search and deletes the result 
        /// cache.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task CancelAsync();

        /// <summary>
        /// Asynchronously disables preview generation for the current search
        /// job.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task DisablePreviewAsync();

        /// <summary>
        /// Asynchronously enables preview generation for the current search
        /// job.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task EnablePreviewAsync();

        /// <summary>
        /// Asynchronously stops the current search job and provides intermediate
        /// results to its results endpoint.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task FinalizeAsync();

        /// <summary>
        /// Asynchronously suspends execution of the current search job.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task PauseAsync();

        /// <summary>
        /// Asynchronously saves the current search job, storing its artifacts
        /// on disk for seven days.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// Add or edit the <c>default_save_ttl</c> value in <a href=
        /// "http://goo.gl/yj4XrO">limits.conf</a> to override the default
        /// value of seven days.
        /// </remarks>
        Task SaveAsync();

        /// <summary>
        /// Sets the priority of the search process.
        /// </summary>
        /// <param name="priority">
        /// A value between <c>0</c> and <c>10</c>.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task SetPriorityAsync(int priority);

        /// <summary>
        /// Sets the time-to-live for the results of the current search job.
        /// </summary>
        /// <param name="ttl">
        /// The time-to-live in seconds.
        /// seconds.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task SetTtlAsync(int ttl);

        /// <summary>
        /// Extends the expiration time for the results of the current search
        /// job.
        /// </summary>
        /// <param name="ttl">
        /// The extended time in seconds.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task TouchAsync(int ttl);

        /// <summary>
        /// Asynchronously resumes execution of the current search job, if
        /// it is paused.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task UnpauseAsync();

        /// <summary>
        /// Asynchronously disables any action performed by <see cref="SaveAsync"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task UnsaveAsync();

        #endregion

        #endregion
    }

    [ContractClassFor(typeof(IJob))]
    abstract class IJobContract : IJob
    {
        #region Properties

        public abstract bool CanSummarize { get; }
        public abstract DateTime CursorTime { get; }
        public abstract int DefaultSaveTtl { get; }
        public abstract int DefaultTtl { get; }
        public abstract long DiskUsage { get; }
        public abstract DispatchState DispatchState { get; }
        public abstract double DoneProgress { get; }
        public abstract long DropCount { get; }
        public abstract Eai Eai { get; }
        public abstract DateTime EarliestTime { get; }
        public abstract long EventAvailableCount { get; }
        public abstract long EventCount { get; }
        public abstract int EventFieldCount { get; }
        public abstract bool EventIsStreaming { get; }
        public abstract bool EventIsTruncated { get; }
        public abstract string EventSearch { get; }
        public abstract SortDirection EventSorting { get; }
        public abstract long IndexEarliestTime { get; }
        public abstract long IndexLatestTime { get; }
        public abstract bool IsBatchModeSearch { get; }
        public abstract bool IsDone { get; }
        public abstract bool IsFailed { get; }
        public abstract bool IsFinalized { get; }
        public abstract bool IsPaused { get; }
        public abstract bool IsPreviewEnabled { get; }
        public abstract bool IsRealTimeSearch { get; }
        public abstract bool IsRemoteTimeline { get; }
        public abstract bool IsSaved { get; }
        public abstract bool IsSavedSearch { get; }
        public abstract bool IsZombie { get; }
        public abstract string Keywords { get; }
        public abstract DateTime LatestTime { get; }
        public abstract string NormalizedSearch { get; }
        public abstract int NumPreviews { get; }
        public abstract dynamic Performance { get; }
        public abstract int Pid { get; }
        public abstract int Priority { get; }
        public abstract string RemoteSearch { get; }
        public abstract string ReportSearch { get; }
        public abstract dynamic Request { get; }
        public abstract long ResultCount { get; }
        public abstract bool ResultIsStreaming { get; }
        public abstract long ResultPreviewCount { get; }
        public abstract double RunDuration { get; }
        public abstract Job.RuntimeAdapter Runtime { get; }
        public abstract long ScanCount { get; }
        public abstract string Search { get; }
        public abstract DateTime SearchEarliestTime { get; }
        public abstract DateTime SearchLatestTime { get; }
        public abstract ReadOnlyCollection<string> SearchProviders { get; }
        public abstract string Sid { get; }
        public abstract int StatusBuckets { get; }
        public abstract long Ttl { get; }

        #endregion

        #region Methods

        public abstract Task GetAsync(DispatchState dispatchState, int delay, int retryInterval);

        public abstract Task<bool> SendAsync(HttpMethod method, string action, params Argument[] arguments);

        public abstract Task TransitionAsync(DispatchState dispatchState, int delay, int retryInterval);

        public Task<bool> UpdateAsync(CustomJobArgs arguments)
        {
            Contract.Requires(arguments != null);
            return default(Task<bool>);
        }

        public abstract Task<SearchResultStream> GetSearchEventsAsync(SearchEventArgs args);

        public abstract Task<SearchResultStream> GetSearchEventsAsync(int count = 0);

        public abstract Task<SearchResultStream> GetSearchPreviewAsync(SearchResultArgs args);

        public abstract Task<SearchResultStream> GetSearchPreviewAsync(int count = 0);

        public abstract Task<SearchResultStream> GetSearchResultsAsync(SearchResultArgs args);

        public abstract Task<SearchResultStream> GetSearchResultsAsync(int count = 0);

        public abstract Task CancelAsync();

        public abstract Task DisablePreviewAsync();

        public abstract Task EnablePreviewAsync();

        public abstract Task FinalizeAsync();

        public abstract Task<bool> InvokeAsync(string action);

        public abstract Task PauseAsync();

        public abstract Task SaveAsync();

        public abstract Task SetPriorityAsync(int priority);

        public abstract Task SetTtlAsync(int ttl);

        public abstract Task TouchAsync(int ttl);

        public abstract Task UnpauseAsync();

        public abstract Task UnsaveAsync();

        public abstract Task GetAsync();

        public abstract Task RemoveAsync();

        public abstract Task<bool> UpdateAsync(params Argument[] arguments);

        public abstract Task<bool> UpdateAsync(System.Collections.Generic.IEnumerable<Argument> arguments);

        #endregion
    }
}
