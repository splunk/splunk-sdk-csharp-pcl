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

// [ ] Documentation

namespace Splunk.Sdk
{
    using System;
    using System.Diagnostics.Contracts;

    abstract class ValueConverter<TValue>
    {
        public virtual TValue DefaultValue
        { get { return default(TValue); } }

        public virtual TValue Convert(object value)
        {
            Contract.Requires(value != null);
            throw new NotImplementedException("ValueConverter<TValue>.Convert method");
        }

        protected static readonly string TypeName = typeof(TValue).Name;
    }
}
