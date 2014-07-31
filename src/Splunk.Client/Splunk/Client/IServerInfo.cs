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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Provides an operational interface to the Splunk server info resource.
    /// </summary>
    /// <seealso cref="T:IBaseResource"/>
    public interface IServerInfo : IBaseResource
    {
        /// <summary>
        /// Gets the name of the active license group for the Splunk server instance.
        /// </summary>
        /// <value>
        /// The active license group.
        /// </value>
        string ActiveLicenseGroup
        { get; }

        /// <summary>
        /// Gets the add ons installed on the Splunk server instance.
        /// </summary>
        /// <value>
        /// The add ons.
        /// </value>
        dynamic AddOns
        { get; }

        /// <summary>
        /// Gets the build number for the Splunk server instance.
        /// </summary>
        /// <value>
        /// The build.
        /// </value>
        int Build
        { get; }

        /// <summary>
        /// Gets CPU architecture information for the computer hosting splunkd.
        /// </summary>
        /// <value>
        /// The CPU architecture.
        /// </value>
        string CpuArchitecture
        { get; }

        /// <summary>
        /// Gets the extensible administration interface properties for the Splunk
        /// server instance.
        /// </summary>
        /// <value>
        /// The extensible administration interface properties.
        /// </value>
        Eai Eai
        { get; }

        /// <summary>
        /// Gets the <see cref="Guid"/> identifying the Splunk server instance.
        /// </summary>
        /// <value>
        /// The <see cref="Guid"/> identifying the Splunk server instance.
        /// </value>
        Guid Guid
        { get; }

        /// <summary>
        /// Gets a value indicating whether the server instance is running under a
        /// free license.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is free, <c>false</c> if not.
        /// </value>
        bool IsFree
        { get; }

        /// <summary>
        /// Gets a value indicating whether the server instance is realtime search
        /// enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if a real time search is enabled, <c>false</c> if not.
        /// </value>
        bool IsRealTimeSearchEnabled
        { get; }

        /// <summary>
        /// Gets a value indicating whether the server instance is running under a
        /// trial license.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is trial, <c>false</c> if not.
        /// </value>
        bool IsTrial
        { get; }

        /// <summary>
        /// Gets the list of license keys installed on the server instance.
        /// </summary>
        /// <value>
        /// The license keys.
        /// </value>
        ReadOnlyCollection<string> LicenseKeys
        { get; }

        /// <summary>
        /// Gets the list of labels for the license keys installed on the server
        /// instance.
        /// </summary>
        /// <value>
        /// The license labels.
        /// </value>
        ReadOnlyCollection<string> LicenseLabels
        { get; }

        /// <summary>
        /// Gets the has signature for the license keys installed on the server
        /// instance.
        /// </summary>
        /// <value>
        /// The license signature.
        /// </value>
        string LicenseSignature
        { get; }

        /// <summary>
        /// Gets the status of the licenses installed on the serve instance.
        /// </summary>
        /// <value>
        /// The license state.
        /// </value>
        LicenseState LicenseState
        { get; }

        /// <summary>
        /// Gets the <see cref="Guid"/> identifying the license master for the Splunk
        /// server instance.
        /// </summary>
        /// <value>
        /// Unique identifier of the master.
        /// </value>
        Guid MasterGuid
        { get; }

        /// <summary>
        /// Gets a value that specifies whether the Splunk server is a dedicated
        /// forwarder or a normal instance.
        /// </summary>
        /// <value>
        /// A value specifying whether the Splunk server is a dedicated forwarder
        /// or a normal instance.
        /// </value>
        ServerMode Mode
        { get; }

        /// <summary>
        /// Gets the number of processor cores installed on the Splunk server
        /// instance.
        /// </summary>
        /// <value>
        /// The total number of cores.
        /// </value>
        int NumberOfCores
        { get; }

        /// <summary>
        /// Gets build information for the operating system running splunkd.
        /// </summary>
        /// <value>
        /// The operating system build.
        /// </value>
        string OSBuild
        { get; }

        /// <summary>
        /// Gets the name of the operating system running splunkd.
        /// </summary>
        /// <value>
        /// The name of the operating system.
        /// </value>
        string OSName
        { get; }

        /// <summary>
        /// Gets version information for the operating system running splunkd.
        /// </summary>
        /// <value>
        /// The operating system version.
        /// </value>
        string OSVersion
        { get; }

        /// <summary>
        /// Gets the number of megabytes of physical memory installed on the Splunk
        /// server instance.
        /// </summary>
        /// <value>
        /// The physical memory megabytes.
        /// </value>
        long PhysicalMemoryMB
        { get; }

        /// <summary>
        /// Gets a value that indicates whether realtime search is enabled on the
        /// Splunk server instance.
        /// </summary>
        /// <value>
        /// <c>true</c> if real time search enabled, <c>false</c> if not.
        /// </value>
        bool RealTimeSearchEnabled
        { get; }

        /// <summary>
        /// Gets the fully qualified name of the Splunk server instance.
        /// </summary>
        /// <value>
        /// The name of the server.
        /// </value>
        string ServerName
        { get; }

        /// <summary>
        /// Gets the time that the Splunk server instance last started up.
        /// </summary>
        /// <value>
        /// The startup time.
        /// </value>
        DateTime StartupTime
        { get; }

        /// <summary>
        /// Gets the version of the Splunk server instance.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        Version Version
        { get; }
    }
}
