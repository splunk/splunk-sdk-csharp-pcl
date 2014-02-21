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

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TArgs"></typeparam>
    public abstract class Args<TArgs> where TArgs : Args<TArgs>
    {
        // TODO: Make this crude implementation production quality
        // * Implement support for enumerations
        // * Respect DataMemberAttribute.Order
        // * Rework this into a real serializer, not just a ToString implementation tool (?)
        // * Work on nomenclature (serialization nomenclature is not necessarily appropriate)

        static readonly IReadOnlyDictionary<Type, Func<TArgs, SerializationInfo, string>> Serializers = new Dictionary<Type, Func<TArgs, SerializationInfo, string>>()
        {
            { typeof(bool), FormatBoolean },
            { typeof(byte), FormatNumber },
            { typeof(sbyte), FormatNumber },
            { typeof(short), FormatNumber },
            { typeof(ushort), FormatNumber },
            { typeof(int), FormatNumber },
            { typeof(uint), FormatNumber },
            { typeof(long), FormatNumber },
            { typeof(ulong), FormatNumber },
            { typeof(float), FormatNumber },
            { typeof(double),  FormatNumber },
            { typeof(decimal), FormatNumber },
            { typeof(string), FormatString }
        };

        static Args()
        {
            var serializationInfos = new List<SerializationInfo>();

            foreach (PropertyInfo property in typeof(TArgs).GetRuntimeProperties())
            {
                var dataMember = property.GetCustomAttribute<DataMemberAttribute>();
                Func<TArgs, SerializationInfo, string> serializer;

                if (!Serializers.TryGetValue(property.PropertyType, out serializer))
                {
                    serializer = FormatString;
                }

                serializationInfos.Add(new SerializationInfo(dataMember, property, serializer));
            }

            SerializationInfos = serializationInfos;
        }

        public override string ToString()
        {
            return string.Join("&",
                from info in Args<TArgs>.SerializationInfos select info.Serialize((TArgs)this, info));
        }

        static readonly IReadOnlyList<SerializationInfo> SerializationInfos;

        static string FormatBoolean(TArgs args, SerializationInfo info)
        {
            return string.Concat(info.DataMember.Name, "=", (bool)info.Property.GetValue(args) ? "t" : "f");
        }

        static string FormatEnum(TArgs args, SerializationInfo info)
        {
            throw new NotImplementedException();
        }

        static string FormatNumber(TArgs args, SerializationInfo info)
        {
            return string.Concat(info.DataMember.Name, "=", info.Property.GetValue(args).ToString());
        }

        static string FormatString(TArgs args, SerializationInfo info)
        {
            return string.Concat(info.DataMember.Name, "=", Uri.EscapeUriString(info.Property.GetValue(args).ToString()));
        }

        static string FormatStringList(TArgs args, SerializationInfo info)
        {
            string name = info.DataMember.Name;
            
            return string.Join("&", 
                from value in (IReadOnlyList<string>)info.Property.GetValue(args) select string.Concat(name, "=", value));
        }

        class SerializationInfo
        {
            public SerializationInfo(DataMemberAttribute dataMember, PropertyInfo property, Func<TArgs, SerializationInfo, string> serializer)
            {
                dataMember.Name = Uri.EscapeUriString(dataMember.Name);
                this.DataMember = dataMember;
                this.Property = property;
                this.Serialize = serializer;
            }
            public DataMemberAttribute DataMember
            { get; private set; }

            public PropertyInfo Property
            { get; private set; }

            public Func<TArgs, SerializationInfo, string> Serialize
            { get; private set; }
        }
    }
}
