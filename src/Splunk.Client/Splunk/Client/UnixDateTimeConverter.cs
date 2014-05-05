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
    using System.Diagnostics.Contracts;
    using System.IO;

    /// <summary>
    /// Provides a converter to convert Unix epoch time strings to <see cref=
    /// "DateTime"/> values.
    /// </summary>
    sealed class UnixDateTimeConverter : ValueConverter<DateTime>
    {
        /// <summary>
        /// The default <see cref="UnixDateTimeConverter"/> instance.
        /// </summary>
        public static readonly UnixDateTimeConverter Instance = new UnixDateTimeConverter();

        /// <summary>
        /// Converts the string representation of a <see cref="input"/> Unix 
        /// time value to a <see cref="DateTime"/> value.
        /// </summary>
        /// <param name="input">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// Result of the conversion.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// The <see cref="input"/> does not represent a <see cref="DateTime"/>
        /// value.
        /// </exception>
        public override DateTime Convert(object input)
        {
            var x = input as DateTime?;

            if (x != null)
            {
                return x.Value;
            }

            Int64 value;

            if (Int64.TryParse(input.ToString(), result: out value))
            {
                var dateTime = UnixEpoch.AddSeconds(value).ToLocalTime();
                return dateTime;
            }

            throw new InvalidDataException(string.Format("Expected {0}: {1}", TypeName, input));  // TODO: improved diagnostices
        }

        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
