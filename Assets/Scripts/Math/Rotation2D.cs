using System.Collections;
using System.Collections.Generic;

namespace UnityEngine
{
    [System.Serializable]
    public struct Rotation2D
    {
        public float m00;
        public float m01;
        public float m10;
        public float m11;

        public static Rotation2D Identity = new Rotation2D { m00 = 1, m01 = 0, m10 = 0, m11 = 1 };
		public static Rotation2D East = new Rotation2D { m00 = 0, m01 = 1, m10 = -1, m11 = 0 };
		public static Rotation2D West = new Rotation2D { m00 = 0, m01 = -1, m10 = 1, m11 = 0 };
		public static Rotation2D North = new Rotation2D { m00 = 1, m01 = 0, m10 = 0, m11 = 1 };
		public static Rotation2D South = new Rotation2D { m00 = -1, m01 = 0, m10 = 0, m11 = -1 };

        public Rotation2D(float angle)
        {
            m00 = Mathf.Cos(angle);
            m01 = -Mathf.Sin(angle);
            m10 = -m01;
            m11 = m00;
        }

        public Quaternion Rotation3D
        {
            get
            {
                float cos = Mathf.Sqrt((m00 + 1) / 2);
                if (m10 < 0)
                    cos = -cos;
                float sin = Mathf.Abs(cos) < Mathf.Epsilon ? 1 : (m10 * 0.5f / cos);
                //from right-hand side to left-hand side
                return new Quaternion(0, -sin, 0, cos);
            }
        }

        public static Rotation2D operator *(Rotation2D a, Rotation2D b)
        {
            Rotation2D rot;
            rot.m00 = a.m00 * b.m00 + a.m01 * b.m10;
            rot.m01 = a.m00 * b.m01 + a.m01 * b.m11;
            rot.m10 = a.m10 * b.m00 + a.m11 * b.m10;
            rot.m11 = a.m10 * b.m01 + a.m11 * b.m11;
            return rot;
        }
        public static IntVector3 operator *(Rotation2D rot, IntVector3 pos)
        {
            IntVector3 newPos;
            newPos.x = (int)(rot.m00 * pos.x + rot.m01 * pos.z);
			newPos.y = pos.y;
            newPos.z = (int)(rot.m10 * pos.x + rot.m11 * pos.z);

            return newPos;
        }
    }
}
