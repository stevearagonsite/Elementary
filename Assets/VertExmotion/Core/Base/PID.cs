using UnityEngine;
using System.Collections;


namespace Kalagaan
{
	[System.Serializable]
	public class PID {
		
		public static Color orange = new Color (240f/255f, 158f/255f, 0);

		public float lastTime;
		double errSum;
		//double lastErr;
		double error;

		public float m_target = 0f;
		public float m_timeScale = 1f;
        public bool m_unscaledTime = false;

		public Parameters m_params = new Parameters();
		float dtMax = 1f;//max dt for computing PID
		public static float dtStep = 1f/100f;//60fps step
		//float dtStep = 1f;//60fps step
		float dtStepNext = 0f;

		float delta;
		
		public PID(){}
		
		public PID( float kp, float ki, float kd )
		{
			m_params.kp = kp;
			m_params.ki = ki;
			m_params.kd = kd;
		}
		
		[System.Serializable]
		public class Parameters
		{
			public float kp = 1f, ki = .5f, kd = 0f;
			public float deltaMax = 1f;
			public Vector2 limits = new Vector2(-float.MaxValue, float.MaxValue);

			public Parameters(){}
			public Parameters( Parameters p )
			{
				kp = p.kp;
				ki = p.ki;
				kd = p.kd;
				limits.x = p.limits.x;
                limits.y = p.limits.y;
            }
		}
		
		
		public float Compute( float input )
		{
			//Debug.Log (""+Time.time);

			float result = input;
			float dt = (m_unscaledTime?Time.unscaledTime:Time.time) - lastTime;
			//if( dt >= dtStep )
			{
				result = Compute ( input, dt );
				lastTime = (m_unscaledTime ? Time.unscaledTime : Time.time);
			}
			return result;
		}
		


		public float m_lastInput = 0;
		public float Compute( float input, float dt )
		{
			dt *= m_timeScale;

			if (dt == 0)
				return input;
			
			if (dt > dtMax)
			{
				ForceTarget(1f);
				return m_target;
			}
			float result = input;
			float step = dt;


			float target = m_target;




			//check limits

			float deltaLimit = m_params.ki * (float)errSum + m_params.kp * (float)error;
			target = Mathf.Clamp ( target, m_params.limits.x, m_params.limits.y );


			if ( input + deltaLimit*2f > m_params.limits.y )
			{
				target -= (float)errSum;
				errSum = 0;
			}


			if ( input + deltaLimit*2f < m_params.limits.x )
			{
				target += (float)errSum;
				errSum = 0;
			}

			dt += dtStepNext;
			dtStepNext = 0f;


			while (dt >= dtStep ) 
			//while (dt > 0 )
			{
				//this is a real compute step

				//initialise input from last real step
				result = m_lastInput;

				step = dt>dtStep? dtStep: dt;
				//if( step>= dtStep )
				{
					//compute error
					error = (target - result);
					error *= (double)step;
					errSum += error;
				}
				
				if (m_params.ki == 0)
					errSum = 0;
				
				//double dErr = (error - lastErr) / (double)step;
				
				//lastErr = error;				
				delta = m_params.kp * (float)error + m_params.ki * (float)errSum;// + m_params.kd * (float)dErr;
				result += delta;

				//set limit
				result = Mathf.Clamp ( result, m_params.limits.x, m_params.limits.y );

				dt -= step;

				m_lastInput = result;
			}

			dtStepNext += dt;

			//add delta of current step to fake "out of step" result
			//keep a smooth result between step
			//result will be override by next real step
			result = Mathf.Lerp( m_lastInput,m_lastInput+delta , (dtStepNext / dtStep) );
			return result;
		}


/// <summary>
/// Forces the target to be set earlier
/// </summary>
/// <param name="percent">Percent.</param>
		public void ForceTarget( float percent )
		{
			errSum = (double) Mathf.Lerp ((float)errSum, 0f, percent );
			//lastErr = 0f;
		}


		/// <summary>
		/// Initialize 
		/// </summary>
		public void Init()
		{
			errSum = 0f;
			lastTime = (m_unscaledTime ? Time.unscaledTime : Time.time);
            m_lastInput = 0;
		}


        /// <summary>
		/// Initialize 
		/// </summary>
		public void Init(float input )
        {
            errSum = 0f;
            lastTime = (m_unscaledTime ? Time.unscaledTime : Time.time);
            m_lastInput = input;
        }

