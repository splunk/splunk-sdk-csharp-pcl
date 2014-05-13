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

    public class TestRandomNumbers
    {
        public class HarnessedScript : IDisposable
        {
            public Program Script;
            public StringWriter Stdout;
            public StringWriter Stderr;
            public StringReader Stdin;
            public string[] Args;

            
            public HarnessedScript(string[] args, String stdinText)
            {
                Args = args;
                Script = new Program();
                Stdout = new StringWriter();
                Stderr = new StringWriter();
                Stdin = new StringReader(stdinText);
            }

            public async Task<int> RunAsync()
            {
                return await Program.RunAsync<Program>(Args, Stdin, Stdout, Stderr, Script);
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
                Assert.Equal("", harness.Stderr.ToString());
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

        [Trait("class", "random_numbers.Program")]
        [Fact]
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
                await harness.RunAsync();


            }
        }
    }
}
