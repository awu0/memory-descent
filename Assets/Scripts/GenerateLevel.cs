using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    public enum TerrainType { Path, Grass, Water, Sand, Obstacle, Edge }

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
                map[x, y] = (int)TerrainType.Obstacle; // Default terrain is obstacles
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
            map[row, 0] = (int)TerrainType.Edge;               // Leftmost column
            map[row, size - 1] = (int)TerrainType.Edge;        // Rightmost column
        }
    }

    /// <summary>
    /// Generates only the primary path from left to right.
    /// </summary>
    public static void GeneratePath(int[,] map)
    {
        int currentRow = Random.Range(0, map.GetLength(0));
        int currentCol = 0;

        int maxColReached = 0;  // stores max column reached by path
        List<int> rowsInMaxCol = new List<int>();   // rows in the max column that have path

        map[currentRow, currentCol] = (int)TerrainType.Path;

        // First move must always be right
        currentCol += 1;
        map[currentRow, currentCol] = (int)TerrainType.Path;
        maxColReached += 1;
        rowsInMaxCol.Add(currentRow);

        //Debug.Log("NEW LEVEL");
        //Debug.Log($"R: {currentRow+1}, C: {currentCol+1}");
        
        while (currentCol < map.GetLength(1) - 2)
        {
            List<string> possibleMoves = GenerateMoves(map, currentRow, currentCol);

            if (possibleMoves.Count == 0)
            {
                // No valid moves, so jump to a random tile in the furthest column with a path
                currentCol = maxColReached;
                currentRow = rowsInMaxCol[Random.Range(0, rowsInMaxCol.Count)];
                while(map[currentRow, currentCol] != (int)TerrainType.Path)
                    currentRow = rowsInMaxCol[Random.Range(0, rowsInMaxCol.Count)];
                continue;
            }

            // Randomly select a move from the filtered list
            string move = possibleMoves[Random.Range(0, possibleMoves.Count)];

            if (move == "right")
                currentCol += 1;
            else if (move == "left")
                currentCol -= 1;
            else if (move == "up")
                currentRow -= 1;
            else if (move == "down")
                currentRow += 1;

            map[currentRow, currentCol] = (int)TerrainType.Path;

            // if current column is more than old max
            if (currentCol > maxColReached)
            {
                maxColReached = currentCol;
                rowsInMaxCol.Clear();
                rowsInMaxCol.Add(currentRow);
            }
            // if moving up or down in max column
            else if (currentCol == maxColReached)
            {
                rowsInMaxCol.Add(currentRow);
            }

            //Debug.Log($"R: {currentRow + 1}, C: {currentCol + 1}");
        }
    }

    //public static void GeneratePath(int[,] map)
    //{
    //    int currentRow = Random.Range(0, map.GetLength(0));
    //    int currentCol = 0;

    //    map[currentRow, currentCol] = (int)TerrainType.Path;

    //    // Keep track of the last two movements
    //    Queue<string> lastTwoMoves = new Queue<string>();

    //    while (currentCol < map.GetLength(1) - 1)
    //    {
    //        // Decide possible moves
    //        List<string> possibleMoves = new List<string>();

    //        // Can move right
    //        if (currentCol < map.GetLength(1) - 1)
    //            possibleMoves.Add("right");

    //        // Can move up if not at the top row
    //        if (currentRow > 0)
    //            possibleMoves.Add("up");

    //        // Can move down if not at the bottom row
    //        if (currentRow < map.GetLength(0) - 1)
    //            possibleMoves.Add("down");

    //        // Apply custom movement rules based on last two moves
    //        possibleMoves = ApplyMovementRules(possibleMoves, lastTwoMoves);

    //        // Filter moves that would create 4-way intersections
    //        possibleMoves = possibleMoves.FindAll(move =>
    //        {
    //            int nextRow = currentRow;
    //            int nextCol = currentCol;

    //            if (move == "right")
    //            {
    //                nextCol += 1;
    //            }
    //            else if (move == "up")
    //            {
    //                nextRow -= 1;
    //            }
    //            else if (move == "down")
    //            {
    //                nextRow += 1;
    //            }

    //            // Check if setting a path tile here would create a 4-way intersection
    //            return GetAdjacentPathCount(map, nextRow, nextCol) < 3;
    //        });

    //        if (possibleMoves.Count == 0)
    //        {
    //            // No valid moves, break the loop
    //            break;
    //        }

    //        // Randomly select a move from the filtered list
    //        string move = possibleMoves[Random.Range(0, possibleMoves.Count)];

    //        if (move == "right")
    //        {
    //            currentCol += 1;
    //        }
    //        else if (move == "up")
    //        {
    //            currentRow -= 1;
    //        }
    //        else if (move == "down")
    //        {
    //            currentRow += 1;
    //        }

    //        map[currentRow, currentCol] = (int)TerrainType.Path;

    //        // Update last two moves
    //        lastTwoMoves.Enqueue(move);
    //        if (lastTwoMoves.Count > 2)
    //        {
    //            lastTwoMoves.Dequeue();
    //        }
    //    }
    //}

    public static List<string> GenerateMoves(int[,] map, int currentRow, int currentCol)
    {
        List<string> possibleMoves = new List<string>();
        // Can move right
        if (currentCol < map.GetLength(1) - 2)
            possibleMoves.Add("right");

        // Can move up if not at the top row
        if (currentRow > 0)
            possibleMoves.Add("up");

        // Can move down if not at the bottom row
        if (currentRow < map.GetLength(0) - 1)
            possibleMoves.Add("down");

        // Can move left if not in the leftmost col
        if (currentCol > 1)
            possibleMoves.Add("left");

        List<string> allowedMoves = new List<string>();

        foreach (string move in possibleMoves)
        {
            int nextRow = currentRow;
            int nextCol = currentCol;

            if (move == "right")
                nextCol += 1;
            else if (move == "left")
                nextCol -= 1;
            else if (move == "up")
                nextRow -= 1;
            else if (move == "down")
                nextRow += 1;

            // if next move is already a path, don't add it to allowedMoves
            if (map[nextRow, nextCol] == (int)TerrainType.Path)
                continue;

            // if next move is making a 2x2 square, don't add it to allowedMoves
            if (IsMoveMakingSquare(map, nextRow, nextCol))
                continue;

            allowedMoves.Add(move);
        }

        return allowedMoves;
    }

    public static bool IsMoveMakingSquare(int[,] map, int row, int col)
    {
        // 2x2 square with next move on bottom right corner
        if (row > 0 && col > 1)
            if(map[row - 1, col] == (int)TerrainType.Path && map[row, col-1] == (int)TerrainType.Path && map[row-1, col - 1] == (int)TerrainType.Path)
                return true;
        // 2x2 square with next move on bottom left corner
        if (row > 0 && col < map.GetLength(1) - 2)
            if (map[row - 1, col] == (int)TerrainType.Path && map[row, col + 1] == (int)TerrainType.Path && map[row - 1, col + 1] == (int)TerrainType.Path)
                return true;
        // 2x2 square with next move on top right corner
        if (row < map.GetLength(0) - 1 && col > 1)
            if (map[row + 1, col] == (int)TerrainType.Path && map[row, col - 1] == (int)TerrainType.Path && map[row + 1, col - 1] == (int)TerrainType.Path)
                return true;
        // 2x2 square with next move on top left corner
        if (row < map.GetLength(0) - 1 && col < map.GetLength(1) - 2)
            if (map[row + 1, col] == (int)TerrainType.Path && map[row, col + 1] == (int)TerrainType.Path && map[row + 1, col + 1] == (int)TerrainType.Path)
                return true;
        return false;
    }

    //public static List<string> ApplyMovementRules(List<string> possibleMoves, Queue<string> lastTwoMoves)
    //{
    //    if (lastTwoMoves.Count < 2)
    //        return possibleMoves;

    //    string lastMove = lastTwoMoves.ToArray()[1];
    //    string secondLastMove = lastTwoMoves.ToArray()[0];

    //    // If the last two movements were "up" then "up", "left" would not be available.
    //    if (secondLastMove == "up" && lastMove == "up")
    //    {
    //        possibleMoves.Remove("left");
    //    }

    //    // If the last two movements were "down" then "down", "right" would not be available.
    //    if (secondLastMove == "down" && lastMove == "down")
    //    {
    //        possibleMoves.Remove("right");
    //    }

    //    // If the last two movements were "left" then "left", "up" would not be available.
    //    if (secondLastMove == "left" && lastMove == "left")
    //    {
    //        possibleMoves.Remove("up");
    //    }

    //    // If the last two movements were "up" then "left", "down" would not be available.
    //    if (secondLastMove == "up" && lastMove == "left")
    //    {
    //        possibleMoves.Remove("down");
    //    }

    //    // If the last two movements were "down" then "right", "up" would not be available.
    //    if (secondLastMove == "down" && lastMove == "right")
    //    {
    //        possibleMoves.Remove("up");
    //    }

    //    // If the last two movements were "right" then "up", "left" would not be available.
    //    if (secondLastMove == "right" && lastMove == "up")
    //    {
    //        possibleMoves.Remove("left");
    //    }

    //    // If the last two movements were "left" then "up", "right" would not be available.
    //    if (secondLastMove == "left" && lastMove == "up")
    //    {
    //        possibleMoves.Remove("right");
    //    }

    //    // If the last two movements were "up" then "down", "left" would not be available.
    //    if (secondLastMove == "up" && lastMove == "down")
    //    {
    //        possibleMoves.Remove("left");
    //    }

    //    // If the last two movements were "down" then "up", "right" would not be available.
    //    if (secondLastMove == "down" && lastMove == "up")
    //    {
    //        possibleMoves.Remove("right");
    //    }

    //    // If the last two movements were "right" then "down", "up" would not be available.
    //    if (secondLastMove == "right" && lastMove == "down")
    //    {
    //        possibleMoves.Remove("up");
    //    }


    //    return possibleMoves;
    //}

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
