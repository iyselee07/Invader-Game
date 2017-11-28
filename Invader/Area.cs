using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invader
{
    class Area
    {
        public Vector2 start { set; get; }
        public Vector2 end {  set; get; }

        public Area(double a, double b, double c, double d)
        {
            start = new Vector2(a, b);
            end = new Vector2(c, d);
            regularize();
        }

        public Area(Vector2 a, Vector2 b)
        {
            start = a.copy();
            end = b.copy();
            regularize();
        }

        public Area()
        {
            start = new Vector2();
            end = new Vector2();
        }

        public Area(Area a)
        {
            start = new Vector2(a.start);
            end = new Vector2(a.end);
        }

        private void regularize()
        {
            Vector2 regStart, regEnd;
            regStart = new Vector2();
            regEnd = new Vector2();
            if (start.x < end.x)
            {
                regStart.x = start.x;
                regEnd.x = end.x;
            }
            else
            {
                regStart.x = end.x;
                regEnd.x = start.x;
            }

            if (start.y < end.y)
            {
                regStart.y = start.y;
                regEnd.y = end.y;
            }
            else
            {
                regStart.y = end.y;
                regEnd.y = start.y;
            }
            start = regStart;
            end = regEnd;
        }

        public bool contain(Area target)
        {
            bool
                left = start.x <= target.start.x,
                right = end.x >= target.end.x,
                top = start.y <= target.start.y,
                bottom = end.y >= target.end.y;

            return left && right && top && bottom;
        }

        static public bool isOverrap(Area a, Area b)
        {
            bool inRangeX, inRangeY;
            inRangeX = !(a.end.x <= b.start.x || b.end.x <= a.start.x);
            inRangeY = !(a.end.y <= b.start.y || b.end.y <= a.start.y);
            return inRangeX && inRangeY;
        }
    }
}
