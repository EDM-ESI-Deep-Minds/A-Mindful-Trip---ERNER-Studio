using UnityEngine;

public class BugMovement : MonoBehaviour
{
    private float speed = 0.5f; // Speed of movement
    private float destroyX = -2f; // X position where the bug disappears (adjusted)

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // Store the initial position
        Debug.Log("Bug Start Position: " + startPosition); // Debugging log
    }

    void Update()
    {
        // Move the bug to the left
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Check if the bug has moved past the destroy point
        if (transform.position.x < destroyX)
        {
            Debug.Log("Bug out of bounds! Respawning...");
            Respawn();
        }
    }

    void Respawn()
    {
        // Reset to the original position where the bug was placed in the scene
        transform.position = startPosition;
        Debug.Log("Bug Respawned at: " + startPosition);
    }
}
