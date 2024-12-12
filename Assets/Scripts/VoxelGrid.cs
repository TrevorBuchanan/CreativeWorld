using UnityEngine;

public class VoxelGrid
{
    public float[,,] values; // 3D array to store scalar values
    public static int Seed = 0;
    private NoiseGenerator3D noiseGenerator3D = new NoiseGenerator3D(Seed);
    private NoiseGenerator2D noiseGenerator2D = new NoiseGenerator2D(Seed);

    public VoxelGrid(Vector3 offset, int width, int height, int depth, int divisions)
    {   
        int sections = divisions + 1;
        values = new float[(width * sections) + 1, (height * sections) + 1, (depth * sections) + 1]; // + 1 for edges
        // Debug.Log($"Num divisions: {divisions}");
        // Debug.Log($"Num sections: {sections}");
        // Debug.Log($"Width: {width}");
        // Debug.Log($"Height: {height}");
        // Debug.Log($"Depth: {depth}");

        // Initialize the voxel grid with some values
        for (int x = 0; x <= (width * sections); x++)
        {
            for (int y = 0; y <= (height * sections); y++)
            {
                for (int z = 0; z <= (depth * sections); z++)
                {
                    float noiseVal = noiseGenerator3D.GeneratePerlin(offset.x + ( (float)x / sections ), offset.y + ((float)y / sections), offset.z + ((float)z / sections));
                    // Debug.Log($"Point (x, y, z): {x}, {y}, {z} -> \nDivisions: {(float)x / sections}, {(float)y / sections}, {(float)z / sections}\nOffsets: {offset.x}, {offset.y}, {offset.z}\nValues: {offset.x + ( (float)x / sections )}, {offset.y + ((float)y / sections)}, {offset.z + ((float)z / sections)} -> {noiseVal}");
                    values[x, y, z] = noiseVal;
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
