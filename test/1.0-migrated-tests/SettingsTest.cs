/*
 * Copyright 2013 Splunk, Inc.
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

namespace Splunk.Sdk.UnitTesting
{
    using Splunk.Client;
    using Splunk.Client.UnitTesting;

    using StyleCop;

    using Xunit;

    /// <summary>
    /// This is the settingstest class
    /// </summary>
    public class SettingsTest : TestHelper
    {
        /// <summary>
        /// This method tests geting the events and then sets most, 
        /// and then reverts back to the original
        /// </summary>
        [Fact]
        public void Settings()
        {
            Service service = Connect();

            ServerSettings settings = service.Server.GetSettingsAsync().Result;
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

            // set aside original settings
            string originalTimeout = settings.SessionTimeout;
            bool originalSSL = settings.EnableSplunkWebSsl;
            string originalHost = settings.Host;
            int originalHttpPort = settings.HttpPort;
            int originalMinSpace = settings.MinFreeSpace;
            //int originalMgmtPort = settings.MgmtPort();
            string originalServerName = settings.ServerName;
            bool originalStartWeb = settings.StartWebServer;

            // test update
            ServerSettingValues serverSettingValues = new ServerSettingValues();
            serverSettingValues.EnableSplunkWebSsl = !originalSSL;
            bool updatedSSL = serverSettingValues.EnableSplunkWebSsl.Value;
            serverSettingValues.Host = "sdk-host";
            serverSettingValues.HttpPort = 8001;
            serverSettingValues.MinFreeSpace = originalMinSpace - 100;
            //settings.MgmtHostPort(originalMgmtPort+1);
            serverSettingValues.ServerName = "sdk-test-name";
            serverSettingValues.SessionTimeout = "2h";
            //settings.StartWebServer(!originalStartWeb);
            settings.UpdateAsync(serverSettingValues).Wait();

            // changing ports require a restart
            this.SplunkRestart();
            service = this.Connect();
            settings = service.Server.GetSettingsAsync().Result;

            Assert.NotEqual(originalSSL, updatedSSL);
            Assert.Equal("sdk-host", settings.Host);
            Assert.Equal(8001, settings.HttpPort);
            Assert.Equal(originalMinSpace - 100, settings.MinFreeSpace);
            Assert.Equal("sdk-test-name", settings.ServerName);
            Assert.Equal("2h", settings.SessionTimeout);
            //assertEquals(settings.StartWebServer(), !originalStartWeb);

            // restore original
            serverSettingValues = new ServerSettingValues();
            serverSettingValues.EnableSplunkWebSsl = originalSSL;
            serverSettingValues.Host = originalHost;
            serverSettingValues.HttpPort = originalHttpPort;
            serverSettingValues.MinFreeSpace = originalMinSpace;
            serverSettingValues.ServerName = originalServerName;
            serverSettingValues.SessionTimeout = originalTimeout;
            serverSettingValues.StartWebServer = originalStartWeb;
            settings.UpdateAsync(serverSettingValues).Wait();

            // changing ports require a restart
            this.SplunkRestart();
            service = this.Connect();
            settings = service.Server.GetSettingsAsync().Result;

            Assert.Equal(originalSSL, settings.EnableSplunkWebSsl);
            Assert.Equal(originalHost, settings.Host);
            Assert.Equal(originalHttpPort, settings.HttpPort);
            Assert.Equal(originalMinSpace, settings.MinFreeSpace);
            Assert.Equal(originalServerName, settings.ServerName);
            Assert.Equal(originalTimeout, settings.SessionTimeout);
            Assert.Equal(originalStartWeb, settings.StartWebServer);
        }
    }
}
