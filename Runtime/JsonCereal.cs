using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;


namespace bluejayvrstudio
{
    public static class AsyncIO
    {
        public static async Task SaveFileAsync(string filepath, string json)
        {
            using (StreamWriter r = new StreamWriter(filepath, false))
            {
                await r.WriteAsync(json);
                r.Flush();
                r.Close();
                r.Dispose();
            }
        }

        public static async Task<string> ReadFileAsync(string filepath)
        {
            using (StreamReader r = new StreamReader(filepath))
            {
                string json = await r.ReadToEndAsync();
                r.Close();
                r.Dispose();
                return json;
            }
        }
    }

    public static class CerealAsync
    {
        public static async Task<string> Serialize(object obj)
        {
            return await Task.Run(() =>
            {
                return JsonConvert.SerializeObject(obj);
            });
        }

        public static async Task<T> Deserialize<T>(string json)
        {
            return await Task.Run(() =>
            {
                return JsonConvert.DeserializeObject<T>(json);
            });
        }

        public static async Task<T> DeepCopy<T>(T obj)
        {
            return await Deserialize<T>(await Serialize(obj));
        }
        public static async Task SerializeToPath(object obj, string path)
        {
            await AsyncIO.SaveFileAsync(path, await Serialize(obj));
        }

        public static async Task<T> DeserializeFromPath<T>(string path)
        {
            return await Deserialize<T>(await AsyncIO.ReadFileAsync(path));
        }
    }

    // Names subject to change
    [Serializable]
    public class _Vector3
    {
        public float x;
        public float y;
        public float z;

        public _Vector3() { }
        public _Vector3(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public static _Vector3 FromUnityVector3(Vector3 UnityVector3)
        {
            return new _Vector3(UnityVector3.x, UnityVector3.y, UnityVector3.z);
        }

        public Vector3 ToUnityVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public class _Quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public _Quaternion() { }
        public _Quaternion(float _x, float _y, float _z, float _w)
        {
            x = _x;
            y = _y;
            z = _z;
            w = _w;
        }

        public static _Quaternion FromUnityQuaternion(Quaternion UnityQuaternion)
        {
            return new _Quaternion(UnityQuaternion.x, UnityQuaternion.y, UnityQuaternion.z, UnityQuaternion.w);
        }

        public Quaternion ToUnityQuaternion()
        {
            var rotation = new Quaternion();
            rotation.x = x;
            rotation.y = y;
            rotation.z = z;
            rotation.w = w;
            return rotation;
        }
    }
}