using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace GameDevMajor
{
    public class BlockDependency
    {
        public enum DependencyMode { RemoveBoth, RemoveDependent, RemoveDependentOn, None };

        private Vector2 dependentBlock;
        private Vector2 dependentOnBlock;
        private DependencyMode mode;
        private Level lvl;

        public BlockDependency(Vector2 dependentBlock, Vector2 dependentOnBlock, DependencyMode dependentMode, Level lvl)
        {
            this.dependentBlock = new Vector2(dependentBlock.X, dependentBlock.Y);
            this.dependentOnBlock = new Vector2(dependentOnBlock.X, dependentOnBlock.Y);
            this.mode = dependentMode;
            this.lvl = lvl;
        }

        public void checkRequireAction(Vector2 pos)
        {
            if (mode == DependencyMode.None)
            {
                return;
            }

            if (mode == DependencyMode.RemoveBoth && (pos.Equals(dependentBlock) || pos.Equals(dependentOnBlock)))
            {
                safeRemoveBlock(dependentBlock);
                safeRemoveBlock(dependentOnBlock);

                return;
            }

            if (mode == DependencyMode.RemoveDependent && pos.Equals(dependentOnBlock))
            {
                safeRemoveBlock(dependentBlock);
                return;
            }

            if (mode == DependencyMode.RemoveDependentOn && pos.Equals(dependentBlock))
            {
                safeRemoveBlock(dependentOnBlock);
                return;
            }
        }

        private void safeRemoveBlock(Vector2 pos)
        {
            // unlock the dependency to remove looping
            mode = DependencyMode.None;

            lvl.setBlock(pos, getBasicTile(pos));
        }

        private GameObject getBasicTile(Vector2 pos)
        {
            return lvl.createBlock(new BlockSave(Block.BlockType.Tile, pos, GameObject.Direction.Up));
        }
    }
}
