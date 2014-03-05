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
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;

    public class Job : Entity<Job>, IDisposable
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

        public Job() // TODO: Remove this after refactoring EntityCollection<TEntity> and AtomFeed<TEntity> with an Entity<TEntity> factory
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
            get 
            { 
                if (this.backingFields.DispatchState == DispatchState.Unknown)
                {
                    string value = this.Record.DispatchState.ToString();
                    this.backingFields.DispatchState = (DispatchState)Enum.Parse(typeof(DispatchState), value, ignoreCase: true);
                }
                return this.backingFields.DispatchState;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Release unmanaged resources.
        /// </summary>
        public void Dispose()
        { this.Dispose(true); }

        public async Task<SearchResults> GetSearchResultsEventsAsync(JobEventsArgs args = null)
        {
            await this.TransitionAsync(DispatchState.Done);

            var searchResults = await this.GetAsync("events", args == null ? null : args.AsEnumerable());

            return searchResults;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<SearchResults> GetSearchResultsAsync(JobResultsArgs args = null)
        {
            await this.TransitionAsync(DispatchState.Done);

            var searchResults = await this.GetAsync("results", args == null ? null : args.AsEnumerable());

            return searchResults;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<SearchResults> GetSearchResultsPreviewAsync(JobResultsArgs args = null)
        {
            await this.TransitionAsync(DispatchState.Running);

            var searchResults = await this.GetAsync("results_preview", args == null ? null : args.AsEnumerable());

            return searchResults;
        }

        protected override string GetName(dynamic record)
        {
            Contract.Requires<ArgumentNullException>(record != null);
            return record.Sid;
        }

        protected override void Invalidate()
        {
            this.backingFields = InvalidatedBackingFields;
            base.Invalidate();
        }

        #endregion

        #region Privates

        static readonly BackingFields InvalidatedBackingFields = new BackingFields();
        BackingFields backingFields = new BackingFields();
        HttpResponseMessage response;

        void Dispose(bool disposing)
        {
            if (disposing && this.response != null)
            {
                this.response.Dispose();
                this.response = null;
                GC.SuppressFinalize(this);
            }
        }

        async Task<SearchResults> GetAsync(string endpoint, IEnumerable<KeyValuePair<string, object>> args)
        {
            var resourceName = new ResourceName(this.ResourceName, endpoint);
            this.Dispose(true);

            this.response = await this.Context.GetAsync(this.Namespace, resourceName, args);
            GC.ReRegisterForFinalize(this);
            var stream = await response.Content.ReadAsStreamAsync();
            
            var reader = XmlReader.Create(stream, SearchResultsReader.XmlReaderSettings);

            if (!await reader.ReadToFollowingAsync("results"))
            {
                throw new InvalidDataException();  // TODO: diagnostics
            }

            var searchResults = await SearchResults.CreateAsync(reader, leaveOpen: false);

            return searchResults;
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

        #region Types

        private struct BackingFields
        {
            public DispatchState DispatchState;
        }

        #endregion
    }
}
