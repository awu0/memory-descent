using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    private LevelController levelController;
    // controlled by level controller to set when player is allowed to move
    public bool allowedToMove = false;

    private Vector3 oldPosition;    // stores old position of player while transitioning levels
    private Vector3 targetPosition; // stores target position ^
    private float transitionDuration; // copied from level controller
    private float timeElapsed;      // controls lerp alpha


    void Start()
    {
        levelController = FindObjectOfType<LevelController>();

        if (levelController == null)
        {
            Debug.LogError("LevelController not found in the scene.");
            return;
        }

        transitionDuration = levelController.levelTransitionTime;
    }

    void Update()
    {
        if (!allowedToMove)
        {
            if (timeElapsed < transitionDuration)
            {
                transform.position =
                    Vector3.Lerp(oldPosition, targetPosition, timeElapsed / transitionDuration);
                timeElapsed += Time.deltaTime;
            }
            else
            {
                transform.position = targetPosition;
            }
        }
        else
            HandleMovement();
    }

    void HandleMovement()
    {
        int moveX = 0;
        int moveZ = 0;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            moveZ = -1;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            moveZ = 1;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            moveX = -1;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            moveX = 1;

        if (moveX != 0 || moveZ != 0)
        {
            float tileSize = levelController.groundMaterial.GetComponentInChildren<Renderer>().bounds.size.x;
            Vector3 targetPosition = transform.position + new Vector3(moveX * tileSize, 0, -moveZ * tileSize); // Note the negative for Z-axis

            // Check if the target position is within the grid bounds
            if (IsWithinGridBounds(targetPosition))
            {
                transform.position = targetPosition;
                CheckTile();
            }
        }
    }

    bool IsWithinGridBounds(Vector3 position)
    {
        // Grid dimensions
        float tileSize = levelController.groundMaterial.GetComponentInChildren<Renderer>().bounds.size.x;
        int gridSize = levelController.gridSize;

        float minX = levelController.INITIAL_X_POS;
        float maxX = -levelController.INITIAL_X_POS;

        float maxZ = levelController.INITIAL_Z_POS;
        float minZ = -levelController.INITIAL_Z_POS;

        return position.x >= minX && position.x <= maxX && position.z >= minZ && position.z <= maxZ;
    }

    void CheckTile()
    {
        // Get the player's current grid coordinates
        int row, col;
        GetGridPosition(transform.position, out row, out col);

        int[,] currentMap = levelController.GetCurrentMap();
        int terrainType = currentMap[row, col];

        // if obstacle, fall down
        if (terrainType == (int)GenerateLevel.TerrainType.Obstacle)
        {
            // Start falling
            FallDown();
        }
        else if (col == levelController.gridSize - 1)
        {
            // Reached the right edge, move to the next level
            MoveToNextLevel();
        }
    }

    void GetGridPosition(Vector3 position, out int row, out int col)
    {
        float tileSize = levelController.groundMaterial.GetComponentInChildren<Renderer>().bounds.size.x;

        col = Mathf.RoundToInt((position.x - levelController.INITIAL_X_POS) / tileSize);
        row = Mathf.RoundToInt((levelController.INITIAL_Z_POS - position.z) / tileSize);
    }

    void MoveToNextLevel()
    {
        allowedToMove = false;
        levelController.AdvanceToNextLevel();
    }

    void FallDown()
    {
        allowedToMove = false;
        levelController.BackToPreviousLevel();
    }

    public void ResetPosition()
    {
        allowedToMove = false;
        // Position the player at the starting point
        float tileSize = levelController.groundMaterial.GetComponentInChildren<Renderer>().bounds.size.x;
        float startX = levelController.INITIAL_X_POS;
        float startZ = levelController.INITIAL_Z_POS - ((levelController.gridSize - 1) / 2) * tileSize;

        targetPosition = new Vector3(startX, 1f, startZ);
        oldPosition = transform.position;
        timeElapsed = 0f;
    }
}
