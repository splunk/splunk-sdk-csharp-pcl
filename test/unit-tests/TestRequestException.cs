/*
 * Copyright 2014 Splunk, Inc.
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
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using Xunit;

    public class TestRequestException
    {
        [Trait("unit-test", "Splunk.Client.RequestException")]
        [Fact]
        void CanConstructRequestException()
        {
            RequestException requestException;

            foreach (var type in new MessageType[] { MessageType.Debug, MessageType.Error, MessageType.Fatal, MessageType.Information, MessageType.Warning })
            {
                var details = new ReadOnlyCollection<Message>(new Message[] 
                {
                    new Message(type, "Information on the cause of the RequestException")
                });
                
                requestException = new RequestException(new HttpResponseMessage(HttpStatusCode.InternalServerError), details);

                Assert.Equal(string.Format("500: Internal Server Error\n  {0}: Information on the cause of the RequestException", type), requestException.Message);
                Assert.Equal(HttpStatusCode.InternalServerError, requestException.StatusCode);
                Assert.Equal(1, requestException.Details.Count);
                Assert.Equal(details, requestException.Details.AsEnumerable());
            }

            requestException = new RequestException(new HttpResponseMessage(HttpStatusCode.NotFound), null);
            Assert.Equal("404: Not Found", requestException.Message);
            Assert.Equal(0, requestException.Details.Count);
            Assert.Equal(HttpStatusCode.NotFound, requestException.StatusCode);

            for (int i = 2; i < 10; ++i)
            {
                var message = new StringBuilder("404: Not Found");
                var details = new Message[i];

                for (int j = 0; j < i; ++j)
                {
                    details[j] = new Message(MessageType.Warning, "Information on the cause of the RequestException");
                    message.Append("\n  ");
                    message.Append(details[j]);
                }

                requestException = new RequestException(
                    new HttpResponseMessage(HttpStatusCode.NotFound), new ReadOnlyCollection<Message>(details));

                Assert.Equal(HttpStatusCode.NotFound, requestException.StatusCode);
                Assert.Equal(message.ToString(), requestException.Message);
                Assert.Equal(i, requestException.Details.Count);
                Assert.Equal(details, requestException.Details.AsEnumerable());
            }
        }
   }
}
