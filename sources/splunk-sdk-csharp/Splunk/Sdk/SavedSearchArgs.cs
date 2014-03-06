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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class SavedSearchArgs : Dictionary<string, object>
    {
        #region Constructors

        public SavedSearchArgs()
            : base()
        { }

        #endregion

        #region Methods

        public IEnumerable<KeyValuePair<string, object>> AsArguments()
        {
            foreach (KeyValuePair<string, object> arg in this)
            {
                yield return new KeyValuePair<string, object>("args." + arg.Key, arg.Value);
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (KeyValuePair<string, object> arg in this)
            {
                builder.Append("args.");
                builder.Append(arg.Key);
                builder.Append('=');
                builder.Append(arg.Value);
                builder.Append("; ");
            }

            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 2;
            }

            return builder.ToString();
        }

        #endregion
    }
}
