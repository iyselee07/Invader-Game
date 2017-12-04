using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invader
{
    class Wall:GameObject
    {
        public int hitPoint { protected set; get; }
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
            dying.add(this);
        }

        public static void makeTemplateDefenceWall()
        {
            int wallNum = Def.wallTeamNum;
            double wallSize = Def.wallSize;
            Vector2 teamPos = Def.wallInit.copy();
            for (int i = 0; i < wallNum; i++)
            {
                foreach (Vector2 pos in Def.wallTeam)
                {
                    Vector2 wallPos = teamPos + pos;
                    new Wall(wallPos, wallSize, wallSize, Def.wallHP);
                }
                teamPos += Def.wallInterval; 
            }
        }
    }
}
