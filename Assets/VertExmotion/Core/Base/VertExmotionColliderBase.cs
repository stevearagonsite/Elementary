using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kalagaan
{
    /// Compute collision
    /// require a VertExmotionSensor component
    //[RequireComponent(typeof(VertExmotionSensor))]
    public class VertExmotionColliderBase : MonoBehaviour
    {

        [System.Serializable]
        public class CollisionZone
        {
            public Vector3 positionOffset = Vector3.zero;
            public float radius = 1f;
            [HideInInspector]
            public Vector3 collisionVector = Vector3.zero;
            [HideInInspector]
            public RaycastHit[] hits;
        }


        [HideInInspector]
        public string className = "VertExmotionCollider";

        ///Layer mask for physic interactions
        /// 
        public LayerMask m_layerMask = -1;

        ///Smooth factor
        /// 0 : no smooth
        /// 1 : realistic reaction with inertia
        /// 10 : low reaction to physic
        public float m_smooth = 1f;

        /// Enable wobble fx                
        public bool m_wobble = false;
        public float m_damping = 1f;
        public float m_bouncing = 1f;
        public float m_limit = 1f;

        ///List of CollisionZone
        ///add several zone to fit mesh volume
        public List<CollisionZone> m_collisionZones = new List<CollisionZone>();
        //float m_collisionScaleFactor = 1f;
        public List<Collider> m_ignoreColliders = new List<Collider>();

        public bool m_maximiZeSphereCollision;

        VertExmotionSensorBase m_sensor;

        Collider[] m_hitColliders = new Collider[100];
        RaycastHit[] m_hitResult = new RaycastHit[100];

        public PID_V3 m_pid = new PID_V3();
        Vector3 m_smoothTarget;

        void Start()
        {
            m_sensor = GetComponentInParent<VertExmotionSensorBase>();
            if (m_sensor == null)
            {
                enabled = false;
                Debug.LogError("VertExmotion collider must be a child of a sensor");
            }

            //check on larger sphere to get better precision for unity 4
            //if (Application.unityVersion.StartsWith("4"))
            //   m_collisionScaleFactor = 100f;

            
            m_pid.Init();
        }


        /// <summary>
        /// Ignore collider from colliding with collision zone
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="ignore"></param>
        public void IgnoreCollision( Collider collider, bool ignore )
        {
            if (ignore && !m_ignoreColliders.Contains(collider))
                m_ignoreColliders.Add(collider);

            if( !ignore && m_ignoreColliders.Contains(collider))
                m_ignoreColliders.Remove(collider);
        }


        //void LateUpdate()
        void FixedUpdate()
        {
            Vector3 target = Vector3.zero;
            float count = 0;            
            
            for (int i = 0; i < m_collisionZones.Count; ++i)
            {
                UpdateCollisionZone(m_collisionZones[i]);
                if (m_collisionZones[i].collisionVector != Vector3.zero)
                {
                    if (target == Vector3.zero)
                    {
                        target = m_collisionZones[i].collisionVector;
                    }
                    else
                    {
                        //if( target.magnitude < m_collisionZones[i].collisionVector.magnitude )
                        //  target = m_collisionZones[i].collisionVector;
                        target = Vector3.Lerp( m_collisionZones[i].collisionVector, target, target.magnitude / (target.magnitude+ m_collisionZones[i].collisionVector.magnitude) );
                    }
                    count++;
                }
            }

            if (count > 1)
                target /= count;

            m_smoothTarget = Vector3.Lerp(m_smoothTarget, target, m_sensor.deltaTime * 100f);
            //m_smoothTarget = target;

            //set target
            m_smooth = Mathf.Clamp(m_smooth, 0, 10f);

            
            //if (target.magnitude == 0 && m_wobble )
            if(m_wobble)
            {
                //wobble

                if (target.magnitude != 0)//collision On
                {
                    m_pid.m_params.kp = m_damping * 10f;
                    m_pid.m_params.ki = m_bouncing * .1f;
                    //wobble to target
                    m_pid.m_target = m_sensor.m_collision;
                }
                else//collision off
                {
                    m_pid.m_params.kp = m_damping;
                    m_pid.m_params.ki = m_bouncing;

                    if (m_smooth > 0f)
                        m_pid.m_target = Vector3.Lerp(m_sensor.m_collision, m_smoothTarget, m_sensor.deltaTime * (10f / m_smooth));
                    else
                        m_pid.m_target = Vector3.zero;
                }
                

                m_pid.m_params.limits.x = -m_limit;
                m_pid.m_params.limits.y = m_limit;
                
                
                m_sensor.m_collision = m_pid.Compute(Vector3.zero);
            }
            

            //if(target.magnitude != 0 || !m_wobble)
            {
                //smooth
                if (m_smooth > 0f)
                    m_sensor.m_collision = Vector3.Lerp(m_sensor.m_collision, m_smoothTarget, m_sensor.deltaTime * (10f / m_smooth));
                else
                    m_sensor.m_collision = m_smoothTarget;


                


                //m_pid.IgnoreFrame();
                //m_pid.m_target = m_sensor.m_collision;
            }
        }




        //List<Vector3> _collisionList = new List<Vector3>();

        public void UpdateCollisionZone(CollisionZone cz)
        {

            Vector3 collisionCenter = transform.TransformPoint(cz.positionOffset);// transform.position + transform.rotation * cz.positionOffset;
            float radius = cz.radius * VertExmotionBase.GetScaleFactor(transform);
            int nbCol = Physics.OverlapSphereNonAlloc(collisionCenter, radius, m_hitColliders, m_layerMask);

            /*
            #if KVTM_DEBUG
            Debug.DrawLine(collisionCenter, collisionCenter - Vector3.forward * radius, Color.black);
            Debug.DrawLine(collisionCenter, collisionCenter + Vector3.forward * radius, Color.black);
            Debug.DrawLine(collisionCenter, collisionCenter - Vector3.up * radius, Color.black);
            Debug.DrawLine(collisionCenter, collisionCenter + Vector3.up * radius, Color.black);
            Debug.DrawLine(collisionCenter, collisionCenter - Vector3.right * radius, Color.black);
            Debug.DrawLine(collisionCenter, collisionCenter + Vector3.right * radius, Color.black);
            #endif
            */

            int j = 0;
            cz.collisionVector = Vector3.zero;
            //int collisionListId = 0;
            
            while (j < nbCol)
        	{
                if (m_ignoreColliders.Contains(m_hitColliders[j]))
                {
                    j++;
                    continue;
                }

                Vector3 HitPoint = m_hitColliders[j].ClosestPointOnBounds(collisionCenter);

                /*
                SphereCollider sc = m_hitColliders[j] as SphereCollider;
                if (sc != null)
                {
                    //Sphere collider
                    Vector3 d = sc.transform.TransformPoint(sc.center) - collisionCenter;                   
                    HitPoint = d.normalized * (d.magnitude - radius) + collisionCenter;
                    if(  Vector3.Dot((HitPoint- collisionCenter).normalized, (sc.transform.TransformPoint(sc.center) - collisionCenter).normalized) > 0 )
                        cz.collisionVector += (radius - Vector3.Distance(HitPoint, collisionCenter)) * (collisionCenter - HitPoint).normalized;
                    else if(m_maximiZeSphereCollision)
                        cz.collisionVector += -(radius + Vector3.Distance(HitPoint, collisionCenter)) * (collisionCenter - HitPoint).normalized;
                   
                }
                else*/
                {
                    //box collider and bounding box
                    Vector3 castDir = (HitPoint - collisionCenter).normalized;//use the bounding box closeset point approximation as a cast direction
                    int nbHit = Physics.SphereCastNonAlloc(collisionCenter - castDir * radius, radius, castDir, m_hitResult);

                    for(int i=0; i< nbHit; ++i )
                    {
                        if (m_hitResult[i].collider != m_hitColliders[j])
                            continue;

                        HitPoint = m_hitResult[i].point;


                        if (Vector3.Dot((HitPoint - collisionCenter).normalized, m_hitResult[i].normal) < 0)
                            cz.collisionVector += (radius - Vector3.Distance(HitPoint, collisionCenter)) * (collisionCenter - HitPoint).normalized;
                        else if (m_maximiZeSphereCollision)
                            cz.collisionVector += -(radius + Vector3.Distance(HitPoint, collisionCenter)) * (collisionCenter - HitPoint).normalized;


                        /*
                        if (_collisionList.Count == collisionListId)
                            _collisionList.Add(Vector3.zero);

                        if (Vector3.Dot((HitPoint - collisionCenter).normalized, m_hitResult[i].normal) < 0)
                            _collisionList[collisionListId] = (radius - Vector3.Distance(HitPoint, collisionCenter)) * (collisionCenter - HitPoint).normalized; // *2f;
                        else if (m_maximiZeSphereCollision)
                            _collisionList[collisionListId] = -(radius + Vector3.Distance(HitPoint, collisionCenter)) * (collisionCenter - HitPoint).normalized;

                        collisionListId++;
                        */
                    }

                }
                /*
                for (int i = 0; i < collisionListId; ++i)
                {
                    if(_collisionList[i].magnitude> cz.collisionVector.magnitude)
                        cz.collisionVector = _collisionList[i];
                }
                */

                /*
#if KVTM_DEBUG
                Debug.DrawLine(HitPoint, collisionCenter, Color.green);
#endif
                */
                j++;
        	}
        }



#if KVTM_DEBUG
        /*
        Color m_gizmoColor = new Color(1f, 1f, 1f, .3f);
        void OnDrawGizmos()
        {
            for (int id = 0; id < m_collisionZones.Count; ++id)
            {
                Gizmos.color = m_gizmoColor;
                Gizmos.DrawWireSphere(transform.position + m_collisionZones[id].positionOffset, m_collisionZones[id].radius * VertExmotionBase.GetScaleFactor(transform));
                Gizmos.color = Color.red;

                if (m_collisionZones[id].hits != null)
                    for (int i = 0; i < m_collisionZones[id].hits.Length; ++i)
                    {
                        Gizmos.DrawSphere(m_collisionZones[id].hits[i].point, .01f);
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(m_collisionZones[id].hits[i].point, m_collisionZones[id].hits[i].normal);
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(m_collisionZones[id].hits[i].point, m_collisionZones[id].hits[i].point + (transform.position + m_collisionZones[id].positionOffset - m_collisionZones[id].hits[i].point).normalized);
                    }

            }
            

        }
        */
#endif





    }
}
