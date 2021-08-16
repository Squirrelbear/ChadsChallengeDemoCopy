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

namespace GameDevMajor
{
    public class Monster : GameObject
    {
        #region Instance Variables
        /// <summary>
        /// The enum collection of different monster types.
        /// </summary>
        public enum MonsterType { SampleMonster = 0, Armor = 1, PatrolMonster = 2, Imp = 3, Assassin = 4, Rat = 5, Moveable = 6, Ice = 7, Arrow = 8 };

        /// <summary>
        /// The type of monster this instance is.
        /// </summary>
        private MonsterType monsterType;

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
        /// A counter to manage the time taken
        /// </summary>
        protected float moveDelay;

        /// <summary>
        /// The delay time that must be waited between moves.
        /// </summary>
        protected float moveDelayTime;
        #endregion

        public Monster(Vector2 pos, Texture2D sprites, ArrayList spritePosList, Direction dir, MonsterType monsterType, Level lvl)
            : base(pos, GameObject.Type.Monster, sprites, spritePosList, new Vector2(50.0f, 50.0f), dir, lvl)
        {
            this.monsterType = monsterType;
            transition = 0.0f;
            speedMultiplier = 1.0f;
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
        }
        
        /// <summary>
        /// Get the type of monster.
        /// </summary>
        /// <returns>The type of monster.</returns>
        public MonsterType getMonsterType()
        {
            return monsterType;
        }

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

            /*
            Vector2 diff = from - to;
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
            }
        }

        /// <summary>
        /// Get the current target cell
        /// </summary>
        /// <returns>Target cell</returns>
        public Vector2 getTargetCell()
        {
            return targetCell;            
        }


        /// <summary>
        /// Get the chained target cell
        /// </summary>
        /// <returns>Chained target cell</returns>
        public Vector2 getChainedTargetCell()
        {
            return chainTargetCell;
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

        /// <summary>
        /// Generate a standard save state for monsters.
        /// </summary>
        /// <returns></returns>
        public virtual MonsterSave createSaveState()
        {
            MonsterSave state = new MonsterSave();
            state.monsterType = monsterType;
            state.facing = facing;
            state.position = position;
            return state;
        }
    }
}
