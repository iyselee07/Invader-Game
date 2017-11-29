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
        public static readonly double[,] enemyRange =  new double[3, 2] {{100.0, 200.0 },{100.0, 100.0},{300.0, 300.0} };
        public static readonly double[] playerRange = new double[2] { 100.0, 200.0 };
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
    }


}
