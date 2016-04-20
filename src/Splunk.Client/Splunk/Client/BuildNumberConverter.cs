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
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Provides a converter to convert strings to <see cref="Int64"/> values.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.ValueConverter{System.Int32}"/>
    sealed class BuildNumberConverter : ValueConverter<Int64>
    {
        /// <summary>
        /// The default <see cref="EnumConverter&lt;TEnum&gt;"/> instance.
        /// </summary>
        public static readonly BuildNumberConverter Instance = new BuildNumberConverter();

        /// <summary>
        /// Converts the string representation of the <paramref name="input"/> object to an <see cref="Int64"/> build
        /// number.
        /// </summary>
        /// <param name="input">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// Result of the conversion.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// The <paramref name="input"/> does not represent an <see cref="Int64"/> value.
        /// </exception>
        /// <remarks>
        /// If the string representation <paramref name="input"/> is exactly 12 bytes long, it is assumed to be a 
        /// hexadecimal Git commit number; otherwise, it is assumed to be an unsigned 32-bit decimal value.
        /// </remarks>
        public override Int64 Convert(object input)
        {
            var x = input as Int64?;

            if (x != null)
            {
                return x.Value;
            }

            var value = input.ToString();

            if (value.Length == 12)  // Assume hexadecimal Git commit number
            {
                Int64 result;

                if (Int64.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }
            }
            else  // Assume unsigned 32-bit decimal value
            {
                Int32 result;

                if (Int32.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, result: out result))
                {
                    return result;
                }
            }

            throw NewInvalidDataException(input);
        }
    }
}
