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
//// [X] Contracts - there are none
//// [O] Documentation

namespace Splunk.Client.Refactored
{
    using Splunk.Client;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides an operational interface to the Splunk application collection.
    /// </summary>
    public interface IServerInfo
    {
        /// <summary>
        /// Gets the name of the active license group for the Splunk server
        /// instance.
        /// </summary>
        string ActiveLicenseGroup
        { get; }

        /// <summary>
        /// Gets the add ons installed on the Splunk server instance.
        /// </summary>
        dynamic AddOns
        { get; }

        /// <summary>
        /// Gets the build number for the Splunk server instance.
        /// </summary>
        int Build
        { get; }

        /// <summary>
        /// Gets CPU architecture information for the computer hosting splunkd.
        /// </summary>
        string CpuArchitecture
        { get; }

        /// <summary>
        /// Gets the access control lists for the Splunk server instance.
        /// </summary>
        Eai Eai
        { get; }

        /// <summary>
        /// Gets the <see cref="Guid"/> identifying the Splunk server instance.
        /// </summary>
        Guid Guid
        { get; }

        /// <summary>
        /// Gets a value indicating whether the server instance is running 
        /// under a free license.
        /// </summary>
        bool IsFree
        { get; }

        /// <summary>
        /// Gets a value indicating whether the server instance is realtime
        /// search enabled.
        /// </summary>
        bool IsRealtimeSearchEnabled
        { get; }

        /// <summary>
        /// Gets a value indicating whether the server instance is running 
        /// under a trial license.
        /// </summary>
        bool IsTrial
        { get; }

        /// <summary>
        /// Gets the list of license keys installed on the server instance.
        /// </summary>
        IReadOnlyList<string> LicenseKeys
        { get; }

        /// <summary>
        /// Gets the list of labels for the license keys installed on the
        /// server instance.
        /// </summary>
        IReadOnlyList<string> LicenseLabels
        { get; }

        /// <summary>
        /// Gets the has signature for the license keys installed on the server
        /// instance.
        /// </summary>
        string LicenseSignature
        { get; }

        /// <summary>
        /// Gets the status of the licenses installed on the serve instance.
        /// </summary>
        LicenseState LicenseState
        { get; }

        /// <summary>
        /// Gets the <see cref="Guid"/> identifying the license master for the 
        /// Splunk server instance.
        /// </summary>
        Guid MasterGuid
        { get; }

        /// <summary>
        /// Gets a value that specifies whether the Splunk server is a dedicated 
        /// forwarder or a normal instance.
        /// </summary>
        ServerMode Mode
        { get; }

        /// <summary>
        /// Gets the number of processor cores installed on the Splunk server
        /// instance.
        /// </summary>
        int NumberOfCores
        { get; }

        /// <summary>
        /// Gets build information for the operating system running splunkd.
        /// </summary>
        string OSBuild
        { get; }

        /// <summary>
        /// Gets the name of the operating system running splunkd.
        /// </summary>
        string OSName
        { get; }

        /// <summary>
        /// Gets version information for the operating system running splunkd.
        /// </summary>
        string OSVersion
        { get; }

        /// <summary>
        /// Gets the number of megabytes of physical memory installed on the 
        /// Splunk server instance.
        /// </summary>
        long PhysicalMemoryMB
        { get; }

        /// <summary>
        /// Gets a value that indicates whether realtime search is enabled on
        /// the Splunk server instance.
        /// </summary>
        bool RealtimeSearchEnabled
        { get; }

        /// <summary>
        /// Gets the fully qualified name of the Splunk server instance.
        /// </summary>
        string ServerName
        { get; }

        /// <summary>
        /// Gets the time that the Splunk server instance last started up.
        /// </summary>
        DateTime StartupTime
        { get; }

        /// <summary>
        /// Gets the version of the Splunk server instance.
        /// </summary>
        Version Version
        { get; }
    }
}
