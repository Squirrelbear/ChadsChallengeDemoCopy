using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using GameDevMajor.Level_Code.LevelWnds;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace GameDevMajor {

    class LevelSummary : WndBase {

        int theScore, timeLeft, timeBonus, levelBonus, totalScore;
        
        Texture2D line, title;

        LeaderBoard theLeaderBoard;

        Button continueButton, resetButton;

        NameEntryScreen nameEntryScreen;

        public LevelSummary(Level lvl, int timeLeft, ContentManager contentManager, LeaderBoard leaderboard)
            : base(lvl, lvl.getGameRef(), contentManager) {

            this.timeLeft = timeLeft;

            theLeaderBoard = leaderboard;
            theContentManager = contentManager;
            theLevel = lvl;

            timeBonus = timeLeft * 10;
            levelBonus = theLevel.getCoinsCollected() * 50 + theLevel.getLifeBonus() * 100;
            theScore = (lvl.getLevelInfo().levelNumber * 200 + timeBonus + levelBonus);
            totalScore = lvl.getGameRef().updateTotalScore(theScore);
            loadContent();
            init();
        }

        void loadContent() {

            line = theContentManager.Load<Texture2D>("./WndBaseContent/LevelSummary/line");
            title = theContentManager.Load<Texture2D>("./WndBaseContent/LevelSummary/levelComplete");
        }

        void init() {

            continueButton = new Button(theContentManager,
                new Vector2(drawRegion.X + (drawRegion.Width * .22f), drawRegion.Y + (drawRegion.Height * .75f)),
                new Vector2(drawRegion.Width * .18f, drawRegion.Height * .12f),
                true, "onward2", "onward");

            resetButton = new Button(theContentManager,
                new Vector2(drawRegion.X + (drawRegion.Width * .60f), drawRegion.Y + (drawRegion.Height * .75f)),
                new Vector2(drawRegion.Width * .18f, drawRegion.Height * .12f),
                false, "reset2", "reset");

            buttonList.add(continueButton);
            buttonList.add(resetButton);

            //if the score is a highscore, create nameEntryScreen
            if (theLeaderBoard.isHighScore(theScore))
                nameEntryScreen = new NameEntryScreen(theLevel, this, theContentManager);
        }

        public override void draw(SpriteBatch sb) {

            if (isVisible) {

                base.draw(sb);

                //dividing line between level summary & highscores
                sb.Draw(line, new Rectangle(
                    (int)(drawRegion.X + (drawRegion.Width * .5)),
                    (int)(drawRegion.Y + (drawRegion.Height * .30)),
                    (int)(drawRegion.Width * 0.005),
                    (int)(drawRegion.Height * .5)),
                    Color.White);

                //draw comment..e.g. you suck
                myDrawString(sb, getComment(theScore), .24f, .30f);

                sb.Draw(title, new Rectangle(
                    (int)(drawRegion.X + (drawRegion.Width * .2)),
                    (int)(drawRegion.Y + (drawRegion.Height * .05)),
                    (int)(drawRegion.Width * .6),
                    (int)(drawRegion.Width * .15)),
                    Color.White);

                myDrawString(sb, "Time Bonus:", .16f, .42f);
                myDrawString(sb, "" + timeBonus, .37f, .42f);

                myDrawString(sb, "Level Bonus:", .16f, .47f); 
                myDrawString(sb, "" + levelBonus, .37f, .47f);

                myDrawString(sb, "Level Score:", .16f, .52f);
                myDrawString(sb, "" + theScore, .37f, .52f);

                myDrawString(sb, "Total Score:", .16f, .57f);
                myDrawString(sb, "" + totalScore, .37f, .57f); 

                //draw highscores title...
                myDrawString(sb, "HIGHSCORES", .59f, .30f);

                //draw highscores 
                float modifier = .37f;
                for (int i = 1; i < 8; i++) {

                    sb.DrawString(theFont, "#" + i + "  "
                        + theLeaderBoard.getScores()[i - 1].getName() + "  "
                        + theLeaderBoard.getScores()[i - 1].getScore(),
                         new Vector2(drawRegion.X + (drawRegion.Width * .52f),
                         drawRegion.Y + (drawRegion.Height * modifier)),
                        Color.Black);

                    modifier += .05f;
                }

                buttonList.draw(sb);
            }

            //draw if necessary
            if (nameEntryScreen != null)
                nameEntryScreen.draw(sb);
        }

        public override void update(GameTime gameTime) {

            kbState = theLevel.getKeyboardState();
            gpState = theLevel.getGamePadState();

            if (isVisible && (checkKey(Keys.Enter) || checkPad(Buttons.A))) {

                buttonList.playSelectedSound();

                if (buttonList.getSelected().Equals(resetButton))
                    theLeaderBoard.resetScores();

                if (buttonList.getSelected().Equals(continueButton)){

                    // this just returns to level now, 
                    // in release will launch next level
                    isVisible = false;
                    //launch next level...
                    theLevel.getGameRef().nextLevel();
                
                }
            }

            if (checkKey(Keys.Right) || checkPad(Buttons.DPadRight))
                buttonList.next();

            if (checkKey(Keys.Left) || checkPad(Buttons.DPadLeft))
                buttonList.previous();

            if (nameEntryScreen != null && nameEntryScreen.isActive())
                nameEntryScreen.update(gameTime);
            else {

                nameEntryScreen = null;
                GC.Collect();
            }

            oldKbState = kbState;
            oldGPstate = gpState;
        }

        String getComment(int playerScore) {
            
            int maxScore = (int)(theLevel.getLevelInfo().levelNumber * 200 + theLevel.getCountDown() * 0.7f * 10 + theLevel.getCoinsCollected() * 50 + 5 * 100);
            int minScore = (theLevel.getLevelInfo().levelNumber * 200 - 3 * 100);

            maxScore -= minScore;
            int modScore = theScore - minScore; 
            float scale = modScore * 100f / maxScore;

            if ( scale > 85)
                return "Outstanding!";
            if (scale > 75)
                return "Not bad!";
            if (scale > 60)
                return "Average...";
            if (scale > 50)
                return "Yawn...";
            else
                return "Pathetic....";
        }

        internal override Boolean isActive() {

            if (nameEntryScreen == null)
                return isVisible;
            else
                return isVisible || nameEntryScreen.isActive();
        }

        internal KeyboardState getKeyboardState() {

            return kbState;
        }

        internal GamePadState getGamePadState() {

            return gpState;
        }

        internal void setState(Boolean state) {

            isVisible = state;
        }

        internal void addHighScore(string name) {

            theLeaderBoard.addHighScore(name, theScore);
            theLeaderBoard.sortHighScores();
            theLeaderBoard.saveHighScores();
            theLeaderBoard.loadScores();
        }
    }
}
