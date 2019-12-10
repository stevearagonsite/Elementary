using UnityEngine;
using System;
namespace AmplifyShaderEditor
{

    [Serializable]
    [NodeAttributes("VertExmotion (Advanced)", "VertExmotion", "VertExmotion softbody\nwithout normal correction")]
    public class VertExmotionAdvancedASENode : VertExmotionASEParentNode
    {   

        protected override void CommonInit(int uniqueId)
        {
            base.CommonInit(uniqueId);
            AddInputPort(WirePortDataType.FLOAT3, false, "Vertex position", -1, MasterNodePortCategory.Vertex);
            AddInputPort(WirePortDataType.COLOR, false, "Vertex color", -1, MasterNodePortCategory.Vertex);
            AddOutputPort(WirePortDataType.FLOAT3, "Vertex Offset");

             
        }
    

        public override string GenerateShaderForOutput(int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
        {
            UpdateVertExmotionIncludePath(ref dataCollector);

            string valueInput0 = m_inputPorts[0].GeneratePortInstructions(ref dataCollector);
            string valueInput1 = m_inputPorts[1].GeneratePortInstructions(ref dataCollector);
            string finalCalculation = string.Format("VertExmotionAdvancedASE({0},{1})", valueInput0, valueInput1);
            return finalCalculation;
        }
    }
}