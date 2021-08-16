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
    class ArrowMonster : Monster
    {
        int flightDistance = 0;

        public ArrowMonster(Vector2 pos, Direction facing, Level lvl)
            : base(pos, lvl.getSprite(Type.Monster, (int)MonsterType.Arrow), new ArrayList(), facing, MonsterType.Arrow, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(50, 0));
            spritePosList.Add(new Vector2(100, 0));
            spritePosList.Add(new Vector2(150, 0));

            // set play enter, but monsters can't
            setCanEnterMonsterAll(true);
            setCanEnterPlayerAll(true);

            moveDelayTime = 0;
        }

        /// <summary>
        /// Update the movement of the monster.
        /// </summary>
        /// <param name="gameTime">The current time.</param>
        public override void update(GameTime gameTime)
        {
            // update current transitions
            base.update(gameTime);

            // do nothing if already moving
            if (isMoving()) return;

            // delay the time between movements.
            moveDelay += gameTime.ElapsedGameTime.Milliseconds;
            if (moveDelay > moveDelayTime)
            {
                moveDelay = 0;

                // check if entry is allowed in the direction of current facing
                if (lvl.canMove(this, facing) && flightDistance++ <= 8)
                    move(facing);
                else
                {
                    lvl.removeMonstersAt(position);
                    lvl.removeMonster(this);
                }
            }
        }

        /// <summary>
        /// Handle the player entering this monster.
        /// </summary>
        /// <param name="obj">The object entering</param>
        public override void onEntered(GameObject obj)
        {
            if (obj.getType() == Type.Player)
            {
                lvl.failLevel("BOOM HEADSHOT!.. You may want to duck next time.");
            }
            if (obj.getType() == Type.Monster)
            {
                lvl.removeMonstersAt(position);
                lvl.removeMonster((Monster)obj);
                lvl.removeMonster(this);
            }
        }

        /// <summary>
        /// Don't technically need this because it can use the base
        /// This is just an example of what you would need if you h
        /// </summary>
        /// <returns>A monster save.</returns>
        public override MonsterSave createSaveState()
        {
            MonsterSave save = base.createSaveState();
            //save.variableData.Add(whateverextradatatosave);
            return save;
        }
    }
}
