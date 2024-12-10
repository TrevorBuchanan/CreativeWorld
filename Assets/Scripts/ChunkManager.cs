using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public GameObject spherePrefab;  // TODO: REMOVE
    public Color StartColor = Color.black;  // TODO: REMOVE
    public Color EndColor = Color.white;  // TODO: REMOVE

    public int chunkWidth = 1;
    public int chunkHeight = 1;
    public int chunkDepth = 1;
    public float isoLevel = 0.5f;

    private Chunk chunk;

    void Start()
    {
        // Create and generate the chunk mesh
        chunk = new Chunk(chunkWidth, chunkHeight, chunkDepth);


        // TODO: REMOVE ____________________________________________
        for (int x = 0; x < chunkWidth + 1; x++)
        {
            for (int y = 0; y < chunkHeight + 1; y++)
            {
                for (int z = 0; z < chunkDepth + 1; z++)
                {
                    
                    float noiseValue = chunk.voxelGrid.values[x, y, z];
                    Vector3 position = new Vector3(x, y, z);

                    GameObject sphere = Instantiate(spherePrefab, position, Quaternion.identity);
                    sphere.transform.localScale = Vector3.one * 0.1f;

                    Renderer sphereRenderer = sphere.GetComponent<Renderer>();
                    if (sphereRenderer != null)
                    {
                        if (noiseValue > isoLevel) sphereRenderer.material.color = EndColor;
                        else sphereRenderer.material.color = StartColor;
                    }
                }
            }
        }
        // TODO: REMOVE ____________________________________________

        chunk.GenerateMesh(isoLevel);

        // Assign the generated mesh to the MeshFilter component
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = chunk.chunkMesh;

        // Optionally, assign a MeshRenderer if you want it to be visible
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }
}
