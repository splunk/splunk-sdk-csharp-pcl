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
    /// Provides a converter to convert strings to <see cref="Double"/> values.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.ValueConverter{System.Double}"/>
    sealed class DoubleConverter : ValueConverter<Double>
    {
        /// <summary>
        /// The default <see cref="DoubleConverter"/> instance.
        /// </summary>
        public static readonly DoubleConverter Instance = new DoubleConverter();

        /// <summary>
        /// Converts the string representation of the <paramref name="input"/>
        /// object to a <see cref="Double"/> value.
        /// </summary>
        /// <param name="input">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// Result of the conversion.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// The <paramref name="input"/> does not represent a <see cref="Double"/>
        /// value.
        /// </exception>
        public override Double Convert(object input)
        {
            var x = input as Double?;

            if (x != null)
            {
                return x.Value;
            }

            Double value;

            if (Double.TryParse(input.ToString(), result: out value))
            {
                return value;
            }

            throw NewInvalidDataException(input);
        }
    }
}
