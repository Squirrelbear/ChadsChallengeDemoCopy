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
    class BlueBlock : Block
    {
        public BlueBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Blue), new ArrayList(), facing, BlockType.Blue, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(50, 0));

            setCanEnterPlayerAll((facing == Direction.Right));
            setCanEnterMonsterAll((facing == Direction.Right));
        }

        /// <summary>
        /// Set the facing of this particular game object and update the sprite.
        /// </summary>
        /// <param name="facing">The new direction of face.</param>
        public override void setFacing(Direction facing)
        {
            base.setFacing(facing);

            setCanEnterPlayerAll((facing == Direction.Right));
            setCanEnterMonsterAll((facing == Direction.Right));
        }
    }
}
