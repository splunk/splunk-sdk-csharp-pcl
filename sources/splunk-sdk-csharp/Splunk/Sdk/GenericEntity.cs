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
    using System.Dynamic;

    public class GenericEntity : Entity<GenericEntity>
    {
        #region Constructors

        internal GenericEntity(Context context, Namespace @namespace, ResourceName collection, string name)
            : base(context, @namespace, collection, name)
        { }

        public GenericEntity() // TODO: Remove this after refactoring EntityCollection<TEntity> and AtomFeed<TEntity> with a Entity<TEntity> factory
        { }

        #endregion
    }
}
