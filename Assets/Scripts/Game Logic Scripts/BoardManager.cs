using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    private bool hasRunGizmos = false;
    public static BoardManager Instance { get; private set; }
    public Tilemap boardTilemap;
    public Dictionary<Vector3Int, PathTile> pathTiles = new Dictionary<Vector3Int, PathTile>();
    private List<Vector3Int> pathTilePositions = new List<Vector3Int>();
    public static List<Vector3> storedStartPositions = new List<Vector3>();
    public static List<Vector3> storedEndPositions = new List<Vector3>();

    void Start()
    {
        InitializeBoard();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent multiple instances
            return;
        }
        Instance = this;
    }

    void InitializeBoard()
    {
        BoundsInt bounds = boardTilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = boardTilemap.GetTile(pos);
            if (tile != null)
            {
                PathTile newTile = new PathTile(tile.name); 
                newTile.isIntersection = false;
                pathTiles[pos] = newTile;


                Vector3 worldPos = boardTilemap.GetCellCenterWorld(pos);
                pathTilePositions.Add(pos); // Store grid position instead of world position
            }
        }

        // get the possible moves 
        foreach (var tileEntry in pathTiles)
        {
            Vector3Int pos = tileEntry.Key;
            tileEntry.Value.possibleMoves = GetValidMoves(pos);
        }

        Debug.Log($"Total Path Tiles Initialized: {pathTiles.Count}");
        GetStartTiles();
        FindIntersections();
        GetEndTiles();
    }

    private Vector3Int[] GetValidMoves(Vector3Int tilePos)
    {
        List<Vector3Int> validMoves = new List<Vector3Int>();

        // Define all possible movement offsets (matching GetStartTiles)
        Vector3Int[] neighborOffsets = new Vector3Int[]
        {
        new Vector3Int(3, 0, 0), new Vector3Int(-3, 0, 0),
        new Vector3Int(0, 3, 0), new Vector3Int(0, -3, 0),
        new Vector3Int(2, 0, 0), new Vector3Int(-2, 0, 0),
        new Vector3Int(0, 2, 0), new Vector3Int(0, -2, 0),
        new Vector3Int(2, 1, 0), new Vector3Int(2, -1, 0),
        new Vector3Int(-2, 1, 0), new Vector3Int(-2, -1, 0),
        new Vector3Int(1, 2, 0), new Vector3Int(-1, 2, 0),
        new Vector3Int(1, -2, 0), new Vector3Int(-1, -2, 0),
        new Vector3Int(2, 2, 0), new Vector3Int(-2, -2, 0),
        new Vector3Int(2, -2, 0), new Vector3Int(-2, 2, 0),
        new Vector3Int(1, 1, 0), new Vector3Int(-1, 1, 0),
        new Vector3Int(1, -1, 0), new Vector3Int(-1, -1, 0), // Adding 1 and 2 (missing cases)
        new Vector3Int(1, 2, 0), new Vector3Int(-1, 2, 0), // Same but re-checking for moves
        new Vector3Int(1, -2, 0), new Vector3Int(-1, -2, 0),
        new Vector3Int(4, 0, 0), new Vector3Int(-4, 0, 0),
        new Vector3Int(0, 4, 0), new Vector3Int(0, -4, 0)
        };

        foreach (Vector3Int offset in neighborOffsets)
        {
            Vector3Int neighborPos = tilePos + offset;

            // Check if the position exists in pathTiles
            if (pathTiles.ContainsKey(neighborPos))
            {
                bool isValidMove = true;

                // Check for diagonal or combined moves: both x and y change
                if (offset.x != 0 && offset.y != 0)
                {
                    // Allow diagonal moves (both x and y changing)
                    if (neighborPos.x == tilePos.x || neighborPos.y == tilePos.y)
                    {
                        isValidMove = false;
                    }
                }
                else
                {
                    // Handle axis-aligned moves: either x or y changes
                    if ((offset.x != 0 && neighborPos.y != tilePos.y) || (offset.y != 0 && neighborPos.x != tilePos.x))
                    {
                        isValidMove = false;
                    }
                }

                // If valid, add the move to the list
                if (isValidMove)
                {
                    validMoves.Add(offset);
                }
            }
        }

        return validMoves.ToArray();
    }

    private void FindIntersections()
    {
        foreach (var tilePos in pathTiles.Keys)
        {
            Vector3Int[] validMoves = GetValidMoves(tilePos); // Get possible moves for this tile

            if (validMoves.Length > 2 && !HasOneUnitNeighbor(tilePos)) // More than 2 means it's an intersection
            {
                pathTiles[tilePos].isIntersection = true;
            }
        }
    }

    private bool HasOneUnitNeighbor(Vector3Int tilePos)
    {
        // Define 1-unit neighbor offsets
        Vector3Int[] oneUnitOffsets = new Vector3Int[]
        {
        new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0)
        };

        foreach (Vector3Int offset in oneUnitOffsets)
        {
            if (pathTiles.ContainsKey(tilePos + offset))
            {
                return true; // Found a 1-unit neighbor, so it's not a valid intersection
            }
        }

        return false; // No 1-unit neighbors, intersection remains valid
    }


    public bool IsPathTile(Vector3Int pos)
    {
        return pathTiles.ContainsKey(pos);
    }

    void OnDrawGizmos()
    {

        if (pathTilePositions == null || pathTiles == null) return;

        // Draw path tiles in red
        Gizmos.color = Color.red;
        foreach (Vector3Int pos in pathTilePositions)
        {
            Vector3 worldPos = boardTilemap.GetCellCenterWorld(pos);
            Gizmos.DrawCube(worldPos, new Vector3(0.3f, 0.3f, 0.1f));
        }

        // Draw start tiles in blue
        Gizmos.color = Color.blue;
        List<Vector3Int> startTiles = GetStartTiles();
        foreach (Vector3Int tilePos in startTiles)
        {
            Vector3 worldPos = boardTilemap.GetCellCenterWorld(tilePos);
            Gizmos.DrawSphere(worldPos, 0.2f);
        }

        // Draw start tiles in blue
        Gizmos.color = Color.black;
        List<Vector3Int> EndTiles = GetEndTiles();
        foreach (Vector3Int tilePos in EndTiles)
        {
            Vector3 worldPos = boardTilemap.GetCellCenterWorld(tilePos);
            Gizmos.DrawSphere(worldPos, 0.2f);
        }

        // Draw intersection tiles in GREEN
        Gizmos.color = Color.green;
        foreach (var tileEntry in pathTiles)
        {
            if (tileEntry.Value.isIntersection)
            {
                Vector3 worldPos = boardTilemap.GetCellCenterWorld(tileEntry.Key);
                Gizmos.DrawCube(worldPos, new Vector3(0.3f, 0.3f, 0.1f));
            }
        }


    }

    List<Vector3Int> GetStartTiles()
    {
        List<Vector3Int> potentialStartTiles = new List<Vector3Int>();

        // Define all possible neighbor offsets
        Vector3Int[] neighborOffsets = new Vector3Int[]
        {
        new Vector3Int(3, 0, 0), new Vector3Int(-3, 0, 0),
        new Vector3Int(0, 3, 0), new Vector3Int(0, -3, 0),
        new Vector3Int(2, 0, 0), new Vector3Int(-2, 0, 0),
        new Vector3Int(0, 2, 0), new Vector3Int(0, -2, 0),
        new Vector3Int(2, 1, 0), new Vector3Int(2, -1, 0),
        new Vector3Int(-2, 1, 0), new Vector3Int(-2, -1, 0),
        new Vector3Int(1, 2, 0), new Vector3Int(-1, 2, 0),
        new Vector3Int(1, -2, 0), new Vector3Int(-1, -2, 0),
        new Vector3Int(2, 2, 0), new Vector3Int(-2, -2, 0),
        new Vector3Int(2, -2, 0), new Vector3Int(-2, 2, 0)
        };

        foreach (var pos in pathTiles.Keys)
        {
            int neighborCount = 0;
            bool hasLeftNeighbor = false;

            foreach (Vector3Int offset in neighborOffsets)
            {
                Vector3Int neighborPos = pos + offset;
                if (pathTiles.ContainsKey(neighborPos))
                {
                    neighborCount++;

                    if (offset == new Vector3Int(-3, 0, 0) || offset == new Vector3Int(-2, 0, 0) ||
                        offset == new Vector3Int(-2, 1, 0) || offset == new Vector3Int(-2, -1, 0))
                    {
                        hasLeftNeighbor = true;
                    }
                }
            }

            // Only add if it's a valid start tile
            if (neighborCount == 1 && !hasLeftNeighbor)
            {
                potentialStartTiles.Add(pos);
            }
        }

        // Sort by X position (smallest first)
        potentialStartTiles.Sort((a, b) => a.x.CompareTo(b.x));

        // Pick the first 4
        List<Vector3Int> startTiles = potentialStartTiles.Take(4).ToList();

        storedStartPositions.Clear();
        foreach (var tile in startTiles)
        {
            storedStartPositions.Add(boardTilemap.GetCellCenterWorld(tile));
        }

        foreach (var tile in startTiles)
        {
            Vector3 worldPos = boardTilemap.GetCellCenterWorld(tile);
            Debug.Log($"Start Tile at Grid Pos: {tile}, World Pos: {worldPos}");
        }

        Debug.Log($"Final Start Tiles Count: {startTiles.Count}");
        return startTiles;
    }

    List<Vector3Int> GetEndTiles()
    {
        List<Vector3Int> potentialEndTiles = new List<Vector3Int>();

        // Define all possible neighbor offsets
        Vector3Int[] neighborOffsets = new Vector3Int[]
        {
        new Vector3Int(3, 0, 0), new Vector3Int(-3, 0, 0),
        new Vector3Int(0, 3, 0), new Vector3Int(0, -3, 0),
        new Vector3Int(2, 0, 0), new Vector3Int(-2, 0, 0),
        new Vector3Int(0, 2, 0), new Vector3Int(0, -2, 0),
        new Vector3Int(2, 1, 0), new Vector3Int(2, -1, 0),
        new Vector3Int(-2, 1, 0), new Vector3Int(-2, -1, 0),
        new Vector3Int(1, 2, 0), new Vector3Int(-1, 2, 0),
        new Vector3Int(1, -2, 0), new Vector3Int(-1, -2, 0),
        new Vector3Int(2, 2, 0), new Vector3Int(-2, -2, 0),
        new Vector3Int(2, -2, 0), new Vector3Int(-2, 2, 0)
        };

        foreach (var pos in pathTiles.Keys)
        {
            int neighborCount = 0;
            bool hasRightNeighbor = false;

            foreach (Vector3Int offset in neighborOffsets)
            {
                Vector3Int neighborPos = pos + offset;
                if (pathTiles.ContainsKey(neighborPos))
                {
                    neighborCount++;

                    // Check if it has a right-side neighbor
                    if (offset == new Vector3Int(3, 0, 0) || offset == new Vector3Int(2, 0, 0) ||
                        offset == new Vector3Int(2, 1, 0) || offset == new Vector3Int(2, -1, 0))
                    {
                        hasRightNeighbor = true;
                    }
                }
            }

            // Only add if it's a valid end tile (1 neighbor and it's on the right)
            if (neighborCount == 1 && !hasRightNeighbor)
            {
                potentialEndTiles.Add(pos);
            }
        }

        // Sort by X position (largest first)
        potentialEndTiles.Sort((a, b) => b.x.CompareTo(a.x));

        // Pick the first 4
        List<Vector3Int> endTiles = potentialEndTiles.Take(4).ToList();

        storedEndPositions.Clear();
        foreach (var tile in endTiles)
        {
            storedEndPositions.Add(boardTilemap.GetCellCenterWorld(tile));
        }

        foreach (var tile in endTiles)
        {
            Vector3 worldPos = boardTilemap.GetCellCenterWorld(tile);
            Debug.Log($"End Tile at Grid Pos: {tile}, World Pos: {worldPos}");
        }

        Debug.Log($"Final End Tiles Count: {endTiles.Count}");
        return endTiles;
    }


    public static Vector3 GetWorldPositionFromGrid(float x, float y)
    {
        if (Instance == null || Instance.boardTilemap == null)
        {
            Debug.LogError("BoardManager or Board Tilemap is not assigned!");
            return Vector3.zero;
        }

        Vector3Int gridPosition = new Vector3Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y), 0);
        return Instance.boardTilemap.GetCellCenterWorld(gridPosition);
    }

    public static Vector3Int GetGridPositionFromWorld(Vector3 worldPosition)
    {
        if (Instance == null || Instance.boardTilemap == null)
        {
            Debug.LogError("BoardManager or Board Tilemap is not assigned!");
            return Vector3Int.zero;  // Return zero if something is wrong
        }

        // Convert world position to grid position
        return Instance.boardTilemap.WorldToCell(worldPosition);
    }



}