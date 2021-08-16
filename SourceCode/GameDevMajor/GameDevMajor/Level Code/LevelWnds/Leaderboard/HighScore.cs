using System;

namespace GameDevMajor {

    internal class HighScore : IComparable, IEquatable<HighScore> {

        String playerName;
        int playerScore;

        internal HighScore(String name, int score) {

            playerName = name;
            playerScore = score;
        }

        internal int getScore() {

            return playerScore;
        }

        internal String getName() {

            return playerName;
        }

        public int CompareTo(Object obj) {

            HighScore hs = (HighScore)obj;

            if (this.playerScore > hs.playerScore)
                return 1;
            else if (this.playerScore == hs.playerScore)
                return 0;
            else
                return -1;
        }

        public Boolean Equals(HighScore other) {

            return (playerName.Equals(other.playerName) && playerScore == other.playerScore);
        }

        public override string ToString() {

            return "Highscore: " + playerName + "," + playerScore;
        }
    }
}
