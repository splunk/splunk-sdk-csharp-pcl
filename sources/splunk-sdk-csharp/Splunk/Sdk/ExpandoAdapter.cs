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

// TODO:
// [ ] Documentation

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;

    public class ExpandoAdapter
    {
        #region Constructors

        protected ExpandoAdapter()
        { }

        protected ExpandoAdapter(ExpandoObject expandoObject)
        {
            Contract.Requires<InvalidOperationException>(expandoObject != null);
            this.ExpandoObject = expandoObject;
        }

        #endregion

        #region Properties

        protected ExpandoObject ExpandoObject
        {
            get
            { return this.expandoObject; }

            set
            { this.expandoObject = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a named item from an <see cref="IDictionary<string, object>"/>
        /// and applies a <see cref="ValueConverter"/>.
        /// </summary>
        /// <typeparam name="TValue">
        /// The type of value to return.
        /// </typeparam>
        /// </param>
        /// <param name="name">
        /// The name of the item to be returned.
        /// </param>
        /// <param name="valueConverter">
        /// The <see cref="ValueConverter"/> applied to the item identified by
        /// <see cref="name"/>.
        /// </param>
        /// <returns>
        /// A value of type <see cref="TValue"/>.
        /// </returns>
        /// <remarks>
        /// The value returned by this method is stored into <see cref="record"/>
        /// to reduce conversion overhead.
        /// </remarks>

        protected TValue GetValue<TValue>(string name, ValueConverter<TValue> valueConverter)
        {
            Contract.Requires<InvalidOperationException>(this.ExpandoObject != null);
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentNullException>(valueConverter != null);

            var dictionary = (IDictionary<string, object>)this.ExpandoObject;
            object value;

            if (!dictionary.TryGetValue(name, out value))
            {
                return valueConverter.DefaultValue;
            }

            if (value is TValue)
            {
                return (TValue)value;
            }

            var x = valueConverter.Convert(value);
            dictionary[name] = x;

            return x;
        }

        #endregion

        #region Privates

        private volatile ExpandoObject expandoObject;

        #endregion
    }
}
