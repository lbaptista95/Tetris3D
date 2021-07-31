using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class sets the grid size and adjust the camera position and FoV according to the grid scale.
/// Besides that, it checks if the blocks are inside its limits and deletes horizontal layers if they are full.
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager instance = null;

    [Header("Grid Settings")]
    [SerializeField] private GameObject plane;
    [SerializeField] private int sizeX;
    [SerializeField] private int sizeY;
    private Transform[,] grid;

    public int SizeX { get { return sizeX; } }
    public int SizeY { get { return sizeY; } }    

    private MeshRenderer mRenderer;

    [Header("Camera Settings")]
    [SerializeField] float cameraDistance;

    #region MONOBEHAVIOUR_METHODS
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Init();
    }
    private void OnEnable()
    {
        PieceBehaviour.OnMove += UpdateGrid;
        PieceBehaviour.OnReachBottom += DeleteRow;
    }
    private void OnDisable()
    {
        PieceBehaviour.OnMove -= UpdateGrid;
        PieceBehaviour.OnReachBottom -= DeleteRow;
    }

    #endregion

    #region SCENE_SETUP

    public void Init()
    {
        Vector2 size = GameManager.instance.GetGridSize();
        sizeX = (int)size.x;
        sizeY = (int)size.y;

        grid = new Transform[sizeX, sizeY];

        SetGrid();
        SetCamera();
    }

    private void SetGrid()
    {
        Vector3 newScale = new Vector3((float)sizeX / 10, 1, (float)sizeY / 10);
        plane.transform.localScale = newScale;

        plane.transform.localPosition = new Vector3((float)-sizeX / 2, (float)sizeY / 2, transform.position.z);

        if (mRenderer == null)
            mRenderer = plane.GetComponentInChildren<MeshRenderer>();

        mRenderer.sharedMaterial.mainTextureScale = new Vector2(sizeX, sizeY);
    }

    private void SetCamera()
    {
        Vector3 objectSizes = mRenderer.bounds.max - mRenderer.bounds.min;
        float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
        float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * Camera.main.fieldOfView);
        float distance = cameraDistance * objectSize / cameraView;
        distance += 0.5f * objectSize;
        Camera.main.transform.position = mRenderer.bounds.center - distance * Camera.main.transform.forward;
    }
    #endregion

    #region GAMEPLAY
    //Checks if the grid contains a position.
    public bool IsInsideGrid(Vector3 position)
    {
        Vector3 roundPos = RoundVector(position);

        return ((roundPos.x >= 0 && roundPos.x < sizeX) &&
            (roundPos.y >= 0 && roundPos.y < sizeY));
    }

    //Returns a valid grid position.
    public Transform GetSquareFromPosition(Vector3 pos)
    {
        Vector3 position = RoundVector(pos);

        if (position.y >= sizeY - 1)
        {
            return null;
        }
        else
        {
            return (grid[(int)position.x, (int)position.y]);
        }
    }

    //Delete all blocks from a row if it's full.
    private void DeleteRow()
    {
        for (int y = 0; y < sizeY; y++)
        {
            if (IsRowFull(y))
            {
                for (int x = 0; x < sizeX; x++)
                {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }

                GameManager.instance.IncreaseScore();

                MoveAllBlocksDown(y);
            }
        }
    }

    //Checks if row is full of blocks
    private bool IsRowFull(int y)
    {
        for (int x = 0; x < sizeX; x++)
        {
            if (grid[x, y] == null)
            {
                return false;
            }

        }
        return true;
    }

    //Move blocks down after deleting a row
    private void MoveAllBlocksDown(int deletedRow)
    {
        for (int y = deletedRow; y < sizeY; y++)
        {
            MoveOneRowDown(y);
        }
    }

    //Move one block down in the grid array
    private void MoveOneRowDown(int y)
    {
        for (int x = 0; x < sizeX; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += Vector3.down;
            }
        }
    }

    //Updates the position of a piece inside the grid array.
    private void UpdateGrid(PieceBehaviour piece)
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == piece.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform block in piece.transform)
        {
            Vector3 position = RoundVector(block.position);
            if (position.y < sizeY)
            {
                grid[(int)position.x, (int)position.y] = block;
            }
        }

    }
    #endregion

    #region UTILS

    private Vector3 RoundVector(Vector3 rawVector)
    {
        return new Vector3(Mathf.Round(rawVector.x), 
                           Mathf.Round(rawVector.y), 
                           Mathf.Round(rawVector.z));
    }


    #endregion
}
