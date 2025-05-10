using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerFreeMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float collisionOffset = 0.001f;
    [SerializeField] private ContactFilter2D movementFilter;

    private Vector2 movementInput;
    private Rigidbody2D rb;
    private Animator animator;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private Vector2 lastMovementDirection = Vector2.right;
    private bool isWalking = false;


    // NetworkVariable to synchronize movement inputs
    private NetworkVariable<Vector2> networkMovementInput = new NetworkVariable<Vector2>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            networkMovementInput.OnValueChanged += OnMovementInputChanged;
        }

        // Listen for all players ready event
        ReadyManager.AllReadyClient += HandleAllPlayersReady;
    }

    private void OnDestroy()
    {
        // Always unsubscribe to avoid memory leaks
        ReadyManager.AllReadyClient -= HandleAllPlayersReady;
    }

    private void HandleAllPlayersReady()
    {
        if (IsOwner)
        {
            animator.Play("idle_right");
        }
    }

    private void OnMovementInputChanged(Vector2 previousValue, Vector2 newValue)
    {
        movementInput = newValue;
    }

    private void Update()
    {
        // Only for owner
        if (!IsOwner || GameStateManager.IsSceneChanging) return;

        // Managing local inputs
        HandleInput();
    }

    /* private void HandleInput()
     {
         // Getting the movement input
         Vector2 input = new Vector2(
             Input.GetAxisRaw("Horizontal"),
             Input.GetAxisRaw("Vertical")
         ).normalized;

         // Updating the network variable
         if (input != Vector2.zero)
         {
             UpdateMovementInputServerRpc(input);
         }
     }
    */
    private void HandleInput()
    {
        // Getting the movement input
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        // Updating the network variable (even if player is not moving)
        UpdateMovementInputServerRpc(input);
    }

    [ServerRpc]
    private void UpdateMovementInputServerRpc(Vector2 input)
    {
        // Synchronizing the input for all clients
        networkMovementInput.Value = input;
    }

    private void FixedUpdate()
    {
        // Only for owner
        if (!IsOwner || GameStateManager.IsSceneChanging) return;

        // Using synchronized input
        Vector2 currentInput = networkMovementInput.Value;

        if (currentInput != Vector2.zero)
        {
            // Store last direction
            lastMovementDirection = currentInput.normalized;

            bool success = TryMove(currentInput);

            if (!success)
            {
                // Try moving horizontally if the full move fails
                success = TryMove(new Vector2(currentInput.x, 0));

                // If that fails, try moving vertically
                if (!success)
                {
                    success = TryMove(new Vector2(0, currentInput.y));
                }
            }

            // If movement was successful, update animation
            if (success)
            {
                UpdateAnimation(currentInput);
                // Starting walking loop once
                if (!isWalking)
                {
                    isWalking = true;
                    AudioManager.instance?.StartWalkingLoop();
                }
            }
        }
        else
        {
            // If there's no input, play the idle animation
            SetIdleAnimation();

            // Stopping walk sound when idle
            if (isWalking)
            {
                isWalking = false;
                AudioManager.instance?.StopWalkingLoop();
            }
        }
    }

    private bool TryMove(Vector2 direction)
    {
        // Check for collisions before moving
        int count = rb.Cast(
            direction,
            movementFilter,
            castCollisions,
            moveSpeed * Time.fixedDeltaTime + collisionOffset);

        if (count == 0)
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            return true;
        }
        return false;
    }

    private void UpdateAnimation(Vector2 direction)
    {
        if (direction.x > 0)
            animator.Play("move_right");
        else if (direction.x < 0)
            animator.Play("move_left");
        else if (direction.y > 0)
            animator.Play("move_up");
        else if (direction.y < 0)
            animator.Play("move_down");
    }

    private void SetIdleAnimation()
    {
        if (ReadyManager.allReady)
        {
            animator.Play("idle_right");
            return;
        }

        if (lastMovementDirection.x > 0)
            animator.Play("idle_right");
        else if (lastMovementDirection.x < 0)
            animator.Play("idle_left");
        else if (lastMovementDirection.y > 0)
            animator.Play("idle_up");
        else
            animator.Play("idle_down");
    }
}