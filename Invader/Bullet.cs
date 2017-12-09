using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invader
{
    class Bullet : MovableObject
    {
        private List<int> whiteList;
        private Vector2 angle;
        private double velocity;
        private Area existArea;
        public event EventHandler VanishedByHit;

        public Bullet(int tID, Vector2 ang, double v, params int[] wlist)
        {
            position = new Vector2();
            double width = Def.bulletRange[0], height = Def.bulletRange[1];
            Vector2 end = new Vector2(position.x + width, position.y + height);
            hitBox = new Area(position, end);
            teamID = tID;
            angle = ang;
            velocity = v;
            whiteList = new List<int>();
            foreach (int items in wlist)
            {
                whiteList.Add(items);
            }
            existArea = new Area(new Vector2(), new Vector2(Def.existWidth, Def.existHeight));
        }

        public override void move()
        {
            double 
                nx = nextPosition.x + velocity * angle.x,
                ny = nextPosition.y + velocity * angle.y;
            Vector2 next = new Vector2(nx, ny);
            base.move();
            isHitToSomething();
            base.setNextPos(next);
            if (!existArea.contain(hitBox))
            {
                vanish();
            }
        }

        private void isHitToSomething()
        {
            foreach(GameObject item in exist.iterate())
            {
                if (item.Equals(this))
                {
                    continue;
                }

                bool isFriend = false;
                foreach(int id in whiteList)
                {
                    isFriend |= id == item.teamID;
                }

                if(isFriend)
                {
                    continue;
                }

                if (Area.isOverrap(this.hitBox, item.hitBox))
                {
                    item.beDamaged();
                    this.vanish();
                    break;
                }
            }
        }

        public void vanish()
        {
            exist.remove(this);
            dying.add(this);
            EventHandler handler = VanishedByHit;
            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public override void beDamaged()
        {
            vanish();
        }
    }
}
