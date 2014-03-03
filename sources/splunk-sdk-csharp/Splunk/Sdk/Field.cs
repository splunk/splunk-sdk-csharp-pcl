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

    /// <summary>
    /// A field can be accessed as either an <see cref="Array"/>
    /// or as a delimited string (using an implicit string converter or 
    /// <see cref="GetArray()"/>). Splunk recommends accessing values as an 
    /// array when possible.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The delimiter for field values depends on the underlying 
    /// result format. If the underlying format does not specify 
    /// a delimiter, such as with the <see cref="ResultsReaderXml"/> class, 
    /// the delimiter is <see cref="DefaultDelimiter"/>, 
    /// which is a comma (,).
    /// </para>
    /// </remarks>
    // Note that the type conversion methods are not from IConvertible. 
    // That is primarily because IConvertible methods require 
    // IFormatProvider parameter which make it 
    // not as easy to use. System.Convert methods do not require 
    // IFormatProvider parameter.
    public class Field
    {
        /// <summary>
        /// A single value or delimited set of values. 
        /// Null if <see cref="arrayValues"/> is used.
        /// </summary>
        private string singleOrDelimited;

        /// <summary>
        /// An array of values. Null if <see cref="singleOrDelimited"/> 
        /// is used.
        /// </summary>
        private string[] arrayValues;

        /// <summary>
        /// The default delimiter, which is a comma (,).
        /// </summary>
        public const string DefaultDelimiter = ",";

        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> 
        /// class.
        /// </summary>
        /// <param name="singleOrDelimited">
        /// The single value or delimited set of values.
        /// </param>
        public Field(string singleOrDelimited)
        {
            this.singleOrDelimited = singleOrDelimited;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/>
        /// class.
        /// </summary>
        /// <param name="arrayValues">Array of values.</param>
        public Field(string[] arrayValues)
        {
            this.arrayValues = arrayValues;
        }

        /// <summary>
        /// Gets the values for the field.
        /// </summary>
        /// <remarks>
        /// <b>Caution:</b> This variant of the <b>GetArray</b> method 
        /// is unsafe for <see cref="ResultsReader"/> implementations that 
        /// require a delimiter. Therefore, this method should only be 
        /// used for results that are returned by 
        /// <see cref="ResultsReaderXml"/>. For other readers, use the 
        /// <see cref="GetArray(String)"/> method instead.
        /// If the underlying <see cref="ResultsReader"/> object has 
        /// no delimiter, the original array of values is returned. 
        /// If the object does have a delimiter, the single/delimited value
        /// is split based on the 
        /// <see cref="DefaultDelimiter"/> and is returned as an array.
        /// </remarks>
        /// <returns>
        /// The original array of values if there is no delimiter, or the 
        /// array of values split by the delimiter.
        /// </returns>
        public string[] GetArray()
        {
            return this.GetArray(DefaultDelimiter);
        }

        /// <summary>
        /// Gets the values for the field.
        /// </summary>
        /// <remarks>
        /// The delimiter must be determined empirically based on the search
        /// string and the data format of the index. The delimiter can 
        /// differ between fields in the same <see cref="Record"/>.
        /// <para>
        /// If the underlying <see cref="ResultsReader"/> object has 
        /// no delimiter, the original array of values is returned. 
        /// If the object does have a delimiter, the single/delimited value
        /// is split based on the specified delimiter 
        /// and is returned as an array.
        /// </para>
        /// </remarks>
        /// <param name="delimiter">The delimiter, which is be passed to 
        /// <see cref="System.String.Split(string[], StringSplitOptions)"/> 
        /// to perform the split.</param>
        /// <returns>
        /// The original array of values if there is no delimiter,
        /// or the array of values split by the delimiter.
        /// </returns>
        public string[] GetArray(string delimiter)
        {
            return this.arrayValues ?? this.singleOrDelimited.Split(
                new string[] { delimiter },
                StringSplitOptions.None);
        }

        /// <summary>
        /// Returns the single value or delimited set of values for the
        /// field.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When getting a multi-valued field, use the
        /// <see cref="GetArray()"/> methods instead.
        /// </para>
        /// </remarks>
        /// <returns>The single value or set of values delimited by 
        /// <see cref="DefaultDelimiter"/>.
        /// </returns>
        public override string ToString()
        {
            return this.singleOrDelimited ?? string.Join(
                DefaultDelimiter,
                this.arrayValues);
        }

        /// <summary>
        /// Converts a <see cref="Field"/> to a <c>string</c>.
        /// Same as <see cref="ToString"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// When getting a multi-valued field, use the
        /// <see cref="GetArray()"/> methods instead.
        /// </para>
        /// </remarks>
        /// <param name="value">Field value</param>
        /// <returns>The single value or set of values delimited by the 
        /// <see cref="DefaultDelimiter"/>.
        /// </returns>
        public static implicit operator string(Field value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts an <see cref="Field"/> to a <c>double</c>.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The converted value.</returns>
        public static explicit operator double(Field value)
        {
            return Convert.ToDouble(value.ToString());
        }

        /// <summary>
        /// Converts an <see cref="Field"/> to a <c>float</c>.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The converted value.</returns>
        public static explicit operator float(Field value)
        {
            return Convert.ToSingle(value.ToString());
        }

        /// <summary>
        /// Converts an <see cref="Field"/> to a <c>byte</c>.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The converted value.</returns>
        public static explicit operator byte(Field value)
        {
            return Convert.ToByte(value.ToString());
        }

        /// <summary>
        /// Converts an <see cref="Field"/> to a <c>ushort</c>.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The converted value.</returns>
        public static explicit operator ushort(Field value)
        {
            return Convert.ToUInt16(value.ToString());
        }

        /// <summary>
        /// Converts an <see cref="Field"/> to a <c>uint</c>.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The converted value.</returns>
        public static explicit operator uint(Field value)
        {
            return Convert.ToUInt32(value.ToString());
        }

        /// <summary>
        /// Converts an <see cref="Field"/> to a <c>ulong</c>.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The converted value.</returns>
        public static explicit operator ulong(Field value)
        {
            return Convert.ToUInt64(value.ToString());
        }

        /// <summary>
        /// Converts an <see cref="Field"/> to a <c>sbyte</c>.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The converted value.</returns>
        public static explicit operator sbyte(Field value)
        {
            return Convert.ToSByte(value.ToString());
        }

        /// <summary>
        /// Converts an <see cref="Field"/> to a <c>short</c>/.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The converted value.</returns>
        public static explicit operator short(Field value)
        {
            return Convert.ToInt16(value.ToString());
        }

        /// <summary>
        /// Converts an <see cref="Field"/> to a <c>int</c>.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The converted value.</returns>
        public static explicit operator int(Field value)
        {
            return Convert.ToInt32(value.ToString());
        }

        /// <summary>
        /// Converts an <see cref="Field"/> to a <c>ulong</c>.
        /// </summary>
        /// <param name="value">The field value.</param>
        /// <returns>The converted value.</returns>
        public static explicit operator long(Field value)
        {
            return Convert.ToInt64(value.ToString());
        }

        /// <summary>
        /// Converts to a <c>byte</c>.
        /// </summary>
        /// <returns>The converted value.</returns>
        public byte ToByte()
        {
            return Convert.ToByte(this.ToString());
        }

        /// <summary>
        /// Converts to a <c>ushort</c>.
        /// </summary>
        /// <returns>The converted value.</returns>
        public ushort ToUInt16()
        {
            return Convert.ToUInt16(this.ToString());
        }

        /// <summary>
        /// Converts to a <c>uint</c>.
        /// </summary>
        /// <returns>The converted value.</returns>
        public uint ToUInt32()
        {
            return Convert.ToUInt32(this.ToString());
        }

        /// <summary>
        /// Converts to a <c>ulong</c>.
        /// </summary>
        /// <returns>The converted value</returns>
        public ulong ToUInt64()
        {
            return Convert.ToUInt64(this.ToString());
        }

        /// <summary>
        /// Converts to a <c>sbyte</c>.
        /// </summary>
        /// <returns>The converted value.</returns>
        public sbyte ToSByte()
        {
            return Convert.ToSByte(this.ToString());
        }

        /// <summary>
        /// Converts to a <c>short</c>/.
        /// </summary>
        /// <returns>The converted value.</returns>
        public short ToInt16()
        {
            return Convert.ToInt16(this.ToString());
        }

        /// <summary>
        /// Converts to a <c>int</c>.
        /// </summary>
        /// <returns>The converted value</returns>
        public int ToInt32()
        {
            return Convert.ToInt32(this.ToString());
        }

        /// <summary>
        /// Converts to a <c>ulong</c>.
        /// </summary>
        /// <returns>The converted value.</returns>
        public long ToInt64()
        {
            return Convert.ToInt64(this.ToString());
        }
    }
}
