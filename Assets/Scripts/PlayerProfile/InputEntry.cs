using System;

[Serializable]
public class InputEntry
{
    public string playerName;
    public int Elo;

    public InputEntry(string name, int Elo)
    {
        playerName = name;
        this.Elo = Elo;
    }
}