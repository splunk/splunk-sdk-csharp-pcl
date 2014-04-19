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

// [O] TODO: Contract checks on all public methods
// [ ] TODO: Documentation

namespace Splunk.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;

    //// TODO: Eliminate unused extensions and justify the remainder

    internal static class XmlReaderExtensions
    {
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

        public static async Task<TValue> ReadElementContentAsync<TValue>(this XmlReader reader, ValueConverter<TValue> valueConverter)
        {
            return valueConverter.Convert(await reader.ReadElementContentAsStringAsync());
        }

        public static async Task<string> ReadResponseElementAsync(this XmlReader reader, string name)
        {
            await reader.ReadAsync();

            if (reader.NodeType == XmlNodeType.XmlDeclaration)
            {
                await reader.ReadAsync();
            }

            if (!(reader.NodeType == XmlNodeType.Element && reader.Name == "response"))
            {
                throw new InvalidDataException(); // TODO: Diagnostics
            }

            await reader.ReadAsync();

            if (!(reader.NodeType == XmlNodeType.Element && reader.Name == name))
            {
                throw new InvalidDataException(); // TODO: Diagnostics
            }

            var text = await reader.ReadElementContentAsStringAsync();
            return text;
        }

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
    }
}
