namespace Proposal
{
    using Splunk.Client;
    using Splunk.Client.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        public async Task AsIs()
        {
            using (var service = await TestHelper.CreateService())
            {
                var collection = await service.GetApplicationsAsync();
                await collection.ReloadAsync(); // Instructs Splunk to reload all application state
                await collection.GetAsync();    // Updates cached content

                foreach (var application in collection)
                {
                    if (application.Name == "twitter2")
                    {
                        await application.RemoveAsync(); // Removes the twitter2 application, but does not update the application entity's cached content

                        try
                        {
                            await application.GetAsync();
                        }
                        catch (ResourceNotFoundException)
                        {
                            // As expected because the application no longer exists
                        }

                        var path = Path.Combine(Environment.CurrentDirectory, "Data", "app-for-twitter-data_230.spl");

                        await application.InstallAsync(path, update: true); // incorrectly implemented; should be changed; should be on applications
                        await service.InstallApplicationAsync("twitter2", path, update: true); // better

                        var applicationSetupInfo = await application.GetSetupInfoAsync();
                        var applicationUpdateInfo = await application.GetUpdateInfoAsync();

                        await application.RemoveAsync(); // confusing
                        await service.RemoveApplicationAsync("twitter2"); // better
                    }
                }

                //// Create an app from one of the built-in templates

                var name = string.Format("delete-me-{0:N}", Guid.NewGuid());

                var creationAttributes = new ApplicationAttributes()
                {
                    ApplicationAuthor = "Splunk",
                    Configured = true,
                    Description = "This app confirms that an app can be created from a template",
                    Label = name,
                    Version = "2.0.0",
                    Visible = true
                };

                var templatedApplication = await service.CreateApplicationAsync(name, "barebones", creationAttributes);

                var updateAttributes = new ApplicationAttributes()
                {
                    ApplicationAuthor = "Splunk, Inc.",
                    Configured = true,
                    Description = "This app update confirms that an app can be updated from a template",
                    Label = name,
                    Version = "2.0.1",
                    Visible = true
                };

                await templatedApplication.UpdateAsync(updateAttributes, checkForUpdates: true);
                await templatedApplication.GetAsync(); // Because UpdateAsync doesn't return the updated entity

                await templatedApplication.DisableAsync();
                await templatedApplication.GetAsync(); // Because POST apps/local/{name}/disable does not return new data

                await templatedApplication.EnableAsync();
                await templatedApplication.GetAsync(); // Because POST apps/local/{name}/enable does not return new data

                var templatedApplicationArchiveInfo = await templatedApplication.PackageAsync();
                await templatedApplicationArchiveInfo.GetAsync(); // Repackages the application because that's what this entity/endpoint does on GET.
                await templatedApplication.RemoveAsync();
            }
        }

        public async Task MayBe()
        {
            using (var service = await TestHelper.CreateService())
            {
                var collection = await service.Applications.GetAsync();
                var status = await service.Applications.ReloadAsync(); // Instructs Splunk to reload all applications
                collection = await service.Applications.GetAsync();

                foreach (var application in collection)
                {
                    if (application.Name == "twitter2")
                    {
                        service.Applications.RemoveAsync("twitter2");

                        try
                        {
                            await service.Applications.GetAsync("twitter2");
                        }
                        catch (ResourceNotFoundException)
                        {
                            // As expected because the application no longer exists
                        }

                        applicationEndpoint = await service.Applications.CreateEndpoint("twitter2");

                        try
                        {
                            await applicationEndpoint.GetAsync();
                        }
                        catch (ResourceNotFoundException)
                        {
                            // As expected because the application no longer exists
                        }

                        var path = Path.Combine(Environment.CurrentDirectory, "Data", "app-for-twitter-data_230.spl");

                        await service.Applications.InstallAsync(path, update: true);

                        var applicationSetupInfo = await applicationEndpoint.GetSetupInfoAsync();
                        var applicationUpdateInfo = await applicationEndpoint.GetUpdateInfoAsync();

                        await applicationEndpoint.RemoveAsync();
                    }
                }

                //// Create an app from one of the built-in templates

                var name = string.Format("delete-me-{0:N}", Guid.NewGuid());

                var creationAttributes = new ApplicationAttributes()
                {
                    ApplicationAuthor = "Splunk",
                    Configured = true,
                    Description = "This app confirms that an app can be created from a template",
                    Label = name,
                    Version = "2.0.0",
                    Visible = true
                };

                var templatedApplicationEndpoint = await service.Applications.CreateEndpointAsync(name);

                templatedApplicationEndpoint.CreateAsync("barebones", creationAttributes);

                var updateAttributes = new ApplicationAttributes()
                {
                    ApplicationAuthor = "Splunk, Inc.",
                    Configured = true,
                    Description = "This app update confirms that an app can be updated from a template",
                    Label = name,
                    Version = "2.0.1",
                    Visible = true
                };

                await templatedApplicationEndpoint.UpdateAsync(updateAttributes, checkForUpdates: true);
                await templatedApplicationEndpoint.GetAsync();

                await templatedApplicationEndpoint.DisableAsync();
                await templatedApplicationEndpoint.GetAsync(); // Because POST apps/local/{name}/disable does not return new data

                await templatedApplicationEndpoint.EnableAsync();
                await templatedApplicationEndpoint.GetAsync(); // Because POST apps/local/{name}/enable does not return new data

                var templatedApplicationArchiveInfo = await templatedApplicationEndpoint.PackageAsync();
                await templatedApplicationArchiveInfo.GetAsync(); // won't compile because resources aren't endpoints
                await templatedApplicationEndpoint.RemoveAsync();
            }
        }
    }
}
