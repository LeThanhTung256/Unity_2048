using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private int pixelDistToDetect = 200;
    private bool isFingerDown = false;
    private Vector2Int direction;

    private void Update()
    {
        // Nếu đầu vào là touch
        if (Input.touchCount > 0 && direction == Vector2Int.zero)
        {
            if (!isFingerDown && Input.touches[0].phase == TouchPhase.Began)
            {
                startTouchPosition = Input.touches[0].position;
                isFingerDown = true;
            }

            if (isFingerDown && Input.GetMouseButtonUp(0))
            {
                Vector2 endTouchPosition = Input.touches[0].position;
                if(Vector2.Distance(startTouchPosition, Input.touches[0].position) >= pixelDistToDetect)
                {
                    Vector2 subVector = endTouchPosition - startTouchPosition;
       
                    if (Mathf.Abs(subVector.x) > Mathf.Abs(subVector.y))
                    {
                        direction = new Vector2Int((int)(subVector.x * 1/Mathf.Abs(subVector.x)), 0);
                    } else
                    {
                        direction = new Vector2Int(0, (int)(subVector.y * 1 / Mathf.Abs(subVector.y)));
                    }

                    isFingerDown = false;
                }   
            }

        }

        // Nếu không thì lấy từ bàn phím
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            direction = Vector2Int.up;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            direction = Vector2Int.left;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            direction = Vector2Int.down;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            direction = Vector2Int.right;
        }
    }

    public Vector2Int GetDirection()
    {
        return this.direction;
    }

    public void ResetDirection()
    {
        this.direction = Vector2Int.zero;
    }
}
