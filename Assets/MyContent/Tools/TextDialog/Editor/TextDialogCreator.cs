using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class TextDialogCreator : EditorWindow
{
    private static TextDialogCreator _mySelf;

    private List<BaseNode> allNodes;
    private GUIStyle myStyle;
    private float toolbarHeight = 100;

    //para el paneo
    private bool _panningScreen;
    private Vector2 graphPan;
    private Vector2 _originalMousePosition;
    private Vector2 prevPan;
    private Rect roomGraph;

    //graph defaults
    float bottomBarheight = 50;
    int smallBorder = 25;
    int gridSeparation = 20;
    int gridBold = 5;
    private Color defaultColor;

    Color gridColor = new Color(0.2f, 0.2f, 0.2f, 1);
    Color editorColor = new Color(194f / 255f, 194f / 255f, 194f / 255f, 1);

    Vector2 _prevPan;

    public GUIStyle wrapTextFieldStyle;

    //Nodes variables
    private BaseNode _selectedNode;

    //variables
    string dialogName = "Dialog 1";

    [MenuItem("Tools/TextDialogCreator")]
    public static void OpenWindow()
    {
        
        _mySelf = GetWindow<TextDialogCreator>();
        _mySelf.allNodes = new List<BaseNode>();
        _mySelf.myStyle = new GUIStyle();
        _mySelf.myStyle.fontSize = 20;
        _mySelf.myStyle.alignment = TextAnchor.MiddleCenter;
        _mySelf.myStyle.fontStyle = FontStyle.BoldAndItalic;

        _mySelf.graphPan = new Vector2(_mySelf.smallBorder,_mySelf.smallBorder);
        _mySelf.roomGraph = new Rect(0, _mySelf.toolbarHeight, 1000000, 1000000);

        //creo un style para asignar a los textos de manera que usen wordwrap
        //le paso el style por defecto como parametro para mantener el mismo "look"
        _mySelf.wrapTextFieldStyle = new GUIStyle(EditorStyles.textField);
        _mySelf.wrapTextFieldStyle.wordWrap = true;
    }

    private void OnGUI()
    {
        defaultColor = GUI.color;
        

        roomGraph.x = graphPan.x;
        roomGraph.y = graphPan.y;
        EditorGUI.DrawRect(new Rect(smallBorder, smallBorder, position.width - smallBorder - smallBorder, position.height - bottomBarheight), Color.gray);


        GUI.BeginGroup(roomGraph);

        BeginWindows();

        DrawGrid();
        CheckMouseInput(Event.current);
        DrawNodes();


        EndWindows();

        GUI.EndGroup();

        DrawBorders();

        //Editor Button
        EditorGUILayout.BeginHorizontal(GUILayout.Height(position.height - bottomBarheight));
        EditorGUILayout.LabelField("        Dialog Creator", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUILayout.Height(bottomBarheight));
        var opts = new GUILayoutOption[] { GUILayout.Width(100), GUILayout.Height(40) };
        EditorGUILayout.LabelField("     Dialog Name", EditorStyles.boldLabel,opts);
        opts = new GUILayoutOption[] { GUILayout.Width(200), GUILayout.Height(40) };
        dialogName = EditorGUILayout.TextField(dialogName, opts);
        opts = new GUILayoutOption[] { GUILayout.Width(80), GUILayout.Height(40) };

        if (GUILayout.Button("Save",opts))
        {
            var dialogue = new Dialogue();
            dialogue.nodes = new List<BaseNode>();
            BaseNode initialNode = new BaseNode(0,0,0,0);
            foreach (var item in allNodes)
            {
                if (item.isInitialNode)
                {
                    initialNode = item;
                    break;
                }
                
            }
            dialogue.nodes.Add(initialNode);
            while (initialNode.nextNode != null)
            {
                initialNode = initialNode.nextNode;
                dialogue.nodes.Add(initialNode);
            }
            ScriptableObjectUtility.CreateAsset(dialogue,dialogName);
            var aux = allNodes;
            _mySelf.allNodes = new List<BaseNode>();
            foreach (var item in aux)
            {
                _mySelf.allNodes.Add(item);
            }
        }
        if (GUILayout.Button("Load", opts))
        {
            Dialogue dialogue = ScriptableObjectUtility.LoadAsset<Dialogue>(dialogName);
            allNodes = new List<BaseNode>();
            _mySelf.allNodes = new List<BaseNode>();
            foreach (var item in dialogue.nodes)
            {
                _mySelf.allNodes.Add(item);
            }
            for (int i = 0; i < dialogue.nodes.Count; i++)
            {
                _mySelf.allNodes.Add(dialogue.nodes[i]);
                if(i + 1 < dialogue.nodes.Count)
                    _mySelf.allNodes[i].nextNode = dialogue.nodes[i + 1];
            }
        }
        EditorGUILayout.EndHorizontal();

        if (_selectedNode != null)
            Repaint();

    }

    #region CONTEXT MENU
    private void ContextMenuOpen(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create New Dialog Box"), false, AddNode, mousePosition);
        menu.ShowAsContext();
    }

    private void ContextMenuNode(BaseNode overNode)
    {
        GenericMenu menu = new GenericMenu();
        if (_selectedNode == null)
        {
            menu.AddItem(new GUIContent("Make initial Box"), false, MakeInitialNode, overNode);
        }
        menu.AddItem(new GUIContent("Copy Dialog Box"), false, CopyNode, overNode);
        menu.AddItem(new GUIContent("Delete Dialog Box"), false, DeleteNode, overNode);
        if(_selectedNode == null)
        {
            menu.AddItem(new GUIContent("Connect Dialog Box"), false, ConnectNode, overNode);
            
        }

        menu.ShowAsContext();
    }
    #endregion

    void DrawNodes() 
    {
        var oriCol = GUI.backgroundColor;
        for (int i = 0; i < allNodes.Count; i++)
        {
            if (allNodes[i].nextNode != null)
            {
                DrawNodeCurve(
                    new Vector2(allNodes[i].myRect.position.x + allNodes[i].myRect.width,
                                allNodes[i].myRect.position.y + allNodes[i].myRect.height / 4f * 3f), 
                    new Vector2(allNodes[i].nextNode.myRect.position.x,
                                allNodes[i].nextNode.myRect.position.y + allNodes[i].nextNode.myRect.height / 4f)
                    );

            }
        }

        for (int i = 0; i < allNodes.Count; i++)
        {
            if (allNodes[i].isInitialNode)
            {
                GUI.backgroundColor = Color.yellow;
            }
            allNodes[i].myRect = GUI.Window(i, allNodes[i].myRect, DrawNode,"");
            GUI.backgroundColor = oriCol;
        }
    }

    private void AddNode(object p)
    {
        var pos = (Vector2)p;
        var newNode = new BaseNode(pos.x, pos.y, 200, 150);
        if(allNodes.Count == 0)
        {
            newNode.isInitialNode = true;
        }
        allNodes.Add(newNode);
        if (_selectedNode != null)
        {
            _selectedNode.nextNode = newNode;
            _selectedNode = null;
        }
        Repaint();
    }

    private void MakeInitialNode(object p)
    {
        var overNode = (BaseNode)p;
        if (overNode != null)
        {
            foreach (var n in allNodes)
            {
                n.isInitialNode = false;
            }
            overNode.isInitialNode = true;
        }
    }
    private void CopyNode(object p)
    {
        var overNode = (BaseNode)p;
        if (overNode != null)
        {
            var newNode = new BaseNode(0, 0, 200, 150);
            newNode.dialog = overNode.dialog;
            newNode.duration = overNode.duration;
            
            allNodes.Add(newNode);
            if(_selectedNode != null)
            {
                _selectedNode.nextNode = newNode;
                _selectedNode = null;
            }
        }
        Repaint();
    }

    private void ConnectNode(object p)
    {
        _selectedNode = (BaseNode)p;
    }

    private void DeleteNode(object p)
    {
        var overNode = (BaseNode)p;

        if(overNode != null)
        {
            if (allNodes.Contains(overNode))
            {
                allNodes.Remove(overNode);
            }
            foreach (var n in allNodes)
            {
                if(n.nextNode == overNode)
                {
                    n.nextNode = null;
                }
            }
        }
    }

    private void DrawNode(int id)
    {
        //Draw Node
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Dialogue", GUILayout.Width(60));

        allNodes[id].dialog = EditorGUILayout.TextField(allNodes[id].dialog, wrapTextFieldStyle, GUILayout.Height(50));
        EditorGUILayout.EndHorizontal();
        allNodes[id].duration = EditorGUILayout.FloatField("Duration", allNodes[id].duration);
        allNodes[id].skipable = EditorGUILayout.Toggle("Skipable", allNodes[id].skipable);

        EditorGUI.DrawRect(new Rect(2, 2, 196, 18), gridColor);

        if (!_panningScreen)
        {
           
            GUI.DragWindow(new Rect(0,0,200,20));

            if (!allNodes[id].OverNode) return;

            
            if (allNodes[id].myRect.x < 0)
                allNodes[id].myRect.x = 0;

            if (allNodes[id].myRect.y < smallBorder - graphPan.y)
                allNodes[id].myRect.y = smallBorder - graphPan.y;
        }
    }

    void DrawBorders()
    {
        //Editor Borders
        EditorGUI.DrawRect(new Rect(0, 0, smallBorder, position.height), editorColor);
        EditorGUI.DrawRect(new Rect(0, position.height - bottomBarheight, position.width, bottomBarheight), editorColor);
        EditorGUI.DrawRect(new Rect(0, 0, position.width, smallBorder), editorColor);
        EditorGUI.DrawRect(new Rect(position.width - smallBorder, 0, smallBorder, position.height), editorColor);
    }

    void DrawGrid()
    {
        //Horizontal Lines
        for (int i = 0; i * gridSeparation + graphPan.y <= position.height - bottomBarheight; i++)
        {
            var b = i % gridBold == 0 ? 2 : 1;
            var width = roomGraph.width;
            EditorGUI.DrawRect(new Rect(0, i * gridSeparation, width, b), gridColor);
        }

        //Vertical Lines
        for (int i = 0; i * gridSeparation + graphPan.x <= position.width - smallBorder; i++)
        {
            var b = i % 5 == 0 ? 2 : 1;
            var height = roomGraph.height;
            EditorGUI.DrawRect(new Rect(i * gridSeparation, 0, b, height), gridColor);
        }
    }
    void DrawNodeCurve(Vector2 start, Vector2 end )
    {
        Vector2 startTan = start + Vector2.right * 50;
        Vector2 endTan = end + Vector2.left * 50;
        Color shadowCol = new Color(255, 255, 255);

        Handles.DrawBezier(start, end, startTan, endTan, Color.white, null, 5);
    }

    private void CheckMouseInput(Event current)
    {
        if (!roomGraph.Contains(current.mousePosition) || !(focusedWindow == this || mouseOverWindow == this))
            return;

        if (current.button == 2 && current.type == EventType.MouseDown)
        {
            _panningScreen = true;
            _prevPan = new Vector2(graphPan.x, graphPan.y);
            _originalMousePosition = current.mousePosition;
        }
        else if (current.button == 2 && current.type == EventType.MouseUp)
        {
            _panningScreen = false;
        }

        if (_panningScreen)
        {
            graphPan.x = Mathf.Min(smallBorder, _prevPan.x + current.mousePosition.x - _originalMousePosition.x);
            graphPan.y = Mathf.Min(smallBorder, _prevPan.y + current.mousePosition.y - _originalMousePosition.y);
            Repaint();
        }

        if (current.button == 0 && (current.type == EventType.MouseDown || current.type == EventType.MouseDrag))
        {
            
        }
        else if (current.button == 1 && (current.type == EventType.MouseDown))
        {
            ContextMenuOpen(current.mousePosition);
        }
        else if (current.button == 1 && (current.type == EventType.MouseDown || current.type == EventType.MouseDrag))
        {
            
        }
        else if (current.button == 1 && current.type == EventType.MouseUp)
        {
            
        }

        if(_selectedNode != null)
        {
            DrawNodeCurve(new Vector2(_selectedNode.myRect.position.x + _selectedNode.myRect.width, _selectedNode.myRect.position.y + _selectedNode.myRect.height / 4f * 3f),
                         current.mousePosition);
        }

        //Node context menu
        BaseNode overNode = null;
        for (int i = 0; i < allNodes.Count; i++)
        {
            allNodes[i].CheckMouse(Event.current, graphPan);
            if (allNodes[i].OverNode)
                overNode = allNodes[i];
        }

        if (overNode != null) 
        {
            if (current.button == 1 && current.type == EventType.MouseDown)
            {
                ContextMenuNode(overNode);
            }
            if (current.button == 0 && current.type == EventType.MouseDown)
            {
                if(_selectedNode != null)
                {
                    _selectedNode.nextNode = overNode;
                    _selectedNode = null;
                }
            }
        }
        
    }

}
