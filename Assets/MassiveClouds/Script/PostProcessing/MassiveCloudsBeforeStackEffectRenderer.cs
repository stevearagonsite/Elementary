using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif
using UnityEngine.Rendering;


#if UNITY_POST_PROCESSING_STACK_V2
namespace Mewlist
{
    public sealed class MassiveCloudsBeforeStackEffectRenderer : AbstractMassiveCloudsEffectRenderer<MassiveCloudsBeforeStackEffectSettings>
    {
    }
}
#endif