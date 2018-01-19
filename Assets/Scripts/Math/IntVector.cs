using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{
    [System.Serializable]
    public struct IntVector3
    {
        public int x;
        public int y;
        public int z;

        public static IntVector3 Invalid = new IntVector3(int.MaxValue, int.MaxValue, int.MaxValue);
		public static IntVector3 Zero = new IntVector3 (0, 0, 0);
        public IntVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public IntVector3(Vector3 vec)
        {
            this.x = (int)vec.x;
            this.y = (int)vec.y;
            this.z = (int)vec.z;
        }
        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
        public override bool Equals(object other)
        {
            if (!(other is IntVector3))
            {
                return false;
            }
            IntVector3 vector = (IntVector3)other;
            return this.x.Equals(vector.x) && this.y.Equals(vector.y) && this.z.Equals(vector.z);
        }
        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
        }
        public static IntVector3 operator +(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static IntVector3 operator -(IntVector3 a)
        {
            return new IntVector3(-a.x, -a.y, -a.z);
        }
        public static IntVector3 operator -(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static IntVector3 operator *(int d, IntVector3 a)
        {
            return new IntVector3(a.x * d, a.y * d, a.z * d);
        }
        public static IntVector3 operator *(IntVector3 a, int d)
        {
            return new IntVector3(a.x * d, a.y * d, a.z * d);
        }
        public static IntVector3 operator /(IntVector3 a, float d)
        {
            return new IntVector3((int)(a.x / d), (int)(a.y / d), (int)(a.z / d));
        }
        public static bool operator ==(IntVector3 lhs, IntVector3 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }
        public static bool operator !=(IntVector3 lhs, IntVector3 rhs)
        {
            return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
        }
    }
}