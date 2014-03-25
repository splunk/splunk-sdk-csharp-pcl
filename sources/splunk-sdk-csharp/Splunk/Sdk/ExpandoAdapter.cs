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
// [O] Documentation

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Dynamic;

    public class ExpandoAdapter
    {
        #region Constructors

        public ExpandoAdapter(ExpandoObject expandoObject)
        {
            Contract.Requires<InvalidOperationException>(expandoObject != null);
            this.expandoObject = expandoObject;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a named item from the underlying <see cref="ExpandoObject"/>"/>
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
        /// The value returned by this method is stored into underlying <see 
        /// cref="ExpandoObject"/> to reduce conversion overhead. 
        /// </remarks>
        public TValue GetValue<TValue>(string name, ValueConverter<TValue> valueConverter)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentNullException>(valueConverter != null);

            if (this.expandoObject == null)
            {
                throw new InvalidOperationException(); // TODO: diagnostics
            }

            var dictionary = (IDictionary<string, object>)this.expandoObject;
            object value;

            if (!dictionary.TryGetValue(name, out value))
            {
                return valueConverter.DefaultValue;
            }

            if (value is ConvertedValue)
            {
                return ((ConvertedValue)value).Convert<TValue>(valueConverter);
            }

            // Tradeoff: 
            // Risk paying extra for conversion in order to minimize lock time.
            //
            // Rationale: 
            // It is unsafe to assume that value converters will not cause 
            // problems in a critical section. We only lock on code/data that's
            // under our direct control.
            
            object convertedValue;
            int count = 0;

            do
            {
                convertedValue = valueConverter.Convert(value);

                lock (this.gate)
                {
                    value = dictionary[name];

                    if (value is ConvertedValue)
                    {
                        convertedValue = ((ConvertedValue)value).GetAs<TValue>();
                        value = ((ConvertedValue)value).Get();
                    }
                    else
                    {
                        dictionary[name] = new ConvertedValue(convertedValue);
                    }
                }

                Debug.Assert(++count < 2);
            }
            while (convertedValue == null);

            return (TValue)convertedValue;
        }

        #endregion

        #region Privates

        ExpandoObject expandoObject;
        object gate = new object();

        #endregion

        #region Type

        class ConvertedValue
        {
            public ConvertedValue(object value)
            {
                this.value = value;
            }

            public TValue Convert<TValue>(ValueConverter<TValue> valueConverter)
            {
                return this.value is TValue ? (TValue)this.value : valueConverter.Convert(this.value);
            }

            public object Get()
            {
                return this.value;                
            }

            public object GetAs<TValue>()
            {
                return this.value is TValue ? value : null;
            }

            readonly object value;
        }

        #endregion
    }
}
