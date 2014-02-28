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

namespace Splunk.Sdk
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The <see cref="SearchResultsReader"/> class reads <see cref="SearchResults"/>.
    /// </summary>
    public class SearchResultsReader : IEnumerable<SearchResults>, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResultsReader"/> class.
        /// This is a base contructor that should not be used directly.
        /// </summary>
        /// <param name="stream">
        /// The underlying <see cref="Stream"/>.
        /// </param>
        private SearchResultsReader(Stream stream)
        {
            this.searchResults = new SearchResults(stream, true);
        }

        /// <summary>
        /// Returns an enumerator over the sets of results from this reader.
        /// </summary>
        /// <returns>An enumerator of events.</returns>
        public IEnumerator<SearchResults> GetEnumerator()
        {
            while (this.searchResults.NextResultSet())
            {
                yield return this.searchResults;
            } 
        }

        /// <summary>
        /// Returns an enumerator over the sets of results from this reader.
        /// </summary>
        /// <returns>An enumerator of events.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Release unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        #region Privates

        readonly SearchResults searchResults;
        bool disposed;

        private void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.searchResults.Dispose();
                this.disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}