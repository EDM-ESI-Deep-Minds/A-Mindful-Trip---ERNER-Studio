using UnityEngine;

[System.Serializable]
public class PathTile
{
    public string tileName;
    public bool isIntersection;
    public Vector3Int[] possibleMoves; // Stores valid movement directions

    public PathTile(string name)
    {
        tileName = name;
        isIntersection = false;
        possibleMoves = new Vector3Int[0]; // Initialize empty
    }
}