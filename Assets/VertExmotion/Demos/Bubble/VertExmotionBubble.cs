using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Kalagaan
{

    public class VertExmotionBubble : MonoBehaviour
    {

        [HideInInspector]
        public VertExmotion m_vtm;
        [Range(0, 10)]
        public int m_sensorCount = 6;
        [Range(0, 1)]
        public float m_sensorPositionFromCenter = .8f;
        [Range(0, 1)]
        public float m_sensorMotionFactor = .8f;
        [Range(0, 1)]
        public float m_sensorRadius = .6f;        
        [Range(0, 1)]
        public float m_sensorMotionSmooth = .8f;
        [Range(0, 1)]
        public float m_deformationAmplitude = .3f;


        List<PID_V3> m_smoothPositions = new List<PID_V3>();

        void Start()
        {

            if (m_vtm == null)
                m_vtm = GetComponent<VertExmotion>();

            if (m_vtm == null)
            {
                //there's no VertExmotion component, let's add it
                m_vtm = gameObject.AddComponent<VertExmotion>();
                m_vtm.m_editMode = false;//close edition pannel

                //set the paint data
                MeshFilter mf = GetComponent<MeshFilter>();
                m_vtm.m_vertexColors = new Color32[mf.sharedMesh.vertexCount];
                for (int i = 0; i < m_vtm.m_vertexColors.Length; ++i)
                    m_vtm.m_vertexColors[i] = Color.white;//set the weight to 1 on each channel
                mf.mesh.colors32 = m_vtm.m_vertexColors;
            }

            for (int i = 0; i < m_sensorCount; ++i)
                CreateSensor();

        }

        void CreateSensor()
        {
            GameObject go = new GameObject("sensor");
            go.transform.parent = m_vtm.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            VertExmotionSensor s = go.AddComponent<VertExmotionSensor>();
            s.m_params.translation.amplitudeMultiplier = m_sensorMotionFactor;
            //we want a smooth interraction from the object motion
            s.m_params.damping = 1f;
            s.m_params.bouncing = .1f;
            m_vtm.m_VertExmotionSensors.Add(s);

            //add a smooth object for computing the position
            PID_V3 smooth = new PID_V3();
            smooth.m_params.kp = 1f;//damping
            smooth.m_params.ki = .1f;//bouncing

            if (m_smoothPositions.Count < m_vtm.m_VertExmotionSensors.Count)
                m_smoothPositions.Add(smooth);
        }


        void Update()
        {
            float scale = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3f;
            for (int i = 0; i < m_vtm.m_VertExmotionSensors.Count; ++i)
            {
                m_vtm.m_VertExmotionSensors[i].m_envelopRadius = m_sensorRadius;

                //set a new random position in the sphere
                Vector3 newPos = Random.insideUnitSphere.normalized * m_sensorPositionFromCenter;

                //smooth it and assign the new local pos            
                m_smoothPositions[i].m_target = Vector3.Lerp(newPos, m_vtm.m_VertExmotionSensors[i].transform.localPosition, m_sensorMotionSmooth);
                m_vtm.m_VertExmotionSensors[i].transform.localPosition = m_smoothPositions[i].Compute(m_vtm.m_VertExmotionSensors[i].transform.localPosition);
                m_vtm.m_VertExmotionSensors[i].m_params.translation.amplitudeMultiplier = m_sensorMotionFactor;
                //set the deformation offset out of the sphere, based on the sensor postion.
                m_vtm.m_VertExmotionSensors[i].m_params.translation.localOffset = m_vtm.m_VertExmotionSensors[i].transform.localPosition * m_deformationAmplitude * scale;
            }

            while (m_vtm.m_VertExmotionSensors.Count < m_sensorCount)
            {
                CreateSensor();
            }


            while (m_vtm.m_VertExmotionSensors.Count > m_sensorCount)
            {
                VertExmotionSensorBase s = m_vtm.m_VertExmotionSensors[m_vtm.m_VertExmotionSensors.Count - 1];
                m_vtm.m_VertExmotionSensors.Remove(s);
                Destroy(s.gameObject);
            }
        }
    }
}