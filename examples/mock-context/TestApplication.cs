namespace Splunk.Client.UnitTests
{
    using Splunk.Client;
    using Splunk.Client.Helpers;

    using System;
    using System.Linq;
    using System.Threading.Tasks;
    
    using Xunit;
    
    public class ApplicationTest
    {
        [Trait("acceptance-test", "Splunk.Client.Application")]
        [MockContext]
        [Fact]
        public async Task TestApplications()
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
            }
        }

        async Task CheckApplication(Application app)
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
    }
}
