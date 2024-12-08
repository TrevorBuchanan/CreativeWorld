using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public GameObject chunkPrefab;
    public int chunksPerRow = 4;
    public float chunkSize = 16f;

    void Start()
    {
        for (int x = 0; x < chunksPerRow; x++)
        {
            for (int z = 0; z < chunksPerRow; z++)
            {
                // Instantiate a chunk at a specific position
                Vector3 chunkPosition = new Vector3(x * chunkSize, 0, z * chunkSize);
                GameObject chunk = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
                chunk.name = $"Chunk_{x}_{z}";
            }
        }
    }
}
