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
    class MainMenu : WndBase {

        Texture2D castle, title, creditsimage;
        List<Texture2D> helpImages;
        Button newGame, continueGame, levelEditor, credits, quit;
        Button help, levelSelect;
        Game1 theGame;

        bool showCredits = false;
        bool showHelp = false;
        int helpPos = 0;

        public MainMenu(ContentManager contentManager, Game1 game)
            : base(null, game, contentManager) {

            theContentManager = contentManager;
            theGame = game;
            loadContent();
            init();
        }

        void loadContent() {

            castle = theContentManager.Load<Texture2D>("WndBaseContent/Menus/menucastle_cartoon");
            title = theContentManager.Load<Texture2D>("WndBaseContent/Menus/chadsChallenge");
            creditsimage = theContentManager.Load<Texture2D>("WndBaseContent/Menus/Credits");

            helpImages = new List<Texture2D>();
            helpImages.Add(theContentManager.Load<Texture2D>("WndBaseContent/Menus/Helpmk2"));
            helpImages.Add(theContentManager.Load<Texture2D>("WndBaseContent/Menus/Help2mk1"));
            helpImages.Add(theContentManager.Load<Texture2D>("WndBaseContent/Menus/Help3mk1"));
        }

        void init() {

            newGame = new Button(theContentManager,
                new Vector2(drawRegion.X + (drawRegion.Width * .27f), drawRegion.Y + (drawRegion.Height * .20f)),
                new Vector2(drawRegion.Width * .45f, drawRegion.Height * .16875f),
                true, "newGame", "newGame2");

            continueGame = new Button(theContentManager,
                new Vector2(drawRegion.X + (drawRegion.Width * .27f), drawRegion.Y + (drawRegion.Height * .32f)),
                new Vector2(drawRegion.Width * .45f, drawRegion.Height * .16875f),
                false, "continueGame", "continueGame2");

            levelSelect = new Button(theContentManager,
                  new Vector2(drawRegion.X + (drawRegion.Width * .27f), drawRegion.Y + (drawRegion.Height * .44f)),
                 new Vector2(drawRegion.Width * .45f, drawRegion.Height * .16875f),
                  false, "levelSelect", "levelSelect2");

            levelEditor = new Button(theContentManager,
                new Vector2(drawRegion.X + (drawRegion.Width * .27f), drawRegion.Y + (drawRegion.Height * .56f)),
                new Vector2(drawRegion.Width * .45f, drawRegion.Height * .16875f),
                false, "levelEditor", "levelEditor2");

            help = new Button(theContentManager,
                new Vector2(drawRegion.X + (drawRegion.Width * .27f), drawRegion.Y + (drawRegion.Height * .68f)),
               new Vector2(drawRegion.Width * .45f, drawRegion.Height * .16875f),
                false, "help", "help2");

            credits = new Button(theContentManager,
                new Vector2(drawRegion.X + (drawRegion.Width * .27f), drawRegion.Y + (drawRegion.Height * .80f)),
                new Vector2(drawRegion.Width * .45f, drawRegion.Height * .16875f),
                false, "credits", "credits2");

            quit = new Button(theContentManager,
                new Vector2(drawRegion.X + (drawRegion.Width * .27f), drawRegion.Y + (drawRegion.Height * .92f)),
                new Vector2(drawRegion.Width * .45f, drawRegion.Height * .16875f),
                false, "quit", "quit2");

            buttonList.add(newGame);
            buttonList.add(continueGame);
            buttonList.add(levelSelect);
            buttonList.add(levelEditor);
            buttonList.add(help);
            buttonList.add(credits);
            buttonList.add(quit);
        }

        public override void draw(SpriteBatch sb) {

            base.draw(sb);

            if (isVisible) {

                sb.Draw(castle, new Rectangle(
                    0,
                    0,
                    (int)(castle.Width),
                    (int)(castle.Width)),
                    Color.White);

                if (!showCredits) {

                    sb.Draw(title, new Rectangle(
                   (int)(drawRegion.X + (drawRegion.Width * .2)),
                   (int)(drawRegion.Y + (drawRegion.Height * .05)),
                   (int)(drawRegion.Width * .6),
                   (int)(drawRegion.Width * .14)),
                   Color.White);
                }

                if (showCredits) {
                    sb.Draw(creditsimage, new Rectangle(
                    (int)(drawRegion.X - (drawRegion.Width * .20)),
                    (int)(drawRegion.Y - (drawRegion.Height * .05)),
                    (int)(drawRegion.Width * 1.4),
                    (int)(drawRegion.Height * 1.4)),
                    Color.White);

                    myDrawString(sb, Color.White, "Press B (xbox) or Escape to Return", .2f, 1.05f);

                } else if (showHelp) {
                    sb.Draw(helpImages[helpPos], new Rectangle(
                    (int)(drawRegion.X + (drawRegion.Width * .15)),
                    (int)(drawRegion.Y + (drawRegion.Height * .25)),
                    (int)(drawRegion.Width * .7),
                    (int)(drawRegion.Height * .7)),
                    Color.White);

                    myDrawString(sb, Color.White, "Arrows or DPad to Cycle Help Images", .2f, 0.95f);
                    myDrawString(sb, Color.White, "Press B (xbox) or Escape to Return", .2f, 1f);
                } else {
                    buttonList.draw(sb);
                }

            }
        }

        public override void update(GameTime gt) {

            kbState = theGame.getKeyboardState();
            gpState = theGame.getGamePadState();

            if (isVisible) {

                if (checkKey(Keys.Enter) || checkPad(Buttons.A)) {

                    buttonList.playSelectedSound();

                    if (buttonList.getSelected().Equals(newGame)) {

                        isVisible = false;
                        theGame.newGame();
                    }

                    if (buttonList.getSelected().Equals(continueGame)) {
                        //continues with the current level
                        theGame.continueGame();
                    }

                    if (buttonList.getSelected().Equals(levelEditor)) {
                        //opens level editor with a blank level
                        theGame.startNewLevel();
                    }

                    if (buttonList.getSelected().Equals(help)) {

                        showHelp = true;
                    }

                    if (buttonList.getSelected().Equals(levelSelect)) {
                        theGame.openLevelSelectMenu();
                    }

                    if (buttonList.getSelected().Equals(credits)) {
                        showCredits = true;
                    }

                    if (buttonList.getSelected().Equals(quit)) {
                        theGame.Exit();
                    }
                }

                if (showCredits || showHelp) {
                    if (checkKey(Keys.Escape) || checkPad(Buttons.B)) {
                        showCredits = false;
                        showHelp = false;
                    }

                    if (checkKey(Keys.Right) || checkPad(Buttons.DPadRight))
                    {
                        helpPos++;
                        if (helpPos >= helpImages.Count) helpPos = 0;
                    }

                    if (checkKey(Keys.Left) || checkPad(Buttons.DPadLeft))
                    {
                        helpPos--;
                        if (helpPos < 0) helpPos = helpImages.Count - 1;
                    }
                } else {
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
