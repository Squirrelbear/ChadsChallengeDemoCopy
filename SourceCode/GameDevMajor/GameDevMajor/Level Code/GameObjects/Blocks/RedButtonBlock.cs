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

namespace GameDevMajor.Blocks
{
    class RedButtonBlock : Block
    {
        public RedButtonBlock(Vector2 position, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.RedButton), new ArrayList(), GameObject.Direction.Up, BlockType.RedButton, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));
        }

        public override void onEntered(GameObject obj)
        {
            base.onEntered(obj);

            lvl.setBlockFacingToggleAll(BlockType.Red, Direction.Up, Direction.Right);
        }

       /* public override void onExited(GameObject obj)
        {
            base.onExited(obj);

            lvl.setBlockFacingToggleAll(BlockType.Red, Direction.Up, Direction.Right);
        }*/
    }
}
