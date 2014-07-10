namespace mock_interface
{
    using Moq;
 
    using Splunk.Client;
    using System;
    using System.Threading.Tasks;

    using Xunit;

    public class ApplicationInterfaceTest
    {
        [Trait("unit-test", "Mock Interface")]
        [Fact]
        public void Test()
        {
            var application = new Mock<IApplication>();
            var archiveInfo = new Mock<ApplicationArchiveInfo>();

            application.Setup(a => a.PackageAsync()).Returns(Task.FromResult(archiveInfo.Object));
            archiveInfo.Setup(a => a.ApplicationName).Returns("searchcommands_app");
            archiveInfo.Setup(a => a.Path).Returns(@"C:\Program Files\Splunk\var\upload\searchcommands_app.spl");

            this.Run(application.Object).Wait();
        }

        async Task Run(IApplication application)
        {
            ApplicationArchiveInfo info = await application.PackageAsync();

            Console.WriteLine("Application name: {0}", info.ApplicationName);
            Console.WriteLine("Application path: {0}", info.Path);
        }
    }
}
