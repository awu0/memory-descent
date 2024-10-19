using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    public enum TerrainType { Path, Grass, Water, Sand, Obstacle }

    public int gridSize = 10;           // You can scale this value to increase complexity dynamically

    void Start()
    {
        int[,] map = GenerateArray(gridSize);
        GeneratePath(map);

        // Pad the left and right sides with paths
        PadMapEdges(map);

        // Print the map for debugging
        PrintMap(map);
    }

    public static int[,] GenerateArray(int size)
    {
        int[,] map = new int[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                map[x, y] = (int)TerrainType.Grass; // Default terrain is grass
            }
        }
        return map;
    }

    /// <summary>
    /// Pads the leftmost and rightmost columns with paths.
    /// </summary>
    public static void PadMapEdges(int[,] map)
    {
        int size = map.GetLength(0); // Assuming a square map
        for (int row = 0; row < size; row++)
        {
            map[row, 0] = (int)TerrainType.Path;               // Leftmost column
            map[row, size - 1] = (int)TerrainType.Path;        // Rightmost column
        }
    }

    /// <summary>
    /// Generates only the primary path from left to right.
    /// </summary>
    public static void GeneratePath(int[,] map)
    {
        int currentRow = Random.Range(0, map.GetLength(0));
        int currentCol = 0;

        map[currentRow, currentCol] = (int)TerrainType.Path;

        // Keep track of the last two movements
        Queue<string> lastTwoMoves = new Queue<string>();

        while (currentCol < map.GetLength(1) - 1)
        {
            // Decide possible moves
            List<string> possibleMoves = new List<string>();

            // Can move right
            if (currentCol < map.GetLength(1) - 1)
                possibleMoves.Add("right");

            // Can move up if not at the top row
            if (currentRow > 0)
                possibleMoves.Add("up");

            // Can move down if not at the bottom row
            if (currentRow < map.GetLength(0) - 1)
                possibleMoves.Add("down");

            // Apply custom movement rules based on last two moves
            possibleMoves = ApplyMovementRules(possibleMoves, lastTwoMoves);

            // Filter moves that would create 4-way intersections
            possibleMoves = possibleMoves.FindAll(move =>
            {
                int nextRow = currentRow;
                int nextCol = currentCol;

                if (move == "right")
                {
                    nextCol += 1;
                }
                else if (move == "up")
                {
                    nextRow -= 1;
                }
                else if (move == "down")
                {
                    nextRow += 1;
                }

                // Check if setting a path tile here would create a 4-way intersection
                return GetAdjacentPathCount(map, nextRow, nextCol) < 3;
            });

            if (possibleMoves.Count == 0)
            {
                // No valid moves, break the loop
                break;
            }

            // Randomly select a move from the filtered list
            string move = possibleMoves[Random.Range(0, possibleMoves.Count)];

            if (move == "right")
            {
                currentCol += 1;
            }
            else if (move == "up")
            {
                currentRow -= 1;
            }
            else if (move == "down")
            {
                currentRow += 1;
            }

            map[currentRow, currentCol] = (int)TerrainType.Path;

            // Update last two moves
            lastTwoMoves.Enqueue(move);
            if (lastTwoMoves.Count > 2)
            {
                lastTwoMoves.Dequeue();
            }
        }
    }

    public static List<string> ApplyMovementRules(List<string> possibleMoves, Queue<string> lastTwoMoves)
    {
        if (lastTwoMoves.Count < 2)
            return possibleMoves;

        string lastMove = lastTwoMoves.ToArray()[1];
        string secondLastMove = lastTwoMoves.ToArray()[0];

        // If the last two movements were "up" then "up", "left" would not be available.
        if (secondLastMove == "up" && lastMove == "up")
        {
            possibleMoves.Remove("left");
        }

        // If the last two movements were "down" then "down", "right" would not be available.
        if (secondLastMove == "down" && lastMove == "down")
        {
            possibleMoves.Remove("right");
        }

        // If the last two movements were "left" then "left", "up" would not be available.
        if (secondLastMove == "left" && lastMove == "left")
        {
            possibleMoves.Remove("up");
        }

        // If the last two movements were "up" then "left", "down" would not be available.
        if (secondLastMove == "up" && lastMove == "left")
        {
            possibleMoves.Remove("down");
        }

        // If the last two movements were "down" then "right", "up" would not be available.
        if (secondLastMove == "down" && lastMove == "right")
        {
            possibleMoves.Remove("up");
        }

        // If the last two movements were "right" then "up", "left" would not be available.
        if (secondLastMove == "right" && lastMove == "up")
        {
            possibleMoves.Remove("left");
        }

        // If the last two movements were "left" then "up", "right" would not be available.
        if (secondLastMove == "left" && lastMove == "up")
        {
            possibleMoves.Remove("right");
        }

        // If the last two movements were "up" then "down", "left" would not be available.
        if (secondLastMove == "up" && lastMove == "down")
        {
            possibleMoves.Remove("left");
        }

        // If the last two movements were "down" then "up", "right" would not be available.
        if (secondLastMove == "down" && lastMove == "up")
        {
            possibleMoves.Remove("right");
        }

        // If the last two movements were "right" then "down", "up" would not be available.
        if (secondLastMove == "right" && lastMove == "down")
        {
            possibleMoves.Remove("up");
        }


        return possibleMoves;
    }

    /// <summary>
    /// Returns the number of adjacent path tiles to the given tile.
    /// </summary>
    public static int GetAdjacentPathCount(int[,] map, int row, int col)
    {
        int count = 0;
        int numRows = map.GetLength(0);
        int numCols = map.GetLength(1);

        // Check up
        if (row > 0 && map[row - 1, col] == (int)TerrainType.Path) count++;
        // Check down
        if (row < numRows - 1 && map[row + 1, col] == (int)TerrainType.Path) count++;
        // Check left
        if (col > 0 && map[row, col - 1] == (int)TerrainType.Path) count++;
        // Check right
        if (col < numCols - 1 && map[row, col + 1] == (int)TerrainType.Path) count++;

        return count;
    }

    public static void PrintMap(int[,] map)
    {
        for (int row = 0; row < map.GetLength(0); row++)
        {
            string rowOutput = "";
            for (int col = 0; col < map.GetLength(1); col++)
            {
                rowOutput += $"{map[row, col]} ";
            }
            Debug.Log(rowOutput);
        }
    }
}
