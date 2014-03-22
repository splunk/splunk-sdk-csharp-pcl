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
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    public sealed class Job : Entity<Job>
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="namespace"></param>
        /// <param name="collection"></param>
        /// <param name="name"></param>
        internal Job(Context context, Namespace @namespace, ResourceName collection, string name)
            : base(context, @namespace, collection, name)
        { }

        public Job() // TODO: Remove this after refactoring with an Entity<TEntity> factory
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value that indicates whether the current <see cref="Job"/>
        /// has completed.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current <see cref="Job"/> is complete; otherwise,
        /// <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Clients that call <see cref="Job.UpdateAsync"/> to poll for job 
        /// status use this property to determine whether search results are
        /// ready.
        /// </remarks>
        public bool IsCompleted
        {
            get { return this.DispatchState == DispatchState.Done; }
        }

        /// <summary>
        /// Gets value that indicates the current <see cref="Job"/> dispatch
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
            get { return this.GetValue("DispatchState", EnumConverter<DispatchState>.Instance); }
        }

        #endregion

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

        protected override string GetTitle()
        {
            return this.GetValue("Sid", StringConverter.Instance);
        }

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
                await this.UpdateAsync();
            }
        }

        #endregion
    }
}
