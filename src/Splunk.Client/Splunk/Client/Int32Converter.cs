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
    using System.IO;

    /// <summary>
    /// Provides a converter to convert strings to <see cref="Int32"/> values.
    /// </summary>
    sealed class Int32Converter : ValueConverter<Int32>
    {
        /// <summary>
        /// The default <see cref="EnumConverter&lt;TEnum&gt;"/> instance.
        /// </summary>
        public static readonly Int32Converter Instance = new Int32Converter();

        /// <summary>
        /// Converts the string representation of the <paramref name="input"/> 
        /// object to a <see cref="Int32"/> value.
        /// </summary>
        /// <param name="input">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// Result of the conversion.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// The <paramref name="input"/> does not represent a <see cref="Int32"/>
        /// value.
        /// </exception>
        public override Int32 Convert(object input)
        {
            var x = input as Int32?;

            if (x != null)
            {
                return x.Value;
            }

            Int32 value;

            if (Int32.TryParse(input.ToString(), result: out value))
            {
                return value;
            }

            throw new InvalidDataException(string.Format("Expected {0}: {1}", TypeName, input)); // TODO: improved diagnostices
        }
    }
}
