//-----------------------------------------------------------------------
// Copyright (c) Microsoft Open Technologies, Inc.
// All Rights Reserved
// Apache License 2.0
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.IdentityModel.Test
{
    /// <summary>
    /// Mixed bag of funtionality:
    ///     Generically calling Properties
    /// </summary>
    public static class TestUtilities
    {

        /// <summary>
        /// Gets a named property on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <param name="propertyValue"></param>
        public static object GetProperty(object obj, string property)
        {
            Type type = obj.GetType();
            PropertyInfo propertyInfo = type.GetProperty(property);

            Assert.IsNotNull(propertyInfo, "property is not found: " + property + ", type: " + type.ToString());

            return propertyInfo.GetValue(obj);
        }


        /// <summary>
        /// Set a named property on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <param name="propertyValue"></param>
        public static void SetProperty(object obj, string property, object propertyValue)
        {
            Type type = obj.GetType();
            PropertyInfo propertyInfo = type.GetProperty(property);

            Assert.IsNotNull(propertyInfo, "property is not found: " + property + ", type: " + type.ToString());

            object retval = propertyInfo.GetValue(obj);
            if (propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(obj, propertyValue);
            }
            else
            {
                Assert.Fail("property 'set' is not found: " + property + ", type: " + type.ToString());
            }
        }

        /// <summary>
        /// Gets and sets a named property on an object. Checks: initial value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <param name="initialPropertyValue"></param>
        /// <param name="setPropertyValue"></param>
        public static void GetSet(object obj, string property, object initialPropertyValue, object[] setPropertyValues)
        {
            Type type = obj.GetType();
            PropertyInfo propertyInfo = type.GetProperty(property);

            Assert.IsNotNull(propertyInfo, "property get is not found: " + property + ", type: " + type.ToString());

            object retval = propertyInfo.GetValue(obj);
            Assert.IsTrue(initialPropertyValue == retval);

            if (propertyInfo.CanWrite)
            {
                foreach (object propertyValue in setPropertyValues)
                {
                    propertyInfo.SetValue(obj, propertyValue);
                    retval = propertyInfo.GetValue(obj);
                    Assert.IsTrue(propertyValue == retval);
                }
            }
        }

        /// <summary>
        /// Gets and sets a named property on an object.
        /// </summary>
        /// <param name="obj">object that has 'get' and 'set'.</param>
        /// <param name="property">the name of the property.</param>
        /// <param name="propertyValue">value to set on the property.</param>
        /// <param name="expectedException">checks that exception is correct.</param>
        public static void GetSet(object obj, string property, object propertyValue, ExpectedException expectedException)
        {
            Assert.IsNotNull(obj, "'obj' can not be null");
            Assert.IsFalse(string.IsNullOrWhiteSpace(property), "'property' can not be null or whitespace");

            Type type = obj.GetType();
            PropertyInfo propertyInfo = type.GetProperty(property);

            Assert.IsNotNull(propertyInfo, "'get is not found for property: '" + property + "', type: '" + type.ToString() + "'");
            Assert.IsTrue(propertyInfo.CanWrite, "can not write to property: '" + property + "', type: '" + type.ToString() + "'");

            try
            {
                propertyInfo.SetValue(obj, propertyValue);
                object retval = propertyInfo.GetValue(obj);
                Assert.AreEqual(propertyValue, retval);
                expectedException.ProcessNoException();
            }
            catch (Exception exception)
            {
                // pass inner exception
                expectedException.ProcessException(exception.InnerException);
            }
        }

        /// <summary>
        /// Calls all public instance and static properties on an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="testcase">contains info about the current test case</param>
        public static void CallAllPublicInstanceAndStaticPropertyGets(object obj, string testcase)
        {
            if (obj == null)
            {
                Console.WriteLine(string.Format("Entering: '{0}', obj is null, have to return.  Is the Testcase: '{1}' right?", MethodBase.GetCurrentMethod(), testcase ?? "testcase is null"));
                return;
            }

            Type type = obj.GetType();
            Console.WriteLine(string.Format("Testcase: '{0}', type: '{1}', Method: '{2}'.", testcase ?? "testcase is null", type, MethodBase.GetCurrentMethod()));

            // call get all public static properties of MyClass type

            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            // Touch each public property
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                try
                {
                    if (propertyInfo.GetMethod != null)
                    {
                        object retval = propertyInfo.GetValue(obj, null);
                    }
                }
                catch (Exception ex)
                {
                    Assert.Fail(string.Format("Testcase: '{0}', type: '{1}', property: '{2}', exception: '{3}'", type, testcase ?? "testcase is null", propertyInfo.Name, ex));
                }
            }
        }

        public static string SerializeAsSingleCommaDelimitedString(IEnumerable<string> strings)
        {
            if (null == strings)
            {
                return "null";
            }

            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (string str in strings)
            {
                if (first)
                {
                    sb.AppendFormat("{0}", str ?? "null");
                    first = false;
                }
                else
                {
                    sb.AppendFormat(", {0}", str ?? "null");
                }
            }

            if (first)
            {
                return "empty";
            }

            return sb.ToString();
        }
    }
}