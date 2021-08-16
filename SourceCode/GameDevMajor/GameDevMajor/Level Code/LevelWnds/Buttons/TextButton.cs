using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameDevMajor
{
    internal class TextButton : Button
    {
        String text;
        SpriteFont font;

        public TextButton(ContentManager content, Vector2 location, Vector2 size, Boolean selected, String selectedTexture, String unSelectedTexture, String text, SpriteFont font)
            : base(content, location, size, selected, selectedTexture, unSelectedTexture)
        {
            this.text = text;
            this.font = font;
        }

        internal override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (!visible) return;

            spriteBatch.DrawString(font, text, buttonLocation + new Vector2(78, 20), Color.MidnightBlue);
        }
    }
}
