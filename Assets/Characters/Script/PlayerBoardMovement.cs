using System.Collections;
using UnityEngine;

public class PlayerBoardMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform[] pathTiles;
    private int currentTileIndex = 0;
    private bool isMoving = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void MovePlayer(int steps)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveAlongPath(steps));
        }
    }

    private IEnumerator MoveAlongPath(int steps)
    {
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            if (currentTileIndex + 1 >= pathTiles.Length)
                break;

            Vector3 targetPos = pathTiles[currentTileIndex + 1].position;
            Vector3 direction = (targetPos - transform.position).normalized;

            SetAnimation(direction);

            while (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos;
            currentTileIndex++;
        }

        animator.Play("Idle_" + GetDirectionFromTile());

        isMoving = false;
    }

    private void SetAnimation(Vector3 direction)
    {
        if (direction.x > 0) animator.Play("Walk_Right");
        else if (direction.x < 0) animator.Play("Walk_Left");
        else if (direction.y > 0) animator.Play("Walk_Up");
        else if (direction.y < 0) animator.Play("Walk_Down");
    }

    private string GetDirectionFromTile()
    {
        if (currentTileIndex == 0) return "Down";
        Vector3 direction = (pathTiles[currentTileIndex].position - pathTiles[currentTileIndex - 1].position).normalized;

        if (direction.x > 0) return "Right";
        if (direction.x < 0) return "Left";
        if (direction.y > 0) return "Up";
        return "Down";
    }
}
