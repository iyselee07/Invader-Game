using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invader
{
    class Wall:GameObject
    {
        int hitPoint;
        public Wall(Vector2 pos, double width, double height, int HP)
        {
            position =  pos;
            Vector2 end = new Vector2(pos.x + width, pos.y + height);
            hitBox = new Area(pos, end);
            hitPoint = HP;
            teamID = (int)Def.ObjectID.ObjectID;
            exist.add(this);
        }

        public override void beDamaged()
        {
            hitPoint--;
            if (hitPoint > 0)
            {
                return;
            }
            exist.remove(this);
        }
    }
}