        /// <summary>
		/// Ignore current frame 
		/// </summary>
        public void IgnoreFrame()
        {
            errSum = 0f;            
            lastTime = (m_unscaledTime ? Time.unscaledTime : Time.time);            
        }

        /*	
            public void GUIDrawResponse( Rect area, float target, float timeUnit )
            {

                Color c = new Color (1f, 1f, 1f, .1f); 
                GLUtility.DrawBox (area, c, 1f);

                Init ();
                //unit step
                m_target = target;
                //m_limits.y = 1f;//TEST limit
                float r = 0;
                Vector2 start = new Vector2( area.x, area.y+area.height );
                Vector2 end = start;


                for (int i=0; i<timeUnit; ++i)
                {
                    start = new Vector2( (float)i*area.width / timeUnit, area.y+area.height );
                    end = new Vector2( (float)i*area.width / timeUnit, area.y );
                    GLUtility.DrawLine (start, end, c, 1f);
                }

                start = new Vector2( area.x, area.y+area.height*.5f );
                end = new Vector2( area.x+area.width, area.y+area.height*.5f );
                GLUtility.DrawLine (start, end, c, 1f);


                start = new Vector2( area.x, area.y+area.height );
                end = start;

                for( int i=0; i<area.width; ++i )
                {
                    r = Compute( r , timeUnit / area.width );


                    end.x++;			
                    end.y = area.height-r*area.height*.5f + area.y;
                    end.y = Mathf.Clamp( end.y,  area.y,  area.y+area.height );
                    //end.y = Mathf.Clamp( end.y,  1f,  area.y+area.height );

                    GLUtility.DrawLine (start, end, orange, 1f);
                    start = end;
                }

            }
            */


    }
	
	
	
	
	[System.Serializable]
	public class PID_V3 {
		
		
		public Vector3 m_target = Vector3.zero;
		public PID.Parameters m_params = new PID.Parameters();
		
		public PID m_pidX = new PID();
		public PID m_pidY = new PID();
		public PID m_pidZ = new PID();
        public bool m_useGlobalParameters = true;


		public float timeScale
		{
			get
			{
				return m_pidX.m_timeScale;
			}
			set{
				m_pidX.m_timeScale = value;
				m_pidY.m_timeScale = value;
				m_pidZ.m_timeScale = value;
			}
		}

        public bool unscaledTime
        {
            get
            {
                return m_pidX.m_unscaledTime;
            }
            set
            {
                m_pidX.m_unscaledTime = value;
                m_pidY.m_unscaledTime = value;
                m_pidZ.m_unscaledTime = value;
            }
        }



        public Vector3 Compute( Vector3 input )
		{
            if (m_useGlobalParameters)
            {
                m_pidX.m_params = m_params;
                m_pidY.m_params = m_params;
                m_pidZ.m_params = m_params;
            }
            else
            {
                if(m_pidX.m_params == m_params)
                {
                    m_pidX.m_params = new PID.Parameters(m_params);
                    m_pidY.m_params = new PID.Parameters(m_params);
                    m_pidZ.m_params = new PID.Parameters(m_params);
                }

                m_pidX.m_params.kp = m_pidY.m_params.kp = m_pidZ.m_params.kp = m_params.kp;
                m_pidX.m_params.ki = m_pidY.m_params.ki = m_pidZ.m_params.ki = m_params.ki;
                m_pidX.m_params.kd = m_pidY.m_params.kd = m_pidZ.m_params.kd = m_params.kd;

            }
            m_pidX.m_target = m_target.x;
			m_pidY.m_target = m_target.y;
			m_pidZ.m_target = m_target.z;
			
			
			Vector3 r = Vector3.zero;
			r.x = m_pidX.Compute ( input.x );
			r.y = m_pidY.Compute ( input.y );
			r.z = m_pidZ.Compute ( input.z );
			
			return r;
		}
		
		public void Init()
		{
			m_pidX.Init (m_target.x);
			m_pidY.Init (m_target.y);
			m_pidZ.Init (m_target.z);
		}

        /// <summary>
		/// Ignore current frame 
		/// </summary>
        public void IgnoreFrame()
        {
            m_pidX.IgnoreFrame();
            m_pidY.IgnoreFrame();
            m_pidZ.IgnoreFrame();
        }

        public void ForceTarget( float percent )
		{
			m_pidX.ForceTarget (percent);
			m_pidY.ForceTarget (percent);
			m_pidZ.ForceTarget (percent);
		}

				
	}
}
