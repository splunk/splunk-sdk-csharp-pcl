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

    sealed class DateTimeConverter : ValueConverter<DateTime>
    {
        static DateTimeConverter()
        {
            Instance = new DateTimeConverter();
        }

        public static DateTimeConverter Instance
        { get; private set; }

        public override DateTime Convert(object input)
        {
            var x = input as DateTime?;

            if (x != null)
            {
                return x.Value;
            }

            DateTime value;

            if (DateTime.TryParse(input.ToString(), result: out value))
            {
                return value;
            }

            throw new InvalidDataException(string.Format("Expected {0}: {1}", TypeName, input));  // TODO: improved diagnostices
        }
    }
}
