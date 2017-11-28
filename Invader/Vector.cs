using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invader
{
    class Vector2
    {
        public double x, y;

        public Vector2()
        {
            x = y = 0.0;
        }

        public Vector2(double inix, double iniy)
        {
            x = inix;
            y = iniy;
        }

        public Vector2(Vector2 vec)
        {
            x = vec.x;
            y = vec.y;
        }

        public Vector2 copy()
        {
            return new Vector2(x, y);
        }

        public double power()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator *(double d, Vector2 a)
        {
            return new Vector2(d * a.x, d * a.y);
        }
        public static Vector2 operator *(Vector2 a, double d)
        {
            return new Vector2(d * a.x, d * a.y);
        }

        public static Vector2 operator /(Vector2 a, double d)
        {
            return new Vector2(a.x / d, a.y / d);
        }

    }
}
