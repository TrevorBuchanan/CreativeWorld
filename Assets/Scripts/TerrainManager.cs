using System;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{

    public GameObject ChunkPrefab;
    // public int RenderDistance; // In chunks
    public int Size;

    void Start()
    {
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                for (int z = 0; z < Size; z++)
                {
                    // Instantiate a chunk at a specific position
                    Vector3 chunkPosition = new Vector3(x * Chunk.Size, y * Chunk.Size, z * Chunk.Size);
                    GameObject chunk = Instantiate(ChunkPrefab, chunkPosition, Quaternion.identity);
                    chunk.name = $"Chunk_{x}_{z}";
                }
            }
        }
    }
}
