using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator3D
{
    public float Frequency = 0.1f; // How "stretched" the noise is
    public float Amplitude = 0.9f; // Intensity of the noise
    public Vector3 Offset = Vector3.zero; // Offset for the noise, useful for scrolling
    private int seed;

    public NoiseGenerator3D(int seed) {
        this.seed = seed;
    }
    
    /// <summary>
    /// Generates 3D Perlin Noise for a given point.
    /// </summary>
    public float GeneratePerlin(float x, float y, float z)
    {
        // Use the seed to create an offset that's consistent across calls
        Vector3 seededOffset = Offset + new Vector3(seed, seed, seed);

        // Scale coordinates by Frequency and add the deterministic Offset
        float xy = Mathf.PerlinNoise((x * Frequency + seededOffset.x), (y * Frequency + seededOffset.y));
        float yz = Mathf.PerlinNoise((y * Frequency + seededOffset.y), (z * Frequency + seededOffset.z));
        float xz = Mathf.PerlinNoise((x * Frequency + seededOffset.x), (z * Frequency + seededOffset.z));

        // Combine the values to make it truly 3D
        float noise = (xy + yz + xz) / 3.0f;

        return noise * Amplitude;
    }

    /// <summary>
    /// Generates 3D Value Noise for a given point.
    /// </summary>
    public float GenerateValueNoise(float x, float y, float z)
    {
        int xi = Mathf.FloorToInt(x * Frequency + Offset.x);
        int yi = Mathf.FloorToInt(y * Frequency + Offset.y);
        int zi = Mathf.FloorToInt(z * Frequency + Offset.z);

        float xf = x * Frequency + Offset.x - xi;
        float yf = y * Frequency + Offset.y - yi;
        float zf = z * Frequency + Offset.z - zi;

        // Random hash function for each grid point
        float Random3D(int x, int y, int z) 
        {
            int h = x * 374761393 + y * 668265263 + z * 1870769191; // Some large prime numbers
            h = (h ^ (h >> 13)) * 1274126177;
            return (h & 0x7fffffff) / (float)0x7fffffff;
        }

        // Interpolation between grid points
        float v000 = Random3D(xi, yi, zi);
        float v100 = Random3D(xi + 1, yi, zi);
        float v010 = Random3D(xi, yi + 1, zi);
        float v110 = Random3D(xi + 1, yi + 1, zi);
        float v001 = Random3D(xi, yi, zi + 1);
        float v101 = Random3D(xi + 1, yi, zi + 1);
        float v011 = Random3D(xi, yi + 1, zi + 1);
        float v111 = Random3D(xi + 1, yi + 1, zi + 1);

        float u = Mathf.SmoothStep(0, 1, xf);
        float v = Mathf.SmoothStep(0, 1, yf);
        float w = Mathf.SmoothStep(0, 1, zf);

        float x00 = Mathf.Lerp(v000, v100, u);
        float x10 = Mathf.Lerp(v010, v110, u);
        float x01 = Mathf.Lerp(v001, v101, u);
        float x11 = Mathf.Lerp(v011, v111, u);

        float y0 = Mathf.Lerp(x00, x10, v);
        float y1 = Mathf.Lerp(x01, x11, v);

        return Mathf.Lerp(y0, y1, w) * Amplitude;
    }

    /// <summary>
    /// Generates 3D Worley Noise (approximation) for a given point.
    /// </summary>
    public float GenerateWorleyNoise(float x, float y, float z)
    {
        int numCells = Mathf.CeilToInt(Frequency);
        float minDist = float.MaxValue;

        for (int xi = -1; xi <= 1; xi++)
        {
            for (int yi = -1; yi <= 1; yi++)
            {
                for (int zi = -1; zi <= 1; zi++)
                {
                    Vector3 cell = new Vector3(
                        Mathf.Floor(x * Frequency + xi) + Random.value,
                        Mathf.Floor(y * Frequency + yi) + Random.value,
                        Mathf.Floor(z * Frequency + zi) + Random.value
                    );

                    float dist = Vector3.Distance(new Vector3(x, y, z), cell);
                    minDist = Mathf.Min(minDist, dist);
                }
            }
        }

        return minDist * Amplitude;
    }
}
