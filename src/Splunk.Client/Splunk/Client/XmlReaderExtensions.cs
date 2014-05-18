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
//// [ ] Documentation

namespace Splunk.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;

    internal static class XmlReaderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader">
        /// 
        /// </param>
        /// <param name="nodeType">
        /// 
        /// </param>
        /// <param name="name">
        /// 
        /// </param>
        public static void EnsureMarkup(this XmlReader reader, XmlNodeType nodeType, string name)
        {
            Contract.Requires<ArgumentNullException>(reader != null);

            if (reader.NodeType == nodeType && (name == null || name == reader.Name))
            {
                return;
            }

            switch (reader.ReadState)
            {
                case ReadState.EndOfFile:
                    throw new InvalidDataException(string.Format(
                        "Reached end of file where {0} was expected.", FormatNode(nodeType, name)));
                case ReadState.Interactive:
                    throw new InvalidDataException(string.Format(
                        "Read {0} where {1} was expected.", FormatNode(reader.NodeType, reader.Name), FormatNode(nodeType, name)));
                default:
                    throw new InvalidOperationException(); // TODO: Diagnostics
            }
        }

        public static string GetRequiredAttributeValue(this XmlReader reader, string attributeName)
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            Contract.Requires<ArgumentNullException>(attributeName != null);
            Contract.Requires<ArgumentException>(reader.NodeType == XmlNodeType.Element);

            string value = reader[attributeName];

            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidDataException(string.Format("Value of <{0}> attribute {1} is missing.", reader.Name, attributeName));
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader">
        /// 
        /// </param>
        /// <param name="name">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static async Task<bool> MoveToDocumentElementAsync(this XmlReader reader, string name = null)
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

            reader.EnsureMarkup(XmlNodeType.Element, name);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task ReadEachDescendantAsync(this XmlReader reader, string name, Func<Task> task)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name), "name");
            Contract.Requires<ArgumentNullException>(task != null, "action");
            Contract.Requires<ArgumentNullException>(reader != null, "reader");

            if (await reader.ReadToDescendantAsync(name))
            {
                await task();

                while (await reader.ReadToNextSiblingAsync(name))
                {
                    await task();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="reader"></param>
        /// <param name="valueConverter"></param>
        /// <returns></returns>
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

                if (!(reader.NodeType == XmlNodeType.Element && reader.Name == name))
                {
                    throw new InvalidDataException(); // TODO: Diagnostics : unexpected start tag
                }
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
                if (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == name))
                {
                    throw new InvalidDataException(); // TODO: Diagnostics : unexpected end tag
                }

                await reader.ReadAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader">
        /// 
        /// </param>
        /// <param name="name">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public static async Task<string> ReadResponseElementAsync(this XmlReader reader, string name)
        {
            if (!await reader.MoveToDocumentElementAsync("response"))
            {
                throw new InvalidDataException(); // TODO: Diagnostics : premature end of file
            }

            await reader.ReadElementSequenceAsync(name);

            var text = await reader.ReadElementContentAsStringAsync();
            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader">
        /// 
        /// </param>
        /// <param name="name">
        /// 
        /// </param>
        /// <returns></returns>
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
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
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

        #region Privates/internals

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

        #endregion
    }
}
