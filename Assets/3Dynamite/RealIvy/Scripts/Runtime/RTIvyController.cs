using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTIvyController : MonoBehaviour {
	[HideInInspector]
	public int selectedPreset;
	public Preset preset;
	[HideInInspector]
	public Vector2 presetBoxScroll;
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
	[HideInInspector]
	public RuntimeIvy RTIvy;
	[HideInInspector]
	public RuntimeIvyGeom RTGeom;
	[HideInInspector]
	public RuntimeIvyLeaves RTLeaves;

	MeshRenderer meshRenderer;

	IvyParameters ivyParameters;

	float elapsedTime;
	float speed;
	float oldSpeed;
	bool oldGrowing;
	public bool IvyEnabled = true;
	bool finished = false;

	void Start () {
		GetIvyComponents ();
		if (preWarmEnabled) {
			oldGrowing = RTIvy.growing;
			oldSpeed = RTIvy.speed;
			RTIvy.growing = true;
			RTIvy.speed = 200f;

			for (int i = 1; i <= preWarm; i++) {
				RTIvy.Update ();
				RTGeom.Update ();
				RTLeaves.Update ();
			}

			RTIvy.growing = oldGrowing;
			RTIvy.speed = oldSpeed;
		}
		growing = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (IvyEnabled && !finished) {
			elapsedTime += Time.deltaTime;

			if (elapsedTime >= delay) {
				growing = true;
			}

			float normalizedElapsedTime = (elapsedTime - delay) / lifeTime;
			if (speedOverLifeTimeEnabled) {
				speed = maxSpeed * speedOverLifeTime.Evaluate (normalizedElapsedTime);
			} 
			else {
				speed = maxSpeed;
			}

			if (elapsedTime >= lifeTime + delay) {
				growing = false;
				finished = true;
			}
		}
		else {
			growing = false;
		}

		RTIvy.growing = growing;
		RTIvy.speed = speed;
	}
//	[ContextMenuItem("LOLO")]
	public void SendParameters(Preset preset){
		GetIvyComponents ();
		ivyParameters = preset.presetParameters;
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
			RTLeaves.leaveMesh = ivyParameters.leavesMesh;
		}
		else{
			Material[] newMats = new Material[1];
			newMats[0] = ivyParameters.branchMaterial;
			meshRenderer.sharedMaterials = newMats;
		}

		RTIvy.randomSeed = ivyParameters.randomSeed ;
		RTIvy.stepSize = ivyParameters.stepSize ;
		RTIvy.gravity = ivyParameters.gravity ;
		RTIvy.maxBranches = ivyParameters.maxBranchs ;
		RTIvy.branchProbavility = ivyParameters.branchProvability ;
		RTIvy.grabProbavilityOnCorner = ivyParameters.fallProbavilityOnCorner ;
		RTIvy.directionAmplitude = ivyParameters.directionAmplitude ;
		RTIvy.directionFrequency = ivyParameters.directionFrequenzy  ;
		RTIvy.directionRandomness = ivyParameters.directionRandomness  ;
		RTIvy.distanceToSurface = ivyParameters.distanceToSurface  ;
		RTIvy.distanceToSurfaceAmplitude = ivyParameters.distanceToSurfaceAmplitude;
		RTIvy.distanceToSurfaceFrequency = ivyParameters.distanceToSurfaceFrequenzy  ;
		RTIvy.leaveEvery = ivyParameters.leaveEvery;
		RTIvy.randomLeaveEvery = ivyParameters.leaveEveryRandomness ;
		RTIvy.optimizeAngleBias = angleBias;

		RTGeom.generateBranch = ivyParameters.generateBranch ;
		RTGeom.sides = ivyParameters.sides ;
		RTGeom.maxRadius = ivyParameters.maxRadius ;
		RTGeom.minRadius = ivyParameters.minRadius ;
		RTGeom.tipInfluence = ivyParameters.tipInfluenceBranch ;
		RTGeom.radiusVarFrequenzy = ivyParameters.radiusVariationFrequenzy ;
		RTGeom.radiusVarPhase = ivyParameters.radiusVariationPhase ;
		RTGeom.uvH = ivyParameters.uvU ;
		RTGeom.uvV = ivyParameters.uvV  / 5f;

		RTLeaves.generateLeaves = ivyParameters.generateLeaves ;
		RTLeaves.maxScale = ivyParameters.maxScale ;
		RTLeaves.minScale = ivyParameters.minScale ;
		RTLeaves.tipInfluence = Mathf.FloorToInt( ivyParameters.tipInfluenceLeave * 30 );
		RTLeaves.offset = ivyParameters.positionOffset ;
		RTLeaves.randomOffset = ivyParameters.positionOffsetRandomness ;
		RTLeaves.rotationOffset = ivyParameters.directionVector ;
		RTLeaves.randomRotationOffset = ivyParameters.directionVectorRandomness ;
		RTLeaves.globalOrientation = ivyParameters.globalDirection;
		RTLeaves.globalOrientationDirection = ivyParameters.globalDirectionVector;
		RTLeaves.globalOrientationInfluence = ivyParameters.globalDirectionInfluence;

		generateBranches = ivyParameters.generateBranch;
		generateLeaves = ivyParameters.generateLeaves;
	}

	public void GetIvyComponents(){
		RTIvy = GetComponent<RuntimeIvy> ();
		RTGeom = GetComponent<RuntimeIvyGeom> ();
		RTLeaves = GetComponent<RuntimeIvyLeaves> ();
		meshRenderer = GetComponent<MeshRenderer> ();
	}
}