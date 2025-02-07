using System;

namespace Game
{
    [Serializable]
    public class GameResult
    {
        public string name;
        public float score;
        public float playtime;
        public float posX;
        public float posY;
        public int clickCount;
        public int jumpCount;
        public int version;

        [NonSerialized]
        public bool HasSent;
    }
}