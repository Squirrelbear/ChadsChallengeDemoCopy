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
    class ImpMonster : Monster
    {
        Random gen = new Random();
        AStar aStar = new AStar();
        List<Direction> path = new List<Direction>();
        Vector2[] sides = { new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0) };
        int stepCount;
        bool hasKey = false;
        GameObject.ItemCode key;
        Vector2 myKeyLocation;

        public ImpMonster(Vector2 pos, Direction facing, Level lvl)
            : base(pos, lvl.getSprite(Type.Monster, (int)MonsterType.Imp), new ArrayList(), facing, MonsterType.Imp, lvl)
        {
            this.lvl = lvl;

            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 50));
            spritePosList.Add(new Vector2(0, 100));
            spritePosList.Add(new Vector2(0, 150));
            spritePosList.Add(new Vector2(300, 100));
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

                if (!hasKey)
                {
                    GameObject[] keyLocation = lvl.getAllBlockOfType(Block.BlockType.Key).ToArray();

                    if (path.Count == 0 || stepCount == path.Count || stepCount == 2)
                    {
                        stepCount = 0;

                        List<List<Direction>> pathList = new List<List<Direction>>();

                        for (int i = 0; i < keyLocation.Length; i++)
                            pathList.Add(aStar.getPath(this, keyLocation[i], lvl));

                        foreach (List<Direction> list in pathList)
                        {
                            Direction lastDir = list.FindLast(
                                delegate(Direction dir)
                                {
                                    return dir == Direction.Neutral;
                                });

                            //If the key cannot be reached.
                            if (lastDir != Direction.Neutral) continue;

                            //If the path is not yet set.
                            if (path.Count == 0) path = list;

                            //If the next path is shorter than the last.
                            if (path.Count > list.Count) path = list;
                        }

                        //If path has not been set by now, move in a random direction.
                        if (path.Count == 0)
                        {
                            int dir = gen.Next(4);
                            if (lvl.canEnter(this, position - sides[dir], (Direction)dir))
                                move((Direction)dir);

                            return;
                        }
                    }

                    if (path.Count > 0)
                    {
                        Direction toDir = (Direction)path.First<Direction>();
                        if(toDir != Direction.Neutral)
                            move(toDir);
                        path.Remove(toDir);
                    }
                }
                else
                {
                    double maxDist = 0;
                    Direction sendTo = Direction.Up;

                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 loc = position - sides[i];

                        if (!lvl.canEnter(this, loc, (Direction)i)) continue;

                        double distFrom = aStar.GetEstimate(lvl.getPlayer().getPosition(), loc);

                        if (distFrom > maxDist)
                        {
                            sendTo = (Direction)i;
                            maxDist = distFrom;
                        }
                    }
                    move(sendTo);
                }

            }
        }

        public void giveItem(GameObject.ItemCode key, Vector2 location)
        {
            this.key = key;
            hasKey = true;
            myKeyLocation = new Vector2(location.X, location.Y);
            stepCount = 2;
            path.Clear();
        }

        /// <summary>
        /// Handle the player entering this monster.
        /// </summary>
        /// <param name="obj">The object entering</param>
        public override void onEntered(GameObject obj)
        {
            if (obj.getType() == Type.Player && hasKey)
            {
                hasKey = false;
                lvl.getPlayer().giveItem(key);
            }
            else if (obj.getType() == Type.Monster)
            {
                Monster m = (Monster)obj;
                if (m.getMonsterType() == MonsterType.Arrow)
                {
                    m.onEntered(this);
                }
            }
        }

        public void onDeath()
        {
            if (!hasKey) return;

            Direction newFacing = Direction.Up;
            switch (key)
            {
                case ItemCode.KeyBlue:
                    newFacing = Direction.Right;
                    break;
                case ItemCode.KeyYellow:
                    newFacing = Direction.Down;
                    break;
                case ItemCode.KeyGreen:
                    newFacing = Direction.Left;
                    break;
            }

            BlockSave newBlock = new BlockSave();
            newBlock.blockType = Block.BlockType.Key;
            newBlock.facing = newFacing;
            newBlock.position = myKeyLocation;
            GameObject block = lvl.createBlock(newBlock);
            lvl.setBlock(myKeyLocation, block);
        }

        public override bool canEnter(GameObject obj, GameObject.Direction dir, int depth)
        {
            if (obj.getType() == Type.Monster)
                if (((Monster)obj).getMonsterType() == MonsterType.Arrow)
                    return true;

            return base.canEnter(obj, dir, 0);
        }

        /// <summary>
        /// Draw this game object to the window.
        /// </summary>
        /// <param name="spriteBatch">The place to draw the sprite to.</param>
        /// <param name="pos">Where to place</param>
        public override void draw(SpriteBatch spriteBatch, Vector2 pos)
        {
            int posx = (int)pos.X;
            int posy = (int)pos.Y;

            if (transition > 0.0f)
            {
                if (facing == Direction.Left || facing == Direction.Right)
                {
                    posx = (int)(pos.X + transition * lvl.TILEDIMENSIONS.X * lvl.getDirectionMultiplier(facing));
                }
                if (facing == Direction.Up || facing == Direction.Down)
                {
                    posy = (int)(pos.Y + transition * lvl.TILEDIMENSIONS.Y * lvl.getDirectionMultiplier(facing));
                }
            }

            Rectangle dest = new Rectangle(posx, posy, (int)lvl.TILEDIMENSIONS.X, (int)lvl.TILEDIMENSIONS.Y);
            Vector2 sourcePos = (Vector2)spritePosList[spriteIndex];
            Rectangle source;
            if (animatedFrames[(int)facing] != 0)
            {
                source = new Rectangle((int)(sourcePos.X + ((int)curFrame * dimensions.X)), (int)sourcePos.Y, (int)dimensions.X, (int)dimensions.Y);
            }
            else
            {
                source = new Rectangle((int)(sourcePos.X), (int)sourcePos.Y, (int)dimensions.X, (int)dimensions.Y);
            }

            if (!hasKey)
                spriteBatch.Draw(sprites, dest, source, Color.White);
            else
            {
                switch (key)
                {
                    case ItemCode.KeyRed:
                        spriteBatch.Draw(sprites, dest, source, Color.Red);
                        break;
                    case ItemCode.KeyBlue:
                        spriteBatch.Draw(sprites, dest, source, Color.Blue);
                        break;
                    case ItemCode.KeyYellow:
                        spriteBatch.Draw(sprites, dest, source, Color.Yellow);
                        break;
                    case ItemCode.KeyGreen:
                        spriteBatch.Draw(sprites, dest, source, Color.Green);
                        break;
                }
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
