using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
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
        public static readonly double[] startRowHight = new double[9] {120.0, 132.0, 144.0, 156.0, 168.0, 180.0, 196.0, 208.0, 220.0};
        public const int enemyPlatoonRow = 5, enemyPlatoonCol = 11;

        public const double playerBulletSpeed = 10.0, enemyBulletSpeed = 6.0;
        public const double enemyShotInitialHeight = 16.0, playerShotInitialHeight = -16.0;
        // invading enemy use this values
        public const double enemyInvadingColInterval = 16.0, enemyInitColInterval = 24.0, enemyInitRowInterval = 32.0, enemyInvade = 3.0;
        public const int leastInvadeSpeedCount = 5, InvadeSpeedChange = 5, maxInvadeSpeedCount = 20;

        public const int leastInvadeAttackCount = 20, maxInvadeAttackCount = 50;
        public const double aimingThreshold = 0.1;
        // player is allowed to move this row line
        public const double playerLine = existHeight - 20.0;
        public const double playerSpeed = 2.0;


        public readonly static Vector2
            playerRemainInitPos = new Vector2(20.0, existHeight + 20.0),
            playerRemainInterval = new Vector2(playerRange[0]* 1.5, 0.0);

        // values used for when player destroyed
        public static readonly int[] enemyScores = new int[3] { 10, 20, 30 };

        // defintion play area
        public const double existHeight = 400.0, existWidth = 450.0;
        public const double invadingHeight = existHeight - 30.0, invadingWidth = existWidth;

        //public const int frameSpan = 1000 / 60;
        public const int frameSpan = 1000 / 60;

        public const int wallTeamNum = 4, wallHP = 3, wallRowNum = 5;
        public const double wallSize = 15.0, wallTeamWidth = wallSize * wallRowNum, wallIntervalX = 100.0;
        public static readonly Vector2 wallInterval = new Vector2(wallIntervalX, 0.0);
        public static readonly Vector2 wallInit = new Vector2((existWidth - ((wallTeamNum - 1) * wallIntervalX + wallTeamWidth))/ 2.0, playerLine - wallSize * 3.0);
        public static readonly List<Vector2> wallTeam = new List<Vector2>() {
            new Vector2(0.0, 0.0),  new Vector2(0.0, wallSize), new Vector2(0.0, wallSize * 2.0),
            new Vector2(wallSize, -wallSize), new Vector2(wallSize, 0.0), new Vector2(wallSize, wallSize), //new Vector2(wallSize, wallSize * 2.0),
            new Vector2(wallSize * 2.0, -wallSize), new Vector2(wallSize * 2.0, 0.0), new Vector2(wallSize * 2.0, wallSize),
            /*new Vector2(wallSize * 3.0, -wallSize), */new Vector2(wallSize * 3.0, 0.0), new Vector2(wallSize * 3.0, wallSize), new Vector2(wallSize * 3.0, wallSize * 2.0),
            //new Vector2(wallSize * 4.0, 0.0),  new Vector2(wallSize * 4.0, wallSize), new Vector2(wallSize * 4.0, wallSize * 2.0),
        };

        public const int enemyMagazineSize = 3, playerMagazineSize = 1;

        public const int PlayerShotdownTime = 70;
        public const int shotdownTime = 70;
        public const int playerAnimateCycle = 0, playerDyingCycle = 2;
        public const int enemyAnimateCycle = 30, enemyDyingCycle = 10;
        public const int bulletAnimateCycle = 0, bulletDyingCycle = 10;

        public readonly static BitmapImage bImgE1_1 = new BitmapImage(new Uri("ms-appx:///Image/E1-1.png"));
        public readonly static BitmapImage bImgE1_2 = new BitmapImage(new Uri("ms-appx:///Image/E1-2.png"));
        public readonly static BitmapImage bImgE2_1 = new BitmapImage(new Uri("ms-appx:///Image/E2-1.png"));
        public readonly static BitmapImage bImgE2_2 = new BitmapImage(new Uri("ms-appx:///Image/E2-2.png"));
        public readonly static BitmapImage bImgE3_1 = new BitmapImage(new Uri("ms-appx:///Image/E3-1.png"));
        public readonly static BitmapImage bImgE3_2 = new BitmapImage(new Uri("ms-appx:///Image/E3-2.png"));
        public readonly static BitmapImage bImgP1 = new BitmapImage(new Uri("ms-appx:///Image/P-1.png"));
        public readonly static BitmapImage bImgW1 = new BitmapImage(new Uri("ms-appx:///Image/W1.png"));
        //public readonly static BitmapImage bImgW2 = new BitmapImage(new Uri("ms-appx:///Image/W2.png"));
        public readonly static BitmapImage bImgW3 = new BitmapImage(new Uri("ms-appx:///Image/W3.png"));
        public readonly static BitmapImage bImgW4 = new BitmapImage(new Uri("ms-appx:///Image/W4.png"));
        public readonly static BitmapImage bImgB = new BitmapImage(new Uri("ms-appx:///Image/B.png"));
        public readonly static BitmapImage bImgESD = new BitmapImage(new Uri("ms-appx:///Image/ESD.png"));
        public readonly static BitmapImage bImgPSD1 = new BitmapImage(new Uri("ms-appx:///Image/P-2.png"));
        public readonly static BitmapImage bImgPSD2 = new BitmapImage(new Uri("ms-appx:///Image/P-3.png"));
        public readonly static BitmapImage bImgBSD = new BitmapImage(new Uri("ms-appx:///Image/BSD.png"));


    }


}
