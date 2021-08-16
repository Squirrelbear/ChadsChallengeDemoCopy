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
using Microsoft.Xna.Framework.Storage;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using GameDevMajor.Level_Code.LevelWnds;

namespace GameDevMajor
{
    public class Level
    {
        #region Enum/Constant definitions
        public enum GamePlayState { Playing, Paused, LevelPassed, LevelFailed };
        public Vector2 TILEDIMENSIONS = new Vector2(50, 50);
        #endregion

        #region Instance Variables
        /// <summary>
        /// Manager for the level content.
        /// </summary>
        protected ContentManager Content;

        /// <summary>
        /// The current state of gameplay.
        /// </summary>
        protected GamePlayState gamePlayState;

        /// <summary>
        /// Variable holding how large the map is in dimensions.
        /// This is how many tiles wide and high the map is. 
        /// Valid values include: 64, 128, 256
        /// </summary>
        protected int mapSize;

        /// <summary>
        /// Tiles is the collection of tiles that is in a 2D grid.
        /// </summary>
        protected GameObject[,] tiles;

        /// <summary>
        /// Monsters contains the set of monsters or some special 
        /// tiles that can move. This is really anything that can 
        /// move around, but isn't the player.
        /// </summary>
        protected ArrayList monsters;

        /// <summary>
        /// This is the player object and is what is used to control
        /// the players interacting and movement throughout the game.
        /// </summary>
        protected Player player;

        /// <summary>
        /// Score is the score that will be recieved for passing this
        /// level when it is completed.
        /// </summary>
        protected int score;

        /// <summary>
        /// The number of this level.
        /// </summary>
        protected int levelNumber;

        /// <summary>
        /// A string representing the name of the level.
        /// </summary>
        protected String levelName;

        /// <summary>
        /// The high scores that are for this level.
        /// </summary>
        internal LeaderBoard leaderboard;

        /// <summary>
        /// The number of items that must be collected
        /// before the object blocking the end position 
        /// to pass the level is removed.
        /// </summary>
        protected int itemsToCollect;

        /// <summary>
        /// The time to complete the level within.
        /// </summary>
        protected float countDown;

        /// <summary>
        /// The current progress of counting down.
        /// </summary>
        protected float currentCountDown;

        /// <summary>
        /// The number of collected items so far.
        /// </summary>
        protected int collectedItems;

        /// <summary>
        /// The sprites for all the block tiles.
        /// </summary>
        protected ArrayList blockSprites;

        /// <summary>
        /// The sprites for all the monster tiles.
        /// </summary>
        protected ArrayList monsterSprites;

        /// <summary>
        /// The sprites for the various states of player tile.
        /// </summary>
        protected Texture2D playerSprites;

        /// <summary>
        /// The sprites for the cursor
        /// </summary>
        protected Texture2D cursorSprites;

        /// <summary>
        /// Special collection of blocks that have been defined as "state blocks".
        /// Blocks should only be made state blocks when the block is needed to be 
        /// accessed as a collection of every item of that type of block. 
        /// 
        /// For example, when you press a toggle button it toggles all of the blocks
        /// of that type. So that would be defined as a stateblock.
        /// </summary>
        protected ArrayList stateBlocks;

        /// <summary>
        /// A factory object to create objects based on a series of parameters.
        /// </summary>
        protected GameObjFactory gameObjFactory;

        /// <summary>
        /// Is the level in editor mode or is it being played.
        /// </summary>
        protected bool isEditor;

        /// <summary>
        /// Level file name.
        /// </summary>
        protected string levelFile;

        /// <summary>
        /// The original state of the level.
        /// </summary>
        protected LevelSave savedLevel;

        /// <summary>
        /// The top left corner coordinate that is currently being displayed.
        /// </summary>
        protected Vector2 topLeftCorner;

        /// <summary>
        /// A cursor that may be used by the editor mode.
        /// </summary>
        protected Cursor cursor;

        /// <summary>
        /// A pointer to the game reference.
        /// </summary>
        protected Game1 gameRef;

        /// <summary>
        /// List of monsters that after monster updates have occured will be removed.
        /// </summary>
        protected ArrayList monstersToRemove;

        /// <summary>
        /// List of monsters that after monster updates have occured will be added
        /// </summary>
        protected ArrayList monstersToAdd;

        //bretts additions
        LevelSummary levelSummary;
        PauseMenu pauseMenu;
        LevelFailed levelFailed;
        HUD HUD;
        WindowCollection windowCollection;
        LevelEditor editor;
        //end

        /// <summary>
        /// An object to handle automatic dependency management
        /// </summary>
        protected DependencyCollection dependencyManager;

        /// <summary>
        /// Used for drawing when it is needed to be known (not a normal param)
        /// </summary>
        public float deltaTime;

        /// <summary>
        /// Number of lives that is used to calcuate a bonus based on number of deaths
        /// </summary>
        protected int lifeBonus;

        protected List<Vector2> otherMonsters;

        #endregion

        #region Constructors and Destructors
        /// <summary>
        /// Normal constructor for the level object. Used to load a current level
        /// file in either the editor mode or as a playable level.
        /// </summary>
        /// <param name="asEditor">Whether the level is in editor mode or not.</param>
        /// <param name="levelFile">The level file to load.</param>
        /// <param name="gameRef">The pointer to the main game.</param>
        public Level(bool asEditor, int levelNumber, Game1 gameRef)
        {

            this.gameRef = gameRef;
            this.isEditor = asEditor;
            this.levelFile = "Level_" + levelNumber + ".lvl";
            this.levelNumber = levelNumber;
            this.levelName = "Level_" + levelNumber;

            loadSprites();

            gameObjFactory = new GameObjFactory(this);
            stateBlocks = new ArrayList();
            monsters = new ArrayList();
            monstersToRemove = new ArrayList();
            monstersToAdd = new ArrayList();
            dependencyManager = new DependencyCollection(this);

            //bretts additions
            leaderboard = new LeaderBoard(levelName);
            windowCollection = new WindowCollection();
            //end

            //kanes additions
            otherMonsters = new List<Vector2>();
            //end

            if (levelFile != null)
            {
                loadLevelFromFile();
            }

            if (asEditor)
            {

                editor = new LevelEditor(getGameRef(), this, Content);
                cursor = new Cursor(player.getPosition(), playerSprites, this, editor);
                windowCollection.add(editor);

            }
            else
            {

                HUD = new HUD(this, Content, levelNumber);
                windowCollection.add(HUD);
            }



            this.gamePlayState = GamePlayState.Playing;
        }

