using UnityEngine;

public class VoxelGrid
{
    public float[,,] values; // 3D array to store scalar values
    public static int Seed = 0; // FIXME: Seed doesn't work like it should
    private NoiseGenerator3D noiseGenerator3D = new NoiseGenerator3D(Seed);
    private NoiseGenerator2D noiseGenerator2D = new NoiseGenerator2D(Seed);

    public VoxelGrid(int width, int height, int depth)
    {
        values = new float[width + 1, height + 1, depth + 1];

        // Initialize the voxel grid with some values
        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < depth + 1; z++)
                {
                    values[x, y, z] = noiseGenerator3D.GeneratePerlin(x, y, z);
                    Debug.Log(values[x, y, z]);
                }
            }
        }

        // FIXME: REMOVE
        // values[0, 0, 0] = 0.0f;
        // values[0, 0, 1] = 1.0f;
        // values[0, 1, 0] = 1.0f;
        // values[0, 1, 1] = 1.0f;
        // values[1, 0, 0] = 1.0f;
        // values[1, 0, 1] = 1.0f;
        // values[1, 1, 0] = 1.0f;
        // values[1, 1, 1] = 1.0f;
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
