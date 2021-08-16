using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections;
using System.Diagnostics;

namespace GameDevMajor.Blocks
{
    class WinBlock : Block
    {
        public WinBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Win), new ArrayList(), facing, BlockType.Win, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));
            setAnimatedFrames(new int[] { 2, 0, 0, 0, 0 });
            frameTime = 400;

            setCanEnterPlayerAll(true);
            setCanEnterMonsterAll(true);
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 pos)
        {
            if (animatedFrames[(int)facing] != 0)
            {
                curFrame += lvl.deltaTime / frameTime;
                if ((int)curFrame >= animatedFrames[(int)facing]) curFrame = 0;
            }

            base.draw(spriteBatch, pos);
        }

        public override void onEntered(GameObject obj)
        {
            base.onEntered(obj);

            if (obj.getType() == Type.Player)
                lvl.finishLevel();
        }
    }
}

