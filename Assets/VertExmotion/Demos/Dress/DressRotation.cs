using UnityEngine;
using System.Collections;

namespace Kalagaan
{
    public class DressRotation : MonoBehaviour
    {

        public VertExmotionSensor m_rotationSensor;
        public float m_rotationFactor = 10f;
        public float m_rotationDamping = 1f;
        public float m_rotationBouncing = 1f;
        public float m_maxAngle = 90f;
        public float m_rotationToInflate = .1f;
        public float m_inflateDamping = 1f;
        public float m_inflateBouncing = 1f;

        Quaternion m_lastRotation;

        public float m_angle = 0f;
        public float m_inflate = 0f;
        Vector3 m_lastUp;

        PID anglePid = new PID();
        PID inflatePid = new PID();

        void Start()
        {
            m_lastRotation = m_rotationSensor.transform.rotation;
            m_lastUp = m_rotationSensor.transform.up;
        }

        
        void Update()
        {
            float newAngle = Quaternion.Angle(m_rotationSensor.transform.rotation, m_lastRotation);
            Vector3 c1 = Vector3.Cross(m_lastUp, m_rotationSensor.transform.up);
            newAngle *= Vector3.Dot(c1.normalized, m_rotationSensor.transform.forward) > 0f ? 1f : -1f;

            anglePid.m_target = - newAngle * m_rotationFactor;
            anglePid.m_params.kp = m_rotationDamping;
            anglePid.m_params.ki = m_rotationBouncing;
            anglePid.m_params.limits.x = -m_maxAngle;
            anglePid.m_params.limits.y = m_maxAngle;
            m_angle = anglePid.Compute(0f) ;

            inflatePid.m_target = Mathf.Abs(newAngle) * m_rotationToInflate;
            inflatePid.m_params.kp = m_inflateDamping;
            inflatePid.m_params.ki = m_inflateBouncing;
            inflatePid.m_params.limits.x = 0;
            inflatePid.m_params.limits.y = 1;
            m_inflate = inflatePid.Compute(0f);

            m_rotationSensor.m_params.rotation.angle = m_angle;
            m_rotationSensor.m_params.inflate = m_inflate;

            m_lastRotation = m_rotationSensor.transform.rotation;
            m_lastUp = m_rotationSensor.transform.up;

        }
    }
}