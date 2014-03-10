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

namespace Splunk.Sdk.Examples.ReactiveUI
{
    using System.Windows;
    using System.Net;
    using Splunk.Sdk;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Properties

        public Service Service
        { get; private set; }

        #endregion

        #region Methods

        protected override void OnExit(ExitEventArgs e)
        {
            this.Service.Dispose();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };
            this.Service = new Service(Scheme.Https, "localhost", 8089);
        }

        #endregion
    }
}
