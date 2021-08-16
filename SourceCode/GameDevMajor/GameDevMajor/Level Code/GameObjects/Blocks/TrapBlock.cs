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
    class TrapBlock : Block
    {
        public TrapBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Trap), new ArrayList(), facing, BlockType.Trap, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(50, 0));

            setCanEnterPlayerAll(true);
            setCanEnterMonsterAll(true);
        }

        public override void setFacing(Direction facing)
        {
            if (this.facing == Direction.Up) base.setFacing(Direction.Right);
            else base.setFacing(Direction.Up);
        }

        public override bool canExit(GameObject obj, Direction dir)
        {
            if (facing == Direction.Up)
                return false;

            return base.canExit(obj, dir);
        }
    }
}

