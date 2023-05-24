using UnityEngine;

namespace _GAME
{
    public static class VectorExtension
    {
        public static Vector2 SetX(this Vector2 v, float a)
        {
            v.x = a;
            return v;
        }

        public static Vector2 SetY(this Vector2 v, float a)
        {
            v.y = a;
            return v;
        }

        public static Vector2 AddX(this Vector2 v, float a)
        {
            v.x += a;
            return v;
        }

        public static Vector2 AddY(this Vector2 v, float a)
        {
            v.y += a;
            return v;
        }

        public static Vector3 SetX(this Vector3 v, float a)
        {
            v.x = a;
            return v;
        }

        public static Vector3 SetY(this Vector3 v, float a)
        {
            v.y = a;
            return v;
        }

        public static Vector3 SetZ(this Vector3 v, float a)
        {
            v.z = a;
            return v;
        }

        public static Vector3 AddX(this Vector3 v, float a)
        {
            v.x += a;
            return v;
        }

        public static Vector3 AddY(this Vector3 v, float a)
        {
            v.y += a;
            return v;
        }

        public static Vector3 AddZ(this Vector3 v, float a)
        {
            v.z += a;
            return v;
        }

        public static Vector3 FromFloat(float value)
        {
            return new Vector3(value, value, value);
        }
    }
}