using UnityEngine;

[ExecuteInEditMode]
public class RedScreenEffect : MonoBehaviour
{
    public Material redScreenMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (redScreenMaterial != null)
        {
            Graphics.Blit(src, dest, redScreenMaterial);
        }
        else
        {
            Graphics.Blit(src, dest); // If no material, just pass through
        }
    }
}
