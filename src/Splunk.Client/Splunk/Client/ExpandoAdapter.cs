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
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.IO;

    /// <summary>
    /// Provides a base class for implementing strong types backed by <see 
    /// cref="System.Dynamic.ExpandoObject"/> instances.
    /// </summary>
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
        /// Infrastructure. Initializes a new instance of the <see cref="ExpandoAdapter"/>
        /// class.
        /// </summary>
        public ExpandoAdapter()
        { }

        #endregion

        #region Fields

        /// <summary>
        /// 
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
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return ((IDictionary<string, object>)this.Object).Keys;
        }

        /// <summary>
        /// Gets a named item from the <see cref="System.Dynamic.ExpandoObject"/>"/>
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
        /// Gets a named item from the underlying <see cref="Object"/>"/>
        /// and applies a <see cref="ValueConverter&lt;TValue&gt;"/>.
        /// </summary>
        /// <typeparam name="TValue">
        /// The type of value to return.
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
        /// <remarks>
        /// The value returned by this method is stored into the backing <see 
        /// cref="System.Dynamic.ExpandoObject"/> to reduce conversion 
        /// overhead.
        /// </remarks>
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

                Debug.Assert(++count < 2, string.Concat("count: ", count));
            }
            while (convertedValue == null);

            return (TValue)convertedValue;
        }

        /// <summary>
        /// Provides the implementation for operations that get dynamic member
        /// values.
        /// </summary>
        /// <param name="binder">
        /// Provides information about the object that called the dynamic 
        /// operation. The <paramref name="binder"/>.Name property provides the
        /// name of the member on which the dynamic operation is performed.
        /// The <paramref name="binder"/>.IgnoreCase property specifies whether
        /// the member name is case-sensitive.
        /// </param>
        /// <param name="result">
        /// The result of the operation.
        /// </param>
        /// <returns>
        /// <c>true</c> if the operation is successful; otherwise, <c>false</c>.
        /// If this method returns <c>false</c>, the run-time binder of the 
        /// language determines the behavior. In most cases, a run-time 
        /// exception is thrown.
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
        public class Converter : ValueConverter<ExpandoAdapter>
        {
            static Converter()
            {
                Instance = new Converter();
            }

            /// <summary>
            /// 
            /// </summary>
            public static Converter Instance
            { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="input">
            /// 
            /// </param>
            /// <returns>
            /// 
            /// </returns>
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

                throw new InvalidDataException(string.Format("Expected {0}: {1}", TypeName, input)); // TODO: improved diagnostices
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
    /// The type inheriting from this class.
    /// </typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = 
        "Generic and non-generic versions of a class should be contained in the same C# document.")]
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
        new public class Converter : ValueConverter<TExpandoAdapter>
        {
            static Converter()
            {
                Instance = new Converter();
            }

            /// <summary>
            /// 
            /// </summary>
            public static Converter Instance
            { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="input">
            /// 
            /// </param>
            /// <returns>
            /// 
            /// </returns>
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

                throw new InvalidDataException(string.Format("Expected {0}: {1}", TypeName, input)); // TODO: improved diagnostices
            }
        }

        #endregion
    }
}
