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
//// [0] Contracts
//// [O] Documentation

namespace Splunk.Client.Refactored
{
    using Splunk.Client;

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClass(typeof(IJobCollectionContract<>))]
    public interface IJobCollection<TJob> : IPaginated, IEntityCollection<TJob> 
        where TJob : ResourceEndpoint, IJob, new()
    {
        Task<TJob> CreateAsync(string search, JobArgs args, CustomJobArgs customArgs, DispatchState requiredState);
        Task GetSliceAsync(JobCollection.Filter criteria);
    }

    [ContractClassFor(typeof(IJobCollection<>))]
    abstract class IJobCollectionContract<TJob> : IJobCollection<TJob>
        where TJob : ResourceEndpoint, IJob, new()
    {
        #region Properties

        public abstract TJob this[int index] { get; }
        public abstract int Count { get; }
        public abstract IReadOnlyList<Message> Messages { get; }
        public abstract Pagination Pagination { get; }

        #endregion 

        #region Methods

        public Task<TJob> CreateAsync(string search, JobArgs args, CustomJobArgs customArgs, DispatchState requiredState)
        {
            Contract.Requires<ArgumentOutOfRangeException>(args == null || args.ExecutionMode != ExecutionMode.Oneshot);
            Contract.Requires<ArgumentNullException>(search != null);
            return default(Task<TJob>);
        }

        public abstract Task GetAllAsync();

        public abstract Task<TJob> GetAsync(string name);

        public abstract IEnumerator<TJob> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return default(IEnumerator);
        }

        public abstract Task GetSliceAsync(params Argument[] arguments);

        public abstract Task GetSliceAsync(IEnumerable<Argument> arguments);

        public Task GetSliceAsync(JobCollection.Filter criteria)
        {
            Contract.Requires<ArgumentNullException>(criteria != null);
            return default(Task);
        }

        public abstract Task ReloadAsync();

        #endregion
    }
}
