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
    class ArmorMonster : Monster
    {
        bool isAlive = false;
        Vector2[] sides = { new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0) };
        AStar aStar = new AStar();
        List<Direction> path = new List<Direction>();
        List<Vector2> targetZone = new List<Vector2>();

        public ArmorMonster(Vector2 pos, Direction facing, Level lvl)
            : base(pos, lvl.getSprite(Type.Monster, (int)MonsterType.Armor), new ArrayList(), facing, MonsterType.Armor, lvl)
        {

            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 50));
            spritePosList.Add(new Vector2(0, 100));
            spritePosList.Add(new Vector2(0, 150));
            spritePosList.Add(new Vector2(300, 100));
            setAnimatedFrames(new int[] { 8, 8, 8, 8, 0 });

            // set play enter, but monsters can't
            setCanEnterMonsterAll(false);
            setCanEnterPlayerAll(true);

            moveDelayTime = 225;
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

                //If not alive then check to see if player is infront of monster.
                if (!isAlive)
                {
                    setAnimate(false);

                    if (targetZone.Count == 0)
                        for (int i = 1; i <= 3; i++)
                            targetZone.Add(new Vector2(position.X - (sides[(int)facing] * i).X, position.Y - (sides[(int)facing] * i).Y));

                    foreach (Vector2 tar in targetZone)
                        if (Vector2.Equals(lvl.getPlayer().getPosition(), tar))
                            isAlive = true;

                    return;
                }

                //If alive then give chase.
                if (isAlive)
                {
                    targetZone.Clear();

                    path = aStar.getPath(this, lvl);
                    // Note the check is added to ensure no crash occurs
                    if (path.Count == 0)
                        setAnimate(false);
                    else
                    {
                        if (!getisAnimating())
                            setAnimate(true);

                        move((Direction)path.First<Direction>());
                    }

                    if (Vector2.Distance(lvl.getPlayer().getPosition(), getPosition()) > 3)
                        isAlive = false;
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
                lvl.failLevel("There wasn't even anything inside that armour!");
            }
            else if (obj.getType() == Type.Monster)
            {
                Monster m = (Monster)obj;
                if (m.getMonsterType() == MonsterType.Arrow)
                    m.onEntered(this);
            }
        }

        public override bool canEnter(GameObject obj, GameObject.Direction dir, int depth)
        {
            if (obj.getType() == Type.Monster && ((Monster)obj).getMonsterType() == MonsterType.Arrow) return true;

            return base.canEnter(obj, dir, depth);
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
