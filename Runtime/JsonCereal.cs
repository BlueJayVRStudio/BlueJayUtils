using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
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
            Debug.Log("saved!");
        }

        public static async Task<string> ReadFileAsync(string filepath)
        {
            using (StreamReader r = new StreamReader(filepath))
            {
                string json = await r.ReadToEndAsync();
                r.Close();
                r.Dispose();
                Debug.Log("loaded!");
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

        public static async Task SerializeToPathEncrypted(object obj, string path, string password)
        {
            string JSON = await Serialize(obj);

            if (password.Length == 0) return;
            string pwd1 = password;

            byte[] salt1 = new byte[8];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(salt1);
            }
            string data1 = JSON;

            int myIterations = 1000;

            try
            {
                Rfc2898DeriveBytes k1 = new Rfc2898DeriveBytes(pwd1, salt1, myIterations);
                Rfc2898DeriveBytes k2 = new Rfc2898DeriveBytes(pwd1, salt1);
                // Encrypt the data.
                Aes encAlg = Aes.Create();
                encAlg.Key = k1.GetBytes(16);
                MemoryStream encryptionStream = new MemoryStream();
                CryptoStream encrypt = new CryptoStream(encryptionStream, encAlg.CreateEncryptor(), CryptoStreamMode.Write);
                byte[] utfD1 = new System.Text.UTF8Encoding(false).GetBytes(data1);

                encrypt.Write(utfD1, 0, utfD1.Length);
                encrypt.FlushFinalBlock();
                encrypt.Close();
                byte[] edata1 = encryptionStream.ToArray();
                k1.Reset();

                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                {
                    fileStream.Write(edata1, 0, edata1.Length);
                }

                using (FileStream fileStream = new FileStream(path+".iv", FileMode.Create))
                {
                    fileStream.Write(encAlg.IV, 0, encAlg.IV.Length);
                }

                using (FileStream fileStream = new FileStream(path+".salt", FileMode.Create))
                {
                    fileStream.Write(salt1, 0, salt1.Length);
                }

            }
            catch (Exception e)
            {
                Debug.Log($"Error: {e}");
            }
        }

        public static async Task<T> DeserializeFromPath<T>(string path)
        {
            return await Deserialize<T>(await AsyncIO.ReadFileAsync(path));
        }

        public static async Task<T> DeserializeFromPathEncrypted<T>(string path, string password)
        {
            if (password.Length == 0) return await Deserialize<T>("");

            string Json = await AsyncIO.ReadFileAsync(path);
            byte[] iv;
            byte[] salt;
            byte[] eJSON;

            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                eJSON = new byte[fileStream.Length];
                fileStream.Read(eJSON, 0, eJSON.Length);
            }
            using (FileStream fileStream = new FileStream(path+".iv", FileMode.Open))
            {
                iv = new byte[fileStream.Length];
                fileStream.Read(iv, 0, iv.Length);
            }
            using (FileStream fileStream = new FileStream(path+".salt", FileMode.Open))
            {
                salt = new byte[fileStream.Length];
                fileStream.Read(salt, 0, salt.Length);
            }
            
            string pwd1 = password;

            int myIterations = 1000;

            try
            {
                Rfc2898DeriveBytes k1 = new Rfc2898DeriveBytes(pwd1, salt, myIterations);
                Rfc2898DeriveBytes k2 = new Rfc2898DeriveBytes(pwd1, salt);
  
                Aes decAlg = Aes.Create();
                decAlg.Key = k2.GetBytes(16);
                decAlg.IV = iv;
                MemoryStream decryptionStreamBacking = new MemoryStream();
                CryptoStream decrypt = new CryptoStream(decryptionStreamBacking, decAlg.CreateDecryptor(), CryptoStreamMode.Write);

                decrypt.Write(eJSON, 0, eJSON.Length);
                decrypt.Flush();
                decrypt.Close();
                k2.Reset();
                string data2 = new UTF8Encoding(false).GetString(decryptionStreamBacking.ToArray());

                return await Deserialize<T>(data2);
            }
            catch (Exception e)
            {
                Debug.Log($"Error: {e}");
                return await Deserialize<T>("");
            }
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