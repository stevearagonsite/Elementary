using UnityEngine;
using System;
namespace AmplifyShaderEditor
{

    [Serializable]
    [NodeAttributes("VertExmotion", "VertExmotion", "VertExmotion softbody\nwith normal correction")]
    public class VertExmotionASENode : VertExmotionASEParentNode
    {
        protected override void CommonInit(int uniqueId)
        {
            base.CommonInit(uniqueId);
            AddOutputPort(WirePortDataType.FLOAT3, "Vertex Offset");
        }

        public override string GenerateShaderForOutput(int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
        {
            UpdateVertExmotionIncludePath(ref dataCollector);

            string finalCalculation = string.Format("VertExmotionASE(v)");
            return finalCalculation;
        }

    }
}