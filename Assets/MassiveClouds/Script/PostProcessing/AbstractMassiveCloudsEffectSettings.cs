using System;
using Mewlist;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif


#if UNITY_POST_PROCESSING_STACK_V2
[Serializable]
public class AbstractMassiveCloudsEffectSettings : PostProcessEffectSettings
{
    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        return enabled.value;
    }
}
#endif