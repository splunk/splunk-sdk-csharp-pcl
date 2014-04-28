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
    using System.Linq;
    using Splunk.Client;
    using Splunk.Client.UnitTesting;
    using Xunit;

    /// <summary>
    /// This class tests Splunk Messages
    /// </summary>
    public class MessageTest : TestHelper
    {
        /// <summary>
        /// The base assert string
        /// </summary>
        private string assertRoot = "Messages assert: ";

        /// <summary>
        /// Tests basic messages
        /// </summary>
        [Fact]
        public void Message()
        {
            Service service = Connect();

            ServerMessageCollection messageCollection = service.Server.GetMessagesAsync().Result;

            if (messageCollection.Any(a => a.Name == "sdk-test-message1"))
            {
                service.Server.RemoveMessageAsync("sdk-test-message1").Wait();
                messageCollection.GetAsync().Wait();
            }
            Assert.False(messageCollection.Any(a => a.Name == "sdk-test-message1"), this.assertRoot + "#1");

            if (messageCollection.Any(a => a.Name == "sdk-test-message2"))
            {
                service.Server.RemoveMessageAsync("sdk-test-message2").Wait();
                messageCollection.GetAsync().Wait();
            }
            Assert.False(messageCollection.Any(a => a.Name == "sdk-test-message1"), this.assertRoot + "#2");

            service.Server.CreateMessageAsync("sdk-test-message1", ServerMessageSeverity.Information, "hello.").Wait();
            messageCollection.GetAsync().Wait();
            Assert.True(messageCollection.Any(a => a.Name == "sdk-test-message1"), this.assertRoot + "#3");

            ServerMessage message = service.Server.GetMessageAsync("sdk-test-message1").Result;
            Assert.Equal("sdk-test-message1", message.Name);
            Assert.Equal("hello.", message.Text);

            service.Server.CreateMessageAsync("sdk-test-message2", ServerMessageSeverity.Information, "world.").Wait();
            messageCollection.GetAsync().Wait();
            Assert.True(messageCollection.Any(a => a.Name == "sdk-test-message2"), this.assertRoot + "#6");

            message = service.Server.GetMessageAsync("sdk-test-message2").Result;
            messageCollection.GetAsync().Wait();
            Assert.Equal("sdk-test-message2", message.Name);
            Assert.Equal("world.", message.Text);

            service.Server.RemoveMessageAsync("sdk-test-message1").Wait();
            messageCollection.GetAsync().Wait();
            Assert.False(messageCollection.Any(a => a.Name == "sdk-test-message1"), this.assertRoot + "#9");
            Assert.True(messageCollection.Any(a => a.Name == "sdk-test-message2"), this.assertRoot + "#10");

            service.Server.RemoveMessageAsync("sdk-test-message2").Wait();
            messageCollection.GetAsync().Wait();
            Assert.False(messageCollection.Any(a => a.Name == "sdk-test-message2"), this.assertRoot + "#10");
        }
    }
}
