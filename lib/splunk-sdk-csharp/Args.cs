namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
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
        // * Respect DataMemberAttribute.Order
        // * Ensure correct behavior with nullable boolean/numeric values
        // * Do not serialize default values => define default values and check for them
        // * Rework this into a real serializer, not just a ToString implementation tool (?)
        // * Work on nomenclature (serialization nomenclature is not necessarily appropriate)
        // * More, I'm sure...

        #region Constructors

        static Args()
        {
            var serializers = new Dictionary<Type, Func<object, SerializationInfo, string>>()
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

            var serializationInfos = new List<SerializationInfo>();

            foreach (PropertyInfo property in typeof(TArgs).GetRuntimeProperties())
            {
                var dataMember = property.GetCustomAttribute<DataMemberAttribute>();
                Func<object, SerializationInfo, string> serializer;

                if (!serializers.TryGetValue(property.PropertyType, out serializer))
                {
                    TypeInfo typeInfo = property.PropertyType.GetTypeInfo();

                    if (typeInfo.IsEnum)
                    {
                        var map = new Dictionary<int, string>();

                        foreach (var value in Enum.GetValues(property.PropertyType))
                        {
                            string name = Enum.GetName(property.PropertyType, value);
                            FieldInfo field = property.PropertyType.GetRuntimeField(name);
                            var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();

                            map[(int)value] = Uri.EscapeUriString(enumMember == null ? name : enumMember.Value);
                        }

                        serializer = (object value, SerializationInfo info) =>
                        {
                            return string.Concat(info.DataMember.Name, "=", map[(int)value]);
                        };

                    }
                    else
                    {
                        serializer = FormatString;
                    }

                    serializers[property.PropertyType] = serializer;
                }

                serializationInfos.Add(new SerializationInfo(dataMember, property, serializer));
            }

            SerializationInfos = serializationInfos;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var info in Args<TArgs>.SerializationInfos)
            {
                object value = info.Property.GetValue(this);

                if (value != null)
                {
                    builder.Append(info.Serialize(value, info));
                    builder.Append('&');
                }
            }

            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 1;
            }
            
            return builder.ToString();
        }

        #endregion

        #region Privates

        static readonly IReadOnlyList<SerializationInfo> SerializationInfos;

        static string FormatBoolean(object value, SerializationInfo info)
        {
            return string.Concat(info.DataMember.Name, "=", (bool)value ? "t" : "f");
        }

        static string FormatNumber(object value, SerializationInfo info)
        {
            return string.Concat(info.DataMember.Name, "=", value.ToString());
        }

        static string FormatString(object value, SerializationInfo info)
        {
            return string.Concat(info.DataMember.Name, "=", Uri.EscapeUriString(value.ToString()));
        }

        static string FormatStringList(object value, SerializationInfo info)
        {
            string name = info.DataMember.Name;
            
            return string.Join("&", 
                from item in (IReadOnlyList<string>)value select string.Concat(name, "=", item));
        }

        class SerializationInfo
        {
            public SerializationInfo(DataMemberAttribute dataMember, PropertyInfo property, Func<object, SerializationInfo, string> serializer)
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

            public Func<object, SerializationInfo, string> Serialize
            { get; private set; }
        }

        #endregion
    }
}