        /// <summary>
        /// Constructor that should only be used when creating a new
        /// level from scratch. It will fill the level with default settings.
        /// </summary>
        /// <param name="newMapSize">The size of the new map.</param>
        /// <param name="gameRef">Pointer to the game.</param>
        public Level(int newMapSize, int levelNumber, Game1 gameRef)
        {
            this.gameRef = gameRef;
            this.isEditor = true;
            this.mapSize = newMapSize;

            loadSprites();
            gameObjFactory = new GameObjFactory(this);
            stateBlocks = new ArrayList();
            monsters = new ArrayList();
            monstersToRemove = new ArrayList();
            monstersToAdd = new ArrayList();
            dependencyManager = new DependencyCollection(this);

            this.levelNumber = levelNumber;
            this.levelName = "Level " + levelNumber;
            this.levelFile = "Level_" + levelNumber + ".lvl";

            //bretts addition
            windowCollection = new WindowCollection();

            //end

            createNewLevel(newMapSize);

            editor = new LevelEditor(getGameRef(), this, Content);
            cursor = new Cursor(new Vector2(5, 5), playerSprites, this, editor);

            this.gamePlayState = GamePlayState.Playing;
        }

        /// <summary>
        /// Free up the memory that was taken for use within the level.
        /// </summary>
        ~Level()
        {
            Content.Unload();
        }
        #endregion

        #region Saving and Loading Level
        /// <summary>
        /// Load serialized data from the level file.
        /// </summary>
        public void loadLevelFromFile()
        {
#if XBOX
            // based off of the code at: http://msdn.microsoft.com/en-us/library/bb203924.aspx
#else
            // based off of the code at: http://www.jonasjohn.de/snippets/csharp/xmlserializer-example.htm

            // create the serializer
            XmlSerializer SerializerObj = new XmlSerializer(typeof(LevelSave));

            // Create a new file stream for reading the XML file
            FileStream ReadFileStream = new FileStream(gameRef.getGamePath() + "\\Saves\\" + levelFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            Debug.Print("Reading level file: " + gameRef.getGamePath() + "\\Saves\\" + levelFile);

            // Load the object saved above by using the Deserialize function
            LevelSave LoadedObj = (LevelSave)SerializerObj.Deserialize(ReadFileStream);

            savedLevel = LoadedObj;

            // Cleanup
            ReadFileStream.Close();

            // set the default life bonus
            lifeBonus = 6;

            // load everything into memory from the save data
            resetLevel();
#endif
        }

        /// <summary>
        /// Serialize and save data to a level file.
        /// </summary>
        public void saveLevelToFile()
        {
            // create the savedLevel
            savedLevel = new LevelSave();
            savedLevel.storeMap(mapSize, tiles);
            savedLevel.setPlayerStartPosition(player.getPosition());
            savedLevel.storeMonsters(monsters);
            savedLevel.setLevelProperties(levelNumber, levelName, (int)countDown);

#if XBOX
            // based off of the code at: http://msdn.microsoft.com/en-us/library/bb203924.aspx
#else
            // based off of the code at: http://www.jonasjohn.de/snippets/csharp/xmlserializer-example.htm

            // Create a new XmlSerializer instance
            XmlSerializer SerializerObj = new XmlSerializer(typeof(LevelSave));

            // Create a new file stream to write the serialized object to a file
            TextWriter WriteFileStream = new StreamWriter(gameRef.getGamePath() + "\\Saves\\" + levelFile);
            SerializerObj.Serialize(WriteFileStream, savedLevel);

            // Cleanup
            WriteFileStream.Close();

            getGameRef().saveLevelDetails(levelNumber, levelName);
#endif
        }

        /// <summary>
        /// Set the level back to its original state.
        /// </summary>
        public void resetLevel()
        {
            updateLifeBonus();

            this.stateBlocks.Clear();
            this.collectedItems = 0;
            this.itemsToCollect = 0;
            this.monsters.Clear();
            this.otherMonsters.Clear();

            this.mapSize = savedLevel.getMapSize();
            this.tiles = new GameObject[mapSize, mapSize];

            for (int i = 0; i < savedLevel.getSavedMap().Count; i++)
            {
                GameObject obj = gameObjFactory.createObject(savedLevel.getSavedMap()[i]);
                setBlockNoRemove(obj.getPosition(), obj);
            }

            player = new Player(savedLevel.getPlayerStartPosition(), this);

            monsters = new ArrayList();
            monstersToAdd = new ArrayList();
            monstersToRemove = new ArrayList();
            for (int i = 0; i < savedLevel.getSavedMonsters().Count; i++)
            {
                GameObject obj = gameObjFactory.createObject(savedLevel.getSavedMonsters()[i]);
                addMonster(obj);
            }

            this.levelNumber = savedLevel.getLevelNumber();
            this.levelName = savedLevel.getLevelName();
            this.countDown = savedLevel.getCountDown();
            this.currentCountDown = savedLevel.getCountDown();
        }

        /// <summary>
        /// Generate a new level based on a specific map size.
        /// </summary>
        /// <param name="mapSize">The size to set the new level to.</param>
        public void createNewLevel(int mapSize)
        {
            // populate the map with an initial tile everywhere
            BlockSave wall = new BlockSave();
            wall.blockType = Block.BlockType.Wall;
            wall.facing = GameObject.Direction.Up;

            BlockSave basic = new BlockSave();
            basic.blockType = Block.BlockType.Tile;
            basic.facing = GameObject.Direction.Up;
            basic.variableData = new ArrayList();
            basic.variableData.Add(new Vector2(0, 0));

            tiles = new GameObject[mapSize, mapSize];
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (i == 0 || j == 0 || j == mapSize - 1 || i == mapSize - 1)
                    {
                        wall.position = new Vector2(i, j);
                        GameObject newObj = gameObjFactory.createObject(wall);
                        setBlockNoRemove(wall.position, newObj);
                    }
                    else
                    {
                        basic.position = new Vector2(i, j);
                        GameObject newObj = gameObjFactory.createObject(basic);
                        setBlockNoRemove(basic.position, newObj);
                    }
                }
            }

            monsters = new ArrayList();

            // create a player start position
            player = new Player(new Vector2(5, 5), this);

            topLeftCorner = new Vector2(0, 0);

