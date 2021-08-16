using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;

namespace GameDevMajor
{
    [Serializable]
    public class BlockSave
    {
        /// <summary>
        /// The type of block that is saved.
        /// </summary>
        public Block.BlockType blockType;

        /// <summary>
        /// The position of where that block is.
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// The state related to that block.
        /// </summary>
        public GameObject.Direction facing;

        /// <summary>
        /// Additional optional parameters about the block.
        /// </summary>
        public ArrayList variableData;

        public BlockSave()
        {
            blockType = 0;
            position = Vector2.Zero;
            facing = 0;
            variableData = new ArrayList();
        }

        public BlockSave(Block.BlockType blockType, Vector2 position, GameObject.Direction facing)
        {
            this.blockType = blockType;
            this.position = position;
            this.facing = facing;
        }
    }
}
