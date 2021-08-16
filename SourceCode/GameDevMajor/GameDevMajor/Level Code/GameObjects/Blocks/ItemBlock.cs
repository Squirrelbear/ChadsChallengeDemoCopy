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
    class ItemBlock : Block
    {
        ItemCode item;

        public ItemBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Item), new ArrayList(), facing, BlockType.Item, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(50, 0));
            spritePosList.Add(new Vector2(100, 0));
            spritePosList.Add(new Vector2(150, 0));

            switch (facing)
            {
                case Direction.Up:
                    item = ItemCode.FireBoots;
                    break;
                case Direction.Right:
                    item = ItemCode.Flippers;
                    break;
                case Direction.Down:
                    item = ItemCode.Suction;
                    break;
                case Direction.Left:
                    item = ItemCode.IceBoots;
                    break;
                default:
                    Debug.Assert(false, "Error, a item of facing " + facing + " could not be created!");
                    break;
            }

            setCanEnterPlayerAll(true);
            setCanEnterMonsterAll(true);
        }

        public override void onEntered(GameObject obj)
        {
            base.onEntered(obj);

            if (obj.getType() == Type.Player)
            {
                lvl.getPlayer().giveItem(item);
                BlockSave newBlock = new BlockSave();
                newBlock.blockType = BlockType.Tile;
                newBlock.facing = Direction.Up;
                newBlock.position = new Vector2(position.X, position.Y);
                GameObject block = lvl.createBlock(newBlock);
                lvl.setBlock(position, block);
            }
        }
    }
}

