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
using System.IO;

// for Application.StartupPath
using System.Reflection;
using System.Diagnostics;
using System.Xml.Serialization;

namespace GameDevMajor
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public enum GameState { MainMenu, LevelSelection, LevelEditor, Level, LevelSummary, Help, Credits };

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState oldKeyboardState;
        KeyboardState currentKeyboardState;
        GamePadState oldGamePadState;
        GamePadState currentGamePadState;

        Level level;
        SavedLevelList savedLevelList;
        GameState state;

        String gamePath;

        List<Song> music;
        int curSongID;

        MainMenu menu;
        LevelSelect levelSelect;
        Random rand;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            rand = new Random();

            // prepare music
            music = new List<Song>();
            curSongID = 0;
            music.Add(Content.Load<Song>("Music\\ambush"));
            music.Add(Content.Load<Song>("Music\\notafriendlyplace"));
            music.Add(Content.Load<Song>("Music\\orphans_cry"));
            music.Add(Content.Load<Song>("Music\\pulse"));
            

            // setup a string for the game path
            gamePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Game1)).CodeBase);
            gamePath = gamePath.Substring(6);

            if (!Directory.Exists(gamePath + "\\Saves"))
            {
                Directory.CreateDirectory(gamePath + "\\Saves");
            }

            if (File.Exists(gamePath + "\\Saves\\LevelList.save"))
            {
                // create the serializer
                XmlSerializer SerializerObj = new XmlSerializer(typeof(SavedLevelList));

                // Create a new file stream for reading the XML file
                FileStream ReadFileStream = new FileStream(gamePath + "\\Saves\\LevelList.save", FileMode.Open, FileAccess.Read, FileShare.Read);

                // Load the object saved above by using the Deserialize function
                SavedLevelList LoadedObj = (SavedLevelList)SerializerObj.Deserialize(ReadFileStream);

                savedLevelList = LoadedObj;

                // Cleanup
                ReadFileStream.Close();
            }
            else
            {
                savedLevelList = new SavedLevelList();

                level = new Level(64, savedLevelList.getNextLevelNumber(), this);

                savedLevelList.AddLevel(level.getLevelInfo());

                savePlayState();
            }

            //level = new Level(true, savedLevelList.getLevelList()[0].levelNumber, this);

            openMainMenu();
        }

        /// <summary>
        /// Save the current level and level list.
        /// </summary>
        public void savePlayState()
        {
            // Create a new XmlSerializer instance
            XmlSerializer SerializerObj = new XmlSerializer(typeof(SavedLevelList));

            // Create a new file stream to write the serialized object to a file
            TextWriter WriteFileStream = new StreamWriter(gamePath + "\\Saves\\LevelList.save");
            SerializerObj.Serialize(WriteFileStream, savedLevelList);

            // Cleanup
            WriteFileStream.Close();
        }

        /// <summary>
        /// Store updated level name in the level save file
        /// </summary>
        /// <param name="levelNumber">Number to change</param>
        /// <param name="levelName">Name to change for the number</param>
        public void saveLevelDetails(int levelNumber, string levelName)
        {
            savedLevelList.updateLevelName(levelNumber, levelName);
        }

        /// <summary>
        /// Increase the total score by the amount
        /// </summary>
        /// <param name="newScore">Amount to increase score by</param>
        /// <returns>The new updated score</returns>
        public int updateTotalScore(int newScore)
        {
            return savedLevelList.updateTotalScore(newScore);
        }

        /// <summary>
        /// Get the current total score.
        /// </summary>
        /// <returns>Total score.</returns>
        public int getTotalScore()
        {
            return savedLevelList.getTotalScore();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            updateInputStates();

            updateMusic();

            // TODO: decide if we leave this in
            if (Keyboard.GetState().IsKeyDown(Keys.F1) && getLastKeyboardState().IsKeyUp(Keys.F1))
                graphics.ToggleFullScreen();

            if (state == GameState.Level && Keyboard.GetState().IsKeyDown(Keys.F2) && getLastKeyboardState().IsKeyUp(Keys.F2))
                resetLevel(level.getLevelInfo().levelNumber, level.getLifeBonus());

            if (state == GameState.LevelEditor && Keyboard.GetState().IsKeyDown(Keys.F3) && getLastKeyboardState().IsKeyUp(Keys.F3))
            {
                level.saveLevelToFile();
                selectLevel(level.getLevelInfo().levelNumber, false);
            }

            if (state == GameState.MainMenu)
            {
                menu.update(gameTime);
                return;
            }
            else if (state == GameState.LevelSelection)
            {
                levelSelect.update(gameTime);
                return;
            }

            level.update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Update the music and music audio controls
        /// </summary>
        private void updateMusic()
        {
            if (MediaPlayer.State == MediaState.Stopped)
            {
                startNextSong();
            }
            else if (MediaPlayer.State == MediaState.Playing && Keyboard.GetState().IsKeyDown(Keys.M) && getLastKeyboardState().IsKeyUp(Keys.M))
            {
                MediaPlayer.Pause();
            }
            else if (MediaPlayer.State == MediaState.Paused && Keyboard.GetState().IsKeyDown(Keys.M) && getLastKeyboardState().IsKeyUp(Keys.M))
            {
                MediaPlayer.Resume();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus) && getLastKeyboardState().IsKeyUp(Keys.OemMinus))
            {
                if (MediaPlayer.Volume >= 0.0)
                    MediaPlayer.Volume -= 0.05f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.OemPlus) && getLastKeyboardState().IsKeyUp(Keys.OemPlus))
            {
                if (MediaPlayer.Volume <= 1.0)
                    MediaPlayer.Volume += 0.05f;
            }
        }

        public Vector2 getDisplayDimensions()
        {
            return new Vector2(graphics.GraphicsDevice.Viewport.Bounds.Width, graphics.GraphicsDevice.Viewport.Bounds.Height);
        }

        #region Change Functionality Methods

        /// <summary>
        /// Create a new level.
        /// </summary>
        public void createNewLevel()
        {
            level = new Level(64, savedLevelList.getNextLevelNumber(), this);

            savedLevelList.AddLevel(level.getLevelInfo());

            // Create a new XmlSerializer instance
            XmlSerializer SerializerObj = new XmlSerializer(typeof(SavedLevelList));

            // Create a new file stream to write the serialized object to a file
            TextWriter WriteFileStream = new StreamWriter(gamePath + "\\Saves\\LevelList.save");
            SerializerObj.Serialize(WriteFileStream, savedLevelList);

            // Cleanup
            WriteFileStream.Close();

            selectLevel(level.getLevelInfo().levelNumber, true);
        }

        /// <summary>
        /// Begin another song from the library.
        /// </summary>
        public void startNextSong()
        {
            MediaPlayer.Stop();
            int nextSongID;
            do
            {
                nextSongID = rand.Next(music.Count);
            } while (nextSongID == curSongID);
            curSongID = nextSongID;
            MediaPlayer.Play(music[curSongID]);
        }

        /// <summary>
        /// Begin a new game.
        /// </summary>
        public void newGame()
        {
            savedLevelList.resetLevel();
            savedLevelList.resetTotalScore();
            savePlayState();
            level = new Level(false, savedLevelList.getLevelList()[savedLevelList.getCurrentLevelNumber()].levelNumber, this);
            state = GameState.Level;
        }

        /// <summary>
        /// Continue from the last point of gameplay.
        /// </summary>
        public void continueGame()
        {
            level = new Level(false, savedLevelList.getLevelList()[savedLevelList.getCurrentLevelNumber()].levelNumber, this);
            state = GameState.Level;
        }

        /// <summary>
        /// Progress to the next level.
        /// </summary>
        public void nextLevel()
        {
            savedLevelList.nextLevel();
            savePlayState();
            level = new Level(false, savedLevelList.getLevelList()[savedLevelList.getCurrentLevelNumber()].levelNumber, this);
            state = GameState.Level;
        }

        /// <summary>
        /// Create an empty level and then switch to it.
        /// </summary>
        public void startNewLevel()
        {
            createNewLevel();
            state = GameState.Level;
        }

        /// <summary>
        /// Reset the current level back to the start.
        /// </summary>
        public void resetLevel(int levelNumber, int lifeCount)
        {
            //level.resetLevel();
            level = new Level(false, levelNumber, this);
            level.setLifeBonus(lifeCount);
        }

        /// <summary>
        /// Set the level to the specified number
        /// </summary>
        /// <param name="levelNumber">Level to set to.</param>
        /// <param name="asEditor">Whether to open as editor.</param>
        public void selectLevel(int levelNumber, bool asEditor)
        {
            savedLevelList.setLevelNumber(levelNumber);
            savedLevelList.resetTotalScore();
            savePlayState();
            level = new Level(asEditor, levelNumber, this);

            if (!asEditor) state = GameState.Level;
            else state = GameState.LevelEditor;
        }

        /// <summary>
        /// Opens the Main Menu
        /// </summary>
        public void openMainMenu()
        {
            menu = new MainMenu(Content, this);
            state = GameState.MainMenu;
        }

        /// <summary>
        /// Opens the level select menu
        /// </summary>
        public void openLevelSelectMenu()
        {
            levelSelect = new LevelSelect(Content, this, savedLevelList);
            state = GameState.LevelSelection;
        }

        #endregion

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(210,210,200));

            spriteBatch.Begin();

            if (state == GameState.MainMenu)
            {
                menu.draw(spriteBatch);
            }
            else if (state == GameState.LevelSelection)
            {
                levelSelect.draw(spriteBatch);
            }
            else if (state == GameState.Level || state == GameState.LevelEditor)
            {
                level.draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public string getGamePath()
        {
            return gamePath;
        }

        #region Input related methods
        protected void updateInputStates()
        {
            oldGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            oldKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
        }

        public KeyboardState getKeyboardState()
        {
            return currentKeyboardState;
        }

        public KeyboardState getLastKeyboardState()
        {
            return oldKeyboardState;
        }

        public GamePadState getGamePadState()
        {
            return currentGamePadState;
        }

        public GamePadState getLastGamePadState()
        {
            return oldGamePadState;
        }
        #endregion
    }
}
