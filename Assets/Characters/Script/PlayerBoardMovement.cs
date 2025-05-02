using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Linq.Expressions;

public class PlayerBoardMovement : NetworkBehaviour
{
    private DiceManager diceManager;
    private BoardManager boardManager;
    [SerializeField] private float moveSpeed = .3f;
    private Rigidbody2D rb;
    private bool isMoving = false;
    private Animator animator;
    private Vector3Int currentTilePos;
    private Vector3Int previousTilePos;
    public Tilemap boardTilemap;
    private Vector3Int currentGridPosition;
    //private Vector3 lastDirection = Vector3.right;
    public string currentDirection = "";
    public Button rightArrow;
    public Button leftArrow;
    public Button upArrow;
    public Button downArrow;
    public Button backWardButton;
    private bool isChoosingDirection = false;
    private string selectedDirection = "";
    private bool isCrossingBridge = false;
    private int possibleMoves;
    private Vector3Int onlyOneMove;
    private PathTile currentTilePath;
    private float gridSize = 16f;
    private bool isWalkSoundPlaying = false;
    public float playerProgress = 0f; // Player progress on the board
    public Vector3 endPosition;
    // lets add the player path, which is an array of the player movements on the road
    private List<Vector3Int> playerPath = new List<Vector3Int>();
    private ProgressBarController progressBarController;
    private SpriteRenderer spriteRenderer;
    private int playerSprite = -1;
    private bool backWardBridge = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        DontDestroyOnLoad(gameObject);
        TryFindArrowButtons();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsOwner)
        {
            TryFindArrowButtons();
            StartCoroutine(SetupAfterSceneLoad());
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            TryFindArrowButtons();
            StartCoroutine(SetupAfterSceneLoad());
        }
    }

    private IEnumerator SetupAfterSceneLoad()
    {
        yield return StartCoroutine(FindBoardManager());
        yield return StartCoroutine(FindDiceManager());

        // Now it’s safe to access buttons, managers, etc.
        Debug.Log("All setup coroutines finished. Safe to proceed.");
    }

    private IEnumerator FindBoardManager(){
        boardManager = null;
        rightArrow = null;
        leftArrow = null;
        upArrow = null;
        downArrow = null;
        progressBarController = null;

        while (boardManager == null)
        {
            boardManager = FindFirstObjectByType<BoardManager>();
            if (boardManager == null)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        if (progressBarController == null)
        {
            GameObject progressBarGO = GameObject.Find("ProgressBar");
            if (progressBarGO != null)
            {
                progressBarController = progressBarGO.GetComponent<ProgressBarController>();
                if (progressBarController == null)
                {
                    Debug.LogWarning("ProgressBarController component not found on ProgressBar GameObject.");
                }
            }
            else
            {
                Debug.LogWarning("ProgressBar GameObject not found.");
            }
        }


        // save the first end position
        endPosition = BoardManager.storedEndPositions[0];
        Vector3 worldPosition = transform.position;
        Debug.Log($"World Position: {worldPosition}");
        Vector3Int gridPosition= boardManager.boardTilemap.WorldToCell(worldPosition);
        currentTilePos = gridPosition;
        Debug.Log($"Current Tile Position: {currentTilePos}");
        

        while (rightArrow == null || leftArrow == null || downArrow == null || upArrow == null)
        {
            GameObject rightObj = GameObject.Find("RightArrow");
            GameObject leftObj = GameObject.Find("LeftArrow");
            GameObject upObj = GameObject.Find("UpArrow");
            GameObject downObj = GameObject.Find("DownArrow");

            if (rightObj != null && rightArrow == null)
                rightArrow = rightObj.GetComponent<Button>();

            if (leftObj != null && leftArrow == null)
                leftArrow = leftObj.GetComponent<Button>();

            if (upObj != null && upArrow == null)
                upArrow = upObj.GetComponent<Button>();

            if (downObj != null && downArrow == null)
                downArrow = downObj.GetComponent<Button>();

            Debug.Log($"RightArrow: {rightArrow}, LeftArrow: {leftArrow}, UpArrow: {upArrow}, DownArrow: {downArrow}");

            yield return new WaitForSeconds(0.5f);

            //backWardButton = GameObject.Find("backWardButton").GetComponent<Button>();
            Debug.Log($"RightArrow: {rightArrow}, LeftArrow: {leftArrow}, UpArrow: {upArrow}, DownArrow: {downArrow}");
        }

        // backWardButton = GameObject.Find("backWardButton").GetComponent<Button>();

        if (rightArrow == null || leftArrow == null || downArrow == null || upArrow == null)
        {
            Debug.LogError("One or more arrow buttons not found in the scene.");
        }
        else
        {
            // Assign button listeners dynamically
            rightArrow.onClick.AddListener(() => SetChosenDirection("right"));
            leftArrow.onClick.AddListener(() => SetChosenDirection("left"));
            upArrow.onClick.AddListener(() => SetChosenDirection("up"));
            downArrow.onClick.AddListener(() => SetChosenDirection("down"));
        }

        StartCoroutine(DetermineDefaultDirection());

        //   backWardButton.onClick.AddListener(() => StartCoroutine(MoveBackward(5)));

        HideArrows();

        // Detect the current scene and set the grid size
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (currentScene == "City") // Replace with your actual city scene name
        {
            gridSize = 0.32f;
        }
        else
        {
            gridSize = 0.16f;
        }

        Debug.Log("Grid Size Set To: " + gridSize);

        // display the player sprite name
        Debug.Log("Player Sprite Name: " + spriteRenderer.sprite.name);

        string spriteName = spriteRenderer.sprite.name;

        int playerIndex = 0;

        if (spriteName.Contains("Roxy"))
        {
            playerSprite = 0;
        }
        else if (spriteName.Contains("Ren"))
        {
            playerSprite = 1;
        }
        else if (spriteName.Contains("Tarus"))
        {
            playerSprite = 2;
        }
        else if (spriteName.Contains("mar"))
        {
            playerSprite = 3;
        }

        playerProgress = Mathf.Abs(rb.position.x - endPosition.x);
        // intialize the progress
        progressBarController.InitializePlayerProgressServerRpc(playerSprite, playerProgress);
        progressBarController.RequestUpdateProgressBarServerRpc(playerSprite, playerProgress);
        QuestionManager.Instance.setSpriteIndex(playerSprite);
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

        if (!TryFindArrowButtons())
        {
            Debug.LogWarning("Arrow buttons not yet found in HideArrows().");
            return;
        }

        if (rightArrow != null)
        {
            rightArrow.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("RightArrow is not assigned.");
        }

        if (leftArrow != null)
        {
            leftArrow.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("LeftArrow is not assigned.");
        }

        if (upArrow != null)
        {
            upArrow.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("UpArrow is not assigned.");
        }

        if (downArrow != null)
        {
            downArrow.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("DownArrow is not assigned.");
        }
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

        private IEnumerator DetermineDefaultDirection()
    {
        Vector3Int[] validMoves = null;

            while (validMoves == null)
            {
                try
                {
                    Vector3 worldPosition = transform.position;
                    Vector3Int currentTilePos = boardManager.boardTilemap.WorldToCell(worldPosition);

                    // Debugging the current tile position
                    Debug.Log($"Current Tile Position: {currentTilePos}");
                    currentTilePath = boardManager.pathTiles[currentTilePos];
                    validMoves = boardManager.pathTiles[currentTilePos].possibleMoves;
                }
                catch (KeyNotFoundException e)
                {
                    Debug.LogWarning("Tile not found. Retrying...");
                    Debug.LogWarning("Tile not found. Retrying..." + e.Message);
                    validMoves = null; // Force retry
                }

                // Yield to avoid locking the main thread and allow retries in subsequent frames
                yield return null;
            }
        // debugging the current tile  position
        Debug.Log($"Current Tile Position: {currentTilePos}");

        if (SceneManager.GetActiveScene().name != "CountrySide")

            if (SceneManager.GetActiveScene().name != "CountrySide")
            {
                SetIdleAnimation(0);
                currentDirection = "x";
                yield return null;
            }

        if (validMoves.Length == 0)
        {
            currentDirection = "x";
            SetIdleAnimation(0);
            yield return null;
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

    private void StartWalkSFX()
    {
        if (!isWalkSoundPlaying)
        {
            isWalkSoundPlaying = true;
            AudioManager.instance?.StartWalkingLoop();
        }
    }

    private void StopWalkSFX()
    {
        if (isWalkSoundPlaying)
        {
            isWalkSoundPlaying = false;
            AudioManager.instance?.StopWalkingLoop();
        }
    }


    private IEnumerator MoveAlongPath(int steps)
    {
        if (!RolesManager.IsMyTurn) yield break;

        backWardBridge = false;

        isMoving = true;

        StartWalkSFX();

        //Debug.Log($"here is the current tile position {currentTilePos}");

        //Debug.Log($"here is the previous tile position {previousTilePos}");
        while (!TryFindArrowButtons())
        {
            Debug.Log("trying to find the buttons");
        }


         //steps = 20;
        for (int i = 0; i < steps; i++)
        {
            while (!boardManager.pathTiles.ContainsKey(currentTilePos))
            {
                StartCoroutine(FindBoardManager());
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
            currentTilePath = boardManager.pathTiles[currentTilePos];
            if (HasMultipleForwardMoves(currentTilePos) && !currentTilePath.falseIntersection )
            {
                StartCoroutine(ShowArrowsBasedOnMoves());
                if (possibleMoves == 1)
                {
                    nextTilePos = currentTilePos + onlyOneMove;
                    HideArrows();
                }
                else
                {
                    selectedDirection = "";
                    isChoosingDirection = true;
                    StopWalkSFX();
                    yield return new WaitUntil(() => !isChoosingDirection);
                    StartWalkSFX();

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
                                     moveOffset == new Vector3Int(0, -2, 0) ||
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
                                    moveOffset == new Vector3Int(1, 1, 0) || moveOffset == new Vector3Int(1, 2, 0) ||
                                    moveOffset == new Vector3Int(2, -1, 0) || moveOffset == new Vector3Int(1, -1, 0) ||
                                    moveOffset  == new Vector3Int(2, 2, 0) || 
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
                                    (moveOffset == new Vector3Int(2, 1, 0) || moveOffset == new Vector3Int(2, -2, 0) || moveOffset == new Vector3Int(1, -2, 0) ||
                                    moveOffset == new Vector3Int(1, 1, 0) || moveOffset == new Vector3Int(3, 0, 0) ||
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
                    Debug.Log("possible move : " + move);
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
                            if ((nextTilePos == Vector3Int.zero || potentialNextPos.y <= nextTilePos.y) && move.x >= 0)
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

            //Debug.Log($"Current: {currentTilePos}, Next: {nextTilePos}, Offset: {offset}");

            //Debug.Log($"Target World Pos: {targetPos}");

            // Move based on the current direction
            if (currentDirection == "x")
            {
                // Move along X first
                if (offset.x != 0)
                {
                    int animIndex = offset.x > 0 ? 0 : 3;
                    SetIdleAnimation(animIndex);
                    Vector3 targetXPos = new Vector3(targetPos.x, rb.position.y, 0);
                    while (Mathf.Abs(rb.position.x - targetXPos.x) > (gridSize * 0.001f))
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
                    while (Mathf.Abs(rb.position.y - targetYPos.y) > (gridSize * 0.001f))
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
                    while (Mathf.Abs(rb.position.y - targetYPos.y) > (gridSize * 0.001f))
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
                    while (Mathf.Abs(rb.position.x - targetXPos.x) > (gridSize * 0.001f))
                    {
                        SetAnimation(animIndex);
                        rb.MovePosition(Vector3.MoveTowards(rb.position, targetXPos, moveSpeed * Time.deltaTime));
                        yield return null;
                    }
                    currentDirection = "x"; // Reset to 'x' when moving horizontally
                    SetIdleAnimation(animIndex);
                }


            }
            Debug.Log("current player direction: " + currentDirection);
            rb.position = targetPos;
            previousTilePos = currentTilePos;
            playerPath.Add(currentTilePos);
            currentTilePos = nextTilePos;

            bool triggered = false;

            isMoving = false;
            currentTilePath = boardManager.pathTiles[currentTilePos];
            Debug.Log($"Current Tile Path Type: {currentTilePath.tileType}");
            if (currentTilePath.tileType == "curse_0")
            {
                currentTilePath.tileType = "Curse";
                currentTilePath.falseIntersection = true;
            }

            if ((currentTilePath.tileType == "End"))
            {
                EventTrigger.SelectEventToTrigger(currentTilePath.tileType);
                steps = 0;
            }

            if (currentTilePath.changeDirection && currentDirection == "y-up")
            {
                currentDirection = "y-down";
            }

            // update the player progress, which represent the distance from the player current position to the end position on the x axis
            playerProgress = Mathf.Abs(rb.position.x - endPosition.x);

            progressBarController.RequestUpdateProgressBarServerRpc(playerSprite,playerProgress);

            //debug
            //Debug.Log($"end Tile Position: {endPosition}");
            //Debug.Log($"Current Player Position: {rb.position.x}");

            //Debug.Log($"Player Progress: {playerProgress}");

            //yield return new WaitForSeconds(0.1f);

            // Check if this tile has a trigger event
            yield return StartCoroutine(CheckForTileTrigger(result => triggered = result));

            if (triggered)
            {
                i++;
            }
        }

        playerPath.Add(currentTilePos);
        currentTilePath = boardManager.pathTiles[currentTilePos];
        //Debug.Log($"here is the current tile path type {currentTilePath.tileType}");
        EventTrigger.SelectEventToTrigger(currentTilePath.tileType);
        if (currentTilePath.changeDirection && currentDirection == "y-up")
        {
            currentDirection = "y-down";
        }
        SetIdleAnimation(0);
        isMoving = false;
        StopWalkSFX();
        yield return new WaitForSeconds(0.1f);
    }

    //  now lets add a function that takes the number of steps that moves the player backward
    public IEnumerator MoveBackward(int steps)
    {
        if (!RolesManager.IsMyTurn) yield break;
        backWardBridge = true;

        if (playerPath.Count < 1) yield break;

        StartWalkSFX();

        for (int i = 0; i < steps; i++)
        {
            if (playerPath.Count < 2) break; 

            // Get the last position and remove it from the path
            Vector3Int lastTilePos = playerPath[playerPath.Count - 2]; 
            playerPath.RemoveAt(playerPath.Count - 1);

            Vector3 targetPos = GetWorldPosition(lastTilePos);
            Vector3Int offset = currentTilePos - lastTilePos; // Reverse offset
            // log the offset 
            // Move based on the current direction
            if (currentDirection == "x")
            {
                // Move along X first
                if (offset.x != 0)
                {
                    int animIndex = offset.x > 0 ? 3 :  0;
                    SetIdleAnimation(animIndex);
                    yield return new WaitForSeconds(0.05f);
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
                    int animIndex = offset.y > 0 ? 2 : 1 ; // 1 for up, 2 for down
                    SetIdleAnimation(animIndex);
                    yield return new WaitForSeconds(0.05f);
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
                    int animIndex = offset.y > 0 ? 2 : 1; // 1 for up, 2 for down
                    SetIdleAnimation(animIndex);
                    yield return new WaitForSeconds(0.05f);
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
                    int animIndex = offset.x > 0 ? 3 : 0; // 0 for right, 3 for left
                    SetIdleAnimation(animIndex);
                    yield return new WaitForSeconds(0.05f);
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

            playerProgress = Mathf.Abs(rb.position.x - endPosition.x);

            progressBarController.RequestUpdateProgressBarServerRpc(playerSprite, playerProgress);

            rb.position = targetPos;
            previousTilePos = lastTilePos;
            currentTilePos = lastTilePos;
            bool triggered = false;
            yield return StartCoroutine(CheckForTileTrigger(result => triggered = result));

            if (triggered)
            {
                i++;
            }
        }
        isMoving = false;
        StopWalkSFX();
        UpdateFace();
        BonusCurseUIManager UIManager = Object.FindFirstObjectByType<BonusCurseUIManager>();
        UIManager.StartCoroutine(DelaySwitchTurn());
    }

    public static IEnumerator DelaySwitchTurn()
    {
        yield return new WaitForSeconds(1f);
        RolesManager.SwitchRole();
    }

    // this function to update the sprite of the player depending on the current direction
    private void UpdateFace()
    {
        if (currentDirection == "x")
        {
            SetIdleAnimation(0); // Left or Right
        }
        else if (currentDirection == "y-up")
        {
            SetIdleAnimation(1); // Up
        }
        else if (currentDirection == "y-down")
        {
            SetIdleAnimation(2); // Down
        }
    }

    // method to check for valid intersection, ie the number of offsets that has x greater than 0 is greater than 2
    private bool HasMultipleForwardMoves(Vector3Int tilePos)
    {
        int forwardMoveCount = 0;
        PathTile tile = boardManager.pathTiles[tilePos];

        foreach (Vector3Int move in tile.possibleMoves)
        {
            Vector3Int potentialNextPos = tilePos + move;

            // Skip going back to the previous tile
            if (!tile.isIntersection || potentialNextPos == previousTilePos)
                continue;

            // Check if the potential next tile exists and is valid
            if (!boardManager.pathTiles.ContainsKey(potentialNextPos))
                continue;

            PathTile nextTile = boardManager.pathTiles[potentialNextPos];

            if(tile.cityFalseIntersection && currentDirection == "y-down")
            {
                currentDirection = "x";
                return false;
            }

            if (tile.falseIntersection)
            {
                return false;
            }
            forwardMoveCount++;
            
        }

        return forwardMoveCount >= 2; // Return true if more than 2 moves have x > 0, greater or equal to 2
    }


    private void SetChosenDirection(string direction)
    {
        selectedDirection = direction;
        //Debug.Log($"here is the selected direction by the user{selectedDirection}");
        isChoosingDirection = false;
        HideArrows();
    }

    IEnumerator ShowArrowsBasedOnMoves()
    {
        HideArrows();
        possibleMoves = 0;

        //Debug.Log($"here is the current player direction {currentDirection}");

        int offset2x = 0;

        foreach (Vector3Int move in boardManager.pathTiles[currentTilePos].possibleMoves)
        {
            Vector3Int offset = move;
            //Debug.Log($"here is the offset {offset}");
            PathTile currentTile = boardManager.pathTiles[currentTilePos];

            if (currentDirection == "x")
            {
                // Right movement cases

                if (offset == new Vector3Int(3, 0, 0) || offset == new Vector3Int(4, 0, 0) ||
                offset == new Vector3Int(2, 0, 0) || offset == new Vector3Int(1, 0, 0) || offset == new Vector3Int(1, 0, 0))
                {
                    if (boardManager.pathTiles[currentTilePos].possibleMoves.Contains(new Vector3Int(4, 0, 0)) && boardManager.pathTiles[currentTilePos].possibleMoves.Contains(new Vector3Int(2, 0, 0)))
                    {
                        possibleMoves = 1;
                        onlyOneMove = new Vector3Int(2, 0, 0);
                        break;
                    }
                    possibleMoves++;
                    onlyOneMove = offset;
                    while (!TryFindArrowButtons())
                    {
                        yield return null; // wait one frame
                    }
                    rightArrow.gameObject.SetActive(true);

                }


                //// Left movement cases
                //if (offset == new Vector3Int(-3, 0, 0) || offset == new Vector3Int(-4, 0, 0) ||   
                //    offset == new Vector3Int(-2, 0, 0) || offset == new Vector3Int(-1, 0, 0))
                //{
                //    leftArrow.gameObject.SetActive(true);
                //}

                // Up movement cases
                if (!currentTile.noUp)
                {
                    if (offset == new Vector3Int(1, 2, 0) || offset == new Vector3Int(2, 2, 0) ||
                    offset == new Vector3Int(0, 3, 0) || offset == new Vector3Int(2, 1, 0) ||
                    offset == new Vector3Int(1, 1, 0) || offset == new Vector3Int(0, 2, 0)
                    //offset == new Vector3Int(-1, 2, 0) || offset == new Vector3Int(-2, 2, 0)
                    )
                    {
                        possibleMoves++;
                        onlyOneMove = offset;
                        while (!TryFindArrowButtons())
                        {
                            yield return null; // wait one frame
                        }
                        upArrow.gameObject.SetActive(true);
                    }
                }

                // Down movement cases
                if (offset == new Vector3Int(1, -2, 0) || offset == new Vector3Int(2, -2, 0) || offset == new Vector3Int(0, -2, 0) ||
                    offset == new Vector3Int(2, -1, 0) || offset == new Vector3Int(0, -3, 0) || offset == new Vector3Int(1, -1, 0)
                        //offset == new Vector3Int(-2, -2, 0) || offset == new Vector3Int(-1, -2, 0) 
                        )

                {
                    possibleMoves++;
                    onlyOneMove = offset;
                    while (!TryFindArrowButtons())
                    {
                        yield return null; // wait one frame
                    }
                    downArrow.gameObject.SetActive(true);
                }
            }
            else if (currentDirection == "y-up")
            {
                // Up movement cases
                if (!currentTile.noUp)
                {
                    if (offset == new Vector3Int(0, 3, 0) || offset == new Vector3Int(0, 4, 0) ||
                    offset == new Vector3Int(0, 2, 0) || offset == new Vector3Int(0, 1, 0))
                    {
                        possibleMoves++;
                        onlyOneMove = offset;
                        while (!TryFindArrowButtons())
                        {
                            yield return null; // wait one frame
                        }
                        upArrow.gameObject.SetActive(true);
                    }
                }
            

                //// Down movement cases
                //if (offset == new Vector3Int(0, -3, 0) || offset == new Vector3Int(0, -4, 0) || 
                //    offset == new Vector3Int(0, -2, 0) || offset == new Vector3Int(0, -1, 0))
                //{
                //    downArrow.gameObject.SetActive(true);
                //}

                // Right movement cases
                if (
                    offset == new Vector3Int(2, 1, 0) || offset == new Vector3Int(1, 1, 0) || offset == new Vector3Int(1, 2, 0) ||
                    offset == new Vector3Int(2, 0, 0) || offset == new Vector3Int(1, 0, 0) || offset == new Vector3Int(2, 2, 0) ||
                    offset == new Vector3Int(3, 0, 0)
                    //offset == new Vector3Int(2, -1, 0) || offset == new Vector3Int(1, -1, 0) || offset == new Vector3Int(2, -2, 0) ||
                    )
                {
                    possibleMoves++;
                    onlyOneMove = offset;
                    while (!TryFindArrowButtons())
                    {
                        yield return null; // wait one frame
                    }
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
            else if (currentDirection == "y-down")
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
                    while (!TryFindArrowButtons())
                    {
                        yield return null; // wait one frame
                    }
                    downArrow.gameObject.SetActive(true);
                }

                // Right movement cases
                if (offset == new Vector3Int(2, -2, 0) || offset == new Vector3Int(1, -2, 0) ||
                    offset == new Vector3Int(2, -1, 0) || offset == new Vector3Int(1, -1, 0) ||
                    offset == new Vector3Int(2, 0, 0) || offset == new Vector3Int(1, 0, 0) ||
                    offset == new Vector3Int(3, 0, 0)
                    //offset == new Vector3Int(2, 1, 0) || offset == new Vector3Int(1, 1, 0) ||
                    )
                {
                    possibleMoves++;
                    onlyOneMove = offset;
                    while (!TryFindArrowButtons())
                    {
                        yield return null; // wait one frame
                    }
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
        return new Vector3(gridPos.x * gridSize, gridPos.y * gridSize, 0);
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

                Debug.Log($"Bridge Offset: {bridgeOffset}");

                if (bridgeOffset.x < 0 && !backWardBridge)
                {
                    isMoving = true;
                    yield break;
                }

                if (bridgeOffset.x > 0 && backWardBridge)
                {
                    isMoving = true;
                    yield break;
                }

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

        StartWalkSFX();

        if (offset.x > 0)
        {
            // Move along Y first
            currentDirection = "y-down";
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
                currentDirection = "x";
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
        }
        else
        {
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

            // Move along Y first
            if (offset.y != 0)
            {
                currentDirection = "y-up";
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
                currentDirection = "x";
            }
        }

        // Update tile position after crossing
        currentTilePos = targetTilePos;
        StopWalkSFX();
        isCrossingBridge = false; // Unlock movement
    }

    void OnDestroy()
    {
        if (diceManager != null)
        {
            diceManager.OnDiceRolled -= OnDiceRolled; // Unsubscribe from dice event
        }

        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from scene change
    }


    private bool TryFindArrowButtons()
    {
        if (rightArrow == null)
            rightArrow = GameObject.Find("RightArrow")?.GetComponent<Button>();
        if (leftArrow == null)
            leftArrow = GameObject.Find("LeftArrow")?.GetComponent<Button>();
        if (upArrow == null)
            upArrow = GameObject.Find("UpArrow")?.GetComponent<Button>();
        if (downArrow == null)
            downArrow = GameObject.Find("DownArrow")?.GetComponent<Button>();

        return rightArrow != null && leftArrow != null && upArrow != null && downArrow != null;
    }
}
