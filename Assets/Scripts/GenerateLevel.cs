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

    public static int[,] GenerateArray(int size) {
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
    public static void ConstructPath(int[,] map) {
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
