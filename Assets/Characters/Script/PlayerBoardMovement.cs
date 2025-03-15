using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UI;

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
    private char currentDirection = 'x';
    public Button rightArrow;
    public Button leftArrow;
    public Button upArrow;
    public Button downArrow;
    private bool isChoosingDirection = false;
    private string selectedDirection = "";

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
        DetermineDefaultDirection();

        rightArrow = GameObject.Find("RightArrow").GetComponent<Button>();
        leftArrow = GameObject.Find("LeftArrow").GetComponent<Button>();
        upArrow = GameObject.Find("UpArrow").GetComponent<Button>();
        downArrow = GameObject.Find("DownArrow").GetComponent<Button>();



        // Assign button listeners dynamically
        rightArrow.onClick.AddListener(() => SetChosenDirection("right"));
        leftArrow.onClick.AddListener(() => SetChosenDirection("left"));
        upArrow.onClick.AddListener(() => SetChosenDirection("up"));
        downArrow.onClick.AddListener(() => SetChosenDirection("down"));

        HideArrows();

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

    private void HideArrows()
    {
        rightArrow.gameObject.SetActive(false);
        leftArrow.gameObject.SetActive(false);
        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);
    }

    private void ShowArrows()
    {
        isChoosingDirection = true; // Stop movement
        rightArrow.gameObject.SetActive(true);
        leftArrow.gameObject.SetActive(true);
        upArrow.gameObject.SetActive(true);
        downArrow.gameObject.SetActive(true);
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

    private void DetermineDefaultDirection()
    {
        Vector3Int[] validMoves = boardManager.pathTiles[currentTilePos].possibleMoves;

        if (validMoves.Length == 0)
        {
            currentDirection = 'x'; 
            return;
        }

        Vector3Int offset = validMoves[0]; 

        if (Mathf.Abs(offset.x) == 3 && offset.y == 0)
        {
            currentDirection = 'x'; 
        }
        else if (offset.x != 0 && offset.y != 0)
        {
          
              currentDirection = 'y'; 
         
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

            if (boardManager.pathTiles[currentTilePos].isIntersection)
            {
                ShowArrowsBasedOnMoves();
                selectedDirection = ""; 
                isChoosingDirection = true;
                yield return new WaitUntil(() => !isChoosingDirection);
            }
            Vector3Int nextTilePos = Vector3Int.zero;
            if (boardManager.pathTiles[currentTilePos].isIntersection)
            {

                foreach (Vector3Int move in currentTile.possibleMoves)
                {
                    Vector3Int potentialNextPos = currentTilePos + move;

                    if (boardManager.pathTiles.ContainsKey(potentialNextPos))
                    {
                        Vector3Int moveOffset = potentialNextPos - currentTilePos;

                        if (currentDirection == 'x')
                        {
                            if (selectedDirection == "right" &&
                                (moveOffset == new Vector3Int(3, 0, 0) || moveOffset == new Vector3Int(4, 0, 0) || moveOffset == new Vector3Int(2, 0, 0)
                                ))
                            {
                                nextTilePos = potentialNextPos;
                                break;
                            }

                            if (selectedDirection == "up" &&
                                (moveOffset == new Vector3Int(1, 2, 0) ||
                                 moveOffset == new Vector3Int(2, 2, 0) ))
                            {
                                nextTilePos = potentialNextPos;
                                break;
                            }

                            if (selectedDirection == "down" &&
                                (moveOffset == new Vector3Int(1, -2, 0)  ||
                                 moveOffset == new Vector3Int(2, -2, 0) ))
                            {
                                nextTilePos = potentialNextPos;
                                break;
                            }
                        }
                        else if (currentDirection == 'y')
                        {
                            if (selectedDirection == "up" &&
                                (moveOffset == new Vector3Int(0, 3, 0) || moveOffset == new Vector3Int(0, 4, 0) || moveOffset == new Vector3Int(0, 2, 0)))
                            {
                                nextTilePos = potentialNextPos;
                                break;
                            }

                            if (selectedDirection == "down" &&
                                (moveOffset == new Vector3Int(0, -3, 0) || moveOffset == new Vector3Int(0, -4, 0) || moveOffset == new Vector3Int(0, -2, 0)))
                            {
                                nextTilePos = potentialNextPos;
                                break;
                            }

                            if (selectedDirection == "right" &&
                                (moveOffset == new Vector3Int(2, 1, 0) || moveOffset == new Vector3Int(1, 1, 0) ||
                                 moveOffset == new Vector3Int(2, -1, 0) || moveOffset == new Vector3Int(1, -1, 0)))
                            {
                                nextTilePos = potentialNextPos;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // Find the valid move with a greater or equal X (or Y) compared to current position
                foreach (Vector3Int move in currentTile.possibleMoves)
                {
                    Vector3Int potentialNextPos = currentTilePos + move;
                    // Determine next tile based on chosen direction and allowed offsets
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

            // Move based on the current direction
            if (currentDirection == 'x')
            {
                // Move along X first
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
                // If Y component exists, move along Y and switch direction
                if (offset.y != 0)
                {
                    SetAnimation(new Vector3(0, offset.y, 0));
                    Vector3 targetYPos = new Vector3(rb.position.x, targetPos.y, 0);
                    while (Mathf.Abs(rb.position.y - targetYPos.y) > 0.016f)
                    {
                        rb.MovePosition(Vector3.MoveTowards(rb.position, targetYPos, moveSpeed * Time.deltaTime));
                        yield return null;
                    }
                    currentDirection = 'y'; // Switch to Y direction
                }
            }
            else if (currentDirection == 'y')
            {
                // Move along Y first
                if (offset.y != 0)
                {
                    SetAnimation(new Vector3(0, offset.y, 0));
                    Vector3 targetYPos = new Vector3(rb.position.x, targetPos.y, 0);
                    while (Mathf.Abs(rb.position.y - targetYPos.y) > 0.016f)
                    {
                        rb.MovePosition(Vector3.MoveTowards(rb.position, targetYPos, moveSpeed * Time.deltaTime));
                        yield return null;
                    }
                }
                // If X component exists, move along X and switch direction
                if (offset.x != 0)
                {
                    SetAnimation(new Vector3(offset.x, 0, 0));
                    Vector3 targetXPos = new Vector3(targetPos.x, rb.position.y, 0);
                    while (Mathf.Abs(rb.position.x - targetXPos.x) > 0.016f)
                    {
                        rb.MovePosition(Vector3.MoveTowards(rb.position, targetXPos, moveSpeed * Time.deltaTime));
                        yield return null;
                    }
                    currentDirection = 'x'; // Switch to X direction
                }
            }
            yield return new WaitForSeconds(0.2f);
            rb.position = targetPos;
            currentTilePos = nextTilePos; 

  
        }

        isMoving = false;
        yield return new WaitForSeconds(0.1f);
        SetIdleAnimation();
    }

    private void SetChosenDirection(string direction)
    {
        selectedDirection = direction;
        Debug.Log($"here is the selected direction by the user{selectedDirection}");
        isChoosingDirection = false; 
        HideArrows(); 
    }

    void ShowArrowsBasedOnMoves()
    {
        HideArrows(); 

        foreach (Vector3Int move in boardManager.pathTiles[currentTilePos].possibleMoves)
        {
            Vector3Int offset = move;
            if (currentDirection == 'x')
            {
                if (offset == new Vector3Int(3, 0, 0) || offset == new Vector3Int(4, 0, 0) ||
                    offset == new Vector3Int(2, 0, 0))
                {
                    rightArrow.gameObject.SetActive(true);
                }
                 

                if (offset == new Vector3Int(1, 2, 0) ||
                    offset == new Vector3Int(2, 2, 0) )
                {
                    upArrow.gameObject.SetActive(true);
                }
                    

                if (offset == new Vector3Int(1, -2, 0)  ||
                    offset == new Vector3Int(2, -2, 0) )
                {
                    downArrow.gameObject.SetActive(true);
                }
                    
            }
            else if (currentDirection == 'y')
            {
                if (offset == new Vector3Int(0, 3, 0) || offset == new Vector3Int(0, 4, 0) ||
                    offset == new Vector3Int(0, 2, 0))
                {
                    upArrow.gameObject.SetActive(true);
                }
                    

                if (offset == new Vector3Int(0, -3, 0) || offset == new Vector3Int(0, -4, 0) ||
                    offset == new Vector3Int(0, -2, 0))
                {
                    downArrow.gameObject.SetActive(true);
                }  

                if (offset == new Vector3Int(2, 1, 0) || offset == new Vector3Int(1, 1, 0) ||
                    offset == new Vector3Int(2, -1, 0) || offset == new Vector3Int(1, -1, 0))
                {
                    rightArrow.gameObject.SetActive(true);
                }

            
            }
        }

        isChoosingDirection = true; // Stop movement until the player selects a direction
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