            // configure other properties:
            countDown = currentCountDown = 60;

            saveLevelToFile();
        }
        #endregion

        #region Sprite Setup
        /// <summary>
        /// Load the sprites for all tile/monster/player assets into the arrays.
        /// </summary>
        protected void loadSprites()
        {
            Content = new ContentManager(gameRef.Content.ServiceProvider, gameRef.Content.RootDirectory);

            // populate each of the sprite array lists with the relevant Texture2Ds

            //Toggle = 0, Button = 1, Wall = 2, Invisible = 3, Dissapearing = 4, Tile = 5, Ice = 6, Fire = 7,
            //Gravel = 8, Theif = 9, Water = 10, Guard = 11, Door = 12, Key = 13, Item = 14, Coin = 15, Speed = 16, Red = 17, Yellow = 18,
            //Green = 19, Blue = 20, RedButton = 21, YellowButton = 22, GreenButton = 23, BlueButton = 24
            // Portal = 25, Armor = 26, Bomb = 27, Bow = 28, Spawner = 29, Trap = 30, Win = 31

            // load the block sprites
            blockSprites = new ArrayList();
            blockSprites.Add(Content.Load<Texture2D>("Tiles/wall")); // 0 - Toggle #DISABLED
            blockSprites.Add(Content.Load<Texture2D>("Tiles/button")); // 1 - Button
            blockSprites.Add(Content.Load<Texture2D>("Tiles/wall")); // 2 - Wall
            blockSprites.Add(Content.Load<Texture2D>("Tiles/basic")); // 3 - Invisible
            blockSprites.Add(Content.Load<Texture2D>("Tiles/wall")); // 4 - Dissapearing
            blockSprites.Add(Content.Load<Texture2D>("Tiles/basic")); // 5 - Tile
            blockSprites.Add(Content.Load<Texture2D>("Tiles/ice")); // 6 - Ice
            blockSprites.Add(Content.Load<Texture2D>("Tiles/fire")); // 7 - Fire
            blockSprites.Add(Content.Load<Texture2D>("Tiles/gravel")); // 8 - Gravel
            blockSprites.Add(Content.Load<Texture2D>("Tiles/thief")); // 9 - Thief
            blockSprites.Add(Content.Load<Texture2D>("Tiles/water")); // 10 - Water
            blockSprites.Add(Content.Load<Texture2D>("Tiles/guard")); // 11 - Guard
            blockSprites.Add(Content.Load<Texture2D>("Tiles/doors")); // 12 - Door
            blockSprites.Add(Content.Load<Texture2D>("Tiles/keys")); // 13 - Key
            blockSprites.Add(Content.Load<Texture2D>("Tiles/items")); // 14 - Item
            blockSprites.Add(Content.Load<Texture2D>("Tiles/coin")); // 15 - Coin
            blockSprites.Add(Content.Load<Texture2D>("Tiles/speed")); // 16 - Speed
            blockSprites.Add(Content.Load<Texture2D>("Tiles/red")); // 17 - Red 
            blockSprites.Add(Content.Load<Texture2D>("Tiles/yellow")); // 18 - Yellow
            blockSprites.Add(Content.Load<Texture2D>("Tiles/green")); // 19 - Green 
            blockSprites.Add(Content.Load<Texture2D>("Tiles/blue")); // 20 - Blue 
            blockSprites.Add(Content.Load<Texture2D>("Tiles/redButton")); // 21 - Red Button 
            blockSprites.Add(Content.Load<Texture2D>("Tiles/yellowButton")); // 22 - Yellow Button
            blockSprites.Add(Content.Load<Texture2D>("Tiles/greenButton")); // 23 - Green Button
            blockSprites.Add(Content.Load<Texture2D>("Tiles/blueButton")); // 24 - Blue Button
            blockSprites.Add(Content.Load<Texture2D>("Tiles/portal")); // 25 - Portal
            blockSprites.Add(Content.Load<Texture2D>("Monsters/armour")); // 26 - Armor 
            blockSprites.Add(Content.Load<Texture2D>("Tiles/bomb")); // 27 - Bomb
            blockSprites.Add(Content.Load<Texture2D>("Tiles/bow")); // 28 - Bow 
            blockSprites.Add(Content.Load<Texture2D>("Tiles/sewerexit")); // 29 - Spawner 
            blockSprites.Add(Content.Load<Texture2D>("Tiles/trap")); // 30 - Trap
            blockSprites.Add(Content.Load<Texture2D>("Tiles/wintile")); // 31 - Win 
            blockSprites.Add(Content.Load<Texture2D>("Tiles/fallingwall")); // 32 - Fallingwall

            // load the monster sprites
            monsterSprites = new ArrayList();
            monsterSprites.Add(Content.Load<Texture2D>("Monsters/Monster")); // 0 - SampleMonster #DISABLED
            monsterSprites.Add(Content.Load<Texture2D>("Monsters/armour")); // 1 - Armour
            monsterSprites.Add(Content.Load<Texture2D>("Monsters/patrol")); // 2 - PatrolMonster
            monsterSprites.Add(Content.Load<Texture2D>("Monsters/imp")); // 3 - Imp
            monsterSprites.Add(Content.Load<Texture2D>("Monsters/assassin")); // 4 - Assassin
            monsterSprites.Add(Content.Load<Texture2D>("Monsters/rat")); // 5 - Rat
            monsterSprites.Add(Content.Load<Texture2D>("Monsters/movingwall")); // 6 - Moveable block
            monsterSprites.Add(Content.Load<Texture2D>("Monsters/MovingIce")); // 7 - Ice moveable block
            monsterSprites.Add(Content.Load<Texture2D>("Monsters/arrow")); // 8 - Arrow block

            // load the player sprites
            playerSprites = Content.Load<Texture2D>("Player/playerSprites");

            // load the cursor sprites
            cursorSprites = Content.Load<Texture2D>("Player/cursor");
        }

        /// <summary>
        /// Get the sprite required for that the specified object/tile type.
        /// </summary>
        /// <param name="gotype">The type of game object.</param>
        /// <param name="objectSubtype">The block type/monster type/player set.</param>
        /// <returns>A set of sprites for that object type.</returns>
        public Texture2D getSprite(GameObject.Type gotype, int objectSubtype)
        {
            if (gotype == GameObject.Type.Block)
            {
                if (objectSubtype < blockSprites.Count)
                {
                    return (Texture2D)blockSprites[objectSubtype];
                }
            }
            else if (gotype == GameObject.Type.Monster)
            {
                if (objectSubtype < monsterSprites.Count)
                {
                    return (Texture2D)monsterSprites[objectSubtype];
                }
            }
            else if (gotype == GameObject.Type.Player)
            {
                return playerSprites;
            }
            else if (gotype == GameObject.Type.Cursor)
            {
                return cursorSprites;
            }

            return null;
        }
        #endregion

