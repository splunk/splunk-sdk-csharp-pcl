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

    /// <summary>
    /// Provides a converter to convert an object to its string form.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.ValueConverter{System.String}"/>
    sealed class StringConverter : ValueConverter<String>
    {
        /// <summary>
        /// The default <see cref="StringConverter"/> instance.
        /// </summary>
        public static readonly StringConverter Instance = new StringConverter();

        /// <summary>
        /// Converts the <paramref name="input"/> object to a <see cref="String"/>
        /// value.
        /// </summary>
        /// <param name="input">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// Result of the conversion.
        /// </returns>
        public override String Convert(object input)
        {
            return input.ToString();
        }
    }
}
