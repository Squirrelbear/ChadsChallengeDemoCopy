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
    class GravelBlock : Block
    {
        public GravelBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Gravel), new ArrayList(), facing, BlockType.Gravel, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));

            setCanEnterPlayerAll(true);
            setCanEnterMonsterAll(false);
        }

        public override bool canEnter(GameObject obj, GameObject.Direction dir, int depth)
        {
            {
                if (obj.getType() == Type.Monster)
                {
                    Monster monster = (Monster)obj;
                    if (monster.getMonsterType() == Monster.MonsterType.Ice ||
                        monster.getMonsterType() == Monster.MonsterType.Moveable ||
                        monster.getMonsterType() == Monster.MonsterType.Arrow)
                    {
                        return true;
                    }
                }

                return base.canEnter(obj, dir, 0);
            }
        }

        public override bool canExit(GameObject obj, GameObject.Direction dir)
        {
            if (obj.getType() == Type.Monster && ((Monster)obj).getMonsterType() == Monster.MonsterType.Arrow) return true;

            return base.canExit(obj, dir);
        }
    }
}

