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

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Provides an object representation of a Splunk search job.
    /// </summary>
    public sealed class Job : Entity<Job>
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
        /// <see cref="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <see cref="context"/> or <see cref="namespace"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="namespace"/> is not specific.
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
        ///   <term><see cref="Service.DispatchSavedSearchAsnyc"/></term>
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
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        public long DiskUsage
        {
            get { return this.GetValue("DiskUsage", Int64Converter.Instance); } // sample value: "86016"
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
            get { return this.GetValue("DispatchState", EnumConverter<DispatchState>.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double DoneProgress
        {
            get { return this.GetValue("DoneProgress", DoubleConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        public DateTime EarliestTime
        {
            get { return this.GetValue("EarliestTime", DateTimeConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public long EventAvailableCount
        {
            get { return this.GetValue("EventAvailableCount", Int64Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public long EventCount
        {
            get { return this.GetValue("EventCount", Int64Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int EventFieldCount
        {
            get { return this.GetValue("EventFieldCount", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EventIsStreaming
        {
            get { return this.GetValue("EventIsStreaming", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EventIsTruncated
        {
            get { return this.GetValue("EventIsTruncated", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string EventSearch
        {
            get { return this.GetValue("EventSearch", StringConverter.Instance); }
        }

        /// <summary>
        /// 
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

        public bool IsDone
        {
            get { return this.GetValue("IsDone", BooleanConverter.Instance); }
        }

        public bool IsFailed
        {
            get { return this.GetValue("IsFailed", BooleanConverter.Instance); }
        }

        public bool IsFinalized
        {
            get { return this.GetValue("IsFinalized", BooleanConverter.Instance); }
        }

        public bool IsPaused
        {
            get { return this.GetValue("IsPaused", BooleanConverter.Instance); }
        }

        public bool IsPreviewEnabled
        {
            get { return this.GetValue("IsPreviewEnabled", BooleanConverter.Instance); }
        }

        public bool IsRealTimeSearch
        {
            get { return this.GetValue("IsRealTimeSearch", BooleanConverter.Instance); }
        }

        public bool IsRemoteTimeline
        {
            get { return this.GetValue("IsRemoteTimeline", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSaved
        {
            get { return this.GetValue("IsSaved", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSavedSearch
        {
            get { return this.GetValue("IsSavedSearch", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsZombie
        {
            get { return this.GetValue("IsZombie", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Keywords
        {
            get { return this.GetValue("Keywords", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LatestTime
        {
            get { return this.GetValue("LatestTime", DateTimeConverter.Instance); }
        }

        // Messages	{System.Dynamic.ExpandoObject}	System.Dynamic.ExpandoObject

        /// <summary>
        /// 
        /// </summary>
        public string NormalizedSearch
        {
            get { return this.GetValue("NormalizedSearch", StringConverter.Instance); } 
        }

        /// <summary>
        /// 
        /// </summary>
        public int NumPreviews
        {
            get { return this.GetValue("NumPreviews", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public int Priority
        {
            get { return this.GetValue("Priority", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string RemoteSearch
        {
            get { return this.GetValue("RemoteSearch", StringConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public dynamic Request
        {
            get { return this.GetValue("Request"); }
        }

        /// <summary>
        /// 
        /// </summary>
        public long ResultCount
        {
            get { return this.GetValue("ResultCount", Int64Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ResultIsStreaming
        {
            get { return this.GetValue("ResultIsStreaming", BooleanConverter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public long ResultPreviewCount
        {
            get { return this.GetValue("ResultPreviewCount", Int64Converter.Instance); }
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public long ScanCount
        {
            get { return this.GetValue("ScanCount", Int64Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Search
        {
            get { return this.Snapshot == null ? null : this.Snapshot.Title; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<string> SearchProviders
        {
            get { return this.GetValue("SearchProviders", CollectionConverter<string, List<string>, StringConverter>.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Sid
        {
            get { return this.Name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int StatusBuckets
        {
            get { return this.GetValue("StatusBuckets", Int32Converter.Instance); }
        }

        /// <summary>
        /// 
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
        /// search/jobs/{search_id}</a> endpoint to update the cached 
        /// cached state of the current <see cref="Job"/>.
        /// </remarks>
        public override async Task GetAsync()
        {
            //// TODO: This retry logic is for jobs. Parmeterize it and move it into the Job class

            // FJR: I assume the retry logic is for jobs, since nothing else requires this. I suggest moving it
            // into Job. Also, it's insufficient. If you're just trying to get some state, this will do it, but
            // as of Splunk 6, getting a 200 and content back does not imply you have all the fields. For pivot
            // support, they're now shoving fields in as they become ready, so you have to wait until the dispatchState
            // field of the Atom entry reaches a certain point.

            RequestException requestException = null;

            for (int i = 3; i > 0; --i)
            {
                try
                {
                    //// Guarantee: unique result because entities have specific namespace

                    using (var response = await this.Context.GetAsync(this.Namespace, this.ResourceName))
                    {
                        //// TODO: Use Response.EnsureStatusCode. Is it true that gets always return HttpStatusCode.OK?

                        if (response.Message.StatusCode == HttpStatusCode.NoContent)
                        {
                            var details = new Message[] 
                            { 
                                new Message(MessageType.Warning, string.Format("Resource '{0}/{1}' is not ready.", 
                                    this.Namespace, this.ResourceName))
                            };

                            throw new RequestException(response.Message, details);
                        }

                        if (!response.Message.IsSuccessStatusCode)
                        {
                            throw new RequestException(response.Message, await Message.ReadMessagesAsync(response.XmlReader));
                        }

                        var reader = response.XmlReader;
                        await reader.ReadAsync();

                        if (reader.NodeType == XmlNodeType.XmlDeclaration)
                        {
                            await response.XmlReader.ReadAsync();
                        }

                        if (reader.NodeType != XmlNodeType.Element)
                        {
                            throw new InvalidDataException(); // TODO: Diagnostics
                        }

                        AtomEntry entry;

                        if (reader.Name == "feed")
                        {
                            AtomFeed feed = new AtomFeed();

                            await feed.ReadXmlAsync(reader);
                            int count = feed.Entries.Count;

                            foreach (var feedEntry in feed.Entries)
                            {
                                string id = feedEntry.Title;
                                id.Trim();
                            }

                            if (feed.Entries.Count != 1)
                            {
                                throw new InvalidDataException(); // TODO: Diagnostics
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

                    return;
                }
                catch (RequestException e)
                {
                    if (e.StatusCode != System.Net.HttpStatusCode.NoContent)
                    {
                        throw;
                    }

                    requestException = e;
                }
                await Task.Delay(500);
            }

            throw requestException;
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
        /// <param name="requiredState"></param>
        /// <returns></returns>
        public async Task TransitionAsync(DispatchState requiredState)
        {
            while (this.DispatchState < requiredState)
            {
                await this.GetAsync();
                await Task.Delay(500);
            }
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
                await response.EnsureStatusCodeAsync(HttpStatusCode.OK);
            }
        }

        #endregion

        #region Methods used by our base class, Entity<TEntity>

        #endregion

        #region Privates

        async Task<SearchResultStream> GetSearchResultsAsync(string endpoint, IEnumerable<Argument> args)
        {
            var resourceName = new ResourceName(this.ResourceName, endpoint);
            var response = await this.Context.GetAsync(this.Namespace, resourceName, args);

            try
            {
                var searchResults = await SearchResultStream.CreateAsync(response, leaveOpen: false);
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
