using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace random_numbers
{
    using Splunk.ModularInputs;
    using System.IO;
    using System.Threading;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Xunit;
    using System.Threading.Tasks;

    public class TestRandomNumbers
    {

        public class HarnessedScript : IDisposable
        {
            public event EventHandler trigger = (sender, args) => { };

            public void TriggerEvent()
            {
                trigger.Invoke(this, new EventArgs());
            }
            
            public CancellationTokenSource cancellationTokenSource;
            public AwaitableProgress<EventWrittenProgressReport> Progress;
            public Program Script;
            public StringWriter Stdout;
            public StringWriter Stderr;
            public StringReader Stdin;
            public string[] Args;

            public HarnessedScript(string[] args, String stdinText)
            {
                Args = args;
                cancellationTokenSource = new CancellationTokenSource();
                Progress = new AwaitableProgress<EventWrittenProgressReport>();
                Stdout = new StringWriter();
                Stderr = new StringWriter();
                Stdin = new StringReader(stdinText);
            }

            public async Task<int> RunAsync()
            {
                return await ModularInput.RunAsync<Program>(Args, Stdin, Stdout, Stderr, cancellationTokenSource.Token, this.Progress);

            }

            public void Dispose()
            {
                Stdout.Dispose();
                Stderr.Dispose();
                Stdin.Dispose();
            }
        }

        [Trait("class", "random_numbers.Program")]
        [Fact]
        public async Task GeneratesScheme()
        {
            using (HarnessedScript harness =
                new HarnessedScript(new string[] { "--scheme" }, ""))
            {
                Assert.Equal(0, await harness.RunAsync());
                Assert.Equal("", harness.Stderr.ToString());
                XDocument doc = XDocument.Parse(harness.Stdout.ToString());
                Assert.Equal("Random numbers", doc.Element("scheme").Element("title").Value);
            }

        }

        [Trait("class", "random_numbers.Program")]
        [Fact]
        public async Task InvalidArguments()
        {
            using (HarnessedScript harness =
                new HarnessedScript(new string[] { "cowabunga!" }, ""))
            {
                Assert.Equal(-1, await harness.RunAsync());
                Assert.Equal("", harness.Stdout.ToString());
                Assert.Equal("ERROR Invalid arguments to modular input.", harness.Stderr.ToString().Trim());
            }
        }

        [Trait("class", "random_numbers.Program")]
        [Fact]
        public async Task SuccessfulValidation()
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("items",
                    new XElement("server_host", "tiny"),
                    new XElement("server_uri", "https://127.0.0.1:8089"),
                    new XElement("checkpoint_dir", "abcd"),
                    new XElement("session_key", "some key"),
                    new XElement("item",
                        new XAttribute("name", "abcdefg"),
                        new XElement("param",
                            new XAttribute("name", "min"),
                            "42"),
                        new XElement("param",
                            new XAttribute("name", "max"),
                            "52")))
           );
            using (HarnessedScript harness =
                new HarnessedScript(new string[] { "--validate-arguments" }, doc.ToString()))
            {
                Assert.Equal(0, await harness.RunAsync());
                Assert.Equal("", harness.Stdout.ToString());
                Assert.Equal("", harness.Stderr.ToString());
            }
        }


        [Trait("class", "random_numbers.Program")]
        [Fact]
        public async Task FailedValidation()
        {
            XDocument doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("items",
                        new XElement("server_host", "tiny"),
                        new XElement("server_uri", "https://127.0.0.1:8089"),
                        new XElement("checkpoint_dir", "abcd"),
                        new XElement("session_key", "some key"),
                        new XElement("item",
                            new XAttribute("name", "abcdefg"),
                            new XElement("param",
                                new XAttribute("name", "min"),
                                "85"),
                            new XElement("param",
                                new XAttribute("name", "max"),
                                "52")))
               );
            using (HarnessedScript harness =
                new HarnessedScript(new string[] { "--validate-arguments" }, doc.ToString()))
            {
                Assert.Equal(0, await harness.RunAsync());
                Assert.Equal("<error><message>min must be less than max.</message></error>",
                    harness.Stdout.ToString());
                Assert.Equal("", harness.Stderr.ToString());
            }
        }

        public class AwaitableProgress<T> : IProgress<T>
        {
            private event Action<T> handler = (T x) => { };

            public void Report(T value)
            {
                this.handler(value);
            }

            public async Task<T> AwaitProgressAsync()
            {
                TaskCompletionSource<T> source = new TaskCompletionSource<T>();
                Action<T> onReport = null;
                onReport = (T x) =>
                {
                    handler -= onReport;
                    source.SetResult(x);
                };
                handler += onReport;
                return await source.Task;
            }
        }

        [Trait("class", "AwaitableProgress")]
        [Fact]
        public async Task AwaitProgressWorks()
        {
            AwaitableProgress<bool> progress = new AwaitableProgress<bool>();
            Task<bool> triggered = progress.AwaitProgressAsync();
            progress.Report(true);
            Assert.Equal(true, await triggered);
        }

        [Trait("class", "random_numbers.Program")]
        [Fact(Timeout = 10)]
        public async Task StreamEvents()
        {
            XDocument doc = new XDocument(
                new XElement("input",
                    new XElement("server_host", "tiny"),
                    new XElement("server_uri", "https://127.0.0.1:8089"),
                    new XElement("checkpoint_dir", "/some/dir"),
                    new XElement("session_key", "1234567890"),
                    new XElement("configuration",
                        new XElement("stanza",
                            new XAttribute("name", "foobar://aaa"),
                            new XElement("param",
                                new XAttribute("name", "min"),
                                0),
                            new XElement("param",
                                new XAttribute("name", "max"),
                                12)))));

            using (HarnessedScript harness =
                new HarnessedScript(new string[0], doc.ToString()))
            {
                Task t = harness.RunAsync();

                Assert.Equal("", harness.Stdout.ToString());
                Assert.Equal("", harness.Stderr.ToString());

                Task<EventWrittenProgressReport> progressReportTask;

                progressReportTask = harness.Progress.AwaitProgressAsync();
                harness.TriggerEvent();
                await progressReportTask;

                String expected = "<stream><event xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                    "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" unbroken=\"0\" stanza=\"foobar://aaa\"><data>";
                Assert.True(harness.Stdout.ToString().Trim().StartsWith(expected));
                Assert.Equal("", harness.Stderr.ToString());

                progressReportTask = harness.Progress.AwaitProgressAsync();
                harness.TriggerEvent();
                await progressReportTask;
                await progressReportTask;
                await progressReportTask;
                await progressReportTask;
                await progressReportTask;

                Assert.Equal("", harness.Stdout.ToString());
                XDocument docComplete = XDocument.Parse(harness.Stdout.ToString());
                Assert.Equal(2, docComplete.Root.Elements().Count());
                Assert.Equal("", harness.Stderr.ToString());

                await t;
            }
        }
    }
}