        #region Collect Object States
        /// <summary>
        /// Get a pointer to the player object.
        /// </summary>
        /// <returns>The player.</returns>
        public Player getPlayer()
        {
            return player;
        }

        /// <summary>
        /// Get the current keyboard state.
        /// </summary>
        /// <returns>The currents state of the keyboard.</returns>
        public KeyboardState getKeyboardState()
        {
            return gameRef.getKeyboardState();
        }

        /// <summary>
        /// Get the last state of the keyboard.
        /// </summary>
        /// <returns>The previous state of the keyboard.</returns>
        public KeyboardState getLastKeyboardState()
        {
            return gameRef.getLastKeyboardState();
        }

        /// <summary>
        /// Get the current state of the GamePad.
        /// </summary>
        /// <returns>The current state of the gamepad.</returns>
        public GamePadState getGamePadState()
        {
            return gameRef.getGamePadState();
        }

        /// <summary>
        /// Get the last state of the gamepad.
        /// </summary>
        /// <returns>The last state of the game pad.</returns>
        public GamePadState getLastGamePadState()
        {
            return gameRef.getLastGamePadState();
        }

        /// <summary>
        /// Get the map size.
        /// </summary>
        /// <returns>Gets the map size in use.</returns>
        public int getMapSize()
        {
            return mapSize;
        }

        /// <summary>
        /// Is the level an instance that is an editor.
        /// </summary>
        /// <returns>True if is an editor</returns>
        public bool getIsEditor()
        {
            return isEditor;
        }

        public LevelInfo getLevelInfo()
        {
            return new LevelInfo(levelNumber, levelName, true);
        }
        #endregion

        #region Update, Draw, and State Modifier
        /// <summary>
        /// Core update method for the level.
        /// It updates all the objects that need to be.
        /// </summary>
        /// <param name="gameTime">The current gameTime.</param>
        public void update(GameTime gameTime)
        {
            deltaTime = gameTime.ElapsedGameTime.Milliseconds;

            //bretts addition
            if (gamePlayState != GamePlayState.Paused && gamePlayState != GamePlayState.LevelFailed &&
                (!getKeyboardState().IsKeyDown(Keys.Escape) && getLastKeyboardState().IsKeyDown(Keys.Escape))
                || (!getGamePadState().IsButtonDown(Buttons.Back) && getLastGamePadState().IsButtonDown(Buttons.Back))
                || (!getGamePadState().IsButtonDown(Buttons.Start) && getLastGamePadState().IsButtonDown(Buttons.Start)))
            {
                pauseMenu = new PauseMenu(this, Content);
                windowCollection.add(pauseMenu);
                this.gamePlayState = GamePlayState.Paused;
            }

            windowCollection.Update(gameTime);
            //end

            if (gamePlayState == GamePlayState.LevelFailed || gamePlayState == GamePlayState.Paused) return;

            if (!isEditor)
            {
                // update the player
                player.update(gameTime);

                // update each of the monsters
                foreach (Monster m in monsters)
                {
                    m.update(gameTime);
                }
                processMonsterAdditions();
                processMonsterRemovals();

                // re-process the positions
                otherMonsters.Clear();
                foreach (Monster m in monsters)
                {
                    otherMonsters.Add(m.getPosition());
                    otherMonsters.Add(m.getTargetCell());
                    otherMonsters.Add(m.getChainedTargetCell());
                }

                currentCountDown -= gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

                if (currentCountDown <= 0)
                {
                    failLevel("You ran out of time..... Pathetic......");
                }
            }
            else
            {
                // update the cursor location 
                cursor.update(gameTime);
            }


        }

        public void updateCamera(int widthTiles, int heightTiles)
        {

            // prepare the camera
            Vector2 follow;
            if (isEditor)
            {
                follow = cursor.getPosition();
            }
            else
            {
                follow = player.getPosition();
            }

            topLeftCorner.X = follow.X - widthTiles / 2 - 1;
            topLeftCorner.Y = follow.Y - heightTiles / 2 - 1;
            if (topLeftCorner.X < 0) topLeftCorner.X = 0;
            if (topLeftCorner.X + widthTiles > mapSize - 1) topLeftCorner.X = mapSize - widthTiles - 1;
            if (topLeftCorner.Y < 0) topLeftCorner.Y = 0;
            if (topLeftCorner.Y + heightTiles > mapSize - 1) topLeftCorner.Y = mapSize - heightTiles - 1;

        }

        /// <summary>
        /// Core drawing method for the level.
        /// It renders all of the sprites that should be in view.
        /// </summary>
        /// <param name="spriteBatch">The drawing pane to render to.</param>
        public void draw(SpriteBatch spriteBatch)
        {
            Rectangle drawArea = new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height); // new Rectangle(0, 0, 1024, 768);//
            int tileAreaWidth = (int)(drawArea.Width * .732f); // 100 = temp HUD width
            int widthTiles = (int)(tileAreaWidth / TILEDIMENSIONS.X);
            int heightTile = (int)(drawArea.Height / TILEDIMENSIONS.Y);

            updateCamera(widthTiles, heightTile);

            Vector2 pos = new Vector2(drawArea.Left, drawArea.Top);
            float modifier = 0;
            if ((int)topLeftCorner.Y + heightTile + 1 >= mapSize)
            {
                modifier = -(spriteBatch.GraphicsDevice.Viewport.Height % TILEDIMENSIONS.Y);
                pos.Y = modifier;
            }

            Vector2 playerOffset = getPlayer().getTransitionOffset();
            Vector2 actualPlayerOffset = Vector2.Zero;
            if ((topLeftCorner.X > 0 && (int)topLeftCorner.X + widthTiles + 1 < mapSize && (playerOffset.X < 0 || (playerOffset.X > 0))))
            {
                actualPlayerOffset.X -= playerOffset.X;
                pos.X -= playerOffset.X;

                if (playerOffset.X < 0)
                {
                    topLeftCorner.X--;
                    pos.X -= TILEDIMENSIONS.X;
                    actualPlayerOffset.X -= TILEDIMENSIONS.X;
                }
            }

