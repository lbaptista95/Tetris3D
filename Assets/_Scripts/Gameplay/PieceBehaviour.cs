using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to control a single piece, which is a group of blocks.
/// </summary>
public class PieceBehaviour : MonoBehaviour
{
    public delegate void PieceMovementEvents(PieceBehaviour piece);
    public static event PieceMovementEvents OnMove;

    public delegate void PieceLimitEvents();
    public static event PieceLimitEvents OnReachBottom;

    private float _timeToFall;
    private float _previousTime;

    [SerializeField] private bool isFalling;

    //Starts the falling routine
    public void Fall(int timeToFall)
    {
        if (!CanMove())
        {
            GameManager.instance.GameOver();
            return;
        }
        _timeToFall = timeToFall;
        isFalling = true;
        StopCoroutine(Falling());
        StartCoroutine(Falling());
    }

    //Changes the piece position and checks if the player pressed an arrow key.
    private IEnumerator Falling()
    {
        yield return new WaitForSeconds(_timeToFall);
        while (isFalling)
        {
            if (Time.time - _previousTime > _timeToFall)
            {
                transform.position += Vector3.down;

                if (!CanMove())
                {
                    transform.position += Vector3.up;
                    isFalling = false;
                    OnReachBottom.Invoke();
                }
                else
                {
                    OnMove.Invoke(this);
                }

                _previousTime = Time.time;
            }

            if (isFalling)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    RotationController(new Vector3(0, 0, 90));

                if (Input.GetKeyDown(KeyCode.DownArrow))
                    MovementController(Vector3.down);

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    MovementController(Vector3.left);

                if (Input.GetKeyDown(KeyCode.RightArrow))
                    MovementController(Vector3.right);
            }

            yield return null;
        }
    }

    //Moves the piece according to an direction - which is determined by the arrow keys.
    private void MovementController(Vector3 direction)
    {
        transform.position += direction;
        if (!CanMove())
        {
            transform.position -= direction;
        }
        else
        {
            OnMove.Invoke(this);
        }
    }

    //Rotates the piece according to an direction - which is determined by the up arrow key.
    private void RotationController(Vector3 rotation)
    {
        transform.Rotate(rotation, Space.World);

        if (!CanMove())
        {
            transform.Rotate(-rotation, Space.World);
        }
        else
        {
            OnMove.Invoke(this);
        }
    }

    //Check if the piece can move (if it's inside the grid and if there's no piece below it).
    private bool CanMove()
    {
        foreach (Transform block in transform)
        {
            if (!GridManager.instance.IsInsideGrid(block.position))
            {
                return false;
            }
        }

        foreach (Transform block in transform)
        {
            Transform gridSquare = GridManager.instance.GetSquareFromPosition(block.position);

            if (gridSquare != null)
            {
                print(transform.position);
                print(gridSquare.position);
            }
            if (gridSquare != null)
            {
                if (gridSquare.parent != transform)
                {
                    print(gridSquare.parent.gameObject.name + " is different from " + gameObject.name);
                    return false;
                }                   
            }
        }
        return true;
    }
}
