using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IvyCaster : MonoBehaviour {
	public Transform cameraTransform;
	[SerializeField]
	IvyParameters parameters;
	public int selectedPreset;
	public Vector2 presetBoxScroll;

	[SerializeField]
	public bool IvyEnabled = true;
	[SerializeField]
	public bool preWarmEnabled;
	[SerializeField]
	public int preWarm;
	[Space]
	[SerializeField]
	public bool growing = true;
	[Space]
	[SerializeField]
	public float maxSpeed = 10f;
	[Space]
	[SerializeField]
	public float lifeTime = 5f;
	[SerializeField]
	public float delay = 0f;
	[SerializeField]
	public bool speedOverLifeTimeEnabled;
	[SerializeField]
	public AnimationCurve speedOverLifeTime;
	[Space]
	[SerializeField]
	public bool autoOptimize = true;
	[SerializeField]
	public float angleBias = 10f;
	[Space]
	//	public int maxBranchesOverLifeTime;
	[Space]
	[SerializeField]
	public bool generateBranches;
	[SerializeField]
	public bool generateLeaves;

	RuntimeIvy newGrow;
	RuntimeIvyGeom newGeom;
	RuntimeIvyLeaves newLeaves;
	RTIvyController newController;
	MeshRenderer meshRenderer;

	GameObject newIvy;

	public bool firstPass = true;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void CastIvy(Vector3 point, Vector3 forwardVector, float offset = 0.05f){
		newIvy = new GameObject ();
		newIvy.name = "Ivy";
		newIvy.transform.position = point + forwardVector * offset;
		newIvy.transform.rotation = Quaternion.LookRotation (-forwardVector);

		meshRenderer = newIvy.AddComponent<MeshRenderer> ();

		newGeom = newIvy.AddComponent<RuntimeIvyGeom> ();
		newLeaves = newIvy.AddComponent<RuntimeIvyLeaves> ();
		newIvy.AddComponent<MeshFilter> ();

		newGrow = newIvy.AddComponent<RuntimeIvy> ();
		newController = newIvy.AddComponent<RTIvyController> ();

		SendParameters (parameters);

		newGrow.Initialize ();
	}

	public void GetParameters (Preset preset){
		parameters = preset.presetParameters;
		generateBranches = parameters.generateBranch;
		generateLeaves = parameters.generateLeaves;
	}

	public void SendParameters(IvyParameters parameters, bool randomize = true){
		IvyParameters ivyParameters = parameters;
		//Casuistica para la asignación de materiales, malla, y booleanos de generate mesh y branch
		if (ivyParameters.generateBranch && ivyParameters.generateLeaves){
			Material[] newMats = new Material[2];
			newMats[0] = ivyParameters.branchMaterial;
			newMats[1] = ivyParameters.leavesMaterial;
			meshRenderer.sharedMaterials = newMats;
		}
		if (ivyParameters.generateBranch && !ivyParameters.generateLeaves){
			Material[] newMats = new Material[1];
			newMats[0] = ivyParameters.branchMaterial;
			meshRenderer.sharedMaterials = newMats;
		}
		if (ivyParameters.generateLeaves && !ivyParameters.generateBranch){
			Material[] newMats = new Material[1];
			newMats[0] = ivyParameters.leavesMaterial;
			meshRenderer.sharedMaterials = newMats;
		}
		if (ivyParameters.leavesMesh){
			newLeaves.leaveMesh = ivyParameters.leavesMesh;
		}
		else{
			Material[] newMats = new Material[1];
			newMats[0] = ivyParameters.branchMaterial;
			meshRenderer.sharedMaterials = newMats;
		}

		newGrow.createdInEditor = false;
		if (randomize) {
			newGrow.randomSeed = Random.Range (-4000, 4000);
		}
		else {
			newGrow.randomSeed = ivyParameters.randomSeed;
		}
		newGrow.stepSize = ivyParameters.stepSize ;
		newGrow.gravity = ivyParameters.gravity ;
		newGrow.maxBranches = ivyParameters.maxBranchs ;
		newGrow.branchProbavility = ivyParameters.branchProvability ;
		newGrow.grabProbavilityOnCorner = ivyParameters.fallProbavilityOnCorner ;
		newGrow.directionAmplitude = ivyParameters.directionAmplitude ;
		newGrow.directionFrequency = ivyParameters.directionFrequenzy  ;
		newGrow.directionRandomness = ivyParameters.directionRandomness  ;
		newGrow.distanceToSurface = ivyParameters.distanceToSurface  ;
		newGrow.distanceToSurfaceAmplitude = ivyParameters.distanceToSurfaceAmplitude;
		newGrow.distanceToSurfaceFrequency = ivyParameters.distanceToSurfaceFrequenzy  ;
		newGrow.leaveEvery = ivyParameters.leaveEvery;
		newGrow.randomLeaveEvery = ivyParameters.leaveEveryRandomness ;

		newGeom.generateBranch = ivyParameters.generateBranch ;
		newGeom.sides = ivyParameters.sides ;
		newGeom.maxRadius = ivyParameters.maxRadius ;
		newGeom.minRadius = ivyParameters.minRadius ;
		newGeom.tipInfluence = ivyParameters.tipInfluenceBranch ;
		newGeom.radiusVarFrequenzy = ivyParameters.radiusVariationFrequenzy ;
		newGeom.radiusVarPhase = ivyParameters.radiusVariationPhase ;
		newGeom.uvH = ivyParameters.uvU ;
		newGeom.uvV = ivyParameters.uvV / 5f;

		newLeaves.generateLeaves = ivyParameters.generateLeaves ;
		newLeaves.maxScale = ivyParameters.maxScale ;
		newLeaves.minScale = ivyParameters.minScale ;
		newLeaves.tipInfluence = Mathf.FloorToInt( ivyParameters.tipInfluenceLeave * 30 );
		newLeaves.offset = ivyParameters.positionOffset ;
		newLeaves.randomOffset = ivyParameters.positionOffsetRandomness ;
		newLeaves.rotationOffset = ivyParameters.directionVector ;
		newLeaves.randomRotationOffset = ivyParameters.directionVectorRandomness ;
		newLeaves.globalOrientation = ivyParameters.globalDirection;
		newLeaves.globalOrientationDirection = ivyParameters.globalDirectionVector;
		newLeaves.globalOrientationInfluence = ivyParameters.globalDirectionInfluence;

		newController.selectedPreset = selectedPreset;
		newController.preWarmEnabled = preWarmEnabled;
		newController.preWarm = preWarm;
		newController.IvyEnabled = IvyEnabled;
		newController.lifeTime = lifeTime;
		newController.delay = delay;
		newController.speedOverLifeTimeEnabled = speedOverLifeTimeEnabled;
		newController.speedOverLifeTime = speedOverLifeTime;
		newController.maxSpeed = maxSpeed;
		newController.autoOptimize = autoOptimize;
		newController.angleBias = angleBias;
		newController.generateBranches = generateBranches;
		newController.generateLeaves = generateLeaves;
	}
}
