using UnityEditor;
using UnityEngine;

namespace Mewlist
{
    [CustomEditor(typeof(MassiveCloudsCameraEffect))]
    public class MassiveCloudsCameraEffectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var logo = Resources.Load<Sprite>("MassiveCloudsLogoWide");
            using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(36f)))
            {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(new GUIContent(logo.texture),
                        new GUIStyle() {padding = new RectOffset(0,0,0,0)},
                        GUILayout.Height(28f), GUILayout.Width(235f));
                    GUILayout.FlexibleSpace();
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(new GUIContent("Camera Effect"),
                        new GUIStyle(GUI.skin.label) { normal = { textColor = new Color(0.5f, 0.7f, 0.8f)}, fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter },
                    GUILayout.Height(28f), GUILayout.Width(235f));
                    GUILayout.FlexibleSpace();
                }
                GUILayout.FlexibleSpace();
            }

            base.OnInspectorGUI();
        }
    }
}