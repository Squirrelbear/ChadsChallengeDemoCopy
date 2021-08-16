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
    class PatrolMonster : Monster
    {
        Vector2[] sides = { new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0) };
        AStar aStar = new AStar();
        List<Vector2> targetList = new List<Vector2>();
        GameObject toPosition;
        List<Direction> path = new List<Direction>();
        int stepCount, pathCount;

        public PatrolMonster(Vector2 pos, Direction facing, ArrayList targets, Level lvl)
            : base(pos, lvl.getSprite(Type.Monster, (int)MonsterType.PatrolMonster), new ArrayList(), facing, MonsterType.PatrolMonster, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 50));
            spritePosList.Add(new Vector2(0, 100));
            spritePosList.Add(new Vector2(0, 150));
            spritePosList.Add(new Vector2(300, 100));
            setAnimatedFrames(new int[] { 8, 8, 8, 8, 0 });

            targetList.Add(position);
            foreach (Vector2 point in targets)
                targetList.Add(point);

            if (Vector2.Equals(targetList.Last<Vector2>(), targetList.First<Vector2>()))
                targetList.RemoveAt(targetList.Count - 1);

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
                moveDelay = moveDelayTime;

                Vector2 firstPoint = ((Vector2)targetList.First<Vector2>());
                if (Vector2.Equals(position, firstPoint))
                {
                    targetList.Remove(firstPoint);
                    targetList.Add(firstPoint);
                    toPosition = lvl.getBlockAt(((Vector2)targetList.First<Vector2>()));
                    path = new List<Direction>();
                }

                if (path.Count == 0 || stepCount == pathCount || stepCount == 2)
                {
                    stepCount = 0;
                    path = aStar.getPath(this, toPosition, lvl);
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

                if (path.Count > 0)
                {
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
                lvl.failLevel("To the dungeons with you!");
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
            foreach (Vector2 point in targetList)
                save.variableData.Add(point);
            return save;
        }
    }
}
