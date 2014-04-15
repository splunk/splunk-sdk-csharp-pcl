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
// [ ] Parameterize Job.Transition strategy. (It's primitive at present)
//     It's the usual sort of thing:
//     + Linear or non-linear time between retries? 
//     + How long before first retry? 
//     + How many retries?
//     Should we cancel the job, if it times out?
//
// [O] Contracts
//
// [O] Documentation
//
// [ ] Trace messages (e.g., when there are no observers)

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public sealed class Job : Entity<Job>
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
        internal Job(Context context, Namespace @namespace, string name)
            : base(context, @namespace, JobCollection.ClassResourceName, name)
        { }

        /// <summary>
        /// 
        /// </summary>
        public Job()
        { }

        #endregion

        #region Properties

        public bool CanSummarize
        {
            get { return this.Content.GetValue("CanSummarize", BooleanConverter.Instance); }
        }

        public DateTime CursorTime
        {
            get { return this.Content.GetValue("CursorTime", DateTimeConverter.Instance); }
        }

        public int DefaultSaveTTL
        {
            get { return this.Content.GetValue("DefaultSaveTTL", Int32Converter.Instance); }
        }

        public int DefaultTTL
        {
            get { return this.Content.GetValue("DefaultTTL", Int32Converter.Instance); }
        }

        public long DiskUsage
        {
            get { return this.Content.GetValue("DiskUsage", Int64Converter.Instance); } // sample value: "86016"
        }

        public override string Title
        {
            get { return this.Content.GetValue("Sid", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates the current <see cref="Job"/> dispatch
        /// state.
        /// </summary>
        /// <returns>
        /// A <see cref="DispatchState"/> value.
        /// </returns>
        /// <remarks>
        /// Clients that call <see cref="Job.Update"/> to poll for job status
        /// use this property to determine the state of the current search job.
        /// </remarks>
        public DispatchState DispatchState
        {
            get { return this.Content.GetValue("DispatchState", EnumConverter<DispatchState>.Instance); }
        }

        public double DoneProgress
        {
            get { return this.Content.GetValue("DoneProgress", DoubleConverter.Instance); }
        }

        public long DropCount
        {
            get { return this.Content.GetValue("DropCount", Int64Converter.Instance); }
        }

        public Eai Eai
        {
            get { return this.Content.GetValue("Eai", Eai.Converter.Instance); }
        }

        public DateTime EarliestTime
        {
            get { return this.Content.GetValue("EarliestTime", DateTimeConverter.Instance); }
        }

        public long EventAvailableCount
        {
            get { return this.Content.GetValue("EventAvailableCount", Int64Converter.Instance); }
        }

        public long EventCount
        {
            get { return this.Content.GetValue("EventCount", Int64Converter.Instance); }
        }

        public int EventFieldCount
        {
            get { return this.Content.GetValue("EventFieldCount", Int32Converter.Instance); }
        }

        public bool EventIsStreaming
        {
            get { return this.Content.GetValue("EventIsStreaming", BooleanConverter.Instance); } // sample value: "1"
        }

        public bool EventIsTruncated
        {
            get { return this.Content.GetValue("EventIsTruncated", BooleanConverter.Instance); } // sample value: "0"
        }

        public string EventSearch
        {
            get { return this.Content.GetValue("EventSearch", StringConverter.Instance); } // sample value: "search index=_internal  | head 10"
        }

        public SortDirection EventSorting
        {
            get { return this.Content.GetValue("EventSorting", EnumConverter<SortDirection>.Instance); }
        }

        public long IndexEarliestTime
        {
            get { return this.Content.GetValue("IndexEarliestTime", Int64Converter.Instance); } // sample value: "1396566178"
        }

        public long IndexLatestTime
        {
            get { return this.Content.GetValue("IndexLatestTime", Int64Converter.Instance); } // sample value: "1396566183"
        }

        public bool IsBatchModeSearch
        {
            get { return this.Content.GetValue("IsBatchModeSearch", BooleanConverter.Instance); } // sample value: "0"
        }

        public bool IsDone
        {
            get { return this.Content.GetValue("IsDone", BooleanConverter.Instance); } // sample value: "1"
        }

        public bool IsFailed
        {
            get { return this.Content.GetValue("IsFailed", BooleanConverter.Instance); } // sample value: "0"
        }

        public bool IsFinalized
        {
            get { return this.Content.GetValue("IsFinalized", BooleanConverter.Instance); } // sample value: "0"
        }

        public bool IsPaused
        {
            get { return this.Content.GetValue("IsPaused", BooleanConverter.Instance); } // sample value: "0"
        }

        public bool IsPreviewEnabled
        {
            get { return this.Content.GetValue("IsPreviewEnabled", BooleanConverter.Instance); } // sample value: "0"
        }

        public bool IsRealTimeSearch
        {
            get { return this.Content.GetValue("IsRealTimeSearch", BooleanConverter.Instance); } // sample value: "0"
        }

        public bool IsRemoteTimeline
        {
            get { return this.Content.GetValue("IsRemoteTimeline", BooleanConverter.Instance); } // sample value: "0"
        }

        public bool IsSaved
        {
            get { return this.Content.GetValue("IsSaved", BooleanConverter.Instance); } // sample value: "0"
        }

        public bool IsSavedSearch
        {
            get { return this.Content.GetValue("IsSavedSearch", BooleanConverter.Instance); } // sample value: "0"
        }

        public bool IsZombie
        {
            get { return this.Content.GetValue("IsZombie", BooleanConverter.Instance); } // sample value: "0"
        }

        public string Keywords
        {
            get { return this.Content.GetValue("Keywords", StringConverter.Instance); } // sample value: "index::_internal"
        }

        // Messages	{System.Dynamic.ExpandoObject}	System.Dynamic.ExpandoObject

        public string NormalizedSearch
        {
            get { return this.Content.GetValue("NormalizedSearch", StringConverter.Instance); } 
        }

        public int NumPreviews
        {
            get { return this.Content.GetValue("NumPreviews", Int32Converter.Instance); }
        }

        // Performance	{System.Dynamic.ExpandoObject}	System.Dynamic.ExpandoObject

        public int Pid
        {
            get { return this.Content.GetValue("Pid", Int32Converter.Instance); } // sample value: "1692"
        }

        public int Priority
        {
            get { return this.Content.GetValue("Priority", Int32Converter.Instance); } // sample value: "5"
        }

        public string RemoteSearch
        {
            get { return this.Content.GetValue("RemoteSearch", StringConverter.Instance); }
        }

        // Request	{System.Dynamic.ExpandoObject}	System.Dynamic.ExpandoObject

        public long ResultCount
        {
            get { return this.Content.GetValue("ResultCount", Int64Converter.Instance); }
        }

        public bool ResultIsStreaming
        {
            get { return this.Content.GetValue("ResultIsStreaming", BooleanConverter.Instance); }
        }

        public long ResultPreviewCount
        {
            get { return this.Content.GetValue("ResultPreviewCount", Int64Converter.Instance); }
        }

        public double RunDuration
        {
            get { return this.Content.GetValue("RunDuration", DoubleConverter.Instance); } // sample value: "0.220000"
        }

        // Runtime	{System.Dynamic.ExpandoObject}	System.Dynamic.ExpandoObject

        public long ScanCount
        {
            get { return this.Content.GetValue("ScanCount", Int64Converter.Instance); }
        }

        //	SearchProviders	{System.Collections.Generic.List<object>}	System.Collections.Generic.List`1[System.Object]

        public string Sid
        {
            get { return this.Content.GetValue("Sid", StringConverter.Instance); } // sample value: "1396566184.230"
        }

        public int StatusBuckets
        {
            get { return this.Content.GetValue("StatusBuckets", Int32Converter.Instance); } // sample value: "0"
        }

        public long Ttl
        {
            get { return this.Content.GetValue("Ttl", Int64Converter.Instance); } // sample value: "599"
        }

        #endregion

        public async Task RemoveAsync()
        {
            using (var response = await this.Context.DeleteAsync(this.Namespace, this.ResourceName))
            {
                await EnsureStatusCodeAsync(response, HttpStatusCode.OK);
            }
        }

        #region Methods for retrieving search results

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<SearchResults> GetSearchResultsAsync(SearchResultsArgs args = null)
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
        public async Task<SearchResults> GetSearchResultsEventsAsync(SearchEventArgs args = null)
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
        public async Task<SearchResults> GetSearchResultsPreviewAsync(SearchResultsArgs args = null)
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

            await this.PostControlCommandAsync(new Argument[] 
            { 
                new Argument("action", "cancel")
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task DisablePreviewAsync()
        {
            await this.TransitionAsync(DispatchState.Running);

            await this.PostControlCommandAsync(new Argument[] 
            { 
                new Argument("action", "disable_preview") 
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task EnablePreviewAsync()
        {
            await this.TransitionAsync(DispatchState.Running);

            await this.PostControlCommandAsync(new Argument[] 
            { 
                new Argument("action", "enable_preview") 
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task FinalizeAsync()
        {
            await this.TransitionAsync(DispatchState.Running);

            await this.PostControlCommandAsync(new Argument[] 
            { 
                new Argument("action", "finalize") 
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task PauseAsync()
        {
            await this.TransitionAsync(DispatchState.Running);

            await this.PostControlCommandAsync(new Argument[] 
            { 
                new Argument("action", "pause") 
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await this.TransitionAsync(DispatchState.Running);

            await this.PostControlCommandAsync(new Argument[] 
            { 
                new Argument("action", "save") 
            });
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

            await this.PostControlCommandAsync(new Argument[] 
            { 
                new Argument("action", "unpause") 
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task UnsaveAsync()
        {
            await this.TransitionAsync(DispatchState.Running);

            await this.PostControlCommandAsync(new Argument[] 
            { 
                new Argument("action", "unsave") 
            });
        }

        public async Task UpdateJobArgs(JobArgs args)
        {
            using (var response = await this.Context.PostAsync(this.Namespace, this.ResourceName, args))
            {
                if (response.Message.StatusCode != HttpStatusCode.OK)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
            }
        }

        #endregion

        #region Methods used by our base class, Entity<TEntity>

        #endregion

        #region Privates

        async Task<SearchResults> GetSearchResultsAsync(string endpoint, IEnumerable<Argument> args)
        {
            var resourceName = new ResourceName(this.ResourceName, endpoint);

            var response = await this.Context.GetAsync(this.Namespace, resourceName, args);
            var searchResults = await SearchResults.CreateAsync(response, leaveOpen: false);

            return searchResults;
        }

        async Task PostControlCommandAsync(IEnumerable<Argument> args)
        {
            var resourceName = new ResourceName(this.ResourceName, "control");

            using (var response = await this.Context.PostAsync(this.Namespace, resourceName, args))
            {
                if (response.Message.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                }
            }
        }

        async Task TransitionAsync(DispatchState requiredState)
        {
            while (this.DispatchState < requiredState)
            {
                await Task.Delay(500);
                await this.GetAsync();
            }
        }

        #endregion
    }
}
