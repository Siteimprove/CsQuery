using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace CsQuery.Utility
{
    /// <summary>
    /// TODO: This class needs some help. While not thrilled about the idea of writing another JSON
    /// serializer, CsQuery does some unique handling for serialization &amp;  deserialization, e.g.
    /// mapping sub-objects to expando objects.
    /// 
    /// We can do a post-op parsing from any other JSON serializer (such as we are doing now) but
    /// this doubles the overhead required. Look at a customized implementation from Newtonsoft,
    /// though any customization makes it difficult to use a simple strategy for drop-in replacement
    /// of the serializer. Perhaps implement an interface for a serializer wrapper class that lets us
    /// pass any generic serializer that performs needed post-op substitutions as part of the base
    /// library, with an optimized native implementation?
    /// </summary>

    public class JsonSerializer: IJsonSerializer
    {
        private StringBuilder sb = new StringBuilder();

        /// <summary>
        /// Serializes an object to JSON
        /// </summary>
        ///
        /// <param name="value">
        /// The object to serialize
        /// </param>
        ///
        /// <returns>
        /// A JSON string
        /// </returns>

        public string Serialize(object value)
        {
            sb.Clear();
            SerializeImpl(value);
            return sb.ToString();
        }

        /// <summary>
        /// Deserializes a JSON string to an object of the specified type
        /// </summary>
        ///
        /// <param name="value">
        /// The JSON string
        /// </param>
        /// <param name="type">
        /// The type of object to create
        /// </param>
        ///
        /// <returns>
        /// A new object of the specified type
        /// </returns>

        public object Deserialize(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type);
        }

        /// <summary>
        /// Deserializes a JSON string to an object of type T.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// Generic type parameter defining the type of object to return.
        /// </typeparam>
        /// <param name="value">
        /// The value.
        /// </param>
        ///
        /// <returns>
        /// A new object of type T.
        /// </returns>

        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        #region private methods

        private void SerializeImpl(object value) {
            //if ((value is IEnumerable && !value.IsExpando()) || value.IsImmutable())
            if (!Objects.IsExtendableType(value))
            {
                valueToJSON(value);
            }
            else
            {
                sb.Append("{");
                bool first = true;
                foreach (KeyValuePair<string,object> kvp in 
                    Objects.EnumerateProperties<KeyValuePair<string,object>>(
                        value,
                        null)
                ) {
                    if (first)
                    {
                        first = false; 
                    }
                    else
                    {
                        sb.Append(",");
                    }
                    sb.Append("\"" + kvp.Key + "\":");
                    SerializeImpl(kvp.Value);

                }
                sb.Append("}");
            }
        }

        private void valueToJSON(object value)
        {
            if (Objects.IsImmutable(value))
            {
                sb.Append(JsonConvert.SerializeObject(value));
            }
            else if (IsDictionary(value))
            {
                sb.Append("{");
                bool first = true;
                foreach (dynamic item in (IEnumerable)value)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(",");
                    }
                    sb.Append("\"" + item.Key.ToString() + "\":" + JSON.ToJSON(item.Value));
                }
                sb.Append("}");
            }
            else if (value is IEnumerable)
            {
                sb.Append("[");
                bool first = true;
                foreach (object obj in (IEnumerable)value)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(",");
                    }
                    if (Objects.IsImmutable(obj))
                    {
                        valueToJSON(obj);
                    }
                    else
                    {
                        SerializeImpl(obj);
                    }
                }
                sb.Append("]");
            }
            else
            {
                throw new InvalidOperationException("Serializer error: valueToJson called for an object");
            }
        }

        /// <summary>
        /// Test if value implements IDictionary&lt;,&gt;
        /// </summary>
        ///
        /// <param name="value">
        /// The value.
        /// </param>
        ///
        /// <returns>
        /// true if a dictionary, false if not.
        /// </returns>

        private bool IsDictionary(object value)
        {
            Type type = value.GetType();

            return type.GetInterfaces()
                .Select(t => t.GetTypeInfo())
                .Where(t => t.IsGenericType)
                .Select(t => t.GetGenericTypeDefinition())
                .Any(t => t == typeof (IDictionary<,>));

        }


        #endregion

    }

   
}
