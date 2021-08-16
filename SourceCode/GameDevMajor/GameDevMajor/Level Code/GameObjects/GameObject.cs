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
    public class GameObject
    {
        #region Enum definitions
        /// <summary>
        /// A direction can be up, down, left, right, or neutral. 
        /// The direction property is mainly referring to the stance of 
        /// the monsters and player. But it is also used as a state variable
        /// for up to 5 states that different blocks can have.
        /// </summary>
        public enum Direction { Up = 0, Right = 1, Down = 2, Left = 3, Neutral = 4 };

        /// <summary>
        /// The various items that can be picked up and collected in inventory.
        /// Keys are for doors, and the other items allow for walking on different surfaces.
        /// </summary>
        public enum ItemCode { KeyRed = 0, KeyBlue, KeyYellow, KeyGreen, FireBoots, Flippers, Suction, IceBoots };
        
        /// <summary>
        /// The four types of game object are Block that are static, monsters that may move by themselves,
        /// and the player that is controlled by the players actions. The last type is Cursor that is for the 
        /// editor mode.
        /// </summary>
        public enum Type { Block = 0, Monster, Player, Cursor };
        #endregion

        #region Instance variables
        /// <summary>
        /// A Texture2D that can contain 1 or more sprites to use for the object.
        /// </summary>
        protected Texture2D sprites;

        /// <summary>
        /// Locations of each of the left corners of sprites (each sprite should have same width/height.
        /// This contains a list of Vector2s.
        /// </summary>
        protected ArrayList spritePosList;

        /// <summary>
        /// The sprite to use from the spritePosList
        /// </summary>
        protected int spriteIndex;

        /// <summary>
        /// Allows each sprite to have a horizontally aligned set of additional frames
        /// Frames are auto offset from the spriteIndex matching the sriteIndex of the spritePosList
        /// </summary>
        protected int[] animatedFrames;

        /// <summary>
        /// The current frame.
        /// </summary>
        protected float curFrame;

        /// <summary>
        /// The time spent on each frame.
        /// </summary>
        protected float frameTime;

        /// <summary>
        /// Whether the thing should be animating
        /// </summary>
        protected bool animate;

        /// <summary>
        /// Where on the map of tiles this object is located.
        /// </summary>
        protected Vector2 position;
        
        /// <summary>
        /// The state of the object. For tiles it may be a rotation, 
        /// but for monsters and players in particular it will be a 
        /// specification of the image to be displayed.
        /// </summary>
        protected Direction facing;

        /// <summary>
        /// A pointer to the level object.
        /// </summary>
        protected Level lvl;

        /// <summary>
        /// The dimensions of the object.
        /// </summary>
        protected Vector2 dimensions;

        /// <summary>
        /// The type of object. (Block/Monster/Player)
        /// </summary>
        protected Type type;

        /// <summary>
        /// Defines whether a general monster is allowed to enter from a particular direction.
        /// </summary>
        protected bool[] canEnterMonster;

        /// <summary>
        /// Defines whether a player is allowed to enter from a particular direction.
        /// </summary>
        protected bool[] canEnterPlayer;
        #endregion

        /// <summary>
        /// Creates a standard Game Object.
        /// </summary>
        /// <param name="position">The position to place the object.</param>
        /// <param name="type">The type of object (a block/monster/player).</param>
        /// <param name="sprites">The one or more sprite elements that will be used to draw the object.</param>
        /// <param name="spritePosList">The list of positions of sprites if there is more than one.</param>
        /// <param name="dimensions">The dimensions of the sprites to be drawn from the sprites image.</param>
        /// <param name="facing">The direction or state for the object to start with.</param>
        /// <param name="lvl">The pointer to the level object.</param>
        public GameObject(Vector2 position, Type type, Texture2D sprites, ArrayList spritePosList, Vector2 dimensions, Direction facing, Level lvl)
        {
            this.position = position;
            this.type = type;
            this.dimensions = dimensions;
            this.spritePosList = spritePosList;
            this.sprites = sprites;
            this.facing = facing;
            this.lvl = lvl;
            setSprite(facing);
            canEnterMonster = new bool[4];
            canEnterPlayer = new bool[4];
            for (int i = 0; i < 4; i++)
            {
                canEnterPlayer[i] = true;
                canEnterMonster[i] = true;
            }

            animatedFrames = new int[5];
            frameTime = 50;
            curFrame = 0;
            animate = true;
        }

        #region Set sprite methods
        /// <summary>
        /// Set the sprite to the specified sprite position.
        /// </summary>
        /// <param name="spriteID">The array id to get the sprite coordinates.</param>
        protected void setSprite(int spriteID)
        {
            // Debug.Assert(spritePosList.Count >= spriteID, "A Sprite ID set was attempted that does not exist!!");
            //if (spritePosList.Count > spriteID)
            //{
                spriteIndex = spriteID;
            //}
                curFrame = 0;
        }

        /// <summary>
        /// Set the sprite using a Direction variable instead of the int.
        /// </summary>
        /// <param name="dir">A direction for the facing to be assumed as.</param>
        protected void setSprite(Direction dir)
        {
            setSprite((int)dir);

        }
        #endregion

        #region Set methods
        /// <summary>
        /// This method will set which directions the player can enter this square from.
        /// </summary>
        /// <param name="canEnter">The boolean checks for if a direction is allowed.</param>
        protected void setCanEnterPlayer(bool[] canEnter)
        {
            for (int i = 0; i < 4; i++)
            {
                canEnterPlayer[i] = canEnter[i];
            }
        }

        /// <summary>
        /// This method will set which directions a monster can enter this square from.
        /// </summary>
        /// <param name="canEnter">The boolean checks for if a direction is allowed.</param>
        protected void setCanEnterMonster(bool[] canEnter)
        {
            for (int i = 0; i < 4; i++)
            {
                canEnterMonster[i] = canEnter[i];
            }
        }

        /// <summary>
        /// Set it so that no player can enter this object.
        /// </summary>
        /// <param name="setting">The boolean checks for if a direction is allowed.</param>
        protected void setCanEnterPlayerAll(bool setting)
        {
            for (int i = 0; i < 4; i++)
            {
                canEnterPlayer[i] = setting;
            }
        }

        /// <summary>
        /// Set it so that no monster can enter this object.
        /// </summary>
        /// <param name="setting">The boolean checks for if a direction is allowed.</param>
        protected void setCanEnterMonsterAll(bool setting)
        {
            for (int i = 0; i < 4; i++)
            {
                canEnterMonster[i] = setting;
            }
        }

        /// <summary>
        /// Set the position of the object to a new coordinate.
        /// </summary>
        /// <param name="pos">The position to place the object at.</param>
        public void setPosition(Vector2 pos)
        {
            this.position = pos;
        }

        /// <summary>
        /// Set the number of frames for all the different animations.
        /// Framecounts MUST have 5 elements.
        /// </summary>
        /// <param name="frameCounts">Number of frames for each facing.</param>
        protected void setAnimatedFrames(int[] frameCounts)
        {
            Debug.Assert(frameCounts.Length == animatedFrames.Length, "ERROR: Invalid frame count!");

            for (int i = 0; i < animatedFrames.Length; i++)
            {
                animatedFrames[i] = frameCounts[i];
            }
        }

        /// <summary>
        /// Set the count for a specific facing.
        /// </summary>
        /// <param name="dir">The direction to set.</param>
        /// <param name="frameCount">The count.</param>
        protected void setAnimatedFrames(Direction dir, int frameCount)
        {
            animatedFrames[(int)dir] = frameCount;
        }

        /// <summary>
        /// The frameTime to use in milliseconds for animation.
        /// </summary>
        /// <param name="frameTime">The time in milliseconds.</param>
        protected void setFrameTime(float frameTime)
        {
            this.frameTime = frameTime;
        }

        /// <summary>
        /// Change whether currently animating
        /// </summary>
        /// <param name="animate">Set whether animation is happening</param>
        protected void setAnimate(bool animate)
        {
            this.animate = animate;
        }
        #endregion

        #region Get State Variables
        /// <summary>
        /// Gets the current direction of facing or state of the object.
        /// </summary>
        /// <returns>The current direction or state.</returns>
        public Direction getFacing()
        {
            return facing;
        }

        /// <summary>
        /// Gets the type of the object (block/monster/player)
        /// </summary>
        /// <returns>The object type.</returns>
        public Type getType()
        {
            return type;
        }

        /// <summary>
        /// Gets the position of the object.
        /// </summary>
        /// <returns>The objects position.</returns>
        public Vector2 getPosition()
        {
            return this.position;
        }

        /// <summary>
        /// Gets whether currently animating
        /// </summary>
        /// <returns></returns>
        public bool getisAnimating()
        {
            return animate;
        }
        #endregion

        #region Core Methods that are Overideable (facing, update, draw)
        /// <summary>
        /// Set the facing of this particular game object and update the sprite.
        /// </summary>
        /// <param name="facing">The new direction of face.</param>
        public virtual void setFacing(Direction facing)
        {
            this.facing = facing;
            setSprite(facing);
        }

        /// <summary>
        /// Defines an overideable update method so that every game object 
        /// includes it for mass calling.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public virtual void update(GameTime gameTime)
        {
            if (animatedFrames[(int)facing] != 0 && animate)
            {
                curFrame += gameTime.ElapsedGameTime.Milliseconds / frameTime;
                if ((int)curFrame >= animatedFrames[(int)facing]) curFrame = 0;
            }
        }

        /// <summary>
        /// Draw this game object to the window.
        /// </summary>
        /// <param name="spriteBatch">The place to draw the sprite to.</param>
        /// <param name="pos">Where to place</param>
        public virtual void draw(SpriteBatch spriteBatch, Vector2 pos)
        {
            Rectangle dest = new Rectangle((int)pos.X, (int)pos.Y, 50, 50);
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
        #endregion

        #region Virtual Events that may be overridden to handle interactions
        /// <summary>
        /// This event is triggered when another object is moving into 
        /// this object.
        /// </summary>
        /// <param name="obj">The object that is moving.</param>
        /// <param name="dir">The direction they are entering from.</param>
        public virtual void onEntering(GameObject obj, Direction dir)
        {

        }

        /// <summary>
        /// This event is triggered once entry into the object is completed.
        /// </summary>
        /// <param name="obj">The object that has now entered this object's area.</param>
        public virtual void onEntered(GameObject obj)
        {

        }

        /// <summary>
        /// This event is triggered as an object is exiting the object.
        /// </summary>
        /// <param name="obj">The object exiting.</param>
        /// <param name="dir">The direction that they are exiting to.</param>
        public virtual void onExiting(GameObject obj, Direction dir)
        {

        }

        /// <summary>
        /// This event is triggered after an object has left this object.
        /// </summary>
        /// <param name="obj">The object that has exited.</param>
        public virtual void onExited(GameObject obj)
        {

        }

        /// <summary>
        /// Check if the object obj can enter this element from direction dir.
        /// Depth allows for composite advanced checks. A non-zero value will 
        /// allow recursive calls that test multiple squares in direction dir.
        /// Each successive call decreases depth by 1. The call is terminated if 
        /// a square is found to have false entry.
        /// This is not implemented by this virutal function, but it is the purpose
        /// that is will contain within the relevant classes inheriting.
        /// </summary>
        /// <param name="obj">The object wanting to test for entry.</param>
        /// <param name="dir">The direction they wish to enter from.</param>
        /// <param name="depth">The maximum depth check for a composite entry test.</param>
        /// <returns>Can the object enter the cell.</returns>
        public virtual bool canEnter(GameObject obj, Direction dir, int depth)
        {
            if (/*depth == 0 &&*/ obj.getType() == GameObject.Type.Player)
            {
                return canEnterPlayer[(int)dir];
            }
            else /*if (depth == 0)*/
            {
                return canEnterMonster[(int)dir];
            }
            /*else
            {
                if (this.type != GameObject.Type.Block)
                {
                    // determine the next square
                    Vector2 pos = this.position;
                    switch (dir)
                    {
                        case GameObject.Direction.Up:
                            pos.Y -= 1;
                            break;
                        case GameObject.Direction.Down:
                            pos.Y += 1;
                            break;
                        case GameObject.Direction.Left:
                            pos.X -= 1;
                            break;
                        case GameObject.Direction.Right:
                            pos.X += 1;
                            break;
                    }

                    // pass the call onto the next chain
                    return this.canEnter(obj, dir) && lvl.canEnter(this, pos, dir, --depth);
                }
                else
                {
                    // just use the state of this block as the determinate
                    return this.canEnter(obj, dir);
                }
            }*/
        }

        /// <summary>
        /// Checks if the object obj can enter this element from direction dir.
        /// </summary>
        /// <param name="obj">The object wanting to test for entry.</param>
        /// <param name="dir">The direction they wish to enter from.</param>
        /// <returns>Can the object enter the cell.</returns>
        public virtual bool canEnter(GameObject obj, Direction dir)
        {
            return canEnter(obj, dir, 0);
        }

        /// <summary>
        /// Used to check if the object currently on the square can exit the square.
        /// </summary>
        /// <param name="obj">The object on this object.</param>
        /// <param name="dir">The direction to leave.</param>
        /// <returns></returns>
        public virtual bool canExit(GameObject obj, Direction dir)
        {
            return canEnter(obj, dir);
        }
        #endregion
    }
}
