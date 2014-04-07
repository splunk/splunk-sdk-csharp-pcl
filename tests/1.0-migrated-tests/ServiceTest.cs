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

using System.Linq;

namespace Splunk.Sdk.UnitTesting
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Splunk.Sdk;

    /// <summary>
    /// This class tests all the Splunk Service methods.
    /// </summary>
    [TestClass]
    public class ServiceTest : TestHelper
    {
        /// <summary>
        /// The base assert string
        /// </summary>
        private string assertRoot = "Service tests: ";

        /// <summary>
        /// Touches the job after it is queryable.
        /// </summary>
        /// <param name="job">The job</param>
        private void CheckJob(Job job, Service service)
        {
            string dummyString;
            string[] dummyList;
            int dummyInt;
            bool dummyBool;
            DateTime dummyDateTime;
            double dummyDouble;

            //// wait until job is queryable
            //this.Ready(job);
            //dummyDateTime = job.CursorTime;
            //dummyString = job.Delegate;
            //dummyInt = job.DiskUsage;
            //dummyString = job.DispatchState;
            //dummyDouble = job.DoneProgress;
            //dummyInt = job.DropCount;
            //dummyDateTime = job.EarliestTime;
            //dummyInt = job.EventAvailableCount;
            //dummyInt = job.EventCount;
            //dummyInt = job.EventFieldCount;
            //dummyBool = job.EventIsStreaming;
            //dummyBool = job.EventIsTruncated;
            //dummyString = job.EventSearch;
            //dummyString = job.EventSorting;
            //dummyString = job.IndexEarliest;
            //dummyString = job.IndexLatest;
            //dummyString = job.Keywords;
            //dummyString = job.Label;

            //if (service.VersionCompare("6.0") < 0)
            //{
            //    dummyDateTime = job.LatestTime;
            //}

            //dummyInt = job.NumPreviews;
            //dummyInt = job.Priority;
            //dummyString = job.RemoteSearch;
            //dummyString = job.ReportSearch;
            //dummyInt = job.ResultCount;
            //dummyBool = job.ResultIsStreaming;
            //dummyInt = job.ResultPreviewCount;
            //dummyDouble = job.RunDuration;
            //dummyInt = job.ScanCount;
            //dummyString = job.Search;
            //dummyString = job.SearchEarliestTime;
            //dummyString = job.SearchLatestTime;
            //dummyList = job.SearchProviders;
            //dummyString = job.Sid;
            //dummyInt = job.StatusBuckets;
            //dummyInt = job.Ttl;
            //dummyBool = job.IsDone;
            //dummyBool = job.IsFailed;
            //dummyBool = job.IsFinalized;
            //dummyBool = job.IsPaused;
            //dummyBool = job.IsPreviewEnabled;
            //dummyBool = job.IsRealTimeSearch;
            //dummyBool = job.IsRemoteTimeline;
            //dummyBool = job.IsSaved;
            //dummyBool = job.IsSavedSearch;
            //dummyBool = job.IsZombie;
            //Assert.AreEqual(job.Name, job.Sid, this.assertRoot + "#1");
        }

        ///// <summary>
        ///// Checks to make sure response is an HTTP OK.
        ///// </summary>
        ///// <param name="response">The repsonse message</param>
        //private void CheckResponse(ResponseMessage response)
        //{
        //    Assert.AreEqual(200, response.Status, this.assertRoot + "#2");
        //    try
        //    {
        //        // Make sure we can at least load the Atom response
        //        AtomFeed.Parse(response.Content);
        //    }
        //    catch (Exception e)
        //    {
        //        Assert.Fail(e.Message);
        //    }
        //}

        /// <summary>
        /// Checks the getters in the Job class
        /// </summary>
        [TestMethod]
        public void JobTest()
        {
            Service service = this.Connect();
            String query = "Search * | head 10";
            Job job = service.StartJobAsync(query).Result;
            /////this.CheckJob(job, service);
            ////// summary of job
            ////job.Summary().Close();
        }

        /// <summary>
        /// Test the expected service capabilities.
        /// </summary>
        [TestMethod]
        public void ServiceCapabilities()
        {
            Service service = this.Connect();

            List<string> expected = new List<string> 
            {
                "admin_all_objects", "change_authentication",
                "change_own_password", "delete_by_keyword",
                "edit_deployment_client", "edit_deployment_server",
                "edit_dist_peer", "edit_forwarders", "edit_httpauths",
                "edit_input_defaults", "edit_monitor", "edit_roles",
                "edit_scripted", "edit_search_server", "edit_server",
                "edit_splunktcp", "edit_splunktcp_ssl", "edit_tcp", "edit_udp",
                "edit_user", "edit_web_settings", "get_metadata",
                "get_typeahead", "indexes_edit", "license_edit", "license_tab",
                "list_deployment_client", "list_forwarders", "list_httpauths",
                "list_inputs", "request_remote_tok", "rest_apps_management",
                "rest_apps_view", "rest_properties_get", "rest_properties_set",
                "restart_splunkd", "rtsearch", "schedule_search", "search",
                "use_file_operator"
            };

            List<object> caps = service.GetCapabilitiesAsync().Result;
            string[] capStrings = caps.Select(a => (string) a).ToArray();
            foreach (string name in expected)
            {
                Assert.IsTrue(this.Contains(capStrings, name), this.assertRoot + "#3");
            }
        }

        ///// <summary>
        ///// Test naked HTTP get
        ///// </summary>
        //[TestMethod]
        //public void ServiceGet()
        //{
        //    Service service = this.Connect();

        //    // Check a few paths that we know exist
        //    string[] paths = 
        //    { 
        //        "/", 
        //        "/services", 
        //        "/services/search/jobs",
        //        "search/jobs",
        //        "authentication/users"
        //    };

        //    foreach (string path in paths)
        //    {
        //        this.CheckResponse(service.Get(path));
        //    }
        //    // And make sure we get the expected 404
        //    try
        //    {
        //        service.Get("/zippy");
        //        Assert.Fail("Expected HttpException");
        //    }
        //    catch (WebException ex)
        //    {
        //        Assert.AreEqual(404, ((HttpWebResponse)ex.Response).StatusCode.GetHashCode(), this.assertRoot + "#4");
        //        return;
        //    }

        //    Assert.Fail(this.assertRoot + "#4.1");
        //}

        ///// <summary>
        ///// Tests the getting of service info (there are no set arguments)
        ///// </summary>
        //[TestMethod]
        //public void ServiceInfo()
        //{
        //    List<string> expected = new List<string> 
        //    {
        //        "build", "cpu_arch", "guid", "isFree", "isTrial", "licenseKeys",
        //        "licenseSignature", "licenseState", "master_guid", "mode",
        //        "os_build", "os_name", "os_version", "serverName", "version"
        //    };

        //    Service service = Connect();
        //    ServiceInfo info = service.GetInfo();

        //    // check for standard fields
        //    foreach (string name in expected)
        //    {
        //        Assert.IsTrue(
        //            info.ContainsKey(name),
        //            string.Format("{0} not found.", name));
        //    }

        //    bool dummyBool;
        //    int dummyInt;
        //    string[] dummyStrings;
        //    string dummyString;

        //    dummyInt = info.Build;
        //    dummyString = info.CpuArch;
        //    dummyString = info.Guid;
        //    dummyStrings = info.LicenseKeys;
        //    dummyStrings = info.LicenseLabels;
        //    dummyString = info.LicenseSignature;
        //    dummyString = info.LicenseState;
        //    dummyString = info.MasterGuid;
        //    dummyString = info.Mode;
        //    dummyString = info.OsBuild;
        //    dummyString = info.OsName;
        //    dummyString = info.OsVersion;
        //    dummyString = info.ServerName;
        //    dummyString = info.Version;
        //    dummyBool = info.IsFree;
        //    dummyBool = info.IsRtSearchEnabled;
        //    dummyBool = info.IsTrial;
        //}

        ///// <summary>
        ///// Test login
        ///// </summary>
        //[TestMethod]
        //public void ServiceLogin()
        //{
        //    ResponseMessage response;

        //    Service service = new Service(Scheme.Https,this.SetUp().Host, this.SetUp().Port);

        //    // Not logged in, should fail with 401
        //    try
        //    {
        //        response = service.Get("/services/authentication/users");
        //        Assert.Fail("Expected HttpException");
        //    }
        //    catch (WebException ex)
        //    {
        //        Assert.AreEqual(401, ((HttpWebResponse)ex.Response).StatusCode.GetHashCode(), this.assertRoot + "#6");
        //    }

        //    // Logged in, request should succeed
        //    service.Login(this.SetUp().Username, this.SetUp().Password);
        //    response = service.Get("/services/authentication/users");
        //    this.CheckResponse(response);

        //    // Logout, the request should fail with a 401
        //    service.Logout();
        //    try
        //    {
        //        response = service.Get("/services/authentication/users");
        //        Assert.Fail("Expected HttpException");
        //    }
        //    catch (WebException ex)
        //    {
        //        Assert.AreEqual(401, ((HttpWebResponse)ex.Response).StatusCode.GetHashCode(), this.assertRoot + "#6");
        //    }
        //}

        ///// <summary>
        ///// Test setters and getters
        ///// </summary>
        //[TestMethod]
        //public void ServiceSettersGetters()
        //{
        //    // The individual classes test most of the set/get methods,
        //    // but call out some specific cases here.
        //    Service service = this.Connect();
        //    Settings settings = service.GetSettings();
        //    string originalHost = settings.Host;
        //    int originalMinSpace = settings.MinFreeSpace;

        //    // make sure set updates state before getting.
        //    // entity.setMethod(value)
        //    // entity.Method() --> gets value.
        //    settings.Host = "sdk-host";
        //    Assert.AreEqual("sdk-host", settings.Host, this.assertRoot + "#8");

        //    // make sure posts arguments are merged
        //    // entity.setMethod(value)
        //    // entity.update(args.create("key2", value2))
        //    settings.Host = "sdk-host2";
        //    settings.Update(new Dictionary<string, object>(Args.Create("minFreeSpace", 500)));
        //    Assert.AreEqual("sdk-host2", settings.Host, this.assertRoot + "#9");
        //    Assert.AreEqual(500, settings.MinFreeSpace, this.assertRoot + "#10");

        //    // make sure live posts argument take precedents over setters
        //    // entity.setMethod(value)
        //    // entity.update(args.create("samekey", value2))
        //    settings.MinFreeSpace = 600;
        //    settings.Update(new Dictionary<string, object>(Args.Create("minFreeSpace", 700)));
        //    Assert.AreEqual(700, settings.MinFreeSpace, this.assertRoot + "#11");

        //    // Restore original
        //    settings.Host = originalHost;
        //    settings.MinFreeSpace = originalMinSpace;
        //    settings.Update();
        //    Assert.AreEqual(settings.MinFreeSpace, originalMinSpace, this.assertRoot + "#12");
        //    Assert.AreEqual(settings.Host, originalHost, this.assertRoot + "#13");
        //}
    }
}
