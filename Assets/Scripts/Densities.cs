using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Densities : MonoBehaviour
{
    public GameObject spherePrefab; // The sphere prefab to instantiate
    public float sphereScale = 0.1f; // Size of the spheres
    // public float Frequency = 1.0f; // How "stretched" the noise is
    // public float Amplitude = 1.0f; // Intensity of the noise
    // public Vector3 Offset = Vector3.zero; // Offset for the noise, useful for scrolling
    public Color StartColor = Color.black;
    public Color EndColor = Color.white;

    private void Start() {
        NoiseGenerator3D noiseGenerator3D = new NoiseGenerator3D();

        // Iterate over a 32x32x32 grid
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                for (int z = 0; z < 32; z++)
                {
                    float noiseValue = noiseGenerator3D.GeneratePerlin(x, y, z);
                    // Debug.Log($"Noise at ({x},{y},{z}): {noiseValue}");

                    // Instantiate a sphere at the (x, y, z) position
                    Vector3 position = new Vector3(x, y, z);

                    // Instantiate the sphere prefab
                    GameObject sphere = Instantiate(spherePrefab, position, Quaternion.identity);
                    sphere.transform.localScale = Vector3.one * sphereScale; // Scale the sphere

                    // Set the color based on the noise value (1 -> black, 0 -> white)
                    Renderer sphereRenderer = sphere.GetComponent<Renderer>();
                    if (sphereRenderer != null)
                    {
                        // Debug.Log(noiseValue);
                        // Interpolate based on noiseValue
                        // Color sphereColor = Color.Lerp(StartColor, EndColor, noiseValue);

                        // Apply the color to the material
                        // sphereRenderer.material.color = sphereColor;

                        if (noiseValue > 0.35) sphereRenderer.material.color = EndColor;
                        else sphereRenderer.material.color = StartColor;
                    }
                }
            }
        }
    }
}
