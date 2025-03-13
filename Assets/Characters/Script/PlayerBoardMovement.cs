using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class PlayerBoardMovement : NetworkBehaviour
{

    private DiceManager diceManager;
    [SerializeField] private float moveSpeed = 1f;

    //public Transform[] pathTiles;
    //private int currentTileIndex = 0;
    private Rigidbody2D rb;
    private bool isMoving = false;
    private Animator animator;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner) // Ensure only the owner finds and subscribes
        {
            StartCoroutine(FindDiceManager()); // Try finding DiceManager
        }
    }

    private IEnumerator FindDiceManager()
    {
        while (diceManager == null)
        {
            diceManager = FindFirstObjectByType<DiceManager>(); // Try to find it

            if (diceManager == null)
            {
                Debug.LogWarning("DiceManager not found, retrying...");
                yield return new WaitForSeconds(0.5f); // Wait and retry
            }
        }

        Debug.Log("DiceManager found!");
        diceManager.OnDiceRolled += OnDiceRolled;
    }

    private void OnDiceRolled(int dice1, int dice2)
    {
        if (!IsOwner) return; // Ensure only the owner moves

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


    void Start()
    {
        animator = GetComponent<Animator>();

        if (diceManager != null)
        {
            // Subscribe to the dice roll event
            diceManager.OnDiceRolled += OnDiceRolled;
        }
        else
        {
            Debug.LogError("DiceManager not found in the scene!");
        }

    }

    private IEnumerator MoveAlongPath(int steps)
    {
        isMoving = true;
        animator.Play("move_right");

        //for (int i = 0; i < steps; i++)
        //{
        //    if (currentTileIndex + 1 >= pathTiles.Length)
        //        break;

        //    Vector3 targetPos = pathTiles[currentTileIndex + 1].position;
        //    Vector3 direction = (targetPos - transform.position).normalized;

        //    SetAnimation(direction);

        //    while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        //    {
        //        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        //        yield return null;
        //    }

        //    transform.position = targetPos;
        //    currentTileIndex++;
        //}
        steps = steps * 3 ;

        for (int i = 0; i < steps; i++)
        {
            Vector2 targetPos = rb.position + new Vector2(.16f, 0); // Move right by 1 unit

            while (Vector2.Distance(rb.position, targetPos) > 0.016f)
            {
                rb.MovePosition(Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.deltaTime));
                yield return null;
            }

            rb.position = targetPos; // Snap to exact position
        }


        isMoving = false;
        animator.Play("idle_right");
    }

    //private void SetAnimation(Vector3 direction)
    //{
    //    if (direction.x > 0) animator.Play("move_right");
    //    else if (direction.x < 0) animator.Play("move_left");
    //    else if (direction.y > 0) animator.Play("move_up");
    //    else if (direction.y < 0) animator.Play("move_down");
    //}

    //private string GetDirectionFromTile()
    //{
    //    if (currentTileIndex == 0) return "down";
    //    Vector3 direction = (pathTiles[currentTileIndex].position - pathTiles[currentTileIndex - 1].position).normalized;

    //    if (direction.x > 0) return "right";
    //    if (direction.x < 0) return "left";
    //    if (direction.y > 0) return "up";
    //    return "right";
    //}

    void OnDestroy()
    {
        if (diceManager != null)
        {
            diceManager.OnDiceRolled -= OnDiceRolled; // Unsubscribe to prevent memory leaks
        }
    }
}
