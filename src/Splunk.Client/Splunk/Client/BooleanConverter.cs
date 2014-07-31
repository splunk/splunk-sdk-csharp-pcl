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
    /// Provides a converter to convert strings to <see cref="Boolean"/> values.
    /// </summary>
    /// <seealso cref="T:Splunk.Client.ValueConverter{System.Boolean}"/>
    sealed class BooleanConverter : ValueConverter<Boolean>
    {
        #region Fields

        /// <summary>
        /// The default <see cref="BooleanConverter"/> instance.
        /// </summary>
        public static readonly BooleanConverter Instance = new BooleanConverter();

        #endregion

        #region Methods

        /// <summary>
        /// Converts the string representation of an object to a
        /// <see cref= "Boolean"/> value.
        /// </summary>
        /// <param name="input">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// Result of the conversion.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// The <paramref name="input"/> does not represent a <see cref="Boolean"/>
        /// value.
        /// </exception>
        public override Boolean Convert(object input)
        {
            var x = input as Boolean?;

            if (x != null)
            {
                return x.Value;
            }

            string value = input.ToString();

            switch (value)
            {
                case "t": return true;
                case "f": return false;
                case "true": return true;
                case "false": return false;
            }

            Int32 result;

            if (Int32.TryParse(input.ToString(), result: out result))
            {
                return result != 0;
            }

            throw NewInvalidDataException(input);
        }

        #endregion
    }
}
