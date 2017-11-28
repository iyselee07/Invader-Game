using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invader
{
    class PlayerCommander
    {
        private Platoon platoon;
        public int remaining { private set; get; }
        private double movingHight, movingWidth, movingSpeed;
        private Vector2 initialPosition;

        public event EventHandler won;
        public event EventHandler losed;

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


        public void organizeAttacker()
        {
            if (platoon.count() != 0)
            {
                return;
            }
            Squad squad = new Squad(0, 1);
            PlayerAttacker attacker = new PlayerAttacker(initialPosition, 0);
            squad.assignAttacker(attacker);
            platoon.assignSquad(squad);

        }

        public void moveLeft()
        {
            platoon.moveLeftAll(movingSpeed);
            if (platoon.platoonBox.start.x < 0.0)
            {
                platoon.moveRightAll(movingSpeed);
            }
        }

        public void moveRight()
        {
            platoon.moveRightAll(movingSpeed);
            if (platoon.platoonBox.end.x > movingWidth)
            {
                platoon.moveLeftAll(movingSpeed);
            }
        }


        private void Platoon_Annihilated(object sender, EventArgs e)
        {
            Platoon pla = sender as Platoon;
            if (remaining != 0)
            {
                remaining--;
                initialPosition = pla.platoonBox.start;
                organizeAttacker();
                return;
            }
            EventHandler handler = losed;
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
        private int col = 11, row = 5;
        private bool nowGoRight;

        private int moveCount, maxCountStep, slowestOffset;

        public event EventHandler losed;
        public event EventHandler won;

        public EnemyCommander(double maxrow, double maxcol, int stageNum)
        {
            Vector2
                start = new Vector2(0.0, 0.0),
                end = new Vector2(maxrow, maxcol);
            invadingArea = new Area(start, end);
            platoon = new Platoon(0, col);
            makeTemplateEnemy(stageNum);
            platoon.Annihilated += Platoon_Annihilated;
            nowGoRight = true;
            moveCount = 0;
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
                squadHead.x += Def.enemyRowInterval;
            }
        }

        public void invade()
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
            platoon.moveDownAll(Def.enemyColInterval);
            if (!invadingArea.contain(platoon.platoonBox))
            {
                EventHandler handler = won;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        private void Platoon_Annihilated(object sender, EventArgs e)
        {
            EventHandler handler = losed;
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
            squad.Annihilated += Squad_Annihilated;
            updatePlatoonBox();
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
            foreach (Squad item in platoon)
            {
                item.moveRightAll(step);
            }
            Vector2 diff = new Vector2(step, 0.0);
            movePlatoonBox(diff);
        }

        public void moveLeftAll(double step)
        {
            foreach (Squad item in platoon)
            {
                item.moveLeftAll(step);
            }
            Vector2 diff = new Vector2(-step, 0.0);
            movePlatoonBox(diff);
        }

        public void moveUpAll(double step)
        {
            foreach (Squad item in platoon)
            {
                item.moveUpAll(step);
            }
            Vector2 diff = new Vector2(0.0, -step);
            movePlatoonBox(diff);
        }

        public void moveDownAll(double step)
        {
            foreach (Squad item in platoon)
            {
                item.moveDownAll(step);
            }
            Vector2 diff = new Vector2(0.0, step);
            movePlatoonBox(diff);
        }


        private void movePlatoonBox(Vector2 diff)
        {
            platoonBox.start += diff;
            platoonBox.end += diff;
        }

        private void updatePlatoonBox()
        {
            foreach (Squad item in platoon)
            {
                Vector2 sPos = item.squadBox.start, ePos = item.squadBox.end;
                platoonBox.start.x = Math.Min(platoonBox.start.x, sPos.x);
                platoonBox.start.y = Math.Min(platoonBox.start.y, sPos.y);
                platoonBox.end.x = Math.Max(platoonBox.end.x, ePos.x);
                platoonBox.end.x = Math.Max(platoonBox.end.y, ePos.y);
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

        public event EventHandler Annihilated;

        public Squad(int sID, int sSize)
        {
            squadID = sID;
            formationVec = new Vector2(0.0, -1.0);
            formationWidth = Def.enemyColInterval;
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
            foreach(Attacker item in squad)
            {
                item.moveRight(step);
            }
            Vector2 diff = new Vector2(step, 0.0);
            moveSquadBox(diff);
        }

        public void moveLeftAll(double step)
        {
            foreach (Attacker item in squad)
            {
                item.moveLeft(step);
            }
            Vector2 diff = new Vector2(-step, 0.0);
            moveSquadBox(diff);
        }

        public void moveUpAll(double step)
        {
            foreach (Attacker item in squad)
            {
                item.moveUp(step);
            }
            Vector2 diff = new Vector2(0.0, -step);
            moveSquadBox(diff);
        }

        public void moveDownAll(double step)
        {
            foreach (Attacker item in squad)
            {
                item.moveDown(step);
            }
            Vector2 diff = new Vector2(0.0, step);
            moveSquadBox(diff);
        }

        private void moveSquadBox(Vector2 diff)
        {
            squadBox.start += diff;
            squadBox.end += diff;
        }

        private void updateSquadBox()
        {
            foreach(Attacker item in squad)
            {
                Vector2 sPos = item.hitBox.start, ePos = item.hitBox.end;
                squadBox.start.x = Math.Min(squadBox.start.x, sPos.x);
                squadBox.start.y = Math.Min(squadBox.start.y, sPos.y);
                squadBox.end.x = Math.Max(squadBox.end.x, ePos.x);
                squadBox.end.x = Math.Max(squadBox.end.y, ePos.y);
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
}
