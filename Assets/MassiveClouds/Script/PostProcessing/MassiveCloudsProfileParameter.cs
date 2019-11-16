using System;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;

#endif

namespace Mewlist
{
#if UNITY_POST_PROCESSING_STACK_V2
    [Serializable]
    public sealed class MassiveCloudsProfileParameter : ParameterOverride<MassiveCloudsProfile>
    {
    }
#endif
}