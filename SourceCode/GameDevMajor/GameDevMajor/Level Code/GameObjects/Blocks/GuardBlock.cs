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
    class GuardBlock : Block
    {
        public GuardBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Guard), new ArrayList(), facing, BlockType.Guard, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 0));

            setCanEnterPlayerAll(false);
            setCanEnterMonsterAll(false);
        }

        public override bool canEnter(GameObject obj, GameObject.Direction dir, int depth)
        {
            if (obj.getType() == Type.Player && lvl.allCoinsCollected())
            {
                BlockSave newBlock = new BlockSave();
                newBlock.blockType = BlockType.Tile;
                newBlock.facing = Direction.Up;
                newBlock.position = new Vector2(position.X, position.Y);

                GameObject block = lvl.createBlock(newBlock);
                lvl.setBlock(position, block);
            }

            return base.canEnter(obj, dir, 0);
        }
    }
}

