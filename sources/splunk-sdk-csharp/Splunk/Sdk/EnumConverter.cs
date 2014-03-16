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
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    sealed class EnumConverter<TEnum> : ValueConverter<TEnum> where TEnum : struct
    {
        static EnumConverter()
        {
            var type = typeof(TEnum);
            var names = Enum.GetNames(type);
            var values = (TEnum[])Enum.GetValues(type);

            var inputConversionTable = new Dictionary<string, TEnum>(names.Length, StringComparer.OrdinalIgnoreCase);
            var outputConversionTable = new Dictionary<TEnum, string>(names.Length);

            foreach (var member in names.Zip(values, (name, value) => new KeyValuePair<string, TEnum>(name, value)))
            {
                var attribute = type.GetRuntimeField(member.Key).GetCustomAttribute<EnumMemberAttribute>();
                var name = attribute == null ? member.Key : attribute.Value;

                inputConversionTable[name] = member.Value;
                outputConversionTable[member.Value] = name;
            }

            Instance = new EnumConverter<TEnum>();
            InputConversionTable = inputConversionTable;
            OutputConversionTable = outputConversionTable;
        }

        public static EnumConverter<TEnum> Instance
        { get; private set; }

        public override TEnum Convert(object input)
        {
            var x = input as TEnum?;

            if (x != null)
            {
                return x.Value;
            }

            TEnum value;

            if (InputConversionTable.TryGetValue(input.ToString(), out value))
            {
                return value;
            }

            throw new InvalidDataException(string.Format("Expected {0}: {1}", TypeName, input)); // TODO: improved diagnostices
        }

        static readonly IReadOnlyDictionary<string, TEnum> InputConversionTable;
        static readonly IReadOnlyDictionary<TEnum, string> OutputConversionTable;
    }
}
