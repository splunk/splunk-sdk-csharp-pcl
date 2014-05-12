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
    /// Provides a converter to convert a string to a relative or absolute 
    /// <see cref="Uri"/> instance.
    /// </summary>
    sealed class UriConverter : ValueConverter<Uri>
    {
        /// <summary>
        /// The default <see cref="UriConverter"/> instance.
        /// </summary>
        public static readonly UriConverter Instance = new UriConverter();

        /// <summary>
        /// Converts the string representation of the <paramref name="input"/> 
        /// object to a <see cref="Uri"/> instance.
        /// </summary>
        /// <param name="input">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// Result of the conversion.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// The <paramref name="input"/> does not represent a <see cref="Uri"/>.
        /// </exception>
        public override Uri Convert(object input)
        {
            var value = input as Uri;

            if (value != null)
            {
                return value;
            }

            if (Uri.TryCreate(input.ToString(), UriKind.RelativeOrAbsolute, out value))
            {
                return value;
            }

            throw new InvalidDataException(string.Format("Expected {0}: {1}", TypeName, input));  // TODO: improved diagnostices
        }
    }
}
