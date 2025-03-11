using System.Collections;
using UnityEngine;

public class PlayerBoardMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    //public Transform[] pathTiles;
    //private int currentTileIndex = 0;
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

        for (int i = 0; i < steps; i++)
        {
            Vector3 targetPos = transform.position + new Vector3(1f, 0, 0); // Move right by 1 unit

            while (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPos; // Snap to exact position
        }

        isMoving = false;

        //animator.Play("idle_" + GetDirectionFromTile());
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
}
