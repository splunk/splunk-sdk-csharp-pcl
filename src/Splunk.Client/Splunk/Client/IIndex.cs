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
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an operational interface to Splunk index entities.
    /// </summary>
    /// <seealso cref="T:IEntity"/>
    public interface IIndex : IEntity
    {
        #region Properties

        /// <summary>
        /// Gets a value that indicates whether all data from the
        /// <see cref= "Index"/> is proper UTF-8.
        /// </summary>
        /// <remarks>
        /// This is a global setting, not a per-index setting.
        /// </remarks>
        /// <value>
        /// <c>true</c> if assure UTF 8, <c>false</c> if not.
        /// </value>
        bool AssureUTF8
        { get; }

        /// <summary>
        /// Gets the number of events that make up a block for block signatures on an
        /// index.
        /// </summary>
        /// <remarks>
        /// The default value is zero (0) indicating that block signatures are
        /// disabled. If your index requires block signatures, a value is 100 is
        /// recommended.
        /// </remarks>
        /// <value>
        /// The size of the block sign.
        /// </value>
        int BlockSignSize
        { get; }

        /// <summary>
        /// Gets the name of the index that stores block signatures of events.
        /// </summary>
        /// <remarks>
        /// This is a global setting, not a per-index setting.
        /// </remarks>
        /// <value>
        /// The block signature database.
        /// </value>
        string BlockSignatureDatabase
        { get; }

        /// <summary>
        /// Gets the bloom filter total size kilobytes.
        /// </summary>
        /// <value>
        /// The bloom filter total size kilobytes.
        /// </value>
        int BloomFilterTotalSizeKB
        { get; }

        /// <summary>
        /// Gets the bucket rebuild memory hint.
        /// </summary>
        /// <value>
        /// The bucket rebuild memory hint.
        /// </value>
        string BucketRebuildMemoryHint
        { get; }

        /// <summary>
        /// Gets the path to the cold databases for the index.
        /// </summary>
        /// <value>
        /// The full pathname of the cold file.
        /// </value>
        string ColdPath
        { get; }

        /// <summary>
        /// Gets the absolute path to the cold databases for the index.
        /// </summary>
        /// <value>
        /// The cold path expanded.
        /// </value>
        string ColdPathExpanded
        { get; }

        /// <summary>
        /// Gets the maximum size in MB for the cold database to reach before a roll
        /// to the frozen archive is triggered.
        /// </summary>
        /// <value>
        /// The cold path maximum data size megabytes.
        /// </value>
        long ColdPathMaxDataSizeMB
        { get; }

        /// <summary>
        /// Gets the path to a directory for the frozen archive of an index.
        /// </summary>
        /// <remarks>
        /// This property is an alternative to <see cref= "ColdToFrozenScript"/>. If
        /// <see cref="ColdToFrozenDir"/> and <see cref="ColdToFrozenScript"/>
        /// are specified, <see cref="ColdToFrozenDir"/> takes precedence. Splunk
        /// will automatically put frozen buckets in this directory.
        /// </remarks>
        /// <value>
        /// The cold to frozen dir.
        /// </value>
        string ColdToFrozenDir
        { get; }

        /// <summary>
        /// Gets the path to an archiving script for the frozen archive of an index.
        /// </summary>
        /// <remarks>
        /// If your script requires a program to run it (for example, python),
        /// specify the program followed by the path. The script must be in
        /// <c>$SPLUNK_HOME/bin</c> or one of its subdirectories.
        /// </remarks>
        /// <value>
        /// The cold to frozen script.
        /// </value>
        string ColdToFrozenScript
        { get; }

        /// <summary>
        /// Gets the path to an archiving script for the frozen archive of an index.
        /// </summary>
        /// <remarks>
        /// If your script requires a program to run it (for example, python),
        /// specify the program followed by the path. The script must be in
        /// <c>$SPLUNK_HOME/bin</c> or one of its subdirectories.
        /// </remarks>
        /// <value>
        /// <c>true</c> if compress raw data, <c>false</c> if not.
        /// </value>
        bool CompressRawData
        { get; }

        /// <summary>
        /// Gets the total size in megabytes of data stored in the current
        /// <see cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// The total includes the size of data in the <see cref="HomePath"/>,
        /// <see cref="ColdPath"/>, and <see cref="ThawedPath"/> databases.
        /// </remarks>
        /// <value>
        /// The current database size megabytes.
        /// </value>
        long CurrentDBSizeMB
        { get; }

        /// <summary>
        /// Gets the name of the index for input data that does not contain index
        /// destination information.
        /// </summary>
        /// <remarks>
        /// If no index destination information is available in the input data, the
        /// index shown here is the destination of that data.
        /// </remarks>
        /// <value>
        /// The default database.
        /// </value>
        string DefaultDatabase
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref="Index"/>
        /// is disabled.
        /// </summary>
        /// <remarks>
        /// This value is <c>true</c>, if the current <see cref="Index"/> is disabled;
        /// otherwise, <c>false</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c> if disabled, <c>false</c> if not.
        /// </value>
        bool Disabled
        { get; }

        /// <summary>
        /// Gets the extensible administration interface properties for the current
        /// <see cref="Index"/>.
        /// </summary>
        /// <value>
        /// The extensible administration interface properties.
        /// </value>
        Eai Eai
        { get; }

        /// <summary>
        /// Gets a value indicating whether the online bucket repair is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if enable online bucket repair, <c>false</c> if not.
        /// </value>
        bool EnableOnlineBucketRepair
        { get; }

        /// <summary>
        /// Gets a value that indicates if realtime searches are enabled.
        /// </summary>
        /// <remarks>
        /// This is a global setting, not a per-index setting.
        /// </remarks>
        /// <value>
        /// <c>true</c> if enable real time search, <c>false</c> if not.
        /// </value>
        bool EnableRealTimeSearch
        { get; }

        /// <summary>
        /// Gets the frozen time period in seconds.
        /// </summary>
        /// <value>
        /// The frozen time period in seconds.
        /// </value>
        int FrozenTimePeriodInSecs
        { get; }

        /// <summary>
        /// Gets the path to the hot and warm buckets for the current
        /// <see cref="Index"/>.
        /// </summary>
        /// <value>
        /// The full pathname of the home file.
        /// </value>
        string HomePath
        { get; }

        /// <summary>
        /// Gets the absolute path to the hot and warm buckets for the current
        /// <see cref="Index"/>.
        /// </summary>
        /// <value>
        /// The home path expanded.
        /// </value>
        string HomePathExpanded
        { get; }

        /// <summary>
        /// Gets the maximum size in MB for the hot and warm buckets for the current
        /// <see cref="Index"/> to reach.
        /// </summary>
        /// <value>
        /// The home path maximum data size megabytes.
        /// </value>
        long HomePathMaxDataSizeMB
        { get; }

        /// <summary>
        /// Gets the number of threads used for indexing.
        /// </summary>
        /// <value>
        /// The index threads.
        /// </value>
        string IndexThreads
        { get; }

        /// <summary>
        /// Gets a value that indicates if the current <see cref="Index"/> is an
        /// internal index.
        /// </summary>
        /// <remarks>
        /// Internal indexes include, for example, <c>"_audit"</c> and <c>
        /// "_internal"</c>.
        /// </remarks>
        /// <value>
        /// <c>true</c> if this object is internal, <c>false</c> if not.
        /// </value>
        bool IsInternal
        { get; }

        /// <summary>
        /// Gets a value indicating whether this object is ready.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is ready, <c>false</c> if not.
        /// </value>
        bool IsReady
        { get; }

        /// <summary>
        /// Gets a value indicating whether this object is virtual.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is virtual, <c>false</c> if not.
        /// </value>
        bool IsVirtual
        { get; }

        /// <summary>
        /// Gets the last initialise sequence number.
        /// </summary>
        /// <value>
        /// The last initialise sequence number.
        /// </value>
        long LastInitSequenceNumber
        { get; }

        /// <summary>
        /// Gets the last initialise time.
        /// </summary>
        /// <value>
        /// The last initialise time.
        /// </value>
        long LastInitTime
        { get; }

        /// <summary>
        /// Gets the maximum bloom backfill bucket age.
        /// </summary>
        /// <value>
        /// The maximum bloom backfill bucket age.
        /// </value>
        string MaxBloomBackfillBucketAge
        { get; }

        /// <summary>
        /// Gets the maximum bucket size cache entries.
        /// </summary>
        /// <value>
        /// The maximum bucket size cache entries.
        /// </value>
        int MaxBucketSizeCacheEntries
        { get; }

        /// <summary>
        /// Gets the maximum concurrent optimizes.
        /// </summary>
        /// <value>
        /// The maximum concurrent optimizes.
        /// </value>
        int MaxConcurrentOptimizes
        { get; }

        /// <summary>
        /// Gets the size of the maximum data.
        /// </summary>
        /// <value>
        /// The size of the maximum data.
        /// </value>
        string MaxDataSize
        { get; }

        /// <summary>
        /// Gets the maximum hot buckets.
        /// </summary>
        /// <value>
        /// The maximum hot buckets.
        /// </value>
        int MaxHotBuckets
        { get; }

        /// <summary>
        /// Gets the maximum hot idle seconds.
        /// </summary>
        /// <value>
        /// The maximum hot idle seconds.
        /// </value>
        int MaxHotIdleSecs
        { get; }

        /// <summary>
        /// Gets the maximum hot span seconds.
        /// </summary>
        /// <value>
        /// The maximum hot span seconds.
        /// </value>
        int MaxHotSpanSecs
        { get; }

        /// <summary>
        /// Gets the maximum memory megabytes.
        /// </summary>
        /// <value>
        /// The maximum memory megabytes.
        /// </value>
        int MaxMemMB
        { get; }

        /// <summary>
        /// Gets the maximum meta entries.
        /// </summary>
        /// <value>
        /// The maximum meta entries.
        /// </value>
        int MaxMetaEntries
        { get; }

        /// <summary>
        /// Gets the groups the maximum running process belongs to.
        /// </summary>
        /// <value>
        /// The maximum running process groups.
        /// </value>
        int MaxRunningProcessGroups
        { get; }

        /// <summary>
        /// Gets the maximum running process groups low priority.
        /// </summary>
        /// <value>
        /// The maximum running process groups low priority.
        /// </value>
        int MaxRunningProcessGroupsLowPriority
        { get; }

        /// <summary>
        /// Gets the maximum time.
        /// </summary>
        /// <value>
        /// The maximum time.
        /// </value>
        DateTime MaxTime
        { get; }

        /// <summary>
        /// Gets the maximum time unreplicated no acks.
        /// </summary>
        /// <value>
        /// The maximum time unreplicated no acks.
        /// </value>
        int MaxTimeUnreplicatedNoAcks
        { get; }

        /// <summary>
        /// Gets the maximum time unreplicated with acks.
        /// </summary>
        /// <value>
        /// The maximum time unreplicated with acks.
        /// </value>
        int MaxTimeUnreplicatedWithAcks
        { get; }

        /// <summary>
        /// Gets the maximum total data size megabytes.
        /// </summary>
        /// <value>
        /// The maximum total data size megabytes.
        /// </value>
        int MaxTotalDataSizeMB
        { get; }

        /// <summary>
        /// Gets the number of maximum warm databases.
        /// </summary>
        /// <value>
        /// The number of maximum warm databases.
        /// </value>
        int MaxWarmDBCount
        { get; }

        /// <summary>
        /// Gets the memory pool megabytes.
        /// </summary>
        /// <value>
        /// The memory pool megabytes.
        /// </value>
        string MemPoolMB
        { get; }

        /// <summary>
        /// Gets the minimum raw file synchronise seconds.
        /// </summary>
        /// <value>
        /// The minimum raw file synchronise seconds.
        /// </value>
        string MinRawFileSyncSecs
        { get; }

        /// <summary>
        /// Gets the minimum time.
        /// </summary>
        /// <value>
        /// The minimum time.
        /// </value>
        DateTime MinTime
        { get; }

        /// <summary>
        /// Gets the number of bloom filters that have been created for the current
        /// <see cref="Index"/>.
        /// </summary>
        /// <value>
        /// The total number of bloom filters.
        /// </value>
        int NumBloomFilters
        { get; }

        /// <summary>
        /// Gets the number of hot buckets that have been created for the current
        /// <see cref="Index"/>.
        /// </summary>
        /// <value>
        /// The total number of hot buckets.
        /// </value>
        int NumHotBuckets
        { get; }

        /// <summary>
        /// Gets the number of warm buckets that have been created for the current
        /// <see cref="Index"/>.
        /// </summary>
        /// <value>
        /// The total number of warm buckets.
        /// </value>
        int NumWarmBuckets
        { get; }

        /// <summary>
        /// Gets the partial service meta period.
        /// </summary>
        /// <value>
        /// The partial service meta period.
        /// </value>
        int PartialServiceMetaPeriod
        { get; }

        /// <summary>
        /// Gets the process tracker service interval.
        /// </summary>
        /// <value>
        /// The process tracker service interval.
        /// </value>
        int ProcessTrackerServiceInterval
        { get; }

        /// <summary>
        /// Gets the quarantine future seconds.
        /// </summary>
        /// <value>
        /// The quarantine future seconds.
        /// </value>
        int QuarantineFutureSecs
        { get; }

        /// <summary>
        /// Gets the quarantine past seconds.
        /// </summary>
        /// <value>
        /// The quarantine past seconds.
        /// </value>
        int QuarantinePastSecs
        { get; }

        /// <summary>
        /// Gets the raw chunk size bytes.
        /// </summary>
        /// <value>
        /// The raw chunk size bytes.
        /// </value>
        int RawChunkSizeBytes
        { get; }

        /// <summary>
        /// Gets the rep factor.
        /// </summary>
        /// <value>
        /// The rep factor.
        /// </value>
        int RepFactor
        { get; }

        /// <summary>
        /// Gets the rotate period in seconds.
        /// </summary>
        /// <value>
        /// The rotate period in seconds.
        /// </value>
        int RotatePeriodInSecs
        { get; }

        /// <summary>
        /// Gets the service meta period.
        /// </summary>
        /// <value>
        /// The service meta period.
        /// </value>
        int ServiceMetaPeriod
        { get; }

        /// <summary>
        /// Gets a value indicating whether the service only as needed.
        /// </summary>
        /// <value>
        /// <c>true</c> if service only as needed, <c>false</c> if not.
        /// </value>
        bool ServiceOnlyAsNeeded
        { get; }

        /// <summary>
        /// Gets the service subtask timing period.
        /// </summary>
        /// <value>
        /// The service subtask timing period.
        /// </value>
        int ServiceSubtaskTimingPeriod
        { get; }

        /// <summary>
        /// Gets the summary home path expanded.
        /// </summary>
        /// <value>
        /// The summary home path expanded.
        /// </value>
        string SummaryHomePathExpanded
        { get; }

        /// <summary>
        /// Gets the list of indexes for the "index missing" warning banner messages
        /// are suppressed.
        /// </summary>
        /// <remarks>
        /// This is a global setting, not a per index setting.
        /// </remarks>
        /// <value>
        /// A List of suppress banners.
        /// </value>
        string SuppressBannerList
        { get; }

        /// <summary>
        /// Gets a value indicating whether the synchronise.
        /// </summary>
        /// <value>
        /// <c>true</c> if synchronise, <c>false</c> if not.
        /// </value>
        bool Sync
        { get; }

        /// <summary>
        /// Gets a value indicating whether the synchronise meta.
        /// </summary>
        /// <value>
        /// <c>true</c> if synchronise meta, <c>false</c> if not.
        /// </value>
        bool SyncMeta
        { get; }

        /// <summary>
        /// Gets the path to the resurrected databases for the current
        /// <see cref="Index"/>.
        /// </summary>
        /// <value>
        /// The full pathname of the thawed file.
        /// </value>
        string ThawedPath
        { get; }

        /// <summary>
        /// Gets the thawed path expanded.
        /// </summary>
        /// <value>
        /// The thawed path expanded.
        /// </value>
        string ThawedPathExpanded
        { get; }

        /// <summary>
        /// Gets the throttle check period.
        /// </summary>
        /// <value>
        /// The throttle check period.
        /// </value>
        int ThrottleCheckPeriod
        { get; }

        /// <summary>
        /// Gets the number of total events.
        /// </summary>
        /// <value>
        /// The total number of event count.
        /// </value>
        long TotalEventCount
        { get; }

        /// <summary>
        /// Gets the full pathname of the statistics home file.
        /// </summary>
        /// <value>
        /// The full pathname of the statistics home file.
        /// </value>
        string TStatsHomePath
        { get; }

        /// <summary>
        /// Gets the statistics home path expanded.
        /// </summary>
        /// <value>
        /// The t statistics home path expanded.
        /// </value>
        string TStatsHomePathExpanded
        { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously disables the current <see cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the POST data/indexes/{name}/disable endpoint to disable
        /// the current <see cref="Index"/>.
        /// </remarks>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task DisableAsync();

        /// <summary>
        /// Asynchronously enables the current <see cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the POST data/indexes/{name}/enable endpoint to enable
        /// the current <see cref="Index"/>.
        /// </remarks>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        Task EnableAsync();

        /// <summary>
        /// Asychronously updates the current <see cref="Index"/> with new
        /// <see cref="IndexAttributes"/>.
        /// </summary>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/n3S22S">POST
        /// data/indexes/{name} </a> endpoint to update the attributes of the index
        /// represented by this instance.
        /// </remarks>
        /// <param name="attributes">
        /// New attributes for the current <see cref="Index"/> instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if the current snapshot was also updated.
        /// </returns>
        Task<bool> UpdateAsync(IndexAttributes attributes);

        #endregion
    }
}
