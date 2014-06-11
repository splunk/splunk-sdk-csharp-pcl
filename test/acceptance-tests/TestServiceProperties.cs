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

namespace Splunk.Client.UnitTests
{
    using Splunk.Client;
    using Splunk.Client.Helpers;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    
    using Xunit;
    using System.Threading.Tasks;

    /// <summary>
    /// This class tests all the Splunk Service methods.
    /// </summary>
    public class ServiceTest
    {
        /// <summary>
        /// Tests the getting of service info (there are no set arguments)
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.ServiceInfo")]
        [Fact]
        public async Task ServiceInfo()
        {

            using (var service = await SDKHelper.CreateService())
            {
                ServerInfo info = await service.Server.GetInfoAsync();

                bool dummyBool;
                int dummyInt;
                //string[] dummyStrings;
                string dummyString;

                dummyInt = info.Build;
                dummyString = info.CpuArchitecture;
                Guid guid = info.Guid;
                IReadOnlyList<string> licents = info.LicenseKeys;
                IReadOnlyList<string> licentLabel = info.LicenseLabels;
                dummyString = info.LicenseSignature;
                LicenseState state = info.LicenseState;
                Guid guid2 = info.MasterGuid;
                ServerMode mode = info.Mode;
                dummyString = info.OSBuild;
                dummyString = info.OSName;
                dummyString = info.OSVersion;
                dummyString = info.ServerName;
                Version version = info.Version;
                dummyBool = info.IsFree;
                dummyBool = info.IsRealtimeSearchEnabled;
                dummyBool = info.IsTrial;
            }
        }

        /// <summary>
        /// This method tests geting the events and then sets most, 
        /// and then reverts back to the original
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.ServerSettings")]
        [Fact]
        public async Task Settings()
        {
            ServerSettingValues original = null, replacement = null;

            using (var service = await SDKHelper.CreateService())
            {
                ServerSettings settings = await service.Server.GetSettingsAsync();
                string dummyString;
                bool dummBool;
                int dummyInt;

                dummyString = settings.SplunkDB;
                dummyString = settings.SplunkHome;
                dummBool = settings.EnableSplunkWebSsl;
                dummyString = settings.Host;
                dummyInt = settings.HttpPort;
                dummyInt = settings.ManagementHostPort;
                dummyInt = settings.MinFreeSpace;
                dummyString = settings.Pass4SymmetricKey;
                dummyString = settings.ServerName;
                dummyString = settings.SessionTimeout;
                dummBool = settings.StartWebServer;
                dummyString = settings.TrustedIP;

                //// Set aside original settings

                original = new ServerSettingValues()
                {
                    EnableSplunkWebSsl = settings.EnableSplunkWebSsl,
                    Host = settings.Host,
                    //HttpPort = settings.HttpPort,
                    //ManagementHostPort = settings.ManagementHostPort,
                    MinFreeSpace = settings.MinFreeSpace,
                    ServerName = settings.ServerName,
                    SessionTimeout = settings.SessionTimeout,
                    StartWebServer = settings.StartWebServer
                };

                //// Update
                
                replacement = new ServerSettingValues()
                {
                    EnableSplunkWebSsl = !original.EnableSplunkWebSsl,
                    Host = "sdk-test",
                    //HttpPort = 8001,
                    //ManagementHostPort = original.ManagementHostPort + 1,
                    MinFreeSpace = original.MinFreeSpace - 100,
                    ServerName = "sdk-test-name",
                    SessionTimeout = "2h",
                    StartWebServer = !original.StartWebServer
                };

                await service.Server.UpdateSettingsAsync(replacement);
                //await TestHelper.RestartServerAsync(); // because changing ports requires a restart
            }

            using (var service = await SDKHelper.CreateService())
            {
                ServerSettings settings = await service.Server.GetSettingsAsync();

                //// Verify update

                Assert.Equal(replacement.EnableSplunkWebSsl, settings.EnableSplunkWebSsl);
                Assert.Equal(replacement.Host, settings.Host);
                //Assert.Equal(replacement.HttpPort, settings.HttpPort);
                //Assert.Equal(replacement.ManagementHostPort, settings.ManagementHostPort);
                Assert.Equal(replacement.MinFreeSpace, settings.MinFreeSpace);
                Assert.Equal(replacement.ServerName, settings.ServerName);
                Assert.Equal(replacement.SessionTimeout, settings.SessionTimeout);
                Assert.Equal(replacement.StartWebServer, settings.StartWebServer);

                //// Restore original settings

                replacement = new ServerSettingValues()
                {
                    EnableSplunkWebSsl = original.EnableSplunkWebSsl,
                    Host = original.Host,
                    //HttpPort = original.HttpPort,
                    //ManagementHostPort = original.ManagementHostPort,
                    MinFreeSpace = original.MinFreeSpace,
                    ServerName = original.ServerName,
                    SessionTimeout = original.SessionTimeout,
                    StartWebServer = original.StartWebServer
                };

                await service.Server.UpdateSettingsAsync(replacement);
                //await TestHelper.RestartServerAsync(); // because changing ports requires a restart
            }

            using (var service = await SDKHelper.CreateService())
            {
                //// Verify restore operation

                ServerSettings settings = await service.Server.GetSettingsAsync();

                Assert.Equal(original.EnableSplunkWebSsl, settings.EnableSplunkWebSsl);
                Assert.Equal(original.Host, settings.Host);
                //Assert.Equal(original.HttpPort, settings.HttpPort);
                //Assert.Equal(original.ManagementHostPort, settings.ManagementHostPort);
                Assert.Equal(original.MinFreeSpace, settings.MinFreeSpace);
                Assert.Equal(original.ServerName, settings.ServerName);
                Assert.Equal(original.SessionTimeout, settings.SessionTimeout);
                Assert.Equal(original.StartWebServer, settings.StartWebServer);

                //await TestHelper.RestartServerAsync(); // because changing ports requires a restart
            }
        }
    }
}
