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
    using System.Net;
    /// <summary>
    /// Provides a class for storing and parsing cookies
    /// from a Splunk server.
    /// </summary>
    public class CookieStore
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CookieStore"/> class.
        /// </summary>
        public CookieStore()
        {
            this.CookieContainer = new CookieContainer();
        }

        #endregion

        #region Properties

        private CookieContainer CookieContainer;

        // CookieContainer requires a uri to store / parse cookies. 
        // This is because usually cookies are mapped to one specific uri
        // But in the case of search head clustering, the cookies may go to several uris
        // This globalUri allows all cookies to be stored under one uri
        private static Uri globalUri = new Uri("https://splunk.local");

        #endregion

        #region Methods

        /// <summary>
        /// Parses a cookie header and stores the cookie contained within the header. 
        /// </summary>
        /// <param name="cookieHeader">
        /// The string value of a "Set-Cookie:" header in an http request.
        /// </param>
        public void AddCookie(string cookieHeader)
        {
            this.CookieContainer.SetCookies(globalUri, cookieHeader);
        }

        /// <summary>
        /// Builds a string from stored cookies to create a string that can be put into a "Cookie:" header
        /// </summary>
        /// <returns>
        /// A string to put into a "Cookie:" header representing the state of the <see cref="CookieStore"/>
        /// </returns>
        public string GetCookieHeader()
        {
            return this.CookieContainer.GetCookieHeader(globalUri);
        }

        /// <summary>
        /// Returns whether the <see cref="CookieStore"/> has no cookies.
        /// </summary>
        /// <returns>
        /// A bool representing whether the <see cref="CookieStore"/> has no cookies.
        /// </returns>
        public bool IsEmpty()
        {
            return this.CookieContainer.Count == 0;
        }

        /// <summary>
        /// Removes all cookies from a <see cref="CookieStore"/> class.
        /// </summary>
        public void ClearCookies()
        {
            this.CookieContainer = new CookieContainer();
        }

        #endregion
    }
}
