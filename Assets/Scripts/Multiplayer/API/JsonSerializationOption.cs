#region copyright
// Copyright (c) 2021 Brandenburgische Technische Universität Cottbus - Senftenberg
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#endregion
using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Multiplayer.API
{
    public class JsonSerializationOption : ISerializationOption
    {
        public string ContentType => "application/json";

        public T Deserialize<T>(byte[] data)
        {
            string response = System.Text.Encoding.UTF8.GetString(data);
            try
            {
                if (!response.Contains("[") && !response.Contains("{"))
                {
                    return (T)ChangeType(response, typeof(T));//(T)Convert.ChangeType(response, typeof(T));
                }
                var result = JsonConvert.DeserializeObject<T>(response);
                Debug.Log($"Success: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Could not parse response {response}. {ex.Message}");
                return default(T);
            }
        }


        private object ChangeType(object value, Type type)
        {
            if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            if (value == null) return null;
            if (type == value.GetType()) return value;
            if (type.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(type, value as string);
                else
                    return Enum.ToObject(type, value);
            }
            if (!type.IsInterface && type.IsGenericType)
            {
                Type innerType = type.GetGenericArguments()[0];
                object innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(type, new object[] { innerValue });
            }
            if (value is string && type == typeof(Guid)) return new Guid(value as string);
            if (value is string && type == typeof(Version)) return new Version(value as string);          
            if (!(value is IConvertible)) return value;
            return Convert.ChangeType(value, type);
        }
    }

    public interface ISerializationOption
    {
        string ContentType { get; }
        T Deserialize<T>(byte[] data);
    }
}
