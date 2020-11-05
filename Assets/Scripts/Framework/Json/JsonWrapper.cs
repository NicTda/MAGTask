//
// Copyright (c) 2020 Nicolas Tanda. All rights reserved
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace CoreFramework
{
    namespace Json
    {
        /// Definition of a json dictionary
        /// 
        public class JsonDictionary : Dictionary<string, object>
        {
        }

        /// Wrapper for the MiniJSON library
        /// 
        public static class JsonWrapper
        {
            private const string k_dateMin = "Min";
            private const string k_dateMax = "Max";
            private const string k_dateFormat = "yyyy/MM/dd-HH:mm:ss";

            /// @return A new json dictionary
            /// 
            public static JsonDictionary CreateJsonDictionary()
            {
                return new JsonDictionary();
            }

            /// @param size
            /// 	The size of the array to create
            /// 
            /// @return A new json array
            /// 
            public static object[] CreateJsonArray(int size)
            {
                return new object[size];
            }

            /// @param jsonData
            /// 	String to deserialize
            /// 
            /// @return The deserialized dictionary
            /// 
            public static object Deserialize(this string jsonData)
            {
                return MiniJSON.Json.Deserialize(jsonData);
            }

            /// @param obj
            /// 	The object to serialize
            /// 
            /// @return The serialized string
            /// 
            public static string Serialize(this object obj)
            {
                return MiniJSON.Json.Serialize(obj);
            }

            /// Will serialize the content of a iList.
            /// 
            /// @param iList
            ///     The iList to serialize
            /// 
            /// @return The json array containing the serialized iList
            /// 
            private static object[] SerializeIListOfTypes<T>(IList<T> iList)
            {
                var jsonArray = JsonWrapper.CreateJsonArray(iList.Count);
                for(int i = 0; i < iList.Count; ++i)
                {
                    jsonArray[i] = iList[i];
                }
                return jsonArray;
            }

            /// Will serialize the content of a iList.
            /// The iList must contain serializable objects.
            /// 
            /// @param iList
            ///     The iList to serialize
            /// 
            /// @return The json array containing the serialized iList
            /// 
            private static object[] SerializeIListOfSerializables<T>(IList<T> iList) where T : ISerializable
            {
                var jsonArray = JsonWrapper.CreateJsonArray(iList.Count);
                for(int i = 0; i < iList.Count; ++i)
                {
                    jsonArray[i] = iList[i].Serialize();
                }
                return jsonArray;
            }

            /// Will serialize the content of a list.
            /// 
            /// @param list
            /// 	The list to serialize
            /// 
            /// @return The json array containing the serialized list
            /// 
            public static object[] SerializeListOfTypes<T>(List<T> list)
            {
                return SerializeIListOfTypes(list);
            }

            /// Will serialize the content of a list.
            /// The list must contain serializable objects.
            /// 
            /// @param list
            ///     The list to serialize
            /// 
            /// @return The json array containing the serialized list
            /// 
            public static object[] SerializeListOfSerializables<T>(List<T> list) where T : ISerializable
            {
                return SerializeIListOfSerializables(list);
            }

            /// Will serialize the content of an array.
            /// 
            /// @param array
            ///     The array to serialize
            /// 
            /// @return The json array containing the serialized array
            /// 
            public static object[] SerializeArrayOfTypes<T>(T[] array)
            {
                return SerializeIListOfTypes(array);
            }

            /// Will serialize the content of an array.
            /// The array must contain serializable objects.
            /// 
            /// @param array
            /// 	The array to serialize
            /// 
            /// @return The json array containing the serialized array
            /// 
            public static object[] SerializeArrayOfSerializables<T>(T[] array) where T : ISerializable
            {
                return SerializeIListOfSerializables(array);
            }

            /// @param path
            /// 	The path of the file to parse
            /// 
            /// @return The parsed dictionary
            /// 
            public static Dictionary<string, object> ParseJsonFile(string path, FileSystem.Location location = FileSystem.Location.Persistent)
            {
#if UNITY_WEBPLAYER == false
                if(FileSystem.DoesFileExist(path, location))
                {
                    // If failing switch Player to Android / iOS
                    string json = FileSystem.ReadTextFile(path, location);
                    return Deserialize(json) as Dictionary<string, object>;
                }
#endif
                return null;
            }

            /// @param textAsset
            /// 	The text asset to parse
            /// 
            /// @return The parsed dictionary
            /// 
            public static object ParseJsonFromTextAsset(TextAsset textAsset)
            {
                if(textAsset != null)
                {
                    return Deserialize(textAsset.text);
                }
                return null;
            }

            /// @param dictionary
            /// 	The dictionary to search
            /// @param key
            /// 	The key to look for
            /// 
            /// @return The value for the given key. Returns null if not found
            /// 
            public static object GetValue(this Dictionary<string, object> dictionary, string key)
            {
                object value = null;
                if(dictionary != null && dictionary.ContainsKey(key))
                {
                    value = dictionary[key];
                }
                return value;
			}

			/// Deserializes a single data value data - asserts on null value
			///
			/// @param data
			///     The json data
			/// @param key
			///     The json key for the value to deserialize
			/// 
			/// @return Deserialized object from the json dictionary
			/// 
			public static object GetValueOrAssert(this Dictionary<string, object> data, string key)
			{
				object dataValue = JsonWrapper.GetValue(data, key);
				Debug.AssertFormat(dataValue != null, "Failed, {0} does not correspond to a value", key);
				return dataValue;
			}

			/// @param data
			///     The json data
			/// @param key
			///     The json key for the value to deserialize
			/// 
			/// @return Deserialized and casted object from the json dictionary
			/// 
			public static DataType GetValueOrAssert<DataType>(this Dictionary<string, object> data, string key) where DataType : IConvertible
			{
				object dataObject = data.GetValueOrAssert(key);
				DataType dataValue = (DataType)Convert.ChangeType(dataObject, typeof(DataType));
				Debug.AssertFormat(dataValue != null, "Failed, {0} is not of a valid type", key);
				return dataValue;
			}

            /// Deserializes a single data value data
            ///
            /// @param data
            ///     The json data
            /// @param key
            ///     The json key for the value to deserialize
            /// 
            /// @return Deserialized object from the json dictionary 
            ///     or a default value if the deserialisation yields a null value
            /// 
            public static object GetValueOrDefault(this Dictionary<string, object> data, string key, object defaultValue)
            {
                object dataValue = JsonWrapper.GetValue(data, key);
                if(dataValue == null)
                {
                    return defaultValue;
                }
                else
                {
                    return dataValue;
                }
            }

            /// @param obj
            /// 	The object to cast
            /// 
            /// @return The casted bool
            /// 
            public static bool AsBool(this object obj)
            {
                return (bool)obj;
            }

            /// @param obj
            /// 	The object to cast
            /// 
            /// @return The casted int
            /// 
            public static int AsInt(this object obj)
            {
                if (obj is int)
                {
                    // The object might already be an int
                    return (int)obj;
                }
                else if(obj is string)
                {
                    // Parse string
                    return int.Parse(obj.AsString());
                }
				else
                {
					// MiniJSON doesn't deal with Int and requires the cast to Long
					return (int)AsLong(obj);
				}
            }

            /// @param obj
            /// 	The object to cast
            /// 
            /// @return The casted long
            /// 
            public static long AsLong(this object obj)
            {
                return (long)obj;
            }

            /// @param obj
            /// 	The object to cast
            /// 
            /// @return The casted float
            /// 
            public static float AsFloat(this object obj)
            {
				if(obj is float)
				{
					// The object is already a float
					return (float)obj;
                }
                else if (obj is string)
                {
                    // Parse string
                    return float.Parse(obj.AsString());
                }
                else
				{
					// MiniJSON doesn't deal with Float and requires the cast to Double
					return (float)AsDouble(obj);
				}
            }

            /// @param obj
            /// 	The object to cast
            /// @param precision
            ///     The precision to use
            /// 
            /// @return The casted float
            /// 
            public static float AsFloatJson(this object obj, double precision = 2)
            {
                if (obj is float)
                {
                    // The object is already a float
                    return (float)obj;
                }
                if (obj is double)
                {
                    // The object is a double
                    return (float)obj.AsDouble();
                }
                return (float)obj.AsDouble() / (float)Math.Pow(10.0, precision);
            }

            /// @param value
            /// 	The value to cast
            /// @param precision
            ///     The precision to use
            /// 
            /// @return The casted int
            /// 
            public static int FloatJson(this float value, double precision = 2)
            {
                return Mathf.RoundToInt(value * (float)Math.Pow(10.0, precision));
            }

            /// @param obj
            /// 	The object to cast
            /// 
            /// @return The casted double
            /// 
            public static double AsDouble(this object obj)
            {
                if(obj is double)
                {
					// The object is already a double
                    return (double)obj;
                }
                else if (obj is string)
                {
                    // Parse string
                    return double.Parse(obj.AsString());
                }
                else
                {
                    return (double)AsLong(obj);
                }
            }

            /// @param obj
            /// 	The object to cast
            /// 
            /// @return The casted string
            /// 
            public static string AsString(this object obj)
            {
                return (string)obj;
            }

            /// @param obj
            /// 	The object to cast
            /// 
            /// @return The casted date time
            /// 
            public static DateTime AsDateTime(this object obj)
            {
                DateTime dateTime = DateTime.MinValue;
                if(obj is string)
                {
                    var dateString = obj.AsString();
                    if (dateString == k_dateMax)
                    {
                        dateTime = DateTime.MaxValue;
                    }
                    else if (dateString == k_dateMin)
                    {
                        dateTime = DateTime.MinValue;
                    }
                    else
                    {
                        try
                        {
                            // Try the forced format first
                            dateTime = DateTime.ParseExact(dateString, k_dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
                        }
                        catch
                        {
                            // Default to local date format
                            DateTime.TryParse(dateString, out dateTime);
                        }

                        if (dateTime.Year == DateTime.MaxValue.Year)
                        {
                            // There seem to be an issue with storing MaxValue as a string
                            dateTime = DateTime.MaxValue;
                        }
                    }
                }
                return dateTime;
            }

            /// @param date
            /// 	The date object
            /// 
            /// @return The date in fixed string format
            /// 
            public static string DateToString(this DateTime date)
            {
                string dateString = string.Empty;
                if (date == DateTime.MaxValue)
                {
                    dateString = k_dateMax;
                }
                else if (date == DateTime.MinValue)
                {
                    dateString = k_dateMin;
                }
                else
                {
                    dateString = date.ToString(k_dateFormat, CultureInfo.InvariantCulture);
                }
                return dateString;
            }

            /// @param date
            /// 	The date object
            /// 
            /// @return The date in string from, depending on device's local format
            /// 
            public static string DateToStringLocal(this DateTime date)
            {
                string dateString = string.Empty;
                if (date == DateTime.MaxValue)
                {
                    dateString = k_dateMax;
                }
                else if (date == DateTime.MinValue)
                {
                    dateString = k_dateMin;
                }
                else
                {
                    dateString = date.ToString();
                }
                return dateString;
            }

            /// @param obj
            /// 	The object to cast
            /// 
            /// @return The casted vector2
            /// 
            public static Vector2 AsVector2(this object obj)
            {
                string stringObj = AsString(obj);
                char[] k_separators = { ',', '(', ')' };
                string[] splitString = stringObj.Split(k_separators);
                if(splitString.Length != 4)
                {
                    Debug.LogError("Cannot deserialize to Vector2 : " + stringObj);
                    return Vector2.zero;
                }
                return new Vector2(float.Parse(splitString[1]), float.Parse(splitString[2]));
            }

            /// @param obj
            /// 	The object to cast
            /// 
            /// @return The casted vector3
            /// 
            public static Vector3 AsVector3(this object obj)
            {
                string stringObj = AsString(obj);
                char[] k_separators = { ',', '(', ')' };
                string[] splitString = stringObj.Split(k_separators);
                if(splitString.Length != 5)
                {
                    Debug.LogError("Cannot deserialize to Vector3 : " + stringObj);
                    return Vector3.zero;
                }
                return new Vector3(float.Parse(splitString[1]), float.Parse(splitString[2]), float.Parse(splitString[3]));
            }

            /// @param obj
            ///     The object to cast
            /// 
            /// @return The casted Color32
            /// 
            public static Color32 AsColor32(this object obj)
            {
                Color32 col = Color.magenta;
                string stringObj = AsString(obj);
                char[] k_separators = { ',', '(', ')' };
                string[] splitString = stringObj.Split(k_separators);
                if(splitString.Length != 6)
                {
                    Debug.LogError("Cannot deserialize to Color32 : " + stringObj);
                }
                else
                {
                    col = new Color32(byte.Parse(splitString[1]), byte.Parse(splitString[2]), byte.Parse(splitString[3]), byte.Parse(splitString[4]));
                }
                return col;
            }

            /// @param obj
            ///     The object to cast
            /// 
            /// @return The casted Color
            /// 
            public static Color AsColor(this object obj)
            {
                Color col = Color.magenta;
                string stringObj = AsString(obj);
                if (ColorUtility.TryParseHtmlString(stringObj, out col) == false)
                {
                    // Manual parsing
                    char[] k_separators = { ',', '(', ')' };
                    string[] splitString = stringObj.Split(k_separators);
                    if (splitString.Length != 6)
                    {
                        Debug.LogError("Cannot deserialize to Color : " + stringObj);
                    }
                    else
                    {
                        float[] rgba = new float[4];
                        bool notFloat = false;
                        for (int i = 0; i < rgba.Length; ++i)
                        {
                            rgba[i] = float.Parse(splitString[i + 1]);
                            notFloat |= (rgba[i] > 1.0f);
                        }

                        if (notFloat)
                        {
                            Debug.LogWarning("Colour passed with value(s) outside of range [0-1]. Assuming range [0-255].");
                            col = (Color)(new Color32((byte)rgba[0], (byte)rgba[1], (byte)rgba[2], (byte)rgba[3]));
                        }
                        else
                        {
                            col = new Color(rgba[0], rgba[1], rgba[2], rgba[3]);
                        }
                    }
                }
                return col;
            }

            /// @param obj
            /// 	The object to cast
            /// 
            /// @return The casted Quaternion
            /// 
            public static Quaternion AsQuaternion(this object obj)
            {
                string stringObj = AsString(obj);
                char[] k_separators = { ',', '(', ')' };
                string[] splitString = stringObj.Split(k_separators);
                if(splitString.Length != 6)
                {
                    Debug.LogError("Cannot deserialize to Quaternion : " + stringObj);
                    return Quaternion.identity;
                }
                return new Quaternion(float.Parse(splitString[1]), float.Parse(splitString[2]), float.Parse(splitString[3]), float.Parse(splitString[4]));
            }

            /// Converts object to enum
            /// 
            /// @param obj
            /// 	The object to cast
            /// 
            /// @return The casted enum
            /// 
            public static EnumType AsEnum<EnumType>(this object obj)
            {
				if(obj is EnumType)
				{
					// The object is already an enum of that type
					return (EnumType)obj;
				}
				else
				{
					return (EnumType)Enum.Parse(typeof(EnumType), obj.AsString());
				}
            }

            /// Converts a list of objects to a list of enums
            /// 
            /// @param data
            ///     The json object to cast
            /// 
            /// @return The list of casted enums
            /// 
            public static List<EnumType> AsEnumList<EnumType>(this object data)
            {
                List<object> objectList = data.AsList<object>();
                List<EnumType> enums = new List<EnumType>();
                foreach(object obj in objectList)
                {
                    enums.Add(obj.AsEnum<EnumType>());
                }

                return enums;
			}

			/// @param obj
			/// 	The object to cast
			/// 
			/// @return The casted json object
			/// 
			public static T AsSerializable<T>(this object obj) where T : ISerializable, new()
			{
				T serializable = new T();
				serializable.Deserialize(obj);
				return serializable;
			}

            /// @param obj
            ///     The object to cast
            /// 
            /// @return The casted Array
            /// 
            public static T[] AsArrayOfSerializables<T>(this object obj) where T : ISerializable, new()
            {
                List<T> list = obj.AsListOfSerializables<T>();
                return list.ToArray();
            }

            /// @param obj
            /// 	The object to cast
            /// 
            /// @return The casted Array
            /// 
            public static T[] AsArray<T>(this object obj)
            {
                List<T> list = new List<T>();
                foreach(var newObject in obj as IEnumerable)
                {
                    if(list is List<string>)
                    {
                        (list as List<string>).Add(Convert.ToString(newObject));
                    }
                    else if(list is List<int>)
                    {
                        (list as List<int>).Add(Convert.ToInt32(newObject));
                    }
                    else if (list is List<float>)
                    {
                        (list as List<float>).Add(Convert.ToSingle(newObject));
                    }
                    else if (list is List<double>)
                    {
                        (list as List<double>).Add(Convert.ToDouble(newObject));
                    }
                    else if(list is List<bool>)
                    {
                        (list as List<bool>).Add(Convert.ToBoolean(newObject));
                    }
                    else if(list is List<object>)
                    {
                        (list as List<object>).Add(newObject);
                    }
                    else
                    {
                        Debug.LogError("Type not handled");
                    }
                    //TODO: other types may be needed?
                }
                return list.ToArray();
            }

            /// @param obj
            /// 	The object to cast
            /// 
            /// @return The casted json list
            /// 
            public static List<T> AsListOfSerializables<T>(this object obj) where T : ISerializable, new()
            {
                List<T> list = new List<T>();
                foreach(var value in obj as IEnumerable)
                {
					T serializable = value.AsSerializable<T>();
                    list.Add(serializable);
                }
                return list;
            }

            /// @param obj
            ///     The object to cast
            /// 
            /// @return The casted json list
            /// 
            public static List<T> AsList<T>(this object obj)
            {
                bool validType = (typeof(T).IsPrimitive) || (typeof(T) == typeof(string)) || (typeof(T) == typeof(object)) || (typeof(T) == typeof(Vector2));
                if(validType == false)
                {
                    Debug.LogWarning("This function is meant to be used with primitive types only. Consider using \"AsListOfSerializables\"");
                }
                List<T> list = new List<T>();
                if (obj != null)
                {
                    foreach (var value in obj as IEnumerable)
                    {
                        // A special case is required for int, as casting object to int fails
                        if (list is List<int>)
                        {
                            (list as List<int>).Add(Convert.ToInt32(value));
                        }
                        else if (list is List<Vector2>)
                        {
                            (list as List<Vector2>).Add(AsVector2(value));
                        }
                        else if (list is List<float>)
                        {
                            (list as List<float>).Add(Convert.ToSingle(value));
                        }
                        else
                        {
                            list.Add((T)value);
                        }
                    }
                }
                return list;
            }

            /// Casts a json dictionary to another dictionary of type <string, object>,
            /// asserts if a null value is produced
            ///
            /// @param obj
            /// 	The object to cast
            /// @param assert
            ///     Whether to assert or allow null value
            /// 
            /// @return The casted json dictionary if successful, else null
            /// 
            public static Dictionary<string, object> AsDictionary(this object obj, bool assert = true)
            {
                Dictionary<string, object> data = obj as Dictionary<string, object>;
                if(assert == true)
                {
                    Debug.Assert(data != null, "Could not cast to dictionary, null data produced");
                }

                return data;
            }

            /// Casts a json dictionary to another dictionary of type <T1, T2>, 
            /// where T1 and T2 are of type IConvertable
            /// 
            /// @param obj
            /// 	The object to cast
            /// 
            /// @return The casted json dictionary as Dictionary<T1, T2> if successful, else null
            /// 
            public static Dictionary<T1, T2> AsDictionary<T1, T2>(this object obj) where T1 : IConvertible where T2 : IConvertible
            {
                var dictionary = obj.AsDictionary();// as Dictionary<string, object>;
                Dictionary<T1, T2> castDictionary = new Dictionary<T1, T2>();
                foreach(var entry in dictionary)
                {
                    T1 firstCast = (T1)Convert.ChangeType(entry.Key, typeof(T1));
                    T2 secondCast = (T2)Convert.ChangeType(entry.Value, typeof(T2));

                    castDictionary[firstCast] = secondCast;
                }

                return castDictionary;
            }
        }
    }
}
