using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace GameDevMajor
{
    [XmlType(AnonymousType = true, Namespace = "")]
    public class LevelInfo : IComparable
    {
        /// <summary>
        /// The number of the level.
        /// </summary>
        public int levelNumber { get; set; }

        /// <summary>
        /// A string representing the name of the level.
        /// </summary>
        public String levelName { get; set; }

        public LevelInfo()
        {
            this.levelNumber = 0;
            this.levelName = "";
        }

        public LevelInfo(int levelNumber, String levelName, bool editable)
        {
            this.levelNumber = levelNumber;
            this.levelName = levelName;
        }

        /// <summary>
        /// Set the level number.
        /// </summary>
        /// <param name="levelNumber">The number of the level.</param>
        public void setLevelNumber(int levelNumber)
        {
            this.levelNumber = levelNumber;
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
        /// Set the level name.
        /// </summary>
        /// <param name="levelNumber">The number of the level.</param>
        public void setLevelName(String levelName)
        {
            this.levelName = levelName;
        }

        /// <summary>
        /// Get the name of the level.
        /// </summary>
        /// <returns>The level name.</returns>
        public String getLevelName()
        {
            return levelName;
        }

        public int CompareTo(object obj)
        {
            LevelInfo o = (LevelInfo)obj;

            if (o.getLevelNumber() > getLevelNumber())
            {
                return -1;
            }
            else if (o.getLevelNumber() < getLevelNumber())
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
