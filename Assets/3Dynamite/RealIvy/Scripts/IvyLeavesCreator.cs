#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class IvyLeavesCreator : MonoBehaviour {
	public bool performanceMode;
	public bool generateLeaves;
	public Vector3 offset;
	public Vector3 offsetRandomness;
	[Space]
	public bool globalOrientation = false;
	[Range (0f, 1f)]
	public float globalOrientationInfluence;
	public Vector3 globalDirection = new Vector3(0f, -1f, 0f);
	public Vector3 globalDirectionRandomness;
	[Space]
	public Vector3 orientation;
	public Vector3 orientationRandomness;
	[Space]
	public float minScale = 1f;
	public float maxScale = 2f;
	public float tipInfluence = 5f;
	[Space]
	public Mesh mesh;
	public Mesh finalMesh;

	private IvyGrowingController ivyPoints;

	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private List<Vector3> normals = new List<Vector3>();
	private List<Vector2> uvs = new List<Vector2>();

	private Quaternion quat;
	private Quaternion quatX;
	private Quaternion quatY;
	private Quaternion quatZ;
	private Quaternion quatG;

	//Variables para el modo rendimiento
	private int perfTarget = 10;
	public int perfCount = 0;

	[HideInInspector]
	public int randomSeed;

	// Use this for initialization
	void Start () {
		ivyPoints = GetComponent<IvyGrowingController>();
		finalMesh = new Mesh();
	}
	
	// Update is called once per frame
	void Update () {
		if (perfCount >= perfTarget || !performanceMode){
			finalMesh.Clear();
			if (ivyPoints.leaves.Count > 0 && mesh && generateLeaves){
				for (int i = 0; i < ivyPoints.leaves.Count; i ++){
					Random.InitState(i);
					float random = Random.value * 2 - 1f;
					float randomRange = Random.Range(minScale, maxScale);
					Vector3 randomVector = Random.onUnitSphere;

					if (!globalOrientation){
						quat = ivyPoints.leaves[i].rotation;
						quatX = Quaternion.AngleAxis(orientation.x + random * orientationRandomness.x, ivyPoints.leaves[i].right);
						quatY = Quaternion.AngleAxis(orientation.y + random * orientationRandomness.y, ivyPoints.leaves[i].up);
						quatZ = Quaternion.AngleAxis(orientation.z + random * orientationRandomness.z, ivyPoints.leaves[i].forward);
					}
					else{
						Vector3 globalDirectionRandomnessFinal = new Vector3(globalDirectionRandomness.x * randomVector.x,
																			globalDirectionRandomness.y * randomVector.y,
																			globalDirectionRandomness.z * randomVector.z);
						Vector3 globalDirectionFinal = globalDirection + globalDirectionRandomnessFinal;
						quatG = Quaternion.Lerp(quat, Quaternion.Euler(globalDirectionFinal), globalOrientationInfluence);
					}

					foreach (Vector3 vertex in mesh.vertices){
						Vector3 v = vertex;

						//Orientation stuff
						if (!globalOrientation){
							v = quat * v;
							v = quatX * v;
							v = quatY * v;
							v = quatZ * v;
						}
						else{
							v = quatG * v;
						}

						//Scale stuff
//						randomRange = Mathf.Lerp(0f, randomRange, Mathf.InverseLerp(0f, tipInfluence, ivyPoints.leaves.Count));
//						if (i > ivyPoints.leaves.Count - tipInfluence){
//							float iFloat = i;
//							randomRange *= (ivyPoints.leaves.Count - iFloat) / tipInfluence;
//						}						
//						v *= randomRange;

						float position = ivyPoints.leaves[i].position;
						float branchLenght = ivyPoints.lenghts[ivyPoints.leaves[i].currentbranch];
						float bias = branchLenght - tipInfluence;

						if (position > bias){
							float scaleValue = Mathf.InverseLerp(branchLenght, bias, position);
							if (scaleValue < 0.9f){
								scaleValue = 0f;
							}
							randomRange *= scaleValue;
						}

						v *= randomRange;
						//Position stuff
						Vector3 newOrigin = ivyPoints.leaves[i].origin;

						newOrigin += ivyPoints.leaves[i].right * offset.x;
						newOrigin += ivyPoints.leaves[i].up * offset.y;
						newOrigin += ivyPoints.leaves[i].forward * offset.z;

						newOrigin += ivyPoints.leaves[i].right * random * offsetRandomness.x;
						newOrigin += ivyPoints.leaves[i].up * random * offsetRandomness.y;
						newOrigin += ivyPoints.leaves[i].forward * random * offsetRandomness.z;

						vertices.Add(v + newOrigin);
					}
					foreach (int triangle in mesh.triangles){
						triangles.Add(triangle + mesh.vertexCount * i);
					}
					foreach (Vector3 normal in mesh.normals){
						Vector3 n = normal;

						if (!globalOrientation){
							n = quat * n;
							n = quatZ * n;
							n = quatX * n;
							n = quatY * n;
						}
						else{
							n = quatG * n;
						}

						normals.Add(n);
					}
					foreach (Vector2 uv in mesh.uv){
						uvs.Add(uv);
					}
				}

				for (int i = 0; i < vertices.Count; i ++){
					vertices [i] -= transform.position;

					Quaternion inverseTransform = Quaternion.Inverse(transform.rotation);

					vertices [i] = inverseTransform * vertices[i];

					normals[i] = inverseTransform * normals[i];
				}

				finalMesh.vertices = vertices.ToArray();
				finalMesh.triangles = triangles.ToArray();
				finalMesh.normals = normals.ToArray();
				finalMesh.uv = uvs.ToArray();

				TangentSolver tg = new TangentSolver();

				finalMesh = tg.Solve(finalMesh);

				vertices.Clear();
				triangles.Clear();
				normals.Clear();
				uvs.Clear();
			}
			perfCount = 0;
		}
		else{
			perfCount ++;
		}
	}

	public void CleanUp(){
		vertices.Clear();
		triangles.Clear();
		normals.Clear();
		uvs.Clear();
		if (finalMesh){
			finalMesh.Clear();
		}
	}
}
#endif