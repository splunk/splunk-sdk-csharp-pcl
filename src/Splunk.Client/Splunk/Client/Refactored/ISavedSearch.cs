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

namespace Splunk.Client.Refactored
{
    using Splunk.Client;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Provids an operational interface to Splunk saved search entities. 
    /// </summary>
    [ContractClass(typeof(ISavedSearchContract))]
    public interface ISavedSearch
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        SavedSearch.Action_t Actions
        { get; }

        /// <summary>
        /// 
        /// </summary>
        SavedSearch.Alert_t Alert
        { get; }

        /// <summary>
        /// 
        /// </summary>
        SavedSearch.AutoSummarize_t AutoSummarize
        { get; }

        /// <summary>
        /// Gets or sets the cron schedule to execute the current <see cref=
        /// "SavedSearch"/>.
        /// </summary>
        string CronSchedule
        { get; }

        /// <summary>
        /// Gets the human-readable description of the current <see cref=
        /// "SavedSearch"/>.
        /// </summary>
        string Description
        { get; }

        /// <summary>
        /// 
        /// </summary>
        SavedSearch.Dispatch_t Dispatch
        { get; }

        /// <summary>
        /// 
        /// </summary>
        SavedSearch.Display_t Display
        { get; }

        /// <summary>
        /// Gets the access control lists for the current <see cref=
        /// "SavedSearch"/>.
        /// </summary>
        Eai Eai
        { get; }

