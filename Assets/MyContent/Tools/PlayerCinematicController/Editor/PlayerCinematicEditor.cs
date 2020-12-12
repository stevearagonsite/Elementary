using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

[CustomEditor(typeof(PlayerMoveOnCinematic))]
public class PlayerCinematicEditor : Editor
{
    private PlayerMoveOnCinematic _target;
    private Option _currentOption;
    private CinematicNode _selectedNode;
    private int _selectedNodeIndex;

    private void OnEnable()
    {
        _target = (PlayerMoveOnCinematic)target;
        _currentOption = Option.POSITION;
    }

    private void OnSceneGUI()
    {
        DrawPathHandles();
        DrawPathNodes();

        Handles.BeginGUI();
        DrawEditor();
        Handles.EndGUI();
    }

    private void DrawEditor()
    {
        GUILayout.BeginArea(new Rect(20, 20, 300, 200));
        var rec = EditorGUILayout.BeginVertical();
        GUI.color = new Color32(194, 194, 194, 255);

        GUI.Box(rec, GUIContent.none);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUIStyle myS = new GUIStyle();
        myS.fontSize = 18;
        GUILayout.Label("Nodes:", myS);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create"))
        {
            Vector3 position = _target.transform.position;
            float radius = 0.5f;
            if(_selectedNode != null)
            {
                position = _selectedNode.position;
                radius = _selectedNode.radius;
            }
            _selectedNode = _target.AddNewNode(position, radius);
        }
        if (GUILayout.Button("Delete"))
        {
            if(_selectedNode != null)
            {
                _target.DeleteNode(_selectedNode);
            }
        }
        if (GUILayout.Button("Scale"))
        {
            _currentOption = Option.SCALE;
        }
        if (GUILayout.Button("Move"))
        {
            _currentOption = Option.POSITION;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

       

        if (_selectedNode != null)
        {
            GUILayout.BeginHorizontal();

            EditorGUI.DrawRect(GUILayoutUtility.GetRect(300, 2), GUI.color);

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
               
            EditorGUI.DrawRect(GUILayoutUtility.GetRect(300, 2), Color.black);
            
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Node: " + _selectedNodeIndex);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Radius");
            _selectedNode.radius = EditorGUILayout.FloatField(_selectedNode.radius);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Position");
            _selectedNode.position = EditorGUILayout.Vector3Field("",_selectedNode.position);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void DrawPathNodes()
    {
        int i = 0;
        if(_target.nodes != null)
        {
            foreach (var n in _target.nodes)
            {
               
                if(_currentOption == Option.POSITION)
                {
                    var pos = n.position;
                    var aux = pos;
                    pos = Handles.PositionHandle(pos, Quaternion.identity);
                    n.position = pos;
                    if (aux != pos)
                    {
                        _selectedNode = n;
                        _selectedNodeIndex = i;
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                }else if(_currentOption == Option.SCALE)
                {
                    var r = n.radius;
                    var aux = r;
                    var rVector = new Vector3(r, r, r);
                    rVector = Handles.ScaleHandle(rVector, n.position, Quaternion.identity,HandleUtility.GetHandleSize(n.position));
                    n.radius = rVector.x;
                    if(aux != n.radius)
                    {
                        _selectedNode = n;
                        _selectedNodeIndex = i;
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                }
                
                if(_selectedNode == n)
                {
                    Handles.color = Color.red;
                }
                Handles.DrawWireDisc(n.position, Vector3.up, n.radius);
                Handles.DrawWireDisc(n.position, Vector3.right, n.radius);
                Handles.DrawWireDisc(n.position, Vector3.forward, n.radius);
                i++;
                Handles.color = Color.white;
            }

        }
    }

    private void DrawPathHandles()
    {
        if(_target.nodes != null && _target.nodes.Count > 0)
        {
            for (int i = 0; i < _target.nodes.Count - 1; i++)
            {
                var dir = Quaternion.LookRotation((_target.nodes[i + 1].position - _target.nodes[i].position).normalized);
                var distance = Vector3.Distance(_target.nodes[i + 1].position, _target.nodes[i].position);
                Handles.ArrowHandleCap(i, _target.nodes[i].position, dir, distance,EventType.Ignore);
                Handles.DrawLine(_target.nodes[i].position, _target.nodes[i + 1].position);
                Handles.Label(_target.nodes[i].position + Vector3.up, "Node: " + i);
            }
            Handles.Label(_target.nodes[_target.nodes.Count - 1].position + Vector3.up, "Node: " + (_target.nodes.Count - 1));
        }
    }

    enum Option
    {
        POSITION,
        SCALE
    }


}
