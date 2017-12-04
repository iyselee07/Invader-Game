using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Invader
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class GameScreen : Page
    {
        Stage stage;
        Existence exist;
        DyingExistence dying;
        public GameScreen()
        {
            this.InitializeComponent();
            exist = Existence.getInstance();
            dying = DyingExistence.getInstance();
            stage = Stage.getInstance();
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

                    break;
            }
        }

        private void syncObjToImg()
        {
            
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
        private int animateSpan, animateCount;
        private int dyingSpan, dyingCount;
        public bool isDyingLoop { private set; get; }
        private Canvas canvas;


        public GameObjectImage()
        {
            displayImage = new Image();
            displayImage.Source = new ImageSource();
        }
    }
}
