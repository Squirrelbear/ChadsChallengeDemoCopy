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
    class SampleMonster : Monster
    {
        Random gen;

        public SampleMonster(Vector2 pos, Direction facing, Level lvl)
            : base(pos, lvl.getSprite(Type.Monster, (int)MonsterType.SampleMonster), new ArrayList(), facing, MonsterType.SampleMonster, lvl)
        {
            Debug.Assert(false, "THIS MONSTER IS DEPRECATED!!");
            gen = new Random();

            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 0));

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
            if(isMoving()) return;

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
                        setFacing(Direction.Neutral);
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
                lvl.setState(Level.GamePlayState.LevelFailed);
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
