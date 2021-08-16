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
    class SpeedBlock : Block
    {
        public SpeedBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Speed), new ArrayList(), facing, BlockType.Speed, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(50, 0));
            spritePosList.Add(new Vector2(100, 0));
            spritePosList.Add(new Vector2(150, 0));

            setCanEnterPlayerAll(true);
            setCanEnterMonsterAll(true);
        }

        public override void onEntered(GameObject obj)
        {
            base.onEntered(obj);

            if (obj.getType() == Type.Player)
            {
                if (lvl.getPlayer().hasItem(ItemCode.Suction))
                    return;
                lvl.getPlayer().setSpeedMulti(1.5f);
            }
            else
            {
                ((Monster)obj).setSpeedMulti(1.5f);
            }


            lvl.moveObject(obj, facing);
            obj.setFacing(facing);
        }

        public override void onExited(GameObject obj)
        {
            base.onExited(obj);

            if (obj.getType() == Type.Player)
            {
                lvl.getPlayer().resetSpeedMultiplier();
            }
            else
            {
                ((Monster)obj).resetSpeedMultiplier();
            }
        }

        public override bool canEnter(GameObject obj, GameObject.Direction dir, int depth)
        {
            if (obj.getType() == Type.Player)
                if (lvl.getPlayer().hasItem(ItemCode.Suction))
                    return true;

            if (obj.getType() == Type.Monster)
                return dir != facing;
            return base.canEnter(obj, dir, depth);
        }

        public override bool canExit(GameObject obj, GameObject.Direction dir)
        {
            if (obj.getType() == Type.Player)
                if (lvl.getPlayer().hasItem(ItemCode.Suction))
                    return true;

            return (dir == facing);
        }
    }
}

