using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public float INITIAL_X_POS = 0;
    public float INITIAL_Z_POS = 0;
    

    public float hideTilesTime = 3f; // Time in seconds before tiles are hidden

    public GameObject groundMaterial;
    public GameObject routeMaterial;
    public GameObject obstacleMaterial; // Obstacle tile prefab

    public int gridSize = 8;
    public int totalLevels = 5; // Total number of levels in the game

    private int currentLevelIndex = 0;
    private int[,] currentMap;
    private List<int[,]> currentMapList; // Stores all maps generated in order
    private GameObject currentLevelParent; // Parent GameObject for current level tiles

    private PlayerController player;

    [Header("User Interface")]
    public TextMeshProUGUI levelNumberText;
    public GameObject WinScreen;
    public GameObject LoseScreen;
    public TextMeshProUGUI highscoreText;

    // Start is called before the first frame update
    void Start()
    {
        currentMapList = new List<int[,]>();
        player = FindObjectOfType<PlayerController>();
        BuildLevel(currentLevelIndex); // Updated method name
        WinScreen.SetActive(false);
        LoseScreen.SetActive(false);
    }

    public void BuildLevel(int levelIndex) // Renamed from GenerateLevel to BuildLevel
    {
        // if level not generated already, generate new one
        if (levelIndex > currentMapList.Count - 1)
        {
            currentMap = GenerateLevel.GenerateArray(gridSize);
            GenerateLevel.GeneratePath(currentMap);
            GenerateLevel.PadMapEdges(currentMap);

            // Optionally, add obstacles here if desired
            // GenerateLevel.AddObstacles(currentMap);

            currentMapList.Add(currentMap);
        }
        else
        {
            currentMap = currentMapList[levelIndex];
        }
        

        // Place the map at the appropriate height
        float y_pos = 0; // Since we only render one level at a time, y_pos can be zero
        PlaceMap(currentMap, y_pos);
        StartCoroutine(HideMap(currentMap, y_pos));
        levelNumberText.text = $"Level: {currentLevelIndex + 1}";
    }

    void PlaceMap(int[,] map, float y_pos)
    {
        // Clean up previous level
        if (currentLevelParent != null)
        {
            Destroy(currentLevelParent);
        }
        currentLevelParent = new GameObject("Level_" + currentLevelIndex);

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
                else if (map[row, col] == (int)GenerateLevel.TerrainType.Path || map[row, col] == (int)GenerateLevel.TerrainType.Edge) // if path or start/end edge
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

    IEnumerator HideMap(int[,] map, float y_pos)
    {
        yield return new WaitForSeconds(hideTilesTime);

        // UNCOMMENT THESE TO DESTROY VISIBLE MAP FIRST
        //if (currentLevelParent != null)
        //{
        //    Destroy(currentLevelParent);
        //}
        //currentLevelParent = new GameObject("Level_" + currentLevelIndex);

        // Where to place the new block
        float x_pos = INITIAL_X_POS;
        float z_pos = INITIAL_Z_POS;

        for (int row = 0; row < map.GetLength(0); row++)
        {
            x_pos = INITIAL_X_POS;
            for (int col = 0; col < map.GetLength(1); col++)
            {
                GameObject newTile;
                Vector3 position = new Vector3(x_pos, y_pos, z_pos);

                if (map[row, col] == (int)GenerateLevel.TerrainType.Edge)
                {
                    newTile = Instantiate(routeMaterial, position, Quaternion.identity, currentLevelParent.transform);
                }
                else
                {
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

        // map is now hidden so allow player to move
        if (player != null)
            player.allowedToMove = true;
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
            if (player != null)
                player.allowedToMove = false;
            ShowGameOverScreen(true);
            return;
        }

        BuildLevel(currentLevelIndex); // Updated method name

        // Move player to starting position
        if (player != null)
        {
            player.ResetPosition();
        }
    }

    public void BackToPreviousLevel()
    {
        currentLevelIndex--;
        if (currentLevelIndex < 0)
        {
            // Game Completed
            Debug.Log("You Lose!");
            // Implement game completion logic here
            if (player != null)
                player.allowedToMove = false;
            ShowGameOverScreen(false);
            return;
        }

        BuildLevel(currentLevelIndex); // Updated method name

        // Move player to starting position
        if (player != null)
        {
            player.ResetPosition();
        }
    }

    public void ShowGameOverScreen(bool gameWon)
    {
        if(gameWon && WinScreen != null)
            WinScreen.SetActive(true);
        else if (!gameWon && LoseScreen != null)
        {
            LoseScreen.SetActive(true);
            highscoreText.text = $"Highest level reached: {currentMapList.Count}";
        }
            
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
