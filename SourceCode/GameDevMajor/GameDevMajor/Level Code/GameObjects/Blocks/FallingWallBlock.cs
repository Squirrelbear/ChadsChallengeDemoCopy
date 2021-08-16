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
    class FallingWallBlock : Block
    {
        public FallingWallBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.FallingWall), new ArrayList(), Direction.Up, BlockType.FallingWall, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));

            setCanEnterPlayerAll(true);
            setCanEnterMonsterAll(false);
        }

        public override void  onExited(GameObject obj)
        {
 	        base.onExited(obj);

            BlockSave blockTemplate = new BlockSave(BlockType.Wall, getPosition(), Direction.Up);
            GameObject newBlock = lvl.createBlock(blockTemplate);
            lvl.setBlock(getPosition(), newBlock);
        }
    }
}

