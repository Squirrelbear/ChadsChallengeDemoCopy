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
    class SpawnerBlock : Block
    {
        Vector2[] sides = { new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0) };

        public SpawnerBlock(Vector2 position, Direction facing, Level lvl)
            : base(position, lvl.getSprite(GameObject.Type.Block, (int)BlockType.Spawner), new ArrayList(), facing, BlockType.Spawner, lvl)
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
        public override void setFacing(GameObject.Direction facing)
        {
            //Button has been pressed.
            if (facing == Direction.Down)
            {
                MonsterSave newMonster = new MonsterSave();
                newMonster.monsterType = Monster.MonsterType.Rat;
                newMonster.facing = this.facing;
                newMonster.position = position - sides[(int)this.facing];
                GameObject monster = lvl.createMonster(newMonster);

                if (lvl.canEnter(monster, newMonster.position, this.facing))
                    lvl.addMonster(monster);
            }

            //Button has been released.
            if (facing == Direction.Up)
            {
                return;
            }
        }
    }
}

