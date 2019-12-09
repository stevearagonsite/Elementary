using UnityEngine;
using System.Collections;


namespace Kalagaan
{
    [ExecuteInEditMode]
    public class SplashScript : MonoBehaviour
    {

        Renderer m_r;
        public Transform m_plane;
        public float m_spashAmplitude = .1f;
        public float m_spashMinDist = .3f;
        [Range(0, 1)]
        public float m_splashDeformation = .5f;

        void Awake()
        {
            m_r = GetComponent<MeshRenderer>();
        }

        void LateUpdate()
        {

            if (m_plane != null)
            {
                m_r.sharedMaterial.SetVector("planePoint", m_plane.position);
                m_r.sharedMaterial.SetVector("planeNormal", m_plane.up);
                m_r.sharedMaterial.SetFloat("splashAmplitude", m_spashAmplitude);
                m_r.sharedMaterial.SetFloat("splashMinDist", m_spashMinDist);
                m_r.sharedMaterial.SetFloat("splashDeformation", m_splashDeformation);
            }

        }

    }
}