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
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for job collection.
    /// </summary>
    /// <typeparam name="TJob">
    /// Type of the job.
    /// </typeparam>
    /// <seealso cref="T:IPaginated"/>
    /// <seealso cref="T:IEntityCollection{TJob"/>
    [ContractClass(typeof(IJobCollectionContract<>))]
    public interface IJobCollection<TJob> : IPaginated, IEntityCollection<TJob, Resource> 
        where TJob : BaseEntity<Resource>, IJob, new()
    {
        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="search">
        /// The search.
        /// </param>
        /// <param name="count">
        /// 
        /// </param>
        /// <param name="mode">
        ///                    
        /// </param>
        /// <param name="args">
        /// The arguments.
        /// </param>
        /// <param name="customArgs">
        /// The custom arguments.
        /// </param>
        /// <param name="requiredState">
        /// State of the required.
        /// </param>
        /// <returns>
        /// The new asynchronous.
        /// </returns>
        Task<TJob> CreateAsync(string search, int count = 0, ExecutionMode mode = ExecutionMode.Normal, 
            JobArgs args = null, CustomJobArgs customArgs = null, 
            DispatchState requiredState = DispatchState.Running);

        /// <summary>
        /// Gets slice asynchronous.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// The slice asynchronous.
        /// </returns>
        Task GetSliceAsync(JobCollection.Filter criteria);
    }

    /// <summary>
    /// A job collection contract.
    /// </summary>
    /// <typeparam name="TJob">
    /// Type of the job.
    /// </typeparam>
    /// <seealso cref="T:Splunk.Client.IJobCollection{TJob}"/>
    [ContractClassFor(typeof(IJobCollection<>))]
    abstract class IJobCollectionContract<TJob> : IJobCollection<TJob>
        where TJob : BaseEntity<Resource>, IJob, new()
    {
        #region Properties

        public abstract TJob this[int index] { get; }

        public abstract int Count { get; }

        public abstract ReadOnlyCollection<Message> Messages { get; }

        public abstract Pagination Pagination { get; }

        #endregion 

        #region Methods

        public Task<TJob> CreateAsync(string search, int count = 0, ExecutionMode mode = ExecutionMode.Normal,
            JobArgs args = null, CustomJobArgs customArgs = null, 
            DispatchState requiredState = DispatchState.Running)
        {
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
