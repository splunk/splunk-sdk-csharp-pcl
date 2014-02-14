namespace Splunk.Sdk
{
    using System;
    using System.Net;
    using System.Net.Security;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Splunk.Sdk;

    [TestClass]
    public class TestSplunkClient
    {
        static TestSplunkClient()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };
        }

        [TestMethod]
        public void Construct()
        {
            SplunkClient client = new SplunkClient(Protocol.Https, "localhost", 8089);

            Assert.AreEqual(client.Protocol, Protocol.Https);
            Assert.AreEqual(client.Host, "localhost");
            Assert.AreEqual(client.Port, 8089);
            Assert.IsNull(client.SessionKey);

            Assert.AreEqual(client.ToString(), "https://localhost:8089");
        }

        [TestMethod]
        public void Login()
        {
            SplunkClient client = new SplunkClient(Protocol.Https, "localhost", 8089);
            Task task;

            task = client.Login("admin", "changeme");
            task.Wait();

            Assert.AreEqual(task.Status, TaskStatus.RanToCompletion);
            Assert.IsNotNull(client.SessionKey);

            task = client.Login("admin", "bad-password");
            task.Wait();

            Assert.AreEqual(task.Status, TaskStatus.Faulted);
            Assert.IsInstanceOfType(task.Exception, typeof(AggregateException));
        }

        #region Privates

        #endregion
    }
}
