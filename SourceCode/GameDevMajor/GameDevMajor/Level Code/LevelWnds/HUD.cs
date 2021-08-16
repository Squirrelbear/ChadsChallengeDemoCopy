using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameDevMajor
{

    class HUD : WndBase
    {

        int theLevelNumber, timeLeft, coinsLeft;
        Texture2D hud, keys, items;

        public HUD(Level lvl, ContentManager contentManager, int levelNumber)
            : base(lvl, lvl.getGameRef(), contentManager)
        {

            theContentManager = contentManager;
            theLevelNumber = levelNumber;
            loadContent();
            init();
        }

        void loadContent()
        {

            hud = theContentManager.Load<Texture2D>("./WndBaseContent/hud");
            keys = theLevel.getSprite(GameObject.Type.Block, (int)GameDevMajor.Block.BlockType.Key);
            items = theLevel.getSprite(GameObject.Type.Block, (int)GameDevMajor.Block.BlockType.Item);
        }

        void init()
        {

            drawRegion = new Rectangle(
                (int)(screenRes.X * .732f),
                0,
                (int)(screenRes.X * .27f),
                (int)(screenRes.Y));
            isVisible = true;
        }

        public override void draw(SpriteBatch sb)
        {

            sb.Draw(hud, drawRegion, Color.White);

            myDrawString(sb, "" + theLevelNumber, .45f, .15f);
            myDrawString(sb, theLevel.getLevelInfo().levelName, .45f - (theLevel.getLevelInfo().levelName.Length * .0185f), 0.20f);
            myDrawString(sb, "" + timeLeft, getTimeRegion(), .35f);
            myDrawString(sb, "" + coinsLeft, .45f, .51f);

            drawKeys(sb);
            drawItems(sb);
        }

        void drawKeys(SpriteBatch sb)
        {

            if (theLevel.getPlayer().hasItem(GameObject.ItemCode.KeyRed))
            {
                drawKey(sb, .78f, 0, theLevel.getPlayer().countItem(GameObject.ItemCode.KeyRed));
            }

            if (theLevel.getPlayer().hasItem(GameObject.ItemCode.KeyBlue))
            {
                drawKey(sb, .825f, 50, theLevel.getPlayer().countItem(GameObject.ItemCode.KeyBlue));
            }

            if (theLevel.getPlayer().hasItem(GameObject.ItemCode.KeyYellow))
            {
                drawKey(sb, .871f, 100, theLevel.getPlayer().countItem(GameObject.ItemCode.KeyYellow));
            }

            if (theLevel.getPlayer().hasItem(GameObject.ItemCode.KeyGreen))
            {
                drawKey(sb, .916f, 150, theLevel.getPlayer().countItem(GameObject.ItemCode.KeyGreen));
            }
        }

        void drawKey(SpriteBatch sb, float xMod, int xFrame, int count)
        {

            sb.Draw(keys, new Rectangle(
                    (int)(screenRes.X * xMod),
                    (int)(screenRes.Y * .7069),
                    (int)(screenRes.X * .047f),
                    (int)(screenRes.Y * .0701)),
                    new Rectangle(xFrame, 0, 50, 50), Color.White);

            if(count > 1)
                sb.DrawString(theFont, ""+count,
                new Vector2(
                    screenRes.X * xMod,
                    screenRes.Y * .7069f),
                    Color.Black);
        }

        void drawItems(SpriteBatch sb)
        {

            if (theLevel.getPlayer().hasItem(GameObject.ItemCode.FireBoots))
                drawItem(sb, .78f, 0);

            if (theLevel.getPlayer().hasItem(GameObject.ItemCode.Flippers))
                drawItem(sb, .825f, 50);

            if (theLevel.getPlayer().hasItem(GameObject.ItemCode.Suction))
                drawItem(sb, .871f, 100);

            if (theLevel.getPlayer().hasItem(GameObject.ItemCode.IceBoots))
                drawItem(sb, .916f, 150);
        }

        void drawItem(SpriteBatch sb, float xMod, int xFrame)
        {

            sb.Draw(items, new Rectangle(
                    (int)(screenRes.X * xMod),
                    (int)(screenRes.Y * .779),
                    (int)(screenRes.X * .047f),
                    (int)(screenRes.Y * .0701)),
                    new Rectangle(xFrame, 0, 50, 50), Color.White);
        }

        public override void update(GameTime gt)
        {

            timeLeft = theLevel.getTimeLeft();
            coinsLeft = theLevel.getCoinsLeft();
        }

        float getTimeRegion()
        {

            if (timeLeft < 10)
                return .45f;
            else if (timeLeft < 100)
                return .42f;
            else
                return .39f;
        }

    }
}
