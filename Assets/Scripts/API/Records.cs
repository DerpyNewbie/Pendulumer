using System;
using System.Collections.Generic;

namespace API
{
    [Serializable]
    public record UserRecord(
        int Id,
        string Name,
        int TotalScoreCount,
        int TotalJumpCount,
        int TotalGamePlayedCount,
        float TotalTimePlayedInSeconds)
    {
        public int Id { get; } = Id;
        public string Name { get; } = Name;
        public int TotalScoreCount { get; } = TotalScoreCount;
        public int TotalJumpCount { get; } = TotalJumpCount;
        public int TotalGamePlayedCount { get; } = TotalGamePlayedCount;
        public float TotalTimePlayedInSeconds { get; } = TotalTimePlayedInSeconds;
    }

    [Serializable]
    public struct ResultRecord
    {
        public int id;
        public int userId;
        public int rank;
        public DateTime createdAt;
        public float score;
        public float playtime;
        public float posX;
        public float posY;
        public int clickCount;
        public int jumpCount;
        public int version;
    }

    [Serializable]
    public struct StatsRecord
    {
        public int totalUsers;
        public int totalResults;
        public float totalPlaytime;
        public int totalJumpCount;
        public int totalClickCount;
    }

    [Serializable]
    public struct LeaderboardRecord
    {
        public int rank;
        public string name;
        public float score;
        public int userId;
        public int resultId;
    }

    [Serializable]
    public struct LeaderboardResponse
    {
        public int count;
        public int offset;
        public List<LeaderboardRecord> records;
    }
}