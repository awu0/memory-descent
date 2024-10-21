using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelController : MonoBehaviour
{
    public float INITIAL_X_POS = 0;
    public float INITIAL_Z_POS = 0;

    public float hideTilesTime = 3f; // Time in seconds before tiles are hidden

    public GameObject groundMaterial;
    public GameObject routeMaterial;
    public GameObject obstacleMaterial; // Obstacle tile prefab

    public int startGridSize = 6; // starting grid size
    public int increaseGridInterval = 2; // increase grid size after every 2 levels
    public float cameraZoomChange = 1.25f; // how much to zoom out/in camera
    public float levelTransitionTime = 1f; // time for moving levels while transitioning
    public int totalLevels = 5; // Total number of levels in the game
    public int gridSize; // current grid size. shouldn't be changed in editor

    private int currentLevelIndex = 0;
    private int[,] currentMap;
    private List<int[,]> currentMapList; // Stores all maps generated in order
    private GameObject currentLevelParent; // Parent GameObject for current level tiles

    private PlayerController player;
    private IsometricCamera isoCamera;

    [Header("User Interface")]
    public TextMeshProUGUI levelNumberText;
    public GameObject WinScreen;
    public GameObject LoseScreen;
    public TextMeshProUGUI highscoreText;

    private Vector3 oldLevelTarget;     // lerp target for position of old level
    private GameObject oldLevelParent;  // parent of old level
    private Vector3 newLevelStart;     // lerp start for position of new level
    private float transitionTimeElapsed;// used to control lerp alpha

    // Start is called before the first frame update
    void Start()
    {
        gridSize = startGridSize;
        currentMapList = new List<int[,]>();
        player = FindObjectOfType<PlayerController>();
        isoCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<IsometricCamera>();
        BuildLevel(currentLevelIndex, 0f); // Updated method name
        player.ResetPosition();
        WinScreen.SetActive(false);
        LoseScreen.SetActive(false);
        transitionTimeElapsed = levelTransitionTime;
    }

    void Update()
    {
        if (transitionTimeElapsed < levelTransitionTime)
        {
            oldLevelParent.transform.position =
                Vector3.Lerp(Vector3.zero, oldLevelTarget, transitionTimeElapsed / levelTransitionTime);
            currentLevelParent.transform.position =
                Vector3.Lerp(newLevelStart, Vector3.zero, transitionTimeElapsed / levelTransitionTime);

            transitionTimeElapsed += Time.deltaTime;
        }
        else
        {
            currentLevelParent.transform.position = Vector3.zero;
        }
    }

    public void BuildLevel(int levelIndex, float y_pos) // Renamed from GenerateLevel to BuildLevel
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
        PlaceMap(currentMap, y_pos);
        StartCoroutine(HideMap(currentMap, 0f));
        levelNumberText.text = $"Level: {currentLevelIndex + 1}";
    }

    void PlaceMap(int[,] map, float y_pos)
    {
        currentLevelParent = new GameObject("Level_" + currentLevelIndex);
        currentLevelParent.transform.position = new Vector3(currentLevelParent.transform.position.x, y_pos,
            currentLevelParent.transform.position.z);

        Renderer tempRenderer = groundMaterial.GetComponentInChildren<Renderer>();
        float x_size = tempRenderer.bounds.size.x;
        float z_size = tempRenderer.bounds.size.z;
        INITIAL_X_POS = -(((float)gridSize / 2) - 0.5f) * x_size;
        INITIAL_Z_POS = (((float)gridSize / 2) - 0.5f) * z_size;

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
                Vector3 position = new Vector3(x_pos, currentLevelParent.transform.position.y, z_pos);

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
        yield return new WaitForSeconds(levelTransitionTime + hideTilesTime);

        // UNCOMMENT THESE TO DESTROY VISIBLE MAP FIRST
        if (currentLevelParent != null)
        {
            Destroy(currentLevelParent);
        }
        currentLevelParent = new GameObject("Level_" + currentLevelIndex);
        currentLevelParent.transform.position = new Vector3(currentLevelParent.transform.position.x, y_pos,
            currentLevelParent.transform.position.z);

        Renderer tempRenderer = groundMaterial.GetComponentInChildren<Renderer>();
        float x_size = tempRenderer.bounds.size.x;
        float z_size = tempRenderer.bounds.size.z;
        INITIAL_X_POS = -(((float)gridSize / 2) - 0.5f) * x_size;
        INITIAL_Z_POS = (((float)gridSize / 2) - 0.5f) * z_size;

        // Where to place the new block
        float x_pos = INITIAL_X_POS;
        float z_pos = INITIAL_Z_POS;

        for (int row = 0; row < map.GetLength(0); row++)
        {
            x_pos = INITIAL_X_POS;
            for (int col = 0; col < map.GetLength(1); col++)
            {
                GameObject newTile;
                Vector3 position = new Vector3(x_pos, currentLevelParent.transform.position.y, z_pos);

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

        // destroy previous level if it exists
        if (oldLevelParent != null)
            Destroy(oldLevelParent);
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

        // increase grid size
        if (currentLevelIndex % increaseGridInterval == 0)
        {
            gridSize += 1;
            isoCamera.ChangeOrthographicSize(cameraZoomChange);
        }

        // move old level down
        oldLevelTarget = new Vector3(0f, -50f, 0f);
        oldLevelParent = currentLevelParent;
        newLevelStart = new Vector3(0f, 50f, 0f);
        transitionTimeElapsed = 0f;

        BuildLevel(currentLevelIndex, 50f); // Updated method name

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

        if ((currentLevelIndex+1) % increaseGridInterval == 0)
        {
            gridSize -= 1;
            isoCamera.ChangeOrthographicSize(-cameraZoomChange);
        }

        // move old level up
        oldLevelTarget = new Vector3(0f, 50f, 0f);
        oldLevelParent = currentLevelParent;
        newLevelStart = new Vector3(0f, -50f, 0f);
        transitionTimeElapsed = 0f;

        BuildLevel(currentLevelIndex, -50f); // Updated method name

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
