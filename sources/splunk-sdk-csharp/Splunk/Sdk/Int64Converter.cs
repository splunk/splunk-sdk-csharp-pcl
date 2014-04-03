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

// TODO:
// [ ] Contracts
// [O] Documentation

namespace Splunk.Sdk
{
    using System;
    using System.IO;

    /// <summary>
    /// Provides a converter to convert strings to <see cref="Int64"/> values.
    /// </summary>
    sealed class Int64Converter : ValueConverter<Int64>
    {
        static Int64Converter()
        {
            Instance = new Int64Converter();
        }

        public static Int64Converter Instance
        { get; private set; }

        public override Int64 Convert(object input)
        {
            var x = input as Int64?;

            if (x != null)
            {
                return x.Value;
            }

            Int64 value;

            if (Int64.TryParse(input.ToString(), result: out value))
            {
                return value;
            }

            throw new InvalidDataException(string.Format("Expected {0}: {1}", TypeName, input)); // TODO: improved diagnostices
        }
    }
}
