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

namespace Splunk.Client.UnitTests
{
    using Splunk.Client;
    using Splunk.Client.Helpers;

    using System;
    using System.Linq;
    using System.Threading.Tasks;
    
    using Xunit;
    
    /// <summary>
    /// Application tests
    /// </summary>
    public class ApplicationTest
    {
        /// <summary>
        /// The assert root string
        /// </summary>
        private static string assertRoot = "Application assert: ";

        /// <summary>
        /// The app tests
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Application")]
        [Fact]
        public async Task Application()
        {
            string dummyString;
            bool dummyBool;

            Service service = await SDKHelper.CreateService();

            ApplicationCollection apps = service.Applications;
            await apps.GetAllAsync();

            foreach (Application app in apps)
            {
                try
                {
                    ApplicationSetupInfo setupInfo = await app.GetSetupInfoAsync();
                    //string setupXml = applicationSetup.SetupXml;
                }
                catch (Exception e)
                {
                    // silent exception, if setup doesn't exist, exception occurs
                }

                ApplicationArchiveInfo applicationArchive = app.PackageAsync().Result;
                dummyString = app.Author;
                dummyBool = app.CheckForUpdates;
                dummyString = app.Description;
                dummyString = app.Label;
                dummyBool = app.Refresh;
                dummyString = app.Version;
                dummyBool = app.Configured;

                if (TestHelper.VersionCompare(service, "5.0") < 0)
                {
                    //dummyBool = app.IsManageable;
                }

                dummyBool = app.Visible;
                dummyBool = app.StateChangeRequiresRestart;

                ApplicationUpdateInfo applicationUpdate = await app.GetUpdateInfoAsync();

                if (applicationUpdate.Update != null)
                {
                    dummyString = applicationUpdate.Update.Checksum;
                    dummyString = applicationUpdate.Update.ChecksumType;
                    dummyString = applicationUpdate.Update.HomePage;
                    long size = applicationUpdate.Update.Size;
                    dummyString = applicationUpdate.Update.ApplicationName;
                    Uri uri = applicationUpdate.Update.ApplicationUri;
                    dummyString = applicationUpdate.Update.Version;
                    dummyBool = applicationUpdate.Update.ImplicitIdRequired;
                }
            }

            if (apps.Any(a => a.Name == "sdk-tests"))
            {
                await TestHelper.RemoveApp("sdk-tests");
                service = await SDKHelper.CreateService();
            }

            apps = service.Applications;
            await apps.GetAllAsync();

            Assert.False(apps.Any(a => a.Name == "sdk-tests"), assertRoot + "#1");

            ApplicationAttributes createArgs = new ApplicationAttributes();
            createArgs.Author = "me";

            if (TestHelper.VersionCompare(service, "4.2.4") >= 0)
            {
                createArgs.Configured = false;
            }

            createArgs.Description = "this is a description";
            createArgs.Label = "SDKTEST";

            if (TestHelper.VersionCompare(service, "5.0") < 0)
            {
                //createArgs.manageable", false);
            }

            createArgs.Visible = false;

            var testApp = await service.Applications.CreateAsync("sdk-tests", "barebones", createArgs);

            Assert.DoesNotThrow(() =>
            {
                var app = service.Applications.GetAsync("sdk-tests").Result;
                dummyBool = app.CheckForUpdates;
                
                Assert.Equal("SDKTEST", app.Label);
                Assert.Equal("me", app.ApplicationAuthor);
                Assert.Equal("nobody", app.Author);
                Assert.False(app.Configured);
                Assert.False(app.Visible);

                ApplicationAttributes attributes = new ApplicationAttributes()
                {
                    Author = "not me",
                    Description = "new description",
                    Label = "new label",
                    Visible = false,
                    Version = "1.5"
                };

                //// Update the application

                app.UpdateAsync(attributes, true).Wait();
                app.GetAsync().Wait();

                Assert.Equal("not me", app.ApplicationAuthor);
                Assert.Equal("nobody", app.Author);
                Assert.Equal("new description", app.Description);
                Assert.Equal("new label", app.Label);
                Assert.Equal("1.5", app.Version);
                Assert.False(app.Visible);

                ApplicationUpdateInfo updateInfo = app.GetUpdateInfoAsync().Result;
                Assert.NotNull(updateInfo.Eai.Acl);

                //// Package the application

                ApplicationArchiveInfo archiveInfo = app.PackageAsync().Result;

                Assert.True(archiveInfo.ApplicationName.Length > 0);
                Assert.True(archiveInfo.Path.Length > 0);
                Assert.True(archiveInfo.Uri.AbsolutePath.Length > 0);
            });

            await TestHelper.RemoveApp("sdk-tests");

            //// TODO: Incorporate or remove these bits

            if (TestHelper.VersionCompare(service, "5.0") < 0)
            {
                //Assert.False(app2.Manageable, assertRoot + "#6");
            }

            // update the app

            if (TestHelper.VersionCompare(service, "5.0") < 0)
            {
                //app2.IsManageable = false;
            }
        }
    }
}
