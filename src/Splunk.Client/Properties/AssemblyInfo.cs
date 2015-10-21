﻿/*
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

using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

//// General Information

[assembly: AssemblyProduct("splunk-sdk-csharp-pcl")]
[assembly: AssemblyTitle("Splunk.Client")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Splunk, Inc. - GitHub custom build")]
[assembly: AssemblyCopyright("Copyright © 2015")]
[assembly: AssemblyTrademark("Splunk® - GitHub custom build")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en-US")]

//// Operational information

[assembly: ComVisible(false)]

[assembly: InternalsVisibleTo("Splunk.Client.Helpers")]
[assembly: InternalsVisibleTo("acceptance-tests")]
[assembly: InternalsVisibleTo("unit-tests")]

//// Version information

[assembly: AssemblyVersion("2.2.2.*")]
