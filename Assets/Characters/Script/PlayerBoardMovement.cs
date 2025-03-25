using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;

public class PlayerBoardMovement : NetworkBehaviour
{
    private DiceManager diceManager;
    private BoardManager boardManager;
    [SerializeField] private float moveSpeed = .1f;
    private Rigidbody2D rb;
    private bool isMoving = false;
    private Animator animator;
    private Vector3Int currentTilePos;
    private Vector3Int previousTilePos;
    public Tilemap boardTilemap;
    private Vector3Int currentGridPosition;
    //private Vector3 lastDirection = Vector3.right;
    private string currentDirection = "";
    public Button rightArrow;
    public Button leftArrow;
    public Button upArrow;
    public Button downArrow;
    private bool isChoosingDirection = false;
    private string selectedDirection = "";
    private bool isCrossingBridge = false;
    private int possibleMoves;
    private Vector3Int onlyOneMove;
    private PathTile currentTilePath;

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
       // if (!IsOwner) return;

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
            currentDirection = "x";
            SetIdleAnimation(0);
            return;
        }

        Vector3Int offset = validMoves[0];

        if (Mathf.Abs(offset.x) == 3 && offset.y == 0)
        {
            currentDirection = "x"; // Moving strictly along X-axis
            SetIdleAnimation(0);
        }
        else if (offset.y != 0)
        {
            if (offset.y > 0)
            {
                currentDirection = "y-up";  // Moving upward
                SetIdleAnimation(1);
            }
            else
            {
                currentDirection = "y-down"; // Moving downward
                SetIdleAnimation(2);
            }
        }
    }

    private IEnumerator MoveAlongPath(int steps)
    {
        isMoving = true;

        Debug.Log($"here is the current tile position {currentTilePos}");

        Debug.Log($"here is the previous tile position {previousTilePos}");

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

            Vector3Int nextTilePos = Vector3Int.zero;

            if (HasMultipleForwardMoves(currentTilePos))
            {
                ShowArrowsBasedOnMoves();
                if (possibleMoves == 1)
                {
                    nextTilePos = currentTilePos + onlyOneMove;
                    HideArrows();
                }
                else
                {
                    selectedDirection = "";
                    isChoosingDirection = true;
                    yield return new WaitUntil(() => !isChoosingDirection);

                    // Now determine the next tile based on the selected direction
                    foreach (Vector3Int move in currentTile.possibleMoves)
                    {
                        Vector3Int potentialNextPos = currentTilePos + move;
                        Vector3Int moveOffset = potentialNextPos - currentTilePos;

                        if (boardManager.pathTiles.ContainsKey(potentialNextPos) &&
                            boardManager.pathTiles[potentialNextPos].isIntersection &&
                            moveOffset == new Vector3Int(2, 0, 0))
                        {
                            nextTilePos = potentialNextPos; // Prioritize intersections
                            break;
                        }

                        if (boardManager.pathTiles.ContainsKey(potentialNextPos))
                        {
                            if (currentDirection == "x")
                            {
                                if (selectedDirection == "right" &&
                                    (moveOffset == new Vector3Int(3, 0, 0) || moveOffset == new Vector3Int(4, 0, 0) ||
                                     moveOffset == new Vector3Int(2, 0, 0) || moveOffset == new Vector3Int(1, 0, 0)))
                                {
                                    nextTilePos = potentialNextPos;
                                    break;
                                }

                                if (selectedDirection == "left" &&
                                    (moveOffset == new Vector3Int(-3, 0, 0) || moveOffset == new Vector3Int(-4, 0, 0) ||
                                     moveOffset == new Vector3Int(-2, 0, 0) || moveOffset == new Vector3Int(-1, 0, 0)))
                                {
                                    nextTilePos = potentialNextPos;
                                    break;
                                }

                                if (selectedDirection == "up" &&
                                    (moveOffset == new Vector3Int(1, 2, 0) || moveOffset == new Vector3Int(0, 3, 0) || moveOffset == new Vector3Int(2, 2, 0) ||
                                     moveOffset == new Vector3Int(2, 1, 0) || moveOffset == new Vector3Int(1, 1, 0) || moveOffset == new Vector3Int(0, 2, 0) ||
                                     moveOffset == new Vector3Int(-1, 2, 0) || moveOffset == new Vector3Int(-2, 2, 0)))
                                {
                                    nextTilePos = potentialNextPos;
                                    break;
                                }

                                if (selectedDirection == "down" &&
                                    (moveOffset == new Vector3Int(1, -2, 0) || moveOffset == new Vector3Int(0, -3, 0) || moveOffset == new Vector3Int(0, -1, 0) ||
                                     moveOffset == new Vector3Int(2, -2, 0) || moveOffset == new Vector3Int(2, -1, 0) || moveOffset == new Vector3Int(1, -1, 0) ||
                                     moveOffset == new Vector3Int(-1, -2, 0) || moveOffset == new Vector3Int(-2, -2, 0)))
                                {
                                    nextTilePos = potentialNextPos;
                                    break;
                                }
                            }
                            else if (currentDirection == "y-up" || currentDirection == "y-down")
                            {
                                if (currentDirection == "y-up")
                                {
                                    if (selectedDirection == "up" &&
                                        (moveOffset == new Vector3Int(0, 3, 0) || moveOffset == new Vector3Int(0, 4, 0) ||
                                         moveOffset == new Vector3Int(0, 2, 0) || moveOffset == new Vector3Int(0, 1, 0)))
                                    {
                                        nextTilePos = potentialNextPos;
                                        break;
                                    }

                                    if (selectedDirection == "right" &&
                                    (moveOffset == new Vector3Int(2, 1, 0) || moveOffset == new Vector3Int(3, 0, 0) ||
                                    moveOffset == new Vector3Int(1, 1, 0) ||
                                    moveOffset == new Vector3Int(2, -1, 0) || moveOffset == new Vector3Int(1, -1, 0) ||
                                    moveOffset == new Vector3Int(2, 0, 0) || moveOffset == new Vector3Int(1, 0, 0)))
                                    {
                                        nextTilePos = potentialNextPos;
                                        Debug.Log($"here is the next tile position {nextTilePos}");
                                        break;
                                    }

                                    if (selectedDirection == "left" &&
                                   (moveOffset == new Vector3Int(-2, 1, 0) || moveOffset == new Vector3Int(-1, 1, 0) ||
                                    moveOffset == new Vector3Int(-2, -1, 0) || moveOffset == new Vector3Int(-1, -1, 0) ||
                                    moveOffset == new Vector3Int(2, -1, 0) ||
                                    moveOffset == new Vector3Int(-2, 0, 0) || moveOffset == new Vector3Int(-1, 0, 0)))
                                    {
                                        nextTilePos = potentialNextPos;
                                        break;
                                    }
                                }
                                else if (currentDirection == "y-down")
                                {
                                    if (selectedDirection == "down" &&
                                        (moveOffset == new Vector3Int(0, -3, 0) || moveOffset == new Vector3Int(0, -4, 0) ||
                                         moveOffset == new Vector3Int(0, -2, 0) || moveOffset == new Vector3Int(0, -1, 0)))
                                    {
                                        nextTilePos = potentialNextPos;
                                        break;
                                    }

                                    if (selectedDirection == "right" &&
                                    (moveOffset == new Vector3Int(2, 1, 0) || moveOffset == new Vector3Int(2, -2, 0) ||
                                    moveOffset == new Vector3Int(1, 1, 0) ||
                                     moveOffset == new Vector3Int(2, -1, 0) || moveOffset == new Vector3Int(1, -1, 0) ||
                                     moveOffset == new Vector3Int(2, 0, 0) || moveOffset == new Vector3Int(1, 0, 0)))
                                    {
                                        nextTilePos = potentialNextPos;
                                        break;
                                    }

                                    if (selectedDirection == "left" &&
                                   (moveOffset == new Vector3Int(-2, 1, 0) || moveOffset == new Vector3Int(-1, 1, 0) ||
                                   moveOffset == new Vector3Int(-1, -2, 0) ||
                                    moveOffset == new Vector3Int(-2, -1, 0) || moveOffset == new Vector3Int(-1, -1, 0) ||
                                    moveOffset == new Vector3Int(-2, 0, 0) || moveOffset == new Vector3Int(-1, 0, 0)))
                                    {
                                        nextTilePos = potentialNextPos;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                
            }
            else
            {
                // Default movement logic for non-intersection tiles
                foreach (Vector3Int move in currentTile.possibleMoves)
                {
                    Vector3Int potentialNextPos = currentTilePos + move;

                    if (boardManager.pathTiles.ContainsKey(potentialNextPos))
                    {
                        // Movement logic based on current sdirection
                        if (currentDirection == "x")
                        {
                            // Pick the tile with the greatest x (or the same x)
                            if (nextTilePos == Vector3Int.zero || potentialNextPos.x >= nextTilePos.x)
                            {
                                nextTilePos = potentialNextPos;
                            }
                        }
                        else if (currentDirection == "y-up")
                        {
                            // Pick the tile with the greatest y (moving up)
                            if (nextTilePos == Vector3Int.zero || potentialNextPos.y >= nextTilePos.y)
                            {
                                nextTilePos = potentialNextPos;
                            }
                        }
                        else if (currentDirection == "y-down")
                        {
                            // Pick the tile with the smallest y (moving down)
                            if (nextTilePos == Vector3Int.zero || potentialNextPos.y <= nextTilePos.y)
                            {
                                nextTilePos = potentialNextPos;
                            }
                        }
                    }
                }
            }

            //If no valid move found, break
            if (nextTilePos == Vector3Int.zero)
            {
                Debug.Log("No valid next tile found!");
                break;
            }

            Vector3Int offset = nextTilePos - currentTilePos;
            Vector3 targetPos = GetWorldPosition(nextTilePos);

            // Move based on the current direction
            if (currentDirection == "x")
            {
                // Move along X first
                if (offset.x != 0)
                {
                    int animIndex = offset.x > 0 ? 0 : 3;
                    SetIdleAnimation(animIndex);
                    Vector3 targetXPos = new Vector3(targetPos.x, rb.position.y, 0);
                    while (Mathf.Abs(rb.position.x - targetXPos.x) > 0.016f)
                    {
                        SetAnimation(animIndex);
                        rb.MovePosition(Vector3.MoveTowards(rb.position, targetXPos, moveSpeed * Time.deltaTime));
                        yield return null;
                    }
                    SetIdleAnimation(animIndex);
                }
                // If Y component exists, move along Y and determine y-up or y-down
                if (offset.y != 0)
                {
                    int animIndex = offset.y > 0 ? 1 : 2; // 1 for up, 2 for down
                    SetIdleAnimation(animIndex);

                    Vector3 targetYPos = new Vector3(rb.position.x, targetPos.y, 0);
                    while (Mathf.Abs(rb.position.y - targetYPos.y) > 0.016f)
                    {
                        SetAnimation(animIndex);
                        rb.MovePosition(Vector3.MoveTowards(rb.position, targetYPos, moveSpeed * Time.deltaTime));
                        yield return null;
                    }
                    // Determine if moving up or down
                    currentDirection = (offset.y > 0) ? "y-up" : "y-down";
                    SetIdleAnimation(animIndex);
                }
            }
            else if (currentDirection == "y" || currentDirection == "y-up" || currentDirection == "y-down")
            {
                // Move along Y first
                if (offset.y != 0)
                {
                    int animIndex = offset.y > 0 ? 1 : 2; // 1 for up, 2 for down
                    SetIdleAnimation(animIndex);

                    Vector3 targetYPos = new Vector3(rb.position.x, targetPos.y, 0);
                    while (Mathf.Abs(rb.position.y - targetYPos.y) > 0.016f)
                    {
                        SetAnimation(animIndex);
                        rb.MovePosition(Vector3.MoveTowards(rb.position, targetYPos, moveSpeed * Time.deltaTime));
                        yield return null;
                    }
                    SetIdleAnimation(animIndex);
                }
                // If X component exists, move along X and switch direction
                if (offset.x != 0)
                {
                    int animIndex = offset.x > 0 ? 0 : 3; // 0 for right, 3 for left
                    SetIdleAnimation(animIndex);
                    Vector3 targetXPos = new Vector3(targetPos.x, rb.position.y, 0);
                    while (Mathf.Abs(rb.position.x - targetXPos.x) > 0.016f)
                    {
                        SetAnimation(animIndex);
                        rb.MovePosition(Vector3.MoveTowards(rb.position, targetXPos, moveSpeed * Time.deltaTime));
                        yield return null;
                    }
                    currentDirection = "x"; // Reset to 'x' when moving horizontally
                    SetIdleAnimation(animIndex);
                }

                
            }

            rb.position = targetPos;
            previousTilePos = currentTilePos;
            currentTilePos = nextTilePos;

            bool triggered = false;
           

            isMoving = false;
            //yield return new WaitForSeconds(0.1f);


            // Check if this tile has a trigger event
            yield return StartCoroutine(CheckForTileTrigger(result => triggered = result));

            if (triggered)
            {
                i++;
            }
        }


        currentTilePath = boardManager.pathTiles[currentTilePos];
        Debug.Log($"here is the current tile path type {currentTilePath.tileType}");
        EventTrigger.SelectEventToTrigger(currentTilePath.tileType);
        SetIdleAnimation(0);
        isMoving = false;
        yield return new WaitForSeconds(0.1f);
    }

    // method to check for valid intersection, ie the number of offsets that has x greater than 0 is greater than 2
    private bool HasMultipleForwardMoves(Vector3Int tilePos)
    {
        int forwardMoveCount = 0;
        PathTile tile = boardManager.pathTiles[tilePos];

        foreach (Vector3Int move in tile.possibleMoves)
        {
            // get the possible move
            Vector3Int potentialNextPos = currentTilePos + move;
            Debug.Log($"here is the move {move}");
            if (tile.isIntersection && potentialNextPos!=previousTilePos ) // Count moves where x is greater or equal to zero, incase the intersection is on y
            {
               forwardMoveCount++;
            }
        }

        return forwardMoveCount >= 2; // Return true if more than 2 moves have x > 0, greater or equal to 2
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
        possibleMoves = 0;

        Debug.Log($"here is the current player direction {currentDirection}");

        int offset2x = 0;

        foreach (Vector3Int move in boardManager.pathTiles[currentTilePos].possibleMoves)
        {
            Vector3Int offset = move;
            Debug.Log($"here is the offset {offset}");
            if (currentDirection == "x")
            {
                // Right movement cases
                if (offset == new Vector3Int(3, 0, 0) || offset == new Vector3Int(4, 0, 0) ||
                    offset == new Vector3Int(2, 0, 0) || offset == new Vector3Int(1, 0, 0) ||   offset == new Vector3Int(1, 0, 0))
                {
                    if (boardManager.pathTiles[currentTilePos].possibleMoves.Contains(new Vector3Int(4, 0, 0)) && boardManager.pathTiles[currentTilePos].possibleMoves.Contains(new Vector3Int(2, 0, 0))) 
                    {
                        possibleMoves= 1;
                        onlyOneMove = new Vector3Int(2, 0, 0);
                        break;
                    }
                    possibleMoves++;
                    onlyOneMove = offset;
                    rightArrow.gameObject.SetActive(true);
                }

                //// Left movement cases
                //if (offset == new Vector3Int(-3, 0, 0) || offset == new Vector3Int(-4, 0, 0) ||   
                //    offset == new Vector3Int(-2, 0, 0) || offset == new Vector3Int(-1, 0, 0))
                //{
                //    leftArrow.gameObject.SetActive(true);
                //}

                // Up movement cases
                if (offset == new Vector3Int(1, 2, 0) || offset == new Vector3Int(2, 2, 0) || 
                    offset == new Vector3Int(0, 3, 0) || offset == new Vector3Int(2, 1, 0) || 
                    offset == new Vector3Int(1, 1, 0) || offset == new Vector3Int(0, 2, 0) 
                    //offset == new Vector3Int(-1, 2, 0) || offset == new Vector3Int(-2, 2, 0)
                    )
                {
                    possibleMoves++;
                    onlyOneMove = offset;
                    upArrow.gameObject.SetActive(true);
                }

                // Down movement cases
                if (offset == new Vector3Int(1, -2, 0) || offset == new Vector3Int(2, -2, 0) || offset == new Vector3Int(0, -2, 0) ||
                    offset == new Vector3Int(2, -1, 0) || offset == new Vector3Int(0, -3, 0) || offset == new Vector3Int(1, -1, 0) 
                     //offset == new Vector3Int(-2, -2, 0) || offset == new Vector3Int(-1, -2, 0) 
                        )

                {
                    possibleMoves++;
                    onlyOneMove = offset;
                    downArrow.gameObject.SetActive(true);
                }
            }
            else if (currentDirection == "y-up")
            {
                // Up movement cases
                if (offset == new Vector3Int(0, 3, 0) || offset == new Vector3Int(0, 4, 0) || 
                    offset == new Vector3Int(0, 2, 0) || offset == new Vector3Int(0, 1, 0))
                {
                    possibleMoves++; 
                    onlyOneMove = offset;
                    upArrow.gameObject.SetActive(true);
                }

                //// Down movement cases
                //if (offset == new Vector3Int(0, -3, 0) || offset == new Vector3Int(0, -4, 0) || 
                //    offset == new Vector3Int(0, -2, 0) || offset == new Vector3Int(0, -1, 0))
                //{
                //    downArrow.gameObject.SetActive(true);
                //}

                // Right movement cases
                if (
                    offset == new Vector3Int(2, 1, 0) || offset == new Vector3Int(1, 1, 0) || 
                    offset == new Vector3Int(2, 0, 0) || offset == new Vector3Int(1, 0, 0) ||
                    offset == new Vector3Int(3, 0, 0)
                    //offset == new Vector3Int(2, -1, 0) || offset == new Vector3Int(1, -1, 0) || offset == new Vector3Int(2, -2, 0) ||
                    )
                {
                    possibleMoves++;
                    onlyOneMove = offset;
                    rightArrow.gameObject.SetActive(true);
                }

                //// Left movement cases
                //if (offset == new Vector3Int(-2, 1, 0) || offset == new Vector3Int(-1, 1, 0) || 
                //    offset == new Vector3Int(-2, -1, 0) || offset == new Vector3Int(-1, -1, 0) ||
                //    offset == new Vector3Int(-1, -2, 0) ||
                //    offset == new Vector3Int(-2, 0, 0) || offset == new Vector3Int(-1, 0, 0))
                //{
                //    leftArrow.gameObject.SetActive(true);
                //}
            }else if(currentDirection == "y-down")
            {
                //// Up movement cases
                //if (offset == new Vector3Int(0, 3, 0) || offset == new Vector3Int(0, 4, 0) ||
                //    offset == new Vector3Int(0, 2, 0) || offset == new Vector3Int(0, 1, 0))
                //{
                //    upArrow.gameObject.SetActive(true);
                //}

                // Down movement cases
                if (offset == new Vector3Int(0, -3, 0) || offset == new Vector3Int(0, -4, 0) ||
                    offset == new Vector3Int(0, -2, 0) || offset == new Vector3Int(0, -1, 0))
                {
                    possibleMoves++;
                    onlyOneMove = offset;
                    downArrow.gameObject.SetActive(true);
                }

                // Right movement cases
                if (offset == new Vector3Int(2, -2, 0) ||
                    offset == new Vector3Int(2, -1, 0) || offset == new Vector3Int(1, -1, 0) ||
                    offset == new Vector3Int(2, 0, 0) || offset == new Vector3Int(1, 0, 0) ||
                    offset == new Vector3Int(3, 0, 0)
                    //offset == new Vector3Int(2, 1, 0) || offset == new Vector3Int(1, 1, 0) ||
                    )
                {
                    possibleMoves++;
                    onlyOneMove = offset;
                    rightArrow.gameObject.SetActive(true);
                }

                //// Left movement cases
                //if (offset == new Vector3Int(-2, 1, 0) || offset == new Vector3Int(-1, 1, 0) ||
                //    offset == new Vector3Int(-2, -1, 0) || offset == new Vector3Int(-1, -1, 0) ||
                //    offset == new Vector3Int(-1, -2, 0) ||
                //    offset == new Vector3Int(-2, 0, 0) || offset == new Vector3Int(-1, 0, 0))
                //{
                //    leftArrow.gameObject.SetActive(true);
                //}
            }
        }

        isChoosingDirection = true; // Stop movement until the player selects a direction
    }

    private void SetAnimation(int i)
    {
        switch (i)
        {
            case 0:
                animator.Play("move_right");
                break;
            case 1:
                animator.Play("move_up");
                break;
            case 2:
                animator.Play("move_down");
                break;
            case 3:
                animator.Play("move_left");
                break;
        }
    }
    private void SetIdleAnimation(int i)
    {
        switch (i)
        {
            case 0:
                animator.Play("idle_right");
                break;
            case 1:
                animator.Play("idle_up");
                break;
            case 2:
                animator.Play("idle_down");
                break;
            case 3:
                animator.Play("idle_left");
                break;
        }
    }

    private Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        return new Vector3(gridPos.x * 0.16f, gridPos.y * 0.16f, 0); // Convert to world space
    }

    private IEnumerator CheckForTileTrigger(System.Action<bool> callback)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(rb.position, 0.1f);
        bool triggered = false;
        foreach (Collider2D collider in colliders)
        {
            BridgeTrigger bridge = collider.GetComponent<BridgeTrigger>();
            if (bridge != null && !isCrossingBridge)
            {
                isMoving = false; // Pause movement
                triggered = true;

                Vector3Int bridgeOffset = new Vector3Int((int)bridge.moveOffset.x, (int)bridge.moveOffset.y, 0);
                Vector3Int targetTilePos = currentTilePos + bridgeOffset;
                Vector3 worldTargetPos = GetWorldPosition(targetTilePos);

                yield return StartCoroutine(MovePlayerAcrossBridge(targetTilePos, worldTargetPos)); // Wait for trigger effect

                isMoving = true; // Resume movement
                yield break; // Exit after handling trigger
            }
        }

        callback(triggered); // No trigger, continue movement immediately
    }


    private IEnumerator MovePlayerAcrossBridge(Vector3Int targetTilePos, Vector3 worldTargetPos)
    {
        isCrossingBridge = true; // Lock movement to prevent multiple triggers

        Vector3Int offset = targetTilePos - currentTilePos;

        // Move along Y first
        if (offset.y != 0)
        {
            int animIndex = offset.y > 0 ? 1 : 2; // 1 for up, 2 for down
            SetIdleAnimation(animIndex);
            Vector3 targetYPos = new Vector3(rb.position.x, worldTargetPos.y, 0);

            while (Mathf.Abs(rb.position.y - targetYPos.y) > 0.016f)
            {
                SetAnimation(animIndex);
                rb.MovePosition(Vector3.MoveTowards(rb.position, targetYPos, moveSpeed * Time.deltaTime));
                yield return null;
            }
            SetIdleAnimation(animIndex);    

            // Determine if moving up or down
            currentDirection =  "x";
        }

        // Move along X after Y
        if (offset.x != 0)
        {
            int animIndex = offset.x > 0 ? 0 : 3; // 0 for right, 3 for left
            SetIdleAnimation(animIndex);
            Vector3 targetXPos = new Vector3(worldTargetPos.x, rb.position.y, 0);

            while (Mathf.Abs(rb.position.x - targetXPos.x) > 0.016f)
            {
                SetAnimation(animIndex);
                rb.MovePosition(Vector3.MoveTowards(rb.position, targetXPos, moveSpeed * Time.deltaTime));
                yield return null;
            }
            SetIdleAnimation(animIndex);
        }

        // Update tile position after crossing
        currentTilePos = targetTilePos;

        isCrossingBridge = false; // Unlock movement
    }

    void OnDestroy()
    {
        if (diceManager != null)
        {
            diceManager.OnDiceRolled -= OnDiceRolled; // Unsubscribe to prevent memory leaks
        }
    }

 
}
