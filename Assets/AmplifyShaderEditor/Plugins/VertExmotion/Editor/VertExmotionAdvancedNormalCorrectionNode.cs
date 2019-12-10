using UnityEngine;
using System;
namespace AmplifyShaderEditor
{

    [Serializable]
    [NodeAttributes("VertExmotion (Advanced NC)", "VertExmotion", "VertExmotion softbody\nwith normal correction")]
    public class VertExmotionAdvancedNormalCorrectionASENode : VertExmotionASEParentNode
    {
        protected override void CommonInit(int uniqueId)
        {
            base.CommonInit(uniqueId);
            AddInputPort(WirePortDataType.FLOAT3, false, "Vertex position", -1, MasterNodePortCategory.Vertex);
            AddInputPort(WirePortDataType.COLOR, false, "Vertex color", -1, MasterNodePortCategory.Vertex);
            AddInputPort(WirePortDataType.FLOAT3, false, "Vertex normal", -1, MasterNodePortCategory.Vertex);
            AddInputPort(WirePortDataType.FLOAT3, false, "Vertex tangent", -1, MasterNodePortCategory.Vertex);
            AddOutputPort(WirePortDataType.FLOAT3, "Vertex Offset");
            AddOutputPort(WirePortDataType.FLOAT3, "Vertex Normal");
        }

        public override string GenerateShaderForOutput(int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
        {
            UpdateVertExmotionIncludePath(ref dataCollector);

            string valueInput0 = m_inputPorts[0].GeneratePortInstructions(ref dataCollector);
            string valueInput1 = m_inputPorts[1].GeneratePortInstructions(ref dataCollector);
            string valueInput2 = m_inputPorts[2].GeneratePortInstructions(ref dataCollector);
            string valueInput3 = m_inputPorts[3].GeneratePortInstructions(ref dataCollector);

            string normalVar = "NewNormal_" + OutputId;
            RegisterLocalVariable(1, valueInput2, ref dataCollector, normalVar);

            string finalCalculation = string.Format("VertExmotionAdvancedASE({0},{1}," + normalVar + ",{2})", valueInput0, valueInput1, valueInput3);
            return finalCalculation;

        }
    }
}