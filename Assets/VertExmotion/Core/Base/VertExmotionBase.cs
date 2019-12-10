using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Kalagaan
{
	//[AddComponentMenu("VertExmotion/VertExmotion")]
	
	/// VertExmotion is the main class.
	/// require MeshFilter or SkinMeshRenderer
    //[ExecuteInEditMode]
	public class VertExmotionBase : MonoBehaviour {		

		// VertExmotion version
		public static string version = "1.8.5";
        ///version of the instance
        public string m_version = "1.8.5";

        /// number of sensor available by material
        /// limit of 6 for shader model 2 (mobile compatibility)
        public static int MAX_SENSOR = 20;

		[HideInInspector]
		public string className = "VertExmotion";

        public bool m_executeOnLateUpdate = true;
        public bool m_dontCheckShaderCompatibility = false;
        public bool m_showMaterialIDWireFrame = false;

        [System.Serializable]
		public class Parameters
		{
			///set to true on first mesh save
			///compatibility with VertExmotion before 1.2.3
			public bool usePaintDataFromMeshColors = false;

			
		}


		//[HideInInspector]
		public Parameters m_params = new Parameters();
		

		/// <summary>
		/// material property block.
		/// </summary>
		[HideInInspector] public MaterialPropertyBlock m_matPropBlk;		

		///mesh reference
		///meshFilter or SkinMeshRenderer
		[System.NonSerialized] public Mesh m_mesh;

		///Sensors list \n
		///sensor parameters are sent to shader each Update
		[HideInInspector] public List<VertExmotionSensorBase> m_VertExmotionSensors = new List<VertExmotionSensorBase>();

        /// 
        ///
        [HideInInspector] public List<float> m_sensorsLinks = new List<float>();

        ///vertices weights\n
        /// 0->static 1->softboby\n
        ///only green parameter is used\n
        /// todo : add 3 other weight layers using RBA channels
        [HideInInspector] public Color32[] m_vertexColors;

		///editor use only
		///used to switch current shader by editor shader 
		[HideInInspector] public Shader[] m_initialShaders;
		///shared material
		/// WIP
		//[HideInInspector]
		//public bool m_shareMaterial = false;
		///shared mesh
		///WIP

		//public bool m_sharedMesh = true;
		[HideInInspector] public bool m_shareMesh = false;
		///mesh copy
		/// WIP 
		[HideInInspector] public bool m_meshCopy = false;
		///Shader parameters names dictionnary
		///disable string allocation for shader array name
		[HideInInspector] public Dictionary<string,List<string>> m_shaderParamNames = new Dictionary<string, List<string>> ();

        

		static Dictionary<Mesh,Mesh> m_meshPool = new Dictionary<Mesh, Mesh> ();
        [HideInInspector]
        public bool m_showEditorPanel = true;
        public bool m_editMode = true;

        //Unity 5.4 array
        [HideInInspector]
        public Vector4[] m_shaderSensorPos;
        [HideInInspector]
        public Vector4[] m_shaderMotionDirection;
        [HideInInspector]
        public Vector4[] m_shaderRCT;
        [HideInInspector]
        public Vector4[] m_shaderSquashStrech;
        [HideInInspector]
        public Vector4[] m_shaderSpeed;
        [HideInInspector]
        public Vector4[] m_shaderMotionAxis;
        [HideInInspector]
        public Vector4[] m_shaderAxisScaleX;
        [HideInInspector]
        public Vector4[] m_shaderAxisScaleY;
        [HideInInspector]
        public Vector4[] m_shaderAxisScaleZ;
        [HideInInspector]
        public float[] m_shaderLink;


        public float m_normalCorrection = 0f;
        public float m_normalSmooth = .2f;

        //Unity 5 compatibility
        Renderer m_renderer;
		public new Renderer renderer
		{
			get{
				if( m_renderer==null )
					m_renderer = GetComponent<Renderer> ();
				return m_renderer;
			}
		}		


		void Awake()
		{
            
            m_matPropBlk = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(m_matPropBlk);
            CleanShaderProperties();
            //renderer.material = new Material(renderer.material.shader);
            //renderer.GetPropertyBlock(m_matPropBlk);
            //m_matPropBlk.Clear();
            

            InitShaderParam ();
            DisableMotion();

            Collider c = GetComponent<Collider>();
            if(c!=null)
                IgnoreCollision(c, true);



            if ( !m_params.usePaintDataFromMeshColors && Application.isPlaying )
			{				
				ApplyMotionData ();
			}
			
		}
		
		
		public static float GetScaleFactor( Transform t )
		{            
            return (t.lossyScale.x + t.lossyScale.y + t.lossyScale.z) / 3f;
            //return t.lossyScale.magnitude;
        }




        void InitShaderParam()
		{

            //precompute shader param name to avoid string memory allocation by frame
            //"_SensorPosition" + i -> 490B allocated

            //not needed in unity 5.4
            /*
            m_shaderParamNames.Add ("_SensorPosition", new List<string> ());
			m_shaderParamNames.Add ("_MotionDirection", new List<string> ());
			m_shaderParamNames.Add ("_MotionAxis", new List<string> ());
			m_shaderParamNames.Add ("_RadiusCentripetalTorque", new List<string> ());
			m_shaderParamNames.Add ("_SquashStretch", new List<string> ());
			m_shaderParamNames.Add ("_Speed", new List<string> ());
			//m_shaderParamNames.Add ("_MotionZoneID", new List<string> ());
			
			
			foreach( KeyValuePair<string,List<string>> kvp in m_shaderParamNames )
				for (int i=0; i<MAX_SENSOR; ++i)
					kvp.Value.Add( kvp.Key + i );
            */

            

            //int size = m_VertExmotionSensors.Count > 0 ? m_VertExmotionSensors.Count : 1;
            int size = MAX_SENSOR;

            //unity 5.4 shader array
            m_shaderSensorPos = new Vector4[size];
            m_shaderMotionDirection = new Vector4[size];
            m_shaderRCT = new Vector4[size];
            m_shaderSquashStrech = new Vector4[size];
            m_shaderSpeed = new Vector4[size];
            m_shaderMotionAxis = new Vector4[size];
            m_shaderAxisScaleX = new Vector4[size];
            m_shaderAxisScaleY = new Vector4[size];
            m_shaderAxisScaleZ = new Vector4[size];
            m_shaderLink = new float[size];

            for (int i = 0; i < m_shaderSensorPos.Length; ++i)
            {
                m_shaderSensorPos[i] = Vector4.zero;
                m_shaderMotionDirection[i] = Vector4.zero;
                m_shaderRCT[i] = Vector4.zero;
                m_shaderSquashStrech[i] = Vector4.zero;
                m_shaderSpeed[i] = Vector4.zero;
                m_shaderMotionAxis[i] = Vector4.zero;
                m_shaderAxisScaleX[i] = new Vector4(1f,0f,0f,1f);
                m_shaderAxisScaleY[i] = new Vector4(0f,1f,0f,1f);
                m_shaderAxisScaleZ[i] = new Vector4(0f,0f,1f,1f);
                m_shaderLink[i] = 0;
            }


            if (m_matPropBlk == null)
            {
                m_matPropBlk = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(m_matPropBlk);
            }            

            m_matPropBlk.SetVectorArray("KVM_SensorPosition", m_shaderSensorPos);
            m_matPropBlk.SetVectorArray("KVM_MotionDirection", m_shaderMotionDirection);
            m_matPropBlk.SetVectorArray("KVM_RadiusCentripetalTorque", m_shaderRCT);
            m_matPropBlk.SetVectorArray("KVM_SquashStretch", m_shaderSquashStrech);
            m_matPropBlk.SetVectorArray("KVM_Speed", m_shaderSpeed);
            m_matPropBlk.SetVectorArray("KVM_MotionAxis", m_shaderMotionAxis);
            m_matPropBlk.SetVectorArray("KVM_AxisXScale", m_shaderAxisScaleX);
            m_matPropBlk.SetVectorArray("KVM_AxisYScale", m_shaderAxisScaleY);
            m_matPropBlk.SetVectorArray("KVM_AxisZScale", m_shaderAxisScaleZ);
            m_matPropBlk.SetFloatArray("KVM_link", m_shaderLink);
            //m_matPropBlk.Clear();
            renderer.SetPropertyBlock(m_matPropBlk);

        }
		
		


		public Mesh GetMesh()
		{
			MeshFilter mf = GetComponent<MeshFilter> ();
			if (mf) {
				if( Application.isPlaying )
					return mf.mesh;
				else
					return mf.sharedMesh;
			}
			SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
			if (smr)
			{
				if( Application.isPlaying )
					return smr.sharedMesh;
				else				
					return smr.sharedMesh;
			}

			return null;
		}



		public void SetMesh( Mesh m )
		{
			MeshFilter mf = GetComponent<MeshFilter> ();
			if (mf) {
				if( Application.isPlaying )
					mf.mesh = m;
				else
					mf.sharedMesh = m;
			}
			SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
			if (smr)
			{
				smr.sharedMesh = m;
			}
		}


		
		/// Check render on current gameObject. \n
		/// Initialise m_mesh reference.
		public void InitMesh()
		{
            if (renderer == null)
                return;

			Material[] materials;
			if( Application.isPlaying )
				materials = renderer.materials;
			else
				materials = renderer.sharedMaterials;

			if( m_initialShaders == null || ( m_initialShaders.Length != materials.Length ) )
			{
				m_initialShaders = new Shader[materials.Length];
				for( int i=0; i<m_initialShaders.Length; ++i )
					m_initialShaders[i] = materials[i].shader;
			}

			m_meshCopy = false;
			MeshFilter mf = GetComponent<MeshFilter> ();
			if (mf) {
				if( Application.isPlaying )
					m_mesh = mf.mesh;
				else
					m_mesh = mf.sharedMesh;
			}
			SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
			if (smr)
			{
                if (Application.isPlaying)
                {
                    m_mesh = smr.sharedMesh;
                }
                else if(smr.sharedMesh != null)
                {
                    m_mesh = Instantiate(smr.sharedMesh) as Mesh;
                    smr.BakeMesh(m_mesh);
                    m_meshCopy = true;
                }
			}



            /*
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr)
            {
                if (Application.isPlaying)
                    m_mesh = sr.sprite.vertices;
                else
                {
                    m_mesh = Instantiate(smr.sharedMesh) as Mesh;
                    smr.BakeMesh(m_mesh);
                    m_meshCopy = true;
                }
            }*/

            DisableMotion ();
		}
		
		
		
		public void ApplyMotionData()
		{
			MeshFilter mf = GetComponent<MeshFilter> ();
			if (mf) {
				if( Application.isPlaying )
				{
					if( m_shareMesh )
					{
						Mesh refMesh = mf.sharedMesh;
						if( !m_meshPool.ContainsKey(refMesh) )
						{
							//store mesh in Mesh pool
							mf.mesh.colors32 = m_vertexColors;
							m_meshPool.Add( refMesh, mf.mesh );
							//Debug.Log("Add mesh to Mesh pool");
						}
						else
						{
							//get mesh from Mesh pool
							mf.mesh = m_meshPool[refMesh];
							//Debug.Log("get mesh from Mesh pool");
						}
					}
					else
					{
						//create a copy in memory
						mf.mesh.colors32 = m_vertexColors;
					}
				}				
			}
			SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
			if (smr)
			{
                if (m_vertexColors != null && m_vertexColors.Length == smr.sharedMesh.vertexCount)
                {
                    if( Application.isPlaying )
                        smr.sharedMesh = Instantiate(smr.sharedMesh) as Mesh;                    
                    smr.sharedMesh.colors32 = m_vertexColors;
                }
			}
		}
		
		
		
		/// Check vertex color array. \n
		/// Initialise vertices weights.
		public void InitVertices()
		{                

            InitMesh();
            
            if (m_mesh == null)
                return;
  
			if ( m_vertexColors == null || m_vertexColors.Length  == 0 || m_vertexColors.Length != m_mesh.colors.Length)
			{
				if( m_mesh.colors.Length == 0 )
				{
                    if(m_vertexColors ==null || m_vertexColors.Length != m_mesh.vertexCount)
					    m_vertexColors = new Color32[m_mesh.vertices.Length];//create new colors informations
					m_mesh.colors32 = m_vertexColors;
				}
				else
					m_vertexColors = m_mesh.colors32;//copy colors from mesh


			}


		}



        public void Update()
        {
            if (!m_executeOnLateUpdate)
                UpdateShaders();
        }

        public void LateUpdate()
        {
            if (m_executeOnLateUpdate)
                UpdateShaders();
        }

       
        public void UpdateShaders ()
		{
            //m_matPropBlk.Clear();

            /*
            if (m_shaderSensorPos.Length != m_VertExmotionSensors.Count)
                InitShaderParam();
            */
            

            if (!Application.isPlaying)
			{
                //editor update
                return;
			}


#if VERTEXMOTION_TRIAL
            if (!Application.isEditor)
                return;
#endif


			if(m_matPropBlk==null)
				m_matPropBlk = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(m_matPropBlk);


            /*
             //-------------------------------------------------------------------
            //UNITY < 5.4

                        //set shader parameters array
                        for (int i=0; i<m_VertExmotionSensors.Count; ++i)
                        {
                            if( i==MAX_SENSOR )
                                break;

                            m_matPropBlk.SetVector ( m_shaderParamNames["_SensorPosition"][i], m_VertExmotionSensors[i].transform.position );
                            m_matPropBlk.SetVector( m_shaderParamNames["_MotionDirection"][i], m_VertExmotionSensors[i].m_motionDirection );

                            Vector4 radiusCentripetalTorque = Vector4.zero;
                            radiusCentripetalTorque.x = m_VertExmotionSensors[i].m_envelopRadius*GetScaleFactor( m_VertExmotionSensors[i].transform );
                            radiusCentripetalTorque.y = m_VertExmotionSensors[i].m_centripetalForce*GetScaleFactor( m_VertExmotionSensors[i].transform );
                            radiusCentripetalTorque.z = m_VertExmotionSensors[i].m_motionTorqueForce;

                            Vector4 squashStrech = Vector4.zero;
                            squashStrech.x = m_VertExmotionSensors[i].m_params.fx.squash;
                            squashStrech.y = m_VertExmotionSensors[i].m_stretch;


                            m_matPropBlk.SetVector( m_shaderParamNames["_RadiusCentripetalTorque"][i], radiusCentripetalTorque );
                            m_matPropBlk.SetVector( m_shaderParamNames["_SquashStretch"][i], squashStrech );
                            m_matPropBlk.SetVector( m_shaderParamNames["_Speed"][i], m_VertExmotionSensors[i].m_speedStrech );

                            m_matPropBlk.SetVector( m_shaderParamNames["_MotionAxis"][i], m_VertExmotionSensors[i].m_torqueAxis );
                        }
            */


            //-------------------------------------------------------------------
            //UNITY 5.4

            

            //for (int i = 0; i < m_VertExmotionSensors.Count; ++i)            
            for (int i = 0; i < MAX_SENSOR; ++i)
            {   

                if (i < m_VertExmotionSensors.Count)
                {

                    if (m_VertExmotionSensors[i] == null)
                    {
                        m_VertExmotionSensors.RemoveAt(i--);
                        continue;
                    }

                    m_shaderSensorPos[i] = m_VertExmotionSensors[i].transform.position;
                    m_shaderSensorPos[i].w = m_VertExmotionSensors[i].m_layerID;
                    m_shaderMotionDirection[i] = m_VertExmotionSensors[i].m_motionDirection;
                    m_shaderMotionDirection[i].w = m_VertExmotionSensors[i].m_params.power;

                    Vector4 radiusCentripetalTorque = Vector4.zero;
                    radiusCentripetalTorque.x = m_VertExmotionSensors[i].m_envelopRadius * GetScaleFactor(m_VertExmotionSensors[i].transform);
                    radiusCentripetalTorque.y = m_VertExmotionSensors[i].m_centripetalForce * GetScaleFactor(m_VertExmotionSensors[i].transform);
                    radiusCentripetalTorque.z = m_VertExmotionSensors[i].m_motionTorqueForce;
                    m_shaderRCT[i] = radiusCentripetalTorque;

                    m_shaderSquashStrech[i].x = m_VertExmotionSensors[i].m_params.fx.squash;
                    m_shaderSquashStrech[i].y = m_VertExmotionSensors[i].m_stretch;

                    m_shaderSpeed[i] = m_VertExmotionSensors[i].m_speedStrech;
                    m_shaderMotionAxis[i] = m_VertExmotionSensors[i].m_torqueAxis;

                    Vector3 axis = m_VertExmotionSensors[i].transform.right;
                    m_shaderAxisScaleX[i].x = axis.x;
                    m_shaderAxisScaleX[i].y = axis.y;
                    m_shaderAxisScaleX[i].z = axis.z;
                    m_shaderAxisScaleX[i].w = m_VertExmotionSensors[i].m_params.scale.x;

                    axis = m_VertExmotionSensors[i].transform.up;
                    m_shaderAxisScaleY[i].x = axis.x;
                    m_shaderAxisScaleY[i].y = axis.y;
                    m_shaderAxisScaleY[i].z = axis.z;
                    m_shaderAxisScaleY[i].w = m_VertExmotionSensors[i].m_params.scale.y;

                    axis = m_VertExmotionSensors[i].transform.forward;
                    m_shaderAxisScaleZ[i].x = axis.x;
                    m_shaderAxisScaleZ[i].y = axis.y;
                    m_shaderAxisScaleZ[i].z = axis.z;
                    m_shaderAxisScaleZ[i].w = m_VertExmotionSensors[i].m_params.scale.z;

                    if(i<m_sensorsLinks.Count)
                        m_shaderLink[i] = m_sensorsLinks[i];
                }
                else
                {
                    m_shaderSensorPos[i] = Vector4.zero;
                    m_shaderMotionDirection[i] = Vector4.zero;
                    m_shaderRCT[i] = Vector4.zero;
                    m_shaderSquashStrech[i] = Vector4.zero;
                    m_shaderSpeed[i] = Vector4.zero;
                    m_shaderMotionAxis[i] = Vector4.zero;
                    m_shaderAxisScaleX[i] = new Vector4(1f, 0f, 0f, 1f);
                    m_shaderAxisScaleY[i] = new Vector4(0f, 1f, 0f, 1f);
                    m_shaderAxisScaleZ[i] = new Vector4(0f, 0f, 1f, 1f);
                }

            }

            m_matPropBlk.SetFloat("KVM_NbSensors", m_VertExmotionSensors.Count);
            m_matPropBlk.SetVectorArray("KVM_SensorPosition", m_shaderSensorPos);
            m_matPropBlk.SetVectorArray("KVM_MotionDirection", m_shaderMotionDirection);
            m_matPropBlk.SetVectorArray("KVM_RadiusCentripetalTorque", m_shaderRCT);
            m_matPropBlk.SetVectorArray("KVM_SquashStretch", m_shaderSquashStrech);
            m_matPropBlk.SetVectorArray("KVM_Speed", m_shaderSpeed);
            m_matPropBlk.SetVectorArray("KVM_MotionAxis", m_shaderMotionAxis);
            m_matPropBlk.SetVectorArray("KVM_AxisXScale", m_shaderAxisScaleX);
            m_matPropBlk.SetVectorArray("KVM_AxisYScale", m_shaderAxisScaleY);
            m_matPropBlk.SetVectorArray("KVM_AxisZScale", m_shaderAxisScaleZ);
            m_matPropBlk.SetFloatArray("KVM_Link", m_shaderLink);

            m_matPropBlk.SetFloat("KVM_NormalCorrection", m_normalCorrection);
            m_matPropBlk.SetFloat("KVM_NormalSmooth", m_normalSmooth);

            //-------------------------------------------------------------------

            renderer.SetPropertyBlock ( m_matPropBlk);


			
		}
		
		/// Clear all motion data. \n
		/// Send all parameters to shader.	
		public void DisableMotion()
		{
            CleanShaderProperties();
            return;

   //         if ( !Application.isPlaying )
   //         {
   //             //renderer.SetPropertyBlock(new MaterialPropertyBlock());
   //             return;
   //         }
			
			//if (m_shaderSensorPos.Length != m_VertExmotionSensors.Count)
			//	InitShaderParam ();

   //         //m_matPropBlk.Clear();
   //         if (m_matPropBlk == null)
   //             m_matPropBlk = new MaterialPropertyBlock();
   //         //renderer.GetPropertyBlock(m_matPropBlk);

   //         /*
   //         //  < unity 5.4
   //         for (int i=0; i<m_VertExmotionSensors.Count; ++i)
			//{

			//	m_matPropBlk.SetVector( m_shaderParamNames["_MotionDirection"][i], Vector4.zero );
			//	m_matPropBlk.SetVector( m_shaderParamNames["_MotionAxis"][i], Vector4.zero );
			//	m_matPropBlk.SetVector( m_shaderParamNames["_RadiusCentripetalTorque"][i], Vector4.zero );				
			//}
   //         */

   //         //unity 5.4
   //         /*
   //         m_shaderSensorPos = new Vector4[MAX_SENSOR];
   //         m_shaderMotionDirection = new Vector4[MAX_SENSOR];
   //         m_shaderRCT = new Vector4[MAX_SENSOR];
   //         m_shaderSquashStrech = new Vector4[MAX_SENSOR];
   //         m_shaderSpeed = new Vector4[MAX_SENSOR];
   //         m_shaderMotionAxis = new Vector4[MAX_SENSOR];
   //         */

   //         //Debug.Log("disable sensors " + m_VertExmotionSensors.Count);

   //         for (int i = 0; i < m_shaderSensorPos.Length; ++i)
   //         //for (int i = 0; i < MAX_SENSOR; ++i)
   //         {
   //             m_shaderSensorPos[i] = Vector4.zero;
   //             m_shaderMotionDirection[i] = Vector4.zero;
   //             m_shaderRCT[i] = Vector4.zero;
   //             m_shaderSquashStrech[i] = Vector4.zero;
   //             m_shaderSpeed[i] = Vector4.zero;
   //             m_shaderMotionAxis[i] = Vector4.zero;

   //         }
            
   //         m_matPropBlk.SetVectorArray("_SensorPosition", m_shaderSensorPos);
   //         m_matPropBlk.SetVectorArray("_MotionDirection", m_shaderMotionDirection);
   //         m_matPropBlk.SetVectorArray("_RadiusCentripetalTorque", m_shaderRCT);
   //         m_matPropBlk.SetVectorArray("_SquashStretch", m_shaderSquashStrech);
   //         m_matPropBlk.SetVectorArray("_Speed", m_shaderSpeed);
   //         m_matPropBlk.SetVectorArray("_MotionAxis", m_shaderMotionAxis);
            

   //         //m_matPropBlk.Clear();
   //         renderer.SetPropertyBlock (m_matPropBlk);
            



        }



        public void CleanShaderProperties()
        {

            //Debug.Log("CleanShaderProperties");
            
            m_matPropBlk = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(m_matPropBlk);
            m_matPropBlk.SetVectorArray("_SensorPosition", new Vector4[MAX_SENSOR]);
            m_matPropBlk.SetVectorArray("_MotionDirection", new Vector4[MAX_SENSOR]);
            m_matPropBlk.SetVectorArray("_RadiusCentripetalTorque", new Vector4[MAX_SENSOR]);
            m_matPropBlk.SetVectorArray("_SquashStretch", new Vector4[MAX_SENSOR]);
            m_matPropBlk.SetVectorArray("_Speed", new Vector4[MAX_SENSOR]);
            m_matPropBlk.SetVectorArray("_MotionAxis", new Vector4[MAX_SENSOR]);
            renderer.SetPropertyBlock(m_matPropBlk);
        }




        /// <summary>
        /// Sets the time scale on each sensor.
        /// </summary>
        /// <param name="timeScale">Time scale.</param>
        public void SetTimeScale( float timeScale )
		{
			for (int i=0; i<m_VertExmotionSensors.Count; ++i)
				m_VertExmotionSensors[i].timeScale = timeScale;
		}



		//System.Type t;
		public VertExmotionSensorBase CreateSensor()
		{
			GameObject go = new GameObject("VertExmotion Sensor");
			VertExmotionSensorBase vms = go.AddComponent<VertExmotionSensorBase>();
			go.transform.parent = transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
			vms.m_parent = transform;
			//m_VertExmotionSensors.Add( vms );

			return vms;
		}






		/// <summary>
		/// ClassExists.
		/// </summary>
		/// <returns><c>true</c>, if class Exist, <c>false</c> otherwise.</returns>
		/// <param name="className">Class name.</param>
		public static bool ClassExists( string className )
		{
			System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
			
			foreach (var A in assemblies)
			{
				System.Type[] types = A.GetTypes();
				foreach (var T in types)				
					if (T.Name == className )					
						return true;
				
			}
			return false;
		}
		
		
		public static System.Type ClassType( string className )
		{
			System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
			
			foreach (var A in assemblies)
			{
				System.Type[] types = A.GetTypes();
				foreach (var T in types)				
					if (T.Name == className )					
						return T;
				
			}
			return null;
		}

        
        void OnDestroy()
        {
            //Debug.Log("OnDestroy");    
            DisableMotion();
            
        }


        ///Ignore current frame motion.
        public void IgnoreFrame()
        {
            for (int i = 0; i < m_VertExmotionSensors.Count; ++i)
                m_VertExmotionSensors[i].IgnoreFrame();
        }


        ///Reset motion.
        public void ResetMotion()
        {
            for (int i = 0; i < m_VertExmotionSensors.Count; ++i)
                m_VertExmotionSensors[i].ResetMotion();
        }


        /// <summary>
        /// Ignore collider from colliding with sensors' collision zones
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="ignore"></param>
        public void IgnoreCollision(Collider collider, bool ignore)
        {
            for (int i = 0; i < m_VertExmotionSensors.Count; ++i)
                m_VertExmotionSensors[i].IgnoreCollision(collider,ignore);
        }

    }

   



}

/* Documentation tag for Doxygen
 */
/*! \mainpage VertExmotion Documentation
 *
 * <img src=VertExmotion.png >
 *
 */
