using System;
using System.Collections.Generic;

[Serializable]
public class InputEntry
{
    public string playerName;
    public int Elo;
    public int character;
    public CategoryElo[] categories;

    public InputEntry(string name, int Elo, int character, CategoryElo[] categories)
    {
        playerName = name;
        this.Elo = Elo;
        this.character = character;
        this.categories = categories;
    }
}

[Serializable]
public class CategoryElo
{
    public string categoryName;
    public int categoryElo;
    public int questionsAnswered;
    public int correctAnswers;

    public CategoryElo(string categoryName, int categoryElo, int questionsAnswered, int correctAnswers)
    {
        this.categoryName = categoryName;
        this.categoryElo = categoryElo;
        this.questionsAnswered = questionsAnswered;
        this.correctAnswers = correctAnswers;
    }
}