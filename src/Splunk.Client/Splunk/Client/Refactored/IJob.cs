namespace Splunk.Client.Refactored
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClass(typeof(IJobContract))]
    interface IJob : IEntity
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        bool CanSummarize
        { get; }

        /// <summary>
        /// Gets the earliest time from which no events are later scanned.
        /// </summary>
        /// <remarks>
        /// This value can be used to indicate progress. See <see cref=
        /// "DoneProgress"/>. 
        /// </remarks>
        DateTime CursorTime
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int DefaultSaveTTL
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int DefaultTTL
        { get; }

        /// <summary>
        /// The total number of bytes of disk space used by the current <see 
        /// cref="Job"/>.
        /// </summary>
        long DiskUsage
        { get; }

        /// <summary>
        /// Gets the <see cref="DispatchState"/> of the current <see cref="Job"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="DispatchState"/> value.
        /// </returns>
        DispatchState DispatchState
        { get; }

        /// <summary>
        /// Gets a number between 0 and 1.0 that indicates the approximate 
        /// progress of the current <see cref="Job"/>
        /// </summary>
        /// <remarks>
        /// This value is computed as (<see cref="LatestTime"/> - <see cref=
        /// "CursorTime"/>) / (<see cref="LatestTime"/> - <see cref=
        /// "EarliestTime"/>).
        /// </remarks>
        double DoneProgress
        { get; }

        /// <summary>
        /// Gets the number of possible events that were dropped from the 
        /// current <see cref="Job"/> due to the realtime queue size.
        /// </summary>
        /// <remarks>
        /// This value only applies to realtime search jobs.
        /// </remarks>
        long DropCount
        { get; }

        /// <summary>
        /// Gets the access control lists for the current <see cref="Job"/>.
        /// </summary>
        Eai Eai
        { get; }

        /// <summary>
        /// Gets the earliest time the current <see cref="Job"/> is configured
        /// to start.
        /// </summary>
        /// <remarks>
        /// This value can be used to indicate progress. See <see cref=
        /// "DoneProgress"/>.
        /// </remarks>
        DateTime EarliestTime
        { get; }

        /// <summary>
        /// Gets the number of events that are available for export from the 
        /// current <see cref="Job"/>.
        /// </summary>
        long EventAvailableCount
        { get; }

        /// <summary>
        /// Gets the number of events found by the current <see cref="Job"/>.
        /// </summary>
        long EventCount
        { get; }

        /// <summary>
        /// Gets the number of fields found in the search results produced by 
        /// the current <see cref="Job"/>.
        /// </summary>
        int EventFieldCount
        { get; }

        /// <summary>
        /// Gets a value that indicates if the events of the current <see cref=
        /// "Job"/> are being streamed.
        /// </summary>
        bool EventIsStreaming
        { get; }

        /// <summary>
        /// Gets a value that indicates if the events produced by the current 
        /// <see cref="Job"/> have not been stored, and thus are not available 
        /// from the events endpoint for the <see cref="Job"/>.
        /// </summary>
        bool EventIsTruncated
        { get; }

        /// <summary>
        /// Gets that subset of the search command that appears before any 
        /// transforming commands.
        /// </summary>
        /// <remarks>
        /// The <a href="http://goo.gl/P3x68V">timeline</a> and <a href=
        /// "http://goo.gl/7kNQSb">events</a> endpoints represent the results
        /// of this part of the search.
        /// </remarks>
        string EventSearch
        { get; }

        /// <summary>
        /// Gets the <see cref="SortDirection"/> of the current <see cref="Job"/>.
        /// </summary>
        SortDirection EventSorting
        { get; }

        /// <summary>
        /// 
        /// </summary>
        long IndexEarliestTime
        { get; }

        /// <summary>
        /// 
        /// </summary>
        long IndexLatestTime
        { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsBatchModeSearch
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref="Job"/>
        /// has completed.
        /// </summary>
        bool IsDone
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref="Job"/>
        /// failed due to a fatal error.
        /// </summary>
        /// <remarks>
        /// A <see cref="Job"/> may fail, for example, because of syntax errors
        /// in the search command.
        /// </remarks>
        bool IsFailed
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref="Job"/>
        /// was finalized before completion.
        /// </summary>
        bool IsFinalized
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref="Job"/>
        /// is paused.
        /// </summary>
        bool IsPaused
        { get; }

        /// <summary>
        /// Gets a value that indicates whether previews are enabled for the 
        /// current <see cref="Job"/>.
        /// </summary>
        bool IsPreviewEnabled
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref="Job"/>
        /// is executing a realtime search.
        /// </summary>
        bool IsRealTimeSearch
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the timeline feature is enabled
        /// for the current <see cref="Job"/>.
        /// </summary>
        bool IsRemoteTimeline
        { get; }

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
        bool IsSaved
        { get; }

        /// <summary>
        /// Gets a value that indicates if the current <see cref="Job"/> is a 
        /// saved search that was run by the Splunk scheduler.
        /// </summary>
        bool IsSavedSearch
        { get; }

        /// <summary>
        /// Gets a value that indicates if the process running the current <see 
        /// cref="Job"/> is dead, but with the search not finished.
        /// </summary>
        bool IsZombie
        { get; }

        /// <summary>
        /// Gets the positive keywords used by this search.
        /// </summary>
        /// <remarks>
        /// A positive keyword is a keyword that is not in a <c>NOT</c> clause.
        /// </remarks>
        string Keywords
        { get; }

        /// <summary>
        /// Gets the latest time a search job is configured to start.
        /// </summary>
        /// <remarks>
        /// This value can be used to indicate progress. See <see cref=
        /// "DoneProgress"/>.
        /// </remarks>
        DateTime LatestTime
        { get; }

        //// TODO: Messages	{System.Dynamic.ExpandoObject}	System.Dynamic.ExpandoObject

        /// <summary>
        /// 
        /// </summary>
        string NormalizedSearch
        { get; }

        /// <summary>
        /// Gets the number of previews that have been generated by the current
        /// <see cref="Job"/> so far.
        /// </summary>
        int NumPreviews
        { get; }

        /// <summary>
        /// Gets the executions costs of the current <see cref="Job"/>.
        /// </summary>
        dynamic Performance
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int Pid
        { get; }

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
        int Priority
        { get; }

        /// <summary>
        /// Gets the search string that is sent to every search peer.
        /// </summary>
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
        string ReportSearch
        { get; }

        /// <summary>
        /// 
        /// </summary>
        dynamic Request
        { get; }

        /// <summary>
        /// Gets the total number of results returned by the search.
        /// </summary>
        /// <remarks>
        /// This is the subset of scanned events (represented by the <see cref=
        /// "ScanCount"/> property) that actually matches the search terms.
        /// </remarks>
        long ResultCount
        { get; }

        /// <summary>
        /// Gets a value that indicates if the final results of the search are
        /// available using streaming.
        /// </summary>
        bool ResultIsStreaming
        { get; }

        /// <summary>
        /// Gets the number of result rows in the latest preview results.
        /// </summary>
        long ResultPreviewCount
        { get; }

        /// <summary>
        /// Gets the time in seconds that the current <see cref="Job"/> took
        /// to complete.
        /// </summary>
        double RunDuration
        { get; }

        /// <summary>
        /// 
        /// </summary>
        Job.Runtime_t Runtime
        { get; }

        /// <summary>
        /// Gets the number of events that are scanned or read from disk.
        /// </summary>
        long ScanCount
        { get; }

        /// <summary>
        /// Gets the full text of the search command for the current <see cref=
        /// "Job"/>.
        /// </summary>
        string Search
        { get; }

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
        DateTime SearchEarliestTime
        { get; }

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
        DateTime SearchLatestTime
        { get; }

        /// <summary>
        /// Gets the list of all search peers that were contacted by the current
        /// <see cref="Job"/>
        /// </summary>
        IReadOnlyList<string> SearchProviders
        { get; }

        /// <summary>
        /// Gets the search ID for the current <see cref="Job"/>.
        /// </summary>
        /// <remarks>
        /// This property is a synonym for <see cref="Resource&lt;TResource&gt;.Name"/>.
        /// </remarks>
        string Sid
        { get; }

        /// <summary>
        /// Gets the maximum number of timeline buckets.
        /// </summary>
        int StatusBuckets
        { get; }

        /// <summary>
        /// Gets the time in seconds before the current <see cref="Job"/> expires
        /// after it completes.
        /// </summary>
        long Ttl
        { get; }

        #endregion

        #region Methods

        #region Getting, removing, and updating the current Job

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
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// This method always retrieves a fresh copy of the current <see cref=
        /// "Job"/>.
        /// </remarks>
        Task GetAsync(DispatchState dispatchState, int delay, int retryInterval);

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
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// This method returns immediately if <see cref="DispatchState"/> is 
        /// greater than or equal to <paramref name="dispatchState"/>.
        /// </remarks>
        Task TransitionAsync(DispatchState dispatchState, int delay, int retryInterval);

        /// <summary>
        /// Updates custom arguments to the current <see cref="Job"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/bL4tFk">POST 
        /// search/jobs/{search_id}</a> endpoint to update custom arguments to
        /// the current <see cref="Job"/>.
        /// </remarks>
        Task UpdateAsync(CustomJobArgs arguments);

        #endregion

        #region Retrieving search results

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<SearchResultStream> GetSearchResultsAsync(SearchResultArgs args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<SearchResultStream> GetSearchResultsEventsAsync(SearchEventArgs args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<SearchResultStream> GetSearchResultsPreviewAsync(SearchResultArgs args);

        #endregion

        #region Job control

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task CancelAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task DisablePreviewAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task EnablePreviewAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task FinalizeAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task PauseAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task SaveAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        Task SetPriorityAsync(int priority);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ttl"></param>
        /// <returns></returns>
        Task SetTtlAsync(int ttl);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ttl"></param>
        /// <returns></returns>
        Task TouchAsync(int ttl);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task UnpauseAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        public abstract int DefaultSaveTTL { get; }
        public abstract int DefaultTTL { get; }
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
        public abstract Job.Runtime_t Runtime { get; }
        public abstract long ScanCount { get; }
        public abstract string Search { get; }
        public abstract DateTime SearchEarliestTime { get; }
        public abstract DateTime SearchLatestTime { get; }
        public abstract IReadOnlyList<string> SearchProviders { get; }
        public abstract string Sid { get; }
        public abstract int StatusBuckets { get; }
        public abstract long Ttl { get; }

        #endregion

        #region Methods

        public abstract Task GetAsync(DispatchState dispatchState, int delay, int retryInterval);

        public abstract Task TransitionAsync(DispatchState dispatchState, int delay, int retryInterval);

        public Task UpdateAsync(CustomJobArgs arguments)
        {
            Contract.Requires(arguments != null);
            return default(Task);
        }

        public abstract Task<SearchResultStream> GetSearchResultsAsync(SearchResultArgs args);

        public abstract Task<SearchResultStream> GetSearchResultsEventsAsync(SearchEventArgs args);

        public abstract Task<SearchResultStream> GetSearchResultsPreviewAsync(SearchResultArgs args);

        public abstract Task CancelAsync();

        public abstract Task DisablePreviewAsync();

        public abstract Task EnablePreviewAsync();

        public abstract Task FinalizeAsync();

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
