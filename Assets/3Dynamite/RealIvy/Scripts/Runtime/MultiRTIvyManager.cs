using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiRTIvyManager : MonoBehaviour {

	public List<RTIvyController> rtIvyControllers = new List<RTIvyController>();
	public List<float> lifeTimes = new List<float> ();
	public List<float> delays = new List<float> ();

	public void SendData (){
		for (int i = 0; i < rtIvyControllers.Count; i++) {
			if (rtIvyControllers [i]) {
				rtIvyControllers [i].lifeTime = lifeTimes [i];
				rtIvyControllers [i].delay = delays [i];
			}
		}
	}

	public void RecieveData(){
		for (int i = 0; i < rtIvyControllers.Count; i++) {
			if (rtIvyControllers [i]) {
				lifeTimes [i] = rtIvyControllers [i].lifeTime;
				delays [i] = rtIvyControllers [i].delay;
			}
		}
	}

	public void FindInChildren(){
		rtIvyControllers.Clear ();
		lifeTimes.Clear ();
		delays.Clear ();

		RTIvyController[] temporaryControllers =  GetComponentsInChildren<RTIvyController> ();
		for (int i = 0; i < temporaryControllers.Length; i++) {
			rtIvyControllers.Add (temporaryControllers [i]);
			lifeTimes.Add (temporaryControllers [i].lifeTime);
			delays.Add (temporaryControllers [i].delay);
		}

	}
}