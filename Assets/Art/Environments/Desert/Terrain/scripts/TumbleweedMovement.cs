using UnityEngine;

public class TumbleweedMovement : MonoBehaviour
{
    private float moveSpeed = 0.7f;  // Speed of movement
    private float rotationSpeed = 150f; // Rotation speed
    private float bobAmplitude = 0.02f;  // How much it moves up and down
    private float bobFrequency = 2f;  // Speed of the bobbing motion

    public float respawnX = -10f;  // X position to respawn
    public float despawnX = 12f;   // X position where it despawns

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Move horizontally
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;

        // Rotate the sprite
        transform.Rotate(Vector3.back, rotationSpeed * Time.deltaTime);

        // Bob up and down using sine wave
        float bobOffset = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(transform.position.x, startPos.y + bobOffset, transform.position.z);

        // Check if it moved out of bounds and respawn
        if (transform.position.x > despawnX)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        transform.position = new Vector3(respawnX, startPos.y, startPos.z);
    }
}
