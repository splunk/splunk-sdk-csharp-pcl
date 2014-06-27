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
        Task<TJob> CreateAsync(string search, JobArgs args, CustomJobArgs customArgs, DispatchState requiredState);

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

        /// <summary>
        /// Indexer to get items within this collection using array index syntax.
        /// </summary>
        /// <param name="index">
        /// Zero-based index of the entry to access.
        /// </param>
        /// <returns>
        /// The indexed item.
        /// </returns>
        public abstract TJob this[int index] { get; }

        /// <summary>
        /// Gets the number of. 
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public abstract int Count { get; }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        public abstract IReadOnlyList<Message> Messages { get; }

        /// <summary>
        /// Gets the pagination.
        /// </summary>
        /// <value>
        /// The pagination.
        /// </value>
        public abstract Pagination Pagination { get; }

        #endregion 

        #region Methods

        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="search">
        /// The search.
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
        public Task<TJob> CreateAsync(string search, JobArgs args, CustomJobArgs customArgs, DispatchState requiredState)
        {
            Contract.Requires<ArgumentOutOfRangeException>(args == null || args.ExecutionMode != ExecutionMode.OneShot);
            Contract.Requires<ArgumentNullException>(search != null);
            return default(Task<TJob>);
        }

        /// <summary>
        /// Gets all asynchronously.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public abstract Task GetAllAsync();

        /// <summary>
        /// Asynchronously retrieves a search <see cref="Job"/> by name.
        /// </summary>
        /// <param name="name">
        /// Name of the <see cref="Job"/> to retrieve.
        /// </param>
        /// <returns>
        /// The <see cref="Job"/> retrieved.
        /// </returns>
        public abstract Task<TJob> GetAsync(string name);

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public abstract IEnumerator<TJob> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return default(IEnumerator);
        }

        /// <summary>
        /// Gets slice asynchronous.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The slice asynchronous.
        /// </returns>
        public abstract Task GetSliceAsync(params Argument[] arguments);

        /// <summary>
        /// Gets slice asynchronous.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The slice asynchronous.
        /// </returns>
        public abstract Task GetSliceAsync(IEnumerable<Argument> arguments);

        /// <summary>
        /// Gets slice asynchronous.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// The slice asynchronous.
        /// </returns>
        public Task GetSliceAsync(JobCollection.Filter criteria)
        {
            Contract.Requires<ArgumentNullException>(criteria != null);
            return default(Task);
        }

        /// <summary>
        /// Reload asynchronous.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        public abstract Task ReloadAsync();

        #endregion
    }
}
