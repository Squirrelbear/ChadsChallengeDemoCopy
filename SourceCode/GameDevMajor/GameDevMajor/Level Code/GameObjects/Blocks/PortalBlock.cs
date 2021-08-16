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
    class PortalBlock : Block
    {
        private Vector2 target;
        private Vector2[] sides = { new Vector2(0, -1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0) };

        public PortalBlock(Vector2 position, Direction facing, Vector2 target, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Portal), new ArrayList(), facing, BlockType.Portal, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));

            this.target = target;

            setCanEnterPlayerAll(true);
            setCanEnterMonsterAll(true);
        }

        public override void onEntered(GameObject obj)
        {
            base.onEntered(obj);

            Vector2 endPos = target - sides[(int)lvl.getFlippedDirection(obj.getFacing())];

            if (lvl.canEnter(obj, endPos, obj.getFacing()))
            {
                obj.setPosition(target);
                lvl.moveObject(obj, obj.getFacing());
            }
            else
            {
                obj.setFacing(lvl.getFlippedDirection(obj.getFacing()));
                lvl.moveObject(obj, obj.getFacing());
            }


        }

        public Vector2 getEndPos()
        {
            return target;
        }

        public override BlockSave createSaveState()
        {
            BlockSave save  = base.createSaveState();
            save.variableData = new ArrayList();
            save.variableData.Add(target);
            return save;
        }
    }
}
