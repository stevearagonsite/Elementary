using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeIvyLeaves : MonoBehaviour {
	public Mesh leaveMesh;

	RuntimeIvy RTivy;
	[HideInInspector]
	public Mesh mesh;
	CombineInstance[] combine;

	[HideInInspector]
	public  List <List<Vector3>> vertices = new List <List<Vector3>>();
	[HideInInspector]
	public  List <List<Vector3>> verticesTarget = new List <List<Vector3>>();
	[HideInInspector]
	public  List <List<Vector3>> verticesFrom = new List <List<Vector3>>();
	[HideInInspector]
	public  List <List<Vector3>> normals = new List <List<Vector3>>();
	[HideInInspector]
	public List<List<Vector2>> uvs = new List<List<Vector2>> ();
	[HideInInspector]
	public  List <List<int>> triangles = new List <List<int>>();
	[HideInInspector]
	public List<Mesh> leavesMeshes = new List<Mesh>();

	float[] tipInfluences;
	bool[] growingLeaves;

	//Parámetros
	[Space]
	public float minScale;
	public float maxScale;
	[Space]
	public Vector3 offset;
	public Vector3 randomOffset;
	[Space]
	public Vector3 rotationOffset;
	public Vector3 randomRotationOffset;
	[Space]
	public bool globalOrientation;
	[Range(0f,1f)]
	public float globalOrientationInfluence;
	public Vector3 globalOrientationDirection;
	[Space]
	public int tipInfluence;

	[HideInInspector]
	public bool generateLeaves;

	float ifloat;
	float leaveNumberFloat;
	int leavePosition;
	int leaveNumber;

	Quaternion localRotationCorrection;

	// Use this for initialization
	public void MyStart () {
		localRotationCorrection = Quaternion.Inverse (transform.rotation);
		mesh = new Mesh();
		mesh.name = "Leaves";
//		MF.mesh = mesh;

		RTivy = GetComponent<RuntimeIvy> ();
//		Random.InitState (RTivy.randomSeed);

		vertices.Add(new List<Vector3>());
		uvs.Add ( new List<Vector2>());
		verticesTarget.Add(new List<Vector3>());
		verticesFrom.Add (new List<Vector3> ());
		normals.Add (new List<Vector3> ());
		triangles.Add (new List<int> ());
		leavesMeshes.Add (new Mesh());
		tipInfluences = new float[tipInfluence];
	}
	
	// Update is called once per frame
	public void Update () {
		if (RTivy.growing && generateLeaves) {
			for (int i = 0; i < tipInfluence; i++) {
				tipInfluences [i] = (1 * i + RTivy.tipPercent) / tipInfluence;
			}

			for (int currentBranch = 0; currentBranch < RTivy.leaves.Count; currentBranch++) {
				SharpTip (currentBranch);
				UpdateSubMesh (currentBranch);
			}

			combine = new CombineInstance[leavesMeshes.Count];
			for (int i = 0; i < combine.Length; i++) {
				combine [i].mesh = leavesMeshes [i];
			}
			mesh.CombineMeshes (combine, true, false);
		}
	}

	public void GenerateLeaves(int currentBranch, Quaternion rotation, float randomValue1, float randomValue2, float randomValue3, float randomValue4, float randomValue5, float randomValue6, float randomScale){
		Vector3 randomizedOffset = new Vector3 ((randomValue1 * 2 - 1) * randomOffset.x, (randomValue2 * 2 - 1) * randomOffset.y, (Random.value * 2 - 1) * randomOffset.z);
		Vector3 randomizedRotationOffset = new Vector3((randomValue2 * 2 - 1) * randomRotationOffset.x, (Random.value * 2 - 1) * randomRotationOffset.y, (Random.value * 2 - 1) * randomRotationOffset.z);

		Vector3 right = rotation * Vector3.right;
		Vector3 forward = rotation * Vector3.forward;
		Vector3 up = rotation * Vector3.up;

//		float randomScale = Random.Range (minScale, maxScale);

		for (int i = 0; i < leaveMesh.vertexCount; i++) {
			Vector3 vertex;
			Vector3 normal;
			vertex = leaveMesh.vertices [i];
			normal = leaveMesh.normals [i];
			vertex *= randomScale;
			vertex = rotation * vertex;
			normal = rotation * normal;

			Quaternion quat = Quaternion.AngleAxis (rotationOffset.x + randomizedRotationOffset.x, right) * Quaternion.AngleAxis (rotationOffset.y + randomizedRotationOffset.y, up)
			                  * Quaternion.AngleAxis (rotationOffset.z + randomizedRotationOffset.z, forward);

			Quaternion globalQuat = Quaternion.Inverse( Quaternion.AngleAxis (globalOrientationDirection.x, right) * Quaternion.AngleAxis (globalOrientationDirection.y, up)
				* Quaternion.AngleAxis (globalOrientationDirection.z, forward));

			if (globalOrientation) {
				quat = Quaternion.Inverse (Quaternion.Lerp (quat, globalQuat, globalOrientationInfluence));
			}
			else {
				quat = Quaternion.Inverse (quat);
			}

			vertex = quat * vertex;
			normal = quat * normal;

			vertex += (RTivy.leaves [currentBranch] [RTivy.leaves [currentBranch].Count - 1]);
			vertex += (rotation * randomizedOffset) + (rotation * offset);

			vertex -= transform.position;
			vertex = localRotationCorrection * vertex;
			normal = localRotationCorrection * normal;
			normals [currentBranch].Add (normal);
			verticesTarget [currentBranch].Add (vertex);
			verticesFrom[currentBranch].Add ((  localRotationCorrection *  (RTivy.leaves [currentBranch] [RTivy.leaves [currentBranch].Count - 1] - transform.position)));
			vertices [currentBranch].Add ((RTivy.leaves [currentBranch] [RTivy.leaves [currentBranch].Count - 1]));

			uvs [currentBranch].Add (leaveMesh.uv [i]);
		}
			
		for (int i = 0; i < leaveMesh.triangles.Length; i++) {
			triangles [currentBranch].Add (leaveMesh.triangles [i] + leaveMesh.vertexCount * ( leavesMeshes[currentBranch].vertexCount / leaveMesh.vertexCount ));
		}
		UpdateSubMesh(currentBranch);
	}

	void SharpTip(int currentBranch){
		//Este for lo que hace es recorrer desde el penúltimo vértice una rama, hasta que llegamos al último vértice del último loop afectado por la punta
			for (int i = vertices[currentBranch].Count - 1; i > RTivy.leavesCount[currentBranch]  * leaveMesh.vertexCount - tipInfluence * leaveMesh.vertexCount; i--){
			if (i < 0) {
				break;
			}
				
			ifloat = i;
			leaveNumberFloat =  ((ifloat) / (leaveMesh.vertexCount));

			leaveNumber = Mathf.FloorToInt (leaveNumberFloat);

			leavePosition = RTivy.leaves[currentBranch].Count - RTivy.leavesPosition [currentBranch] [leaveNumber];
			if (leavePosition >= tipInfluence) {
				break;
			}

			vertices [currentBranch] [i] = Vector3.Lerp (verticesFrom[currentBranch] [i], verticesTarget [currentBranch] [i], tipInfluences[leavePosition]);
		}
	}

	void UpdateSubMesh(int currentBranch){
		leavesMeshes [currentBranch].vertices = vertices [currentBranch].ToArray ();
		leavesMeshes [currentBranch].uv = uvs [currentBranch].ToArray ();
		leavesMeshes [currentBranch].triangles = triangles [currentBranch].ToArray ();
		leavesMeshes [currentBranch].normals = normals [currentBranch].ToArray ();
	}
}