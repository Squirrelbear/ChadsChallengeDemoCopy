using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameDevMajor.Level_Code.LevelWnds;
using Microsoft.Xna.Framework.Input;

namespace GameDevMajor {

    class PauseMenu : WndBase {

        Texture2D gamePaused, hourGlass;
        Button resume, menu;

        public PauseMenu(Level lvl, ContentManager contentManager)
            : base(lvl, lvl.getGameRef(), contentManager) {

            theContentManager = contentManager;
            loadContent();
            init();
        }

        void loadContent() {

            gamePaused = theContentManager.Load<Texture2D>("./WndBaseContent/PauseMenu/gamePaused");
            hourGlass = theContentManager.Load<Texture2D>("./WndBaseContent/PauseMenu/hourGlass");
        }

        void init() {

            resume = new Button(theContentManager,
                new Vector2(drawRegion.X + (drawRegion.Width * .2f), drawRegion.Y + (drawRegion.Height * .7f)),
                new Vector2(drawRegion.Width * .3f, drawRegion.Height * .1125f),
                true, "backToGame", "backToGame2");

            menu = new Button(theContentManager,
                new Vector2(drawRegion.X + (drawRegion.Width * .49f), drawRegion.Y + (drawRegion.Height * .7f)),
                new Vector2(drawRegion.Width * .3f, drawRegion.Height * .1125f),
                false, "mainMenu", "mainMenu2");

            buttonList.add(resume);
            buttonList.add(menu);
        }

        public override void draw(SpriteBatch sb) {

            base.draw(sb);

            if (isVisible) {

                sb.Draw(gamePaused, new Rectangle(
                    (int)(drawRegion.X + (drawRegion.Width * .2)),
                    (int)(drawRegion.Y + (drawRegion.Height * .17)),
                    (int)(drawRegion.Width * .6),
                    (int)(drawRegion.Width * .14)),
                    Color.White);

                sb.Draw(hourGlass, new Rectangle(
                    (int)(drawRegion.X + (drawRegion.Width * .41)),
                    (int)(drawRegion.Y + (drawRegion.Height * .41)),
                    (int)(drawRegion.Width * .15),
                    (int)(drawRegion.Width * .15)),
                    Color.White);

                buttonList.draw(sb);
            }
        }

        public override void update(GameTime gt) {

            kbState = theLevel.getKeyboardState();
            gpState = theLevel.getGamePadState();

            if (isVisible) {

                if (checkKey(Keys.Enter) || checkPad(Buttons.A)) {

                    buttonList.playSelectedSound();

                    if (buttonList.getSelected().Equals(resume)){

                        isVisible = false;
                        theLevel.setState(Level.GamePlayState.Playing);
                    }

                    if (buttonList.getSelected().Equals(menu))
                    {
                        isVisible = false;
                        //return to main menu...

                        if (theLevel.getIsEditor())
                        {
                            theLevel.saveLevelToFile();
                        }
                        theLevel.getGameRef().openMainMenu();
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
