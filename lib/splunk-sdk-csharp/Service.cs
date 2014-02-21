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

namespace Splunk.Sdk
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class Service
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="namespace"></param>
        public Service(Context context, Namespace @namespace)
        {
            Contract.Requires(context != null);
            Contract.Requires(@namespace != null);

            this.Context = context;
            this.Namespace = @namespace;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="Context"/> instance for this <see cref="Service"/>.
        /// </summary>
        public Context Context
        { get; private set; }

        /// <summary>
        /// Gets the <see cref="Namespace"/> used by this <see cref="Service"/> instance.
        /// </summary>
        public Namespace Namespace
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Provides user authentication.
        /// </summary>
        /// <param name="username">Splunk account username.</param>
        /// <param name="password">Splunk account password for username.</param>
        /// <remarks>This method uses the Splunk <a href="http://goo.gl/yXJX75">
        /// auth/login</a> endpoint. The session key that it returns is used for 
        /// subsequent requests. It is accessible via the <see cref="SessionKey"/>
        /// property.
        /// </remarks>
        public async Task LoginAsync(string username, string password)
        {
            Contract.Requires(username != null);
            Contract.Requires(password != null);

            XDocument document = await this.Context.PostAsync(ResourceName.Login, new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>("username", username),
                new KeyValuePair<string, object>("password", password)
            });
            this.Context.SessionKey = document.Element("response").Element("sessionKey").Value;
        }

        /// <summary>
        /// Retrieves the collection of installed apps.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// See the <a href="http://goo.gl/izvjYx">apps/local</a> REST API Reference.
        /// </remarks>
        public async Task<EntityCollection<App>> GetAppsAsync(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            var apps = new EntityCollection<App>(this.Context, this.Namespace, ResourceName.AppsLocal, parameters);
            await apps.Update();
            return apps;
        }

        /// <summary>
        /// Retrieves the list of all Splunk system capabilities.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// See the <a href="http://goo.gl/wvB9N5">authorization/capabilities</a> REST API Reference.
        /// </remarks>
        public async Task<IReadOnlyList<string>> GetCapabilitiesAsync()
        {
            XDocument document = await this.Context.GetDocumentAsync(this.Namespace, ResourceName.Capabilities);

            var feed = new AtomFeed<GenericEntity>(this.Context, ResourceName.Capabilities, document);
            GenericEntity entity = feed.Entities[0];
            dynamic capabilities = entity.Record.Capabilities;

            return capabilities;
        }

        /// <summary>
        /// Retrieves the collection of all running search jobs.
        /// </summary>
        /// <remarks>
        /// See the <a href="http://goo.gl/gf67qS">search/jobs</a> REST API Reference.
        /// </remarks>
        public async Task<EntityCollection<Job>> GetJobsAsync(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            var jobs = new EntityCollection<Job>(this.Context, this.Namespace, ResourceName.Jobs, parameters);
            await jobs.Update();
            return jobs;
        }

        /// <summary>
        /// Creates a search <see cref="Job"/>.
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <remarks>
        /// See the <a href="http://goo.gl/b02g1d">POST search/jobs</a> REST API Reference.
        /// </remarks>
        public async Task<Job> SearchAsync(string search, IEnumerable<KeyValuePair<string, object>> parameters = null)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(search));

            var searchParameter = new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("search", "search " + search) };
            parameters = parameters == null ? searchParameter : searchParameter.Concat(parameters);

            XDocument document = await this.Context.Post(this.Namespace, ResourceName.Jobs, parameters);
            
            string searchId = document.Element("response").Element("sid").Value;
            var job = new Job(this.Context, this.Namespace, ResourceName.Jobs, name: searchId);
            await job.UpdateAsync();
            
            return job;
        }

        /// <summary>
        /// Gets the URI string for this <see cref="Service"/> instance. 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join("/", this.Context.ToString(), this.Namespace.ToString());
        }

