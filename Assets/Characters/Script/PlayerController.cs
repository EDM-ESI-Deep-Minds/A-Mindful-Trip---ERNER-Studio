using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Takes and handles input and movement for a player character
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    Vector2 movementInput;
    Rigidbody2D rb;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // If movement input is not 0, try to move
        if (movementInput != Vector2.zero)
        {
            bool success = TryMove(movementInput);
            if (!success)
            {
                success = TryMove(new Vector2(movementInput.x, 0));
                
                if (!success)
                {
                    success = TryMove(new Vector2(0, movementInput.y));
                }
            }
        }
    }

    private bool TryMove(Vector2 direction)
    {

        // Checking for potential collision
        int count = rb.Cast(
            movementInput, // X and Y take values between -1 and 1, represents the direction from the body to look for collisions
            movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
            castCollisions, // List of collision types to store
            moveSpeed * Time.fixedDeltaTime + collisionOffset); // The amount to cast equal to the movement plus an offset

        // If no collisions were found, move the character
        if (count == 0)
        {
            rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
            return true;
        } else {
            return false;
        }
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }
}
