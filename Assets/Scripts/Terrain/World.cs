using System;
using UnityEngine;
using CreativeWorld.Helpers;

namespace CreativeWorld.Terrain
{
    public class World : MonoBehaviour
    {
        [HideInInspector] public RenderTexture DensityMap;
        public Vector3 Scale => transform.localScale;

        [Header("Density Map Settings")]
        public int textureResolution = 256; // Resolution of the density map
        public ComputeShader densityComputeShader;

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

            kernelHandle = densityComputeShader.FindKernel("CSMain");

            // Debug to check if the kernel was found
            if (kernelHandle < 0)
            {
                Debug.LogError("Compute Shader Kernel 'CSMain' not found!");
            }
        }


        void UpdateDensityMap()
        {
            if (densityComputeShader == null || DensityMap == null)
            {
                Debug.LogError("Density Compute Shader or Density Map is not initialized.");
                return;
            }

            // Bind the density map to the compute shader
            densityComputeShader.SetTexture(kernelHandle, "Result", DensityMap);
            densityComputeShader.SetVector("WorldScale", Scale);

            // Dispatch the compute shader
            int threadGroups = Mathf.CeilToInt(textureResolution / 8.0f);
            densityComputeShader.Dispatch(kernelHandle, threadGroups, threadGroups, threadGroups);
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
