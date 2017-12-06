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
        public static readonly double[] startRowHight = new double[9] {400.0, 400.0, 400.0, 400.0, 400.0, 400.0, 400.0, 400.0, 400.0};

        public const double bulletSpeed = 1.0;
        public const double enemyShotInitialHeight = 1.0, playerShotInitialHeight = -1.0;
        // invading enemy use this values
        public const double enemyColInterval = 32.0, enemyRowInterval = 32.0, enemyInvade = 3.0;
        public const int leastInvadeSpeedCount = 20, InvadeSpeedChange = 5, maxInvadeSpeedCount = 20;
        // player is allowed to move this row line
        public const double playerLine = 460.0;
        public const double playerSpeed = 1.0;

        public const double existHeight = 960.0, existWidth = 720.0;
        public const double invadingHeight = 900.0, invadingWidth = 720.0;

        //public const int frameSpan = 1000 / 60;
        public const int frameSpan = 1000 / 60;

        public const int wallTeamNum = 4, wallHP = 4;
        public const double wallSize = 30.0;
        public static readonly Vector2 wallInit = new Vector2(20.0, 380.0);
        public static readonly Vector2 wallInterval = new Vector2(90.0, 0.0);
        public static readonly List<Vector2> wallTeam = new List<Vector2>(5) { new Vector2(0.0, 0.0),  new Vector2(0.0, wallSize), new Vector2(wallSize, wallSize), new Vector2(wallSize * 2.0, wallSize), new Vector2(wallSize * 2.0, 0.0) };

        public const int shotdownTime = 1000 * 1;
        public const int playerAnimateCycle = 0, playerDyingCycle = 1000;
        public const int enemyAnimateCycle = 30, enemyDyingCycle = 10;
        //public const int enemyAnimateCycle = 30, enemyDyingCycle = 10;

        public readonly static BitmapImage bImgE1_1 = new BitmapImage(new Uri("ms-appx:///Image/E1-1.png"));
        public readonly static BitmapImage bImgE1_2 = new BitmapImage(new Uri("ms-appx:///Image/E1-2.png"));
        public readonly static BitmapImage bImgE2_1 = new BitmapImage(new Uri("ms-appx:///Image/E2-1.png"));
        public readonly static BitmapImage bImgE2_2 = new BitmapImage(new Uri("ms-appx:///Image/E2-2.png"));
        public readonly static BitmapImage bImgE3_1 = new BitmapImage(new Uri("ms-appx:///Image/E3-1.png"));
        public readonly static BitmapImage bImgE3_2 = new BitmapImage(new Uri("ms-appx:///Image/E3-2.png"));
        public readonly static BitmapImage bImgP1 = new BitmapImage(new Uri("ms-appx:///Image/P-1.png"));
        public readonly static BitmapImage bImgW1 = new BitmapImage(new Uri("ms-appx:///Image/W1.png"));
        public readonly static BitmapImage bImgW2 = new BitmapImage(new Uri("ms-appx:///Image/W2.png"));
        public readonly static BitmapImage bImgW3 = new BitmapImage(new Uri("ms-appx:///Image/W3.png"));
        public readonly static BitmapImage bImgW4 = new BitmapImage(new Uri("ms-appx:///Image/W4.png"));
        public readonly static BitmapImage bImgB = new BitmapImage(new Uri("ms-appx:///Image/B.png"));
        public readonly static BitmapImage bImgESD = new BitmapImage(new Uri("ms-appx:///Image/ESD.png"));


    }


}
