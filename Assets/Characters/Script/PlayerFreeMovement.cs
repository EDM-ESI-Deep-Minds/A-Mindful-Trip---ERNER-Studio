using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFreeMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.001f;
    public ContactFilter2D movementFilter;

    private Vector2 movementInput;
    private Rigidbody2D rb;
    private Animator animator;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private Vector2 lastMovementDirection = Vector2.right; // Default facing right

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
        {
            // Store last direction
            lastMovementDirection = movementInput.normalized;

            bool success = TryMove(movementInput);

            if (!success)
            {
                // Try moving horizontally if the full move fails
                success = TryMove(new Vector2(movementInput.x, 0));

                // If that fails, try moving vertically
                if (!success)
                {
                    success = TryMove(new Vector2(0, movementInput.y));
                }
            }

            // If movement was successful, update last movement direction
            if (success)
            {
                lastMovementDirection = movementInput.normalized;
                UpdateAnimation(movementInput);
            }
        }
        else
        {
            // If there's no input, play the idle animation
            SetIdleAnimation();
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

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
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
