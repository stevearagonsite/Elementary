//#define KVTM_DEBUG


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Kalagaan
{ 
    //[CanEditMultipleObjects]
    [CustomEditor(typeof(VertExmotionBase),true)]
    public class VertExmotionEditor : Editor {


	    public enum eMode
	    {
		    INFO,
		    PAINT,
		    SENSORS
	    }

        public enum eRendererType
        {
            NONE,
            MESH,
            SKINNEDMESH,
            SPRITE
        }



        Vector3[] m_vtx;
	    Vector3[] m_nrml;


	    public eMode m_mode = eMode.INFO;
        public eRendererType m_rendererType = eRendererType.NONE;

        float m_brushSize;
	    float m_drawRadius = .05f;
	    float m_drawIntensity = 1f;
	    float m_drawFalloff = 1f;

	    //Vector3 m_camLockPosition;
	    //Quaternion m_camLockRotation;

	    public static GUIStyle m_styleBold;
	    public static GUIStyle m_styleTitle;
	    public static GUIStyle m_styleHighLight;
	    public static float m_dt = 0;
	    //Material m_paintMaterial;

	    bool m_eraseMode = false;
	    bool m_modeTrigger = false;
	    bool m_drawBrushSettings = false;
	    bool m_enableBrushMenuContextual = false;
	    Rect m_brushMenuRect;




	    //bool m_shiftDown = false;
	    bool m_ctrlDown = false;
	    bool m_initialized = false;
	    public static bool m_editorInitialized = false;

        public bool m_editorInstanceInitialized = false;

	    int m_sensorId = -1;
	    int m_sensorToDelete = -1;
	    int m_sensorToRemove = -1;

	    Tool m_lastTool = Tool.None;
	    bool m_lastOrthoMode = false;

	    public static string m_exportMeshName = "";
        public static bool m_exportFlag = false;

	    bool m_useUV1 = true;


	    public static VertExmotionBase m_lastVTMSelected;
        //public static eMode m_lastMode = eMode.INFO;

	    VertExmotionSensorBase m_externalSensor;

        MaterialPropertyBlock m_matPropBlk;
        Vector4[] m_sensorpos = new Vector4[50];
        Vector4[] m_RadiusCentripetalTorque = new Vector4[50];
        float[] m_power = new float[50];
        float[] m_shaderSensorLinksEditor = new float[50];

        //bool m_distributionClassFound = false; 
        int m_currentLayer = 0;
        //MeshRenderer m_rendererEditor;
        //bool m_displayScaleFixButton = false;
        bool m_displayAdvancedSettings = false;


        public override void OnInspectorGUI () 
	    {
            

                if (!m_editorInitialized)
			        InitializeEditorParameters ();

			    if( m_exportFlag )
			    {
				    ExportMesh();
				    m_exportFlag = false;
			    }

		    //GUI.backgroundColor = grey;
		    //GUI.contentColor = Color.cyan;
		    GUI.color = Color.white;


		    //GUI.skin = (GUISkin)Resources.Load("VertExmotionSkin",typeof(GUISkin));
		    /*
		     * //show all style in editor
		    int id = 0;
		    foreach(GUIStyle style in GUI.skin.customStyles)
		    {
			    if( style.name.Contains("ack") )
				    GUILayout.Button("" + id + " " + style.name,style);
			    id++;
		    }*/


		    VertExmotionBase vtm = (target) as VertExmotionBase;

            m_showPanel = vtm.m_showEditorPanel;

            //clean sensor list
            while ( vtm.m_VertExmotionSensors.Contains(null) )
			    vtm.m_VertExmotionSensors.Remove( null );
		
		    GUILayout.BeginHorizontal ();
		    GUILayout.Space(30);
		    DrawLogo ();
		    GUILayout.Label ( "v"+VertExmotionBase.version );
		    GUILayout.EndHorizontal ();






#if VERTEXMOTION_TRIAL
                EditorGUILayout.HelpBox("TRIAL VERSION\nthis version won't work in standalone application.\nSome functionnalities are limited.", MessageType.Warning);
#endif
            if (!vtm.m_editMode)
                GUILayout.Space(-25);

            GUILayout.BeginHorizontal();           
           
            if (vtm.m_editMode)
            {
				if (GUILayout.Button (m_showPanel ? "Panel in inspector" : "Panel in scene view")) 
				{
					vtm.m_showEditorPanel = !vtm.m_showEditorPanel;
                    //SceneView.RepaintAll();
				}

               

            }
            GUILayout.FlexibleSpace();            
            if (GUILayout.Button(vtm.m_editMode ? "Close Edition" : "Edit"))
                vtm.m_editMode = !vtm.m_editMode;
            GUILayout.EndHorizontal();

            if (vtm.m_editMode)
            {
                if (!m_showPanel)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    DrawUI(vtm);
                    GUILayout.EndVertical();
                }
                else
                    SceneView.RepaintAll();
            }

            GUILayout.Space(20);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Normal correction", EditorStyles.boldLabel);
            vtm.m_normalCorrection = EditorGUILayout.Slider("Correction",vtm.m_normalCorrection, 0f, 1f);
            vtm.m_normalSmooth = EditorGUILayout.Slider("Smooth", vtm.m_normalSmooth, 0f, 1f);
            GUILayout.EndVertical();

            if (vtm.m_editMode)
            {
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Advanced settings", EditorStyles.miniButtonMid))
                    m_displayAdvancedSettings = !m_displayAdvancedSettings;
                GUILayout.EndHorizontal();
            }
            if (m_displayAdvancedSettings)
            {

                // display a fix button for old versions of vertexmotion with the scale bug
 
                GUILayout.Space(20);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Fix old VertExmotion version", EditorStyles.boldLabel);
                GUILayout.Label("Use the fix button if the radius of the sensors have change with the update");

                GUILayout.BeginHorizontal();
                if ( GUILayout.Button("Fix") )
                {
                    for(int i=0; i<vtm.m_VertExmotionSensors.Count; ++i )
                    {
                        if (vtm.m_VertExmotionSensors[i].m_oldScaleFactorFixed)
                            continue;

                        vtm.m_VertExmotionSensors[i].m_oldScaleFactorFixed = true;

                        float f = vtm.m_VertExmotionSensors[i].transform.lossyScale.magnitude / (vtm.m_VertExmotionSensors[i].transform.lossyScale.x);
                        vtm.m_VertExmotionSensors[i].m_envelopRadius *= f;

                        VertExmotionColliderBase[] colliders = vtm.m_VertExmotionSensors[i].GetComponentsInChildren<VertExmotionColliderBase>();

                        for (int j = 0; j < colliders.Length; ++j)                            
                        {
                            for (int k = 0; k < colliders[j].m_collisionZones.Count; ++k)
                                colliders[j].m_collisionZones[k].radius *= f;
                        }
                    }
                }

                if (GUILayout.Button("Revert"))
                {
                    for (int i = 0; i < vtm.m_VertExmotionSensors.Count; ++i)
                    {
                        if (!vtm.m_VertExmotionSensors[i].m_oldScaleFactorFixed)
                            continue;

                        vtm.m_VertExmotionSensors[i].m_oldScaleFactorFixed = false;

                        float f = (vtm.m_VertExmotionSensors[i].transform.lossyScale.x) / vtm.m_VertExmotionSensors[i].transform.lossyScale.magnitude;
                        vtm.m_VertExmotionSensors[i].m_envelopRadius *= f;

                        VertExmotionColliderBase[] colliders = vtm.m_VertExmotionSensors[i].GetComponentsInChildren<VertExmotionColliderBase>();

                        for (int j = 0; j < colliders.Length; ++j)
                        {
                            for (int k = 0; k < colliders[j].m_collisionZones.Count; ++k)
                                colliders[j].m_collisionZones[k].radius *= f;
                        }
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }



#if KVTM_DEBUG
            GUILayout.Label("--------------\nDEBUG\n--------------");
            DrawDefaultInspector();
#endif

            if ( Application.isPlaying )
			    GUI.enabled = false;
		

		
    //		if( m_distributionClassFound )
    //			{
    //				if (GUILayout.Button ("Pack"))
    //					VertExmotionPacker.Pack ( vtm );
    //			}

			    //Repaint ();
		
	    }



            public static void InitializeEditorParameters()
            {
            m_editorInitialized = true;

                 //if( m_styleBold == null  )
            {


                    m_styleBold = new GUIStyle();
                    if (EditorGUIUtility.isProSkin)
                        m_styleBold.normal.textColor = Color.white;
                    m_styleBold.fontStyle = FontStyle.Bold;

                    m_styleTitle = new GUIStyle();
                    m_styleTitle.normal.textColor = orange;
                    m_styleTitle.normal.background = new Texture2D(1, 1);
                    m_styleTitle.normal.background.SetPixel(0, 0, grey);
                    m_styleTitle.normal.background.Apply();
                    m_styleTitle.fontStyle = FontStyle.Bold;

                    m_styleHighLight = new GUIStyle();
                    m_styleHighLight.normal.textColor = Color.white;
                    m_styleHighLight.fontStyle = FontStyle.Bold;
                    m_styleHighLight.normal.background = new Texture2D(1, 1);
                    m_styleHighLight.normal.background.SetPixel(0, 0, orange);
                    m_styleHighLight.normal.background.Apply();
                    m_styleHighLight.alignment = TextAnchor.MiddleCenter;

                    m_bgStyle.normal.background = new Texture2D(1, 1);
                    m_bgStyle.normal.background.SetPixel(0, 0, grey);
                    m_bgStyle.normal.background.Apply();


                }
            }

            public void InitializeEditorInstance()
            {
                //Debug.Log("InitializeEditorInstance");           

                VertExmotionBase vtm = (target) as VertExmotionBase;

                if (vtm.GetComponent<MeshRenderer>())
                    m_rendererType = eRendererType.MESH;            

                if ( vtm.GetComponent<SkinnedMeshRenderer>() )            
                    m_rendererType = eRendererType.SKINNEDMESH;

                if (vtm.GetComponent<SpriteRenderer>())
                    m_rendererType = eRendererType.SPRITE;

                if (vtm.GetComponent<TextMesh>())
                    m_rendererType = eRendererType.SPRITE;

                m_editorInstanceInitialized = true;
                //Debug.Log("" + m_rendererType);

                m_matPropBlk = new MaterialPropertyBlock();
                vtm.renderer.GetPropertyBlock(m_matPropBlk);
                vtm.CleanShaderProperties();
            
                if (EditorPrefs.HasKey("VertExmotion_LastMode"))
                {
                    //Debug.Log((eMode)EditorPrefs.GetInt("VertExmotion_LastMode"));
                    m_mode = (eMode)EditorPrefs.GetInt("VertExmotion_LastMode");
                    EditorPrefs.DeleteKey("VertExmotion_LastMode");
                }

                //m_lastMode = eMode.INFO;
            }





	    //GUISkin m_skin;
	    void OnSceneGUI()
	    {

            
    //			if (Application.isPlaying)
    //								return;
    /*
                //TEST----------------
                    MeshFilter mf = (target as VertExmotionBase).gameObject.GetComponent<MeshFilter>();
                    Debug.Log("Vertex colors count :" + mf.sharedMesh.colors32.Length);
                    //TEST----------------
    */
		    GUI.backgroundColor = grey;

            //if (Application.isPlaying)
            //	return;


            if (!m_editorInitialized)
				    InitializeEditorParameters ();


            if (!m_editorInstanceInitialized)
                   InitializeEditorInstance();

		    Event e = Event.current;

            VertExmotionBase vtm = (target) as VertExmotionBase;



            if ( !m_initialized && !Application.isPlaying && m_rendererType!= eRendererType.SPRITE )
		    {
			    vtm.InitVertices ();
			    //vtm.InitMesh();
			    InitVerticesPosDictionary ();
			    m_initialized = true;

			    if( m_vtx == null )
				    m_vtx = vtm.m_mesh.vertices.Clone () as Vector3[];
			    if( m_nrml == null )
				    m_nrml = vtm.m_mesh.normals.Clone () as Vector3[];
		    }

		    Camera svCam = SceneView.currentDrawingSceneView.camera;


		
    /*
		    if( m_initialShader == null )
			    m_initialShader = vtm.renderer.sharedMaterial.shader;
		    if( m_initialMaterial == null )
			    m_initialMaterial = vtm.renderer.sharedMaterial;*/

		    Vector2 mp2d = Event.current.mousePosition * EditorGUIUtility.pixelsPerPoint;
		    mp2d.y = svCam.pixelHeight - mp2d.y;		
		    Vector3 mp = svCam.ScreenToWorldPoint ( mp2d );
		    mp +=  svCam.transform.forward; 

		    //Debug.Log ("mouse pos "+ mp);





		    //-----------------------------------------------------------------------
		    //draw motion sensor position
		    bool useHandle = false;
            if (m_mode == eMode.SENSORS && m_sensorId != -1 && m_sensorId < vtm.m_VertExmotionSensors.Count)
            {
                useHandle = VertExmotionSensorEditor.DrawSensorHandle(vtm.m_VertExmotionSensors[m_sensorId], true);

                //link
                // 0 -> unlinked
                // 1 -> master link
                // -1 -> slave link
                // 2-> master & slave

                //check next link -> draw slave
                for (int id = 1; id < vtm.m_VertExmotionSensors.Count - m_sensorId; ++id )
                {
                    if (vtm.m_sensorsLinks[m_sensorId + id -1] == 1 && (m_sensorId + id < vtm.m_VertExmotionSensors.Count))
                        DrawDensorLink(vtm, m_sensorId + id-1, m_sensorId + id);
                    else
                        break;
                }

                //check previous link -> draw master
                for (int id = 1; id <= m_sensorId; ++id)
                {
                    if ((m_sensorId - 1 >= 0) && vtm.m_sensorsLinks[m_sensorId - id] >= 1)
                        DrawDensorLink(vtm, m_sensorId -id +1, m_sensorId - id);
                    else
                        break;
                }

            }


            if( useHandle )
                //Repaint();
                SceneView.currentDrawingSceneView.Repaint();

            //-----------------------------------------------------------------------
            //Paint Vertices
            if ( !useHandle && m_mode == eMode.PAINT && !Application.isPlaying )
                PaintVertice( vtm, svCam, mp, mp2d );
                




            //-----------------------------------------------------------------------


            if (e.control )
		    {
			    //toggle paint mode with shift
			    if( !m_ctrlDown )
				    m_eraseMode = !m_eraseMode;
			    m_ctrlDown = true;
		    }
		    else
		    {
			    m_ctrlDown = false;
		    }




            



		    //switch render mode & shader
		    if ( (m_mode == eMode.PAINT || m_mode == eMode.SENSORS) && !Application.isPlaying )
		    {

			    Selection.activeGameObject = vtm.gameObject;

			    //disable drag selection
			    HandleUtility.AddDefaultControl( GUIUtility.GetControlID( GetHashCode(), FocusType.Passive ) );

			    if( !m_modeTrigger )
			    {
                        for (int i = 0; i < vtm.renderer.sharedMaterials.Length; ++i)
                        {
                            if (vtm.renderer.sharedMaterials[i] == null)
                                continue;

                        //--------------------------------------------
                        vtm.renderer.sharedMaterials[i].shader = Shader.Find("Hidden/VertExmotion_editor");
                        //--------------------------------------------
                    }
                    //m_lastTool = Tools.current;
                    m_lastOrthoMode = SceneView.lastActiveSceneView.orthographic;
			    }

			    //SceneView.currentDrawingSceneView.camera.isOrthoGraphic = true;
			    SceneView.lastActiveSceneView.orthographic = true;
			    Tools.current = Tool.None;
			    m_modeTrigger = true;
			    //vtm.Update();
			    //Event.current.Use();
		    }
		    else
		    {
			    if( m_modeTrigger )
			    {
                        for (int i = 0; i < vtm.renderer.sharedMaterials.Length; ++i)
                        {
                            if (vtm.renderer.sharedMaterials[i] == null)
                                continue;
                            vtm.renderer.sharedMaterials[i].shader = vtm.m_initialShaders[i];
                        }
				    Tools.current = m_lastTool;
				    SceneView.lastActiveSceneView.orthographic = m_lastOrthoMode;
			    }
			    m_modeTrigger = false;
		    }


            

            DrawMenu ( vtm, e );

            

            if (e.alt)
		    {
			    if( !m_drawBrushSettings )
				    m_brushMenuRect = new Rect( Event.current.mousePosition.x*EditorGUIUtility.pixelsPerPoint, Event.current.mousePosition.y*EditorGUIUtility.pixelsPerPoint, 150f, 200f  );
			    DrawBrushMenuContextual (e);
			    m_drawBrushSettings = true;
		    }
		    else
		    {
			    m_drawBrushSettings = false;
		    }


		    if (m_mode == eMode.PAINT )
			    DrawCursor( mp, svCam );

            SceneView.currentDrawingSceneView.Repaint ();
	    }


        void DrawGradientToggle()
        {
            bool showGradient = Shader.IsKeywordEnabled("VERTEXMOTION_GRADIENT_ON");

            if (EditorGUILayout.ToggleLeft("Show gradient", showGradient) != showGradient)
            {
                if (showGradient)
                {
                    Shader.EnableKeyword("VERTEXMOTION_GRADIENT_OFF");
                    Shader.DisableKeyword("VERTEXMOTION_GRADIENT_ON");
                }
                else
                {
                    Shader.EnableKeyword("VERTEXMOTION_GRADIENT_ON");
                    Shader.DisableKeyword("VERTEXMOTION_GRADIENT_OFF");
                }
            }
        }




        void DrawDensorLink( VertExmotionBase vtm, int currentId, int linkedId )
        {
            VertExmotionSensorEditor.DrawSensorHandle(vtm.m_VertExmotionSensors[linkedId], false);

            Color c = VertExmotionEditor.orange;
            c.a = .1f;
            Handles.color = c;
            Handles.DrawDottedLine(vtm.m_VertExmotionSensors[currentId].transform.position, vtm.m_VertExmotionSensors[linkedId].transform.position, 1f);
            Vector3 delta = vtm.m_VertExmotionSensors[linkedId].transform.position - vtm.m_VertExmotionSensors[currentId].transform.position;

            int maxCircles = 30;
            for (int i = 0; i <= maxCircles; ++i)
            {
                float lrp = (float)i / (float)maxCircles;
                Vector3 pos = Vector3.Lerp(vtm.m_VertExmotionSensors[currentId].transform.position, vtm.m_VertExmotionSensors[linkedId].transform.position, lrp);
                float radius = Mathf.Lerp(vtm.m_VertExmotionSensors[currentId].m_envelopRadius, vtm.m_VertExmotionSensors[linkedId].m_envelopRadius, lrp);
                Handles.DrawWireDisc(pos, delta.normalized, radius);
            }
        }




	    public static bool m_showPanel = true;
	    public static float m_showPanelProgress = 0f;
	    static Rect m_menuRect;
	    static PID m_panelPID = new PID( 5f, .5f, 0f );
	    static System.DateTime m_lastTime = System.DateTime.Now;

	    static public void UpdateShowPanel()
	    {
		    //m_showPanelProgress += m_showPanel?.05f:-.05f;
		    m_panelPID.m_target = m_showPanel ? 1f : 0f;
		    m_panelPID.m_params.limits.x = 0;
		    m_panelPID.m_params.limits.y = 1;
		    System.TimeSpan dt = System.DateTime.Now - m_lastTime;
		    m_lastTime = System.DateTime.Now;
		    m_dt = (float)dt.TotalSeconds;
			    m_showPanelProgress = m_panelPID.Compute (m_showPanelProgress, m_dt);
		    //m_showPanelProgress = Mathf.Clamp01( m_showPanelProgress );

		    m_menuRect = new Rect(-(1f-m_showPanelProgress)*m_menuWidth, 0, m_menuWidth, Screen.height);
		    //Debug.Log ( "ShowPanel " + m_showPanelProgress );
	    }



        //-----------------------------------------------------
        //Paint function
        //-----------------------------------------------------
        //int[] m_selectedMaterialTriangles = null;
        int m_previousSelectedMaterialId = -1;
        MeshCollider m_meshCollider = null;
        int[] m_triangles = null;
        void PaintVertice( VertExmotionBase vtm, Camera svCam, Vector3 mp, Vector2 mp2d )
	    {
            if (m_rendererType == eRendererType.SPRITE )
                return;



            if (m_selectedMaterialSlotID != m_previousSelectedMaterialId)
            {
                //m_selectedMaterialTriangles = vtm.m_mesh.GetTriangles(m_selectedMaterialSlotID - 1);
                m_previousSelectedMaterialId = m_selectedMaterialSlotID;
                m_meshCollider = null;//initialize
            }
                
            


            //create a collide
            if ( m_meshCollider == null )
            {
                Transform col = vtm.transform.Find("VertExmotionEditorCollider");
                if( col != null )
                    DestroyImmediate(col.gameObject);

                GameObject go = new GameObject("VertExmotionEditorCollider");
                //go.transform.parent = vtm.transform;                
                m_meshCollider = go.AddComponent<MeshCollider>();

                if (vtm.GetComponent<SkinnedMeshRenderer>() != null)
                {                    
                    m_meshCollider.sharedMesh = new Mesh();
                    vtm.GetComponent<SkinnedMeshRenderer>().BakeMesh(m_meshCollider.sharedMesh);
                    EditorUtility.SetDirty(m_meshCollider);
                    go.transform.SetParent(vtm.transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    //go.transform.localScale = Vector3.one;
                }

                if (vtm.GetComponent<MeshFilter>() != null)
                {
                    m_meshCollider.sharedMesh = vtm.GetComponent<MeshFilter>().sharedMesh;
                    go.transform.SetParent(vtm.transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                }
                       
                
                         
                if ( !vtm.m_showMaterialIDWireFrame)
                    go.hideFlags = HideFlags.HideAndDontSave;
                  
                              
                Mesh m = new Mesh();
                m.vertices = m_meshCollider.sharedMesh.vertices;
                m.normals = m_meshCollider.sharedMesh.normals;
                if (m_selectedMaterialSlotID != 0)
                    m.triangles = vtm.m_mesh.GetTriangles(m_selectedMaterialSlotID - 1);
                else
                    m.triangles = vtm.m_mesh.triangles;
                m_meshCollider.sharedMesh = m;

                //----------------------------
                //Test
                /*
                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                MeshFilter mf = go.AddComponent<MeshFilter>();
                mf.sharedMesh = m;

                if (vtm.GetComponent<SkinnedMeshRenderer>() != null)
                {
                    mr.sharedMaterials = vtm.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
                    for( int i=0; i< mr.sharedMaterials.Length; ++i )
                    {
                        mr.sharedMaterials[i] = new Material(Shader.Find("Hidden/VertExmotion_editor"));

                    }
                }
                */

                //----------------------------
                m_triangles = null;
            }

            if(m_triangles == null )
            {
                m_triangles = m_meshCollider.sharedMesh.triangles;
            }



            //this is a screen base unit, don't change on zoom
            float constUnit = (svCam.ViewportToWorldPoint (Vector3.zero) - svCam.ViewportToWorldPoint (Vector3.one)).magnitude;
		    constUnit = HandleUtility.GetHandleSize(vtm.transform.position) * 10f;
		    m_brushSize = m_drawRadius * constUnit;
		    float paintRange = ( svCam.WorldToScreenPoint (mp) - svCam.WorldToScreenPoint (mp + svCam.transform.right * m_brushSize ) ).magnitude;
            //Debug.Log(""+ m_rendererType);
            //if( false )

            


            List<int> vtx = new List<int>();
            //raycast
            int maxRc = (int) (100f * m_drawRadius * 100f / 25f);
            for (int rc = 0; rc < maxRc; ++rc)
            {

                //Vector2 offset = new Vector2(((float)rc - 5f) / 10f, 0);
                float p = (float)rc / (float)maxRc;
                Vector2 offset = new Vector2(Mathf.Cos(Mathf.PI*2f*p*5f), Mathf.Sin(Mathf.PI * 2f * p*5f)) * p ;

                RaycastHit[] hits = Physics.RaycastAll(svCam.ScreenPointToRay(mp2d + offset * m_drawRadius*1000f));
                for (int hit = 0; hit < hits.Length; ++hit)
                {
                    if (hits[hit].collider != m_meshCollider)
                        continue;

                    int tri = hits[hit].triangleIndex;

                                        
                    if(!vtx.Contains(m_triangles[tri * 3 + 0]) )
                        vtx.Add(m_triangles[tri * 3 + 0]);
                    if (!vtx.Contains(m_triangles[tri * 3 + 1]))
                        vtx.Add(m_triangles[tri * 3 + 1]);
                    if (!vtx.Contains(m_triangles[tri * 3 + 2]))
                        vtx.Add(m_triangles[tri * 3 + 2]);
                    break;
                }

            }

            for (int id = 0; id < vtx.Count; ++id)            
            {
                int i = vtx[id];                
                
                if ( Vector3.Dot( vtm.transform.TransformDirection(m_nrml[i]).normalized, -svCam.transform.forward ) > 0 )
			    {
                    Vector3 localpos = m_vtx[i];
                    if (m_rendererType == eRendererType.SKINNEDMESH)
                    {
                        /*
                        //Debug.Log("eRendererType.SKINNEDMESH");
                        localpos.x /= vtm.transform.localScale.x;
                        localpos.y /= vtm.transform.localScale.y;
                        localpos.z /= vtm.transform.localScale.z;
                        */
                    }

                    //Vector3 worldPos = vtm.transform.TransformPoint(localpos);
                    Vector3 worldPos = m_meshCollider.transform.TransformPoint(localpos);

                    Vector3 vertex2D = svCam.WorldToScreenPoint( worldPos );
				    vertex2D.z=0;
				    Vector3 mouse2D = svCam.WorldToScreenPoint( mp );
				    mouse2D.z=0;
				    float dist = ( vertex2D - mouse2D ).magnitude;
				
				    if( dist < paintRange  )
				    {
					    float falloff = m_drawFalloff * (dist/paintRange);
					    float intensity =  m_drawIntensity - m_drawIntensity*falloff;
					
					
					    Handles.color = Color.Lerp(m_eraseMode?Color.red:Color.green, Color.blue, m_drawIntensity * falloff);
					    Handles.DrawSolidDisc( worldPos, -svCam.transform.forward, ( constUnit*.001f + constUnit*.0015f * intensity  ) );		
					
					    if( !Event.current.alt && Event.current.type == EventType.MouseDrag &&  Event.current.button == 0 /*&& !useHandle*/ )
					    {
						    //int i=kvp.Value[0];
						    Color vc = vtm.m_vertexColors[i];

                            
                            switch(m_currentLayer)
                            {
                                case 0:
                                    vc = Color.Lerp(vc, m_eraseMode?Color.black:Color.white, intensity * .2f);
                                    break;
                                case 1:                                    
                                    vc.r = Mathf.Lerp(vc.r, m_eraseMode ? 0f : 1f, intensity * .2f);
                                    break;
                                case 2:
                                    vc.g = Mathf.Lerp(vc.g, m_eraseMode ? 0f : 1f, intensity * .2f);
                                    break;
                                case 3:
                                    vc.b = Mathf.Lerp(vc.b, m_eraseMode ? 0f : 1f, intensity * .2f);
                                    break;
                            }


                            
                            for (int vid = 0; vid < m_posToVertices[m_vtx[i]].Count; ++vid)
                            {
                                int nVId = m_posToVertices[m_vtx[i]][vid];
                                vtm.m_vertexColors[nVId] = vc;
                            }
                            
						
					    }
				    }
			    }		
			
		    }
		    //-----------------------------------------------------------------------
		    //update vertices
		    if( Event.current.type == EventType.MouseDrag && Event.current.button < 2 )
		    {
			    //refresh colors
			    vtm.m_mesh.colors32 = vtm.m_vertexColors;
			    vtm.ApplyMotionData();
                Undo.RecordObject(vtm, "vertices paint");
                //EditorUtility.SetDirty( vtm );
                //Debug.Log( "update vertices" );
                //m_meshCollider.sharedMesh.colors32 = vtm.m_vertexColors;
            }
	    }
	
	
	
	
	
	    //-----------------------------------------------------
	    //Draw Menu
	    //-----------------------------------------------------
	    static float m_menuWidth = 235f;

	    public static Color orange = new Color (240f/255f, 158f/255f, 0);
	    public static Color grey = new Color (41f/255f, 41f/255f, 41f/255f, .9f);

	    Vector2 m_scrollViewPos;
	    void DrawMenu( VertExmotionBase vtm, Event e )
	    {

            
		    UpdateShowPanel ();

            if (m_showPanelProgress > 0.1f && !vtm.m_showEditorPanel
                || m_showPanelProgress < 1f && vtm.m_showEditorPanel
                )
                SceneView.currentDrawingSceneView.Repaint();

            if (m_showPanelProgress <= 0.1 || !vtm.m_editMode)
                return;

            

            Handles.BeginGUI ();

		    //GUI.matrix.SetTRS (Vector3.right * (-200f), Quaternion.identity, Vector3.one); 

		    DrawBackground ();

		    GUI.backgroundColor = orange;
		    if( GUI.Button( new Rect( (m_menuWidth+12) *m_showPanelProgress-12,0,24,35 ), vtm.m_showEditorPanel ? "x":">" ) )
                vtm.m_showEditorPanel = !vtm.m_showEditorPanel;

            m_showPanel = vtm.m_showEditorPanel;
            GUI.backgroundColor = Color.white;

           
            GUILayout.BeginArea( m_menuRect );


		    m_scrollViewPos = GUILayout.BeginScrollView (m_scrollViewPos, GUILayout.Width (m_menuWidth));
            GUILayout.BeginVertical(GUILayout.Width(m_menuWidth-15));


            GUI.backgroundColor = Color.gray;

		    DrawLogo();
    #if VERTEXMOTION_TRIAL
                EditorGUILayout.HelpBox("TRIAL VERSION", MessageType.Warning);
    #endif

            GUI.backgroundColor = Color.clear;

            

            DrawUI(vtm);
            //GUILayout.EndHorizontal();

            GUILayout.Space (100);

            GUILayout.EndVertical();

            GUILayout.EndScrollView ();
		    GUILayout.EndArea();
		    Handles.EndGUI ();

	    }



        public void DrawUI(VertExmotionBase vtm )
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (DrawInfoIcon(m_mode == eMode.INFO))
                m_mode = eMode.INFO;

            GUILayout.FlexibleSpace();
            if (warning)
                GUI.color = Color.gray;

            if (DrawPaintIcon(m_mode == eMode.PAINT) && !warning)
                m_mode = eMode.PAINT;

            GUILayout.FlexibleSpace();

            if (DrawSensorIcon(m_mode == eMode.SENSORS) && !warning)
                m_mode = eMode.SENSORS;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;

            GUI.color = Color.white;

            

            //GUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            switch (m_mode)
            {
                case eMode.INFO:
                    DrawInfo(vtm);
                    break;
                case eMode.PAINT:
                    DrawPaint(vtm);
                    break;
                case eMode.SENSORS:
                    DrawSensors(vtm);
                    break;
            }

            
        }


	    //------------------------------------------------------------------------------------------------------
	    //Draw Menu mode
	    //------------------------------------------------------------------------------------------------------
	    public bool warning = false;
	    void DrawInfo( VertExmotionBase vtm )
	    {
		    warning = false;

		    GUILayout.Label( "Info", m_styleHighLight, GUILayout.ExpandWidth(true), GUILayout.Height(30) );

            bool AlloyDetected = false;

            int fixMaterialID = -1;
                for (int i = 0; i < vtm.renderer.sharedMaterials.Length; ++i)
                {
                    if (vtm.renderer.sharedMaterials[i] == null)
                        continue;

                if (vtm.renderer.sharedMaterials[i].shader.name.Contains("Alloy/"))
                    AlloyDetected = true;

                    if (!vtm.renderer.sharedMaterials[i].shader.name.Contains("VertExmotion/")  
                    && !vtm.renderer.sharedMaterials[i].shader.name.Contains("Alloy/")
                    && vtm.renderer.sharedMaterials[i].shader.name != "Hidden/VertExmotion_editor"
                    
                    )
                    //if (!vtm.renderer.sharedMaterials[i].shader.name.Contains("VertExmotion/"))
                    {
                        fixMaterialID = i;
                        break;
                    }
                }


		    //---------------------------------------------------------
		    //material check
		    //---------------------------------------------------------
		    if(vtm.renderer.sharedMaterial == null || vtm.renderer.sharedMaterial.name == "Default-Diffuse"  || vtm.renderer.sharedMaterial.name == "Default-Material" || vtm.renderer.sharedMaterial.name == "Sprites-Default")
		    {

			    GUILayout.Label ("\nWarning!\n", m_styleTitle);
			    GUILayout.Label ("Object is using default material,", m_styleTitle);
			    GUILayout.Label ("please assign a new material", m_styleTitle);
			    GUILayout.Label ("with a VertExmotion shader\n", m_styleTitle);
			    warning = true;

		    }
			    else if( m_mode == eMode.INFO && fixMaterialID != -1 )
		    {
			    GUILayout.Label ("\nWarning!\n", m_styleTitle);
			    GUILayout.Label ("material "+ fixMaterialID +" doesn't use", m_styleTitle);
			    GUILayout.Label ("a VertExmotion shader.\n", m_styleTitle);
			    warning = true;

			    if( GUILayout.Button("Fix material") )
			    {
				    //Selection.activeObject = vtm.renderer.sharedMaterial;
				    //InitMaterialEditor( vtm );
				    string shaderName = vtm.renderer.sharedMaterials[fixMaterialID].shader.name;
				    if( Shader.Find ( "VertExmotion/" + shaderName ) != null )
				    {
					    vtm.renderer.sharedMaterials[fixMaterialID].shader = Shader.Find ( "VertExmotion/" + shaderName );	
					    vtm.m_initialShaders[fixMaterialID] = vtm.renderer.sharedMaterials[fixMaterialID].shader;
				    }
				    else
				    {
					    Debug.LogError( "Material use a non compatible shader ("+shaderName+"), please select a VertexMotion shader in your material properties or add VertExmotion function to your custom shader." );
				    }

                    

			    }

                if (vtm.m_dontCheckShaderCompatibility)
                    warning = false;

                vtm.m_dontCheckShaderCompatibility = EditorGUILayout.ToggleLeft("Don't check shader compatibility", vtm.m_dontCheckShaderCompatibility);

            }
		    else
		    {
                if(AlloyDetected)
                {
                    DrawAlloyLogo();
                    EditorGUILayout.HelpBox(
                        "Check the documentation for Alloy compatibility",
                        MessageType.Info
                        );

                }


			    GUILayout.Label ("Help :", m_styleTitle);
			    GUILayout.Space( 10 );
			    GUILayout.Label ("1. Paint vertices", m_styleBold);
			    GUILayout.Label ("   white for motion");
			    GUILayout.Label ("   black for static");
			    GUILayout.Space( 5 );
			    GUILayout.Label ("2. Add a sensor", m_styleBold);
			    GUILayout.Label ("   set position and range");
			    GUILayout.Label ("   vertices in range are highlighted");
			    GUILayout.Label ("   set the bouncing properties");
			    GUILayout.Space( 5 );
			    GUILayout.Label ("3. Add more sensors if needed", m_styleBold);
			    GUILayout.Label ("   20 sensors max");
			    GUILayout.Space( 5 );
			    GUILayout.Label ("4. Try it in play mode", m_styleBold);
			    GUILayout.Label ("   Move your gameobject");
			    GUILayout.Label ("   Have fun!");
		    }
            /*
            for (int i = 0; i < vtm.renderer.sharedMaterials.Length; ++i)
            {
                vtm.renderer.sharedMaterials[i].SetInt("_SensorId", -1);
                vtm.renderer.sharedMaterials[i].SetInt("_LayerId", m_currentLayer);
            }
            */

            if (!Application.isPlaying)
            {
                m_matPropBlk.SetFloat("_SensorId", -1);
                m_matPropBlk.SetFloat("_LayerId", m_currentLayer);
                vtm.renderer.SetPropertyBlock(m_matPropBlk);
            }
        }



	    Texture2D m_paintMap = null;
	    Mesh m_importMesh = null;
        static public bool m_paintFromMapAdditiveMode = false;
        

	    void DrawPaint( VertExmotionBase vtm )
	    {
		    GUILayout.Label( "Paint", m_styleHighLight, GUILayout.ExpandWidth(true), GUILayout.Height(30) );

            if (!Application.isPlaying && m_rendererType != eRendererType.SPRITE)
            {

                DrawBrushMenu();

                //if( GUILayout.Button ("Erase (ctrl)" ) )
                //	m_eraseMode = !m_eraseMode;

                m_eraseMode = EditorGUILayout.ToggleLeft("Erase mode (ctrl)", m_eraseMode);
                m_enableBrushMenuContextual = EditorGUILayout.ToggleLeft("contexual brush settings", m_enableBrushMenuContextual);

                DrawGradientToggle();
                

                GUILayout.Space(20);


                switch (m_currentLayer)
                {
                    case 0:
                        if (GUILayout.Button("Fill all layers"))
                            PaintAll(true);
                        if (GUILayout.Button("Clear all layers"))
                            PaintAll(false);
                        break;

                    case 1:
                        if (GUILayout.Button("Fill layer 1"))
                            PaintAll(Color.red,true);
                        if (GUILayout.Button("Clear layer 1"))
                            PaintAll(Color.red, false);
                        break;
                    case 2:
                        if (GUILayout.Button("Fill layer 2"))
                            PaintAll(Color.green, true);
                        if (GUILayout.Button("Clear layer 2"))
                            PaintAll(Color.green, false);
                        break;

                    case 3:
                        if (GUILayout.Button("Fill layer 3"))
                            PaintAll(Color.blue, true);
                        if (GUILayout.Button("Clear layer 3"))
                            PaintAll(Color.blue, false);
                        break;
            }



#if VERTEXMOTION_TRIAL
                    EditorGUILayout.HelpBox("TRIAL VERSION", MessageType.Warning);
                    GUI.enabled = false;
#endif
                GUILayout.Space(5);
                GUILayout.Label("Import/Export texture", m_styleTitle);

                GUILayout.Label("Export to texture", EditorStyles.boldLabel);
                if ( GUILayout.Button("Export") )
                {
                    ExportWeightsToTexture();
                }




                GUILayout.Label("Import from texture", EditorStyles.boldLabel);
                //paint map
                m_paintMap = EditorGUILayout.ObjectField( "Paint map", m_paintMap, typeof(Texture2D), false, GUILayout.Height(15) ) as Texture2D;

				    GUILayout.Label( AssetPreview.GetAssetPreview( m_paintMap ) );
			

			
			    if( m_paintMap == null )
			    {
			    GUI.enabled = false;
			    }
			    else
			    {
				    GUILayout.Label( "Paint map must be readable" );
				    GUILayout.Label( "texture inspector settings :" );
				    GUILayout.Label( "  - Texture Type : Advanced" );
				    GUILayout.Label( "  - Enable Read/Write" );

				    if( vtm.m_mesh.uv2.Length> 0 )
				    {
					    //choose UV1 or UV2
					    GUILayout.BeginHorizontal();
					    m_useUV1 = EditorGUILayout.ToggleLeft( "UV1", m_useUV1, GUILayout.Width(50) );
					    m_useUV1 = !EditorGUILayout.ToggleLeft( "UV2", !m_useUV1, GUILayout.Width(50) );
					    GUILayout.EndHorizontal();
				    }

                    m_paintFromMapAdditiveMode = EditorGUILayout.Toggle("Additive mask", m_paintFromMapAdditiveMode);

                    

                    if ( GUILayout.Button("Import") )
				    {
					    //Texture2D txtr = Instantiate( m_paintMap ) as Texture2D;
					    Vector2[] uv = vtm.m_mesh.uv;
					    if( !m_useUV1 )
						    uv = vtm.m_mesh.uv2;

                        int max = vtm.m_vertexColors.Length;
                        List<int> ids = new List<int>();

                        if ( m_selectedMaterialSlotID != 0 )
                        {                            

                            int[] tri = vtm.m_mesh.GetTriangles(m_selectedMaterialSlotID - 1);
                            for (int t = 0; t < tri.Length; ++t)
                            {
                                if (!ids.Contains(tri[t]))
                                    ids.Add(tri[t]);
                            }

                            max = ids.Count;
                        }


                        for (int id = 0; id < max; ++id)
                        {

                            int i = id;

                            if (m_selectedMaterialSlotID != 0)
                                i = ids[id];


                                Color32 c = m_paintMap.GetPixel((int)(uv[i].x * m_paintMap.width), (int)(uv[i].y * m_paintMap.height));

                            switch (m_currentLayer)
                            {

                                case 0:
                                    if (c.r >= vtm.m_vertexColors[i].r || !m_paintFromMapAdditiveMode)
                                        vtm.m_vertexColors[i].r = c.r;
                                    if (c.g >= vtm.m_vertexColors[i].g || !m_paintFromMapAdditiveMode)
                                        vtm.m_vertexColors[i].g = c.g;
                                    if (c.b >= vtm.m_vertexColors[i].b || !m_paintFromMapAdditiveMode)
                                        vtm.m_vertexColors[i].b = c.b;
                                    break;

                                case 1:
                                    if (c.r >= vtm.m_vertexColors[i].r || !m_paintFromMapAdditiveMode)
                                        vtm.m_vertexColors[i].r = c.r;
                                    break;

                                case 2:
                                    if (c.g >= vtm.m_vertexColors[i].g || !m_paintFromMapAdditiveMode)
                                        vtm.m_vertexColors[i].g = c.g;
                                    break;

                                case 3:
                                    if (c.b >= vtm.m_vertexColors[i].b || !m_paintFromMapAdditiveMode)
                                        vtm.m_vertexColors[i].b = c.b;
                                    break;

                            }

                        }
					    vtm.ApplyMotionData();

				    }
			    }

                GUI.enabled = true;


                GUILayout.Space(10);
                GUILayout.Label("Mesh reference", m_styleTitle);
                vtm.m_params.usePaintDataFromMeshColors = EditorGUILayout.Toggle("Use mesh reference", vtm.m_params.usePaintDataFromMeshColors);
               
                GUILayout.Label("Using a mesh reference avoid\nthe mesh duplication in memory,\nwhen there's multiple instances.", EditorStyles.helpBox);

                if (vtm.m_params.usePaintDataFromMeshColors)
                {

                    //-----------------------------------
                    //Export mesh data
#if VERTEXMOTION_TRIAL
                    //EditorGUILayout.HelpBox("TRIAL VERSION", MessageType.Warning);
                    GUI.enabled = false;
#endif



                    
                    string assetName = GetAssetName();
                    if (m_exportMeshName == "")
                        m_exportMeshName = assetName.Replace("_VertExmotion", "");


                    GUILayout.Label("Export mesh", m_styleBold);
                    GUILayout.Label("Create a new mesh reference\nincluding painting data");
                    m_exportMeshName = EditorGUILayout.TextField(m_exportMeshName);


                    //if( m_exportMeshName == assetName.Replace("_VertExmotion","") && assetName.Contains("_VertExmotion") )
                    //    GUI.enabled = false;

                    if (GUILayout.Button("Save mesh reference"))
                    {
                        //ExportMesh();
                        m_exportFlag = true;
                        Repaint();
                    }


                    GUI.enabled = true;

#if VERTEXMOTION_TRIAL
                    //EditorGUILayout.HelpBox("TRIAL VERSION", MessageType.Warning);
                    GUI.enabled = false;
#endif
                    GUILayout.Label("Import mesh reference", m_styleBold);
                    m_importMesh = EditorGUILayout.ObjectField(m_importMesh, typeof(Mesh), false) as Mesh;

                    GUI.enabled = m_importMesh != null;
                    if (GUILayout.Button("Import"))
                    {
                        //apply new mesh
                        vtm.SetMesh(m_importMesh);
                        vtm.InitMesh();
                        vtm.m_params.usePaintDataFromMeshColors = true;//enable mesh data driven
                    }


                    GUI.enabled = true;
                    /*
                    if (GUILayout.Button("Find mesh reference"))
                        EditorGUIUtility.PingObject(vtm.m_mesh);
                        */


                }
    //tips


			    if( m_enableBrushMenuContextual )
			    {
				    GUILayout.Space(20);
				    GUILayout.Label ("tips :", m_styleTitle );
				    GUILayout.Space(10);

				    GUILayout.Label ("Alt : contextual brush settings" );
			    }

		    }
		    else
		    {
                if(m_rendererType != eRendererType.SPRITE )
			        GUILayout.Label ("Paint disabled in Play mode", m_styleBold );
                else
                    GUILayout.Label("Paint disabled for Sprite and TextMesh", m_styleBold);
                }

            /*
            for (int i = 0; i < vtm.renderer.sharedMaterials.Length; ++i)
                vtm.renderer.sharedMaterials[i].SetInt( "_SensorId", -1 );
                */

            if (!Application.isPlaying)
            {
                m_matPropBlk.SetFloat("_SensorId", -1);
                vtm.renderer.SetPropertyBlock(m_matPropBlk);
            }
            
            
        }



        /// <summary>
        /// Draw sensors settings
        /// </summary>
        /// <param name="vtm"></param>
	    void DrawSensors( VertExmotionBase vtm )
	    {
		    GUILayout.Label( "Sensors", m_styleHighLight, GUILayout.ExpandWidth(true), GUILayout.Height(30) );
		    DrawSensorsList ( vtm );

            /*
            for( int i=0; i< vtm.renderer.sharedMaterials.Length; ++i )
                vtm.renderer.sharedMaterials[i].SetInt( "_SensorId", m_sensorId );
                */

            if (!Application.isPlaying)
            {
                m_matPropBlk.SetFloat("_SensorId", m_sensorId);
                vtm.renderer.SetPropertyBlock(m_matPropBlk);
            }

            /*
		    if( Event.current.isKey && Event.current.keyCode == KeyCode.F && m_sensorId>-1 )
		    {
			    //SceneView.currentDrawingSceneView.LookAt( vtm.m_VertExmotionSensors[m_sensorId].transform.position );
			    SceneView.currentDrawingSceneView.camera.transform.position = vtm.m_VertExmotionSensors[m_sensorId].transform.position + (SceneView.currentDrawingSceneView.camera.transform.position-vtm.m_VertExmotionSensors[m_sensorId].transform.position) * vtm.m_VertExmotionSensors[m_sensorId].m_envelopRadius;
			    Event.current.Use();
		    }*/

        }


        //------------------------------------------------------------------------------------------------------
        //Draw menu parts
        //------------------------------------------------------------------------------------------------------

        //List<float> m_sensorLinksEditor = new List<float>();
        

	    void DrawSensorsList( VertExmotionBase vtm )
	    {
		    GUILayout.Label( "Sensors list", m_styleTitle );
		    //m_paintMode = GUILayout.Toggle (m_paintMode, "paint mode");

		    if( vtm.m_VertExmotionSensors.Count== 0 )
			    m_sensorId = -1;
		    else if( m_sensorId == -1 )
			    m_sensorId = 0;
		
		    for( int i=0; i<vtm.m_VertExmotionSensors.Count; ++i )
		    {

                if (vtm.m_VertExmotionSensors[i] == null)
                   continue;

                if (vtm.m_sensorsLinks.Count <= i)
                    vtm.m_sensorsLinks.Add(0);


                if ( i==m_sensorId )
				    GUI.color = orange;

			    GUILayout.BeginHorizontal();
			    if( GUILayout.Toggle( i==m_sensorId, "" + (i+1), GUILayout.Width(34)) )
			    {
				    m_sensorId = i;
			    }
			    vtm.m_VertExmotionSensors[i].name = EditorGUILayout.TextField( vtm.m_VertExmotionSensors[i].name );

                float previousSensorLink = 0f;

                //-----------------
                //link system                
                // 0 -> unlinked
                // 1 -> master link
                // -1 -> slave link
                // 2-> master & slave
                GUI.color = Color.white;
                if (i > 0)
                {
                    GUILayout.Space(-6);
                    GUILayout.BeginVertical();
                    GUILayout.Space(-8);
                    //bool lnk = GUILayout.Toggle(vtm.m_sensorsLinks[i-1] == 1, DrawlinkIcon(vtm.m_sensorsLinks[i - 1] == 1) );
                    bool lnk = vtm.m_sensorsLinks[i - 1] == 1;
                    if (GUILayout.Button(DrawlinkIcon(vtm.m_sensorsLinks[i - 1] == 1), EditorStyles.boldLabel))
                        lnk = !lnk;

                    GUILayout.EndVertical();
                    if(i>1)
                        previousSensorLink = vtm.m_sensorsLinks[i - 2];

                    vtm.m_sensorsLinks[i-1] = lnk ? 1 : previousSensorLink == 1 ? -1 : 0;
                    if(vtm.m_sensorsLinks[i - 1]==1 && vtm.m_sensorsLinks[i]!=1)
                        vtm.m_sensorsLinks[i] = -1;

                    if (i == vtm.m_VertExmotionSensors.Count-1)
                        vtm.m_sensorsLinks[i] = vtm.m_sensorsLinks[i - 1] >=1?-1:0;

                }
                else
                {
                    GUILayout.Space(18);                   
                }

                //-----------------

                //GUILayout.Label("" + vtm.m_sensorsLinks[i]);
                GUILayout.EndHorizontal();

			    GUI.color = Color.white;
                /*
			    if( i==3 )
				    GUILayout.Label("      max sensor limit for SM2 shaders");
                    */
			    //if( i==7 )
				//    GUILayout.Label( "      max sensor limit for SM2 shaders" );
		    }

		
		
		    if( vtm.m_VertExmotionSensors.Count<VertExmotionBase.MAX_SENSOR )
		    {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();

                GUI.enabled = m_sensorId > 0;
                if(GUILayout.Button(DrawUpDownIcon(true), EditorStyles.miniButtonLeft,GUILayout.Width(18), GUILayout.Height(14)))
                {
                    VertExmotionSensorBase tmp = vtm.m_VertExmotionSensors[m_sensorId-1];
                    vtm.m_VertExmotionSensors[m_sensorId - 1] = vtm.m_VertExmotionSensors[m_sensorId];
                    vtm.m_VertExmotionSensors[m_sensorId] = tmp;
                    m_sensorId = m_sensorId - 1;

                }
                GUI.enabled = m_sensorId < vtm.m_VertExmotionSensors.Count-1;
                if (GUILayout.Button(DrawUpDownIcon(false), EditorStyles.miniButtonRight, GUILayout.Width(18), GUILayout.Height(14)))
                {
                    VertExmotionSensorBase tmp = vtm.m_VertExmotionSensors[m_sensorId + 1];
                    vtm.m_VertExmotionSensors[m_sensorId + 1] = vtm.m_VertExmotionSensors[m_sensorId];
                    vtm.m_VertExmotionSensors[m_sensorId] = tmp;
                    m_sensorId = m_sensorId + 1;
                }
                GUI.enabled = true;
                GUILayout.FlexibleSpace();

                if ( GUILayout.Button("New sensor", EditorStyles.miniButton, GUILayout.Height(14)) )
			    {
    #if VERTEXMOTION_TRIAL
                    if (vtm.m_VertExmotionSensors.Count >= 1)
                    {
                        EditorUtility.DisplayDialog("TRIAL VERSION", "The number of sensors is limited to one for the trial version.", "OK");
                    }
                    else
    #endif
                    {
                        EditorPrefs.SetInt("VertExmotion_LastMode", (int)m_mode);
                        VertExmotionSensorBase sensor = vtm.CreateSensor();
                        GameObject go = sensor.gameObject;
                        ReplaceBaseClass(sensor);
                        //EditorUtility.SetDirty(go);
                        Undo.RecordObject(go, "new sensor");
                        sensor = go.GetComponent<VertExmotionSensorBase>();
                        //EditorUtility.SetDirty(sensor);
                        //EditorUtility.SetDirty( vtm );
                        vtm.m_VertExmotionSensors.Add(sensor);
                        vtm.m_sensorsLinks.Add(0);
                        m_sensorId = vtm.m_VertExmotionSensors.Count - 1;

                        //m_lastMode = m_mode;//Editor come back to the current mode


                        //vtm.m_VertExmotionSensors[vtm.m_VertExmotionSensors.Count-1] = sensor as VertExmotionSensorBase;
                    }
                }

                GUILayout.Space(18);
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.Label( "Add existing sensor", EditorStyles.miniLabel);
                GUILayout.BeginHorizontal();
                m_externalSensor = EditorGUILayout.ObjectField( m_externalSensor, typeof(VertExmotionSensorBase), true ) as VertExmotionSensorBase;
			    GUI.enabled = m_externalSensor != null && !vtm.m_VertExmotionSensors.Contains(m_externalSensor);
                if (GUILayout.Button("Add sensor", EditorStyles.miniButton))
                {
                    vtm.m_VertExmotionSensors.Add(m_externalSensor);
                    vtm.m_sensorsLinks.Add(0);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUI.enabled = true;


			    
		    }
		    else
		    {
			    GUILayout.Label( "      max sensor limit" );
		    }


            DrawGradientToggle();


		    if (!Application.isPlaying)
		    {
                
                m_matPropBlk.SetFloat("_SensorId", m_sensorId);
                m_matPropBlk.SetFloat("_LayerId", m_currentLayer);

                /*
                if (m_sensorId > -1 && m_sensorId < vtm.m_VertExmotionSensors.Count && vtm.m_VertExmotionSensors[m_sensorId] != null)
                    m_matPropBlk.SetFloat("_Power", vtm.m_VertExmotionSensors[m_sensorId].m_params.power);
                    */
                m_matPropBlk.SetFloat("KVM_NbSensors", vtm.m_VertExmotionSensors.Count);
                vtm.renderer.SetPropertyBlock(m_matPropBlk);
                
            }

		    if(m_sensorId>-1 && m_sensorId<vtm.m_VertExmotionSensors.Count && vtm.m_VertExmotionSensors[m_sensorId] != null )
		    {

                if (!Application.isPlaying )
                {

                    if (m_mode == eMode.SENSORS)
                    {

                        /*
                            m_sensorpos[m_sensorId] = vtm.m_VertExmotionSensors[m_sensorId].transform.position;
                            m_sensorpos[m_sensorId].w = vtm.m_VertExmotionSensors[m_sensorId].m_layerID;


                            Vector4 radiusCentripetalTorque = Vector4.zero;
                            radiusCentripetalTorque.x = vtm.m_VertExmotionSensors[m_sensorId].m_envelopRadius;
                            radiusCentripetalTorque.y = vtm.m_VertExmotionSensors[m_sensorId].m_centripetalForce;
                            radiusCentripetalTorque.z = vtm.m_VertExmotionSensors[m_sensorId].m_motionTorqueForce;
                            m_RadiusCentripetalTorque[m_sensorId] = radiusCentripetalTorque;
                            m_RadiusCentripetalTorque[m_sensorId].x *= VertExmotionBase.GetScaleFactor(vtm.transform);

                            //vtm.renderer.GetPropertyBlock(m_matPropBlk);
                            //m_matPropBlk.SetVectorArray("_SensorPositionEditor", m_sensorpos);
                            //m_matPropBlk.SetVectorArray("_RadiusCentripetalTorqueEditor", m_RadiusCentripetalTorque);
                        */


                        if (m_sensorId != -1)
                        {
                            //if (m_shaderSensorLinksEditor == null || m_shaderSensorLinksEditor.Length != vtm.m_VertExmotionSensors.Count)
                                m_shaderSensorLinksEditor = new float[50];

                            //assign current sensor link info
                            m_shaderSensorLinksEditor[m_sensorId] = vtm.m_sensorsLinks[m_sensorId];
                            
                            //get next links
                            
                            bool linked = vtm.m_sensorsLinks[m_sensorId] == 1;
                            for (int i = m_sensorId + 1; i < vtm.m_VertExmotionSensors.Count; ++i)
                            {
                                if (vtm.m_sensorsLinks[i] != 1)
                                    linked = false;
                                m_shaderSensorLinksEditor[i] = linked?1:-1;
                            }
                            

                            //get previous links
                            linked = vtm.m_sensorsLinks[m_sensorId] == 1 || vtm.m_sensorsLinks[m_sensorId] == -1;
                            for (int i = m_sensorId - 1; i >= 0; --i)
                            {
                                if (vtm.m_sensorsLinks[i] != 1)
                                    linked = false;
                                m_shaderSensorLinksEditor[i] = linked ? 1 : -1;
                            }
                            
                            
                            

                            for (int i = 0; i < vtm.m_VertExmotionSensors.Count; ++i)
                            {
                                m_sensorpos[i] = vtm.m_VertExmotionSensors[i].transform.position;
                                m_sensorpos[i].w = vtm.m_VertExmotionSensors[i].m_layerID;
                                Vector4 radiusCentripetalTorque = Vector4.zero;
                                radiusCentripetalTorque.x = vtm.m_VertExmotionSensors[i].m_envelopRadius;
                                m_RadiusCentripetalTorque[i] = radiusCentripetalTorque;
                                m_RadiusCentripetalTorque[i].x *= VertExmotionBase.GetScaleFactor(vtm.transform);
                                m_power[i] = vtm.m_VertExmotionSensors[i].m_params.power;
                            }
                        }
                    }


                    m_matPropBlk.SetVectorArray("KVMEditor_SensorPosition", m_sensorpos);
                    m_matPropBlk.SetVectorArray("KVMEditor_RadiusCentripetalTorque", m_RadiusCentripetalTorque);
                    m_matPropBlk.SetFloatArray("KVMEditor_Power", m_power);
                    m_matPropBlk.SetFloatArray("KVMEditor_Links", m_shaderSensorLinksEditor);
                    m_matPropBlk.SetFloat("KVM_NbSensors", vtm.m_VertExmotionSensors.Count);


                    vtm.renderer.SetPropertyBlock(m_matPropBlk);
                                            
                }

			    VertExmotionSensorEditor.DrawSensorSettings( vtm.m_VertExmotionSensors[m_sensorId] );

			    GUILayout.Space( 20 );

			    if( GUILayout.Button( "Show in Inspector" ) )
			    {
				    //m_paintMode = false;
				    m_mode = eMode.INFO;
				    Selection.activeGameObject = vtm.m_VertExmotionSensors[m_sensorId].gameObject;
			    }
		    }


		    if( m_sensorId != -1 && m_sensorToRemove == -1 && m_sensorToDelete == -1 && GUILayout.Button("Delete sensor") )
		    {
			    m_sensorToDelete = m_sensorId;
		    }

		    if( m_sensorId != -1 && m_sensorToRemove == -1 && m_sensorToDelete == -1 && GUILayout.Button("Remove sensor") )
		    {
			    m_sensorToRemove = m_sensorId;
		    }



		    if( m_sensorToDelete != -1 )
		    {
			
			    GUILayout.BeginHorizontal();
			    GUILayout.Label("Delete sensor ?\ndestroy gameobject", m_styleTitle);
			    if( GUILayout.Button("Yes") )
			    {
				    if( Application.isPlaying )
					    Destroy( vtm.m_VertExmotionSensors[m_sensorToDelete].gameObject );
				    else
					    DestroyImmediate( vtm.m_VertExmotionSensors[m_sensorToDelete].gameObject );

				    vtm.m_VertExmotionSensors.RemoveAt( m_sensorToDelete );

				    m_sensorId = -1;
				    m_sensorToDelete = -1;
			    }

			    if( GUILayout.Button("No") )
				    m_sensorToDelete = -1;

			    GUILayout.EndHorizontal();
		    }



		    if( m_sensorToRemove != -1 )
		    {
			    GUILayout.BeginHorizontal();
			    GUILayout.Label("Remove sensor ?", m_styleTitle);
			    if( GUILayout.Button("Yes") )
			    {
				    vtm.m_VertExmotionSensors.RemoveAt( m_sensorToRemove );
				    m_sensorId = -1;
				    m_sensorToRemove = -1;
			    }
			
			    if( GUILayout.Button("No") )
				    m_sensorToRemove = -1;
			
			    GUILayout.EndHorizontal();
		    }




	    }



        List<string> m_materialSlotsName = null;
        int[] m_materialSlotIDs = null;
        int m_selectedMaterialSlotID = 0;

        void DrawBrushMenu()
	    {
            VertExmotionBase vtm = target as VertExmotionBase;

		    GUILayout.Label ("Brush settings", m_styleTitle, GUILayout.ExpandWidth(true));	
		    GUILayout.Space (10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Layer", m_styleBold, GUILayout.Width(80));
            string[] layersName = { "All", "1", "2", "3" };
            int[] layersIds = { 0, 1, 2, 3 };
            m_currentLayer = EditorGUILayout.IntPopup(m_currentLayer, layersName, layersIds, GUILayout.Width(50));

            m_matPropBlk.SetFloat("_LayerId", m_currentLayer);
            vtm.renderer.SetPropertyBlock(m_matPropBlk);
            GUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            
            if (vtm.m_mesh.subMeshCount > 1)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Material ID", m_styleBold, GUILayout.Width(80));
                if (m_materialSlotsName == null)
                {
                    m_materialSlotsName = new List<string>();
                    m_materialSlotsName.Add("All");                    
                    m_materialSlotIDs = new int[vtm.m_mesh.subMeshCount+1];
                    m_materialSlotIDs[0] = 0;
                    for (int i = 0; i < vtm.m_mesh.subMeshCount; ++i)
                    {
                        m_materialSlotIDs[i + 1] = i+1;
                        m_materialSlotsName.Add("" + (i + 1));
                    }
                }                
                m_selectedMaterialSlotID = EditorGUILayout.IntPopup(m_selectedMaterialSlotID, m_materialSlotsName.ToArray(), m_materialSlotIDs, GUILayout.Width(50));
                GUILayout.EndHorizontal();
                if (m_selectedMaterialSlotID != 0)
                {
                    vtm.m_showMaterialIDWireFrame = EditorGUILayout.ToggleLeft("show material wireframe", vtm.m_showMaterialIDWireFrame);
                }
                else
                {
                    EditorGUILayout.LabelField("");
                    vtm.m_showMaterialIDWireFrame = false;
                }
            }
            else
            {
                m_selectedMaterialSlotID = 0;
            }
            
            if (EditorGUI.EndChangeCheck())
                m_meshCollider = null;


            GUILayout.Space(5);
            GUILayout.Label ("Size : "+m_drawRadius*100f/25f, m_styleBold, GUILayout.ExpandWidth(true));	
		    m_drawRadius = GUILayout.HorizontalSlider ( m_drawRadius, .01f, .25f );
		    GUILayout.Label ("Intensity : "+ Mathf.Round( m_drawIntensity *100f )/100f, m_styleBold, GUILayout.ExpandWidth(true));	
		    m_drawIntensity = GUILayout.HorizontalSlider ( m_drawIntensity*100f/100, 0.01f, 1f );
		    GUILayout.Label ("Falloff : "+  Mathf.Round( m_drawFalloff *100f ) / 100f, m_styleBold, GUILayout.ExpandWidth(true));	
		    m_drawFalloff = GUILayout.HorizontalSlider ( m_drawFalloff, 0f, 1f );

	    }


	    void DrawBrushMenuContextual( Event e )
	    {
		    //float menuWidth = 150f;
		    if( m_enableBrushMenuContextual )
		    {
			    Handles.BeginGUI ();

			    GUI.Box (m_brushMenuRect, "", m_bgStyle );
			    GUILayout.BeginArea( m_brushMenuRect );

			    DrawBrushMenu ();

			    GUILayout.EndArea ();
			    Handles.EndGUI ();
		    }
	    }






	    void DrawCursor( Vector3 mp, Camera svCam )
	    {
		    Color c = m_eraseMode ? Color.red : Color.gray;
		    c.a = .05f + .1f * m_drawIntensity;
		    Handles.color = c;
		    Handles.DrawSolidDisc( mp, -svCam.transform.forward, m_brushSize );	

		    c = orange;
		    c.a = 1f;
		    Handles.color = c;
		    Handles.DrawWireDisc( mp, -svCam.transform.forward, m_brushSize );	

		    c.a = .5f;
		    Handles.color = c;
		    Handles.DrawWireDisc( mp, -svCam.transform.forward, m_brushSize * (1f-m_drawFalloff) );	
	    }





	    //------------------------------------------------------------------------------------------------------
	    //On enable / disable / destroy
	    //------------------------------------------------------------------------------------------------------

	    void OnEnable()
	    {
            

                m_editorInitialized = false;
                m_editorInstanceInitialized = false;

                InitializeEditorInstance();

		    //Debug.Log ("OnEnable");
		    VertExmotionBase vtm = (target) as VertExmotionBase;

            /*
            string[] versionSplit = vtm.m_version.Split('.');
            if (versionSplit[0]=="1" && float.Parse(versionSplit[1])<6 )
            {                
                    m_displayScaleFixButton = true;
            }
            */


            //vtm.m_params.version = VertExmotionBase.version;

            if ( vtm.GetComponent<MeshFilter>() == null 
                    && vtm.GetComponent<SkinnedMeshRenderer>() == null 
                    && vtm.GetComponent<SpriteRenderer>() == null
                    && vtm.GetComponent<TextMesh>() == null
                    )
            //if(m_rendererType==eRendererType.NONE)
		    {
			    Debug.LogError( "VertExmotion need a MeshFilter, a SkinnedMeshRenderer or a SpriteRenderer component" );
			    if( !Application.isPlaying ) 
				    DestroyImmediate( vtm );
			    else
				    Destroy ( vtm );

			    return;
		    }


		    //m_paintMode = false;
		    //m_mode = eMode.INFO;
		    m_lastTool = Tools.current;
		    m_showPanelProgress = 0;

		    if( !Application.isPlaying && vtm.m_meshCopy )
			    DestroyImmediate ( vtm.m_mesh );
		    vtm.m_mesh = null;

		    //FIX when script compile while edition
		    if( vtm.m_initialShaders != null && vtm.renderer.sharedMaterials.Length == vtm.m_initialShaders.Length )
		    {
			    for( int i=0; i<vtm.renderer.sharedMaterials.Length; ++i )
			    {
                        if (vtm.renderer.sharedMaterials[i] == null)
                            continue;

				    if( vtm.renderer.sharedMaterials[i].shader.name.StartsWith("Hidden" ) )
				    {
					    vtm.renderer.sharedMaterials[i].shader = vtm.m_initialShaders[i];
                        EditorUtility.SetDirty( vtm.renderer.sharedMaterials[i] );                        
                    }
			    }
		    }

		    //update shader list
		    vtm.m_initialShaders = new Shader[vtm.renderer.sharedMaterials.Length];

		    for( int i=0; i<vtm.renderer.sharedMaterials.Length; ++i )
		    {
			    vtm.m_initialShaders[i] = vtm.renderer.sharedMaterials[i].shader;			
		    }

		    m_initialized = false;


	    }


	    void OnDisable()
	    {
           m_exportMeshName = "";

           if (Application.isPlaying)
	            return;

		    if (target == null)
			    return;

            
            if (m_meshCollider != null)
                DestroyImmediate(m_meshCollider.gameObject);
            
      
		    VertExmotionBase vtm = (target) as VertExmotionBase;
		    if( vtm == null )
			    return;

            

		    for( int i=0; i<vtm.renderer.sharedMaterials.Length; ++i )
		    {
                    if (vtm.renderer.sharedMaterials[i] == null)
                        continue;

			    if( vtm.renderer.sharedMaterials[i].shader.name == "Hidden/VertExmotion_editor" )
				    vtm.renderer.sharedMaterials[i].shader = vtm.m_initialShaders[i];
			    EditorUtility.SetDirty( vtm.renderer.sharedMaterials[i] );
                    vtm.DisableMotion();

            }
	
		    Tools.current = m_lastTool;

		    if( SceneView.lastActiveSceneView != null && m_mode == eMode.PAINT || m_mode == eMode.SENSORS )
			    SceneView.lastActiveSceneView.orthographic = m_lastOrthoMode;
		    //m_mode = eMode.INFO;

		    m_lastVTMSelected = vtm;

		    
            }


	    public static GUIStyle m_winBg = null;
	    public static GUIStyle m_bgStyle = new GUIStyle();
	    public static void DrawBackground()
	    {

		    if( m_winBg == null )
		    {
			    foreach(GUIStyle style in GUI.skin.customStyles)
                    //if( style.name == "WindowBackground" )
                    if (style.name == "Wizard Box")
                        m_winBg = style;
		    }

		    GUI.color = Color.white;
		    GUI.backgroundColor = Color.white;
            if(m_winBg!=null)
		        GUI.Label (m_menuRect,"", m_winBg);
		
	    }











	    //------------------------------------------------------------------------------------------------------
	    //Logo & icons
	    //------------------------------------------------------------------------------------------------------

	    static Texture2D m_logo;
	    public static void DrawLogo()
	    {
		    if( m_logo==null )
		    {
			    m_logo = (Texture2D)Resources.Load("Icons/VertExmotion_banner_200",typeof(Texture2D));
		    }

		    GUILayout.BeginHorizontal ();
		    GUILayout.FlexibleSpace ();
		    GUILayout.Label(m_logo, GUILayout.MaxWidth(200f) );		
		    GUILayout.FlexibleSpace ();
		    GUILayout.EndHorizontal ();
	    }


        static Texture2D m_AlloyLogo;
        public static void DrawAlloyLogo()
        {
            if (m_AlloyLogo == null)
            {
                m_AlloyLogo = (Texture2D)Resources.Load("Icons/Alloy", typeof(Texture2D));
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(m_AlloyLogo, GUILayout.MaxWidth(200f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        static Texture2D[] m_infoIcons = new Texture2D[2];
	    public static bool DrawInfoIcon( bool enable )
	    {
		    for(int i=0; i<2; ++i)
			    if( m_infoIcons[i]==null )		
				    m_infoIcons[i] = (Texture2D)Resources.Load( "Icons/info_icon_"+(i==0?"off":"on"),typeof(Texture2D));
		    if (GUILayout.Button (enable ? m_infoIcons [1] : m_infoIcons [0], GUILayout.Width (50), GUILayout.Height (50)))
			    return !enable;
		
		    return enable;
	    }


	    static Texture2D[] m_paintIcons = new Texture2D[2];
	    public static bool DrawPaintIcon( bool enable )
	    {
		    for(int i=0; i<2; ++i)
			    if( m_paintIcons[i]==null )		
				    m_paintIcons[i] = (Texture2D)Resources.Load( "Icons/paint_icon_"+(i==0?"off":"on"),typeof(Texture2D));

		    if (GUILayout.Button (enable ? m_paintIcons [1] : m_paintIcons [0], GUILayout.Width (50), GUILayout.Height (50)))
			    return !enable;

		    return enable;
	    }

	    static Texture2D[] m_sensorIcons = new Texture2D[2];
	    public static bool DrawSensorIcon( bool enable )
	    {
		    for(int i=0; i<2; ++i)
			    if( m_sensorIcons[i]==null )		
				    m_sensorIcons[i] = (Texture2D)Resources.Load( "Icons/sensor_icon_"+(i==0?"off":"on"),typeof(Texture2D));
		
		    if (GUILayout.Button (enable ? m_sensorIcons [1] : m_sensorIcons [0], GUILayout.Width (50), GUILayout.Height (50)))
			    return !enable;
		
		    return enable;
	    }

        
        static Texture2D[] m_linkIcon = new Texture2D[2];
        public static Texture2D DrawlinkIcon( bool linked)
        {
            if (m_linkIcon[linked?0:1] == null)
                m_linkIcon[linked ? 0 : 1] = (Texture2D)Resources.Load("Icons/link_icon_"+ (linked?"on":"off"), typeof(Texture2D));
            return m_linkIcon[linked ? 0 : 1];
        }

        static Texture2D[] m_upDownIcon = new Texture2D[2];
        public static Texture2D DrawUpDownIcon(bool up)
        {
            if (m_upDownIcon[up ? 0 : 1] == null)
                m_upDownIcon[up ? 0 : 1] = (Texture2D)Resources.Load("Icons/" + (up ? "up_icon" : "down_icon"), typeof(Texture2D));
            return m_upDownIcon[up ? 0 : 1];
        }


        //------------------------------------------------------------------------------------------------------
        //check vertices on same position
        //------------------------------------------------------------------------------------------------------
        //Vector3[] m_verticesPos;
        Dictionary<Vector3,List<int>> m_posToVertices = new Dictionary<Vector3,List<int>>();
	    void InitVerticesPosDictionary()
	    {
                if ((target as VertExmotionBase).m_mesh == null)
                    return;

		    foreach( KeyValuePair<Vector3,List<int>> kvp in m_posToVertices )
			    kvp.Value.Clear();
		    m_posToVertices.Clear ();
		    m_vtx = (target as VertExmotionBase).m_mesh.vertices; 

		    for( int i=0; i<m_vtx.Length; ++i )
		    {
			    if( !m_posToVertices.ContainsKey( m_vtx[i] ) )
				    m_posToVertices.Add( m_vtx[i], new List<int>() );

			    m_posToVertices[m_vtx[i]].Add( i );
		    }
	    }



		void PaintAll( bool paintWhite )
		{
            VertExmotionBase vtm = (target as VertExmotionBase);

            if (m_selectedMaterialSlotID != 0)
            {
                int[] tri = vtm.m_mesh.GetTriangles(m_selectedMaterialSlotID - 1);
                for (int i = 0; i < tri.Length; ++i)
                {
                    vtm.m_vertexColors[tri[i] + 0] = paintWhite ? Color.white * m_drawIntensity : Color.black;                        
                }
            }
            else
            {
                    
                for (int i = 0; i < vtm.m_vertexColors.Length; ++i)
                {                        
                    vtm.m_vertexColors[i] = paintWhite ? Color.white * m_drawIntensity : Color.black;
                }
            }
			//refresh colors
			vtm.m_mesh.colors32 = vtm.m_vertexColors;
			vtm.ApplyMotionData();
                
            Undo.RecordObject(vtm,"Paint all");

        }


        void PaintAll(Color color, bool fill )
        {
            VertExmotionBase vtm = (target as VertExmotionBase);

        if (m_selectedMaterialSlotID != 0)
        {
            int[] tri = vtm.m_mesh.GetTriangles(m_selectedMaterialSlotID - 1);
            for (int i = 0; i < tri.Length; ++i)
            {
                Color c;
                c = vtm.m_vertexColors[tri[i] + 0];
                c.r = color.r > 0 ? (fill ? m_drawIntensity : 0) : c.r;
                c.g = color.g > 0 ? (fill ? m_drawIntensity : 0) : c.g;
                c.b = color.b > 0 ? (fill ? m_drawIntensity : 0) : c.b;
                vtm.m_vertexColors[tri[i]] = c;
                    
            }
        }
        else
        {
            for (int i = 0; i < vtm.m_vertexColors.Length; ++i)
            {

                Color c = vtm.m_vertexColors[i];
                c.r = color.r > 0 ? (fill ? m_drawIntensity : 0) : c.r;
                c.g = color.g > 0 ? (fill ? m_drawIntensity : 0) : c.g;
                c.b = color.b > 0 ? (fill ? m_drawIntensity : 0) : c.b;
                vtm.m_vertexColors[i] = c;
            }
        }
            //refresh colors
            vtm.m_mesh.colors32 = vtm.m_vertexColors;
            vtm.ApplyMotionData();
            //EditorUtility.SetDirty(vtm);
            Undo.RecordObject(vtm, "paint all");
        }



        /// <summary>
        /// Gets the name of the asset.
        /// </summary>
        /// <returns>The asset name.</returns>
        string GetAssetName()
		{
			VertExmotionBase vtm = (target as VertExmotionBase);
			string path = AssetDatabase.GetAssetPath ( vtm.m_mesh );

			SkinnedMeshRenderer smr = vtm.GetComponent<SkinnedMeshRenderer>();
			if (smr)			
				path = AssetDatabase.GetAssetPath ( smr.sharedMesh );


			//Debug.Log ("path " + path);
			string[] pathParts = path.Split('/');
			return pathParts [pathParts.Length - 1].Split('.')[0];
		}

		//-------------------------------------------------------------------
		void ExportMesh()
		{
			VertExmotionBase vtm = (target as VertExmotionBase);
			//Mesh initMesh = vtm.GetMesh ();
			Mesh m = (Mesh)Instantiate( vtm.m_mesh );
			m.name = vtm.m_mesh.name;
			string path = AssetDatabase.GetAssetPath ( vtm.m_mesh );

			//SkinnedMesh Setttings
			SkinnedMeshRenderer smr = vtm.GetComponent<SkinnedMeshRenderer>();
			if (smr)
			{
				path = AssetDatabase.GetAssetPath ( smr.sharedMesh );
				m = (Mesh)Instantiate( smr.sharedMesh );
				m.colors32 = vtm.m_mesh.colors32;
				m.name = smr.sharedMesh.name;
			}

			string[] pathParts = path.Split('/');
			path = "";
			for(int i=0; i<pathParts.Length-1; ++i)
				path += pathParts[i]+"/";

			if( path == "" || path == "Library/")//built-in models
				path = "Assets/";

			string prefabName = path + m_exportMeshName + "_VertExmotion.prefab";



			Object prefab = AssetDatabase.LoadAssetAtPath (prefabName, typeof(Object) );

            if(prefab != null)
            {
                if(EditorUtility.DisplayDialog("Template exists", "override this template?", "ok", "cancel"))
                {
                    DestroyImmediate(prefab, true);
                    prefab = null;
                }
                else
            {
                return;
            }
            }

			if( prefab == null )
			{
                GameObject tmp = new GameObject("VertExmotion template");
                prefab = PrefabUtility.CreatePrefab(prefabName, tmp);//disable the warning message
                //prefab = PrefabUtility.CreateEmptyPrefab ( prefabName );  
                AssetDatabase.AddObjectToAsset( m, prefabName );
				AssetDatabase.SaveAssets();
                DestroyImmediate(tmp);

                
                //apply new mesh
                vtm.SetMesh (m);
				vtm.InitMesh();
                
				EditorGUIUtility.PingObject ( prefab );
				Debug.Log ( "New mesh reference :\n" + prefabName );
			}
			else
			{
				Debug.LogError ( "Export mesh failed : " + prefabName + " exists\nPlease change template name" );
			}




			vtm.m_params.usePaintDataFromMeshColors = true;//enable mesh data driven


			//GUIUtility.ExitGUI ();

			//AssetDatabase.CreateAsset ( m, path + m.name);
			//AssetDatabase.Refresh ();
		}



		public static void ReplaceBaseClass( MonoBehaviour obj )
		{
			if( obj == null )
				return;

			var so = new SerializedObject( obj );
			var sp = so.FindProperty("m_Script");
			MonoScript script = GetScriptFromBase( obj.GetType().Name );
			if( script != null )  
			{
				sp.objectReferenceValue = script;
				so.ApplyModifiedProperties();                
			}            
		}




		static MonoScript GetScriptFromBase( string type )
		{
            if (!type.Contains("Base"))
                return null;

			var types = Resources
				.FindObjectsOfTypeAll (typeof(MonoScript))
					.Where (x => x.name == type.Replace("Base","") )
					//.Where (x => x.GetType () == typeof(MonoScript))
					.Cast<MonoScript> ()
					.Where (x => x.GetClass () != null)
					//.Where( x => x.GetClass().Assembly.FullName.Split(',')[0] == "" )
					.ToList ();


			if( types.Count == 1 )
			{				    
				return types[0];
			}
			return null;
		}



        void ExportWeightsToTexture()
        {
            VertExmotionBase vtm = target as VertExmotionBase;
            Vector2[] uv = vtm.m_mesh.uv;

            int tsize = 1024;
            Texture2D t = new Texture2D(tsize, tsize);


            for( int i=0; i<uv.Length; ++i )
            {
                int x = (int)( (float)tsize * uv[i].x );
                int y = (int)( (float)tsize * uv[i].y );
                t.SetPixel(x, y, vtm.m_vertexColors[i]);
            }
            t.Apply();

            string texturePath = vtm.m_mesh!=null? AssetDatabase.GetAssetPath(vtm.m_mesh):"/Assets";
            string filename = vtm.m_mesh != null ? vtm.m_mesh.name.Replace("(Clone)","") + "_VertExmotionData" : "VertExmotionData";

            string path = EditorUtility.SaveFilePanel(
               "Save texture as PNG",
               texturePath,
               filename + ".png",
               "png");

            if (path.Length == 0)
                return;


            

            byte[] bytes = t.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();

            path = path.Replace(Application.dataPath, "Assets");
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
            }
            else
            {
                Debug.LogError("Fail to set read/write flag");
            }
        }

        
    }
}
