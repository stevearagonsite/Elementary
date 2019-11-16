using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif
using UnityEngine.Rendering;


#if UNITY_POST_PROCESSING_STACK_V2
namespace Mewlist
{
    public class AbstractMassiveCloudsEffectRenderer<T> : PostProcessEffectRenderer<T>
        where T : AbstractMassiveCloudsEffectSettings
    {
        private MassiveClouds currentMassiveClouds;

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth;
        }

        public override void Render(PostProcessRenderContext context)
        {
            if (currentMassiveClouds == null)
            {
                var massiveClouds = context.camera.GetComponent<MassiveClouds>();
                if (massiveClouds == null)
                {
                    var mainCamera = GameObject.FindWithTag("MainCamera");
                    massiveClouds = mainCamera.GetComponent<MassiveClouds>();
                }

                currentMassiveClouds = massiveClouds;
            }

            CommandBuffer cmd = context.command;
            if (currentMassiveClouds == null || !currentMassiveClouds.enabled)
            {
                cmd.Blit(context.source, context.destination);
                return;
            }
            currentMassiveClouds.BuildCommandBuffer(cmd, context.source, context.destination);
        }
    }
}
#endif