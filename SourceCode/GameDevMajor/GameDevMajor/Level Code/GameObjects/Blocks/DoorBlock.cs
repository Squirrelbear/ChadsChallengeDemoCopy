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
    class DoorBlock : Block
    {
        ItemCode key;

        public DoorBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Door), new ArrayList(), facing, BlockType.Door, lvl)
        {

            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(50, 0));
            spritePosList.Add(new Vector2(100, 0));
            spritePosList.Add(new Vector2(150, 0));

            switch (facing)
            {
                case Direction.Up:
                    key = ItemCode.KeyRed;
                    break;
                case Direction.Right:
                    key = ItemCode.KeyBlue;
                    break;
                case Direction.Down:
                    key = ItemCode.KeyYellow;
                    break;
                case Direction.Left:
                    key = ItemCode.KeyGreen;
                    break;
                default:
                    Debug.Assert(false, "Error, a door of facing " + facing + " could not be created!");
                    break;
            }


            setCanEnterPlayerAll(false);
            setCanEnterMonsterAll(false);
        }

        public override bool canEnter(GameObject obj, GameObject.Direction dir, int depth)
        {
            if (obj.getType() == Type.Player)
            {
                if (lvl.getPlayer().hasItem(key))
                {
                    lvl.getPlayer().removeItem(key);

                    BlockSave newBlock = new BlockSave();
                    newBlock.blockType = BlockType.Tile;
                    newBlock.facing = Direction.Up;
                    newBlock.position = new Vector2(position.X, position.Y);
                    GameObject block = lvl.createBlock(newBlock);
                    lvl.setBlock(position, block);
                    return true;
                }
            }

            return base.canEnter(obj, dir, 0);
        }
    }
}

