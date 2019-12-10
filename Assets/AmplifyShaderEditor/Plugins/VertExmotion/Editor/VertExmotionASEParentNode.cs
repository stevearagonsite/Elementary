using UnityEngine;
using UnityEditor;
using System;
namespace AmplifyShaderEditor
{

    [Serializable]
    public class VertExmotionASEParentNode : ParentNode
    {
        //public bool m_autoInclude = true;
        //public const string m_defaultIncludePath = "assets/VertExmotion/Shaders/VertExmotion.cginc";

        protected override void CommonInit(int uniqueId)
        {
            if (!UIUtils.HasColorCategory("VertExmotion"))
                UIUtils.AddColorCategory("VertExmotion", new Color(.945f, .624f, 0, 1));

            base.CommonInit(uniqueId);
        }

        /*
        public override void DrawProperties()
        {
            base.DrawProperties();
            m_autoInclude = EditorGUILayout.Toggle("autolink VertExmotion.cginc", m_autoInclude);
        }
        */

        /*
        public bool NeedVertExmotionInclude(ref MasterNodeDataCollector dataCollector)
        {

            if (!m_autoInclude)
                return false;

            for (int i = 0; i < dataCollector.IncludesList.Count; ++i)
                if (dataCollector.IncludesList[i].PropertyName.Contains("VertExmotion.cginc"))
                    return false;

            return true;
        }
        */
        public void UpdateVertExmotionIncludePath(ref MasterNodeDataCollector dataCollector)
        {
            /*
            if (NeedVertExmotionInclude(ref dataCollector))
                dataCollector.AddToIncludes(UniqueId, m_defaultIncludePath);
            */
        }
        
    }
}