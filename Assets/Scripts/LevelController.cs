using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public float INITIAL_X_POS = 0;
    public float INITIAL_Z_POS = 0;

    public GameObject groundMaterial;
    public GameObject routeMaterial;
    public GameObject obstacleMaterial; // Obstacle tile prefab

    public int gridSize = 8;
    public int totalLevels = 5; // Total number of levels in the game

    private int currentLevelIndex = 0;
    private int[,] currentMap;

    private GameObject currentLevelParent; // Parent GameObject for current level tiles

    // Start is called before the first frame update
    void Start()
    {
        BuildLevel(currentLevelIndex); // Updated method name
    }

    public void BuildLevel(int levelIndex) // Renamed from GenerateLevel to BuildLevel
    {
        // Clean up previous level
        if (currentLevelParent != null)
        {
            Destroy(currentLevelParent);
        }

        currentLevelParent = new GameObject("Level_" + levelIndex);

        currentMap = GenerateLevel.GenerateArray(gridSize);
        GenerateLevel.GeneratePath(currentMap);
        GenerateLevel.PadMapEdges(currentMap);

        // Optionally, add obstacles here if desired
        // GenerateLevel.AddObstacles(currentMap);

        // Place the map at the appropriate height
        float y_pos = 0; // Since we only render one level at a time, y_pos can be zero
        PlaceMap(currentMap, y_pos);
    }

    void PlaceMap(int[,] map, float y_pos)
    {
        // Where to place the new block
        float x_pos = INITIAL_X_POS;
        float z_pos = INITIAL_Z_POS;

        // Create the grid
        for (int row = 0; row < map.GetLength(0); row++)
        {
            x_pos = INITIAL_X_POS;
            for (int col = 0; col < map.GetLength(1); col++)
            {
                GameObject newTile;
                Vector3 position = new Vector3(x_pos, y_pos, z_pos);

                if (map[row, col] == (int)GenerateLevel.TerrainType.Grass)
                {
                    newTile = Instantiate(groundMaterial, position, Quaternion.identity, currentLevelParent.transform);
                }
                else if (map[row, col] == (int)GenerateLevel.TerrainType.Path)
                {
                    newTile = Instantiate(routeMaterial, position, Quaternion.identity, currentLevelParent.transform);
                }
                else if (map[row, col] == (int)GenerateLevel.TerrainType.Obstacle)
                {
                    newTile = Instantiate(obstacleMaterial, position, Quaternion.identity, currentLevelParent.transform);
                }
                else
                {
                    // Default to ground material if terrain type is unrecognized
                    newTile = Instantiate(groundMaterial, position, Quaternion.identity, currentLevelParent.transform);
                }

                // Calculate next block's location
                Renderer tileRenderer = newTile.GetComponentInChildren<Renderer>();
                Vector3 tileSize = tileRenderer.bounds.size;
                x_pos += tileSize.x;
            }
            // Move to the next row after all columns are placed
            Renderer groundRenderer = groundMaterial.GetComponentInChildren<Renderer>();
            z_pos -= groundRenderer.bounds.size.z;
        }
    }

    public int[,] GetCurrentMap()
    {
        return currentMap;
    }

    public void AdvanceToNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex >= totalLevels)
        {
            // Game Completed
            Debug.Log("You Win!");
            // Implement game completion logic here
            return;
        }

        BuildLevel(currentLevelIndex); // Updated method name

        // Move player to starting position
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.ResetPosition();
        }
    }
}
