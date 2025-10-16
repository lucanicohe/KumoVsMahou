using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SafePixelationFeature : ScriptableRendererFeature
{
    class PixelationPass : ScriptableRenderPass
    {
        private Material pixelMaterial;
        private int tempTextureID;

        public PixelationPass(Material material)
        {
            pixelMaterial = material;
            tempTextureID = Shader.PropertyToID("_TempPixelationTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (pixelMaterial == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("PixelationPass");

            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            // Get temp RT
            cmd.GetTemporaryRT(tempTextureID, desc, FilterMode.Point);

            // Get camera target and convert RTHandle to RenderTargetIdentifier
            var sourceRTHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
            RenderTargetIdentifier sourceIdentifier = new RenderTargetIdentifier(sourceRTHandle.nameID, 0, CubemapFace.Unknown, -1);

            // Blit from camera to temp with pixelation
            cmd.Blit(sourceIdentifier, tempTextureID, pixelMaterial);
            
            // Blit from temp back to camera
            cmd.Blit(tempTextureID, sourceIdentifier);

            // Release temp
            cmd.ReleaseTemporaryRT(tempTextureID);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public Material pixelMaterial;
    private PixelationPass pixelPass;

    public override void Create()
    {
        pixelPass = new PixelationPass(pixelMaterial);
        pixelPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (pixelMaterial == null) return;

        renderer.EnqueuePass(pixelPass);
    }
}