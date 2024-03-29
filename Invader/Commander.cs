﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invader
{
    class PlayerCommander
    {
        private Platoon platoon;
        private PlayerAttacker pAttacker;
        public int remaining { private set; get; }
        public Vector2 pAttackerCenter { get { return (pAttacker.hitBox.start + pAttacker.hitBox.end) / 2.0; } }
        public Vector2 pAttackerPosition { get { return pAttacker.hitBox.start; } }
        private double movingHight, movingWidth, movingSpeed;
        private Vector2 initialPosition;
        
        public event EventHandler won;
        public event EventHandler lost;

        public PlayerCommander()
        {
            remaining = 3;
            movingHight = Def.playerLine;
            movingWidth = Def.existWidth;
            movingSpeed = Def.playerSpeed;
            platoon = new Platoon(0, 1);
            initialPosition = new Vector2(0.0, movingHight);
            platoon.Annihilated += Platoon_Annihilated;
        }

        public void initializePlayerValues()
        {
            remaining = 3;
            initialPosition = new Vector2(0.0, movingHight);
        }

        public void organizeAttacker()
        {
            if (platoon.count() != 0)
            {
                pAttacker.respawnAttacker();
                return;
            }
            Squad squad = new Squad(0, 1);
            //PlayerAttacker attacker = new PlayerAttacker(initialPosition, 0);
            pAttacker = new PlayerAttacker(initialPosition, 0);
            squad.assignAttacker(pAttacker);
            platoon.assignSquad(squad);

        }

        public void moveLeft()
        {
            if (platoon.platoonBox.start.x >= 0.0)
            {
                platoon.moveLeftAll(movingSpeed);
            }
            //platoon.moveLeftAll(movingSpeed);
            //if (platoon.platoonBox.start.x < 0.0)
            //{
            //    platoon.moveRightAll(movingSpeed);
            //}
        }

        public void moveRight()
        {
            
            if (platoon.platoonBox.end.x <= movingWidth)
            {
                platoon.moveRightAll(movingSpeed);
            }
        }

        public void shot()
        {
            platoon.firstLeaderShot((int)Def.ObjectID.PlayerID);
        }


        private void Platoon_Annihilated(object sender, EventArgs e)
        {
            Platoon pla = sender as Platoon;
            if (remaining != 0)
            {
                remaining--;
                initialPosition = pla.platoonBox.start;
                //organizeAttacker();
                //return;
            }
            EventHandler handler = lost;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    class EnemyCommander
    {
        private Platoon platoon;
        private Area invadingArea;
        private Random attackingRand = new Random();
        private int col = Def.enemyPlatoonCol, row = Def.enemyPlatoonRow;
        private bool nowGoRight;

        private int moveCount, maxCountStep, slowestOffset;
        private int attackCount, attackTiming;
        private int minAttackCount = Def.leastInvadeAttackCount, maxInvadeAttackCount = Def.maxInvadeAttackCount;
        private double aimingThreshold = Def.aimingThreshold;

        public delegate void TeamDamageEventhandler(object sender, TeamDamageEventArgs e);

        public event TeamDamageEventhandler teamDamage;
        public event EventHandler lost;
        public event EventHandler won;

        public EnemyCommander(double maxrow, double maxcol, int stageNum)
        {
            Vector2
                start = new Vector2(0.0, 0.0),
                end = new Vector2(maxrow, maxcol);
            invadingArea = new Area(start, end);
            platoon = new Platoon(0, col);
            makeTemplateEnemy(stageNum);
            platoon.teamDamage += Platoon_teamDamage;
            platoon.Annihilated += Platoon_Annihilated;
            nowGoRight = true;
            moveCount = 0;
            attackCount = 0;
            attackTiming = attackingRand.Next(minAttackCount, maxInvadeAttackCount);
            maxCountStep = col * row / Def.InvadeSpeedChange;
            slowestOffset = (Def.maxInvadeSpeedCount - Def.leastInvadeSpeedCount) / maxCountStep;
        }

        public EnemyCommander(int stageNum)
        {
            Vector2
                start = new Vector2(0.0, 0.0),
                end = new Vector2(Def.invadingWidth, Def.invadingHeight);
            invadingArea = new Area(start, end);
            platoon = new Platoon(0, col);
            makeTemplateEnemy(stageNum);
            platoon.teamDamage += Platoon_teamDamage;
            platoon.Annihilated += Platoon_Annihilated;
            nowGoRight = true;
            moveCount = 0;
            attackCount = 0;
            maxCountStep = col * row / Def.InvadeSpeedChange;
            slowestOffset = (Def.maxInvadeSpeedCount - Def.leastInvadeSpeedCount) / maxCountStep;
        }

        private void makeTemplateEnemy(int sNum)
        {
            int startHightNum;
            if (sNum <= 1)
            {
                startHightNum = 0;
            }
            else
            {
                startHightNum = (sNum - 2) % 7 + 1;
            }
            double startHeight = Def.startRowHight[startHightNum];
            Vector2 squadHead = new Vector2(0.0, startHeight);
            for(int i = 0; i < col; i++)
            {
                Squad squad = new Squad(i, row);
                squad.makeEnemyTemplateSquad(squadHead);
                platoon.assignSquad(squad);
                squadHead.x += Def.enemyInitRowInterval;
            }
        }

        public void invade(Vector2 pos, params int[] friendly)
        {
            invadeMoving();
            invadeAttacking(pos, friendly);
        }

        private void invadeAttacking(Vector2 pos, params int[] friendly)
        {
            attackCount++;
            if (attackCount < attackTiming)
            {
                return;
            }
            if (attackingRand.NextDouble() < aimingThreshold)
            {
                platoon.nearestLeaderShot(pos, friendly);
            }
            else
            {
                platoon.randomizeLeaderShot(friendly);
            }
            attackTiming = attackingRand.Next(minAttackCount, maxInvadeAttackCount);
            attackCount = 0;
        }

        private void invadeMoving()
        {
            moveCount++;
            double step = Def.enemyInvade;
            int nowSpeedCount = Def.leastInvadeSpeedCount + slowestOffset * (platoon.count() / Def.InvadeSpeedChange);
            if (moveCount < nowSpeedCount)
            {
                return;
            }
            if (nowGoRight)
            {
                platoon.moveRightAll(step);
                if(!invadingArea.contain(platoon.platoonBox))
                {
                    nowGoRight = false;
                    platoon.moveLeftAll(step);
                    downInvading();
                }
            }
            else
            {
                platoon.moveLeftAll(step);
                if (!invadingArea.contain(platoon.platoonBox))
                {
                    nowGoRight = true;
                    platoon.moveRightAll(step);
                    downInvading();
                }
            }
            moveCount = 0;
        }

        private void downInvading()
        {
            platoon.moveDownAll(Def.enemyInvadingColInterval);
            if (!invadingArea.contain(platoon.platoonBox))
            {
                EventHandler handler = won;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        private void Platoon_teamDamage(object sender, TeamDamageEventArgs e)
        {
            TeamDamageEventhandler handler = teamDamage;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void Platoon_Annihilated(object sender, EventArgs e)
        {
            EventHandler handler = lost;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

    }

    class Platoon
    {
        private List<Squad> platoon;
        private int platoonID, platoonSize;

        public Area platoonBox { private set; get; }

        public delegate void TeamDamageEventhandler(object sender, TeamDamageEventArgs e);

        public event TeamDamageEventhandler teamDamage;
        public event EventHandler Annihilated;

        public Platoon(int pID, int pSize)
        {
            platoon = new List<Squad>();
            platoonBox = new Area();
            platoonID = pID;
            platoonSize = pSize;
        }

        public void assignSquad(Squad squad)
        {
            if (platoon.Count >= platoonSize)
            {
                return;
            }
            platoon.Add(squad);
            squad.teamDamage += Squad_teamDamage;
            squad.Annihilated += Squad_Annihilated;
            updatePlatoonBox();
        }

        public void dismissSquad()
        {
            foreach (Squad squad in platoon)
            {
                squad.teamDamage -= Squad_teamDamage;
                squad.Annihilated -= Squad_Annihilated;
            }
            platoon.Clear();
        }

        
        public void firstLeaderShot(params int [] friendly)
        {
            platoon[0].makeLeaderShot(friendly);
        }

        public void randomizeLeaderShot(params int[] friendly)
        {
            Random rand = new Random();
            int chosenNum = rand.Next(platoon.Count);
            platoon[chosenNum].makeLeaderShot(friendly);
        }

        public void randomizeRandomizeShot(params int[] friendly)
        {
            Random rand = new Random();
            int chosenNum = rand.Next(platoon.Count);
            platoon[chosenNum].randomizeShot(friendly);
        }

        public void nearestLeaderShot(Vector2 pos, params int[] friendly)
        {
            double nearX = double.MaxValue;
            Squad definedSquad = platoon[0];
            foreach (Squad item in platoon)
            {
                double diff = Math.Abs(pos.x - item.squadBox.start.x);
                if (nearX > diff)
                {
                    nearX = diff;
                    definedSquad = item;
                }
            }
            definedSquad.makeLeaderShot(friendly);
        }

        public void moveRightAll(double step)
        {
            Vector2 diff = new Vector2(step, 0.0);
            movePlatoonBox(diff);
            foreach (Squad item in platoon)
            {
                item.moveRightAll(step);
            }
        }

        public void moveLeftAll(double step)
        {
            Vector2 diff = new Vector2(-step, 0.0);
            movePlatoonBox(diff);
            foreach (Squad item in platoon)
            {
                item.moveLeftAll(step);
            }
        }

        public void moveUpAll(double step)
        {
            Vector2 diff = new Vector2(0.0, -step);
            movePlatoonBox(diff);
            foreach (Squad item in platoon)
            {
                item.moveUpAll(step);
            }
        }

        public void moveDownAll(double step)
        {
            Vector2 diff = new Vector2(0.0, step);
            movePlatoonBox(diff);
            foreach (Squad item in platoon)
            {
                item.moveDownAll(step);
            }
        }


        private void movePlatoonBox(Vector2 diff)
        {
            platoonBox.start += diff;
            platoonBox.end += diff;
        }

        private void updatePlatoonBox()
        {

            platoonBox.start.x = double.MaxValue;
            platoonBox.start.y = double.MaxValue;
            platoonBox.end.x = double.MinValue;
            platoonBox.end.y = double.MinValue;
            foreach (Squad item in platoon)
            {
                Vector2 sPos = item.squadBox.start, ePos = item.squadBox.end;
                platoonBox.start.x = Math.Min(platoonBox.start.x, sPos.x);
                platoonBox.start.y = Math.Min(platoonBox.start.y, sPos.y);
                platoonBox.end.x = Math.Max(platoonBox.end.x, ePos.x);
                platoonBox.end.y = Math.Max(platoonBox.end.y, ePos.y);
            }
        }

        public int count()
        {
            int sum = 0;
            foreach(Squad line in platoon)
            {
                sum += line.count();
            }
            return sum;
        }

        private void Squad_teamDamage(object sender, TeamDamageEventArgs e)
        {
            TeamDamageEventhandler handler = teamDamage;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void Squad_Annihilated(object sender, EventArgs e)
        {
            if (sender is Squad)
            {
                platoon.Remove(sender as Squad);
            }

            if (platoon.Count == 0)
            {
                EventHandler handler = Annihilated;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
            else
            {
                updatePlatoonBox();
            }
        }
    }

    class Squad
    {
        private List<Attacker> squad;
        private Attacker squadLeader;
        private Vector2 formationVec;
        private double formationWidth;
        private int squadID, squadSize;

        public Area squadBox { private set; get; }

        public delegate void TeamDamageEventhandler(object sender, TeamDamageEventArgs e);

        public event TeamDamageEventhandler teamDamage;
        public event EventHandler Annihilated;

        public Squad(int sID, int sSize)
        {
            squadID = sID;
            formationVec = new Vector2(0.0, -1.0);
            formationWidth = Def.enemyInitColInterval;
            squadSize = sSize;
            squadBox = new Area();
            squad = new List<Attacker>();
        }
        public Squad(int sID, int sSize, Vector2 fVec, double fWid)
        {
            squadID = sID;
            formationVec = fVec;
            formationWidth = fWid;
            squadSize = sSize;
            squadBox = new Area();
            squad = new List<Attacker>();
        }

        public void makeScramble(Attacker original)
        {
            Vector2 formation = formationWidth * formationVec;
            Vector2 pos = new Vector2(original.position);
            for (int i = 0; i < squadSize; i++)
            {
                Attacker tmpAttacker = original.copy();
                tmpAttacker.setInitialPosition(pos);
                tmpAttacker.shotDown += Attacker_shotDown;
                squad.Add(tmpAttacker);
                pos += formation;
            }
            electLeader();
            updateSquadBox();
        }

        public void makeEnemyTemplateSquad(Vector2 leaderPos)
        {
            Vector2 formation = formationWidth * formationVec;
            Vector2 pos = new Vector2(leaderPos);
            for (int i = 0; i < squadSize; i++)
            {
                int eType = (i < 2 ? 0 : (i < 4 ? 1 : 2));
                EnemyAttacker attacker = new EnemyAttacker(pos, eType, i);
                attacker.shotDown += Attacker_shotDown;
                squad.Add(attacker);
                pos += formation;
            }
            squadLeader = squad[0];
            alignByCenter();
            updateSquadBox();
        }

        public void assignAttacker(Attacker attacker)
        {
            if (squad.Count >= squadSize)
            {
                return;
            }
            squad.Add(attacker);
            attacker.shotDown += Attacker_shotDown;
            electLeader();
            updateSquadBox();
        }

        private void electLeader()
        {
            Attacker tmpAttacker = squad[0];
            double firstY = 0.0;
            foreach (Attacker item in squad)
            {
                if (firstY < item.position.y)
                {
                    tmpAttacker = item;
                    firstY = item.position.y;
                }
                squadLeader = tmpAttacker;
            }
        }

        private void alignByCenter()
        {
            Area leaderArea = squadLeader.hitBox;
            Vector2 leaderCenter = (leaderArea.start + leaderArea.end) / 2.0;
            foreach (Attacker item in squad)
            {
                if (item.Equals(squadLeader))
                {
                    continue;
                }
                Area itemArea = item.hitBox;
                Vector2 itemCenter = (itemArea.start + itemArea.end) / 2.0;
                double offset = itemCenter.x - itemArea.start.x;
                double centerX = leaderCenter.x + (formationVec.x / formationVec.y) * (itemCenter.y - leaderCenter.y);
                Vector2 alignedPos = new Vector2(centerX - offset, item.position.y);
                item.setInitialPosition(alignedPos);
            }
        }

        public void randomizeShot(params int[] friendly)
        {
            Random rand = new Random();
            int chosenNum = rand.Next(squad.Count);
            squad[chosenNum].shot(friendly);
        }

        public void makeLeaderShot(params int[] friendly)
        {
            squadLeader.shot(friendly);
        }

        public void moveRightAll(double step)
        {
            Vector2 diff = new Vector2(step, 0.0);
            moveSquadBox(diff);
            foreach (Attacker item in squad)
            {
                item.moveRight(step);
            }
        }

        public void moveLeftAll(double step)
        {
            Vector2 diff = new Vector2(-step, 0.0);
            moveSquadBox(diff);
            foreach (Attacker item in squad)
            {
                item.moveLeft(step);
            }
        }

        public void moveUpAll(double step)
        {
            Vector2 diff = new Vector2(0.0, -step);
            moveSquadBox(diff);
            foreach (Attacker item in squad)
            {
                item.moveUp(step);
            }
        }

        public void moveDownAll(double step)
        {
            Vector2 diff = new Vector2(0.0, step);
            moveSquadBox(diff);
            foreach (Attacker item in squad)
            {
                item.moveDown(step);
            }
        }

        private void moveSquadBox(Vector2 diff)
        {
            squadBox.start += diff;
            squadBox.end += diff;
        }

        private void updateSquadBox()
        {

            squadBox.start.x = double.MaxValue;
            squadBox.start.y = double.MaxValue;
            squadBox.end.x = double.MinValue;
            squadBox.end.y = double.MinValue;
            foreach (Attacker item in squad)
            {
                Vector2 sPos = item.hitBox.start, ePos = item.hitBox.end;
                squadBox.start.x = Math.Min(squadBox.start.x, sPos.x);
                squadBox.start.y = Math.Min(squadBox.start.y, sPos.y);
                squadBox.end.x = Math.Max(squadBox.end.x, ePos.x);
                squadBox.end.y = Math.Max(squadBox.end.y, ePos.y);
            }
        }

        public int count()
        {
            return squad.Count;
        }

        private void Attacker_shotDown(object sender, ShotDownEventArgs e)
        {
            if (sender is Attacker)
            {
                squad.Remove(sender as Attacker);
            }

            if (sender is EnemyAttacker)
            {
                EnemyAttacker eAttacker = sender as EnemyAttacker;
                TeamDamageEventhandler handler = teamDamage;
                if (handler != null)
                {
                    handler(this, new TeamDamageEventArgs(eAttacker.enemyType));
                }
            }

            if (squad.Count == 0)
            {
                EventHandler handler = Annihilated;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
            else
            {
                electLeader();
                updateSquadBox();
            }
        }
    }

    public class TeamDamageEventArgs : EventArgs
    {
        public int EnemyAttackerType { get; private set; }

        public TeamDamageEventArgs(int eAtcType)
        {
            EnemyAttackerType = eAtcType;
        }

    }
}
