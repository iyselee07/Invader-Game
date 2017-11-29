using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;

namespace Invader
{
    class Stage
    {
        private PlayerCommander pCommander;
        private EnemyCommander eCommander;
        public Def.State state { private set; get; }
        public int stageNum { private set; get; }
        
        public Stage()
        {
            state = Def.State.Title;
            stageNum = 1;
            pCommander = new PlayerCommander();
            pCommander.lost += PCommander_lost;
        }

        private void PCommander_lost(object sender, EventArgs e)
        {
            if (state == Def.State.InGame)
            {
                state = Def.State.BeShotDown; 
            }
        }

        public void interactByKey(object sender, KeyRoutedEventArgs e)
        {
            switch (state)
            {
                case Def.State.Title:
                    if (e.Key == Windows.System.VirtualKey.Space)
                    {
                        state = Def.State.EnemyInit;
                    }
                    else if (e.Key == Windows.System.VirtualKey.Escape)
                    {
                        state = Def.State.End;
                    }
                    break;
                case Def.State.GameOver:
                    if (e.Key == Windows.System.VirtualKey.Space)
                    {
                        state = Def.State.Title;
                    }
                    else if (e.Key == Windows.System.VirtualKey.Escape)
                    {
                        state = Def.State.End;
                    }
                    break;
                case Def.State.InGame:
                    if (e.Key == Windows.System.VirtualKey.Left)
                    {
                        pCommander.moveLeft();
                    }
                    else if (e.Key == Windows.System.VirtualKey.Right)
                    {
                        pCommander.moveRight();
                    }
                    else if (e.Key == Windows.System.VirtualKey.Space)
                    {
                        pCommander.shot();
                    }
                    break;
            }
        }
    }
}
