using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDevMajor
{
    [Serializable]
    public class SavedLevelList
    {
        public List<LevelInfo> levels;
        public int currentLevel;
        public int totalScore;
        public int nextLevelNumber;

        public SavedLevelList()
        {
            levels = new List<LevelInfo>();
            nextLevelNumber = 1;
            currentLevel = 0;
        }

        public void AddLevel(LevelInfo info)
        {
            levels.Add(info);
            sortList();
        }

        public List<LevelInfo> getLevelList()
        {
            return levels;
        }

        public int getNextLevelNumber()
        {
            return nextLevelNumber++;
        }

        public int getCurrentLevelNumber()
        {
            return currentLevel;
        }

        public void nextLevel()
        {
            if (currentLevel+1 >= levels.Count) resetLevel();
            else currentLevel++;
        }

        public void resetLevel()
        {
            currentLevel = 0;
        }

        public void setLevelNumber(int levelNumber)
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].levelNumber == levelNumber)
                {
                    currentLevel = i;
                    return;
                }
            }
            currentLevel = 0;
            totalScore = 0;
        }

        public void resetTotalScore()
        {
            totalScore = 0;
        }



        public void updateLevelName(int levelNumber, string levelName)
        {
            foreach (LevelInfo level in levels)
            {
                if (level.levelNumber == levelNumber)
                {
                    level.levelName = levelName;
                    return;
                }
            }
        }

        public int getTotalScore()
        {
            return totalScore;
        }

        public void setTotalScore(int score)
        {
            totalScore = score;
        }

        public int updateTotalScore(int amount)
        {
            totalScore += amount;
            return totalScore;
        }

        private void sortList()
        {
            levels.Sort(delegate(LevelInfo l1, LevelInfo l2)
            {
                return l1.getLevelNumber().CompareTo(l2.getLevelNumber());
            });
        }
    }
}
