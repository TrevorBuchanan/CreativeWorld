using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public ComputeShader meshComputeShader;

    private Mesh generatedMesh;
    private ComputeBuffer vertexBuffer;
    private ComputeBuffer indexBuffer;

    void Start()
    {
        int gridWidth = 100;
        int numVertices = gridWidth * gridWidth;

        // Create buffers
        vertexBuffer = new ComputeBuffer(numVertices, sizeof(float) * 3); // For vertex positions
        indexBuffer = new ComputeBuffer(numVertices, sizeof(uint));       // For indices if needed

        // Bind buffers to the compute shader
        int kernel = meshComputeShader.FindKernel("CSMain");
        meshComputeShader.SetBuffer(kernel, "VertexBuffer", vertexBuffer);
        meshComputeShader.SetBuffer(kernel, "IndexBuffer", indexBuffer);
        meshComputeShader.SetInt("GridWidth", gridWidth);

        // Dispatch compute shader
        int threadGroups = Mathf.CeilToInt((float)numVertices / 256);
        meshComputeShader.Dispatch(kernel, threadGroups, 1, 1);

        // Retrieve data from the GPU
        Vector3[] vertices = new Vector3[numVertices];
        vertexBuffer.GetData(vertices);

        // Generate the mesh
        GenerateMesh(vertices, gridWidth);
    }

    void GenerateMesh(Vector3[] vertices, int gridWidth)
    {
        generatedMesh = new Mesh();

        // Set vertices
        generatedMesh.vertices = vertices;

        // Generate indices for a grid (triangle list)
        int[] indices = new int[(gridWidth - 1) * (gridWidth - 1) * 6];
        int index = 0;
        for (int y = 0; y < gridWidth - 1; y++)
        {
            for (int x = 0; x < gridWidth - 1; x++)
            {
                int i = y * gridWidth + x;

                // Triangle 1
                indices[index++] = i;
                indices[index++] = i + gridWidth;
                indices[index++] = i + 1;

                // Triangle 2
                indices[index++] = i + 1;
                indices[index++] = i + gridWidth;
                indices[index++] = i + gridWidth + 1;
            }
        }

        generatedMesh.triangles = indices;

        // Apply the mesh to a MeshFilter
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter.mesh = generatedMesh;

        // Set a material (requires a material in your assets)
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }

    void OnDestroy()
    {
        // Release buffers
        vertexBuffer.Release();
        indexBuffer.Release();
    }
}