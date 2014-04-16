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

namespace Splunk.Client
{
    using System.Dynamic;
    using System.IO;

    /// <summary>
    /// Provides a converter to convert <see cref="ExpandoObject"/> instances to
    /// <see cref="Permissions"/> objects.
    /// </summary>
    sealed class PermissionsConverter : ValueConverter<Permissions>
    {
        static PermissionsConverter()
        {
            Instance = new PermissionsConverter();
        }

        public static PermissionsConverter Instance
        { get; private set; }

        public override Permissions Convert(object input)
        {
            var value = input as Permissions;

            if (value != null)
            {
                return value;
            }

            var expandoObject = input as ExpandoObject;

            if (expandoObject != null)
            {
                return new Permissions(expandoObject);
            }

            throw new InvalidDataException(string.Format("Expected {0}: {1}", TypeName, input)); // TODO: improved diagnostices
        }
    }
}
