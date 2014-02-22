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
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TArgs"></typeparam>
    public abstract class Args<TArgs> where TArgs : Args<TArgs>
    {
        // TODO: Ensure this code is solid
        // [ ] Respect DataMemberAttribute.Order
        // [ ] Ensure correct behavior with nullable boolean/numeric values
        // [ ] Do not serialize default values => define default values and check for them
        // [ ] Rework this into a real serializer, not just a ToString implementation tool (?)
        // [ ] Work on nomenclature (serialization nomenclature is not necessarily appropriate)
        // [ ] More, I'm sure...

        #region Constructors

        static Args()
        {
            var propertyFormatters = new Dictionary<Type, Formatter>()
            {
                { typeof(bool),    new Formatter { Format = FormatBoolean } },
                { typeof(byte),    new Formatter { Format = FormatNumber  } },
                { typeof(sbyte),   new Formatter { Format = FormatNumber  } },
                { typeof(short),   new Formatter { Format = FormatNumber  } },
                { typeof(ushort),  new Formatter { Format = FormatNumber  } },
                { typeof(int),     new Formatter { Format = FormatNumber  } },
                { typeof(uint),    new Formatter { Format = FormatNumber  } },
                { typeof(long),    new Formatter { Format = FormatNumber  } },
                { typeof(ulong),   new Formatter { Format = FormatNumber  } },
                { typeof(float),   new Formatter { Format = FormatNumber  } },
                { typeof(double),  new Formatter { Format = FormatNumber  } },
                { typeof(decimal), new Formatter { Format = FormatNumber  } },
                { typeof(string),  new Formatter { Format = FormatString  } }
            };

            var defaultFormatter = new Formatter { Format = FormatString };
            var serializationEntries = new List<SerializationEntry>();

            foreach (PropertyInfo propertyInfo in typeof(TArgs).GetRuntimeProperties())
            {
                var dataMember = propertyInfo.GetCustomAttribute<DataMemberAttribute>();

                if (dataMember == null)
                {
                    throw new InvalidDataContractException(string.Format("Missing DataMemberAttribute on {0}.{1}", propertyInfo.PropertyType.Name, propertyInfo.Name));
                }

                var propertyType = propertyInfo.PropertyType;
                var propertyTypeInfo = propertyType.GetTypeInfo();

                Formatter? formatter = GetPropertyFormatter(null, propertyType, propertyTypeInfo, propertyFormatters);

                if (formatter == null)
                {
                    var interfaces = propertyType.GetTypeInfo().ImplementedInterfaces;
                    var isCollection = false;

                    formatter = defaultFormatter;

                    foreach (Type @interface in interfaces)
                    {
                        if (@interface.IsConstructedGenericType)
                        {
                            if (@interface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                            {
                                Type itemType = @interface.GenericTypeArguments[0];
                                formatter = GetPropertyFormatter(propertyType, itemType, itemType.GetTypeInfo(), propertyFormatters);
                            }
                        }
                        else if (@interface == typeof(IEnumerable))
                        {
                            isCollection = true;
                        }
                    }

                    formatter = new Formatter { Format = formatter.Value.Format, IsCollection = isCollection };
                }

                var defaultValue = propertyInfo.GetCustomAttribute<DefaultValueAttribute>();

                serializationEntries.Add(new SerializationEntry(propertyInfo)
                {
                    // Properties

                    ParameterName = dataMember.Name,
                    ParameterOrder = dataMember.Order,
                    DefaultValue = defaultValue == null ? null : defaultValue.Value,
                    EmitDefaultValue = dataMember.EmitDefaultValue,
                    IsCollection = formatter.Value.IsCollection,
                    IsRequired = dataMember.IsRequired,

                    // Methods

                    Format = (formatter ?? defaultFormatter).Format
                });
            }

            SerializationEntries = serializationEntries;
        }

        protected Args()
        {
            foreach (var serializationEntry in SerializationEntries.Where(entry => entry.DefaultValue != null))
            {
                serializationEntry.SetValue(this, serializationEntry.DefaultValue);
            }
        }

        #endregion

        #region Methods

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (var entry in Args<TArgs>.SerializationEntries)
            {
                object value = entry.GetValue(this);

                if (value == null)
                {
                    if (entry.IsRequired)
                    {
                        throw new SerializationException(string.Format("Missing value for required parameter {0}", entry.ParameterName));
                    }
                    continue;
                }
                if (entry.DefaultValue != null && !entry.EmitDefaultValue && value.Equals(entry.DefaultValue))
                {
                    continue;
                }
                if (!entry.IsCollection)
                {
                    yield return new KeyValuePair<string, string>(entry.ParameterName, entry.Format(value));
                    continue;
                }
                foreach (var item in (IEnumerable)value)
                {
                    yield return new KeyValuePair<string, string>(entry.ParameterName, entry.Format(item));
                }
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            Action<object, string, Func<object, string>> append = (item, name, format) =>
            {
                builder.Append(Uri.EscapeDataString(name));
                builder.Append('=');
                builder.Append(Uri.EscapeDataString(format(item)));
                builder.Append("; ");
            };

            foreach (var entry in Args<TArgs>.SerializationEntries)
            {
                object value = entry.GetValue(this);

                if (value == null)
                {
                    append("null", entry.ParameterName, FormatString);
                    continue;
                }
                if (!entry.IsCollection)
                {
                    append(value, entry.ParameterName, entry.Format);
                    continue;
                }
                foreach (var item in (IEnumerable)value)
                {
                    append(item, entry.ParameterName, entry.Format);
                }
            }

            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 2;
            }

            return builder.ToString();
        }

        #endregion

        #region Privates

        static readonly IReadOnlyList<SerializationEntry> SerializationEntries;

        static string FormatBoolean(object value)
        {
            return (bool)value ? "t" : "f";
        }

        static string FormatNumber(object value)
        {
            return value.ToString();
        }

        static string FormatString(object value)
        {
            return value.ToString();
        }

        static Formatter? GetPropertyFormatter(Type container, Type type, TypeInfo info, Dictionary<Type, Formatter> formatters)
        {
            Formatter formatter;

            if (formatters.TryGetValue(container ?? type, out formatter))
            {
                return formatter;
            }

            if (container != null && formatters.TryGetValue(type, out formatter))
            {
                formatter = new Formatter { Format = formatter.Format, IsCollection = true };
            }
            else if (info.IsEnum)
            {
                var map = new Dictionary<int, string>();

                foreach (var value in Enum.GetValues(type))
                {
                    string name = Enum.GetName(type, value);
                    FieldInfo field = type.GetRuntimeField(name);
                    var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();

                    map[(int)value] = enumMember == null ? name : enumMember.Value;
                }

                formatter = new Formatter { Format = (object value) => map[(int)value], IsCollection = container != null };
            }
            else if (container != null)
            {
                formatter = new Formatter { Format = FormatString, IsCollection = true };
            }
            else
            {
                return null;
            }

            formatters.Add(container ?? type, formatter);
            return formatter;
        }

        #endregion

        #region Types

        struct Formatter
        {
            public Func<object, string> Format;
            public bool IsCollection;
        }

        class SerializationEntry
        {
            public SerializationEntry(PropertyInfo propertyInfo)
            { this.propertyInfo = propertyInfo; }

            public string ParameterName
            { get; set; }

            public int ParameterOrder
            { get; set; }

            public object DefaultValue
            { get; set; }

            public bool EmitDefaultValue
            { get; set; }

            public bool IsCollection
            { get; set; }

            public bool IsRequired
            { get; set; }

            public Func<object, string> Format
            { get; set; }

            public object GetValue(object o)
            {
                return this.propertyInfo.GetValue(o);
            }

            public void SetValue(object o, object value)
            {
                this.propertyInfo.SetValue(o, value);
            }

            PropertyInfo propertyInfo;
        }

        #endregion
    }
}
