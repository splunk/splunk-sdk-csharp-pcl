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
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an operational interface to Splunk index entities.
    /// </summary>
    public interface IIndex : IEntity
    {
        #region Properties

        /// <summary>
        /// Gets a value that indicates whether all data from the <see cref=
        /// "Index"/> is proper UTF-8.
        /// </summary>
        /// <remarks>
        /// This is a global setting, not a per-index setting.
        /// </remarks>
        bool AssureUTF8
        { get; }

        /// <summary>
        /// Gets the number of events that make up a block for block signatures
        /// on an index.
        /// </summary>
        /// <remarks>
        /// The default value is zero (0) indicating that block signatures are
        /// disabled. If your index requires block signatures, a value is 100
        /// is recommended. 
        /// </remarks>
        int BlockSignSize
        { get; }

        /// <summary>
        /// Gets the name of the index that stores block signatures of events.
        /// </summary>
        /// <remarks>
        /// This is a global setting, not a per-index setting.
        /// </remarks>
        string BlockSignatureDatabase
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int BloomFilterTotalSizeKB
        { get; }

        /// <summary>
        /// 
        /// </summary>
        string BucketRebuildMemoryHint
        { get; }

        /// <summary>
        /// Gets the path to the cold databases for the index.
        /// </summary>
        string ColdPath
        { get; }

        /// <summary>
        /// Gets the absolute path to the cold databases for the index.
        /// </summary>
        string ColdPathExpanded
        { get; }

        /// <summary>
        /// Gets the maximum size in MB for the cold database to reach before
        /// a roll to the frozen archive is triggered.
        /// </summary>
        long ColdPathMaxDataSizeMB
        { get; }

        /// <summary>
        /// Gets the path to a directory for the frozen archive of an index.
        /// </summary>
        /// <remarks>
        /// This property is an alternative to <see cref= "ColdToFrozenScript"/>. 
        /// If <see cref="ColdToFrozenDir"/> and <see cref="ColdToFrozenScript"/> 
        /// are specified, <see cref="ColdToFrozenDir"/> takes precedence. 
        /// Splunk will automatically put frozen buckets in this directory.
        /// </remarks>        
        string ColdToFrozenDir
        { get; }

        /// <summary>
        /// Gets the path to an archiving script for the frozen archive of an 
        /// index.
        /// </summary>
        /// <remarks>
        /// If your script requires a program to run it (for example, python), 
        /// specify the program followed by the path. The script must be in 
        /// <c>$SPLUNK_HOME/bin</c> or one of its subdirectories.
        /// </remarks>
        string ColdToFrozenScript
        { get; }

        /// <summary>
        /// Gets or sets the path to an archiving script for the frozen archive
        /// of an index.
        /// </summary>
        /// <remarks>
        /// If your script requires a program to run it (for example, python), 
        /// specify the program followed by the path. The script must be in 
        /// <c>$SPLUNK_HOME/bin</c> or one of its subdirectories.
        /// </remarks>
        bool CompressRawData
        { get; }

        /// <summary>
        /// Gets the total size in megabytes of data stored in the current <see 
        /// cref="Index"/>.
        /// </summary>
        /// <remarks>
        /// The total includes the size of data in the <see cref="HomePath"/>, 
        /// <see cref="ColdPath"/>, and <see cref="ThawedPath"/> databases.
        /// </remarks>
        long CurrentDBSizeMB
        { get; }

        /// <summary>
        /// Gets the name of the index for input data that does not contain
        /// index destination information.
        /// </summary>
        /// <remarks>
        /// If no index destination information is available in the input data, 
        /// the index shown here is the destination of that data.
        /// </remarks>
        string DefaultDatabase
        { get; }

        /// <summary>
        /// Gets a value that indicates whether the current <see cref="Index"/>
        /// is disabled.
        /// </summary>
        /// <remarks>
        /// This value is <c>true</c>, if the current <see cref="Index"/> is
        /// disabled; otherwise, <c>false</c>.
        /// </remarks>
        bool Disabled
        { get; }

        /// <summary>
        /// Gets the access control lists for the current <see cref="Index"/>.
        /// </summary>
        Eai Eai
        { get; }

        /// <summary>
        /// 
        /// </summary>
        bool EnableOnlineBucketRepair
        { get; }

        /// <summary>
        /// Gets a value that indicates if realtime searches are enabled.
        /// </summary>
        /// <remarks>
        /// This is a global setting, not a per-index setting.
        /// </remarks>
        bool EnableRealTimeSearch
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int FrozenTimePeriodInSecs
        { get; }

        /// <summary>
        /// Gets the path to the hot and warm buckets for the current <see 
        /// cref="Index"/>.
        /// </summary>
        string HomePath
        { get; }

        /// <summary>
        /// Gets the absolute path to the hot and warm buckets for the current
        /// <see cref="Index"/>.
        /// </summary>
        string HomePathExpanded
        { get; }

        /// <summary>
        /// Gets the maximum size in MB for the hot and warm buckets for the
        /// current <see cref="Index"/> to reach.
        /// </summary>
        long HomePathMaxDataSizeMB
        { get; }

        /// <summary>
        /// Gets the number of threads used for indexing.
        /// </summary>
        string IndexThreads
        { get; }

        /// <summary>
        /// Gets a value that indicates if the current <see cref="Index"/> is
        /// an internal index.
        /// </summary>
        /// <remarks>
        /// Internal indexes include, for example, <c>"_audit"</c> and <c>
        /// "_internal"</c>.
        /// </remarks>
        bool IsInternal
        { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsReady
        { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsVirtual
        { get; }

        /// <summary>
        /// 
        /// </summary>
        long LastInitSequenceNumber
        { get; }

        /// <summary>
        /// 
        /// </summary>
        long LastInitTime
        { get; }

        /// <summary>
        /// 
        /// </summary>
        string MaxBloomBackfillBucketAge
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int MaxBucketSizeCacheEntries
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int MaxConcurrentOptimizes
        { get; }

        /// <summary>
        /// 
        /// </summary>
        string MaxDataSize
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int MaxHotBuckets
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int MaxHotIdleSecs
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int MaxHotSpanSecs
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int MaxMemMB
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int MaxMetaEntries
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int MaxRunningProcessGroups
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int MaxRunningProcessGroupsLowPriority
        { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTime MaxTime
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int MaxTimeUnreplicatedNoAcks
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int MaxTimeUnreplicatedWithAcks
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int MaxTotalDataSizeMB
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int MaxWarmDBCount
        { get; }

        /// <summary>
        /// 
        /// </summary>
        string MemPoolMB
        { get; }

        /// <summary>
        /// 
        /// </summary>
        string MinRawFileSyncSecs
        { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTime MinTime
        { get; }

        /// <summary>
        /// Gets the number of bloom filters that have been created for the 
        /// current <see cref="Index"/>.
        /// </summary>
        int NumBloomFilters
        { get; }

        /// <summary>
        /// Gets the number of hot buckets that have been created for the 
        /// current <see cref="Index"/>.
        /// </summary>
        int NumHotBuckets
        { get; }

        /// <summary>
        /// Gets the number of warm buckets that have been created for the 
        /// current <see cref="Index"/>.
        /// </summary>
        int NumWarmBuckets
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int PartialServiceMetaPeriod
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int ProcessTrackerServiceInterval
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int QuarantineFutureSecs
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int QuarantinePastSecs
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int RawChunkSizeBytes
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int RepFactor
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int RotatePeriodInSecs
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int ServiceMetaPeriod
        { get; }

        /// <summary>
        /// 
        /// </summary>
        bool ServiceOnlyAsNeeded
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int ServiceSubtaskTimingPeriod
        { get; }

        /// <summary>
        /// 
        /// </summary>
        string SummaryHomePathExpanded
        { get; }

        /// <summary>
        /// Gets the list of indexes for the "index missing" warning banner 
        /// messages are suppressed.
        /// </summary>
        /// <remarks>
        /// This is a global setting, not a per index setting.
        /// </remarks>
        string SuppressBannerList
        { get; }

        /// <summary>
        /// 
        /// </summary>
        bool Sync
        { get; }

        /// <summary>
        /// 
        /// </summary>
        bool SyncMeta
        { get; }

        /// <summary>
        /// Gets the path to the resurrected databases for the current <see 
        /// cref="Index"/>.
        /// </summary>
        string ThawedPath
        { get; }

        /// <summary>
        /// 
        /// </summary>
        string ThawedPathExpanded
        { get; }

        /// <summary>
        /// 
        /// </summary>
        int ThrottleCheckPeriod
        { get; }

        /// <summary>
        /// 
        /// </summary>
        long TotalEventCount
        { get; }

        /// <summary>
        /// 
        /// </summary>
        string TStatsHomePath
        { get; }

        /// <summary>
        /// 
        /// </summary>
        string TStatsHomePathExpanded
        { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously disables the current <see cref="Index"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// This method uses the POST data/indexes/{name}/disable endpoint to 
        /// disable the current <see cref="Index"/>.
        /// </remarks>
        Task DisableAsync();

        /// <summary>
        /// Asynchronously enables the current <see cref="Index"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// This method uses the POST data/indexes/{name}/enable endpoint to 
        /// enable the current <see cref="Index"/>.
        /// </remarks>
        Task EnableAsync();

        /// <summary>
        /// Asychronously updates the current <see cref="Index"/> with new <see
        /// cref="IndexAttributes"/>.
        /// </summary>
        /// <param name="attributes">
        /// New attributes for the current <see cref="Index"/> instance.
        /// </param>
        /// <returns>
        /// A value of <c>true</c> indicating that the cached state of the
        /// current <see cref="Index"/> was also updated.
        /// </returns>
        /// <remarks>
        /// This method uses the <a href="http://goo.gl/n3S22S">POST 
        /// data/indexes/{name} </a> endpoint to update the attributes of the
        /// index represented by this instance.
        /// </remarks>
        Task<bool> UpdateAsync(IndexAttributes attributes);

        #endregion
    }
}