            //|| (topLeftCorner.Y == 1 && playerOffset.Y > 0)
            if ((topLeftCorner.Y > 0 && modifier == 0 && ((playerOffset.Y < 0) || (playerOffset.Y > 0)))
                )
            {
                actualPlayerOffset.Y -= playerOffset.Y;
                pos.Y -= playerOffset.Y;

                if (playerOffset.Y < 0)
                {
                    topLeftCorner.Y--;
                    pos.Y -= TILEDIMENSIONS.Y;
                    actualPlayerOffset.Y -= TILEDIMENSIONS.Y;
                }
            }

            // First draw the tiles
            for (int i = (int)topLeftCorner.Y; i < (int)topLeftCorner.Y + heightTile + 1 + 2; i++)
            {
                if (i >= mapSize) continue;

                for (int j = (int)topLeftCorner.X; j <= (int)topLeftCorner.X + widthTiles + 2; j++)
                {
                    if (j >= mapSize) continue;
                    tiles[j, i].draw(spriteBatch, pos);
                    pos.X += TILEDIMENSIONS.X;
                }

                pos = new Vector2(actualPlayerOffset.X + drawArea.Left, pos.Y + TILEDIMENSIONS.Y);
            }


            // Draw the Player in
            pos = new Vector2(drawArea.Left + (player.getPosition().X - topLeftCorner.X) * TILEDIMENSIONS.X + actualPlayerOffset.X,
                              drawArea.Top + (player.getPosition().Y - topLeftCorner.Y) * TILEDIMENSIONS.Y + modifier + actualPlayerOffset.Y);
            player.draw(spriteBatch, pos);

            // Draw each of the monsters in
            foreach (Monster m in monsters)
            {
                // perform occlision
                if (!(m.getPosition().X > topLeftCorner.X + widthTiles + 1
                    || m.getPosition().X < topLeftCorner.X - 1
                    || m.getPosition().Y > topLeftCorner.Y + heightTile + 1
                    || m.getPosition().Y < topLeftCorner.Y - 1))
                {
                    pos = new Vector2(drawArea.Left + actualPlayerOffset.X + (m.getPosition().X - topLeftCorner.X) * TILEDIMENSIONS.X,
                              drawArea.Top + actualPlayerOffset.Y + (m.getPosition().Y - topLeftCorner.Y) * TILEDIMENSIONS.Y + modifier);
                    m.draw(spriteBatch, pos);
                }
            }

            if (isEditor)
            {
                pos = new Vector2(drawArea.Left + (cursor.getPosition().X - topLeftCorner.X) * TILEDIMENSIONS.X,
                              drawArea.Top + (cursor.getPosition().Y - topLeftCorner.Y) * TILEDIMENSIONS.Y + modifier);
                cursor.draw(spriteBatch, pos);
            }

