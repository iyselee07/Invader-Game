using System;
using System.Collections.Generic;
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
        private Existence exist;
        private DyingExistence dying;
        private Dictionary<GameObject, GameObjectImage> display;
        public GameScreen()
        {
            this.InitializeComponent();
            exist = Existence.getInstance();
            dying = DyingExistence.getInstance();
            stage = Stage.getInstance();
            display = new Dictionary<GameObject, GameObjectImage>();
            stage.clock += Stage_clock;
            this.Loaded += delegate {
                this.keyholder.Focus(FocusState.Keyboard);
                this.keyholder.LostFocus += (s, e) => this.keyholder.Focus(FocusState.Keyboard);
                this.keyholder.IsTabStop = true;
                //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryResizeView(new Size { Width = Def.existWidth, Height = Def.existHeight });
            };
        }

        private async void Stage_clock(object sender, EventArgs e)
        {
            switch (stage.state)
            {
                case Def.State.GameOver:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                    {
                        this.Frame.Navigate(typeof(GameOver));
                    });
                    break;
                case Def.State.BeShotDown:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                    {
                        this.Frame.Navigate(typeof(ShotDown));
                    });
                    break;
                case Def.State.InGame:
                    await displayImage();
                    break;
            }
        }

        private Task displayImage()
        {
            List<Task> tasks = new List<Task>();
            foreach (GameObject gmObj in exist.iterate())
            {
                GameObjectImage gmObjImg;
                lock (thislock)
                {
                    try
                    {
                        gmObjImg = display[gmObj];
                        Task task = Task.Run(async () =>
                        {
                            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                            {
                                gmObjImg.animate();
                            });
                        });
                        tasks.Add(task);
                    }
                    catch (KeyNotFoundException)
                    {
                        Task task = Task.Run(async () =>
                        {

                            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                            {
                                Image img;
                                img = new Image();
                                canvas.Children.Add(img);
                                gmObjImg = new GameObjectImage(canvas, gmObj, img);
                                display.Add(gmObj, gmObjImg);



                                display[gmObj] = gmObjImg;
                            });
                        });
                        tasks.Add(task);
                    }
                }

            }
            foreach (GameObject gmObj in dying.iterate())
            {
                lock (thislock)
                {
                    GameObjectImage gmObjImg = display[gmObj];

                    Task task = Task.Run(async () =>
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                        {
                            gmObjImg.dying();
                        });
                    });
                    tasks.Add(task);
                }
            }
            return Task.WhenAll(tasks);
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
        GameObject gmObj;

        public GameObjectImage(Canvas can, GameObject gmObj, Image image)
        {
            displayImage = image;
            canvas = can;
            animateCount = 0;
            dyingCount = 0;
            initialdeath = true;
            imageState = 0;
            canvas.Children.Add(displayImage);
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
                dyingAnimation.Add(Def.bImgESD);
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

                animateCycle = 0;
                dyingCycle = 0;
                isDyingLoop = false;
                changeImages.Add(Def.bImgB);
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
                changeImages.Add(Def.bImgW2);
                changeImages.Add(Def.bImgW3);
                changeImages.Add(Def.bImgW4);
                displayImage.Source = changeImages[0];
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
                initialdeath = false;
            }
            if (dyingCount >= dyingCycle)
            {
                imageState++;
                if (!isDyingLoop && imageState >= dyingAnimation.Count)
                {
                    canvas.Children.Remove(displayImage);
                    DyingExistence.getInstance().remove(gmObj);
                    return;
                }
                imageState %= dyingAnimation.Count;
                dyingCount = 0;
            }
            else
            {
                dyingCount++;
            }
        }
    }
}
