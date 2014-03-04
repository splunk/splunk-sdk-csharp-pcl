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
// [ ] Trace messages (e.g., when there are no observers)

namespace Splunk.Sdk
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class Job : Entity<Job>
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

        public bool IsCompleted
        {
            get { return this.DispatchState == DispatchState.Done; }
        }

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

        public async Task GetResults(JobResultsArgs args = null)
        {
            while (!this.IsCompleted)
            {
                await Task.Delay(500);
                await this.UpdateAsync();
            }
            HttpResponseMessage response = await this.Context.GetAsync(this.Namespace, this.ResourceName, args == null ? null : args.AsEnumerable());
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

        #endregion

        #region Types

        private struct BackingFields
        {
            public DispatchState DispatchState;
        }

        #endregion
    }
}
