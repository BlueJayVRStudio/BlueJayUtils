using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluejayvrstudio
{
    public class CustomM : MonoBehaviour
    {
        public static Vector3 XZPlane(Vector3 vec)
        {
            return new Vector3(vec.x,0,vec.z);
        }
        public static Vector3 XYPlane(Vector3 vec)
        {
            return new Vector3(vec.x,vec.y,0);
        }
        public static Vector3 YZPlane(Vector3 vec)
        {
            return new Vector3(0,vec.y,vec.z);
        }
        
        public static Vector3 XZPlane(Vector3 vec, float replace)
        {
            return new Vector3(vec.x,replace,vec.z);
        }
        public static Vector3 XYPlane(Vector3 vec, float replace)
        {
            return new Vector3(vec.x,vec.y,replace);
        }
        public static Vector3 YZPlane(Vector3 vec, float replace)
        {
            return new Vector3(replace,vec.y,vec.z);
        }
        public static Vector3 ScaleBy(Vector3 vec, float x, float y, float z)
        {
            return new Vector3(vec.x * x, vec.y * y, vec.z * z);
        }
        public static Vector3 TranslateBy(Vector3 vec, float x, float y, float z)
        {
            return new Vector3(vec.x + x, vec.y + y, vec.z + z);
        }
    }

    public class TimeFormatter
    {
        public static string TimeFormat(float _Time)
        {
            int Seconds = ((int)_Time)%60;
            int Minutes = (int)(_Time/60.0f);

            if (Minutes == 0) 
            {
                if (Seconds == 0) return "00:00";
                else if (Seconds < 10) return "00" + ":0" + Seconds.ToString();
                else return "00" + ":" + Seconds.ToString();
            }
            else if (Minutes < 10) 
            {
                if (Seconds == 0) return "0" + Minutes.ToString() + ":00";
                else if (Seconds < 10) return "0" + Minutes.ToString() + ":0" + Seconds.ToString();
                else return "0" + Minutes.ToString() + ":" + Seconds.ToString();
            }
            else
            {
                if (Seconds == 0) return Minutes.ToString() + ":00";
                else if (Seconds < 10) return Minutes.ToString() + ":0" + Seconds.ToString();
                else return Minutes.ToString() + ":" + Seconds.ToString();
            }
        }
    }
}