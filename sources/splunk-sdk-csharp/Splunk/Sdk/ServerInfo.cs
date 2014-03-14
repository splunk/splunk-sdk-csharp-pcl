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
            get 
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                var x = this.Record.Build as int?;

                if (x == null)
                {
                    x = int.Parse(this.Record.Build.ToString());
                    this.Record.Build = x.Value;
                }

                return x.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="Guid"/> identifying the current Splunk server instance.
        /// </summary>
        public Guid Guid
        {
            get 
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                var x = this.Record.Guid as Guid?;

                if (x == null)
                {
                    x = System.Guid.Parse(this.Record.Guid.ToString());
                    this.Record.Guid = x.Value;
                }

                return x.Value;
            }
        }

        public bool IsFree
        {
            get
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                var x = this.Record.IsFree as bool?;

                if (x == null)
                {
                    x = int.Parse(this.Record.IsFree.ToString()) != 0;
                    this.Record.IsFree = x.Value;
                }

                return x.Value;
            }
        }

        public bool IsRealTimeSearchEnabled
        {
            get
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                var x = this.Record.RtsearchEnabled as bool?;

                if (x == null)
                {
                    x = int.Parse(this.Record.RtsearchEnabled.ToString()) != 0;
                    this.Record.RtsearchEnabled = x.Value;
                }

                return x.Value;
            }
        }

        public bool IsTrial
        {
            get
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                var x = this.Record.IsTrial as bool?;

                if (x == null)
                {
                    x = int.Parse(this.Record.IsTrial.ToString()) != 0;
                    this.Record.IsTrial = x.Value;
                }
                return x.Value;
            }
        }

        public IReadOnlyList<string> LicenseKeys
        {
            get 
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                var value = this.Record.LicenseKeys as IReadOnlyList<string>;

                if (value == null)
                {
                    value = new List<string>(from licenseKey in (IEnumerable<object>)this.Record.LicenseKeys select licenseKey.ToString());
                    this.Record.LicenseKeys = value;
                }

                return value;
            }
        }

        public IReadOnlyList<string> LicenseLabels
        {
            get
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                var value = this.Record.LicenseLabels as IReadOnlyList<string>;

                if (value == null)
                {
                    value = new List<string>(from licenseLabels in (IEnumerable<object>)this.Record.LicenseLabels select licenseLabels.ToString());
                    this.Record.LicenseLabels = value;
                }

                return value;
            }
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
            get 
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);

                if (!(this.Record.LicenseState is LicenseState))
                {
                    switch ((string)this.Record.Mode)
                    {
                        default:
                            this.Record.LicenseState = LicenseState.Unknown;
                            break;
                        case "OK":
                            this.Record.LicenseState = LicenseState.OK;
                            break;
                        case "Expired":
                            this.Record.LicenseState = LicenseState.Expired;
                            break;
                    }
                }
                return this.Record.LicenseState; 
            }
        }

        /// <summary>
        /// Gets the <see cref="Guid"/> identifying the license master for the 
        /// current Splunk server instance.
        /// </summary>
        public Guid MasterGuid
        {
            get
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                var x = this.Record.MasterGuid as Guid?;

                if (x == null)
                {
                    x = System.Guid.Parse(this.Record.MasterGuid.ToString());
                    this.Record.MasterGuid = x.Value;
                }

                return x.Value;
            }
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
            get 
            {
                Contract.Requires<InvalidOperationException>((object)this.Record != null);
                var value = this.Record.Version as Version;

                if (value == null)
                {
                    value = System.Version.Parse(this.Record.Version);
                    this.Record.Version = value;
                }

                return value;
            }
        }

        #endregion
    }
}
