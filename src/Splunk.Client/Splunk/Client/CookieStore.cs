/*
 * Copyright 2015 Splunk, Inc.
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

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net;

    class CookieStore
    {
        #region Constructors

        public CookieStore()
        {
            this.CookieContainer = new CookieContainer();
            // CookieContainer requires a uri to store / parse cookies. 
            // This is because usually cookies are mapped to one specific uri
            // But in the case of search head clustering, the cookies may go to several uris
            // This is a hack to get cookie parsing from CookieContainer by setting and getting all cookies
            // by this globalUri
            this.globalUri = new Uri("https://splunk.local");
        }

        #endregion

        #region Properties

        private CookieContainer CookieContainer;

        private Uri globalUri;

        #endregion

        #region Methods

        public void SetCookies(string cookieHeader)
        {
            this.CookieContainer.SetCookies(globalUri, cookieHeader);
        }

        public string GetCookieHeader()
        {
            return this.CookieContainer.GetCookieHeader(globalUri);
        }

        public bool isEmpty()
        {
            return this.CookieContainer.Count == 0;
        }

        public void clearCookies()
        {
            this.CookieContainer = new CookieContainer();
        }

        #endregion
    }
}
