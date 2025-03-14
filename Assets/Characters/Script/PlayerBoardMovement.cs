using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerBoardMovement : NetworkBehaviour
{

    private DiceManager diceManager;
    private BoardManager boardManager;
    [SerializeField] private float moveSpeed = 1f;
    private Rigidbody2D rb;
    private bool isMoving = false;
    private Animator animator;
    private Vector3Int currentTilePos; 
    public Tilemap boardTilemap;
    private Vector3Int currentGridPosition;
    private Vector3 lastDirection = Vector3.right;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner) 
        {
            StartCoroutine(FindDiceManager()); 
            StartCoroutine(FindBoardManager());
        }
    }

    private IEnumerator FindBoardManager()
    {
        while (boardManager == null)
        {
            boardManager = FindFirstObjectByType<BoardManager>(); 

            if (boardManager == null)
            {
                Debug.LogWarning("BoardManager not found, retrying...");
                yield return new WaitForSeconds(0.5f); 
            }
        }

        Vector3 worldPosition = transform.position;
        Vector3Int gridPosition = BoardManager.GetGridPositionFromWorld(worldPosition);
        currentTilePos = gridPosition;

    }

    private IEnumerator FindDiceManager()
    {
        while (diceManager == null)
        {
            diceManager = FindFirstObjectByType<DiceManager>();

            if (diceManager == null)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
        diceManager.OnDiceRolled += OnDiceRolled;
    }

  

    void Start()
    {
        animator = GetComponent<Animator>();
        if (boardManager == null)
        {
            Debug.LogError("BoardManager is not found! Ensure BoardManager exists in the scene.");
            return;
        }

        if (boardManager.pathTiles == null)
        {
            Debug.LogError("pathTiles in BoardManager is not initialized!");
            return;
        }

        if (diceManager != null)
        {
            diceManager.OnDiceRolled += OnDiceRolled;
        }
        else
        {
            Debug.LogError("DiceManager not found in the scene!");
        }

      


    }


    private void OnDiceRolled(int dice1, int dice2)
    {
        if (!IsOwner) return;

        int totalSteps = dice1 + dice2;
        MovePlayer(totalSteps);
    }

    public void MovePlayer(int steps)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveAlongPath(steps));
        }
    }

    private IEnumerator MoveAlongPath(int steps)
    {
        isMoving = true;
        

        for (int i = 0; i < steps; i++)
        {
            if (!boardManager.pathTiles.ContainsKey(currentTilePos))
            {
                Debug.Log("Invalid current tile position: " + currentTilePos);
                break;
            }
            PathTile currentTile = boardManager.pathTiles[currentTilePos];
            if (currentTile.possibleMoves.Length == 0)
            {
                Debug.Log("No valid moves from current tile: " + currentTilePos);  // Debug: No valid moves
                break;
            }

            // Find the valid move with a greater or equal X (or Y) compared to current position
            Vector3Int nextTilePos = Vector3Int.zero;
            foreach (Vector3Int move in currentTile.possibleMoves)
            {
                Vector3Int potentialNextPos = currentTilePos + move;

                if (boardManager.pathTiles.ContainsKey(potentialNextPos))
                {
                    // Prioritize moves with greater or equal X (or Y)
                    if (nextTilePos == Vector3Int.zero ||
                        (move.x >= 0 && potentialNextPos.x >= nextTilePos.x))
                    {
                        nextTilePos = potentialNextPos;
                    }
                }
            }

            // If no valid move found, break
            if (nextTilePos == Vector3Int.zero)
            {
                Debug.Log("No valid next tile found!");  
                break;
            }

           
            Vector3Int offset = nextTilePos - currentTilePos;

           
            Vector3 direction = offset;
            SetAnimation(direction);

            Vector3 targetPos = GetWorldPosition(nextTilePos); 

            
            if (offset.x != 0)
            {
                SetAnimation(new Vector3(offset.x, 0, 0));
                Vector3 targetXPos = new Vector3(targetPos.x, rb.position.y, 0);
                while (Mathf.Abs(rb.position.x - targetXPos.x) > 0.016f)
                {
                    rb.MovePosition(Vector3.MoveTowards(rb.position, targetXPos, moveSpeed * Time.deltaTime));
                    yield return null;
                }

            }

            // Then, move along the Y axis
            if (offset.y != 0)
            {
                SetAnimation(new Vector3(0, offset.y, 0));
                Vector3 targetYPos = new Vector3(rb.position.x, targetPos.y,0);
                while (Mathf.Abs(rb.position.y - targetYPos.y) > 0.016f)
                {
                    rb.MovePosition(Vector3.MoveTowards(rb.position, targetYPos, moveSpeed * Time.deltaTime));
                    yield return null;
                }
                yield return new WaitForSeconds(0.2f);
            }

            rb.position = targetPos;
            currentTilePos = nextTilePos; 

  
        }

        isMoving = false;
        yield return new WaitForSeconds(0.1f);
        SetIdleAnimation();
    }


    private void SetAnimation(Vector3 direction)
    {
        if (direction.x > 0)
        {
            animator.Play("move_right");
            lastDirection = Vector3.right;
        }
        else if (direction.x < 0)
        {
            animator.Play("move_left");
            lastDirection = Vector3.left;
        }
        else if (direction.y > 0)
        {
            animator.Play("move_up");
            lastDirection = Vector3.up;
        }
        else if (direction.y < 0)
        {
            animator.Play("move_down");
            lastDirection = Vector3.down;
        }
    }
    private void SetIdleAnimation()
    {
        if (lastDirection == Vector3.right)
            animator.Play("idle_right");
        else if (lastDirection == Vector3.left)
            animator.Play("idle_left");
        else if (lastDirection == Vector3.up)
            animator.Play("idle_up");
        else if (lastDirection == Vector3.down)
            animator.Play("idle_down");
    }

    private Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        return new Vector3(gridPos.x * 0.16f, gridPos.y * 0.16f, 0); // Convert to world space
    }


    void OnDestroy()
    {
        if (diceManager != null)
        {
            diceManager.OnDiceRolled -= OnDiceRolled; // Unsubscribe to prevent memory leaks
        }
    }
}
