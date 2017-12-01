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

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Invader
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class End : Page
    {
        Stage stage;
        public End()
        {
            this.InitializeComponent();
            stage = Stage.getInstance();
            stage.clock += Stage_clock;
            this.Loaded += delegate { Application.Current.Exit(); };
        }

        private void keyPressed(object sender, KeyRoutedEventArgs e)
        {

        }

        private void keyReleased(object sender, KeyRoutedEventArgs e)
        {

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void Stage_clock(object sender, EventArgs e)
        {
            //
            //Application.Current.Exit();
        }
    }
}
