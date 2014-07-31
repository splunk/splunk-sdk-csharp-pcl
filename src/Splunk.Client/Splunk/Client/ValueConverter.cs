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
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Provides a way to convert objects to values of some type.
    /// </summary>
    /// <remarks>
    /// If you want to create a value converter, create a class that implements
    /// the <see cref="Convert"/> method and--optionally--overrides the
    /// <see cref="DefaultValue"/> property. Your <see cref="Convert"/> method
    /// should accept a value of any type and produce a
    /// <typeparamref name="TValue"/>
    /// or throw an <see cref="InvalidDataException"/>.
    /// </remarks>
    /// <typeparam name="TValue">
    /// Type of the value.
    /// </typeparam>
    [ContractClass(typeof(ValueConverterContract<>))]
    public abstract class ValueConverter<TValue>
    {
        #region Properties

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        public virtual TValue DefaultValue
        { 
            get { return default(TValue); } 
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts the given input.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// A TValue.
        /// </returns>
        public abstract TValue Convert(object input);

        /// <summary>
        /// Creates a new invalid data exception.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// An InvalidDataException.
        /// </returns>
        protected static InvalidDataException NewInvalidDataException(object input)
        {
            var text = string.Format(CultureInfo.CurrentCulture, "Expected {0} value: {1}", TypeName, input);
            return new InvalidDataException(text);
        }

        #endregion

        /// <summary>
        /// The comparer.
        /// </summary>
        protected static readonly EqualityComparer<TValue> Comparer = EqualityComparer<TValue>.Default;

        /// <summary>
        /// Name of the type.
        /// </summary>
        protected static readonly string TypeName = typeof(TValue).Name;
    }

    /// <summary>
    /// A value converter contract.
    /// </summary>
    /// <typeparam name="TValue">
    /// Type of the value.
    /// </typeparam>
    /// <seealso cref="T:Splunk.Client.ValueConverter{TValue}"/>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification =
        "Contract classes should be contained in the same C# document as the class they reprsent.")
    ]
    [ContractClassFor(typeof(ValueConverter<>))]
    abstract class ValueConverterContract<TValue> : ValueConverter<TValue>
    {
        public override TValue Convert(object value)
        {
            Contract.Requires<ArgumentNullException>(value != null);
            return default(TValue);
        }
    }
}
