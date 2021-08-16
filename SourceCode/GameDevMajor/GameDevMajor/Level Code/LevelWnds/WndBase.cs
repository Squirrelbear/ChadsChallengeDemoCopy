using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using GameDevMajor.Level_Code.LevelWnds;
using Microsoft.Xna.Framework.Input;

namespace GameDevMajor{

    class WndBase{

        /// <summary>
        /// A pointer to the level object
        /// </summary>
        protected Level theLevel;

        protected Game1 game;

        protected ContentManager theContentManager;

        /// <summary>
        /// A background for whatever the window is.
        /// </summary>
        protected Texture2D background;

        protected Vector2 screenRes = new Vector2(1024, 720); // this should be grabbed from game somehow
 
        /// <summary>
        /// The area within which drawing is allowed.
        /// </summary>
        protected Rectangle drawRegion;

        /// <summary>
        /// The actual region that drawing will be done
        /// This is used with drawRegion to centre the object.
        /// </summary>
        protected Rectangle wndArea;

        /// <summary>
        /// Container class for any button elements
        /// </summary>
        protected ButtonCollection buttonList;

        /// <summary>
        /// Used to fade out game screen
        /// </summary>
        protected Texture2D fader;

        /// <summary>
        /// Determines whether to draw the screen
        /// </summary>
        protected Boolean isVisible;

        protected KeyboardState kbState, oldKbState;
        protected GamePadState gpState, oldGPstate;

        protected SpriteFont theFont;

        public WndBase(Level lvl, Game1 game, ContentManager contentManager){
            this.game = game;
            theLevel = lvl;
            theContentManager = contentManager;
            background = theContentManager.Load<Texture2D>("./WndBaseContent/WndBase/ScrollBG");
            fader = theContentManager.Load<Texture2D>("./WndBaseContent/WndBase/fader");
            theFont = selectFont(theContentManager);

            init();
        }

        void init() {

            isVisible = true;

            screenRes = game.getDisplayDimensions();

            wndArea = new Rectangle(0, 0, (int)screenRes.X, (int)screenRes.Y);

            drawRegion = new Rectangle(
                (int)((wndArea.Width - screenRes.X * .80f) * .5f),
                (int)((wndArea.Height - screenRes.Y * .85f) * .25f),
                (int)(screenRes.X * .80f),
                (int)(screenRes.Y * .85f));

            buttonList = new ButtonCollection(theContentManager);
        }

        public virtual void draw(SpriteBatch sb){

            if (isVisible) {

                sb.Draw(fader, new Rectangle(0, 0, (int)screenRes.X, (int)screenRes.Y), new Color(255, 255, 255, 200));
                sb.Draw(background, drawRegion, Color.White);
            }
        }

        public virtual void update(GameTime gt) { 
        
        }

        //returns a suitable font size for the screen res
        internal SpriteFont selectFont(ContentManager contentManager) {

            if (screenRes.X > 1200)
                return theContentManager.Load<SpriteFont>("./WndBaseContent/Fonts/hugeFont");
            else if (screenRes.X > 800)
                return theContentManager.Load<SpriteFont>("./WndBaseContent/Fonts/largeFont");
            else if (screenRes.X > 600)
                return theContentManager.Load<SpriteFont>("./WndBaseContent/Fonts/mediumFont");
            else
                return theContentManager.Load<SpriteFont>("./WndBaseContent/Fonts/smallFont");
        }

        protected void myDrawString(SpriteBatch sb, String text, float xMod, float yMod) {

            sb.DrawString(theFont, text,
                new Vector2(
                    drawRegion.X + (drawRegion.Width * xMod),
                    drawRegion.Y + (drawRegion.Height * yMod)),
                    Color.Black);
        }

        protected void myDrawString(SpriteBatch sb, SpriteFont customFont, String text, float xMod, float yMod)
        {

            sb.DrawString(customFont, text,
                new Vector2(
                    drawRegion.X + (drawRegion.Width * xMod),
                    drawRegion.Y + (drawRegion.Height * yMod)),
                    Color.Black);
        }

        protected void myDrawString(SpriteBatch sb, SpriteFont customFont, Color color, String text, float xMod, float yMod)
        {

            sb.DrawString(customFont, text,
                new Vector2(
                    drawRegion.X + (drawRegion.Width * xMod),
                    drawRegion.Y + (drawRegion.Height * yMod)),
                    color);
        }

        protected void myDrawString(SpriteBatch sb, Color color, String text, float xMod, float yMod)
        {

            sb.DrawString(theFont, text,
                new Vector2(
                    drawRegion.X + (drawRegion.Width * xMod),
                    drawRegion.Y + (drawRegion.Height * yMod)),
                    color);
        }

        protected Boolean checkKey(Keys theKey) {

            return oldKbState.IsKeyDown(theKey) && kbState.IsKeyUp(theKey);
        }

        protected Boolean checkPad(Buttons theButton) {

            return oldGPstate.IsButtonDown(theButton) && gpState.IsButtonUp(theButton);
        }

        internal virtual Boolean isActive() {

            return isVisible;
        }
    }
}
