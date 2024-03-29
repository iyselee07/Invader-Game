﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;
using Windows.System.Threading;
using System.Threading;

namespace Invader
{
    class Stage
    {
        private PlayerCommander pCommander;
        private EnemyCommander eCommander;
        //private ThreadPoolTimer timer;
        Timer timer;
        private SortedDictionary<string, bool> moveDic = new SortedDictionary<string, bool>() {{"left", false}, {"right", false}};
        public bool currentSpace = false, previousSpace = false, oneShot = false;
        public Def.State state { private set; get; }
        public int stageNum { private set; get; }
        public int score { private set; get; }
        public Vector2 playerPosition { get { return pCommander.pAttackerPosition; } }
        public int remainingPlayerAttacker { get { return pCommander.remaining; } }
        private int shotdownCount = 0;

         public static Stage singleton = new Stage();

        public event EventHandler clock;
        public event EventHandler stageChange;

        private Stage()
        {
            state = Def.State.Title;
            score = 0;
            stageNum = 1;
            pCommander = new PlayerCommander();
            pCommander.lost += PCommander_lost;
            //timer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(timerEvent), TimeSpan.FromMilliseconds(Def.frameSpan));
            TimerCallback callback = state => { timerEvent(); };
            timer = new Timer(callback, null, 0, Def.frameSpan);

        }
        public static Stage getInstance()
        {
            return singleton;
        }

        //private void timerEvent(ThreadPoolTimer timer)
        private void timerEvent()
        {
            switch (state)
            {
                case Def.State.EnemyInit:
                    state = Def.State.PlayerInit;
                    eCommander = new EnemyCommander(stageNum);
                    eCommander.teamDamage += ECommander_teamDamage;
                    eCommander.lost += ECommander_lost;
                    eCommander.won += ECommander_won;
                    Wall.makeTemplateDefenceWall();
                    break;
                case Def.State.PlayerInit:
                    state = Def.State.InGame;
                    pCommander.organizeAttacker();
                    
                    break;
                case Def.State.InGame:
                    if (moveDic["left"]) pCommander.moveLeft();
                    if (moveDic["right"]) pCommander.moveRight();
                    oneShot = currentSpace && !previousSpace;
                    previousSpace = currentSpace;
                    if (oneShot) pCommander.shot();
                    if (eCommander != null) eCommander.invade(pCommander.pAttackerCenter, (int)Def.ObjectID.EnemyID);
                    Existence.getInstance().moveAll();
                    break;
                case Def.State.BeShotDown:
                    if (shotdownCount <= Def.shotdownTime)
                    {
                        shotdownCount++;
                    }
                    else
                    {
                        shotdownCount = 0;
                        if (pCommander.remaining == 0)
                        {
                            Existence.getInstance().clear();
                            DyingExistence.getInstance().clear();
                            pCommander.initializePlayerValues();
                            state = Def.State.GameOver;
                        }
                        else
                        {
                            state = Def.State.PlayerInit;
                        }
                    }
                    break; 
            }
        }

        

        public void interactByKeyPress(object sender, KeyRoutedEventArgs e)
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
                        score = 0;
                    }
                    else if (e.Key == Windows.System.VirtualKey.Escape)
                    {
                        state = Def.State.End;
                    }
                    break;
                case Def.State.InGame:
                    if (e.Key == Windows.System.VirtualKey.Left)
                    {
                        moveDic["left"] = true;
                    }
                    else if (e.Key == Windows.System.VirtualKey.Right)
                    {
                        moveDic["right"] = true;
                    }
                    else if (e.Key == Windows.System.VirtualKey.Space)
                    {
                        currentSpace = true;
                    }
                    break;
            }
        }

        public void interactByKeyRelease(object sender, KeyRoutedEventArgs e)
        {
            switch (state)
            {
                case Def.State.InGame:
                case Def.State.BeShotDown:
                    if (e.Key == Windows.System.VirtualKey.Left)
                    {
                        moveDic["left"] = false;
                    }
                    else if (e.Key == Windows.System.VirtualKey.Right)
                    {
                        moveDic["right"] = false;
                    }
                    else if (e.Key == Windows.System.VirtualKey.Space)
                    {
                        currentSpace = false;
                    }
                    break;
            }
        }


        private void PCommander_lost(object sender, EventArgs e)
        {
            if (state == Def.State.InGame)
            {
                state = Def.State.BeShotDown;
            }
        }

        private void ECommander_teamDamage(object sender, TeamDamageEventArgs e)
        {
            score += Def.enemyScores[e.EnemyAttackerType];
        }
        private void ECommander_won(object sender, EventArgs e)
        {
            Existence.getInstance().clear();
            DyingExistence.getInstance().clear();
            stageNum = 1;
            state = Def.State.GameOver;
        }

        private void ECommander_lost(object sender, EventArgs e)
        {
            Existence.getInstance().clear();
            DyingExistence.getInstance().clear();
            stageNum++;
            state = Def.State.EnemyInit;
        }

    }
}
