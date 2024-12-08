using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public VoxelGrid voxelGrid;
    public Mesh chunkMesh;

    public Chunk(int width, int height, int depth)
    {
        voxelGrid = new VoxelGrid(width, height, depth);
        chunkMesh = new Mesh();
    }

    // Generate the mesh for the chunk using Marching Cubes
    public void GenerateMesh(float isoLevel)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Iterate through each cube in the chunk
        for (int x = 0; x < voxelGrid.values.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < voxelGrid.values.GetLength(1) - 1; y++)
            {
                for (int z = 0; z < voxelGrid.values.GetLength(2) - 1; z++)
                {
                    // Define an array to hold the corner values (scalar field values at each corner)
                    float[] cornerValues = new float[8];

                    // Get the scalar values at each corner of the cube
                    for (int i = 0; i < 8; i++)
                    {
                        Vector3 cornerPos = GetCubeCorner(x, y, z, i);
                        cornerValues[i] = voxelGrid.GetValue((int)cornerPos.x, (int)cornerPos.y, (int)cornerPos.z);
                    }

                    // Determine the cube index (based on 8 corner values)
                    int cubeIndex = 0;
                    // for (int i = 0; i < 8; i++)
                    // {
                    //     if (cornerValues[i] < isoLevel)
                    //     {
                    //         cubeIndex |= 1 << i;
                    //     }
                    // }
                    // Same but easier to read
                    if (cornerValues[0] < isoLevel) cubeIndex |= 1;
                    if (cornerValues[1] < isoLevel) cubeIndex |= 2;
                    if (cornerValues[2] < isoLevel) cubeIndex |= 4;
                    if (cornerValues[3] < isoLevel) cubeIndex |= 8;
                    if (cornerValues[4] < isoLevel) cubeIndex |= 16;
                    if (cornerValues[5] < isoLevel) cubeIndex |= 32;
                    if (cornerValues[6] < isoLevel) cubeIndex |= 64;
                    if (cornerValues[7] < isoLevel) cubeIndex |= 128;

                    // Use the triangle table to generate the triangles
                    int[] edges = MarchingCubes.TriangleTable[cubeIndex];
                    if (edges[0] != -1) // Skip empty cubes
                    {
                        for (int i = 0; edges[i] != -1; i += 3)
                        {
                            Vector3 v1 = MarchingCubes.VertexInterpolate(
                                GetCubeCorner(x, y, z, MarchingCubes.EdgeVertexIndices[edges[i], 0]),
                                GetCubeCorner(x, y, z, MarchingCubes.EdgeVertexIndices[edges[i + 1], 0]),
                                cornerValues[MarchingCubes.EdgeVertexIndices[edges[i], 0]],
                                cornerValues[MarchingCubes.EdgeVertexIndices[edges[i + 1], 0]],
                                isoLevel
                            );

                            Vector3 v2 = MarchingCubes.VertexInterpolate(
                                GetCubeCorner(x, y, z, MarchingCubes.EdgeVertexIndices[edges[i + 1], 0]),
                                GetCubeCorner(x, y, z, MarchingCubes.EdgeVertexIndices[edges[i + 2], 0]),
                                cornerValues[MarchingCubes.EdgeVertexIndices[edges[i + 1], 0]],
                                cornerValues[MarchingCubes.EdgeVertexIndices[edges[i + 2], 0]],
                                isoLevel
                            );

                            Vector3 v3 = MarchingCubes.VertexInterpolate(
                                GetCubeCorner(x, y, z, MarchingCubes.EdgeVertexIndices[edges[i + 2], 0]),
                                GetCubeCorner(x, y, z, MarchingCubes.EdgeVertexIndices[edges[i], 0]),
                                cornerValues[MarchingCubes.EdgeVertexIndices[edges[i + 2], 0]],
                                cornerValues[MarchingCubes.EdgeVertexIndices[edges[i], 0]],
                                isoLevel
                            );

                            vertices.Add(v1);
                            vertices.Add(v2);
                            vertices.Add(v3);
                            triangles.Add(vertices.Count - 3);
                            triangles.Add(vertices.Count - 2);
                            triangles.Add(vertices.Count - 1);
                        }
                    }
                }
            }
        }

        chunkMesh.Clear();
        chunkMesh.vertices = vertices.ToArray();
        chunkMesh.triangles = triangles.ToArray();
        chunkMesh.RecalculateNormals();
    }

    // Get the position of a cube corner based on its index (0-7)
    private Vector3 GetCubeCorner(int x, int y, int z, int cornerIndex)
    {
        Vector3[] cornerOffsets = {
            new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0, 0, 1),
            new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1), new Vector3(0, 1, 1)
        };
        return new Vector3(x, y, z) + cornerOffsets[cornerIndex];
    }
}
