using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace GameDevMajor.Level_Code.LevelWnds {

    class NameEntryScreen : WndBase{

        LevelSummary levelSummary;
        ContentManager contentManager;

        Texture2D title, enterName;
        Rectangle titleDrawRegion, enterNameDrawRegion;

        Button yes, no;

        Boolean enteringName;

        String name = "";

        Keys[] keysToCheck = new Keys[] { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
                                        Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
                                        Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
                                        Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
                                        Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
                                        Keys.Z, Keys.Back, Keys.Space };

        internal NameEntryScreen(Level level, LevelSummary ls, ContentManager cm) : base (level, level.getGameRef(), cm) {

            levelSummary = ls;
            contentManager = cm;

            title = contentManager.Load<Texture2D>("./WndBaseContent/LevelSummary/highScore");
            enterName = contentManager.Load<Texture2D>("./WndBaseContent/LevelSummary/enterName");

            levelSummary.setState(false);
            init();
        }

        void init() {

            enteringName = false;

            titleDrawRegion = new Rectangle(

               (int)(drawRegion.X + (drawRegion.Width * .2f)),
               (int)(drawRegion.Y + (drawRegion.Height * .25f)),
               (int)(drawRegion.Width * .6f),
               (int)(drawRegion.Width * .20f));

            enterNameDrawRegion = new Rectangle(

               (int)(drawRegion.X + (drawRegion.Width * .2f)),
               (int)(drawRegion.Y + (drawRegion.Height * .25f)),
               (int)(drawRegion.Width * .6f),
               (int)(drawRegion.Width * .2f));

            yes = new Button(contentManager,
                new Vector2(drawRegion.X + (drawRegion.Width * .33f), drawRegion.Y + (drawRegion.Height * .58f)),
                new Vector2(drawRegion.Width * .14f, drawRegion.Width * .065f),
                true, "yes", "yes2");

            no = new Button(contentManager,
                new Vector2(drawRegion.X + (drawRegion.Width * .50f), drawRegion.Y + (drawRegion.Height * .58f)),
                new Vector2(drawRegion.Width * .14f, drawRegion.Width * .065f),
                false, "no", "no2");

            buttonList.add(yes);
            buttonList.add(no);
        }

        public override void draw(SpriteBatch sb) {

            if (isVisible) {

                base.draw(sb);
                sb.Draw(title, titleDrawRegion, Color.White);

                if (buttonList != null)
                    buttonList.draw(sb);
            }

            if (enteringName) {

                sb.Draw(fader, new Rectangle(0, 0, (int)screenRes.X, (int)screenRes.Y), new Color(255, 255, 255, 200));
                sb.Draw(background, drawRegion, Color.White);
                sb.Draw(enterName, enterNameDrawRegion, Color.White);
                myDrawString(sb, name, .40f, .45f);
            }
        }

        public override void update(GameTime gt) {

            kbState = levelSummary.getKeyboardState();
            gpState = levelSummary.getGamePadState();

            foreach (Keys key in keysToCheck) {

                if (checkKey(key)) {

                    AddCharToName(key);
                    break;
                }
            }

            if (checkKey(Keys.Enter) || checkPad(Buttons.A)) {

                if (!enteringName && !isVisible)
                    return;

                if (enteringName && name != "") {

                    levelSummary.addHighScore(name);
                    isVisible = false;
                    enteringName = false;
                    levelSummary.setState(true);
                    buttonList.playSelectedSound();
                    return;
                }

                if (buttonList.getSelected().Equals(no)) {

                    levelSummary.addHighScore("Anonymous");
                    isVisible = false;
                    levelSummary.setState(true);
                    buttonList.playSelectedSound();
                }

                if (buttonList.getSelected().Equals(yes)) {

                    isVisible = false;
                    enteringName = true;
                    buttonList.playSelectedSound();
                }
            }

            if (checkKey(Keys.Right) || checkPad(Buttons.DPadRight))
                buttonList.next();

            if (checkKey(Keys.Left) || checkPad(Buttons.DPadLeft))
                buttonList.previous();

            oldKbState = kbState;
            oldGPstate = gpState;
        }

        void AddCharToName(Keys key) {

            string newChar = "";

            if (name.Length >= 20 && key != Keys.Back)
                return;

            switch (key) {
                case Keys.A:
                    newChar += "a";
                    break;
                case Keys.B:
                    newChar += "b";
                    break;
                case Keys.C:
                    newChar += "c";
                    break;
                case Keys.D:
                    newChar += "d";
                    break;
                case Keys.E:
                    newChar += "e";
                    break;
                case Keys.F:
                    newChar += "f";
                    break;
                case Keys.G:
                    newChar += "g";
                    break;
                case Keys.H:
                    newChar += "h";
                    break;
                case Keys.I:
                    newChar += "i";
                    break;
                case Keys.J:
                    newChar += "j";
                    break;
                case Keys.K:
                    newChar += "k";
                    break;
                case Keys.L:
                    newChar += "l";
                    break;
                case Keys.M:
                    newChar += "m";
                    break;
                case Keys.N:
                    newChar += "n";
                    break;
                case Keys.O:
                    newChar += "o";
                    break;
                case Keys.P:
                    newChar += "p";
                    break;
                case Keys.Q:
                    newChar += "q";
                    break;
                case Keys.R:
                    newChar += "r";
                    break;
                case Keys.S:
                    newChar += "s";
                    break;
                case Keys.T:
                    newChar += "t";
                    break;
                case Keys.U:
                    newChar += "u";
                    break;
                case Keys.V:
                    newChar += "v";
                    break;
                case Keys.W:
                    newChar += "w";
                    break;
                case Keys.X:
                    newChar += "x";
                    break;
                case Keys.Y:
                    newChar += "y";
                    break;
                case Keys.Z:
                    newChar += "z";
                    break;
                case Keys.Space:
                    newChar += " ";
                    break;
                case Keys.Back:
                    if (name.Length != 0)
                        name = name.Remove(name.Length - 1);
                    return;
            }

            if (kbState.IsKeyDown(Keys.RightShift) || kbState.IsKeyDown(Keys.LeftShift))
                newChar = newChar.ToUpper();

            name += newChar;
        }

        internal override Boolean isActive() {

            return isVisible || enteringName;
        }
    }
}
