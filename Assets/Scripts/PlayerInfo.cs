using UnityEngine;

public class PlayerInfo
{
    public string playerName;
    public int score;

    public PlayerInfo(string playerName)
    {
        this.playerName = playerName;
        score = 0;
    }
}
