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

// TODO:
// [ ] Contracts
// [ ] Documentation
// [O] Property accessors should not throw, but return default value if the underlying field is undefined (?)
// [X] Property accessors should throw InvalidOperationException when this ServerInfo.Record is null.

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;

    public sealed class ServerInfo : Entity<ServerInfo>
    {
        #region Constructors

        public ServerInfo(Service service)
            : base(service.Context, service.Namespace, ResourceName.ServerInfo, "server-info")
        { }

        public ServerInfo()
        { }

        #endregion

        #region Properties

        public int Build
        {
            get { return this.GetValue("Build", Int32Converter.Default); }
        }

        public Eai Eai
        {
            get { return this.GetValue("Eai", EaiConverter.Instance); }
        }

        /// <summary>
        /// Gets the <see cref="Guid"/> identifying the current Splunk server instance.
        /// </summary>
        public Guid Guid
        {
            get { return this.GetValue("Guid", GuidConverter.Default); }
        }

        public bool IsFree
        {
            get { return this.GetValue("IsFree", BooleanConverter.Instance); }
        }

        public bool IsRealtimeSearchEnabled
        {
            get { return this.GetValue("RtsearchEnabled", BooleanConverter.Instance); }
        }

        public bool IsTrial
        {
            get { return this.GetValue("IsTrial", BooleanConverter.Instance); }
        }

        public IReadOnlyList<string> LicenseKeys
        {
            get { return this.GetValue("LicenseKeys", CollectionConverter<string, List<string>, StringConverter>.Default); }
        }

        public IReadOnlyList<string> LicenseLabels
        {
            get { return this.GetValue("LicenseLabels", CollectionConverter<string, List<string>, StringConverter>.Default); }
        }

        public string LicenseSignature
        {
            get { return this.GetValue("LicenseSignature", StringConverter.Default); }
        }

        public LicenseState LicenseState
        {
            get { return this.GetValue("LicenseState", EnumConverter<LicenseState>.Default); }
        }

        /// <summary>
        /// Gets the <see cref="Guid"/> identifying the license master for the 
        /// current Splunk server instance.
        /// </summary>
        public Guid MasterGuid
        {
            get { return this.GetValue("MasterGuid", GuidConverter.Default); }
        }

        public ServerMode Mode
        {
            get { return this.GetValue("Mode", EnumConverter<ServerMode>.Default); }
        }

        public string OSBuild
        {
            get { return this.GetValue("OsBuild", StringConverter.Default); }
        }

        public string OSName
        {
            get { return this.GetValue("OsName", StringConverter.Default); }
        }

        public string OSVersion
        {
            get { return this.GetValue("OsVersion", StringConverter.Default); }
        }

        public string CpuArchitecture
        {
            get { return this.GetValue("CpuArch", StringConverter.Default); }
        }

        public string ServerName
        {
            get { return this.GetValue("ServerName", StringConverter.Default); }
        }

        public Version Version
        {
            get { return this.GetValue("Version", VersionConverter.Default); }
        }

        #endregion
    }
}
