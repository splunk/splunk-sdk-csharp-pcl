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

namespace Splunk.Client
{
    #if __MonoCS__

    //// This doesn't seem like it should not be necessary according to 
    //// Mono 2.8 notes: http://www.mono-project.com/Release_Notes_Mono_2.8
    //// as it states if the CONTRACTS_FULL compiler directive is not set, the
    //// calls to Contract should be removed by the compiler. However when
    //// testing in Xamarin Studio, this does not appear to be the case.
    public static class Contract
    {
        public static void Requires (bool check, string name="") 
        { }

        public static void Requires<T> (bool check, string name="")
        { }
    }

    #endif
}