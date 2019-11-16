using UnityEditor;

namespace Mewlist
{
    [CustomEditor(typeof(MassiveCloudsProfile))]
    public class MassiveCloudsProfileEditor : AbstractMassiveCloudsEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var parameter = serializedObject.FindProperty("Parameter");

            ProfileGUI(parameter);
        }
    }
}