        /// <summary>
        /// Gets a value that indicates if the current <see cref="SavedSearch"/>
        /// is disabled.
        /// </summary>
        /// <remarks>
        /// Disabled saved searches are not visible in Splunk Web.
        /// </remarks>
        bool IsDisabled
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref=
        /// "SavedSearch"/> runs on a schedule.
        /// </summary>
        bool IsScheduled
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref=
        /// "SavedSearch"/> appears in the list of saved searches on Splunk
        /// Web.
        /// </summary>
        bool IsVisible
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int MaxConcurrent
        { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTime NextScheduledTime
        { get; }

        /// <summary>
        /// 
        /// </summary>
        string QualifiedSearch
        { get; }

        /// <summary>
        /// 
        /// </summary>
        bool RealtimeSchedule
        { get; }

        /// <summary>
        /// 
        /// </summary>
        SavedSearch.Request_t Request
        { get; }

        /// <summary>
        /// 
        /// </summary>
        bool RestartOnSearchPeerAdd
        { get; }

        /// <summary>
        /// 
        /// </summary>
        bool RunOnStartup
        { get; }

        /// <summary>
        /// 
        /// </summary>
        IReadOnlyList<DateTime> ScheduledTimes
        { get; }

        /// <summary>
        /// Gets the search command for the current <see cref="SavedSearch"/>.
        /// </summary>
        string Search
        { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Dispatches the current <see cref="SavedSearch"/> just like the 
        /// scheduler would.
        /// </summary>
        /// <param name="dispatchArgs">
        /// A set of arguments to the dispatcher.
        /// </param>
        /// <param name="templateArgs">
        /// A set of template arguments to the saved search.
        /// </param>
        /// <returns>
        /// An object representing the search job created by the dispatcher.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/AfzBJO">POST 
        /// saved/searches/{name}/dispatch</a> endpoint to dispatch the 
        /// current <see cref="SavedSearch"/>. It then uses the <a href=
        /// "http://goo.gl/C9qc98">GET search/jobs/{search_id}</a> endpoint to 
        /// retrieve the <see cref="Job"/> object that it returns.
        /// </remarks>
        Task<Job> DispatchAsync(SavedSearchDispatchArgs dispatchArgs = null, SavedSearchTemplateArgs templateArgs = null);

        /// <summary>
        /// Asynchronously retrieves information about the current <see cref="SavedSearch"/>.
        /// </summary>
        /// <param name="criteria">
        /// Constrains the information returned about the current instance.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing this operation.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/L4JLwn">GET 
        /// saved/searches/{name}</a> endpoint to get information about the
        /// current <see cref="SavedSearch"/> instance.
        /// </remarks>
        Task GetAsync(SavedSearch.Filter criteria);

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
        Task<JobCollection> GetHistoryAsync();

        /// <summary>
        /// Asynchronously retrieves the scheduled times for the current
        /// <see cref="SavedSearch"/>.
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
        /// This method uses the <a href="http://goo.gl/jFifZ7">GET 
        /// saved/searches/{name}/scheduled_times</a> endpoint to retrieve
        /// the scheduled times it returns.
        /// </remarks>
        Task<IReadOnlyList<DateTime>> GetScheduledTimesAsync(DateTime earliestTime, DateTime latestTime);

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
        Task ScheduleAsync(DateTime? scheduleTime = null);

        /// <summary>
        /// Asynchronously updates the saved search represented by the current
        /// instance.
        /// </summary>
        /// <param name="attributes">
        /// New attributes for the saved search to be updated.
        /// </param>
        /// <param name="dispatchArgs">
        /// New dispatch arguments for the saved search to be updated.
        /// </param>
        /// <param name="templateArgs">
        /// New template arguments for the saved search to be updated.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="attributes"/>, <paramref name="dispatchArgs"/>, and <paramref 
        /// name="templateArgs"/> are <c>null</c>.
        /// </exception>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/aV9eiZ">POST 
        /// saved/searches{name}</a> endpoint to update the saved search
        /// represented by the current instance.
        /// </remarks>
        Task UpdateAsync(SavedSearchAttributes attributes = null, SavedSearchDispatchArgs dispatchArgs = null, SavedSearchTemplateArgs templateArgs = null);

        #endregion
    }

    [ContractClassFor(typeof(ISavedSearch))]
    abstract class ISavedSearchContract : ISavedSearch
    {
        #region Properties

        public abstract SavedSearch.Action_t Actions { get; }
        public abstract SavedSearch.Alert_t Alert { get; }
        public abstract SavedSearch.AutoSummarize_t AutoSummarize { get; }
        public abstract string CronSchedule { get; }
        public abstract string Description { get; }
        public abstract SavedSearch.Dispatch_t Dispatch { get; }
        public abstract SavedSearch.Display_t Display { get; }
        public abstract Eai Eai { get; }
        public abstract bool IsDisabled { get; }
        public abstract bool IsScheduled { get; }
        public abstract bool IsVisible { get; }
        public abstract int MaxConcurrent { get; }
        public abstract DateTime NextScheduledTime { get; }
        public abstract string QualifiedSearch { get; }
        public abstract bool RealtimeSchedule { get; }
        public abstract SavedSearch.Request_t Request { get; }
        public abstract bool RestartOnSearchPeerAdd { get; }
        public abstract bool RunOnStartup { get; }
        public abstract IReadOnlyList<DateTime> ScheduledTimes { get; }
        public abstract string Search { get; }
        
        #endregion

        #region Methods

        public abstract Task<Job> DispatchAsync(SavedSearchDispatchArgs dispatchArgs = null, 
            SavedSearchTemplateArgs templateArgs = null);

        public Task GetAsync(SavedSearch.Filter criteria)
        {
            Contract.Requires<ArgumentNullException>(criteria != null);
            return default(Task);
        }

        public abstract Task<JobCollection> GetHistoryAsync();

        public abstract Task<IReadOnlyList<DateTime>> GetScheduledTimesAsync(DateTime earliestTime, DateTime latestTime);

        public abstract Task ScheduleAsync(DateTime? scheduleTime = null);

        public Task UpdateAsync(SavedSearchAttributes attributes = null, SavedSearchDispatchArgs dispatchArgs = null, SavedSearchTemplateArgs templateArgs = null)
        {
            Contract.Requires<ArgumentException>(!(attributes == null && dispatchArgs == null && templateArgs == null));
            return default(Task);
        }

        #endregion
    }
}