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

namespace GameDevMajor.Monsters
{
    class WallMonster : Monster
    {
        Vector2[] sides = { new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0) };

        public WallMonster(Vector2 pos, Direction facing, Level lvl)
            : base(pos, lvl.getSprite(Type.Monster, (int)MonsterType.Moveable), new ArrayList(), facing, MonsterType.Moveable, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 0));

            //lvl.addOtherMonster(pos);

            // set play enter, but monsters can't
            setCanEnterMonsterAll(false);
            setCanEnterPlayerAll(true);
        }

        public override bool canEnter(GameObject obj, GameObject.Direction dir, int depth)
        {
            if (obj.getType() == GameObject.Type.Player)
            {
                bool entry = lvl.canEnter(this, position - sides[(int)lvl.getFlippedDirection(dir)], lvl.getFlippedDirection(dir))
                    && lvl.canExit(this, dir);

                Block onBlock = lvl.getBlockAt(position);
                if (onBlock.getBlockType() != Block.BlockType.Speed && onBlock.getBlockType() != Block.BlockType.Ice) return entry;
                return false;
                /*if (onBlock.getBlockType() == Block.BlockType.Ice && onBlock.getFacing() != Direction.Neutral) return false;
                if (dir == facing || dir == lvl.getFlippedDirection(facing)) return false;
                return entry;*/

            }


            return base.canEnter(obj, dir, depth);
        }

        public override void onEntering(GameObject obj, Direction dir)
        {
            base.onEntering(obj, dir);

            if (obj.getType() == Type.Player)
            {
                //if (lvl.getBlockAt(this.getPosition()).getBlockType() == Block.BlockType.Ice) return;
                //if (lvl.getBlockAt(this.getPosition()).getBlockType() == Block.BlockType.Speed) return;
                move(lvl.getFlippedDirection(dir));
            }
        }
    }
}
