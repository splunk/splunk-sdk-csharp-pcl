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
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Provids an operational interface to Splunk saved search entities.
    /// </summary>
    /// <seealso cref="T:IEntity"/>
    [ContractClass(typeof(ISavedSearchContract))]
    public interface ISavedSearch : IEntity
    {
        #region Properties

        /// <summary>
        /// Gets the actions.
        /// </summary>
        /// <value>
        /// The actions.
        /// </value>
        SavedSearch.ActionsAdapter Actions
        { get; }

        /// <summary>
        /// Gets the alert.
        /// </summary>
        /// <value>
        /// The alert.
        /// </value>
        SavedSearch.AlertAdapter Alert
        { get; }

        /// <summary>
        /// Gets the automatic summarize.
        /// </summary>
        /// <value>
        /// The automatic summarize.
        /// </value>
        SavedSearch.AutoSummarizeAdapter AutoSummarize
        { get; }

        /// <summary>
        /// Gets the Cron schedule to execute the current <see cref= "SavedSearch"/>.
        /// </summary>
        /// <value>
        /// The cron schedule.
        /// </value>
        string CronSchedule
        { get; }

        /// <summary>
        /// Gets the human-readable description of the current
        /// <see cref= "SavedSearch"/>.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        string Description
        { get; }

        /// <summary>
        /// Gets the dispatch.
        /// </summary>
        /// <value>
        /// The dispatch.
        /// </value>
        SavedSearch.DispatchAdapter Dispatch
        { get; }

        /// <summary>
        /// Gets the display.
        /// </summary>
        /// <value>
        /// The display.
        /// </value>
        SavedSearch.DisplayAdapter Display
        { get; }

        /// <summary>
        /// Gets the extensible administration interface properties for the current
        /// <see cref= "SavedSearch"/>.
        /// </summary>
        /// <value>
        /// The extensible administration interface properties.
        /// </value>
        Eai Eai
        { get; }

        /// <summary>
        /// Gets a value that indicates if the current <see cref="SavedSearch"/>
        /// is disabled.
        /// </summary>
        /// <remarks>
        /// Disabled saved searches are not visible in Splunk Web.
        /// </remarks>
        /// <value>
        /// <c>true</c> if this object is disabled, <c>false</c> if not.
        /// </value>
        bool IsDisabled
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current
        /// <see cref= "SavedSearch"/> runs on a schedule.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is scheduled, <c>false</c> if not.
        /// </value>
        bool IsScheduled
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current
        /// <see cref= "SavedSearch"/> appears in the list of saved searches on
        /// Splunk Web.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is visible; otherwise, <c>false</c>.
        /// </value>
        bool IsVisible
        { get; }

        /// <summary>
        /// Gets the maximum concurrent.
        /// </summary>
        /// <value>
        /// The maximum concurrent.
        /// </value>
        int MaxConcurrent
        { get; }

        /// <summary>
        /// Gets the next scheduled time.
        /// </summary>
        /// <value>
        /// The next scheduled time.
        /// </value>
        DateTime NextScheduledTime
        { get; }

        /// <summary>
        /// Gets the qualified search.
        /// </summary>
        /// <value>
        /// The qualified search.
        /// </value>
        string QualifiedSearch
        { get; }

        /// <summary>
        /// Gets a value indicating whether the real time schedule.
        /// </summary>
        /// <value>
        /// <c>true</c> if real time schedule, <c>false</c> if not.
        /// </value>
        bool RealTimeSchedule
        { get; }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>
        /// The request.
        /// </value>
        SavedSearch.RequestAdapter Request
        { get; }

        /// <summary>
        /// Gets a value indicating whether the restart on search peer add.
        /// </summary>
        /// <value>
        /// <c>true</c> if restart on search peer add, <c>false</c> if not.
        /// </value>
        bool RestartOnSearchPeerAdd
        { get; }

        /// <summary>
        /// Gets a value indicating whether the run on startup.
        /// </summary>
        /// <value>
        /// <c>true</c> if run on startup, <c>false</c> if not.
        /// </value>
        bool RunOnStartup
        { get; }

        /// <summary>
        /// Gets a list of times of the scheduled.
        /// </summary>
        /// <value>
        /// A list of times of the scheduled.
        /// </value>
        ReadOnlyCollection<DateTime> ScheduledTimes
        { get; }

        /// <summary>
        /// Gets the search command for the current <see cref="SavedSearch"/>.
        /// </summary>
        /// <value>
        /// The search.
        /// </value>
        string Search
        { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Dispatches the current <see cref="SavedSearch"/> just like the scheduler
        /// would.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/AfzBJO">POST
        /// saved/searches/{name}/dispatch</a> endpoint to dispatch the current
        /// <see cref="SavedSearch"/>. It then uses the
        /// <a href= "http://goo.gl/C9qc98">GET search/jobs/{search_id}</a> endpoint
        /// to retrieve the <see cref="Job"/> object that it returns.
        /// </remarks>
        /// <param name="dispatchArgs">
        /// A set of arguments to the dispatcher.
        /// </param>
        /// <param name="templateArgs">
        /// A set of template arguments to the saved search.
        /// </param>
        /// <returns>
        /// An object representing the search job created by the dispatcher.
        /// </returns>
        Task<Job> DispatchAsync(SavedSearchDispatchArgs dispatchArgs = null, SavedSearchTemplateArgs templateArgs = null);

        /// <summary>
        /// Asynchronously retrieves information about the current
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/L4JLwn">GET
        /// saved/searches/{name}</a> endpoint to get information about the current
        /// <see cref="SavedSearch"/> instance.
        /// </remarks>
        /// <param name="criteria">
        /// Constrains the information returned about the current instance.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task GetAsync(SavedSearch.Filter criteria);

        /// <summary>
        /// Asynchronously retrieves the collection of jobs created from the current
        /// instance.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/kv9L1l">GET
        /// saved/searches/{name}/history</a> endpoint to construct the
        /// <see cref="JobCollection"/> object it returns.
        /// </remarks>
        /// <returns>
        /// An object representing the collection of jobs created from the current
        /// instance.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification =
            "This is by design")
        ]
        Task<JobCollection> GetHistoryAsync();

        /// <summary>
        /// Asynchronously retrieves the scheduled times for the current
        /// <see cref="SavedSearch"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/jFifZ7">GET
        /// saved/searches/{name}/scheduled_times</a> endpoint to retrieve the
        /// scheduled times it returns.
        /// </remarks>
        /// <param name="earliestTime">
        /// An absolute or relative time string specifying a lower-bound for the
        /// scheduled time range.
        /// </param>
        /// <param name="latestTime">
        /// An absolute or relative time string specifying a upper-bound for the
        /// scheduled time range.
        /// </param>
        /// <returns>
        /// The scheduled times asynchronous.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification =
            "This is by design")
        ]
        Task<ReadOnlyCollection<DateTime>> GetScheduledTimesAsync(string earliestTime, string latestTime);

        /// <summary>
        /// Asynchronously reschedules the saved search represented by the current
        /// instance.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/SJcEr5">POST
        /// saved/searches/{name}/reschedule</a> endpoint to reschedule the saved
        /// search represented by the current instance.
        /// </remarks>
        /// <param name="scheduleTime">
        /// A time string specifying the next time to run the search. This value
        /// defaults to null indicating that the saved search should be run as soon
        /// as possible.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task ScheduleAsync(DateTime? scheduleTime = null);

        /// <summary>
        /// Asynchronously updates the saved search represented by the current
        /// instance.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/aV9eiZ">POST
        /// saved/searches{name}</a> endpoint to update the saved search represented
        /// by the current instance.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// <paramref name="attributes"/>, <paramref name="dispatchArgs"/>, and
        /// <paramref name="templateArgs"/> are <c>null</c>.
        /// </exception>
        /// <param name="search">
        /// The search command for the current <see cref="SavedSearch"/>.
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
        /// <returns>
        /// <c>true</c> if the current snapshot was also updated.
        /// </returns>
        Task<bool> UpdateAsync(string search = null, SavedSearchAttributes attributes = null, 
            SavedSearchDispatchArgs dispatchArgs = null, SavedSearchTemplateArgs templateArgs = null);

        #endregion
    }

    [ContractClassFor(typeof(ISavedSearch))]
    abstract class ISavedSearchContract : ISavedSearch
    {
        #region Properties

        public abstract SavedSearch.ActionsAdapter Actions { get; }
        public abstract SavedSearch.AlertAdapter Alert { get; }
        public abstract SavedSearch.AutoSummarizeAdapter AutoSummarize { get; }
        public abstract string CronSchedule { get; }
        public abstract string Description { get; }
        public abstract SavedSearch.DispatchAdapter Dispatch { get; }
        public abstract SavedSearch.DisplayAdapter Display { get; }
        public abstract Eai Eai { get; }
        public abstract bool IsDisabled { get; }
        public abstract bool IsScheduled { get; }
        public abstract bool IsVisible { get; }
        public abstract int MaxConcurrent { get; }
        public abstract DateTime NextScheduledTime { get; }
        public abstract string QualifiedSearch { get; }
        public abstract bool RealTimeSchedule { get; }
        public abstract SavedSearch.RequestAdapter Request { get; }
        public abstract bool RestartOnSearchPeerAdd { get; }
        public abstract bool RunOnStartup { get; }
        public abstract ReadOnlyCollection<DateTime> ScheduledTimes { get; }
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

        public Task<ReadOnlyCollection<DateTime>> GetScheduledTimesAsync(string earliestTime, string latestTime) 
        {
            Contract.Requires<ArgumentNullException>(earliestTime != null);
            Contract.Requires<ArgumentNullException>(latestTime != null);
            return default(Task<ReadOnlyCollection<DateTime>>);
        }

        public abstract Task ScheduleAsync(DateTime? scheduleTime = null);

        public abstract Task GetAsync();

        public abstract Task<bool> InvokeAsync(string action);

        public abstract Task RemoveAsync();

        public abstract Task<bool> SendAsync(HttpMethod method, string action, params Argument[] arguments);

        public abstract Task<bool> UpdateAsync(params Argument[] arguments);

        public abstract Task<bool> UpdateAsync(IEnumerable<Argument> arguments);

        public Task<bool> UpdateAsync(string search = null, SavedSearchAttributes attributes = null, 
            SavedSearchDispatchArgs dispatchArgs = null, SavedSearchTemplateArgs templateArgs = null)
        {
            Contract.Requires<ArgumentException>(!(search == null && attributes == null && dispatchArgs == null && templateArgs == null));
            return default(Task<bool>);
        }

        #endregion
    }
}