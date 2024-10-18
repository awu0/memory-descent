using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static int[,] GenerateArray(int size)
    {
        int[,] map = new int[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                map[x, y] = 0;
            }
        }
        return map;
    }


    /// <summary>
    /// Generate the path the player has to take for the level.
    /// 0 is not on the path
    /// 1 is on the path
    /// </summary>
    /// <param name="map"></param>
    public static void ConstructPath(int[,] map)
    {
        int WIDTH = map.GetLength(0);
        int LENGTH = map.GetLength(1);

        // Start at a random row in the first column
        int currentRow = Random.Range(0, WIDTH);
        int currentCol = 0;

        map[currentRow, currentCol] = 1; // Mark the starting position
        currentCol++;
        map[currentRow, currentCol] = 1; // The next position has to go to the right

        int previousDirection = 0;
        while (currentCol < LENGTH - 1)
        {
            // Choose a direction: right, up, or down
            int direction = Random.Range(-1, 2); // -1 for up, 0 for right, 1 for down
            int newRow = currentRow;
            int newCol = currentCol;

            // prevent reversing directions (i.e. last direction was up, but this direction is down)
            // also check bounds
            while (
                (newRow == currentRow && newCol == currentCol) ||
                (previousDirection == -1 && direction == 1) ||
                (previousDirection == 1 && direction == -1) ||
                newRow < 0 ||
                newRow >= LENGTH ||
                (newRow >= 0 && newRow < LENGTH && direction != 0 && map[newRow, currentCol - 1] == 1)
            )
            {
                direction = Random.Range(-1, 2);

                newRow = currentRow + direction;
                newCol = currentCol;
                if (direction == 0)
                {
                    newCol = currentCol + 1;
                }
            }

            // Debug.Log(newRow.ToString() + ", " + newCol.ToString());

            previousDirection = direction;
            currentRow = newRow;
            currentCol = newCol;
            map[currentRow, currentCol] = 1;
        }
    }

    public static void PrintMap(int[,] map)
    {
        for (int row = 0; row < map.GetLength(0); row++) // Loop through rows
        {
            string rowOutput = "";
            for (int col = 0; col < map.GetLength(1); col++) // Loop through columns
            {
                rowOutput += map[row, col] + " "; // Append each value to the row string
            }
            Debug.Log(rowOutput); // Print the row string in Unity's Console
        }
    }
}