#if false

    ////
    // Returns a +Collection+ of all the configuration files visible on Splunk.
    //
    // The configurations you see are dependent on the namespace your +Service+
    // is connected with. So if you are connected in the system namespace, you
    // may see different values than if you're connected in the app namespace
    // associated with a particular app, since the app may override some values
    // within its scope.
    //
    // The configuration files which are the contents of this +Collection+ are
    // not +Entity+ objects, but +Collection+ objects in their own right. They
    // contain +Entity+ objects representing the stanzas in that configuration
    // file.
    //
    // Returns: +Configurations+ (a subclass of +Collection+ containing
    // +ConfigurationFile+ objects).
    //
    public void Getconfs
      Configurations.new(self)
    end

    ////
    // Creates an asynchronous search job.
    //
    // The search job requires a _query_, and takes a hash of other, optional
    // arguments, which are documented in the {Splunk REST documentation}[http://docs.splunk.com/Documentation/Splunk/latest/RESTAPI/RESTsearch//search.2Fjobs].
    //
    public void Getcreate_search(query, args={})
      jobs.create(query, args)
    end

    ////
    // Creates a blocking search without transforming search commands.
    //
    // The +create_export+ method starts a search _query_, and any optional 
    // arguments specified in a hash (which are identical to those taken by 
    // the +create+ methods). It then blocks until the job is finished, and 
    // returns the events found by the job before any transforming search 
    // commands (equivalent to calling +events+ on a +Job+).
    //
    // Returns: a stream readable by +MultiResultsReader+.
    //
    public void Getcreate_export(query, args={}) // :nodoc:
      jobs.create_export(query, args)
    end

    // DEPRECATED. Use create_export instead.
    public void Getcreate_stream(query, args={})
      warn "[DEPRECATION] Service//create_stream is deprecated. Use Service//create_export instead."
      jobs.create_export(query, args)
    end

    ////
    // Returns a +Collection+ of all +Index+ objects.
    //
    // +Index+ is a subclass of +Entity+, with additional methods for
    // manipulating indexes in particular.
    //
    public void Getindexes
      Collection.new(self, PATH_INDEXES, entity_class=Index)
    end

    ////
    // Returns a +Hash+ containing Splunk's runtime information.
    //
    // The +Hash+ has keys such as "+build+" (the number of the build of this
    // Splunk instance) and "+cpu_arch+" (what CPU Splunk is running on), and
    // "+os_name+" (the name of the operating system Splunk is running on).
    //
    // Returns: A +Hash+ that has +Strings+ as both keys and values.
    //
    public void Getinfo
      response = request(:namespace => Splunk::namespace(:sharing => "default"),
                         :resource => PATH_INFO)
      feed = AtomFeed.new(response.body)
      feed.entries[0]["content"]
    end

    ////
    // Return a collection of the input kinds.
    //
    public void Getinputs
      InputKinds.new(self, PATH_INPUTS)
    end

    ////
    // Returns a collection of the loggers in Splunk.
    //
    // Each logger logs errors, warnings, debug info, or informational
    // information about a specific part of the Splunk system (e.g.,
    // +WARN+ on +DeploymentClient+).
    //
    // Returns: a +Collection+ of +Entity+ objects representing loggers.
    //
    // *Example*:
    //     service = Splunk::connect(:username => 'admin', :password => 'foo')
    //     service.loggers.each do |logger|
    //       puts logger.name + ":" + logger['level']
    //     end
    //     // Prints:
    //     //   ...
    //     //   DedupProcessor:WARN
    //     //   DeployedApplication:INFO
    //     //   DeployedServerClass:WARN
    //     //   DeploymentClient:WARN
    //     //   DeploymentClientAdminHandler:WARN
    //     //   DeploymentMetrics:INFO
    //     //   ...
    //
    public void Getloggers
      Collection.new(self, PATH_LOGGER)
    end

    ////
    // Returns a collection of the global messages on Splunk.
    //
    // Messages include such things as warnings and notices that Splunk needs to
    // restart.
    //
    // Returns: A +Collection+ of +Message+ objects (which are subclasses 
    // of +Entity+).
    //
    public void Getmessages
      Messages.new(self, PATH_MESSAGES, entity_class=Message)
    end

    ////
    // Returns a read only collection of modular input kinds.
    //
    // The modular input kinds are custom input kinds on this Splunk instance.
    // To access the actual inputs of these kinds, use the +Service+//+inputs+
    // method. This method gives access to the metadata describing the input
    // kinds.
    //
    // Returns: A +ReadOnlyCollection+ of +ModularInputKind+ objects representing
    // all the custom input types added to this Splunk instance.
    //
    public void Getmodular_input_kinds
      if self.splunk_version[0] < 5
        raise IllegalOperation.new("Modular input kinds are " +
                                       "not supported before Splunk 5.0")
      else
        ReadOnlyCollection.new(self, PATH_MODULAR_INPUT_KINDS,
                               entity_class=ModularInputKind)
      end
    end

    ////
    // Returns a collection of the roles on the system.
    //
    // Returns: A +Collection+ of +Entity+ objects representing the roles on
    // this Splunk instance.
    //
    public void Getroles
      CaseInsensitiveCollection.new(self, PATH_ROLES)
    end

    ////
    //
    //
    //
    public void Getsaved_searches
      Collection.new(self, PATH_SAVED_SEARCHES, entity_class=SavedSearch)
    end

    ////
    // Returns an +Entity+ of Splunk's mutable runtime information.
    //
    // The +settings+ method includes values such as "+SPLUNK_DB+" 
    // and "+SPLUNK_HOME+". Unlike the values returned by the +info+ method, 
    // these settings can be updated.
    //
    // Returns: an +Entity+ with all server settings.
    //
    // *Example*:
    //     service = Splunk::connect(:username => 'admin', :password => 'foo')
    //     puts svc.settings.read
    //     // Prints:
    //     //    {"SPLUNK_DB" => "/path/to/splunk_home/var/lib/splunk",
    //     //     "SPLUNK_HOME" => "/path/to/splunk_home",
    //     //     ...}
    //
    public void Getsettings
      // Though settings looks like a collection on the server, it always has
      // a single entity, of the same name, giving the actual settings. We access
      // that entity directly rather than putting a collection inbetween.
      Entity.new(self, Splunk::namespace(:sharing => "default"),
                 PATH_SETTINGS, "settings").refresh()
    end

    ////
    // Returns the version of Splunk this +Service+ is connected to.
    //
    // The version is represented as an +Array+ of length 3, with each
    // of its components an integer. For example, on Splunk 4.3.5,
    // +splunk_version+ would return [+4+, +3+, +5+], while on Splunk 5.0.2,
    // +splunk_version+ would return [+5+, +0+, +2+].
    //
    // Returns: An +Array+ of +Integers+ of length 3.
    //
    public void Getsplunk_version
      info["version"].split(".").map() {|v| Integer(v)}
    end

    ////
    // Return a +Collection+ of the users defined on Splunk.
    //
    public void Getusers
      CaseInsensitiveCollection.new(self, PATH_USERS)
    end

#endif
        #endregion
    }
}
