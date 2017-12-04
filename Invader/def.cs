using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invader
{
    static class Def
    {
        // mainly, for avoid frindly-fire
        public enum ObjectID { PlayerID, EnemyID, ObjectID };
        public enum State {Title, EnemyInit, PlayerInit, InGame, BeShotDown, GameOver, Init, End};
        // difine attacker size
        public static readonly double[,] enemyRange =  new double[3, 2] {{24.0, 16.0 },{22.0, 16.0},{16.0, 16.0} };
        public static readonly double[] playerRange = new double[2] { 32.0, 16.0 };
        public static readonly double[] bulletRange = new double[2] { 4.0, 16.0 };
        // initial row position for each attackers that belong to template squad 
        public static readonly double[] startRowHight = new double[9] {100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 100.0};

        public const double bulletSpeed = 1.0;
        public const double enemyShotInitialHeight = 1.0, playerShotInitialHeight = -1.0;
        // invading enemy use this values
        public const double enemyColInterval = 10.0, enemyRowInterval = 10.0, enemyInvade = 3.0;
        public const int leastInvadeSpeedCount = 20, InvadeSpeedChange = 5, maxInvadeSpeedCount = 60;
        // player is allowed to move this row line
        public const double playerLine = 460.0;
        public const double playerSpeed = 1.0;

        public const double existHeight = 480.0, existWidth = 360.0;
        public const double invadingHeight = 400.0, invadingWidth = 360.0;

        public const int frameSpan = 1000 / 60;

        public const int wallTeamNum = 4, wallHP = 4;
        public const double wallSize = 15.0;
        public static readonly Vector2 wallInit = new Vector2(20.0, 380.0);
        public static readonly Vector2 wallInterval = new Vector2(60.0, 0.0);
        public static readonly List<Vector2> wallTeam = new List<Vector2>(5) { new Vector2(0.0, 0.0),  new Vector2(0.0, wallSize), new Vector2(wallSize, wallSize), new Vector2(wallSize * 2.0, wallSize), new Vector2(wallSize * 2.0, 0.0) };

        public const int shotdownTime = 1000 * 1;
        public const int playerAnimateCycle = 0, playerDyingCycle = 1000;
        public const int enemyAnimateCycle = 30, enemyDyingCycle = 10;
        //public const int enemyAnimateCycle = 30, enemyDyingCycle = 10;
    }


}
