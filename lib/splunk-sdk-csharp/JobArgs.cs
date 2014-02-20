namespace Splunk.Sdk
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class JobArgs
    {
        [DataMember(Name = "search")]
        public string Search
        { get; set; }

        [DataMember(Name = "auto_cancel")]
        public int AutoCancel
        { get; set; }
        
        [DataMember(Name = "auto_finalize_ec")]
        public int AutoFinalizeEventCount
        { get; set; }

        [DataMember(Name = "auto_pause")]
        public int AutoPause
        { get; set; }

        [DataMember(Name = "earliest_time")]
        public string EarliestTime // TODO: Convenience class for specifying a time string. See 
        { get; set; }

        [DataMember(Name = "enable_lookups")]
        public bool EnableLookups
        { get; set; }

        [DataMember(Name = "exec_mode")]
        public ExecutionMode ExecutionMode
        { get; set; }

        [DataMember(Name = "force_bundle_replication")]
        public bool ForceBundleReplication
        { get; set; }

        [DataMember(Name = "id")]
        public string Id
        { get; set; }

        [DataMember(Name = "index_earliest")]
        public string IndexEarliest
        { get; set; }

        [DataMember(Name = "index_latest")]
        public string IndexLatest
        { get; set; }

        [DataMember(Name = "latest_time")]
        public string LatestTime
        { get; set; }

        [DataMember(Name = "max_count")]
        public int MaxCount
        { get; set; }

        [DataMember(Name = "max_time")]
        public int MaxTime
        { get; set; }

        [DataMember(Name = "namespace")]
        public string Namespace
        { get; set; }

        [DataMember(Name = "now")]
        public string Now
        { get; set; }

        [DataMember(Name = "reduce_freq")]
        public int ReduceFrequency
        { get; set; }

        [DataMember(Name = "reload_macros")]
        public bool ReloadMacros
        { get; set; }

        [DataMember(Name = "remote_server_list")]
        public IReadOnlyList<string> RemoteServerList
        { get; set; }

        [DataMember(Name = "rf")]
        public IReadOnlyList<string> RequiredFieldList // TODO: Searialze as a list of "rf" parameters (e.g., rf=foo&rf=bar&rf=foo%20bar) for { "foo", "bar", "foo bar" })
        { get; set; }

        [DataMember(Name = "reuse_max_seconds_ago")]
        public int ReuseMaxSecondsAgo
        { get; set; }

        [DataMember(Name = "rt_blocking")]
        public bool RealTimeBlocking
        { get; set; }

        [DataMember(Name = "rt_indexfilter")]
        public bool RealTimeIndexFilter
        { get; set; }

        [DataMember(Name = "rt_maxblocksecs")]
        public int RealTimeMaxBlockSeconds
        { get; set; }

        [DataMember(Name = "rt_queue_size")]
        public int RealTimeQueueSize
        { get; set; }

        [DataMember(Name = "search_listener")]
        public string SearchListener
        { get; set; }

        [DataMember(Name = "search_mode")]
        public SearchMode SearchMode
        { get; set; }

        [DataMember(Name = "spawn_process")]
        public bool SpawnProcess
        { get; set; }

        [DataMember(Name = "status_buckets")]
        public int StatusBuckets
        { get; set; }

        [DataMember(Name = "sync_bundle_replication")]
        public bool SyncBundleReplication
        { get; set; }

        [DataMember(Name = "time_format")]
        public string TimeFormat
        { get; set; }

        [DataMember(Name = "timeout")]
        public int Timeout
        { get; set; }

    }
}
