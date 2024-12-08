using UnityEngine;

public class ChunkRenderer : MonoBehaviour
{
    public Chunk chunk;

    void Start()
    {
        chunk.GenerateMesh(0.5f); // Use a suitable iso level (e.g., 0.5 for surface)
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter.mesh = chunk.chunkMesh;
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }
}
