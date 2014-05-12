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

//// TODO:
//// [ ] Documentation

namespace Splunk.Client
{
    using System.Dynamic;

    /// <summary>
    /// Provides a class that represents a Splunk server's Extensible 
    /// Administration Interface.
    /// </summary>
    public class Eai : ExpandoAdapter<Eai>
    {
        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Eai"/> class.
        /// </summary>
        public Eai()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public EaiAcl Acl
        {
            get { return this.GetValue("Acl", EaiAcl.Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public EaiAttributes Attributes
        {
            get { return this.GetValue("Attributes", EaiAttributes.Converter.Instance); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Setup
        {
            get { return this.GetValue("Setup", StringConverter.Instance); }
        }

        #endregion
    }
}
