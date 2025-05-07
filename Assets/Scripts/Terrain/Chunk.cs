using System;
using UnityEngine;

namespace CreativeWorld.Terrain
{
    public class Chunk : MonoBehaviour
    {
        [HideInInspector] public RenderTexture DensityMap;
        public Vector3 Scale => transform.localScale;

        [Header("Chunk Gen Settings")]
        public int textureResolution = 256; // Resolution of the chunk map
        public ComputeShader chunkGenComputeShader;

        private int kernelHandle;

        void Start()
        {
            InitializeDensityMap();
            UpdateDensityMap();
        }

        void InitializeDensityMap()
        {
            if (DensityMap == null)
            {
                DensityMap = new RenderTexture(textureResolution, textureResolution, 0, RenderTextureFormat.RFloat)
                {
                    enableRandomWrite = true,
                    volumeDepth = textureResolution,
                    dimension = UnityEngine.Rendering.TextureDimension.Tex3D
                };
                DensityMap.Create();
            }

            kernelHandle = chunkGenComputeShader.FindKernel("ChunkGen");

            // Debug to check if the kernel was found
            if (kernelHandle < 0)
            {
                Debug.LogError("Compute Shader Kernel 'ChunkGen' not found!");
            }
        }


        void UpdateDensityMap()
        {
            if (chunkGenComputeShader == null || DensityMap == null)
            {
                Debug.LogError("Chunk Gen Compute Shader or Density Map is not initialized.");
                return;
            }

            // Bind the density map to the compute shader
            chunkGenComputeShader.SetTexture(kernelHandle, "Result", DensityMap);
            chunkGenComputeShader.SetVector("ChunkScale", Scale);

            // Dispatch the compute shader
            int threadGroups = Mathf.CeilToInt(textureResolution / 8.0f);
            chunkGenComputeShader.Dispatch(kernelHandle, threadGroups, threadGroups, threadGroups);
        }

        private void OnDestroy()
        {
            if (DensityMap != null)
            {
                DensityMap.Release();
            }
        }
    }
}
