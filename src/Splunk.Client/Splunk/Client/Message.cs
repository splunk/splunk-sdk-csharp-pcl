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
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Provides a class that represents a Splunk service response message.
    /// </summary>
    public sealed class Message : IComparable, IComparable<Message>, IEquatable<Message>
    {
        #region Constructors
        
        internal Message(MessageType type, string text)
        {
            Contract.Requires<ArgumentOutOfRangeException>(MessageType.Debug <= type && type <= MessageType.Fatal, "type");
            Contract.Requires<ArgumentNullException>(text != null, "text");
            this.Type = type;
            this.Text = text;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the text of the current <see cref="Message"/>.
        /// </summary>
        public string Text
        { get; private set; }

        /// <summary>
        /// Gets the type of the current <see cref="Message"/>.
        /// </summary>
        public MessageType Type
        { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Compares the current <see cref="Message"/> with another object
        /// and returns an integer that indicates whether the current <see 
        /// cref="Message"/> precedes, follows, or appears in the same 
        /// position in the sort order as the other object.
        /// </summary>
        /// <param name="other">
        /// The object to compare to the current <see cref="Message"/>.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance 
        /// precedes, follows, or appears in the same position in the sort 
        /// order as <paramref name="other"/>.
        /// <list type="table">
        /// <listheader>
        ///   <term>
        ///     Value
        ///   </term>
        ///   <description>
        ///     Condition
        ///   </description>
        /// </listheader>
        /// <item>
        ///   <term>
        ///     Less than zero
        ///   </term>
        ///   <description>
        ///     This instance precedes <paramref name="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Zero
        ///   </term>
        ///   <description>
        ///     This instance is in the same position in the sort order as
        ///     paramref name="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Greater than zero
        ///   </term>
        ///   <description>
        ///     This instance follows <paramref name="other"/>, <paramref name=
        ///     "other"/> is not a <see cref="Message"/>, or <paramref name=
        ///     "other"/> is <c>null</c>.
        ///   </description>
        /// </item>
        /// </list>
        /// </returns>
        public int CompareTo(object other)
        {
            return this.CompareTo(other as Message);
        }

        /// <summary>
        /// Compares the current <see cref="Message"/> with another one and
        /// returns an integer that indicates whether the current <see cref=
        /// "Message"/> precedes, follows, or appears in the same position in
        /// the sort order as the other one.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Message"/>.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that indicates whether this instance 
        /// precedes, follows, or appears in the same position in the sort 
        /// order as <paramref name="other"/>.
        /// <list type="table">
        /// <listheader>
        ///   <term>
        ///     Value
        ///   </term>
        ///   <description>
        ///     Condition
        ///   </description>
        /// </listheader>
        /// <item>
        ///   <term>
        ///     Less than zero
        ///   </term>
        ///   <description>
        ///     This instance precedes <paramref name="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Zero
        ///   </term>
        ///   <description>
        ///     This instance is in the same position in the sort order as 
        ///      <paramref name="other"/>.
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>
        ///     Greater than zero
        ///   </term>
        ///   <description>
        ///     This instance follows <paramref name="other"/> or <paramref 
        ///     name="other"/> is <c>null</c>.
        ///   </description>
        /// </item>
        /// </list>
        /// </returns>
        public int CompareTo(Message other)
        {
            if (other == null)
            {
                return 1;
            }

            if (object.ReferenceEquals(this, other))
            {
                return 0;
            }
            
            int difference = this.Type - other.Type;
            return difference != 0 ? difference : this.Text.CompareTo(other.Text);
        }

        /// <summary>
        /// Determines whether the current <see cref="Message"/> and another
        /// object are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Message"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is a non <c>null</c> <see 
        /// cref="Message"/> and is the same as the current <see cref="Message"/>;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            return this.Equals(other as Message);
        }

        /// <summary>
        /// Determines whether the current <see cref="Message"/> and another
        /// one are equal.
        /// </summary>
        /// <param name="other">
        /// The object to compare with the current <see cref="Message"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is non <c>null</c> and is 
        /// the same as the current <see cref="Message"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Message other)
        {
            if (other == null)
            {
                return false;
            }
            return object.ReferenceEquals(this, other) || (this.Type == other.Type && this.Text == other.Text);
        }

        /// <summary>
        /// Computes the hash code for the current <see cref="Message"/>.
        /// </summary>
        /// <returns>
        /// The hash code for the current <see cref="Message"/>.
        /// </returns>
        public override int GetHashCode()
        {
            // TODO: Check this against the algorithm presented in Effective Java
            int hash = 17;

            hash = (hash * 23) + this.Type.GetHashCode();
            hash = (hash * 23) + this.Text.GetHashCode();
            
            return hash;
        }

        /// <summary>
        /// Gets a string representation for the current <see cref="Message"/>.
        /// </summary>
        /// <returns>
        /// A string representation of the current <see cref="Message"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Concat(this.Type.ToString(), ": ", this.Text);
        }

        #endregion

        #region Privates/internals

        internal static async Task<IReadOnlyList<Message>> ReadMessagesAsync(XmlReader reader)
        {
            var messages = new List<Message>();

            if (await reader.MoveToDocumentElementAsync("response"))
            {
                await reader.ReadAsync();
                reader.EnsureMarkup(XmlNodeType.Element, "messages");
                await reader.ReadAsync();

                while (reader.NodeType == XmlNodeType.Element && reader.Name == "msg")
                {
                    var name = reader.GetRequiredAttribute("type");
                    var type = EnumConverter<MessageType>.Instance.Convert(name);
                    var text = await reader.ReadElementContentAsStringAsync();
                    
                    messages.Add(new Message(type, text));
                }

                reader.EnsureMarkup(XmlNodeType.EndElement, "messages");
            }

            return messages;
        }

        #endregion
    }
}
