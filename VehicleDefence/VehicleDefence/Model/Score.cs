namespace VehicleDefence.Model
{
    struct Score
    {
        private int gameScore;
        private int gameHighScore;

        public Score(int score, int highScore)
        {
            gameScore = score;
            gameHighScore = highScore;
        }

        public void UpdateGameScore(int score)
        {
            gameScore = score;
        }

        public void UpdateGameHighScore(int score)
        {
            gameHighScore = score;
        }

        public int GetGameScore()
        {
            return gameScore;
        }

        public int GetGameHighScore()
        {
            return gameHighScore;
        }
    }
}
