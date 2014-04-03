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

using System.Text.RegularExpressions;

namespace SplunkSDKHelper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using net.sf.dotnetcli;

    /// <summary>
    /// Command line process (TODO: need 3rd party option processor, like Java).
    /// For now, only parse .splunkrc file.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// The Application Name.
        /// </summary>
        private string appName;

        /// <summary>
        /// The rules that the application has.
        /// </summary>
        private Options rules = new Options();

        /// <summary>
        /// The parsed command line arguments
        /// </summary>
        private string[] args = new string[0];

        /// <summary>
        /// The parsed command line flags (i.e. those that begin with - and --)
        /// </summary>
        public Dictionary<string, object> Opts = 
            new Dictionary<string, object>();

        /// <summary>
        /// The application default field
        /// </summary>
        public string App = null;

        /// <summary>
        /// Help or usage.
        /// </summary>
        public bool Help = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="appName">The application name</param>
        public Command(string appName) 
        { 
            this.appName = appName;
        }

        /// <summary>
        /// Gets the host to connect to.
        /// </summary>
        public string Host
        {
            get
            {
                var value = this.Opts["host"];
                return (string)value ?? "localhost";
            }
        }

        /// <summary>
        /// Gets the owner
        /// </summary>
        public string Owner
        {
            get
            {
                return (string)this.Opts["owner"];
            }
        }

        /// <summary>
        /// Gets the port to connect to.
        /// </summary>
        public int Port
        {
            get
            {
                var value = this.Opts["port"];
                return (int)(value ?? 8089);
            }
        }

        /// <summary>
        /// Gets the password used for authentication.
        /// </summary>
        public string Password
        {
            get
            {
                return (string)this.Opts["password"];
            }
        }

        /// <summary>
        /// Gets the scheme to use (http or https).
        /// </summary>
        public string Scheme
        {
            get
            {
                var value = this.Opts["scheme"];
                return (string)value ?? "https";
            }
        }

        /// <summary>
        /// Gets the username used for authentication.
        /// </summary>
        public string Username
        {
            get
            {
                return (string)this.Opts["username"];
            }
        }

        /// <summary>
        /// Creates a new Command instance with no name.
        /// </summary>
        /// <returns>The Command instance</returns>
        public static Command Create()
        {
            return Create(null);
        }

        /// <summary>
        /// Creates a new Command instance with the specified application name.
        /// </summary>
        /// <param name="appName">The application name</param>
        /// <returns>The Command instance</returns>
        public static Command Create(string appName)
        {
            return new Command(appName);
        }

        /// <summary>
        /// Exposes the private rule options.
        /// </summary>
        /// <returns>The Options</returns>
        public Options GetRules()
        {
            return this.rules;
        }

        /// <summary>
        /// Initializes with default Splunk command options.
        /// </summary>
        /// <returns>The Command instance.</returns>
        public Command Init()
        {
            this.rules.AddOption(
                "h", "help", false, "Display this help message");
            this.rules.AddOption(
                null, "host", true, "Host name (default localhost)");
            this.rules.AddOption(OptionBuilder.Factory
                    .WithLongOpt("port")
                    .HasArg(true)
                    .WithType(typeof(int))
                    .WithDescription("Port number")
                    .Create());
            this.rules.AddOption(
                null, "scheme", true, "Scheme (default https)");
            this.rules.AddOption(
                null, "username", true, "Username to login with");
            this.rules.AddOption(
                null, "password", true, "Password to login with");
            this.rules.AddOption(null, "app", true, "App/namespace context");
            this.rules.AddOption(null, "owner", true, "Owner/user context");
            // This is here only for compatibility with the JavaScript SDK's .splunkrc.
            this.rules.AddOption(null, "version", true, "Version (irrelevant for C#)");
            return this;
        }

        /// <summary>
        /// Adds a rule (for presence).
        /// </summary>
        /// <param name="name">The name of the rule</param>
        /// <param name="description">The description of the rule</param>
        /// <returns>The Command instance</returns>
        public Command AddRule(string name, string description)
        {
            this.rules.AddOption(null, name, false, description);
            return this;
        }

        /// <summary>
        /// Adds a rule with a specific type.
        /// </summary>
        /// <param name="name">The name of the rule</param>
        /// <param name="argType">The type of the argument</param>
        /// <param name="description">The description</param>
        /// <returns>The Command instance</returns>
        public Command AddRule(string name, Type argType, string description)
        {
            this.rules.AddOption(
                OptionBuilder.Factory
                    .WithLongOpt(name)
                    .HasArg(true)
                    .WithType(argType)
                    .WithDescription(description)
                    .Create());
            return this;
        }

        /// <summary>
        /// Load a file of options and arguments
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <returns>The Command instance</returns>
        public Command Load(string path) 
        {
            StreamReader streamReader;
            try 
            {
               streamReader = new StreamReader(path);
            }
            catch (FileNotFoundException) 
            { 
                return this; 
            }

            List<string> argList = new List<string>(4);
            while (true) 
            {
                string line;
                try 
                {
                    line = streamReader.ReadLine();
                }
                catch (IOException) 
                { 
                    return this; 
                }

                if (line == null)
                {
                    break;
                }

                if (line.StartsWith("#"))
                {
                    continue;
                }

                line = line.Trim();
                if (line.Length == 0)
                {
                    continue;
                }

                if (!line.StartsWith("-"))
                {
                    line = "--" + line;
                }

                argList.Add(line);
            }

            this.Parse(argList.ToArray());
            streamReader.Close();
            return this;
        }

        /// <summary>
        /// Parse the given argument vector
        /// </summary>
        /// <param name="argv">The arguments</param>
        /// <returns>The Command instance</returns>
        public Command Parse(string[] argv) 
        {
            PosixParser parser = new PosixParser();

            CommandLine cmdline = null;
            cmdline = parser.Parse(this.rules, argv);

            // Unpack the cmdline into a simple Map of options and optionally
            // assign values to any corresponding fields found in the Command 
            // class.
            foreach (Option option in cmdline.Options) 
            {
                string name = option.LongOpt;
                object value = option.Values;
                if (option.NumberOfArgs == 1)
                {
                    value = ((string[])value)[0];
                }

                // Figure out the type of the option and convert the value.
                if (!option.HasArg) 
                {
                    // If it has no arg, then its implicitly boolean and 
                    // presence of the argument indicates truth.
                    value = true;
                }
                else 
                {
                    Type type = (Type)option.Type;
                    if (type == null || type == typeof(string)) 
                    {
                        // Null implies string, check for multivalue 
                        // (unsupported)
                    } 
                    else if (type == typeof(int)) 
                    {
                        value = Convert.ToInt32((string)value);
                    }
                    else 
                    {
                        throw new Exception("Unsupported type"); 
                    }
                }

                if (this.Opts.ContainsKey(name))
                {
                    // the established value is being overwritten by a newer 
                    // value
                    this.Opts.Remove(name);
                }
                this.Opts.Add(name, value);

                // Look for a field of the Command class (or subclass) that
                // matches the long name of the option and, if found, assign the
                // corresponding option value in order to provide simplified
                // access to command options.
                FieldInfo field = this.GetType().GetField(name);
                if (field != null)
                {
                    field.SetValue(this, value);
                }
            }

            string[] orig = this.args;
            string[] more = cmdline.Args;
            this.args = new string[orig.Length + cmdline.Args.Length];

            orig.CopyTo(this.args, 0);
            cmdline.Args.CopyTo(this.args, orig.Length);

            if (this.Help) 
            {
                this.PrintHelp();
                Environment.Exit(0);
            }

            return this;
        }

        /// <summary>
        /// Print the help
        /// </summary>
        public void PrintHelp() 
        {
            HelpFormatter formatter = new HelpFormatter();
            string appName = this.appName == null ? "App" : this.appName;
            formatter.PrintHelp(appName, this.rules);
        }

        /// <summary>
        /// Creates a new Command instance, with default splunk command line
        /// rules, and no application name.
        /// </summary>
        /// <returns>The Command instance</returns>
        public static Command Splunk() 
        {
            return Splunk(null);
        }

        /// <summary>
        /// Creates a command instance, initializes with the default Splunk
        /// command line rules and attempts to load the default options file.
        /// </summary>
        /// <param name="appName">The application name</param>
        /// <returns>The Command instance.</returns>
        public static Command Splunk(string appName) 
        {
            return Command.Create(appName).Init().Splunkrc();
        }

        /// <summary>
        /// Load the default options file (.splunkrc) if it exists; we look in
        /// the home drive and homepath.
        /// </summary>
        /// <returns>The Command instance.</returns>
        public Command Splunkrc()
        {
            const string ToExpand = "%HOMEDRIVE%%HOMEPATH%";
            var home = 
                Environment.ExpandEnvironmentVariables(ToExpand);

            // If failed to expand, try a different method.
            // This happens when running inside SharePoint/IIS.
            if (home == ToExpand)
            {
                var path = Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData);
                // Path is in the form of <userhomepath>\AppData\Local,
                // for example, C:\Users\Andy\AppData\Local.
                var matches = Regex.Matches(path, @"\\");
                var len = matches[matches.Count - 2].Index;
                home = path.Substring(0, len);
            }
            this.Load(home + "\\.splunkrc");
            return this;
        }
    }
}
