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
    using System.IO;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides information about a Splunk server instance.
    /// </summary>
    public class ServerInfo : Resource, IServerInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerInfo"/> class.
        /// </summary>
        /// <param name="entry">
        /// An object representing a Splunk atom entry response.
        /// </param>
        /// <param name="generatorVersion">
        /// The version of the generator producing the <see cref="AtomFeed"/>
        /// feed containing <paramref name="entry"/>.
        /// </param>
        protected internal ServerInfo(AtomEntry entry, Version generatorVersion)
            : base(entry, generatorVersion)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerInfo"/> class.
        /// </summary>
        /// <param name="feed">
        /// An object representing a Splunk atom feed response.
        /// </param>
        protected internal ServerInfo(AtomFeed feed)
        {
            this.Initialize(feed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerInfo"/> class.
        /// </summary>
        /// <param name="other">
        /// Another resource.
        /// </param>
        protected internal ServerInfo(BaseResource other)
        {
            this.Initialize(other);
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "BaseResource"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. 
        /// </remarks>
        public ServerInfo()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the active license group for the Splunk server
        /// instance.
        /// </summary>
        public virtual string ActiveLicenseGroup
        {
            get { return this.Content.GetValue("ActiveLicenseGroup", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the add ons installed on the Splunk server instance.
        /// </summary>
        public virtual dynamic AddOns
        {
            get { return this.Content.GetValue("AddOns", ExpandoAdapter.Converter.Instance); }
        }

        /// <summary>
        /// Gets the build number for the Splunk server instance.
        /// </summary>
        public virtual int Build
        {
            get { return this.Content.GetValue("Build", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets CPU architecture information for the computer hosting splunkd.
        /// </summary>
        public virtual string CpuArchitecture
        {
            get { return this.Content.GetValue("CpuArch", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the access control lists for the Splunk server instance.
        /// </summary>
        public virtual Eai Eai
        {
            get { return this.Content.GetValue("Eai", Eai.Converter.Instance); }
        }

        /// <summary>
        /// Gets the <see cref="Guid"/> identifying the Splunk server instance.
        /// </summary>
        public virtual Guid Guid
        {
            get { return this.Content.GetValue("Guid", GuidConverter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether the server instance is running 
        /// under a free license.
        /// </summary>
        public virtual bool IsFree
        {
            get { return this.Content.GetValue("IsFree", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether the server instance is realtime
        /// search enabled.
        /// </summary>
        public virtual bool IsRealtimeSearchEnabled
        {
            get { return this.Content.GetValue("RtsearchEnabled", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets a value indicating whether the server instance is running 
        /// under a trial license.
        /// </summary>
        public virtual bool IsTrial
        {
            get { return this.Content.GetValue("IsTrial", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the list of license keys installed on the server instance.
        /// </summary>
        public virtual IReadOnlyList<string> LicenseKeys
        {
            get { return this.Content.GetValue("LicenseKeys", CollectionConverter<string, List<string>, StringConverter>.Instance); }
        }

        /// <summary>
        /// Gets the list of labels for the license keys installed on the
        /// server instance.
        /// </summary>
        public virtual IReadOnlyList<string> LicenseLabels
        {
            get { return this.Content.GetValue("LicenseLabels", CollectionConverter<string, List<string>, StringConverter>.Instance); }
        }

        /// <summary>
        /// Gets the has signature for the license keys installed on the server
        /// instance.
        /// </summary>
        public virtual string LicenseSignature
        {
            get { return this.Content.GetValue("LicenseSignature", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the status of the licenses installed on the serve instance.
        /// </summary>
        public virtual LicenseState LicenseState
        {
            get { return this.Content.GetValue("LicenseState", EnumConverter<LicenseState>.Instance); }
        }

        /// <summary>
        /// Gets the <see cref="Guid"/> identifying the license master for the 
        /// Splunk server instance.
        /// </summary>
        public virtual Guid MasterGuid
        {
            get { return this.Content.GetValue("MasterGuid", GuidConverter.Instance); }
        }

        /// <summary>
        /// Gets a value that specifies whether the Splunk server is a dedicated 
        /// forwarder or a normal instance.
        /// </summary>
        public virtual ServerMode Mode
        {
            get { return this.Content.GetValue("Mode", EnumConverter<ServerMode>.Instance); }
        }

        /// <summary>
        /// Gets the number of processor cores installed on the Splunk server
        /// instance.
        /// </summary>
        public virtual int NumberOfCores
        {
            get { return this.Content.GetValue("NumberOfCores", Int32Converter.Instance); }
        }

        /// <summary>
        /// Gets build information for the operating system running splunkd.
        /// </summary>
        public virtual string OSBuild
        {
            get { return this.Content.GetValue("OsBuild", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the name of the operating system running splunkd.
        /// </summary>
        public virtual string OSName
        {
            get { return this.Content.GetValue("OsName", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets version information for the operating system running splunkd.
        /// </summary>
        public virtual string OSVersion
        {
            get { return this.Content.GetValue("OsVersion", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the number of megabytes of physical memory installed on the 
        /// Splunk server instance.
        /// </summary>
        public virtual long PhysicalMemoryMB
        {
            get { return this.Content.GetValue("PhysicalMemoryMB", Int64Converter.Instance); }
        }

        /// <summary>
        /// Gets a value that indicates whether realtime search is enabled on
        /// the Splunk server instance.
        /// </summary>
        public virtual bool RealtimeSearchEnabled
        {
            get { return this.Content.GetValue("RtsearchEnabled", BooleanConverter.Instance); }
        }

        /// <summary>
        /// Gets the fully qualified name of the Splunk server instance.
        /// </summary>
        public virtual string ServerName
        {
            get { return this.Content.GetValue("ServerName", StringConverter.Instance); }
        }

        /// <summary>
        /// Gets the time that the Splunk server instance last started up.
        /// </summary>
        public virtual DateTime StartupTime
        {
            get { return this.Content.GetValue("StartupTime", UnixDateTimeConverter.Instance); }
        }

        /// <summary>
        /// Gets the version of the Splunk server instance.
        /// </summary>
        public virtual Version Version
        {
            get { return this.Content.GetValue("Version", VersionConverter.Instance); }
        }

        #endregion
    }
}
