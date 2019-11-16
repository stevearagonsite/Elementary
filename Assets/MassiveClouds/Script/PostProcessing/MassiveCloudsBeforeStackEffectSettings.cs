using System;
using Mewlist;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif


#if UNITY_POST_PROCESSING_STACK_V2
[Serializable]
[PostProcess(typeof(MassiveCloudsBeforeStackEffectRenderer),
             PostProcessEvent.BeforeStack,
             "Mewlist/MassiveCloudsBeforeStack")]
public sealed class MassiveCloudsBeforeStackEffectSettings : AbstractMassiveCloudsEffectSettings
{
}
#endif