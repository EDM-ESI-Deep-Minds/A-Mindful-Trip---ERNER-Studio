using UnityEngine;

[System.Serializable]
public class PathTile
{
    public string tileType;
    public bool isIntersection;
    public Vector3Int[] possibleMoves; // Stores valid movement directions
    public bool changeDirection = false;
    public bool falseIntersection = false;
    public bool cityFalseIntersection = false;
    public bool noUp = false;
    public bool noDown = false;
    public PathTile(string name)
    {
        tileType = name;
        isIntersection = false;
        possibleMoves = new Vector3Int[0]; // Initialize empty
    }
}