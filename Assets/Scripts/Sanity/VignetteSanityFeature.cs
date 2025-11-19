// VignetteSanityFeature.cs (Updated for URP 14+ / Unity 2022.3+)

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteSanityFeature : ScriptableRendererFeature
{
    class VignetteSanityPass : ScriptableRenderPass
    {
        private Material vignetteMaterial;
        private RTHandle temporaryColorTexture;

        public Color VignetteColor = Color.red;
        public float Intensity = 0.5f;

        public VignetteSanityPass(Material material)
        {
            vignetteMaterial = material;
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderingUtils.ReAllocateIfNeeded(ref temporaryColorTexture,
                renderingData.cameraData.cameraTargetDescriptor,
                name: "VignetteTempTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (vignetteMaterial == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("Vignette Sanity Effect");

            vignetteMaterial.SetColor("_VignetteColor", VignetteColor);
            vignetteMaterial.SetFloat("_Intensity", Intensity);

            var cameraTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

            Blit(cmd, cameraTarget, temporaryColorTexture, vignetteMaterial);
            Blit(cmd, temporaryColorTexture, cameraTarget);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            temporaryColorTexture?.Release();
        }
    }

    [System.Serializable]
    public class VignetteSettings
    {
        public Material vignetteMaterial;
        public Color vignetteColor = new Color(1f, 0f, 0f, 0.5f);
        [Range(0f, 1f)] public float intensity = 0.7f;
        public RenderPassEvent renderEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public VignetteSettings settings = new VignetteSettings();
    private VignetteSanityPass vignettePass;
    public static VignetteSanityFeature Instance;

    public override void Create()
    {
        vignettePass = new VignetteSanityPass(settings.vignetteMaterial)
        {
            VignetteColor = settings.vignetteColor,
            Intensity = settings.intensity,
            renderPassEvent = settings.renderEvent
        };
        Instance = this;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.vignetteMaterial != null)
        {
            renderer.EnqueuePass(vignettePass);
        }
    }
}