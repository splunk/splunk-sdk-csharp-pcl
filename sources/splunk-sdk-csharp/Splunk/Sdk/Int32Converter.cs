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
// [X] Contracts are in the base type: ValueConverter
// [ ] Documentation

namespace Splunk.Sdk
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    sealed class Int32Converter : ValueConverter<Int32>
    {
        public static readonly Int32Converter Default = new Int32Converter();

        public override Int32 Convert(object input)
        {
            Int32 value;

            if (Int32.TryParse(input.ToString(), result: out value))
            {
                return value;
            }

            throw new InvalidDataException(string.Format("Expected {0}: {1}", TypeName, input)); // TODO: improved diagnostices
        }
    }
}
