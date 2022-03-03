// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Spark.Interop;
using Microsoft.Spark.Interop.Ipc;
using Microsoft.Spark.Sql;

namespace Microsoft.Spark.ML.Feature
{
    /// <summary>
    /// Class for utility classes that can save ML instances in Spark's internal format.
    /// </summary>
    public class ScalaMLWriter : IJvmObjectReferenceProvider
    {
        public ScalaMLWriter(JvmObjectReference jvmObject) => Reference = jvmObject;

        public JvmObjectReference Reference { get; private set; }

        /// <summary>Saves the ML instances to the input path.</summary>
        /// <param name="path">The path to save the object to</param>
        public void Save(string path) => Reference.Invoke("save", path);

        /// <summary>
        /// save() handles overwriting and then calls this method. 
        /// Subclasses should override this method to implement the actual saving of the instance.
        /// </summary>
        /// <param name="path">The path to save the object to</param>
        protected void SaveImpl(string path) => Reference.Invoke("saveImpl", path);

        /// <summary>Overwrites if the output path already exists.</summary>
        public ScalaMLWriter Overwrite()
        {
            Reference.Invoke("overwrite");
            return this;
        }

        /// <summary>
        /// Adds an option to the underlying MLWriter. See the documentation for the specific model's
        /// writer for possible options. The option name (key) is case-insensitive.
        /// </summary>
        /// <param name="key">key of the option</param>
        /// <param name="value">value of the option</param>
        public ScalaMLWriter Option(string key, string value)
        {
            Reference.Invoke("option", key, value);
            return this;
        }

        /// <summary>Sets the Spark Session to use for saving/loading.</summary>
        /// <param name="sparkSession">The Spark Session to be set</param>
        public ScalaMLWriter Session(SparkSession sparkSession)
        {
            Reference.Invoke("session", sparkSession);
            return this;
        }
    }

    /// <summary>
    /// Interface for classes that provide ScalaMLWriter.
    /// </summary>
    public interface ScalaMLWritable
    {
        /// <summary>
        /// Get the corresponding ScalaMLWriter instance.
        /// </summary>
        /// <returns>a <see cref="ScalaMLWriter"/> instance for this ML instance.</returns>
        ScalaMLWriter Write();

        /// <summary>Saves this ML instance to the input path</summary>
        /// <param name="path">The path to save the object to</param>
        void Save(string path);
    }

    /// <summary>
    /// Class for utility classes that can load ML instances.
    /// </summary>
    /// <typeparam name="T">ML instance type</typeparam>
    public class ScalaMLReader<T> : IJvmObjectReferenceProvider
    {
        public ScalaMLReader(JvmObjectReference jvmObject) => Reference = jvmObject;

        public JvmObjectReference Reference { get; private set; }

        /// <summary>
        /// Loads the ML component from the input path.
        /// </summary>
        /// <param name="path">The path the previous instance of type T was saved to</param>
        /// <returns>The type T instance</returns>
        public T Load(string path) =>
            WrapAsType((JvmObjectReference)Reference.Invoke("load", path));

        /// <summary>Sets the Spark Session to use for saving/loading.</summary>
        /// <param name="sparkSession">The Spark Session to be set</param>
        public ScalaMLReader<T> Session(SparkSession sparkSession)
        {
            Reference.Invoke("session", sparkSession);
            return this;
        }

        private static T WrapAsType(JvmObjectReference reference)
        {
            ConstructorInfo constructor = typeof(T)
                .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(c =>
                {
                    ParameterInfo[] parameters = c.GetParameters();
                    return (parameters.Length == 1) &&
                        (parameters[0].ParameterType == typeof(JvmObjectReference));
                });

            return (T)constructor.Invoke(new object[] { reference });
        }
    }

    /// <summary>
    /// Interface for objects that provide MLReader.
    /// </summary>
    /// <typeparam name="T">
    /// ML instance type
    /// </typeparam>
    public interface ScalaMLReadable<T>
    {
        /// <summary>
        /// Get the corresponding ScalaMLReader instance.
        /// </summary>
        /// <returns>an <see cref="ScalaMLReader&lt;T&gt;"/> instance for this ML instance.</returns>
        ScalaMLReader<T> Read();
    }
}
