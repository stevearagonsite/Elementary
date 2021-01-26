 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Dialogue))]
public class DialogueEditor : Editor
{
    const int LABEL_WIDTH = 80;
    Dialogue _target;
    private void OnEnable()
    {
        _target = (Dialogue)target;
        if (_target.nodes == null) _target.nodes = new List<BaseNode>();
    }

    public override void OnInspectorGUI()
    {
        var myStyle = new GUIStyle();
        myStyle.fontStyle = FontStyle.BoldAndItalic;
        myStyle.alignment = TextAnchor.MiddleCenter;
        myStyle.fontSize = 20;
        myStyle.wordWrap = true;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Dialogues", myStyle);
        EditorGUILayout.Space();

        DrawDialogues();

        if (GUILayout.Button("Delete last node") && _target.nodes.Count > 0) {
            var index = _target.nodes.Count - 1;
            _target.nodes.RemoveAt(index);
        }

        if (!GUILayout.Button("Save")) return;
        EditorUtility.SetDirty(_target);
        AssetDatabase.SaveAssets();
    }

    private void DrawDialogues()
    {
        EditorGUILayout.BeginVertical();
        foreach (var node in _target.nodes)
        {
            DrawLine(Color.black);
            DrawDialogueBox(node);
        }
        EditorGUILayout.EndVertical();
        DrawLine(Color.black); 
        DrawLine(Color.black);

        if(GUILayout.Button("Add new Dialog"))
        {
            var newNode = new BaseNode(0, 0, 200, 150);
            _target.nodes.Last().nextNode = newNode;
            _target.nodes.Add(newNode);
        }
    }

    private void DrawLine(Color col)
    {
        EditorGUILayout.Space();
        EditorGUI.DrawRect(GUILayoutUtility.GetRect(100, 2), col);
        EditorGUILayout.Space();
    }

    private void DrawDialogueBox(BaseNode node)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Dialogue", EditorStyles.boldLabel, GUILayout.Width(LABEL_WIDTH));
        node.dialog = EditorGUILayout.TextField(node.dialog);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Duration", EditorStyles.boldLabel, GUILayout.Width(LABEL_WIDTH));
        node.duration = EditorGUILayout.FloatField(node.duration);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Skipable", EditorStyles.boldLabel, GUILayout.Width(LABEL_WIDTH));
        node.skipable = EditorGUILayout.Toggle(node.skipable);
        EditorGUILayout.EndHorizontal();

    }
}
