using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private LevelController levelController;
    private bool isFalling = false;

    void Start()
    {
        levelController = FindObjectOfType<LevelController>();

        if (levelController == null)
        {
            Debug.LogError("LevelController not found in the scene.");
            return;
        }

        // Position the player at the starting point
        ResetPosition();
    }

    void Update()
    {
        if (isFalling)
            return;

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
        float maxX = minX + tileSize * (gridSize);

        float maxZ = levelController.INITIAL_Z_POS;
        float minZ = maxZ - tileSize * (gridSize);

        return position.x >= minX && position.x <= maxX && position.z >= minZ && position.z <= maxZ;
    }

    void CheckTile()
    {
        // Get the player's current grid coordinates
        int row, col;
        GetGridPosition(transform.position, out row, out col);

        int[,] currentMap = levelController.GetCurrentMap();
        int terrainType = currentMap[row, col];


        //if (terrainType == (int)GenerateLevel.TerrainType.Obstacle)

        // changed to if not path, fall down
        if (terrainType != (int)GenerateLevel.TerrainType.Path)
        {
            Debug.Log("OBSTACLE HIT");
            // Start falling
            StartCoroutine(FallDown());
        }
        else if (col == levelController.gridSize - 1)
        {
            // Reached the right edge, move to the next level
            StartCoroutine(CountdownToNextLevel());
        }
    }
    IEnumerator CountdownToNextLevel()
    {
        // Wait for 1.5 seconds total (0.5s * 3 steps)
        yield return new WaitForSeconds(0.5f);

        Debug.Log("NEXT LEVEL");
        // Advance to the next level after countdown completes
        levelController.AdvanceToNextLevel();
    }
    void GetGridPosition(Vector3 position, out int row, out int col)
    {
        float tileSize = levelController.groundMaterial.GetComponentInChildren<Renderer>().bounds.size.x;

        col = Mathf.RoundToInt((position.x - levelController.INITIAL_X_POS) / tileSize);
        row = Mathf.RoundToInt((levelController.INITIAL_Z_POS - position.z) / tileSize);
    }

    IEnumerator FallDown()
    {
        isFalling = true;
        levelController.BackToPreviousLevel();

        // Since we only have one level at a time, falling means game over
        //Debug.Log("Game Over");
        // Implement game over logic here


        // Optionally reset the game or display a game over screen
        yield break;
    }

    public void ResetPosition()
    {
        // Position the player at the starting point
        float tileSize = levelController.groundMaterial.GetComponentInChildren<Renderer>().bounds.size.x;
        float startX = levelController.INITIAL_X_POS;
        float startZ = levelController.INITIAL_Z_POS - ((levelController.gridSize - 1) / 2) * tileSize;

        transform.position = new Vector3(startX, 1f, startZ);
        isFalling = false;
    }
}
