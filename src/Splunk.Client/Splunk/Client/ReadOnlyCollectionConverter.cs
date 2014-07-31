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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Provides a converter to convert an <see cref="IEnumerable&lt;T&gt;"/>
    /// object to a collection.
    /// </summary>
    /// <typeparam name="TValue">
    /// Type of the value.
    /// </typeparam>
    /// <typeparam name="TCollection">
    /// Type of the collection.
    /// </typeparam>
    /// <typeparam name="TConverter">
    /// Type of the converter.
    /// </typeparam>
    /// <seealso cref="T:Splunk.Client.ValueConverter{TCollection}"/>
    sealed class ReadOnlyCollectionConverter<TCollection, TConverter, TValue>
        : ValueConverter<ReadOnlyCollection<TValue>>
        where TCollection : IList<TValue>, new()
        where TConverter : ValueConverter<TValue>, new()
    {
        /// <summary>
        /// The default <see cref="ReadOnlyCollectionConverter&lt;TCollection, TConverter, TValue&gt;"/> instance.
        /// </summary>
        public static readonly ReadOnlyCollectionConverter<TCollection, TConverter, TValue> Instance =
            new ReadOnlyCollectionConverter<TCollection, TConverter, TValue>();

        /// <summary>
        /// Converts the given input.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// A read only collection of <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="System.IO.InvalidDataException">
        /// The <paramref name="input"/> does not represent a collection of
        /// <typeparamref name="TValue"/> items.
        /// </exception>
        public override ReadOnlyCollection<TValue> Convert(object input)
        {
            var list = input as IEnumerable<object>;

            if (list == null)
            {
                throw NewInvalidDataException(input);
            }

            var collection = new TCollection();

            foreach (var value in list)
            {
                collection.Add(ValueConverter.Convert(value));
            }

            return new ReadOnlyCollection<TValue>(collection);
        }

        static readonly TConverter ValueConverter = new TConverter();
    }
}
