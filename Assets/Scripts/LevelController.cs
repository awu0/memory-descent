using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private readonly float INITIAL_X_POS = 0;
    private readonly float INITIAL_Z_POS = 0;

    public GameObject groundMaterial;
    public GameObject routeMaterial;

    int[,] currentMap;

    // Start is called before the first frame update
    void Start()
    {
        currentMap = GenerateLevel.GenerateArray(10);

        GenerateLevel.ConstructPath(currentMap);

        PlaceMap(currentMap, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PlaceMap(int[,] map, float y_pos)
    {
        // where to put the new block
        float x_pos = INITIAL_X_POS;
        float z_pos = INITIAL_Z_POS;

        // row is the x, col is the z
        for (int row = 0; row < map.GetLength(0); row++)
        {
            x_pos = INITIAL_X_POS;
            for (int col = 0; col < map.GetLength(1); col++)
            {
                GameObject newGround;
                if (map[row, col] == 0) {
                    newGround = Instantiate(groundMaterial, new Vector3(x_pos, y_pos, z_pos), Quaternion.identity);
                } else {
                    newGround = Instantiate(routeMaterial, new Vector3(x_pos, y_pos, z_pos), Quaternion.identity);
                }

                // calculate next block's location
                Renderer groundRenderer = newGround.GetComponentInChildren<Renderer>();
                Vector3 objectSize = groundRenderer.bounds.size;
                x_pos += objectSize.x;

                // new row after all the blocks are placed
                if (col == map.GetLength(1) - 1) z_pos -= objectSize.z;
            }
        }
    }
}
