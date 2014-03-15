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
// [ ] Property accessors should not throw, but return default value if the underlying field is undefined (?)
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

        public Acl Acl
        {
            get
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                var value = this.Record.Eai.Acl as Acl;

                if (value == null)
                {
                    value = new Acl(this.Record.Eai.Acl);
                    this.Record.Eai.Acl = value;
                }

                return value;
            }
        }

        public int Build
        {
            get { return this.GetValue("Build", Int32Converter.Default); }
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
            get { return this.GetValue("IsFree", BooleanConverter.Default); }
        }

        public bool IsRealtimeSearchEnabled
        {
            get { return this.GetValue("RtsearchEnabled", BooleanConverter.Default); }
        }

        public bool IsTrial
        {
            get { return this.GetValue("IsTrial", BooleanConverter.Default); }
        }

        public IReadOnlyList<string> LicenseKeys
        {
            get { return this.GetValue("LicenseKeys", ListConverter<string, StringConverter>.Default); }
        }

        public IReadOnlyList<string> LicenseLabels
        {
            get { return this.GetValue("LicenseLabels", ListConverter<string, StringConverter>.Default); }
        }

        public string LicenseSignature
        {
            get 
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                return this.Record.LicenseSignature; 
            }
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
            get
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);

                if (!(this.Record.Mode is ServerMode))
                {
                    switch ((string)this.Record.Mode)
                    {
                        default:
                            this.Record.Mode = ServerMode.Unknown;
                            break;
                        case "normal":
                            this.Record.Mode = ServerMode.Normal;
                            break;
                        case "dedicated forwarder":
                            this.Record.Mode = ServerMode.DedicatedForwarder;
                            break;
                    }
                }
                return this.Record.Mode;
            }
        }

        public string OSBuild
        {
            get 
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                return this.Record.OsBuild; 
            }
        }

        public string OSName
        {
            get 
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                return this.Record.OsName; 
            }
        }

        public string OSVersion
        {
            get 
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                return this.Record.OsVersion;  
            }
        }

        public string CpuArchitecture
        {
            get 
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                return this.Record.CpuArch; 
            }
        }

        public string ServerName
        {
            get 
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                return this.Record.ServerName; 
            }
        }

        public Version Version
        {
            get { return this.GetValue("Version", VersionConverter.Default); }
        }

        #endregion
    }
}
