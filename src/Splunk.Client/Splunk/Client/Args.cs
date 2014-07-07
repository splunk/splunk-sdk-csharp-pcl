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

//// TODO: Ensure this code is solid
//// [ ] Support more than one level of inheritance => move away from generic implementation.
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Provides a base class for representing strongly typed arguments to Splunk
    /// endpoints.
    /// </summary>
    /// <typeparam name="TArgs">
    /// Type of the arguments.
    /// </typeparam>
    /// <seealso cref="T:System.Collections.Generic.IEnumerable{Splunk.Client.Argument}"/>
    public abstract class Args<TArgs> : IEnumerable<Argument> where TArgs : Args<TArgs>
    {
        #region Constructors

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification =
            "This is by design.")
        ]
        static Args()
        {
            var propertyFormatters = new Dictionary<Type, Formatter>()
            {
                { typeof(bool),     new Formatter { Format = FormatBoolean } },
                { typeof(bool?),    new Formatter { Format = FormatBoolean } },
                { typeof(byte),     new Formatter { Format = FormatNumber  } },
                { typeof(byte?),    new Formatter { Format = FormatNumber  } },
                { typeof(sbyte),    new Formatter { Format = FormatNumber  } },
                { typeof(sbyte?),   new Formatter { Format = FormatNumber  } },
                { typeof(short),    new Formatter { Format = FormatNumber  } },
                { typeof(short?),   new Formatter { Format = FormatNumber  } },
                { typeof(ushort),   new Formatter { Format = FormatNumber  } },
                { typeof(ushort?),  new Formatter { Format = FormatNumber  } },
                { typeof(int),      new Formatter { Format = FormatNumber  } },
                { typeof(int?),     new Formatter { Format = FormatNumber  } },
                { typeof(uint),     new Formatter { Format = FormatNumber  } },
                { typeof(uint?),    new Formatter { Format = FormatNumber  } },
                { typeof(long),     new Formatter { Format = FormatNumber  } },
                { typeof(long?),    new Formatter { Format = FormatNumber  } },
                { typeof(ulong),    new Formatter { Format = FormatNumber  } },
                { typeof(ulong?),   new Formatter { Format = FormatNumber  } },
                { typeof(float),    new Formatter { Format = FormatNumber  } },
                { typeof(float?),   new Formatter { Format = FormatNumber  } },
                { typeof(double),   new Formatter { Format = FormatNumber  } },
                { typeof(double?),  new Formatter { Format = FormatNumber  } },
                { typeof(decimal),  new Formatter { Format = FormatNumber  } },
                { typeof(decimal?), new Formatter { Format = FormatNumber  } },
                { typeof(string),   new Formatter { Format = FormatString  } }
            };

            var defaultFormatter = new Formatter { Format = FormatString };
            var parameters = new SortedSet<Parameter>();

            foreach (PropertyInfo propertyInfo in typeof(TArgs).GetRuntimeProperties())
            {
                var dataMember = propertyInfo.GetCustomAttribute<DataMemberAttribute>();

                if (dataMember == null)
                {
                    var text = string.Format(CultureInfo.CurrentCulture, "Missing DataMemberAttribute on {0}.{1}",
                        propertyInfo.PropertyType.Name, propertyInfo.Name);
                    throw new InvalidDataContractException(text);
                }

                var propertyName = propertyInfo.Name;
                var propertyType = propertyInfo.PropertyType;
                var propertyTypeInfo = propertyType.GetTypeInfo();

                Formatter? formatter = GetPropertyFormatter(propertyName, null, propertyType, propertyTypeInfo, propertyFormatters);

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
                                //// IEnumerable<T> implements IEnumerable => we are good to go

                                Type itemType = @interface.GenericTypeArguments[0];

                                formatter = GetPropertyFormatter(propertyName, propertyType, itemType, itemType.GetTypeInfo(), propertyFormatters);
                                isCollection = true;

                                break;
                            }
                        }
                        else if (@interface == typeof(IEnumerable))
                        {
                            //// Keep looking because we'd prefer to use a more specific formatter
                            isCollection = true;
                        }
                    }

                    formatter = new Formatter { Format = formatter.Value.Format, IsCollection = isCollection };
                }

                var defaultValue = propertyInfo.GetCustomAttribute<DefaultValueAttribute>();

                parameters.Add(new Parameter(dataMember.Name, dataMember.Order, propertyInfo)
                {
                    // Properties

                    DefaultValue = defaultValue == null ? null : defaultValue.Value,
                    EmitDefaultValue = dataMember.EmitDefaultValue,
                    IsCollection = formatter.Value.IsCollection,
                    IsRequired = dataMember.IsRequired,

                    // Methods

                    Format = (formatter ?? defaultFormatter).Format
                });
            }

            Parameters = parameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Args&lt;TArgs&gt;"/> class.
        /// </summary>
        protected Args()
        {
            foreach (var serializationEntry in Parameters.Where(entry => entry.DefaultValue != null))
            {
                serializationEntry.SetValue(this, serializationEntry.DefaultValue);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets an enumerator that produces an <see cref="Argument"/> sequence
        /// based on the serialization attributes of the properties of the 
        /// current <see cref="Args&lt;TArgs&gt;"/> instance.
        /// </summary>
        /// <exception cref="SerializationException">
        /// Thrown when a Serialization error condition occurs.
        /// </exception>
        /// <returns>
        /// An object for enumerating the <see cref="Argument"/> sequence.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Argument>)this).GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that produces an <see cref="Argument"/> sequence
        /// based on the serialization attributes of the properties of the 
        /// current <see cref="Args&lt;TArgs&gt;"/> instance.
        /// </summary>
        /// <exception cref="SerializationException">
        /// Thrown when a Serialization error condition occurs.
        /// </exception>
        /// <returns>
        /// An object for enumerating the <see cref="Argument"/> sequence.
        /// </returns>
        IEnumerator<Argument> IEnumerable<Argument>.GetEnumerator()
        {
            foreach (var parameter in Args<TArgs>.Parameters)
            {
                object value = parameter.GetValue(this);

                if (value == null)
                {
                    if (parameter.IsRequired)
                    {
                        throw new SerializationException(string.Format("Missing value for required parameter {0}", parameter.Name));
                    }
                    continue;
                }

                if (!parameter.EmitDefaultValue && value.Equals(parameter.DefaultValue))
                {
                    continue;
                }
                
                if (!parameter.IsCollection)
                {
                    yield return new Argument(parameter.Name, parameter.Format(value));
                    continue;
                }
                
                foreach (var item in (IEnumerable)value)
                {
                    yield return new Argument(parameter.Name, parameter.Format(item));
                }
            }
        }

        /// <summary>
        /// Gets a string representation for the current
        /// <see cref="Args&lt;TArgs&gt;"/>.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="Args&lt;TArgs&gt;"/>.
        /// </returns>
        /// <seealso cref="M:System.Object.ToString()"/>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            Action<object, string, Func<object, string>> append = (item, name, format) =>
            {
                builder.Append(name);
                builder.Append('=');
                builder.Append(format(item));
                builder.Append("; ");
            };

            foreach (var parameter in Args<TArgs>.Parameters)
            {
                object value = parameter.GetValue(this);

                if (value == null)
                {
                    append("null", parameter.Name, FormatString);
                    continue;
                }

                if (!parameter.IsCollection)
                {
                    append(value, parameter.Name, parameter.Format);
                    continue;
                }
                
                foreach (var item in (IEnumerable)value)
                {
                    append(item, parameter.Name, parameter.Format);
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

        static readonly SortedSet<Parameter> Parameters;

        static string FormatBoolean(object value)
        {
            return (bool)value ? "1" : "0";
        }

        static string FormatNumber(object value)
        {
            return value.ToString();
        }

        static string FormatString(object value)
        {
            return value.ToString();
        }

        static Formatter? GetPropertyFormatter(string propertyName, Type container, Type type, TypeInfo info, Dictionary<Type, Formatter> formatters)
        {
            Formatter formatter;

            if (formatters.TryGetValue(container ?? type, out formatter))
            {
                return formatter;
            }

            if (container != null && formatters.TryGetValue(type, out formatter))
            {
                formatter.IsCollection = true;
            }
            else
            {
                var enumType = GetEnum(type, info);
                
                if (enumType != null)
                {
                    //// TODO: Add two items to formatters for each enum: 
                    //// formatters.Add(enumType, formatter)
                    //// formatters.Add(info.IsEnum ? typeof(Nullable<>).MakeGenericType(type), formatter)

                    var map = new Dictionary<int, string>();

                    foreach (var value in Enum.GetValues(enumType))
                    {
                        string name = Enum.GetName(enumType, value);
                        FieldInfo field = enumType.GetRuntimeField(name);
                        var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();

                        map[(int)value] = enumMember == null ? name : enumMember.Value;
                    }

                    Func<object, string> format = (object value) =>
                    {
                        string name;

                        if (map.TryGetValue((int)value, out name))
                        {
                            return name;
                        }

                        var text = string.Format(CultureInfo.CurrentCulture, "{0}.{1}: {2}",
                            typeof(TArgs).Name, propertyName, value);

                        throw new ArgumentException(text);
                    };

                    formatter = new Formatter
                    {
                        Format = format,
                        IsCollection = container != null
                    };
                }
                else if (container != null)
                {
                    formatter = new Formatter { Format = FormatString, IsCollection = true };
                }
                else
                {
                    return null;
                }
            }

            formatters.Add(container ?? type, formatter);
            return formatter;
        }

        static Type GetEnum(Type type, TypeInfo info)
        {
            if (info.IsEnum)
            {
                return type;
            }

            type = Nullable.GetUnderlyingType(type);

            if (type == null)
            {
                return null;
            }

            info = type.GetTypeInfo();
            return info.IsEnum ? type : null;
        }

        #endregion

        #region Types

        /// <summary>
        /// A formatter.
        /// </summary>
        /// <seealso cref="T:System.Collections.Generic.IEnumerable{Splunk.Client.Argument}"/>
        struct Formatter
        {
            /// <summary>
            /// Describes the format to use.
            /// </summary>
            public Func<object, string> Format;

            /// <summary>
            /// <c>true</c> if this object is collection.
            /// </summary>
            public bool IsCollection;
        }

        /// <summary>
        /// An ordinal.
        /// </summary>
        /// <seealso cref="T:System.Collections.Generic.IEnumerable{Splunk.Client.Argument}"/>
        struct Ordinal : IComparable<Ordinal>, IEquatable<Ordinal>
        {
            #region Constructos

            public Ordinal(int position, string name)
            {
                this.Position = position;
                this.Name = name;
            }

            #endregion

            #region Fields

            public readonly int Position;

            public readonly string Name;

            #endregion

            #region Methods

            public int CompareTo(Ordinal other)
            {
                int result = this.Position - other.Position;
                return result != 0 ? result : string.Compare(this.Name, other.Name, StringComparison.Ordinal);
            }

            public override bool Equals(object other)
            {
                return other != null && other is Ordinal ? this.Equals((Ordinal)other) : false;
            }

            public bool Equals(Ordinal other)
            {
                return this.Position == other.Position && this.Name == other.Name;
            }

            public override int GetHashCode()
            {
                // TODO: Check this against the algorithm presented in Effective Java
                int hash = 17;

                hash = (hash * 23) + this.Position.GetHashCode();
                hash = (hash * 23) + this.Name.GetHashCode();

                return hash;
            }

            public override string ToString()
            {
                return string.Format(CultureInfo.CurrentCulture, "({0}, {1})", this.Position, this.Name);
            }

            #endregion
        }

        class Parameter : IComparable, IComparable<Parameter>, IEquatable<Parameter>
        {
            #region Constructors

            public Parameter(string name, int position, PropertyInfo propertyInfo)
            {
                this.ordinal = new Ordinal(position, name);
                this.propertyInfo = propertyInfo;
            }

            #endregion

            #region Properties

            public object DefaultValue
            { get; set; }

            public bool EmitDefaultValue
            { get; set; }

            public bool IsCollection
            { get; set; }

            public bool IsRequired
            { get; set; }

            public string Name
            {
                get { return this.ordinal.Name; }
            }

            public int Position
            {
                get { return this.ordinal.Position; }
            }

            #endregion

            #region Methods

            public int CompareTo(object other)
            {
                return this.CompareTo(other as Parameter);
            }

            public int CompareTo(Parameter other)
            {
                if (other == null)
                    return 1;
                if (object.ReferenceEquals(this, other))
                    return 0;
                return this.ordinal.CompareTo(other.ordinal);
            }

            public override bool Equals(object other)
            {
                return this.Equals(other as Parameter);
            }

            public bool Equals(Parameter other)
            {
                if (other == null)
                {
                    return false;
                }
                return object.ReferenceEquals(this, other) || this.ordinal.Equals(other.ordinal);
            }

            public Func<object, string> Format
            { get; set; }

            public override int GetHashCode()
            {
                return this.ordinal.GetHashCode();
            }

            public object GetValue(object o)
            {
                return this.propertyInfo.GetValue(o);
            }

            public void SetValue(object o, object value)
            {
                this.propertyInfo.SetValue(o, value);
            }

            #endregion

            #region Privates

            PropertyInfo propertyInfo;
            Ordinal ordinal;

            #endregion
        }

        #endregion
    }
}
