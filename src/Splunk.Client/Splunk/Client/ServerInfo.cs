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
//// [X] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides information about a Splunk server instance.
    /// </summary>
    public sealed class ServerInfo : Entity<ServerInfo>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerInfo"/> class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        internal ServerInfo(Context context, Namespace ns)
            : base(context, ns, ClassResourceName)
        { }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "ServerInfo"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. Use the <see cref=
        /// "Server.GetInfoAsync"/> method to asynchronously retrieve <see 
        /// cref="ServerInfo"/>.
        /// </remarks>
        public ServerInfo()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the active license group for the Splunk server
        /// instance.
        /// </summary>
        public string ActiveLicenseGroup
        {
            get { return this.GetValue("ActiveLicenseGroup", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the add ons installed on the Splunk server instance.
        /// </summary>
        public dynamic AddOns
        {
            get { return this.GetValue("AddOns"); }
        }

        /// <summary>
        /// Gets the build number for the Splunk server instance.
        /// </summary>
        public int Build
        {
            get { return this.GetValue("Build", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets CPU architecture information for the computer hosting splunkd.
        /// </summary>
        public string CpuArchitecture
        {
            get { return this.GetValue("CpuArch", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the access control lists for the Splunk server instance.
        /// </summary>
        public Eai Eai
        {
            get { return this.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <summary>
        /// Gets the <see cref="Guid"/> identifying the Splunk server instance.
        /// </summary>
        public Guid Guid
        {
            get { return this.GetValue("Guid", GuidConverter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether the server instance is running 
        /// under a free license.
        /// </summary>
        public bool IsFree
        {
            get { return this.GetValue("IsFree", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether the server instance is realtime
        /// search enabled.
        /// </summary>
        public bool IsRealtimeSearchEnabled
        {
            get { return this.GetValue("RtsearchEnabled", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether the server instance is running 
        /// under a trial license.
        /// </summary>
        public bool IsTrial
        {
            get { return this.GetValue("IsTrial", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the list of license keys installed on the server instance.
        /// </summary>
        public IReadOnlyList<string> LicenseKeys
        {
            get { return this.GetValue("LicenseKeys", CollectionConverter<string, List<string>, StringConverter>.Instance); }
        }

        /// <summary>
        /// Gets the list of labels for the license keys installed on the
        /// server instance.
        /// </summary>
        public IReadOnlyList<string> LicenseLabels
        {
            get { return this.GetValue("LicenseLabels", CollectionConverter<string, List<string>, StringConverter>.Instance); }
        }

        /// <summary>
        /// Gets the has signature for the license keys installed on the server
        /// instance.
        /// </summary>
        public string LicenseSignature
        {
            get { return this.GetValue("LicenseSignature", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the status of the licenses installed on the serve instance.
        /// </summary>
        public LicenseState LicenseState
        {
            get { return this.GetValue("LicenseState", EnumConverter<LicenseState>.Instance); }
        }

        /// <summary>
        /// Gets the <see cref="Guid"/> identifying the license master for the 
        /// Splunk server instance.
        /// </summary>
        public Guid MasterGuid
        {
            get { return this.GetValue("MasterGuid", GuidConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that specifies whether the Splunk server is a dedicated 
        /// forwarder or a normal instance.
        /// </summary>
        public ServerMode Mode
        {
            get { return this.GetValue("Mode", EnumConverter<ServerMode>.Instance); }
        }

        /// <summary>
        /// Gets the number of processor cores installed on the Splunk server
        /// instance.
        /// </summary>
        public int NumberOfCores
        {
            get { return this.GetValue("NumberOfCores", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets build information for the operating system running splunkd.
        /// </summary>
        public string OSBuild
        {
            get { return this.GetValue("OsBuild", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the name of the operating system running splunkd.
        /// </summary>
        public string OSName
        {
            get { return this.GetValue("OsName", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets version information for the operating system running splunkd.
        /// </summary>
        public string OSVersion
        {
            get { return this.GetValue("OsVersion", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the number of megabytes of physical memory installed on the 
        /// Splunk server instance.
        /// </summary>
        public long PhysicalMemoryMB
        {
            get { return this.GetValue("PhysicalMemoryMB", Int64Converter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates whether realtime search is enabled on
        /// the Splunk server instance.
        /// </summary>
        public bool RealtimeSearchEnabled
        {
            get { return this.GetValue("RtsearchEnabled", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the fully qualified name of the Splunk server instance.
        /// </summary>
        public string ServerName
        {
            get { return this.GetValue("ServerName", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the time that the Splunk server instance last started up.
        /// </summary>
        public DateTime StartupTime
        {
            get { return this.GetValue("StartupTime", UnixDateTimeConverter.Instance); }
        }

        /// <summary>
        /// Gets the version of the Splunk server instance.
        /// </summary>
        public Version Version
        {
            get { return this.GetValue("Version", VersionConverter.Instance); }
        }

        #endregion

        #region Privates/internals

        internal static readonly ResourceName ClassResourceName = new ResourceName("server", "info");

        #endregion
    }
}
