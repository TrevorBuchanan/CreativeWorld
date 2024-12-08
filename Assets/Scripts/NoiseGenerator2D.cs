using UnityEngine;

public class NoiseGenerator2D
{
    public float Frequency = 1.0f; // Scale of the noise pattern
    public float Amplitude = 1.0f; // Intensity of the noise
    public Vector2 Offset = Vector2.zero; // Offset to shift the noise (useful for scrolling or positioning)

    /// <summary>
    /// Generates 2D Perlin Noise for a given point.
    /// </summary>
    public float GeneratePerlin(float x, float y)
    {
        // Generate Perlin Noise and scale it with Amplitude
        float noise = Mathf.PerlinNoise(x * Frequency + Offset.x, y * Frequency + Offset.y);
        return noise * Amplitude;
    }
}