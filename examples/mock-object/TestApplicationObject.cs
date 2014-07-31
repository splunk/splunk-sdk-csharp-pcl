namespace mock_object
{
    using Moq;
    using Splunk.Client;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    public class TestApplicationObject
    {
        [Trait("unit-test", "Mock Object")]
        [Fact]
        public void Test()
        {
            var service = new Mock<Service>();
            var job = new Mock<Job>();
            
            service.Setup(s => s.LogOnAsync(
                It.IsAny<string>(), 
                It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            service.Setup(s => s.LogOffAsync()).Returns(Task.FromResult(true));

            service.Setup(s => s.SearchAsync(
                It.IsAny<string>(),             // search
                It.IsAny<int>(),                // count
                It.IsAny<ExecutionMode>(),      // mode
                It.IsAny<JobArgs>(),            // args
                It.IsAny<CustomJobArgs>()))     // customArgs
                .Returns(Task.FromResult(job.Object));

            var message = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(new FileStream("search-results.xml", FileMode.Open)),
                };

            var resultStream = SearchResultStream.CreateAsync(message);

            job.Setup(j => j.GetSearchResultsAsync(It.IsAny<SearchResultArgs>())).Returns(resultStream);
            Run(service.Object).Wait();
        }

        async Task Run(Service service)
        {
            Job job = await service.SearchAsync("search index=_internal", count: 1000);
            
            foreach (var result in await job.GetSearchResultsAsync())
            {
                Console.WriteLine(result);
            }
        }
    }
}
