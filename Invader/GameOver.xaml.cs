using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Invader
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class GameOver : Page
    {
        private Stage stage;
        private Timer timer;
        public GameOver()
        {
            stage = Stage.getInstance();
            TimerCallback callback = state => { Stage_clock(); };
            timer = new Timer(callback, null, 0, Def.frameSpan);
            this.InitializeComponent();
            this.Loaded += delegate {
                this.keyholder.Focus(FocusState.Keyboard);
                this.keyholder.LostFocus += (s, e) => this.keyholder.Focus(FocusState.Keyboard);
                this.keyholder.IsTabStop = true;
            };
        }

        private async void Stage_clock()
        {
            if (stage.state == Def.State.End)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    this.Frame.Navigate(typeof(End));
                });
            }
            else if (stage.state != Def.State.GameOver)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    timer.Dispose();
                    this.keyholder.LostFocus += (s, e) => { };
                    this.Frame.Navigate(typeof(MainPage));

                });
            }
            else
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    this.keyholder.Focus(FocusState.Keyboard);
                });
            }
        }

        private void keyPressed(object sender, KeyRoutedEventArgs e)
        {
            if (stage.state == Def.State.GameOver)
            {
                stage.interactByKeyPress(sender, e);
            }
        }

        private void keyReleased(object sender, KeyRoutedEventArgs e)
        {
            if (stage.state == Def.State.GameOver)
            {
                stage.interactByKeyRelease(sender, e);
            }
        }
    }
}
