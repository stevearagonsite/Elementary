using UnityEngine;
using System.Collections;


namespace Kalagaan
{

    [RequireComponent(typeof(VertExmotionSensor))]
    public class VertExmotionInteraction : MonoBehaviour
    {


        public enum eInteractionType
        {
            DIRECTION,
            PUSH,
            PULL
        }

        public VertExmotion m_target;
        public float m_radius = .1f;
        public eInteractionType m_interactionType;

        VertExmotionSensor m_sensor;

        // Use this for initialization
        void Start()
        {

            m_sensor = GetComponent<VertExmotionSensor>();
            m_sensor.m_params.translation.amplitudeMultiplier = 0;

            if (m_target != null)
            {
                m_target.m_VertExmotionSensors.Add(m_sensor);
            }

        }

        public void ChangeTarget(VertExmotion newtarget)
        {
            if (m_target != null)
                m_target.m_VertExmotionSensors.Remove(m_sensor);

            m_target = newtarget;

            if (m_target != null)
                m_target.m_VertExmotionSensors.Add(m_sensor);
        }


        // Update is called once per frame
        void Update()
        {

            float scale = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3f;

            switch (m_interactionType)
            {
                case eInteractionType.DIRECTION:
                    m_sensor.m_params.translation.worldOffset = m_radius * scale * transform.forward;
                    break;

                case eInteractionType.PUSH:
                    m_sensor.m_params.translation.worldOffset = m_radius * scale * (m_target.transform.position - transform.position).normalized;
                    break;

                case eInteractionType.PULL:
                    m_sensor.m_params.translation.worldOffset = m_radius * scale * (transform.position - m_target.transform.position).normalized;
                    break;
            }
            m_sensor.m_envelopRadius = m_radius;

        }
    }
}