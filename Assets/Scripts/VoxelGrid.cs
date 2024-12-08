using UnityEngine;

public class VoxelGrid
{
    public float[,,] values; // 3D array to store scalar values

    public VoxelGrid(int width, int height, int depth)
    {
        values = new float[width, height, depth];

        // Initialize the voxel grid with some values
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    values[x, y, z] = Mathf.PerlinNoise(x * 0.1f, z * 0.1f); // Use Perlin noise for terrain generation
                }
            }
        }
    }

    // Get the value at a specific position (with bounds checking)
    public float GetValue(int x, int y, int z)
    {
        if (x >= 0 && x < values.GetLength(0) && y >= 0 && y < values.GetLength(1) && z >= 0 && z < values.GetLength(2))
        {
            return values[x, y, z];
        }
        return 0f; // Default value if out of bounds
    }
}
