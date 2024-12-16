using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public static int Size = 16;
    public static int Divisions = 3;
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

        // Dictionary to store unique vertices and their indices
        Dictionary<Vector3, int> uniqueVertices = new Dictionary<Vector3, int>();

        int sections = Divisions + 1;

        // Iterate through each cube in the chunk
        for (int x = 0; x < Size * sections; x++)
        {
            for (int y = 0; y < Size * sections; y++)
            {
                for (int z = 0; z < Size * sections; z++)
                {
                    float[] cornerValues = new float[8];

                    // Get the scalar values at each corner of the cube
                    for (int i = 0; i < 8; i++)
                    {
                        Vector3 cornerPos = GetCubeCornerPos(x, y, z, i);
                        cornerValues[i] = VoxelGrid.GetValue((int)cornerPos.x, (int)cornerPos.y, (int)cornerPos.z);
                    }

                    int cubeIndex = 0;
                    if (cornerValues[0] < IsoLevel) cubeIndex |= 1;
                    if (cornerValues[1] < IsoLevel) cubeIndex |= 2;
                    if (cornerValues[2] < IsoLevel) cubeIndex |= 4;
                    if (cornerValues[3] < IsoLevel) cubeIndex |= 8;
                    if (cornerValues[4] < IsoLevel) cubeIndex |= 16;
                    if (cornerValues[5] < IsoLevel) cubeIndex |= 32;
                    if (cornerValues[6] < IsoLevel) cubeIndex |= 64;
                    if (cornerValues[7] < IsoLevel) cubeIndex |= 128;

                    int[] edges = MarchingCubes.TriangleTable[cubeIndex];
                    if (edges[0] != -1)
                    {
                        for (int i = 0; edges[i] != -1; i += 3)
                        {
                            int[] edgeIndices = { edges[i], edges[i + 1], edges[i + 2] };
                            foreach (int edgeIndex in edgeIndices)
                            {
                                int vertex1_index = MarchingCubes.EdgeVertexIndices[edgeIndex, 0];
                                int vertex2_index = MarchingCubes.EdgeVertexIndices[edgeIndex, 1];
                                Vector3 interpolatedVertex = MarchingCubes.VertexInterpolate(
                                    GetCubeCornerPos(x, y, z, vertex1_index),
                                    GetCubeCornerPos(x, y, z, vertex2_index),
                                    cornerValues[vertex1_index],
                                    cornerValues[vertex2_index],
                                    IsoLevel
                                );

                                // Normalize to chunk scale
                                interpolatedVertex /= (Divisions + 1);

                                // Add vertex or reuse existing one
                                if (!uniqueVertices.TryGetValue(interpolatedVertex, out int vertexIndex))
                                {
                                    vertexIndex = vertices.Count;
                                    uniqueVertices[interpolatedVertex] = vertexIndex;
                                    vertices.Add(interpolatedVertex);
                                }

                                triangles.Add(vertexIndex);
                            }
                        }
                    }
                }
            }
        }

        if (vertices.Count > 65535)
        {
            Debug.LogError($"Vertex count exceeds Unity's mesh limit of 65535 at {vertices.Count}.");
        }
        Debug.Log($"Vertex count after deduplication: {vertices.Count}.");

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
