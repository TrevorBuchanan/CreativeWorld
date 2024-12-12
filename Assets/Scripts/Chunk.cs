using System;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    // FIXME: Breaks at certain sizes
    public static int Size = 23;
    public static int Divisions = 1;
    public VoxelGrid VoxelGrid;
    public static float IsoLevel = 0.43f;
    private Mesh chunkMesh;

    void Start()
    {
        VoxelGrid = new VoxelGrid(transform.position, Size, Size, Size, Divisions);
        // Create and generate the chunk mesh
        GenerateMesh();

        // Assign the generated mesh to the MeshFilter component
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = chunkMesh;

        // Optionally, assign a MeshRenderer if you want it to be visible
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }

    // Generate the mesh for the chunk using Marching Cubes
    public void GenerateMesh()
    {
        chunkMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Iterate through each cube in the chunk
        for (int x = 0; x < VoxelGrid.values.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < VoxelGrid.values.GetLength(1) - 1; y++)
            {
                for (int z = 0; z < VoxelGrid.values.GetLength(2) - 1; z++)
                {
                    // Define an array to hold the corner values (scalar field values at each corner)
                    float[] cornerValues = new float[8];

                    // Get the scalar values at each corner of the cube
                    for (int i = 0; i < 8; i++)
                    {
                        Vector3 cornerPos = GetCubeCornerPos(x, y, z, i);
                        cornerValues[i] = VoxelGrid.GetValue((int)cornerPos.x, (int)cornerPos.y, (int)cornerPos.z);
                    }

                    // Determine the cube index (based on 8 corner values)
                    int cubeIndex = 0;
                    if (cornerValues[0] < IsoLevel) cubeIndex |= 1;
                    if (cornerValues[1] < IsoLevel) cubeIndex |= 2;
                    if (cornerValues[2] < IsoLevel) cubeIndex |= 4;
                    if (cornerValues[3] < IsoLevel) cubeIndex |= 8;
                    if (cornerValues[4] < IsoLevel) cubeIndex |= 16;
                    if (cornerValues[5] < IsoLevel) cubeIndex |= 32;
                    if (cornerValues[6] < IsoLevel) cubeIndex |= 64;
                    if (cornerValues[7] < IsoLevel) cubeIndex |= 128;

                    // Use the triangle table to generate the triangles
                    int vertex1_index, vertex2_index;
                    int[] edges = MarchingCubes.TriangleTable[cubeIndex];
                    if (edges[0] != -1) // Skip empty cubes
                    {
                        for (int i = 0; edges[i] != -1; i += 3)
                        {
                            vertex1_index = MarchingCubes.EdgeVertexIndices[edges[i], 0];
                            vertex2_index =  MarchingCubes.EdgeVertexIndices[edges[i], 1];
                            Vector3 v1 = MarchingCubes.VertexInterpolate(
                                GetCubeCornerPos(x, y, z, vertex1_index),
                                GetCubeCornerPos(x, y, z, vertex2_index),
                                cornerValues[vertex1_index],
                                cornerValues[vertex2_index],
                                IsoLevel
                            );

                            vertex1_index = MarchingCubes.EdgeVertexIndices[edges[i + 1], 0];
                            vertex2_index =  MarchingCubes.EdgeVertexIndices[edges[i + 1], 1];
                            Vector3 v2 = MarchingCubes.VertexInterpolate(
                                GetCubeCornerPos(x, y, z, vertex1_index),
                                GetCubeCornerPos(x, y, z, vertex2_index),
                                cornerValues[vertex1_index],
                                cornerValues[vertex2_index],
                                IsoLevel
                            );

                            vertex1_index = MarchingCubes.EdgeVertexIndices[edges[i + 2], 0];
                            vertex2_index =  MarchingCubes.EdgeVertexIndices[edges[i + 2], 1];
                            Vector3 v3 = MarchingCubes.VertexInterpolate(
                                GetCubeCornerPos(x, y, z, vertex1_index),
                                GetCubeCornerPos(x, y, z, vertex2_index),
                                cornerValues[vertex1_index],
                                cornerValues[vertex2_index],
                                IsoLevel
                            );

                            vertices.Add(v1 / (Divisions + 1)); // + 1 because number of segments
                            vertices.Add(v2 / (Divisions + 1));
                            vertices.Add(v3 / (Divisions + 1));
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
    private Vector3 GetCubeCornerPos(int x, int y, int z, int cornerIndex)
    {
        Vector3[] cornerOffsets = {
            new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0),
            new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(0, 1, 1), new Vector3(1, 1, 1)
        };
        return new Vector3(x, y, z) + cornerOffsets[cornerIndex];
    }
}
