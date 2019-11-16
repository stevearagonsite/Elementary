using UnityEngine;

namespace Mewlist
{
	public class MassiveCloudsDemo : MonoBehaviour
	{
		[SerializeField] private Transform TargetCamera;
		[SerializeField] private float a;
		[SerializeField] private float b;
		[SerializeField] private float c;
		[SerializeField] private float Ground = 51f;
		[SerializeField] private float range = 100f;
		[SerializeField] private float velocity = 1f;
		[SerializeField] private float height = 100f;

		private Vector3 Lissajous(float t)
		{
			return new Vector3(
				range * Mathf.Cos(a * t),
				Ground + height * (1f + Mathf.Sin(c * t + 0.1f)) * 0.5f,
				range * Mathf.Sin(b * t)
			);
		}

		// Update is called once per frame
		void Update()
		{
			var t 				= Time.time;
			TargetCamera.transform.position = Lissajous(t * velocity);
			var prevPos = Lissajous(t * velocity - 0.2f);
			var dir = TargetCamera.transform.position - prevPos;
			TargetCamera.transform.LookAt(TargetCamera.transform.position + dir.normalized);
		}
	}
}