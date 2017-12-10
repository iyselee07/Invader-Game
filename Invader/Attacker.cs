using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invader
{
    public class ShotDownEventArgs : EventArgs
    {
        public int AttackerID { get; private set; }
        public int TeamID { get; private set; }

        public ShotDownEventArgs(int aID, int tID)
        {
            AttackerID = aID;
            TeamID = tID;
        }
    }
    class Attacker : MovableObject
    {
        protected Vector2 shotAngle;
        protected int attackerID;
        protected int magazineSize;
        protected double bulletVelocity;
        protected double bulletInitHeight;
        protected List<Bullet> fireBullets;

        public delegate void ShotDownEventHandler(object sender, ShotDownEventArgs e);

        public event ShotDownEventHandler shotDown;


        public Attacker copy()
        {
            Attacker coAttacker = new Attacker();
            coAttacker.attackerID = this.attackerID;
            coAttacker.bulletVelocity = this.bulletVelocity;
            coAttacker.fireBullets = new List<Bullet>();
            coAttacker.magazineSize = this.magazineSize;
            coAttacker.nextPosition = this.nextPosition;
            coAttacker.hitBox = new Area(this.hitBox);
            coAttacker.position = this.position;
            coAttacker.shotAngle = new Vector2(this.shotAngle);
            coAttacker.teamID = this.teamID;
            coAttacker.bulletInitHeight = this.bulletInitHeight;
            return coAttacker;
        }


        public virtual void shot(params int[] friendly)
        {
            if (fireBullets.Count > magazineSize)
            {
                return;
            }
            Bullet bullet = new Bullet(teamID, shotAngle, bulletVelocity, friendly);
            Vector2 centerVec = (hitBox.start + hitBox.end) / 2.0;
            centerVec.x -= Def.bulletRange[0];
            centerVec.y += bulletInitHeight;
            bullet.setInitialPosition(centerVec);
            bullet.VanishedByHit += Bullet_VanishedByHit;
            exist.add(bullet);
            fireBullets.Add(bullet);
        }

        private void Bullet_VanishedByHit(object sender, EventArgs e)
        {
            if (sender is Bullet)
            {
                fireBullets.Remove(sender as Bullet);
            }
        }

        public override void beDamaged()
        {
            exist.remove(this);
            dying.add(this);
            ShotDownEventHandler handler = shotDown;
            if (handler != null)
            {
                handler(this, new ShotDownEventArgs(attackerID, teamID));
            }
        }
        public virtual void moveLeft(double step)
        {
            Vector2 next = new Vector2(position);
            next.x -= step;
            nextPosition = next;
        }
        public virtual void moveRight(double step)
        {
            Vector2 next = new Vector2(position);
            next.x += step;
            nextPosition = next;
        }
        public virtual void moveUp(double step)
        {
            Vector2 next = new Vector2(position);
            next.y -= step;
            nextPosition = next;
        }
        public virtual void moveDown(double step)
        {
            Vector2 next = new Vector2(position);
            next.y += step;
            nextPosition = next;
        }
    }

    class EnemyAttacker : Attacker
    {
        public int enemyType { protected set; get; }
        public EnemyAttacker(Vector2 pos, double width, double height, int aID)
        {
            position = nextPosition = pos;
            bulletInitHeight = Def.enemyShotInitialHeight;
            bulletVelocity = Def.bulletSpeed;
            fireBullets = new List<Bullet>();
            magazineSize = Def.enemyMagazineSize;
            Vector2 end = new Vector2(pos.x+width, pos.y+height);
            hitBox = new Area(pos, end);
            shotAngle = new Vector2(0.0, 1.0);
            attackerID = aID;
            teamID = (int)Def.ObjectID.EnemyID;
            exist.add(this);
        }

        public EnemyAttacker(Vector2 pos, int eType, int aID)
        {
            position = nextPosition = pos;
            enemyType = eType;
            bulletInitHeight = Def.enemyShotInitialHeight;
            bulletVelocity = Def.bulletSpeed;
            fireBullets = new List<Bullet>();
            magazineSize = Def.enemyMagazineSize;
            double width = Def.enemyRange[eType, 0], height = Def.enemyRange[eType, 1];
            Vector2 end = new Vector2(pos.x + width, pos.y + height);
            hitBox = new Area(pos, end);
            shotAngle = new Vector2(0.0, 1.0);
            attackerID = aID;
            teamID = (int)Def.ObjectID.EnemyID;
            exist.add(this);
        }
    }


    class PlayerAttacker : Attacker
    {
        public PlayerAttacker(Vector2 pos, double width, double height, int aID)
        {
            position = nextPosition = pos;
            bulletVelocity = Def.bulletSpeed;
            bulletInitHeight = Def.playerShotInitialHeight;
            fireBullets = new List<Bullet>();
            magazineSize = Def.playerMagazineSize;
            Vector2 end = new Vector2(pos.x + width, pos.y + height);
            hitBox = new Area(pos, end);
            shotAngle = new Vector2(0.0, -1.0);
            attackerID = aID;
            teamID = (int)Def.ObjectID.PlayerID;
            exist.add(this);
        }

        public PlayerAttacker(Vector2 pos, int aID)
        {
            position = nextPosition = pos;
            bulletVelocity = Def.bulletSpeed;
            bulletInitHeight = Def.playerShotInitialHeight;
            fireBullets = new List<Bullet>();
            double width = Def.playerRange[0], height = Def.playerRange[1];
            Vector2 end = new Vector2(pos.x + width, pos.y + height);
            hitBox = new Area(pos, end);
            shotAngle = new Vector2(0.0, -1.0);
            attackerID = aID;
            teamID = (int)Def.ObjectID.PlayerID;
            exist.add(this);
        }

        public override void moveDown(double step)
        {
            //do nothing
        }

        public override void moveUp(double step)
        {
            //do nothing
        }
    }

}
