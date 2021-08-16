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
    class CoinBlock : Block
    {
        public CoinBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Coin), new ArrayList(), facing, BlockType.Coin, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));

            setCanEnterPlayerAll(true);
            setCanEnterMonsterAll(true);

            lvl.increaseCollectibleCoins();
        }

        ~CoinBlock()
        {
            lvl.decreaseCollectibleCoins();
        }

        public override void onEntered(GameObject obj)
        {
            base.onEntered(obj);

            if (obj.getType() == Type.Player)
            {
                lvl.collectCoin();

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

