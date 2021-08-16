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

namespace GameDevMajor
{
    public class Player : GameObject
    {
        #region Instance Variables
        /// <summary>
        /// The inventory of the player.
        /// </summary>
        protected ArrayList items;

        /// <summary>
        /// The progress of movement to another cell
        /// </summary>
        protected float transition;

        /// <summary>
        /// A multiplier value that will make the object move faster or slower.
        /// </summary>
        protected float speedMultiplier;

        /// <summary>
        /// The cell that is being transitioned to.
        /// </summary>
        protected Vector2 targetCell;

        /// <summary>
        /// Used for an automatic chained movement so allow everything to fluidly update.
        /// </summary>
        protected Vector2 chainTargetCell;

        /// <summary>
        /// Whether the player is allowed to input control to the character.
        /// </summary>
        protected bool enableInputControl;

        /// <summary>
        /// A counter to manage the time taken
        /// </summary>
        protected float moveDelay;

        /// <summary>
        /// The delay time that must be waited between moves.
        /// </summary>
        protected float moveDelayTime;
        #endregion

        public Player(Vector2 pos, Level lvl)
            : base(pos, GameObject.Type.Player, lvl.getSprite(GameObject.Type.Player, 0), new ArrayList(), new Vector2(50.0f, 50.0f), GameObject.Direction.Neutral, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));
            spritePosList.Add(new Vector2(0, 50));
            spritePosList.Add(new Vector2(0, 100));
            spritePosList.Add(new Vector2(0, 150));
            spritePosList.Add(new Vector2(300, 100));
            setAnimatedFrames(new int[] { 8, 8, 8, 8, 0 });

            transition = 0.0f;
            speedMultiplier = 1.0f;
            items = new ArrayList();
            enableInputControl = true;
            targetCell = new Vector2(-1, -1);
            chainTargetCell = new Vector2(-1, -1);

