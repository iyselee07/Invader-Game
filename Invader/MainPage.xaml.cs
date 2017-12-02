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

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 を参照してください

namespace Invader
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Stage stage;
        public MainPage()
        {
            this.InitializeComponent();
            stage = Stage.getInstance();
            stage.clock += Stage_clock;
            this.Loaded += delegate { this.Focus(FocusState.Programmatic); };
        }

        private async void Stage_clock(object sender, EventArgs e)
        {
            if (stage.state == Def.State.End)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    this.Frame.Navigate(typeof(End));
                });
            } 
               
        }

        private void keyPressed(object sender, KeyRoutedEventArgs e)
        {
            stage.interactByKeyPress(sender, e);
        }

        private void keyReleased(object sender, KeyRoutedEventArgs e)
        {
            stage.interactByKeyRelease(sender, e);
        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            stage.interactByKeyPress(sender, e);
            textBlock.Text = e.Key.ToString();
        }

        private void Grid_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            stage.interactByKeyRelease(sender, e);
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            stage.interactByKeyPress(sender, e);
            textBlock.Text = e.Key.ToString();
        }
    }
}
