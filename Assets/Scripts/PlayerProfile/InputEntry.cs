using System;

[Serializable]
public class InputEntry
{
    public string playerName;
    public int Elo;
    public int character;

    public InputEntry(string name, int Elo,int character)
    {
        playerName = name;
        this.Elo = Elo;
        this.character = character;
    }
}