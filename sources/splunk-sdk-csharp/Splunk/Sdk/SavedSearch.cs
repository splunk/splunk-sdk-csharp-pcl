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
//
// [O] Contracts
//
// [O] Documentation
//
// [ ] Trace messages

namespace Splunk.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    public sealed class SavedSearch : Entity<SavedSearch>
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <param name="namespace">
        /// </param>
        /// <param name="collection">
        /// </param>
        /// <param name="name">
        /// </param>
        internal SavedSearch(Context context, Namespace @namespace, ResourceName collection, string name)
            : base(context, @namespace, collection, name)
        { }

        public SavedSearch()
        { }

        #endregion

        #region Properties

        public ActionSet Actions
        {
            get { return this.Content.GetValue("Action", ActionSetConverter.Instance); }
        }

        public Eai Eai
        {
            get { return this.Content.GetValue("Eai", EaiConverter.Instance); }
        }

        public bool IsDisabled
        {
            get { return this.Content.GetValue("IsDisabled", BooleanConverter.Instance); }
        }

        public bool IsScheduled
        {
            get { return this.Content.GetValue("IsScheduled", BooleanConverter.Instance); }
        }

        public bool IsVisible
        {
            get { return this.Content.GetValue("IsVisible", BooleanConverter.Instance); }
        }

        public int MaxConcurrent
        {
            get { return this.Content.GetValue("MaxConcurrent", Int32Converter.Instance);  }
        }

        public bool RealtimeSchedule
        {
            get { return this.Content.GetValue("RealtimeSchedule", BooleanConverter.Instance); }
        }

        public bool RestartOnSearchPeerAdd
        {
            get { return this.Content.GetValue("RestartOnSearchpeerAdd", BooleanConverter.Instance); }
        }

        public string QualifiedSearch
        {
            get { return this.Content.GetValue("QualifiedSearch", StringConverter.Instance); }
        }

        public bool RunOnStartup
        {
            get { return this.Content.GetValue("RunOnStartup", BooleanConverter.Instance); }
        }

        public string Search
        {
            get { return this.Content.GetValue("Search", StringConverter.Instance); }
        }

        #endregion

        #region Methods for retrieving search results

        #endregion

        #region Privates

        #endregion

        #region Types

        public class ActionSet : ExpandoAdapter
        {
            internal ActionSet(ExpandoObject expandoObject)
                : base(expandoObject)
            { }

            public EmailAction Email
            {
                get { return this.GetValue("Email", ActionConverter<EmailAction>.Instance); }
            }

            public PopulateLookupAction PopulateLookup
            {
                get { return this.GetValue("PopulateLookup", ActionConverter<PopulateLookupAction>.Instance); }
            }

            public RssAction Rss
            {
                get { return this.GetValue("Rss", ActionConverter<RssAction>.Instance); }
            }

            public SummaryIndexAction SummaryIndex
            {
                get { return this.GetValue("SummaryIndex", ActionConverter<SummaryIndexAction>.Instance); }
            }
        }

        public class Action : ExpandoAdapter
        {
            public Action()
            { }

            public bool IsEnabled
            {
                get { return this.GetValue("IsEnabled", BooleanConverter.Instance); }
            }

            public string MaxResults
            {
                get { return this.GetValue("Maxresults", StringConverter.Instance); }
            }

            public string MaxTime
            {
                get { return this.GetValue("Maxtime", StringConverter.Instance);  }
            }

            public bool TrackAlert
            {
                get { return this.GetValue("TrackAlert", BooleanConverter.Instance); }
            }

            public string Ttl
            {
                get { return this.GetValue("Ttl", StringConverter.Instance); }
            }
        }

        /// <summary>
        /// Provides a converter to convert <see cref="ExpandoObject"/> instances to
        /// <see cref="Acl"/> objects.
        /// </summary>
        sealed class ActionConverter<TAction> : ValueConverter<TAction> where TAction : Action, new()
        {
            static ActionConverter()
            {
                Instance = new ActionConverter<TAction>();
            }

            public static ActionConverter<TAction> Instance
            { get; private set; }

            public override TAction Convert(object input)
            {
                var value = input as TAction;

                if (value != null)
                {
                    return value;
                }

                var expandoObject = input as ExpandoObject;

                if (expandoObject != null)
                {
                    return new TAction() { ExpandoObject = expandoObject };
                }

                throw new InvalidDataException(string.Format("Expected {0}: {1}", TypeName, input)); // TODO: improved diagnostices
            }
        }

        /// <summary>
        /// Provides a converter to convert <see cref="ExpandoObject"/> instances to
        /// <see cref="Acl"/> objects.
        /// </summary>
        sealed class ActionSetConverter : ValueConverter<ActionSet>
        {
            static ActionSetConverter()
            {
                Instance = new ActionSetConverter();
            }

            public static ActionSetConverter Instance
            { get; private set; }

            public override ActionSet Convert(object input)
            {
                var value = input as ActionSet;

                if (value != null)
                {
                    return value;
                }

                var expandoObject = input as ExpandoObject;

                if (expandoObject != null)
                {
                    return new ActionSet(expandoObject);
                }

                throw new InvalidDataException(string.Format("Expected {0}: {1}", TypeName, input)); // TODO: improved diagnostices
            }
        }

        public class EmailAction : Action
        {
            public EmailAction()
            { }
        }

        public class PopulateLookupAction : Action
        {
            public PopulateLookupAction()
            { }
        }

        public class RssAction : Action
        {
            public RssAction()
            { }
        }

        public class SummaryIndexAction : Action
        {
            public SummaryIndexAction()
            { }
        }

        #endregion
    }
}
