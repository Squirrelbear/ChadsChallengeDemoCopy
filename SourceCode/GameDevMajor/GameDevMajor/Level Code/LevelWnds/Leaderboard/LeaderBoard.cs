using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace GameDevMajor {

    internal class LeaderBoard {

        #region Instance Variables
        ///<summary>
        //List containing the highScores for the level
        ///</summary>
        List<HighScore> highScores;

        ///<summary>
        //Name of parent level
        //used to uniquely identify the filename.
        ///</summary>
        String levelName;

        ///<summary>
        //Path & filename where the file is stored
        ///</summary>
        String fileName;

        #endregion

        internal LeaderBoard(String name) {

            levelName = name;
            fileName = "./highscores/" + levelName + ".xml";
            highScores = new List<HighScore>();

            if (!File.Exists(fileName))
                 createFile(fileName);

            loadScores();
        }


        #region Core Functionality

        ///<summary>
        //Adds a new HighScore object to the highscores list
        ///</summary>
        ///<param name="name">The player name</param>
        ///<param name="score">The score achieved on a level</param>
        internal void addHighScore(String name, int score) {

            HighScore hs = new HighScore(name, score);
            highScores.Add(hs);
            sortHighScores();
        }

        ///<summary>
        //Clears the highscore list. This involves:
        // *Deleting the old xml file
        // *Creating a new file with default values.
        // *Loading these default values into highScores
        ///</summary>
        internal void resetScores() {

            highScores.Clear();
            File.Delete(fileName);
            createFile(fileName);
            loadScores();
        }

        ///<summary>
        //Sorts the highScores list in descending order.
        ///</summary>
        internal void sortHighScores() {

            highScores.Sort();
            highScores.Reverse();
        }

        ///<summary>
        //Returns the highScores list
        ///</summary>
        internal List<HighScore> getScores() {

            return highScores;
        }

        ///<summary>
        //returns true if scoreToCheck is >=
        //the position in the list.
        //
        //For example isHighScore(10,10000) returns
        //true if 10000 is in the top ten scores.
        ///<<summary>
        internal Boolean isHighScore(int scoreToCheck) {

            foreach (HighScore hs in highScores) {

                if (scoreToCheck > hs.getScore()) {
                    return true;
                }
            }
            return false;
        }
        #endregion


        #region Load/Save Related

        //Replaces existing highscores XML file with updated one
        internal void saveHighScores() {

            //delete old file
            File.Delete(fileName);

            //create new xml file based on contents of highscores list
            XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8);

            writer.WriteStartDocument();
            writer.WriteStartElement(levelName + "_Highscores");

            foreach (HighScore hs in highScores) {

                writer.WriteStartElement("HighScore");
                writer.WriteStartElement("Name");
                writer.WriteString(hs.getName());
                writer.WriteEndElement();
                writer.WriteStartElement("Score");
                writer.WriteString(hs.getScore().ToString());
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        //Called if an XML file does not exist.
        //Creates an XML file filled with 'anonymous' high-scores
        internal void createFile(String fileName) {

            try {

                XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8);
                writer.WriteStartDocument();
                writer.WriteStartElement(levelName + "_Highscores");

                for (int i = 1; i < 11; i++) {

                    writer.WriteStartElement("HighScore");
                    writer.WriteStartElement("Name");
                    writer.WriteString("Anonymous");
                    writer.WriteEndElement();
                    writer.WriteStartElement("Score");
                    writer.WriteString("0");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }

            //if highscores directory does not exist, create one
            catch (DirectoryNotFoundException) {

                Directory.CreateDirectory("./highscores/");
                createFile(fileName);
            }
        }

        //Populates List<HighScore> highscores with the 
        //contents of the XML file.
        internal void loadScores() {

            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(fileName);
            highScores.Clear();

            foreach (XmlNode node in xmlFile.SelectNodes("descendant::HighScore")) {

                HighScore hs = new HighScore(node.FirstChild.InnerText, int.Parse(node.LastChild.InnerText));
                highScores.Add(hs);
            }

            highScores.Sort();
            highScores.Reverse();

        }
        #endregion
    }

}
