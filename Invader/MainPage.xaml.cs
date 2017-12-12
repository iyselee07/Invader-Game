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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Threading;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 を参照してください

namespace Invader
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Stage stage;
        private Timer timer;
        public MainPage()
        {
            this.InitializeComponent();
            stage = Stage.getInstance();
            //stage.clock += Stage_clock;
            TimerCallback callback = state => { Stage_clock(); };
            timer = new Timer(callback, null, 0, Def.frameSpan);
            this.Loaded += delegate {
                this.keyholder.Focus(FocusState.Keyboard);
                this.keyholder.LostFocus += (s, e) => this.keyholder.Focus(FocusState.Keyboard);
                this.keyholder.IsTabStop = true;
                //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryResizeView(new Size { Width = Def.existWidth, Height = Def.existHeight });
                Image image = new Image();
                canvas.Children.Add(image);
                image.Width = 30;
                image.Height = 30;
                image.Source = Def.bImgW1;

                Canvas.SetLeft(image, 100.0);
                Canvas.SetTop(image, 100.0);
            };
            

        }

        //private async void Stage_clock(object sender, EventArgs e)
        private async void Stage_clock()
        {
            if (stage.state == Def.State.End)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    this.Frame.Navigate(typeof(End));
                });
            }
            else if (stage.state != Def.State.Title)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    timer.Dispose();
                    this.keyholder.LostFocus += (s, e) => { };
                    this.Frame.Navigate(typeof(GameScreen));
                    
                });
            }

        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (stage.state == Def.State.Title)
            {
                stage.interactByKeyPress(sender, e);
            }
        }

        private void Grid_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (stage.state == Def.State.Title)
            {
                stage.interactByKeyRelease(sender, e);
            }
        }
    }
}
