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

using System;

namespace Splunk.ModularInputs
{
    /// <summary>
    /// The <see cref="DebuggerAttachPoints"/> contains a list of points in the
    /// modular input pipeline that one can can specify to have the input pause
    /// until a debugger attaches
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// 

    [Flags]
    public enum DebuggerAttachPoints
    {
        None=0,
        ValidateArguments=1,
        StreamEvents=2,
    }
}