            //bretts addition
            windowCollection.Draw(spriteBatch);
            //end


        }

        /// <summary>
        /// Change the current state of the gameplay.
        /// </summary>
        /// <param name="gamePlayState">The state to set to.</param>
        public void setState(GamePlayState gamePlayState)
        {
            this.gamePlayState = gamePlayState;
        }

        /// <summary>
        /// Get the current gameplay state.
        /// </summary>
        /// <returns>The game play state.</returns>
        public GamePlayState getGamePlayState()
        {
            return gamePlayState;
        }
        #endregion

        #region Movement

        /// <summary>
        /// Move an object in a direction. It performs a check and then
        /// just passes the call to the other more detailed method.
        /// </summary>
        /// <param name="obj">The object moving.</param>
        /// <param name="dir">The direction of motion.</param>
        /// <returns>Is the move valid?</returns>
        public bool moveObject(GameObject obj, GameObject.Direction dir)
        {
            if (obj.getType() == GameObject.Type.Block) return false;

            Vector2 pos = getMoveFromDirection(obj, dir);

            return moveObject(obj, pos, dir);
        }

        /// <summary>
        /// Moves an object and triggers all the appropriate events as it does so.
        /// Also checks to make sure it actually can move there first.
        /// </summary>
        /// <param name="obj">The object that is moving.</param>
        /// <param name="to">The tile being moved to.</param>
        /// <param name="dir">The direction of motion.</param>
        /// <returns>Is the move successful?</returns>
        public bool moveObject(GameObject obj, Vector2 to, GameObject.Direction dir)
        {
            if (obj.getType() == GameObject.Type.Block) return false;

            if (!canExit(obj, dir) || !canEnter(obj, to, dir)) return false;

            if (obj.getType() == GameObject.Type.Player)
            {
                if (!((Player)obj).beginCheckMoveTo(to)) return false;
            }

            //Kanes Code to stop merging but causes duplication!
            if (obj.getType() == GameObject.Type.Monster && getBlockAt(to).getBlockType() != Block.BlockType.Portal)
            {
                if (((Monster)obj).getMonsterType() != Monster.MonsterType.Arrow)
                {
                    foreach (Vector2 pos in otherMonsters)
                        if (Vector2.Equals(to, pos)) return false;

                    otherMonsters.Add(to);
                }
            }

            // Trigger Event Exiting
            triggerOnExiting(obj, dir);

            // Tell object to begin movement
            obj.setFacing(dir);
            if (obj.getType() == GameObject.Type.Monster)
            {
                ((Monster)obj).beginMoveTo(to);
            }
            else
            {
                ((Player)obj).beginMoveTo(to);
            }

            // Trigger the entering event
            triggerOnEntering(to, obj, getFlippedDirection(dir));
            return true;
        }

        /// <summary>
        /// Instantly moves the object to another location without requring a transition.
        /// </summary>
        /// <param name="obj">Object to move.</param>
        /// <param name="to">The location to move to.</param>
        /// <param name="dir">The a custom direction property that may be used to handle functionality.</param>
        public void moveObjectNow(GameObject obj, Vector2 to, GameObject.Direction dir)
        {
            if (obj.getType() == GameObject.Type.Block) return;

            // trigger exiting
            triggerOnExiting(obj, dir);
            // trigger entering
            triggerOnEntering(to, obj, getFlippedDirection(dir));

            Vector2 oldPos = obj.getPosition();
            obj.setPosition(to);

            // trigger exited
            triggerOnExited(oldPos, obj);

            // trigger entered
            triggerOnEntered(obj);
        }

        /// <summary>
        /// Test condition for whether movement is allowed
        /// </summary>
        /// <param name="obj">The object moving.</param>
        /// <param name="to">The direction of movement.</param>
        /// <returns>Whether that object can move in that direction.</returns>
        public bool canMove(GameObject obj, GameObject.Direction to)
        {
            return canExit(obj, to) && canEnter(obj, getMoveFromDirection(obj, to), to);
        }

        /// <summary>
        /// Checks if the object can enter the square at pos by moving with direction to.
        /// </summary>
        /// <param name="obj">The object that wants to move.</param>
        /// <param name="pos">The position that movement to should be checked.</param>
        /// <param name="to">The direction of motion relative to the object.</param>
        /// <returns>True, if the object can enter that square.</returns>
        public bool canEnter(GameObject obj, Vector2 pos, GameObject.Direction to)
        {
            return canEnter(obj, pos, to, 0);
        }

        /// <summary>
        /// Checks if the oject can enter the square at pos by moving with direction to.
        /// Depth allows for an advanced multi stage check that takes into account progressive
        /// blocks in a direction.
        /// </summary>
        /// <param name="obj">The object that wants to move.</param>
        /// <param name="pos">THe position that movement to should be checked.</param>
        /// <param name="to">The direction of motion relative to the object.</param>
        /// <param name="depth">The number of squares maximum that may be checked for in a direction.</param>
        /// <returns>True, if the object can enter that square.</returns>
        public bool canEnter(GameObject obj, Vector2 pos, GameObject.Direction to, int depth)
        {
            GameObject.Direction from = getFlippedDirection(to);

            // if the tile can be entered then also check if there is a monster/player there too
            if (tiles[(int)pos.X, (int)pos.Y].canEnter(obj, from, depth))
            {
                Monster m = getMonsterAt(pos);

                // if there is a monster and they can't enter the next
                if (m != null && !m.canEnter(obj, from, depth))
                {
                    return false;
                }

                /*if (player.getPosition().Equals(pos) && !m.canEnter(obj, from, depth)) {
                    return false;
                }*/

                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if exiting is legal from normally within a particular block.
        /// </summary>
        /// <param name="obj">The object to test with. Using its current position.</param>
        /// <param name="dir">The direction of exit.</param>
        /// <returns></returns>
        public bool canExit(GameObject obj, GameObject.Direction dir)
        {
            Vector2 pos = obj.getPosition();
            return tiles[(int)pos.X, (int)pos.Y].canExit(obj, dir);
        }

        #endregion

        #region Block State Modifiers
        /// <summary>
        /// Set the "state" for the facing of an individual block.
        /// </summary>
        /// <param name="position">The location of block to change.</param>
        /// <param name="facing">The state to set the block to.</param>
        public void setBlockFacing(Vector2 position, GameObject.Direction facing)
        {
            GameObject obj = tiles[(int)position.X, (int)position.Y];

            obj.setFacing(facing);

        }

        /// <summary>
        /// Set the state to a particular value for all of a particular block type.
        /// Note that this can only be used on specific blocks!!
        /// It will only work on blocks that have been defined to be changeable.
        /// </summary>
        /// <param name="blockType">The block type of which to change all of.</param>
        /// <param name="facing">The state to change the block to.</param>
        public void setBlockFacingAll(Block.BlockType blockType, GameObject.Direction facing)
        {
            if (!Block.isStateBlock(blockType)) return;

            foreach (GameObject obj in stateBlocks)
            {
                if (((Block)obj).getBlockType() != blockType) continue;

                obj.setFacing(facing);
            }
        }

        /// <summary>
        /// Toggle the state for a block at a particular position. If it is in toggle1 state, then it will use toggle2.
        /// Otherwise it will set the state to toggle1.
        /// </summary>
        /// <param name="position">The position of the block to toggle.</param>
        /// <param name="toggle1">The state to change to if the current state is not this.</param>
        /// <param name="toggle2">The state to change to if the current state is toggle1.</param>
        public void setBlockFacingToggle(Vector2 position, GameObject.Direction toggle1, GameObject.Direction toggle2)
        {
            GameObject obj = tiles[(int)position.X, (int)position.Y];

            if (obj.getFacing() == toggle1)
            {
                obj.setFacing(toggle2);
            }
            else
            {
                obj.setFacing(toggle1);
            }
        }

        /// <summary>
        /// Toggles the state for all objects of the specified type between one or the other of a particular 
        /// state. 
        /// </summary>
        /// <param name="blockType">The type of block to change for.</param>
        /// <param name="toggle1">The state to change to if the current state is not this.</param>
        /// <param name="toggle2">The state to change to if the current state is toggle1.</param>
        public void setBlockFacingToggleAll(Block.BlockType blockType, GameObject.Direction toggle1, GameObject.Direction toggle2)
        {
            if (!Block.isStateBlock(blockType)) return;

            foreach (GameObject obj in stateBlocks)
            {
                if (((Block)obj).getBlockType() != blockType) continue;

                if (obj.getFacing() == toggle1)
                {
                    obj.setFacing(toggle2);
                }
                else
                {
                    obj.setFacing(toggle1);
                }

            }
        }

        /// <summary>
        /// Get a monster if there is one at the specified grid coordinate.
        /// </summary>
        /// <param name="pos">The position to check for.</param>
        /// <returns>A single monster if there is one there. Or null if there isn't one there.</returns>
        public Monster getMonsterAt(Vector2 pos)
        {
            foreach (Monster m in monsters)
            {
                if (m.getPosition().Equals(pos))
                {
                    return m;
                }
            }

            return null;
        }

        /// <summary>
        /// Get all monsters if there is one at the specified grid coordinate.
        /// </summary>
        /// <param name="pos">The position to check for.</param>
        /// <returns>A collection of monsters that may be empty.</returns>
        public List<Monster> getAllMonsterAt(Vector2 pos)
        {
            List<Monster> result = new List<Monster>();
            foreach (Monster m in monsters)
            {
                if (m.getPosition().Equals(pos))
                {
                    result.Add(m);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a block at a particular position
        /// </summary>
        /// <param name="pos">The position</param>
        /// <returns>The block at that location.</returns>
        public Block getBlockAt(Vector2 pos)
        {
            return (Block)tiles[(int)pos.X, (int)pos.Y];
        }

        /// <summary>
        /// Get all objects of a legal stateBlock type.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <returns>The collection found.</returns>
        public List<Block> getAllBlockOfType(Block.BlockType type)
        {
            List<Block> blocks = new List<Block>();

            foreach (GameObject obj in stateBlocks)
            {
                if (((Block)obj).getBlockType() != type) continue;

                blocks.Add((Block)obj);
            }

            return blocks;
        }
        #endregion

        #region Triggers
        /// <summary>
        /// Trigger all relevant events at the target square.
        /// </summary>
        /// <param name="target">The target position for which to trigger events.</param>
        /// <param name="obj">The object that is triggering the events.</param>
        /// <param name="dir">The direction that the object is entering from.</param>
        public void triggerOnEntering(Vector2 target, GameObject obj, GameObject.Direction dir)
        {
            tiles[(int)target.X, (int)target.Y].onEntering(obj, dir);

            List<Monster> monsters = getAllMonsterAt(target);
            foreach (Monster m in monsters)
            {
                if (m == obj) continue;
                m.onEntering(obj, dir);
            }

            if (player != obj && player.getPosition().Equals(target))
            {
                player.onEntering(obj, dir);
            }
        }

        /// <summary>
        /// Trigger all relevant events at the position of the obj.
        /// </summary>
        /// <param name="obj">The object at a location to trigger events.</param>
        public void triggerOnEntered(GameObject obj)
        {
            Vector2 target = obj.getPosition();
            tiles[(int)target.X, (int)target.Y].onEntered(obj);

            List<Monster> monsters = getAllMonsterAt(target);
            foreach (Monster m in monsters)
            {
                if (m == obj) continue;
                m.onEntered(obj);
            }

            if (player != obj && player.getPosition().Equals(target))
            {
                player.onEntered(obj);
            }
        }

        /// <summary>
        /// Trigger all relevant events at the position where the object was.
        /// (this will still be the current position of theobject)
        /// </summary>
        /// <param name="obj">The object that is leaving a block.</param>
        /// <param name="dir">The direction that the obj is exiting to.</param>
        public void triggerOnExiting(GameObject obj, GameObject.Direction dir)
        {
            Vector2 target = obj.getPosition();

            if (obj.getType() == GameObject.Type.Monster)
                otherMonsters.Remove(target);

            tiles[(int)target.X, (int)target.Y].onExiting(obj, dir);

            List<Monster> monsters = getAllMonsterAt(target);
            foreach (Monster m in monsters)
            {
                if (m == obj) continue;
                m.onExiting(obj, dir);
            }

            if (player != obj && player.getPosition().Equals(target))
            {
                player.onExiting(obj, dir);
            }
        }

        /// <summary>
        /// Trigger all relevant revents at the psoitin where the object has just exited.
        /// </summary>
        /// <param name="exitedPos">The position that was just exited.</param>
        /// <param name="obj">The object that exited.</param>
        public void triggerOnExited(Vector2 exitedPos, GameObject obj)
        {
            tiles[(int)exitedPos.X, (int)exitedPos.Y].onExited(obj);

            List<Monster> monsters = getAllMonsterAt(exitedPos);
            foreach (Monster m in monsters)
            {
                if (m == obj) continue;
                m.onEntered(obj);
            }

            if (player != obj && player.getPosition().Equals(exitedPos))
            {
                player.onExited(obj);
            }
        }
        #endregion

        #region Add and Remove Blocks from Map
        /// <summary>
        /// Load/level generation method ONLY. 
        /// <b>DO NOT USE THIS METHOD</b> while in a running editor or game mode.
        /// It is designed to place a block without needing to remove the old block.
        /// </summary>
        /// <param name="position">The position of block to replace.</param>
        /// <param name="newBlock">The new block type to place.</param>
        protected void setBlockNoRemove(Vector2 position, GameObject newBlock)
        {
            // only do this if it is actually a new block object
            if (newBlock.getType() != GameObject.Type.Block) return;

            // add block to map
            tiles[(int)position.X, (int)position.Y] = newBlock;

            // add block to stateblocks list if it needs to be
            if (((Block)newBlock).isStateBlock())
            {
                stateBlocks.Add(newBlock);
            }
        }

        /// <summary>
        /// Set the block at the defined map position to the new block defined by newBlock.
        /// </summary>
        /// <param name="position">The position of block to replace.</param>
        /// <param name="newBlock">The new block type to place.</param>
        public void setBlock(Vector2 position, GameObject newBlock)
        {
            // only do this if it is actually a new block object
            if (newBlock.getType() != GameObject.Type.Block) return;

            if (isEditor) dependencyManager.checkDependencies(position);
            // remove the old block
            removeBlock(position);

            // add block to map
            tiles[(int)position.X, (int)position.Y] = newBlock;

            // add block to stateblocks list if it needs to be
            if (((Block)newBlock).isStateBlock())
            {
                stateBlocks.Add(newBlock);
            }
        }

        /// <summary>
        /// Remove the block references at the specified position.
        /// </summary>
        /// <param name="position">The position to remove the block at.</param>
        protected void removeBlock(Vector2 position)
        {
            // get block reference
            GameObject removeObj = tiles[(int)position.X, (int)position.Y];

            // remove from block from the map
            tiles[(int)position.X, (int)position.Y] = null;

            // remove from the stateBlock list if it is a changeable type
            if (((Block)removeObj).isStateBlock())
            {
                stateBlocks.Remove(removeObj);
            }
        }

        /// <summary>
        /// Add a dependency between two blocks.
        /// </summary>
        /// <param name="dependency">The dependency configuration.</param>
        public void addBlockDependency(BlockDependency dependency)
        {
            dependencyManager.addDependency(dependency);
        }

        /// <summary>
        /// Adds a newly created monster to a pre-specified inside the object location.
        /// It will replace any existing monster that is at that location!
        /// </summary>
        /// <param name="newMonster">The monster to add.</param>
        public void addMonster(GameObject newMonster)
        {
            // remove the old monster
            // TODO: consider if there are any issues with this implementation
            // need to consider that monsters need to check allowance for entry first
            // assumes monsters never allowed to enter other monsters
            //removeMonstersAt(newMonster.getPosition());

            // only place if there is not a player here if this is editor mode
            if (isEditor && !newMonster.getPosition().Equals(player.getPosition()))
            {

                monsters.Add(newMonster);

            }
            else if (!isEditor)
            {
                // trigger the entering methods for the newly spawned monster appropriately
                monstersToAdd.Add(newMonster);
                triggerOnEntering(newMonster.getPosition(), newMonster, newMonster.getFacing());
                triggerOnEntered(newMonster);
            }
        }

        /// <summary>
        /// Remove a monster if it exists at the specified location.
        /// </summary>
        /// <param name="pos"></param>
        public void removeMonstersAt(Vector2 pos)
        {
            List<Monster> monsters = getAllMonsterAt(pos);
            foreach (Monster current in monsters)
            {
                if (isEditor)
                {
                    if (current != null) this.monsters.Remove(current);
                }
                else
                {
                    removeMonster(current);
                }
            }
            if(!isEditor)
                otherMonsters.Remove(pos);
        }

        /// <summary>
        /// Remove a specific monster from the game.
        /// </summary>
        /// <param name="monster"></param>
        public void removeMonster(Monster monster)
        {
            if (monster != null) monstersToRemove.Add(monster);
        }

        /// <summary>
        /// Removal all monsters that are no longer needed.
        /// </summary>
        private void processMonsterRemovals()
        {
            foreach (Monster m in monstersToRemove)
            {
                if (m.getMonsterType() == Monster.MonsterType.Imp)
                    ((Monsters.ImpMonster)m).onDeath();

                monsters.Remove(m);
                otherMonsters.Remove(m.getPosition());
            }

            monstersToRemove.Clear();
        }

        /// <summary>
        /// Add all new monsters that have been added in last update
        /// </summary>
        private void processMonsterAdditions()
        {
            foreach (GameObject m in monstersToAdd)
            {
                monsters.Add(m);
            }

            monstersToAdd.Clear();
        }

        /// <summary>
        /// Create a monster type object using the game object factory.
        /// </summary>
        /// <param name="template">The template to create the monster from.</param>
        /// <returns>The monster.</returns>
        public GameObject createMonster(MonsterSave template)
        {
            return gameObjFactory.createObject(template);
        }

        /// <summary>
        /// Create a block type object using the game object factory.
        /// </summary>
        /// <param name="template">The template to create the block from.</param>
        /// <returns>The block.</returns>
        public GameObject createBlock(BlockSave template)
        {
            return gameObjFactory.createObject(template);
        }

        /// <summary>
        /// Set the player start location.
        /// </summary>
        public void placePlayerStart(Vector2 pos)
        {
            removeMonstersAt(pos);
            player.setPosition(pos);
        }
        #endregion

        #region Direction Helper Methods
        /// <summary>
        /// Gets the opposite direction to allow for switching between to and from.
        /// </summary>
        /// <param name="dir">The direction to flip.</param>
        /// <returns>The flipped direction.</returns>
        public GameObject.Direction getFlippedDirection(GameObject.Direction dir)
        {
            switch (dir)
            {
                case GameObject.Direction.Down:
                    return GameObject.Direction.Up;
                case GameObject.Direction.Up:
                    return GameObject.Direction.Down;
                case GameObject.Direction.Left:
                    return GameObject.Direction.Right;
                case GameObject.Direction.Right:
                    return GameObject.Direction.Left;
                default:
                    Debug.Assert(false, "Invalid Movement direction used in call to canEnter in Level");
                    break;
            }
            return GameObject.Direction.Up;
        }

        /// <summary>
        /// Get the movement that occurs from a particular movement direction.
        /// The result is the coordinates of the tile that would be moved to.
        /// </summary>
        /// <param name="obj">The object moveing.</param>
        /// <param name="dir">The direction of motion.</param>
        /// <returns></returns>
        public Vector2 getMoveFromDirection(GameObject obj, GameObject.Direction dir)
        {
            Vector2 pos = obj.getPosition();
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
            return pos;
        }


        /// <summary>
        /// Get an integer describing the direction of motion based on a specified direction.
        /// </summary>
        /// <param name="dir">The direction of motion.</param>
        /// <returns>A direction multiplier.</returns>
        public int getDirectionMultiplier(GameObject.Direction dir)
        {
            if (dir == GameObject.Direction.Up || dir == GameObject.Direction.Left)
            {
                return -1;
            }

            if (dir == GameObject.Direction.Down || dir == GameObject.Direction.Right)
            {
                return 1;
            }

            return 0;
        }
        #endregion

        #region Coins State Modifiers
        /// <summary>
        /// Increase the number of coins that have been collected.
        /// </summary>
        public void collectCoin()
        {
            collectedItems++;
        }

        /// <summary>
        /// Check if all the required coins have been collected during the current level.
        /// </summary>
        /// <returns>True only if enough have been collected.</returns>
        public bool allCoinsCollected()
        {
            return (collectedItems >= itemsToCollect);
        }

        /// <summary>
        /// When loading levels and editing levels this may be called
        /// to modify the number of items that need to be collected.
        /// </summary>
        public void increaseCollectibleCoins()
        {
            itemsToCollect++;
        }

        /// <summary>
        /// When editing levels this may be called to modify the number of
        /// items that need to be collected.
        /// </summary>
        public void decreaseCollectibleCoins()
        {
            if (isEditor)
                itemsToCollect--;
        }
        #endregion


        //bretts addition
        internal int getCoinsLeft()
        {

            return itemsToCollect - collectedItems;
        }

        internal int getCoinsCollected()
        {

            return collectedItems;
        }

        internal int getTimeLeft()
        {

            return (int)currentCountDown;
        }

        internal int getCountDown()
        {
            return (int)countDown;
        }

        internal void setCountDown(int time)
        {
            countDown = time;
        }

        internal void setName(string name)
        {
            this.levelName = name;
        }

        internal int getLifeBonus()
        {
            return lifeBonus;
        }

        private void updateLifeBonus()
        {
            if (lifeBonus > -3) lifeBonus--;
        }

        internal void setLifeBonus(int lifeBonus)
        {
            this.lifeBonus = lifeBonus;
        }

        internal void finishLevel()
        {

            levelSummary = new LevelSummary(this, getTimeLeft(), Content, leaderboard);
            windowCollection.add(levelSummary);
            this.gamePlayState = GamePlayState.Paused;
        }

        internal void failLevel(String failMessage)
        {

            otherMonsters.Clear();
            levelFailed = new LevelFailed(this, Content, failMessage);
            windowCollection.add(levelFailed);
            setState(GamePlayState.LevelFailed);
        }

        internal Game1 getGameRef()
        {

            return gameRef;
        }
        //end
    }
}
