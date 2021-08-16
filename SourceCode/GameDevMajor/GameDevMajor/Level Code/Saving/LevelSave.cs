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
    [Serializable]
    public class LevelSave
    {
        #region Saved Instance Variables
        /// <summary>
        /// Variable holding how large the map is in dimensions.
        /// This is how many tiles wide and high the map is. 
        /// Valid values include: 64, 128, 256
        /// </summary>
        public int mapSize;

        /// <summary>
        /// Tiles is the collection of tiles that is in a 2D grid.
        /// These will be used by the object factory to recreate the map.
        /// </summary>
        public List<BlockSave> tiles;

        /// <summary>
        /// The location where the player starts in the level.
        /// </summary>
        public Vector2 playerStartPosition;

        /// <summary>
        /// The collection of monsters that exist in the level.
        /// </summary>
        public List<MonsterSave> monsters;
        
        /// <summary>
        /// The number of the level.
        /// </summary>
        public int levelNumber;

        /// <summary>
        /// A string representing the name of the level.
        /// </summary>
        public string levelName;

        /// <summary>
        /// The time to complete the level within.
        /// </summary>
        public int countDown;
        #endregion

        /// <summary>
        /// Setup a level save so it defaults to being an editable level.
        /// Levels that shouldn't be editable post-release should include
        /// this flag set to false.
        /// </summary>
        public LevelSave()
        {
        }

        #region Getters and Setters
        /// <summary>
        /// Takes the size of the map and the collection of tiles.
        /// It then generates a save state for each one to minimise the
        /// data that needs to be stored. 
        /// </summary>
        /// <param name="mapSize">The size of the maps width and height.</param>
        /// <param name="tiles">The collection of tiles that make up the map.</param>
        public void storeMap(int mapSize, GameObject[,] tiles)
        {
            this.mapSize = mapSize;
            this.tiles = new List<BlockSave>();

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    this.tiles.Add(((Block)tiles[i, j]).createSaveState());
                }
            }
        }

        /// <summary>
        /// Get the size of the maps dimensions.
        /// </summary>
        /// <returns>The width and height in number of tiles for the map.</returns>
        public int getMapSize()
        {
            return mapSize;
        }

        /// <summary>
        ///  Get the collection of saved blocks that can be used by the object
        ///  factory to create the map correctly.
        /// </summary>
        /// <returns>The collection of blocks.</returns>
        public List<BlockSave> getSavedMap()
        {
            return tiles;
        }

        /// <summary>
        ///  Set the location that the player starts when the level starts.
        /// </summary>
        /// <param name="position">The grid coordinates for the player's start.</param>
        public void setPlayerStartPosition(Vector2 position)
        {
            playerStartPosition = position;
        }

        /// <summary>
        /// Get the location that the player starts at for the level.
        /// </summary>
        /// <returns>The location.</returns>
        public Vector2 getPlayerStartPosition()
        {
            return playerStartPosition;
        }

        /// <summary>
        /// Store the collection of monsters into an array.
        /// </summary>
        /// <param name="monsters">The array of monsters that are on the level.</param>
        public void storeMonsters(ArrayList monsters)
        {
            this.monsters = new List<MonsterSave>();

            foreach (Monster m in monsters)
            {
                this.monsters.Add(m.createSaveState());
            }
        }

        /// <summary>
        /// The saved monsters that can be recreated using the object factory.
        /// </summary>
        /// <returns>The array of monsters.</returns>
        public List<MonsterSave> getSavedMonsters()
        {
            return monsters;
        }

        /// <summary>
        /// Set a collection of the level properties together.
        /// </summary>
        /// <param name="levelNumber">The number of the level.</param>
        /// <param name="levelName">The name of the level.</param>
        /// <param name="countDown">The time that the player has to complete the level.</param>
        public void setLevelProperties(int levelNumber, String levelName, int countDown)
        {
            this.levelNumber = levelNumber;
            this.levelName = levelName;
            this.countDown = countDown;
        }

        /// <summary>
        /// Get the level number.
        /// </summary>
        /// <returns>The level number.</returns>
        public int getLevelNumber()
        {
            return levelNumber;
        }

        /// <summary>
        /// Get the name of the level.
        /// </summary>
        /// <returns>The level name.</returns>
        public String getLevelName()
        {
            return levelName;
        }

        /// <summary>
        /// Get the time allowed for the level.
        /// </summary>
        /// <returns>The time allowed for completion.</returns>
        public int getCountDown()
        {
            return countDown;
        }
        #endregion
    }
}
