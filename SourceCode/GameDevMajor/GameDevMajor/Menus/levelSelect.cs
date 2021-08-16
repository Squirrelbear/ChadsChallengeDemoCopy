using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameDevMajor.Level_Code.LevelWnds;
using Microsoft.Xna.Framework.Input;

namespace GameDevMajor
{
    class LevelSelect : WndBase{

        Texture2D castle, creditsimage;
        Game1 theGame;
        SavedLevelList theList;
        int selectLevel = -1;
        SpriteFont font;
        SpriteFont fontLarge;
        LeaderBoard theLeaderBoard;


        public LevelSelect(ContentManager contentManager, Game1 game, SavedLevelList levels)
            : base(null, game, contentManager){

            theContentManager = contentManager;
            theGame = game;
            theList = levels;
            loadContent();
            init();
        }

        void loadContent() {


            castle = theContentManager.Load<Texture2D>("WndBaseContent/Menus/menucastle_cartoon");
            creditsimage = theContentManager.Load<Texture2D>("WndBaseContent/WndBase/scrollBG");
            font = theContentManager.Load<SpriteFont>("WndBaseContent/Fonts/smallFont");
            fontLarge = theContentManager.Load<SpriteFont>("WndBaseContent/Fonts/largeFont");
        }

        void init() {

            buttonList = new ButtonListView(theContentManager, 8, new Vector2(drawRegion.X - (drawRegion.Width * .1f), drawRegion.Y), new Vector2(0, (drawRegion.Height * .1f)));

            int posMod = 0;
            foreach (LevelInfo inf in theList.getLevelList())
            {
                Button btn = new TextButton(theContentManager,
                new Vector2(drawRegion.X - (drawRegion.Width * .1f), drawRegion.Y + (drawRegion.Height * (posMod * .1f))),
                new Vector2(drawRegion.Width * .3f, drawRegion.Height * .1125f),
                (posMod==0), "base2", "base1", "Level " + inf.levelNumber, fontLarge);

                btn.setActionID(inf.levelNumber);

                buttonList.add(btn);
                posMod++;
            }
        }

        public override void draw(SpriteBatch sb) {

            base.draw(sb);

            if (isVisible) {

                sb.Draw(castle, new Rectangle(
                    0,
                    0,
                    (int)(castle.Width),
                    (int)(castle.Height)),
                    Color.White);

                sb.Draw(creditsimage, new Rectangle(
                    (int)(drawRegion.X + (drawRegion.Width * .3)),
                    (int)(drawRegion.Y + (drawRegion.Height * .15)),
                    (int)(drawRegion.Width * .8),
                    (int)(drawRegion.Height * .8)),
                    Color.White);

                if (selectLevel != -1)
                {
                    //should load leaderboard, was test for image location and loading on button press

                    myDrawString(sb, fontLarge, "Level " + theList.getLevelList()[selectLevel - 1].levelNumber + ": " + theList.getLevelList()[selectLevel - 1].levelName, .25f, 0.05f);
                    

                    //draw highscores title...
                    myDrawString(sb, "HIGHSCORES", .59f, .30f);

                    //draw highscores 
                    float modifier = .37f;
                    for (int i = 1; i < 8; i++)
                    {

                        sb.DrawString(theFont, "#" + i + "  "
                            + theLeaderBoard.getScores()[i - 1].getName() + "  "
                            + theLeaderBoard.getScores()[i - 1].getScore(),
                             new Vector2(drawRegion.X + (drawRegion.Width * .52f),
                             drawRegion.Y + (drawRegion.Height * modifier)),
                            Color.Black);

                        modifier += .05f;
                    }

                    myDrawString(sb, "Press A (xbox) or Enter to Play Level", .45f, .95f);
                    myDrawString(sb, "Press Y (xbox) or E to Edit Level", .45f, 1f);
                    myDrawString(sb, "Press B (xbox) or Escape to Exit", .45f, 1.05f);
                }
                else
                {
                    myDrawString(sb, fontLarge, "Select a level to play or edit!", .25f, 0.05f);

                    myDrawString(sb, "Press A (xbox) or Enter to Select a Level", .45f, 1f);
                    myDrawString(sb, "Press B (xbox) or Escape to Exit", .45f, 1.05f);
                }
                

                buttonList.draw(sb);
            }
        }

        public override void update(GameTime gt) {

            kbState = theGame.getKeyboardState();
            gpState = theGame.getGamePadState();

            if (isVisible) {

                if (checkKey(Keys.Enter) || checkPad(Buttons.A)) {
                    buttonList.playSelectedSound();

                    if (selectLevel == -1)
                    {
                        selectLevel = buttonList.getSelected().getActionID();
                        theLeaderBoard = new LeaderBoard("Level_" + theList.getLevelList()[selectLevel - 1].levelNumber);
                    }
                    else
                    {
                        theGame.selectLevel(theList.getLevelList()[selectLevel - 1].levelNumber, false);
                    }
                }

                if (checkKey(Keys.E) || checkPad(Buttons.Y))
                {
                    buttonList.playSelectedSound();

                    if (selectLevel != -1)
                    {
                        theGame.selectLevel(theList.getLevelList()[selectLevel-1].levelNumber, true);
                    }
                }

                if (checkKey(Keys.Escape) || checkPad(Buttons.B))
                {
                    if (selectLevel == -1)
                    {
                        theGame.openMainMenu();
                    }
                    else
                    {
                        selectLevel = -1;
                    }
                }

                if (selectLevel == -1)
                {
                    if (checkKey(Keys.Down) || checkPad(Buttons.DPadDown))
                        buttonList.next();

                    if (checkKey(Keys.Up) || checkPad(Buttons.DPadUp))
                        buttonList.previous();
                }
            }

            oldKbState = kbState;
            oldGPstate = gpState;
        }



    }
}
