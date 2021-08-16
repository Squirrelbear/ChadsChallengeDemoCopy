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
    class ButtonBlock : Block
    {
        /// <summary>
        /// A toggleable target point.
        /// </summary>
        Vector2 target;

        public ButtonBlock(Vector2 position, Vector2 target, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Button), new ArrayList(), GameObject.Direction.Up, BlockType.Button, lvl)
        {
            this.target = target;
            spritePosList.Add(new Vector2(0, 0));
            
            if(lvl.getIsEditor())
                lvl.addBlockDependency(new BlockDependency(position, target, BlockDependency.DependencyMode.RemoveBoth, lvl));
        }

        public override void onEntered(GameObject obj)
        {
            base.onEntered(obj);

            Block tarBlock = lvl.getBlockAt(target);
            tarBlock.setFacing(Direction.Down);

        }

        public override void onExited(GameObject obj)
        {
            base.onExited(obj);

            Block tarBlock = lvl.getBlockAt(target);
            tarBlock.setFacing(Direction.Up);
        }

        public override BlockSave createSaveState()
        {
            BlockSave save = base.createSaveState();
            save.variableData = new ArrayList();
            save.variableData.Add(target);
            return save;
        }
    }
}
