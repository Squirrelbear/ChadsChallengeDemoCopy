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
    class BowBlock : Block
    {
        Vector2[] sides = { new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0) };

        public BowBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Bow), new ArrayList(), facing, BlockType.Bow, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(50, 0));
            spritePosList.Add(new Vector2(100, 0));
            spritePosList.Add(new Vector2(150, 0));

            setCanEnterPlayerAll(false);
            setCanEnterMonsterAll(false);
        }

        //Peter didnt want to copy paste and change a name
        //so we are using setFacing to not set the facing
        //but to define what happens when the corresponding
        //button is pressed or released.
        public override void setFacing(GameObject.Direction press)
        {
            //Button has been pressed.
            if (press == Direction.Down)
            {
                MonsterSave newMonster = new MonsterSave();
                newMonster.monsterType = Monster.MonsterType.Arrow;
                newMonster.facing = facing;
                newMonster.position = position - sides[(int)facing];
                GameObject monster = lvl.createMonster(newMonster);

                if (lvl.canEnter(monster, newMonster.position, facing))
                    lvl.addMonster(monster);
            }

            //Button has been released.
            if (press == Direction.Up)
            {
                return;
            }
        }
    }
}

