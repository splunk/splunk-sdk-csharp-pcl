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
            this.client = new SplunkClient(Protocol.Https, "localhost", 8089);

            Assert.AreEqual(this.client.Protocol, Protocol.Https);
            Assert.AreEqual(this.client.Host, "localhost");
            Assert.AreEqual(this.client.Port, 8089);
            Assert.IsNull(this.client.SessionKey);

            Assert.AreEqual(this.client.ToString(), "https://localhost:8089");
        }

        [TestMethod]
        public void Login()
        {
            Task task;

            this.client = new SplunkClient(Protocol.Https, "localhost", 8089);
            task = this.client.Login("admin", "changeme");
            task.Wait();

            Assert.AreEqual(task.Status, TaskStatus.RanToCompletion);
            Assert.IsNotNull(this.client.SessionKey);

            task = this.client.Login("admin", "bad-password");
            task.Wait();

            Assert.AreEqual(task.Status, TaskStatus.Faulted);
            Assert.IsInstanceOfType(task.Exception, typeof(AggregateException));
        }

        #region Privates

        SplunkClient client = null;

        #endregion
    }
}
