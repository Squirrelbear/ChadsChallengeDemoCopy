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
    class DissapearingBlock : Block
    {
        public DissapearingBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Dissapearing), new ArrayList(), facing, BlockType.Dissapearing, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));

            setCanEnterPlayerAll(true);
            setCanEnterMonsterAll(false);
        }

        public override void onEntering(GameObject obj, Direction dir)
        {
            base.onEntering(obj, dir);

            BlockSave newBlock = new BlockSave();
            newBlock.blockType = BlockType.Tile;
            newBlock.facing = Direction.Up;
            newBlock.position = new Vector2(position.X, position.Y);

            GameObject block = lvl.createBlock(newBlock);
            lvl.setBlock(position, block);
        }
    }
}

