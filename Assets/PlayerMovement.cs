using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;

    private void Update()
    {
        if (!IsOwner) return; // V�rifie que seul le propri�taire contr�le son personnage.

        MovePlayer();
    }

    private void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, vertical,0 ).normalized * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }
}
