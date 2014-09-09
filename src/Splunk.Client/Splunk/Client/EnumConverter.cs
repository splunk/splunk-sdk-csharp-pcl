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
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides a converter to convert strings to <see cref="Enum"/> values.
    /// </summary>
    /// <typeparam name="TEnum">
    /// Type of the <see cref="Enum"/> value to convert.
    /// </typeparam>
    /// <seealso cref="T:Splunk.Client.ValueConverter{TEnum}"/>
    sealed class EnumConverter<TEnum> : ValueConverter<TEnum> where TEnum : struct
    {
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification =
            "This is by design")
        ]
        static EnumConverter()
        {
            var type = typeof(TEnum);
            var names = Enum.GetNames(type);
            var values = (TEnum[])Enum.GetValues(type);

            var inputConversionTable = new Dictionary<string, TEnum>(names.Length, StringComparer.OrdinalIgnoreCase);

            foreach (var member in names.Zip(values, (name, value) => new KeyValuePair<string, TEnum>(name, value)))
            {
                var attribute = type.GetRuntimeField(member.Key).GetCustomAttribute<EnumMemberAttribute>();
                var name = attribute == null ? member.Key : attribute.Value;

                inputConversionTable[name] = member.Value;
            }

            Instance = new EnumConverter<TEnum>();
            InputConversionTable = inputConversionTable;
        }

        /// <summary>
        /// The default <see cref="EnumConverter&lt;TEnum&gt;"/> instance.
        /// </summary>
        public static readonly EnumConverter<TEnum> Instance;

        /// <summary>
        /// Converts the string representation of the <paramref name="input"/>
        /// object to a <typeparamref name="TEnum"/> value.
        /// </summary>
        /// <param name="input">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// Result of the conversion.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// The <paramref name="input"/> does not represent a
        /// <typeparamref name="TEnum"/>
        /// value.
        /// </exception>
        public override TEnum Convert(object input)
        {
            var x = input as TEnum?;

            if (x != null)
            {
                return x.Value;
            }

            TEnum value;

            if (InputConversionTable.TryGetValue(input.ToString(), out value))
            {
                return value;
            }

            throw NewInvalidDataException(input);
        }

        static readonly IReadOnlyDictionary<string, TEnum> InputConversionTable;
    }
}
