using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameDevMajor.Level_Code.LevelWnds;
using Microsoft.Xna.Framework.Input;

namespace GameDevMajor{
 
    class LevelFailed : WndBase{

        Texture2D rip, levelFailed;
        Button retry, menu;
        String failMessage;
        Game1 theGame;

        public LevelFailed(Level lvl, ContentManager contentManager, String msg)
            : base(lvl, lvl.getGameRef(), contentManager){

            theContentManager = contentManager;
            failMessage = msg;
            theGame = lvl.getGameRef();
            loadContent();
            init();
        }

        void loadContent() {

            rip = theContentManager.Load<Texture2D>("./WndBaseContent/LevelFailed/rip");
            levelFailed = theContentManager.Load<Texture2D>("./WndBaseContent/LevelFailed/levelFailed");
        }

        void init() {

            retry = new Button(theContentManager,
                new Vector2(drawRegion.X + (drawRegion.Width * .2f), drawRegion.Y + (drawRegion.Height * .69f)),
                new Vector2(drawRegion.Width * .3f, drawRegion.Height * .1125f),
                true, "retryLevel2", "retryLevel");

            menu = new Button(theContentManager,
                new Vector2(drawRegion.X + (drawRegion.Width * .49f), drawRegion.Y + (drawRegion.Height * .69f)),
                new Vector2(drawRegion.Width * .3f, drawRegion.Height * .1125f),
                false, "mainMenu", "mainMenu2");

            buttonList.add(retry);
            buttonList.add(menu);
        }

        public override void draw(SpriteBatch sb) {

            base.draw(sb);

            if (isVisible) {

                sb.Draw(levelFailed, new Rectangle(
                    (int)(drawRegion.X + (drawRegion.Width * .2)),
                    (int)(drawRegion.Y + (drawRegion.Height * .12)),
                    (int)(drawRegion.Width * .6),
                    (int)(drawRegion.Width * .14)),
                    Color.White);

                sb.Draw(rip, new Rectangle(
                    (int)(drawRegion.X + (drawRegion.Width * .41)),
                    (int)(drawRegion.Y + (drawRegion.Height * .33)),
                    (int)(drawRegion.Width * .15),
                    (int)(drawRegion.Width * .18)),
                    Color.White);

                myDrawString(sb, failMessage, .23f, .64f);

                buttonList.draw(sb);
            }
        }

        public override void update(GameTime gt) {

            kbState = theLevel.getKeyboardState();
            gpState = theLevel.getGamePadState();

            if (isVisible) {

                if (checkKey(Keys.Enter) || checkPad(Buttons.A)) {

                    buttonList.playSelectedSound();

                    if (buttonList.getSelected().Equals(retry)) {
                        
                        isVisible = false;
                        theLevel.resetLevel();
                        theLevel.setState(Level.GamePlayState.Playing);
                        theGame.resetLevel(theLevel.getLevelInfo().levelNumber, theLevel.getLifeBonus());
                    }

                    if (buttonList.getSelected().Equals(menu))
                    {
                        isVisible = false;
                        theGame.openMainMenu();
                    }
                }

                if (checkKey(Keys.Right) || checkPad(Buttons.DPadRight))
                    buttonList.next();

                if (checkKey(Keys.Left) || checkPad(Buttons.DPadLeft))
                    buttonList.previous();
            }

            oldKbState = kbState;
            oldGPstate = gpState;
        }
    }
}
