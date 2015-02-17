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

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Dynamic;

    /// <summary>
    /// Provides a base class for implementing strong types backed by
    /// <see cref="System.Dynamic.ExpandoObject"/> instances.
    /// </summary>
    /// <seealso cref="T:System.Dynamic.DynamicObject"/>
    public class ExpandoAdapter : DynamicObject
    {
        #region Constructors

        /// <summary>
        /// Intializes a new instance of the <see cref="ExpandoAdapter"/>
        /// class.
        /// </summary>
        /// <param name="expandoObject">
        /// An object backing the current <see cref="ExpandoAdapter"/>.
        /// </param>
        protected ExpandoAdapter(ExpandoObject expandoObject)
        {
            Contract.Requires<InvalidOperationException>(expandoObject != null);
            this.expandoObject = expandoObject;
        }

        /// <summary>
        /// Infrastructure. Initializes a new instance of the
        /// <see cref="ExpandoAdapter"/>
        /// class.
        /// </summary>
        public ExpandoAdapter()
        { }

        #endregion

        #region Fields

        /// <summary>
        /// The empty.
        /// </summary>
        public static readonly ExpandoAdapter Empty = new ExpandoAdapter(new ExpandoObject());

        #endregion

        #region Methods

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>
        /// The list of dynamic member names.
        /// </returns>
        /// <seealso cref="M:System.Dynamic.DynamicObject.GetDynamicMemberNames()"/>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return ((IDictionary<string, object>)this.Object).Keys;
        }

        /// <summary>
        /// Gets a named item from the
        /// <see cref="System.Dynamic.ExpandoObject"/>"/&gt;
        /// backing the current <see cref="ExpandoAdapter"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the item to be returned.
        /// </param>
        /// <returns>
        /// A dynamic value.
        /// </returns>
        public dynamic GetValue(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null);

            var dictionary = (IDictionary<string, object>)this.Object;
            object value;
            dictionary.TryGetValue(name, out value);
            
            return value;
        }

        /// <summary>
        /// Gets a named item from the underlying <see cref="Object"/>"/&gt;
        /// and applies a <see cref="ValueConverter&lt;TValue&gt;"/>.
        /// </summary>
        /// <remarks>
        /// The value returned by this method is stored into the backing
        /// <see cref="System.Dynamic.ExpandoObject"/> to reduce conversion overhead.
        /// </remarks>
        /// <typeparam name="TValue">
        /// Type of the value.
        /// </typeparam>
        /// <param name="name">
        /// The name of the item to be returned.
        /// </param>
        /// <param name="valueConverter">
        /// The <see cref="ValueConverter&lt;TValue&gt;"/> applied to the item
        /// identified by <paramref name="name"/>.
        /// </param>
        /// <returns>
        /// A value of type <typeparamref name="TValue"/>.
        /// </returns>
        public TValue GetValue<TValue>(string name, ValueConverter<TValue> valueConverter)
        {
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentNullException>(valueConverter != null);

            var dictionary = (IDictionary<string, object>)this.Object;
            object value;

            if (!dictionary.TryGetValue(name, out value) || value == null)
            {
                return valueConverter.DefaultValue;
            }

            var convertedValue = value as ConvertedValue;

            if (convertedValue != null)
            {
                return convertedValue.Convert<TValue>(valueConverter);
            }

            // Tradeoff: 
            // Risk paying extra for conversion in order to minimize lock time.
            //
            // Rationale: 
            // It is unsafe to assume that value converters will not cause 
            // problems in a critical section. We only lock on code/data that's
            // under our direct control.
            
            object unconvertedValue;
            int count = 0;

            do
            {
                unconvertedValue = valueConverter.Convert(value);

                lock (this.gate)
                {
                    value = dictionary[name];
                    convertedValue = value as ConvertedValue;

                    if (convertedValue != null)
                    {
                        unconvertedValue = convertedValue.GetAs<TValue>();
                        value = convertedValue.Get();
                    }
                    else
                    {
                        dictionary[name] = new ConvertedValue(unconvertedValue);
                    }
                }

                Debug.Assert(++count < 2, string.Concat("count: ", count));
            }
            while (unconvertedValue == null);

            return (TValue)unconvertedValue;
        }

        /// <summary>
        /// Provides the implementation for operations that get dynamic member values.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Thrown when the requested operation is not supported.
        /// </exception>
        /// <param name="binder">
        /// Provides information about the object that called the dynamic operation.
        /// The <paramref name="binder"/>.Name property provides the name of the
        /// member on which the dynamic operation is performed. The
        /// <paramref name="binder"/>.IgnoreCase property specifies whether the
        /// member name is case-sensitive.
        /// </param>
        /// <param name="result">
        /// The result of the operation.
        /// </param>
        /// <returns>
        /// <c>true</c> if the operation is successful; otherwise, <c>false</c>. If
        /// this method returns <c>false</c>, the run-time binder of the language
        /// determines the behavior. In most cases, a run-time exception is thrown.
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder.IgnoreCase)
            {
                throw new NotSupportedException("Case insensitive language bindings are not supported");
            }

            result = this.GetValue(binder.Name);
            return result != null;
        }

        #endregion

        #region Privates/internals

        ExpandoObject expandoObject;
        object gate = new object();

        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the requested operation is invalid.
        /// </exception>
        /// <value>
        /// The object.
        /// </value>
        internal ExpandoObject Object
        { 
            get
            {
                return this.expandoObject;
            }

            set
            {
                Debug.Assert(this.Object == null, "Object is not null");

                if (this.expandoObject != null)
                {
                    throw new InvalidOperationException("ExpandoAdapter.Object is already initialized."); // TODO: diagnostics
                }

                this.expandoObject = value;
            }
        }

        #endregion

        #region Types

        /// <summary>
        /// A converted value.
        /// </summary>
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

            public override string ToString()
            {
                return this.value.ToString();
            }

            readonly object value;
        }

        #region Types

        /// <summary>
        /// Provides a converter to create <see cref="ExpandoAdapter"/>
        /// instances from <see cref="System.Dynamic.ExpandoObject"/>
        /// instances.
        /// </summary>
        /// <seealso cref="T:Splunk.Client.ValueConverter{Splunk.Client.ExpandoAdapter}"/>
        public class Converter : ValueConverter<ExpandoAdapter>
        {
            static Converter()
            {
                Instance = new Converter();
            }

            /// <summary>
            /// Gets the instance.
            /// </summary>
            /// <value>
            /// The instance.
            /// </value>
            public static Converter Instance
            { get; private set; }

            /// <summary>
            /// Converts the given input.
            /// </summary>
            /// <param name="input">
            /// 
            /// </param>
            /// <returns>
            /// An ExpandoAdapter.
            /// </returns>
            /// <exception cref="System.IO.InvalidDataException">
            /// The <paramref name="input"/> does not represent an <see cref=
            /// "ExpandoAdapter"/>.
            /// value.
            /// </exception>
            public override ExpandoAdapter Convert(object input)
            {
                var value = input as ExpandoAdapter;

                if (value != null)
                {
                    return value;
                }

                var expandoObject = input as ExpandoObject;

                if (expandoObject != null)
                {
                    return new ExpandoAdapter(expandoObject);
                }

                throw NewInvalidDataException(input);
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Provides a generic base class for implementing strong types backed by
    /// <see cref="System.Dynamic.ExpandoObject"/> instances.
    /// </summary>
    /// <typeparam name="TExpandoAdapter">
    /// Type of the adapter.
    /// </typeparam>
    /// <seealso cref="T:Splunk.Client.ExpandoAdapter"/>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = 
		"Generic and non-generic versions of a class should be contained in the same C# document.")
	]
    public class ExpandoAdapter<TExpandoAdapter> : ExpandoAdapter where TExpandoAdapter : ExpandoAdapter, new()
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ExpandoAdapter class.
        /// </summary>
        public ExpandoAdapter()
        { }

        #endregion

        #region Types

        /// <summary>
        /// Provides a converter to create <see cref="ExpandoAdapter"/>
        /// instances from <see cref="System.Dynamic.ExpandoObject"/>
        /// instances.
        /// </summary>
        /// <seealso cref="T:Splunk.Client.ValueConverter{TExpandoAdapter}"/>
        new public class Converter : ValueConverter<TExpandoAdapter>
        {
            static Converter()
            {
                Instance = new Converter();
            }

            /// <summary>
            /// Gets the instance.
            /// </summary>
            /// <value>
            /// The instance.
            /// </value>
            public static Converter Instance
            { get; private set; }

            /// <summary>
            /// Converts the given input.
            /// </summary>
            /// <param name="input">
            /// 
            /// </param>
            /// <returns>
            /// A TExpandoAdapter.
            /// </returns>
            /// <exception cref="System.IO.InvalidDataException">
            /// The <paramref name="input"/> does not represent an <see cref=
            /// "ExpandoAdapter"/>.
            /// </exception>
            public override TExpandoAdapter Convert(object input)
            {
                var value = input as TExpandoAdapter;

                if (value != null)
                {
                    return value;
                }

                var expandoObject = input as ExpandoObject;

                if (expandoObject != null)
                {
                    return new TExpandoAdapter() { Object = expandoObject };
                }

                throw NewInvalidDataException(input);
            }
        }

        #endregion
    }
}
