using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public int chunkWidth = 16;
    public int chunkHeight = 16;
    public int chunkDepth = 16;
    public float isoLevel = 0.5f;

    private Chunk chunk;

    void Start()
    {
        // Create and generate the chunk mesh
        chunk = new Chunk(chunkWidth, chunkHeight, chunkDepth);
        chunk.GenerateMesh(isoLevel);

        // Assign the generated mesh to the MeshFilter component
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = chunk.chunkMesh;

        // Optionally, assign a MeshRenderer if you want it to be visible
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }
}