            moveDelay = 0;
            moveDelayTime = 100;
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);

            if (isMoving())
            {
                transition += gameTime.ElapsedGameTime.Milliseconds / 250.0f * speedMultiplier;

                if (transition >= 1.0f)
                {
                    completeMoveTo();
                }
            }

            if (enableInputControl)
            {
                KeyboardState kbState = lvl.getKeyboardState();
                GamePadState gpState = lvl.getGamePadState();

                moveDelay += gameTime.ElapsedGameTime.Milliseconds;
                if (moveDelay > moveDelayTime)
                {
                    moveDelay = moveDelayTime;

                    if ((kbState.IsKeyDown(Keys.Up) || gpState.ThumbSticks.Left.Y > 0.5) && position.Y > 0)
                    {
                        moveDelay = 0;
                        move(Direction.Up);
                    }
                    else if ((kbState.IsKeyDown(Keys.Down) || gpState.ThumbSticks.Left.Y < -0.5) && position.Y < lvl.getMapSize() - 1)
                    {
                        moveDelay = 0;
                        move(Direction.Down);
                    }
                    else if ((kbState.IsKeyDown(Keys.Left) || gpState.ThumbSticks.Left.X < -0.5) && position.X > 0)
                    {
                        moveDelay = 0;
                        move(Direction.Left);
                    }
                    else if ((kbState.IsKeyDown(Keys.Right) || gpState.ThumbSticks.Left.X > 0.5) && position.X < lvl.getMapSize() - 1)
                    {
                        moveDelay = 0;
                        move(Direction.Right);
                    }
                    else
                    {
                        setFacing(Direction.Neutral);
                    }
                }

            }
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
            spriteBatch.Draw(sprites, dest, source, Color.White);
        }

        public Vector2 getTransitionOffset()
        {
            Vector2 result = Vector2.Zero;

            if (facing == Direction.Left || facing == Direction.Right)
            {
                result.X = (int)(transition * lvl.TILEDIMENSIONS.X * lvl.getDirectionMultiplier(facing));
            }
            if (facing == Direction.Up || facing == Direction.Down)
            {
                result.Y = (int)( transition * lvl.TILEDIMENSIONS.Y * lvl.getDirectionMultiplier(facing));
            }

            return result;
        }

        #region Event Handlers
        public override void onEntered(GameObject obj)
        {
            base.onEntered(obj);

            if (obj.getType() == Type.Monster)
            {
                Monster m = (Monster)obj;
                switch (m.getMonsterType())
                {
                    case Monster.MonsterType.Armor:
                        lvl.failLevel("Beware standing too long near statues!");
                        break;
                    case Monster.MonsterType.Assassin:
                        lvl.failLevel("He hunts endlessly! Try to run faster.");
                        break;
                    case Monster.MonsterType.PatrolMonster:
                        lvl.failLevel("Stand aside! The guards don't take prisoners!");
                        break;
                    case Monster.MonsterType.Rat:
                        lvl.failLevel("The vermin are everywhere!");
                        break;
                    case Monster.MonsterType.Arrow:
                        lvl.failLevel("BOOM HEADSHOT!.. You may want to duck next time.");
                        break;
                    case Monster.MonsterType.Ice:
                        lvl.failLevel("Frozen in time. Beware of them ice prisions");
                        break;
                    case Monster.MonsterType.Moveable:
                        lvl.failLevel("Squished by 1000 tonnes of cold hard steel");
                        break;
                    case Monster.MonsterType.Imp:
                        m.onEntered(lvl.getPlayer());
                        break;
                }
            }
        }
        #endregion

        #region Player State Modifiers
        public void setCanInput(bool canInput)
        {
            enableInputControl = canInput;
        }

        public bool getCanInput()
        {
            return enableInputControl;
        }
        #endregion

        #region Movement Modifiers
        /// <summary>
        /// Attempt to initiate movement in the specified direction.
        /// This may fail, it there is something that is blocking the path.
        /// </summary>
        /// <param name="dir">The direction to move the player.</param>
        protected void move(GameObject.Direction dir)
        {
            setFacing(dir);
            lvl.moveObject(this, dir);
        }

        /// <summary>
        /// Initiate a successful move call by setting the target cell and
        /// disabling player input. The player will now move by itself until 
        /// set otherwise.
        /// </summary>
        /// <param name="to">The cell that is being moved to.</param>
        public void beginMoveTo(Vector2 to)
        {

            if (targetCell.X == -1)
            {
                if (!checkValidMove(getPosition(), to)) return;
                targetCell = to;
            }
            else
            {
                if (!checkValidMove(targetCell, to)) return;
                chainTargetCell = to;
            }
            setCanInput(false);
        }

        public bool beginCheckMoveTo(Vector2 to)
        {

            if (targetCell.X == -1)
            {
                if (!checkValidMove(getPosition(), to)) return false;
            }
            else
            {
                if (to.Equals(chainTargetCell) || !checkValidMove(targetCell, to)) return false;
            }
            return true;
        }

        /// <summary>
        /// This method is a "fix" XD
        /// </summary>
        /// <param name="from">Cell 1</param>
        /// <param name="to">Cell 2</param>
        /// <returns>Movement valid.</returns>
        private bool checkValidMove(Vector2 from, Vector2 to)
        {
            return true;

            /*Vector2 diff = from - to;
            int absX = (int)((diff.X < 0) ? -diff.X : diff.X);

            int absY = (int)((diff.Y < 0) ? -diff.Y : diff.Y);

            // hur hur hur this fixes stuff
            if ((absX == 1 && absY == 1) || (absX == 0 && absY == 0) || absX > 1 || absY > 1)
                return false;

            return true;*/
        }

        /// <summary>
        /// Complete the process of moving to a particular cell.
        /// This includes triggering of the relevant events.
        /// </summary>
        protected void completeMoveTo()
        {
            setCanInput(true);
            // trigger exited
            lvl.triggerOnExited(position, this);

            position.X = targetCell.X;
            position.Y = targetCell.Y;
            transition = 0.0f;

            // trigger entered
            lvl.triggerOnEntered(this);

            if (chainTargetCell.X == -1)
            {
                targetCell = new Vector2(-1, -1);
            }
            else
            {
                targetCell = chainTargetCell;
                chainTargetCell = new Vector2(-1, -1);
                setCanInput(false);
            }
        }

        /// <summary>
        /// Set the speed multiplier for this object to a higher or lower value.
        /// </summary>
        /// <param name="multiplier">The value to make the speed.</param>
        public void setSpeedMulti(float multiplier)
        {
            speedMultiplier = multiplier;
        }

        /// <summary>
        /// Reset the speed multiplier to the default value.
        /// </summary>
        public void resetSpeedMultiplier()
        {
            speedMultiplier = 1.0f;
        }

        /// <summary>
        /// Is the object moving?
        /// </summary>
        /// <returns>Whether the object is moving</returns>
        public bool isMoving()
        {
            return targetCell.X != -1 && targetCell.Y != -1;
        }
        #endregion

        #region Inventory Management
        /// <summary>
        /// Give the player a particular item for their inventory.
        /// </summary>
        /// <param name="item"></param>
        public void giveItem(ItemCode item)
        {
            items.Add(item);
        }

        /// <summary>
        /// Remove a single copy of an item form the inventory.
        /// </summary>
        /// <param name="item"></param>
        public void removeItem(ItemCode item)
        {
            items.Remove(item);
        }

        /// <summary>
        /// Removes the tile specific equipment on the player.
        /// </summary>
        public void removeEquipment()
        {
            while (items.Contains(ItemCode.FireBoots))
            {
                items.Remove(ItemCode.FireBoots);
            }

            while (items.Contains(ItemCode.Flippers))
            {
                items.Remove(ItemCode.Flippers);
            }

            while (items.Contains(ItemCode.IceBoots))
            {
                items.Remove(ItemCode.IceBoots);
            }

            while (items.Contains(ItemCode.Suction))
            {
                items.Remove(ItemCode.Suction);
            }
        }

        /// <summary>
        /// Remove all items from the players inventory.
        /// </summary>
        public void removeAllItems()
        {
            items.Clear();
        }

        /// <summary>
        /// Check if the player has a particular item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool hasItem(ItemCode item)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// Counts how many of a particular item the player has.
        /// </summary>
        /// <param name="item">The item to count.</param>
        /// <returns>The number of times the item specified appears in the players inventory.</returns>
        public int countItem(ItemCode item)
        {
            int result = 0;
            foreach (ItemCode c in items)
            {
                if (c.Equals(item))
                {
                    result++;
                }
            }
            return result;
        }
        #endregion
    }
}
