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
    public class Cursor : GameObject
    {
        private enum SelectMode { Normal, Single, MultiTarget, Fill };

        #region Instance Variables
        /// <summary>
        /// A temporary block save so that parameters may be added if needed.
        /// </summary>
        BlockSave tempBlockSave;

        /// <summary>
        /// A temporary monster save so that parameters may be added if needed.
        /// </summary>
        MonsterSave tempMonsterSave;

        /// <summary>
        /// A pointer to the editor object.
        /// </summary>
        LevelEditor editor;

        /// <summary>
        /// The counter for checking delay time between moves.
        /// </summary>
        private float moveDelay;
        
        /// <summary>
        /// The current mode if the cursor.
        /// </summary>
        private SelectMode mode;

        /// <summary>
        /// The varList containing the list
        /// </summary>
        private ArrayList varList;

        /// <summary>
        /// Status messagea about mode.
        /// </summary>
        private string statusMessage;

        /// <summary>
        /// The font to use for status messages
        /// </summary>
        private SpriteFont font;
        #endregion

        /// <summary>
        /// Creates a new instance of the Cursor object.
        /// </summary>
        /// <param name="pos">The position to place the cursor initially</param>
        /// <param name="sprites">The cursor sprite.</param>
        /// <param name="lvl">Pointer to the level object.</param>
        /// <param name="editor">Pointer to the editor object.</param>
        internal Cursor(Vector2 pos, Texture2D sprites, Level lvl, LevelEditor editor)
            : base(pos, GameObject.Type.Cursor, lvl.getSprite(Type.Cursor, 0), new ArrayList(), new Vector2(50.0f, 50.0f), Direction.Up, lvl)
        {
            spritePosList.Add(new Vector2(0, 0));

            this.editor = editor;

            font = lvl.getGameRef().Content.Load<SpriteFont>("WndBaseContent/Fonts/mediumFont");
        }

        /// <summary>
        /// Update the cursor object.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void update(GameTime gameTime)
        {
            if (!editor.allowCursorMove()) return;

            KeyboardState kbState = lvl.getKeyboardState();
            GamePadState gpState = lvl.getGamePadState();

            moveDelay += gameTime.ElapsedGameTime.Milliseconds;

            if (moveDelay > 250)
            {
                moveDelay = 250;

                if ((kbState.IsKeyDown(Keys.Up) || gpState.ThumbSticks.Left.Y > 0.5) && position.Y > 0)
                {
                    moveDelay = 0;
                    position.Y--;
                } 
                if ((kbState.IsKeyDown(Keys.Down) || gpState.ThumbSticks.Left.Y < -0.5) && position.Y < lvl.getMapSize() - 1)
                {
                    moveDelay = 0;
                    position.Y++;
                }
                if((kbState.IsKeyDown(Keys.Left) || gpState.ThumbSticks.Left.X < -0.5) && position.X > 0)
                {
                    moveDelay = 0;
                    position.X--;
                }
                if ((kbState.IsKeyDown(Keys.Right) || gpState.ThumbSticks.Left.X > 0.5) && position.X < lvl.getMapSize() - 1)
                {
                    moveDelay = 0;
                    position.X++;
                }

                /*if (moveDelay == 0)
                {
                    Debug.Print("Cursor now at: " + position.X + "," + position.Y);
                }*/
            }

            if (!editor.allowCursorInput()) return;

            if((kbState.IsKeyDown(Keys.K) && !lvl.getLastKeyboardState().IsKeyDown(Keys.K) 
                || (!lvl.getLastGamePadState().IsButtonDown(Buttons.X) && gpState.IsButtonDown(Buttons.X)
                ) && mode == SelectMode.MultiTarget))
            {
                tempMonsterSave.variableData = varList;
                GameObject monster = lvl.createMonster(tempMonsterSave);
                lvl.addMonster(monster);
                mode = SelectMode.Normal;
                return;
            }

            if ((kbState.IsKeyDown(Keys.F) && !lvl.getLastKeyboardState().IsKeyDown(Keys.F)
                || (!lvl.getLastGamePadState().IsButtonDown(Buttons.X) && gpState.IsButtonDown(Buttons.X))))
            {
                if(mode == SelectMode.Normal) {
                    setMode(SelectMode.Fill, "Select a tile type to fill area on.");
                }
                else mode = SelectMode.Normal;
            }

            if(kbState.IsKeyDown(Keys.R) && !lvl.getLastKeyboardState().IsKeyDown(Keys.R) 
                || (!lvl.getLastGamePadState().IsButtonDown(Buttons.Y) && gpState.IsButtonDown(Buttons.Y)))
            {
                lvl.placePlayerStart(position);
            }

            if((kbState.IsKeyDown(Keys.Space) && !lvl.getLastKeyboardState().IsKeyDown(Keys.Space)) 
                || (!lvl.getLastGamePadState().IsButtonDown(Buttons.A) && gpState.IsButtonDown(Buttons.A)))
            {
                 if (mode == SelectMode.Single)
                 {              
                     if (tempBlockSave.blockType == Block.BlockType.Portal)
                     {
                         tempBlockSave.variableData = new ArrayList();
                         tempBlockSave.variableData.Add(position);
                         GameObject block = lvl.createBlock(tempBlockSave);
                         lvl.setBlock(tempBlockSave.position, block);

                         Block curBlock = lvl.getBlockAt(position);
                         if(curBlock.getBlockType() != Block.BlockType.Portal) {
                             tempBlockSave = new BlockSave(Block.BlockType.Portal, position, Direction.Up);
                         } else {
                             mode = SelectMode.Normal;
                         }
                     } 
                     else if(tempBlockSave.blockType == Block.BlockType.Trap || tempBlockSave.blockType == Block.BlockType.Bow || tempBlockSave.blockType == Block.BlockType.Spawner)
                     {
                         // disallow same location
                         if(position.Equals(tempBlockSave.position)) return;

                         // Create Button
                         BlockSave btn = new BlockSave(Block.BlockType.Button, position, Direction.Up);
                         btn.variableData = new ArrayList();
                         btn.variableData.Add(tempBlockSave.position);
                         btn.position.X = position.X;
                         btn.position.Y = position.Y;
                         GameObject btnBlock = lvl.createBlock(btn);
                         lvl.setBlock(position, btnBlock);

                         // place block
                         GameObject block = lvl.createBlock(tempBlockSave);
                         lvl.setBlock(tempBlockSave.position, block);

                         mode = SelectMode.Normal;
                     }       
            
                     return;
                 }
                 else if (mode == SelectMode.MultiTarget)
                 {
                     varList.Add(position);
                     return;
                 }

                switch (editor.getPlacementType())
                {
                    case Type.Block:
                        
                        tempBlockSave = editor.getSelectedBlock();

                        tempBlockSave.position.X = position.X;
                        tempBlockSave.position.Y = position.Y;

                        switch (tempBlockSave.blockType)
                        {
                           case Block.BlockType.Trap:
                                setMode(SelectMode.Single, "Select a place to have a button for the trap.");
                                return;
                            case Block.BlockType.Bow:
                                setMode(SelectMode.Single, "Select a place to have a button for the bow.");
                                return;
                            case Block.BlockType.Spawner:
                                setMode(SelectMode.Single, "Select a place to have a button for the rat spawner.");
                                return;
                            case Block.BlockType.Portal:
                                setMode(SelectMode.Single, "Select a place for a new portal or existing portal.");
                                return;
                        }

                        if(mode == SelectMode.Fill) 
                        {
                            Block targetInfo = lvl.getBlockAt(position);
                            Block.BlockType type = targetInfo.getBlockType();
                            Direction facing = targetInfo.getFacing();
                            performFill(position, type, facing);
                            mode = SelectMode.Normal;
                        } else {
                            GameObject block = lvl.createBlock(tempBlockSave);
                            lvl.setBlock(position, block);
                        }
                        break;

                    case Type.Monster:
                        tempMonsterSave = editor.getSelectedMonster();

                        tempMonsterSave.position.X = position.X;
                        tempMonsterSave.position.Y = position.Y;

                        if (tempMonsterSave.monsterType == Monster.MonsterType.PatrolMonster)
                        {
                            setMode(SelectMode.MultiTarget, "Select some points for patrolling. Press K or X (xbox) when you are done.");
                            return;
                        }

                        GameObject monster = lvl.createMonster(tempMonsterSave);
                        lvl.addMonster(monster);
                        break;
                }
            }

            if (kbState.IsKeyDown(Keys.Delete) && !lvl.getLastKeyboardState().IsKeyDown(Keys.Delete)
                || (!lvl.getLastGamePadState().IsButtonDown(Buttons.B) && gpState.IsButtonDown(Buttons.B)))
            {
                // if there is a monster, remove that. 
                // Otherwise remove the tile and place a basic tile.
                if (lvl.getMonsterAt(position) != null)
                {
                    lvl.removeMonstersAt(position);
                }
                else
                {
                    BlockSave tile = new BlockSave(Block.BlockType.Tile, position, Direction.Up);
                    GameObject newBlock = lvl.createBlock(tile);
                    lvl.setBlock(position, newBlock);
                }
            }
        }

        private void performFill(Vector2 position, Block.BlockType type, Direction facing)
        {
            // dissallow this to remove infinite loops
            if(type == tempBlockSave.blockType && facing == tempBlockSave.facing) return;

            // get the current target
            Block targetBlock = lvl.getBlockAt(position);
            if (targetBlock.getBlockType() != type || targetBlock.getFacing() != facing) return;

            // remove the current block
            tempBlockSave.position.X = position.X;
            tempBlockSave.position.Y = position.Y;
            GameObject block = lvl.createBlock(tempBlockSave);
            lvl.setBlock(position, block);

            // attempt to fill each other direction
            performFill(new Vector2(position.X - 1, position.Y), type, facing);
            performFill(new Vector2(position.X, position.Y - 1), type, facing);
            performFill(new Vector2(position.X + 1, position.Y), type, facing);
            performFill(new Vector2(position.X, position.Y + 1), type, facing);
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 pos)
        {
            base.draw(spriteBatch, pos);

            if(mode != SelectMode.Normal)
                spriteBatch.DrawString(font, statusMessage, new Vector2(10, 10), Color.Red);
        }

        private void setMode(SelectMode mode, string status)
        {
            this.mode = mode;
            varList = new ArrayList();
            this.statusMessage = status;
        }
    }
}
