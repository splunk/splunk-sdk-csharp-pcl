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
// [ ] Contracts
// [ ] Documentation
// [ ] Properties & Methods
// [ ] Refactor Resource<TResource> to eliminate all abstract members because
//     there are too many places where they aren't relevant. Move them into
//     Entity<TEntity> and EntityCollection<TCollection, TEntity>

namespace Splunk.Sdk
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class Receiver : Resource<Receiver>
    {
        #region Constructors

        internal Receiver(Context context, Namespace @namespace)
            : base(context, @namespace, ResourceName.Receivers)
        { }

        public Receiver()
        { }

        #endregion

        #region Methods


        public override Task GetAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Record> SendAsync(Stream recordStream)
        {
            throw new NotImplementedException();
        }

        public async Task SendAsync(string record)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
