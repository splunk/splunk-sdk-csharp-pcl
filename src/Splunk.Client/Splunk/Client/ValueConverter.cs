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

// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides a way to convert objects to values of some type.
    /// </summary>
    /// <typeparam name="TValue">
    /// The type of values to produce in the conversion.
    /// </typeparam>
    /// <remarks>
    /// If you want to create a value converter, create a class that implements 
    /// the <see cref="Convert"/> method and--optionally--overrides the 
    /// <see cref="DefaultValue"/> property. Your <see cref="Convert"/> method
    /// should accept a value of any type and produce a <typeparamref name="TValue"/>
    /// or throw an <see cref="InvalidDataException"/>.
    /// </remarks>
    public abstract class ValueConverter<TValue>
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual TValue DefaultValue
        { 
            get { return default(TValue); } 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract TValue Convert(object value);

        /// <summary>
        /// 
        /// </summary>
        protected static readonly EqualityComparer<TValue> Comparer = EqualityComparer<TValue>.Default;

        /// <summary>
        /// 
        /// </summary>
        protected static readonly string TypeName = typeof(TValue).Name;
    }
}
