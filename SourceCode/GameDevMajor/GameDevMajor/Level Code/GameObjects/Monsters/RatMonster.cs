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
    class RatMonster : Monster
    {
        Random gen;

        public RatMonster(Vector2 pos, Direction facing, Level lvl)
            : base(pos, lvl.getSprite(Type.Monster, (int)MonsterType.Rat), new ArrayList(), facing, MonsterType.Rat, lvl)
        {
            gen = new Random();

            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 50));
            spritePosList.Add(new Vector2(0, 100));
            spritePosList.Add(new Vector2(0, 150));
            spritePosList.Add(new Vector2(300, 100));
            setAnimatedFrames(new int[] { 8, 8, 8, 8, 0 });

            // set play enter, but monsters can't
            setCanEnterMonsterAll(false);
            setCanEnterPlayerAll(true);
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

                // check if entry is allowed in the direction of current facing
                if (lvl.canMove(this, facing))
                {
                    // can move forward still so do so
                    move(facing);
                }
                else
                {
                    Direction moveDir = Direction.Neutral;

                    if (facing == Direction.Up || facing == Direction.Down)
                    {
                        if (!lvl.canMove(this, Direction.Right) && !lvl.canMove(this, Direction.Left))
                        {
                            // if movement allowed in opposite direction
                            if (lvl.canMove(this, lvl.getFlippedDirection(facing)))
                            {
                                // move in the flipped direction
                                moveDir = lvl.getFlippedDirection(facing);
                            }

                            // if the above failed the direction will be set to neutral
                        }
                        else if (!lvl.canMove(this, Direction.Right))
                        {
                            moveDir = Direction.Left;
                        }
                        else if (!lvl.canMove(this, Direction.Left))
                        {
                            moveDir = Direction.Right;
                        }
                        else
                        {
                            // select random direction
                            moveDir = (gen.Next(2) == 0) ? Direction.Left : Direction.Right;
                        }
                    }
                    else if (facing == Direction.Right || facing == Direction.Left)
                    {
                        if (!lvl.canMove(this, Direction.Down) && !lvl.canMove(this, Direction.Up))
                        {
                            // if movement allowed in opposite direction
                            if (lvl.canMove(this, lvl.getFlippedDirection(facing)))
                            {
                                // move in the flipped direction
                                moveDir = lvl.getFlippedDirection(facing);
                            }

                            // if the above failed the direction will be set to neutral
                        }
                        else if (!lvl.canMove(this, Direction.Down))
                        {
                            moveDir = Direction.Up;
                        }
                        else if (!lvl.canMove(this, Direction.Up))
                        {
                            moveDir = Direction.Down;
                        }
                        else
                        {
                            // select random direction
                            moveDir = (gen.Next(2) == 0) ? Direction.Up : Direction.Down;
                        }
                    }

                    // begin the movement
                    if (moveDir != Direction.Neutral)
                    {
                        move(moveDir);
                    }
                    else
                    {
                        setFacing(Direction.Down);
                    }
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
                lvl.failLevel("The rodents are everywhere... we need a cat...");
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
