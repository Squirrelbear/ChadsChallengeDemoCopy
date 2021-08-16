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

namespace GameDevMajor.Monsters
{
    class AssassinMonster : Monster
    {
        AStar aStar = new AStar();
        List<Direction> path = new List<Direction>();
        int stepCount, pathCount;

        public AssassinMonster(Vector2 pos, Direction facing, Level lvl)
            : base(pos, lvl.getSprite(Type.Monster, (int)MonsterType.Assassin), new ArrayList(), facing, MonsterType.Assassin, lvl)
        {
            //dimensions = new Vector2(96, 96);
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 50));
            spritePosList.Add(new Vector2(0, 100));
            spritePosList.Add(new Vector2(0, 150));
            spritePosList.Add(new Vector2(0, 0));
            setAnimatedFrames(new int[] { 8, 8, 8, 8, 0 });

            // set play enter, but monsters can't
            setCanEnterMonsterAll(false);
            setCanEnterPlayerAll(true);

            // moveDelayTime = xxx <- where xxx is a time between movements
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

                if (path.Count == 0 || stepCount == pathCount || stepCount == 2)
                {
                    stepCount = 0;
                    path = aStar.getPath(this, lvl);
                    pathCount = path.Count;
                    Direction lastDir = path.FindLast(
                        delegate(Direction dir)
                        {
                            return dir == Direction.Neutral;
                        });
                    if (lastDir == Direction.Neutral)
                    {
                        pathCount--;
                        path.RemoveAt(pathCount);
                    }
                }

                if (path.Count == 0)
                    setAnimate(false);
                else
                {
                    if (!getisAnimating())
                        setAnimate(true);

                    Direction toDir = (Direction)path.First<Direction>();
                    move(toDir);
                    path.Remove(toDir);
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
                lvl.failLevel("Beware the assassin, he is swift of foot!");
            }
            else if (obj.getType() == Type.Monster)
            {
                Monster m = (Monster)obj;
                if (m.getMonsterType() == MonsterType.Arrow)
                    lvl.removeMonstersAt(position);
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
