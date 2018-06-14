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

namespace Splunk.Client
{
    /// <summary>
    /// This class is an implementation of the System.Diagnostics.Contracts class to get around
    /// <see href="https://github.com/Microsoft/CodeContracts/issues/476">issues with CCRewrite on newer versions of
    /// Visual Studio.</see>
    /// Contains static methods for representing program contracts.
    /// </summary>
    internal static class Contract
    {
        /// <summary>
        /// Ensure that a certain condition is met and throws an exception if it is not.
        /// </summary>
        /// <param name="condition">Condition to be met.</param>
        /// <exception cref="System.ArgumentException">If condition is false.</exception>
        internal static void Requires(bool condition)
        {
            if (!condition)
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Ensure that a certain condition is met and throw an exception of <typeparamref name="T"/> if it is not.
        /// </summary>
        /// <typeparam name="T">Type of exception to throw.</typeparam>
        /// <param name="condition">Condition to be met.</param>
        /// <param name="message">Message in the exception to throw if condition is not met.</param>
        /// <exception cref="System.Exception">If condition is false.</exception>
        internal static void Requires<T>(bool condition, string message) where T : Exception
        {
            if (!condition)
            {
                throw (T)Activator.CreateInstance(typeof(T), message);
            }
        }

        /// <summary>
        /// Ensure that a certain condition is met and throw an exception of <typeparamref name="T"/> if it is not.
        /// </summary>
        /// <typeparam name="T">Type of exception to throw.</typeparam>
        /// <param name="condition">Condition to be met.</param>
        internal static void Requires<T>(bool condition) where T : Exception, new()
        {
            if (!condition)
            {
                throw new T();
            }
        }
    }
}