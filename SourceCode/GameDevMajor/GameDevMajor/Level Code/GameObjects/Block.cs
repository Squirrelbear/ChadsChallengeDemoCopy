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

namespace GameDevMajor
{
    public class Block : GameObject
    {
        public enum BlockType
        {
            Toggle = 0, Button = 1, Wall = 2, Invisible = 3, Dissapearing = 4, Tile = 5, Ice = 6, Fire = 7,
            Gravel = 8, Theif = 9, Water = 10, Guard = 11, Door = 12, Key = 13, Item = 14, Coin = 15, Speed = 16, Red = 17, Yellow = 18,
            Green = 19, Blue = 20, RedButton = 21, YellowButton = 22, GreenButton = 23, BlueButton = 24,
            Portal = 25, Armor = 26, Bomb = 27, Bow = 28, Spawner = 29, Trap = 30, Win = 31, FallingWall = 32
        };
        protected BlockType blockType;

        public Block(Vector2 position, Texture2D sprites, ArrayList spritePosList, Direction facing, BlockType blockType, Level lvl)
            : base(position, GameObject.Type.Block, sprites, spritePosList, new Vector2(50.0f,50.0f), facing, lvl)
        {
            this.blockType = blockType;
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);
        }

        /// <summary>
        /// Get the type of block this is.
        /// </summary>
        /// <returns></returns>
        public BlockType getBlockType()
        {
            return blockType;
        }

        /// <summary>
        /// Is the block a block that needs to be referenced as one of a whole set.
        /// </summary>
        /// <returns>Is the block a state block.</returns>
        public bool isStateBlock()
        {
            return Block.isStateBlock(blockType);
        }

        /// <summary>
        /// Determine whether a block of the specified type is a state block.
        /// </summary>
        /// <param name="type">The block type to check.</param>
        /// <returns>Whether that block is a state block.</returns>
        public static bool isStateBlock(BlockType type)
        {
            switch (type)
            {
                // case statechangeable block
                case BlockType.Key:
                case BlockType.Blue:
                case BlockType.Green:
                case BlockType.Red:
                case BlockType.Yellow:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Create a save instance of this block.
        /// </summary>
        /// <returns></returns>
        public virtual BlockSave createSaveState()
        {
            BlockSave state = new BlockSave();
            state.blockType = blockType;
            state.position = position;
            state.facing = facing;
            return state;
        }
    }
}
