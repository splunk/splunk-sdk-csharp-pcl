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
    using System.Dynamic;

    public class Job : Entity<Job>
    {
        #region Constructors

        internal Job(Context context, Namespace @namespace, ResourceName collection, string name, ExpandoObject record = null) 
            : base(context, @namespace, collection, name, record)
        { }

        public Job() // TODO: Remove this after refactoring EntityCollection<TEntity> and AtomFeed<TEntity> with a Entity<TEntity> factory
        { }

        #endregion

        #region Properties

        public bool IsCompleted
        {
            get { return this.DispatchState == DispatchState.Done; }
        }

        public DispatchState DispatchState
        { 
            get 
            { 
                if (this.backingFields.DispatchState == DispatchState.Unknown)
                {
                    this.backingFields.DispatchState = Enum.Parse(typeof(DispatchState), this.Record.DispatchState, ignoreCase: true);
                }
                return this.backingFields.DispatchState;
            }
        }

        #endregion

        #region Methods

        protected override void Invalidate()
        {
            this.backingFields = InvalidatedBackingFields;
        }

        #endregion

        #region Privates

        static readonly BackingFields InvalidatedBackingFields = new BackingFields();
        BackingFields backingFields = new BackingFields();

        #endregion

        #region Types

        private struct BackingFields
        {
            public DispatchState DispatchState;
        }

        #endregion
    }
}
