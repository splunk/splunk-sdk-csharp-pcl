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

//// TODO:
//// [X] Eliminate unused extensions and justify the remainder
//// [O] Contract checks on all public methods
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    internal static class XmlReaderExtensions
    {
        /// <summary>
        /// Throws an exception if the source <see cref="XmlReader"/> is not
        /// positioned as expected.
        /// <paramref name="names"/>.
        /// </summary>
        /// <param name="reader">
        /// The source <see cref="XmlReader"/>. 
        /// </param>
        /// <param name="nodeType">
        /// Expected XML node type.
        /// </param>
        /// <param name="names">
        /// Optional list of names which the expected <paramref name="nodeType"/>
        /// may have.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// If <paramref name="reader"/>.NodeType is different than <paramref 
        /// name="nodeType"/> or—if <paramref name="names"/> are provided—
        /// paramref name="reader"/>.Name is not in the list of <paramref name=
        /// "names"/>.
        /// </exception>
        public static void EnsureMarkup(this XmlReader reader, XmlNodeType nodeType, params string[] names)
        {
            Contract.Requires<ArgumentNullException>(reader != null);

            if (reader.NodeType == nodeType)
            {
                if (names == null || names.Length == 0)
                {
                    return;
                }

                if (names.FirstOrDefault(name => reader.Name == name) != null)
                {
                    return;
                }
            }

            var expected = Conjoin("or", names.Select(name => FormatNode(nodeType, name)).ToArray());
            string message;

            switch (reader.ReadState)
            {
                case ReadState.EndOfFile:
                    
                    message = string.Format("Reached end of file where {0} was expected.", expected);
                    break;
                
                case ReadState.Interactive:
                
                    var actual = FormatNode(reader.NodeType, reader.Name);
                    message = string.Format("Read {1} where {0} was expected.", expected, actual);
                    break;
                
                default: throw new InvalidOperationException(); // TODO: Diagnostics
            }

            reader.ThrowInvalidDataException(message);
        }

        /// <summary>
        /// Gets the value of the attribute with the specified name.
        /// </summary>
        /// <param name="reader">
        /// The source <see cref="XmlReader"/>.
        /// </param>
        /// <param name="name">
        /// An attribute name.
        /// </param>
        /// <returns>
        /// The value of attribute <paramref name="name"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> or <paramref name="name"/> are <c>
        /// null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="reader"/> is not positioned on an <see cref=
        /// "XmlNodeType"/>.Element.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// If the value of attribute <paramref name="name"/> is <see cref=
        /// "String"/>.Empty or <c>null</c>.
        /// </exception>
        /// <remarks>
        /// If attribute <paramref name="name"/> is not present or is equal to
        /// <see cref="string"/>.Empty an <see cref="InvalidDataException"/>
        /// is thrown.
        /// </remarks>
        public static string GetRequiredAttribute(this XmlReader reader, string name)
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentException>(reader.NodeType == XmlNodeType.Element);

            string value = reader[name];

            if (string.IsNullOrEmpty(value))
            {
                var message = string.Format("Value of <{0}> attribute {1} is missing.", reader.Name, name);
                reader.ThrowInvalidDataException(message);
            }

            return value;
        }

        /// <summary>
        /// Asynchronously positions the source <see cref="XmlReader"/> to the
        /// start-tag following an <see cref="XmlNodeType"/>.XmlDeclaration.
        /// </summary>
        /// <param name="reader">
        /// The source <see cref="XmlReader"/>.
        /// </param>
        /// <param name="names">
        /// Optional list of permitted document element names.
        /// </param>
        /// <returns>
        /// <c>true</c> if the source <see cref="XmlReader"/> is successfully
        /// positioned at one of the permitted document element <paramref name=
        /// "names"/>; otherwise, if the end of file is reached, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// There is no start-tag following the <see cref="XmlNodeType"/>.XmlDeclaration or
        /// the start-tag is not in the list of permitted document element names.
        /// </exception>
        public static async Task<bool> MoveToDocumentElementAsync(this XmlReader reader, params string[] names)
        {
            Contract.Requires<ArgumentNullException>(reader != null);

            if (reader.ReadState == ReadState.Initial)
            {
                await reader.ReadAsync();

                if (reader.NodeType == XmlNodeType.XmlDeclaration)
                {
                    try
                    {
                        await reader.ReadAsync();
                    }
                    catch (XmlException)
                    {
                        //// WORKAROUND:
                        //// Issue: Some searches return no results and in 
                        //// these cases Splunk writes nothing but an XML 
                        //// Declaration. When nothing follows the declaration 
                        //// the XmlReader fails to detect EOF, does not update
                        //// the current XmlNode, and then throws an XmlException
                        //// because it thinks the XmlNode appears on a line
                        //// other than the first.
                        ////
                        //// We catch the issue here, dispose the reader to 
                        //// ensure that EOF is true, and then return.
                        ////
                        //// Verified against Microsoft .NET 4.5 and Splunk
                        //// 5.0.4, 6.0.3, and 6.1.1.

                        if (reader.NodeType == XmlNodeType.XmlDeclaration)
                        {
                            reader.Dispose();
                            return false;
                        }

                        throw;
                    }
                }
            }
            else
            {
                reader.MoveToElement(); // ensures we're on an element, not an attribute
            }

            if (reader.ReadState != ReadState.Interactive)
            {
                return false;
            }

            reader.EnsureMarkup(XmlNodeType.Element, names);
            return true;
        }

        /// <summary>
        /// Asynchronously visits the named descendants of the element at the 
        /// current <see cref="XmlReader"/> position.
        /// </summary>
        /// <param name="reader">
        /// The source <see cref="XmlReader"/>.
        /// </param>
        /// <param name="name">
        /// Name of the descendant elements to read.
        /// </param>
        /// <param name="task">
        /// An awaitable function to apply to <see cref="reader"/> at each of
        /// the visited elements.
        /// </param>
        /// <returns>
        /// <c>true</c> if at least one matching descendant element is found; 
        /// otherwise <c>false</c>. The <paramref name="reader"/> is positioned 
        /// on the end-tag of the element. If the <paramref name="reader"/> is 
        /// not positioned on an element, this method returns false and the 
        /// position of the <paramref name="reader"/> is not changed.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> or <paramref name="task"/> are <c>null</c>.
        /// </exception>
        public static async Task<bool> ReadEachDescendantAsync(this XmlReader reader, string name, 
            Func<XmlReader, Task> task)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name), "name");
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            Contract.Requires<ArgumentNullException>(task != null, "action");

            if (!await reader.ReadToDescendantAsync(name))
            {
                return false;
            }

            await task(reader);

            while (await reader.ReadToNextSiblingAsync(name))
            {
                await task(reader);
            }

            return true;
        }

        /// <summary>
        /// Asynchronously reads element content using the specified <see cref=
        /// "ValueConverter&lt;Tvalue&gt;"/>.
        /// </summary>
        /// <typeparam name="TValue">
        /// The type of value to be returned.
        /// </typeparam>
        /// <param name="reader">
        /// The source <see cref="XmlReader"/>.
        /// </param>
        /// <param name="valueConverter">
        /// The converter to convert the element content.
        /// </param>
        /// <returns>
        /// The element content converted to the requested type.
        /// </returns>
        public static async Task<TValue> ReadElementContentAsync<TValue>(this XmlReader reader, ValueConverter<TValue> valueConverter)
        {
            return valueConverter.Convert(await reader.ReadElementContentAsStringAsync());
        }

        /// <summary>
        /// Reads a sequence of element start tags.
        /// </summary>
        /// <param name="reader">
        /// The source <see cref="XmlReader"/>. 
        /// </param>
        /// <param name="names">
        /// The sequence of element start tag names to match.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing this operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> or <paramref name="names"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// If the sequence of nodes read from <paramref name="reader"/> does 
        /// not match the sequence of start tag <paramref name="names"/>.
        /// </exception>
        /// <remarks>
        /// The <paramref name="reader"/> should be positioned on the node
        /// preceding the first start tag in the sequence of <paramref name=
        /// "names"/> when this method is called. The 
        /// <paramref name="reader"/> will be positioned on the last start
        /// tag in the sequence of <paramref name="names"/> when this method 
        /// returns.
        /// </remarks>
        public static async Task ReadElementSequenceAsync(this XmlReader reader, params string[] names)
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            Contract.Requires<ArgumentNullException>(names != null);

            foreach (var name in names)
            {
                await reader.ReadAsync();
                reader.EnsureMarkup(XmlNodeType.Element, name);
            }
        }

        /// <summary>
        /// Reads a sequence of element end tags.
        /// </summary>
        /// <param name="reader">
        /// The source <see cref="XmlReader"/>. 
        /// </param>
        /// <param name="names">
        /// The sequence of element end tag names to match.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing this operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> or <paramref name="names"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// If the sequence of nodes read from <paramref name="reader"/> does 
        /// not match the sequence of end tag <paramref name="names"/>.
        /// </exception>
        /// <remarks>
        /// The <paramref name="reader"/> should be positioned on the first 
        /// end tag in the sequence of <paramref name="names"/> when this 
        /// method is called. The <paramref name="reader"/> will be positioned
        /// on the node following the last end tag in the sequence of <paramref 
        /// name="names "/> when this method returns.
        /// </remarks>
        public static async Task ReadEndElementSequenceAsync(this XmlReader reader, params string[] names)
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            Contract.Requires<ArgumentNullException>(names != null);

            foreach (var name in names)
            {
                reader.EnsureMarkup(XmlNodeType.EndElement, name);
                await reader.ReadAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader">
        /// The source <see cref="XmlReader"/>.
        /// </param>
        /// <param name="name">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static async Task<string> ReadResponseElementAsync(this XmlReader reader, string name)
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));

            reader.Requires(await reader.MoveToDocumentElementAsync("response"));
            await reader.ReadElementSequenceAsync(name);
            var text = await reader.ReadElementContentAsStringAsync();

            return text;
        }

        /// <summary>
        /// Asynchronously advances the source <see cref="XmlReader"/> to the 
        /// next descendant element with the specified qualified name.
        /// </summary>
        /// <param name="reader">
        /// The source <see cref="XmlReader"/>.
        /// </param>
        /// <param name="name">
        /// The qualified name of the element you wish to move to.
        /// </param>
        /// <returns>
        /// <c>true</c> if a matching descendant element is found; otherwise 
        /// <c>false</c>. If a matching child element is not found, the 
        /// <paramref name="reader"/> is positioned on the end-tag of the 
        /// element. If the <paramref name="reader"/> is not positioned on an 
        /// element when <see cref="ReadToDescendantAsync"/> was called, this
        /// method returns false and the position of the <paramref name=
        /// "reader"/> is not changed.
        /// </returns>
        public static async Task<bool> ReadToDescendantAsync(this XmlReader reader, string name)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name), "name");
            Contract.Requires<ArgumentNullException>(reader != null, "reader");

            int depth = reader.Depth;

            if (reader.NodeType != XmlNodeType.Element)
            {
                if (reader.ReadState != ReadState.Initial)
                {
                    return false;
                }
                depth--;
            }
            else if (reader.IsEmptyElement)
            {
                return false;
            }
            
            name = reader.NameTable.Add(name);

            while (await reader.ReadAsync() && (reader.Depth > depth))
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader">
        /// The source <see cref="XmlReader"/>.
        /// </param>
        /// <param name="name">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static async Task<bool> ReadToFollowingAsync(this XmlReader reader, string name)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name), "name");
            Contract.Requires<ArgumentNullException>(reader != null, "reader");

            name = reader.NameTable.Add(name);

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == name)
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Asynchronously advances the the source <see cref="XmlReader"/> to 
        /// the next sibling element with the specified qualified name.
        /// </summary>
        /// <param name="reader">
        /// The source <see cref="XmlReader"/>. 
        /// </param>
        /// <param name="name">
        /// A qualified element name.
        /// </param>
        /// <returns>
        /// <c>true</c> if a matching sibling element is found; otherwise <c>
        /// false</c>. If a matching sibling element is not found, <paramref 
        /// name="reader"/> is positioned on the end tag of the parent parent
        /// element.
        /// </returns>
        public static async Task<bool> ReadToNextSiblingAsync(this XmlReader reader, string name)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name), "name");
            Contract.Requires<ArgumentNullException>(reader != null, "reader");

            name = reader.NameTable.Add(name);
            XmlNodeType nodeType;

            do
            {
                if (! await reader.SkipSubtreeAsync())
                {
                    break;
                }

                nodeType = reader.NodeType;

                if ((nodeType == XmlNodeType.Element) && reader.Name == name)
                {
                    return true;
                }
            }
            while (nodeType != XmlNodeType.EndElement && !reader.EOF);
            
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader">
        /// The source <see cref="XmlReader"/>.
        /// </param>
        /// <param name="condition">
        /// 
        /// </param>
        public static void Requires(this XmlReader reader, bool condition)
        {
            if (condition)
            {
                return;
            }

            string message;

            switch (reader.ReadState)
            {
                case ReadState.EndOfFile:
                    message = string.Format("Premature end of file");
                    break;
                case ReadState.Interactive:
                    message = string.Format("Unexpected {0}", FormatNode(reader.NodeType, reader.Name));
                    break;
                default: throw new InvalidOperationException(); // TODO: Diagnostics
            }

            reader.ThrowInvalidDataException(message);
        }

        #region Privates/internals

        static string Conjoin(string conjunction, params string[] names)
        {
            if (names.Length <= 0)
            {
                return "";
            }

            if (names.Length == 1)
            {
                return names[0];
            }

            if (names.Length == 2)
            {
                return string.Join(" ", names[0], conjunction, names[1]);
            }

            int n = names.Length - 1;
            return string.Join(", ", names.Take(n).Concat(new string[] { conjunction, names[n] }));
        }

        static string FormatNode(XmlNodeType nodeType, string name)
        {
            switch (nodeType)
            {
                case XmlNodeType.CDATA:

                    return "CDATA section";

                case XmlNodeType.Text:

                    return "charater data";

                case XmlNodeType.Element:

                    return string.IsNullOrEmpty(name) ? "start-tag" : string.Concat("<", name, ">");
                
                case XmlNodeType.EndElement:
                    
                    return string.IsNullOrEmpty(name) ? "end-tag" : string.Concat("</", name, ">");
                
                default: throw new ArgumentException(string.Format("Unsupported XmlNodeType: {0}", nodeType));
            }
        }

        static async Task<bool> SkipSubtreeAsync(this XmlReader reader)
        {
            reader.MoveToElement(); // the element we're moving to is guaranteed to be accessible so no need for async

            if ((reader.NodeType == XmlNodeType.Element) && !reader.IsEmptyElement)
            {
                int depth = reader.Depth;

                while (await reader.ReadAsync() && (depth < reader.Depth))
                { }

                if (reader.NodeType != XmlNodeType.EndElement)
                {
                    return false;
                }
            }

            return await reader.ReadAsync();
        }

        static void ThrowInvalidDataException(this XmlReader reader, string message)
        {
            var info = (IXmlLineInfo)reader;

            if (info.HasLineInfo())
            {
                string.Format("Line {0}, position {1}: {2}", info.LineNumber, info.LinePosition, message);
            }

            throw new InvalidDataException(message);
        }

        #endregion
    }
}
