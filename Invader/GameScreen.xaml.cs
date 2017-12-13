using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using System.Threading.Tasks;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Invader
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class GameScreen : Page
    {
        private Object thislock = new Object();
        private Stage stage;
        private Timer timer;
        private Existence exist;
        private DyingExistence dying;
        //private Dictionary<GameObject, GameObjectImage> display;
        private ConcurrentDictionary<GameObject, GameObjectImage> display;
        private int playerDeathCount, playerRemainNum;
        Image playerDeathImage;
        List<Image> playerRemainingImages;
        public GameScreen()
        {
            this.InitializeComponent();
            exist = Existence.getInstance();
            dying = DyingExistence.getInstance();
            stage = Stage.getInstance();
            //display = new Dictionary<GameObject, GameObjectImage>();
            display = new ConcurrentDictionary<GameObject, GameObjectImage>();
            playerDeathCount = 0;
            playerDeathImage = new Image();
            playerRemainingImages = new List<Image>();
            playerRemainNum = stage.remainingPlayerAttacker;
            //stage.clock += Stage_clock;
            TimerCallback callback = state => { Stage_clock(); };
            timer = new Timer(callback, null, 0, Def.frameSpan);

            this.Loaded += delegate {
                this.keyholder.Focus(FocusState.Keyboard);
                this.keyholder.LostFocus += (s, e) => this.keyholder.Focus(FocusState.Keyboard);
                this.keyholder.IsTabStop = true;
                //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryResizeView(new Size { Width = Def.existWidth, Height = Def.existHeight });
            };
        }

        //private async void Stage_clock(object sender, EventArgs e)
        private async void Stage_clock()
        {
            switch (stage.state)
            {
                case Def.State.EnemyInit:
                    clearDisplay();
                    break;
                case Def.State.GameOver:
                    clearDisplay();
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                    {
                        timer.Change(Timeout.Infinite, Timeout.Infinite);
                        timer.Dispose();
                        this.keyholder.LostFocus += (s, e) => { };
                        this.Frame.Navigate(typeof(GameOver));
                    });
                    break;
                case Def.State.BeShotDown:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                    {
                        if (playerDeathCount == 0)
                        {
                            canvas.Children.Add(playerDeathImage);
                            playerDeathImage.Width = Def.playerRange[0];
                            playerDeathImage.Height = Def.playerRange[1];
                            Canvas.SetLeft(playerDeathImage, stage.playerPosition.x);
                            Canvas.SetTop(playerDeathImage, stage.playerPosition.y);
                        }
                        if (playerDeathCount % Def.playerDyingCycle == 0)
                        {
                            playerDeathImage.Source = (playerDeathCount / Def.playerDyingCycle % 2 == 0) ? Def.bImgPSD1 : Def.bImgPSD2;
                        }
                        playerDeathCount++;
                        
                    });
                    break;
                case Def.State.InGame:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                    {

                        if (playerDeathCount != 0)
                        {
                            playerDeathCount = 0;
                        }
                        if (canvas.Children.Contains(playerDeathImage))
                        {
                            canvas.Children.Remove(playerDeathImage);
                        }
                        if (playerRemainNum != stage.remainingPlayerAttacker - 1)
                        {
                            playerRemainNum = stage.remainingPlayerAttacker - 1;
                            Vector2 pRemPos = Def.playerRemainInitPos.copy();
                            foreach(Image image in playerRemainingImages)
                            {
                                if (canvas.Children.Contains(image))
                                {
                                    canvas.Children.Remove(image);
                                }
                            }
                            playerRemainingImages.Clear();
                            for (int i = 0; i < playerRemainNum; i++)
                            {
                                Image image = new Image();
                                canvas.Children.Add(image);
                                playerRemainingImages.Add(image);
                                image.Source = Def.bImgP1;
                                image.Width = Def.playerRange[0];
                                image.Height = Def.playerRange[1];
                                Canvas.SetLeft(image, pRemPos.x);
                                Canvas.SetTop(image, pRemPos.y);
                                pRemPos += Def.playerRemainInterval;
                            }
                        }
                    });
                    await displayImage();
                    break;
            }
        }

        //private Task displayImage()
        //{
        //    List<Task> tasks = new List<Task>();
        //    foreach (GameObject gmObj in exist.iterate())
        //    {
        //    GameObjectImage gmObjImg;
        //        if (display.TryGetValue(gmObj, out gmObjImg))
        //        {
        //            gmObjImg = display[gmObj];
        //            Task task = Task.Run(async () =>
        //            {
        //                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
        //                {
        //                    gmObjImg.animate();
        //                });
        //            });
        //            tasks.Add(task);
        //        }
        //        else 
        //        {
        //            Task task = Task.Run(async () =>
        //            {

        //                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
        //                {
        //                    Image img;
        //                    img = new Image();
        //                    canvas.Children.Add(img);
        //                    gmObjImg = new GameObjectImage(canvas, gmObj, img);
        //                    bool isSafe = display.TryAdd(gmObj, gmObjImg);
        //                    if (false)
        //                    {
        //                        throw new Exception();
        //                    }


        //                    //display[gmObj] = gmObjImg;
        //                });
        //            });
        //            tasks.Add(task);
        //        }
        //    }
        //    foreach (GameObject gmObj in dying.iterate())
        //    {

        //        GameObjectImage gmObjImg = display[gmObj];

        //        Task task = Task.Run(async () =>
        //        {
        //            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
        //            {
        //                gmObjImg.dying();
        //            });
        //        });
        //        tasks.Add(task);
                
        //    }
        //    return Task.WhenAll(tasks);
        //}

        private Task displayImage()
        {
            Task task = Task.Run(async () =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    this.keyholder.Focus(FocusState.Keyboard);
                    //Existcount.Text = Existence.getInstance().count.ToString();
                    //Dyingcount.Text = DyingExistence.getInstance().count.ToString();
                    //player.Text = display.Count.ToString();
                    scoreNum.Text = stage.score.ToString();

                    foreach (GameObject gmObj in exist.iterate())
                    {
                        GameObjectImage gmObjImg;
                        if (display.TryGetValue(gmObj, out gmObjImg))
                        {
                            gmObjImg.animate();
                        }
                        else
                        {
                            Image img;
                            img = new Image();
                            canvas.Children.Add(img);
                            gmObjImg = new GameObjectImage(canvas, gmObj, img);
                            gmObjImg.canDispose += GmObjImg_canDispose;
                            bool isSafe = display.TryAdd(gmObj, gmObjImg);
                            //display.Add(gmObj, gmObjImg);
                            if (false)
                            {
                                throw new Exception();
                            }
                        }
                    }
                    foreach (GameObject gmObj in dying.iterate())
                    {
                        GameObjectImage gmObjImg;
                        if (display.TryGetValue(gmObj, out gmObjImg))
                        {
                            gmObjImg.dying();
                        }
                    }
                });
            });
            task.Wait();
            return task;
        }

        private void clearDisplay()
        {
            Task task = Task.Run(async () =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    foreach (KeyValuePair<GameObject, GameObjectImage> pair in display)
                    {
                        eraseImage(pair.Value);
                    }
                    display.Clear();

                });
            });
        }

        private void eraseImage(GameObjectImage gmObjImg)
        {
            if (canvas.Children.Contains(gmObjImg.displayImage))
            {
                canvas.Children.Remove(gmObjImg.displayImage);
            }
        }

        private void GmObjImg_canDispose(object sender, EventArgs e)
        {
            GameObjectImage gmObjImg = sender as GameObjectImage, tmp;
            while(!display.TryRemove(gmObjImg.gmObj, out tmp));
            //display.Remove(gmObjImg.gmObj);
        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
                stage.interactByKeyPress(sender, e);
        }

        private void Grid_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            stage.interactByKeyRelease(sender, e);
        }
    }

    class GameObjectImage
    {
        public Image displayImage { private set; get; }
        private int animateCycle, animateCount;
        private int dyingCycle, dyingCount;
        private int imageState;
        private bool isDyingLoop;
        private bool initialdeath;
        private List<BitmapImage> changeImages;
        private List<BitmapImage> dyingAnimation;
        private Canvas canvas;
        public GameObject gmObj { private set; get; }

        public event EventHandler canDispose;

        public GameObjectImage(Canvas can, GameObject gmObj, Image image)
        {
            displayImage = image;
            canvas = can;
            animateCount = 0;
            dyingCount = 0;
            initialdeath = true;
            imageState = 0;
            //canvas.Children.Add(displayImage);
            changeImages = new List<BitmapImage>();
            dyingAnimation = new List<BitmapImage>();
            this.gmObj = gmObj;
            Canvas.SetLeft(displayImage, gmObj.position.x);
            Canvas.SetTop(displayImage, gmObj.position.y);
            constructByType(gmObj);
        }


        private void constructByType(GameObject gmObj)
        {
            if (gmObj is PlayerAttacker)
            {
                animateCycle = Def.playerAnimateCycle;
                dyingCycle = Def.playerDyingCycle;
                // isDyingLoop = true;
                isDyingLoop = false;
                changeImages.Add(Def.bImgP1);
                //dyingAnimation.Add(Def.bImgESD);
                displayImage.Source = changeImages[0];
                displayImage.Width = Def.playerRange[0];
                displayImage.Height = Def.playerRange[1];
            }
            else if (gmObj is EnemyAttacker)
            {
                EnemyAttacker eAttacker = gmObj as EnemyAttacker;
                int eType = eAttacker.enemyType;
                animateCycle = Def.enemyAnimateCycle;
                dyingCycle = Def.enemyDyingCycle;
                isDyingLoop = false;
                switch (eType)
                {
                    case 0:
                        changeImages.Add(Def.bImgE1_1);
                        changeImages.Add(Def.bImgE1_2);
                        break;
                    case 1:
                        changeImages.Add(Def.bImgE2_1);
                        changeImages.Add(Def.bImgE2_2);
                        break;
                    case 2:
                        changeImages.Add(Def.bImgE3_1);
                        changeImages.Add(Def.bImgE3_2);
                        break;
                }
                dyingAnimation.Add(Def.bImgESD);
                displayImage.Source = changeImages[0];
                displayImage.Width = Def.enemyRange[eType, 0];
                displayImage.Height = Def.enemyRange[eType, 1];
            }
            else if (gmObj is Bullet)
            {

                animateCycle = Def.bulletAnimateCycle;
                dyingCycle = Def.bulletDyingCycle;
                isDyingLoop = false;
                changeImages.Add(Def.bImgB);
                dyingAnimation.Add(Def.bImgBSD);
                displayImage.Source = changeImages[0];
                displayImage.Width = Def.bulletRange[0];
                displayImage.Height = Def.bulletRange[1];

            }
            else if (gmObj is Wall)
            {
                animateCycle = 0;
                dyingCycle = 0;
                isDyingLoop = false;
                changeImages.Add(Def.bImgW1);
                //changeImages.Add(Def.bImgW2);
                changeImages.Add(Def.bImgW3);
                changeImages.Add(Def.bImgW4);
                displayImage.Source = changeImages.First<BitmapImage>();
                displayImage.Width = Def.wallSize;
                displayImage.Height = Def.wallSize;
            }
        }

        public void animate()
        {
            if (gmObj is Wall)
            {
                Wall wall = gmObj as Wall;
                imageState = Def.wallHP - wall.hitPoint;
                displayImage.Source = changeImages[imageState];
                Canvas.SetLeft(displayImage, gmObj.position.x);
                Canvas.SetTop(displayImage, gmObj.position.y);
                return;
            }

            if (animateCount >= animateCycle)
            {
                imageState++;
                imageState %= changeImages.Count;
                displayImage.Source = changeImages[imageState];
                animateCount = 0;
            }
            else
            {
                animateCount++;
            }
            Canvas.SetLeft(displayImage, gmObj.position.x);
            Canvas.SetTop(displayImage, gmObj.position.y);
        }

        public void dying()
        {
            if (dyingAnimation.Count == 0)
            {
                canvas.Children.Remove(displayImage);
                DyingExistence.getInstance().remove(gmObj);
                return;
            }
            if (initialdeath)
            {
                imageState = 0;
                dyingCount = 0;
                initialdeath = false;
                displayImage.Source = dyingAnimation[imageState];
            }
            if (dyingCount >= dyingCycle)
            {
                imageState++;
                if (!isDyingLoop && imageState >= dyingAnimation.Count)
                {
                    canvas.Children.Remove(displayImage);
                    DyingExistence.getInstance().remove(gmObj);
                    if (canDispose != null)
                    {
                        canDispose(this, EventArgs.Empty);
                    }
                    return;
                }
                imageState++;
                dyingCount = 0;
                displayImage.Source = dyingAnimation[imageState];
            }
            else
            {
                dyingCount++;
            }
        }

       
    }
}
