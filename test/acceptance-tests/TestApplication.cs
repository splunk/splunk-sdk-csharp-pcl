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
        /// The app tests
        /// </summary>
        [Trait("acceptance-test", "Splunk.Client.Application")]
        [MockContext]
        [Fact]
        public async Task Application()
        {
            using (var service = await SdkHelper.CreateService())
            {
                ApplicationCollection apps = service.Applications;
                await apps.GetAllAsync();

                foreach (Application app in apps)
                {
                    await CheckApplication(app);
                }

                for (int i = 0; i < apps.Count; i++)
                {
                    await CheckApplication(apps[i]);
                }

                if (await service.Applications.RemoveAsync("sdk-tests"))
                {
                    await service.Server.RestartAsync(2 * 60 * 1000);
                    await service.LogOnAsync();
                }

                ApplicationAttributes attributes = new ApplicationAttributes
                {
                    ApplicationAuthor = "me",
                    Description = "this is a description",
                    Label = "SDKTEST",
                    Visible = false
                };

                var testApp = await service.Applications.CreateAsync("sdk-tests", "barebones", attributes);
                testApp = await service.Applications.GetAsync("sdk-tests");

                Assert.Equal("SDKTEST", testApp.Label);
                Assert.Equal("me", testApp.ApplicationAuthor);
                Assert.Equal("nobody", testApp.Author);
                Assert.False(testApp.Configured);
                Assert.False(testApp.Visible);

                Assert.DoesNotThrow(() =>
                {
                    bool p = testApp.CheckForUpdates;
                });

                attributes = new ApplicationAttributes
                {
                    ApplicationAuthor = "not me",
                    Description = "new description",
                    Label = "new label",
                    Visible = false,
                    Version = "1.5"
                };

                //// Update the application

                await testApp.UpdateAsync(attributes, true);
                await testApp.GetAsync();

                Assert.Equal("not me", testApp.ApplicationAuthor);
                Assert.Equal("nobody", testApp.Author);
                Assert.Equal("new description", testApp.Description);
                Assert.Equal("new label", testApp.Label);
                Assert.Equal("1.5", testApp.Version);
                Assert.False(testApp.Visible);

                ApplicationUpdateInfo updateInfo = await testApp.GetUpdateInfoAsync();
                Assert.NotNull(updateInfo.Eai.Acl);

                //// Package the application

                ApplicationArchiveInfo archiveInfo = await testApp.PackageAsync();
            
                Assert.Equal("Package", archiveInfo.Title);
                Assert.NotEqual(DateTime.MinValue, archiveInfo.Updated);

                Assert.DoesNotThrow(() =>
                {
                    string p = archiveInfo.ApplicationName;
                });
                Assert.True(archiveInfo.ApplicationName.Length > 0);

                Assert.DoesNotThrow(() =>
                {
                    Eai p = archiveInfo.Eai;
                });
                Assert.NotNull(archiveInfo.Eai);
                Assert.NotNull(archiveInfo.Eai.Acl);

                Assert.DoesNotThrow(() =>
                {
                    string p = archiveInfo.Path;
                });
                Assert.True(archiveInfo.Path.Length > 0);

                Assert.DoesNotThrow(() =>
                {
                    bool p = archiveInfo.Refresh;
                });

                Assert.DoesNotThrow(() =>
                {
                    Uri p = archiveInfo.Uri;
                });
                Assert.True(archiveInfo.Uri.AbsolutePath.Length > 0);

                Assert.True(await service.Applications.RemoveAsync("sdk-tests"));
                await service.Server.RestartAsync(2 * 60 * 1000);
            }
        }

        #region Privates/internals

        internal async Task CheckApplication(Application app)
        {
            ApplicationSetupInfo setupInfo = null;

            try 
            {
                setupInfo = await app.GetSetupInfoAsync();

                //// TODO: Install an app which hits this code before this test runs

                Assert.NotNull(setupInfo.Eai);
                Assert.DoesNotThrow(() => { bool p = setupInfo.Refresh; });
            } 
            catch (InternalServerErrorException e) 
            {
                Assert.Contains("Setup configuration file does not exist", e.Message);
            }

            ApplicationArchiveInfo archiveInfo = await app.PackageAsync();

            Assert.DoesNotThrow(() => 
            { 
                string p = app.Author; 
                Assert.NotNull(p);
            });

            Assert.DoesNotThrow(() => { string p = app.ApplicationAuthor; });
            Assert.DoesNotThrow(() => { bool p = app.CheckForUpdates; });
            Assert.DoesNotThrow(() => { string p = app.Description; });
            Assert.DoesNotThrow(() => { string p = app.Label; });
            Assert.DoesNotThrow(() => { bool p = app.Refresh; });
            Assert.DoesNotThrow(() => { string p = app.Version; });
            Assert.DoesNotThrow(() => { bool p = app.Configured; });
            Assert.DoesNotThrow(() => { bool p = app.StateChangeRequiresRestart; });
            Assert.DoesNotThrow(() => { bool p = app.Visible; });

            ApplicationUpdateInfo updateInfo = await app.GetUpdateInfoAsync();
            Assert.NotNull(updateInfo.Eai);

            if (updateInfo.Update != null)
            {
                var update = updateInfo.Update;

                Assert.DoesNotThrow(() =>
                {
                    string p = updateInfo.Update.ApplicationName;
                });
                Assert.DoesNotThrow(() =>
                {
                    Uri p = updateInfo.Update.ApplicationUri;
                });
                Assert.DoesNotThrow(() =>
                {
                    string p = updateInfo.Update.ApplicationName;
                });
                Assert.DoesNotThrow(() =>
                {
                    string p = updateInfo.Update.ChecksumType;
                });
                Assert.DoesNotThrow(() =>
                {
                    string p = updateInfo.Update.Homepage;
                });
                Assert.DoesNotThrow(() =>
                {
                    bool p = updateInfo.Update.ImplicitIdRequired;
                });
                Assert.DoesNotThrow(() =>
                {
                    long p = updateInfo.Update.Size;
                });
                Assert.DoesNotThrow(() =>
                {
                    string p = updateInfo.Update.Version;
                });
            }

            Assert.DoesNotThrow(() => { DateTime p = updateInfo.Updated; });
        }

        #endregion
    }
}
