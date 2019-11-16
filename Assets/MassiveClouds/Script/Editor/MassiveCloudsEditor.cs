using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Mewlist
{
    [CustomEditor(typeof(MassiveClouds))]
    public class MassiveCloudsEditor : AbstractMassiveCloudsEditor
    {
        ReorderableList reorderableList;

        void OnEnable ()
        {
            var profiles = serializedObject.FindProperty("profiles");
            reorderableList = new ReorderableList (serializedObject, profiles);
            reorderableList.drawHeaderCallback = (rect) =>
                EditorGUI.LabelField (rect, "Layers");
            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty property = profiles.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, property, new GUIContent(string.Format("Layer {0}", index + 1)));
            };
            reorderableList.onAddCallback += (list) => {

                //要素を追加
                profiles.arraySize++;

                //最後の要素を選択状態にする
                list.index = profiles.arraySize - 1;
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var massiveClouds = target as MassiveClouds;

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
                GUILayout.FlexibleSpace();
            }

            using (new EditorGUILayout.VerticalScope("Button"))
            {
                var resolutionProperty = serializedObject.FindProperty("resolution");
                EditorGUILayout.PropertyField(resolutionProperty);

                var volumetricShadowResolutionProperty = serializedObject.FindProperty("volumetricShadowResolution");
                EditorGUILayout.PropertyField(volumetricShadowResolutionProperty);

//                EditorGUILayout.PropertyField(serializedObject.FindProperty("lerp"), new GUIContent("Switch Lerping"));

                var durationProperty = serializedObject.FindProperty("duration");
                EditorGUILayout.PropertyField(durationProperty, new GUIContent("Switch Duration"));
            }

            GUILayout.Space(8f);

            using (new EditorGUILayout.VerticalScope("Button"))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudColorMode"),
                    new GUIContent("Cloud Color Type"));
                if (!serializedObject.FindProperty("cloudColorMode").boolValue)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudColor"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("shadowColorMode"),
                    new GUIContent("Shadow Color Type"));
                if (serializedObject.FindProperty("shadowColorMode").intValue == 0)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("shadowColor"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("fogColorMode"),
                    new GUIContent("Fog Color Type"));
                if (serializedObject.FindProperty("fogColorMode").intValue == 0)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("fogColor"));
            }
            GUILayout.Space(8f);
            using (new EditorGUILayout.VerticalScope("Button"))
            {
                var sunMode = (MassiveClouds.LightSourceMode)serializedObject.FindProperty("sunMode").enumValueIndex;
                var moonMode = (MassiveClouds.LightSourceMode)serializedObject.FindProperty("moonMode").enumValueIndex;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sunMode"),
                    new GUIContent("Sun Mode"));
                if (sunMode == MassiveClouds.LightSourceMode.Auto)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("sunReference"),
                        new GUIContent("Sun Source"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sunIntensityScale"),
                    new GUIContent("Sun Intensity Scale"));
                EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sun"),
                    new GUIContent("Sun"), true);
                EditorGUI.indentLevel -= 1;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("moonMode"),
                    new GUIContent("Moon Mode"));
                if (moonMode == MassiveClouds.LightSourceMode.Auto)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("moonReference"),
                        new GUIContent("Moon Source"));
                EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("moon"),
                    new GUIContent("Moon"), true);
                EditorGUI.indentLevel -= 1;
            }

            GUILayout.Space(8f);
            using (new EditorGUILayout.VerticalScope("Button"))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ambientMode"),
                    new GUIContent("Ambient Mode"));
                EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ambient"),
                    new GUIContent("Ambient"), true);
                EditorGUI.indentLevel -= 1;
            }

            GUILayout.Space(8f);
            reorderableList.DoLayoutList ();
            GUILayout.Space(16f);

            var parameters = serializedObject.FindProperty("parameters");
            for (var i=0; i<parameters.arraySize; i++)
            {
                if (i != reorderableList.index) continue;

                if (GUILayout.Button("↑ Save to Profile ↑"))
                    massiveClouds.SaveToProfile();
                var parameter = parameters.GetArrayElementAtIndex(i);
                ProfileGUI(parameter);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}