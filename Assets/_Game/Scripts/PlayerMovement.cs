using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] LayerMask brickLayer;
    [SerializeField] Transform playerAnim;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float heightBrick;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private Vector2 startTouchPosition;
    private Vector2 swipeDirection;

    private enum Direction { None, Up, Down, Left, Right }
    private Direction currentDirection = Direction.None;

    private List<GameObject> bricks = new List<GameObject>();
    void Update()
    {
        HandleSwipeInput();

        if (isMoving)
        {
            MovePlayer();
        }
    }

    void HandleSwipeInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 endTouchPosition = Input.mousePosition;
            swipeDirection = endTouchPosition - startTouchPosition;

            if (swipeDirection.magnitude > 1) // Ngưỡng để xác định vuốt
            {
                swipeDirection.Normalize();
                DetermineDirection();
                if (currentDirection != Direction.None)
                {
                    targetPosition = SetEndPosition();
                    isMoving = true;
                }
            }
        }
    }

    void DetermineDirection()
    {
        if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
        {
            currentDirection = swipeDirection.x > 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            currentDirection = swipeDirection.y > 0 ? Direction.Up : Direction.Down;
        }
    }

    private Vector3 SetEndPosition()
    {
        Vector3 direction = Vector3.zero;
        switch (currentDirection)
        {
            case Direction.Up:
                direction = Vector3.forward;
                break;
            case Direction.Down:
                direction = Vector3.back;
                break;
            case Direction.Left:
                direction = Vector3.left;
                break;
            case Direction.Right:
                direction = Vector3.right;
                break;
        }

        Vector3 currentPosition = transform.position;

        while (CheckBrickGround(currentPosition + direction))
        {
            currentPosition += direction;
        }
        return currentPosition;
    }

    private bool CheckBrickGround(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, 5f, brickLayer))
        {
            return true;
        }
        return false;
    }

    void MovePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            isMoving = false;
            currentDirection = Direction.None;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Brick"))
        {
            AddBrick(other.gameObject);
        }
        else if (other.CompareTag("Pivote_Unbrick"))
        {
            RemoveBrick();
        } 
    }

    void AddBrick(GameObject brick)
    {
        bricks.Add(brick);
        if (bricks.Count == 1) return;
        playerAnim.position += new Vector3(0, heightBrick, 0);
        brick.transform.SetParent(transform);
        brick.GetComponent<BoxCollider>().enabled = false;
        brick.transform.localPosition = new Vector3(0, bricks.Count, 0);
        UpdateBrickPositions();
    }

    void RemoveBrick()
    {
        if (bricks.Count > 0)
        {
            playerAnim.position -= new Vector3(0, heightBrick, 0);
            if (bricks.Count > 0){
            Destroy(bricks[bricks.Count - 1]);
            }
            bricks.RemoveAt(bricks.Count - 1);  
            //UpdateBrickPositions();
        }
    }

    private void UpdateBrickPositions()
    {
        for (int i = 0; i < bricks.Count; i++)
        {
            bricks[i].transform.localPosition = new Vector3(0, heightBrick * i, 0);
        }
    }
}
