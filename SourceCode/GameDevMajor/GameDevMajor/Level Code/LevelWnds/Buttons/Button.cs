using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameDevMajor {

    public class Button {

        protected ContentManager theContentManager;

        //location of button
        protected Vector2 buttonLocation;

        //size of button
        protected Vector2 buttonSize;

        protected Texture2D currentlySelected, unselected;

        //if true, use selected texture,
        //else, use unselected texture
        internal Boolean isSelected;

        internal bool visible;

        protected int actionID;

        internal Button(ContentManager content, Vector2 location, Vector2 size, Boolean selected, String selectedTexture, String unSelectedTexture) {

            theContentManager = content;
            isSelected = selected;
            buttonLocation = location;
            buttonSize = size;
            visible = true;
            actionID = 0;

            unselected = theContentManager.Load<Texture2D>("./WndBaseContent/Buttons/" + unSelectedTexture);
            currentlySelected = theContentManager.Load<Texture2D>("./WndBaseContent/Buttons/" + selectedTexture);
        }

        internal virtual void Draw(SpriteBatch spriteBatch) {
            if (!visible) return;

            if (isSelected)
                spriteBatch.Draw(currentlySelected, new Rectangle((int)buttonLocation.X, (int)buttonLocation.Y, (int)buttonSize.X, (int)buttonSize.Y), Color.White);
            else
                spriteBatch.Draw(unselected, new Rectangle((int)buttonLocation.X, (int)buttonLocation.Y, (int)buttonSize.X, (int)buttonSize.Y), Color.White);

        }

        internal void moveButton(Vector2 amountToMove)
        {
            buttonLocation += amountToMove;
        }

        internal void setLocation(Vector2 location)
        {
            buttonLocation = location;
        }

        internal Vector2 getLocation()
        {
            return buttonLocation;
        }

        internal void setActionID(int actionID)
        {
            this.actionID = actionID;
        }

        internal int getActionID()
        {
            return actionID;
        }

        internal bool isVisible()
        {
            return visible;
        }

        internal void setVisible(bool visible)
        {
            this.visible = visible;
        }
    }
}
