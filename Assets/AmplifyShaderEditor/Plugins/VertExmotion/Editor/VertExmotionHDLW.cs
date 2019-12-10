using UnityEngine;
using System;
namespace AmplifyShaderEditor
{

    [Serializable]
    [NodeAttributes("VertExmotion (HD & LW RP)", "VertExmotion", "VertExmotion softbody\nHD & leightweight RP")]
    public class VertExmotionHDLWASENode : VertExmotionASEParentNode
    {
        protected override void CommonInit(int uniqueId)
        {
            base.CommonInit(uniqueId);
            AddInputPort(WirePortDataType.FLOAT3, false, "World Position", -1, MasterNodePortCategory.Vertex);
            AddInputPort(WirePortDataType.COLOR, false, "Vertex Color", -1, MasterNodePortCategory.Vertex);
            AddOutputPort(WirePortDataType.FLOAT3, "Vertex Offset");                        
        }

        public override string GenerateShaderForOutput(int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
        {            
            dataCollector.AddToDefines(UniqueId, "VERTEXMOTION_ASE_HD_LW_RP");
            UpdateVertExmotionIncludePath(ref dataCollector);

            string valueInput0 = m_inputPorts[0].GeneratePortInstructions(ref dataCollector);
            string valueInput1 = m_inputPorts[1].GeneratePortInstructions(ref dataCollector);
            string finalCalculation = string.Format("VertExmotionWorldPosASE({0},{1})", valueInput0, valueInput1);
            return finalCalculation;
        }
    }
}