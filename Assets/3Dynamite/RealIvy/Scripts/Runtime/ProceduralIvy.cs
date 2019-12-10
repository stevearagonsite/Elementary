#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ProceduralIvy : MonoBehaviour {

	public IvyParameters parameters;
	public int selectedPreset;

	RuntimeIvy newGrow;
	RuntimeIvyGeom newGeom;
	RuntimeIvyLeaves newLeaves;
	RTIvyController newController;
	MeshRenderer meshRenderer;

	public GameObject go;

	public void ConvertToRuntime(){
		go = new GameObject ();
		go.transform.position = transform.position;
		go.transform.rotation = transform.rotation;
		go.transform.localScale = transform.localScale;
		go.name = "RT Ivy";

		meshRenderer = go.AddComponent<MeshRenderer> ();
		newController = go.AddComponent<RTIvyController> ();
		newGrow = go.AddComponent<RuntimeIvy> ();
		newGeom = go.AddComponent<RuntimeIvyGeom> ();
		newLeaves = go.AddComponent<RuntimeIvyLeaves> ();

		go.AddComponent<MeshFilter> ();

		SendParameters (parameters);

		GameObject[] newSel = new GameObject[1];
		newSel [0] = go;
		Selection.objects = newSel;

		DestroyImmediate (gameObject);
	}

	public void SendParameters(IvyParameters parameters){
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

		newGrow.createdInEditor = true;
		newGrow.randomSeed = ivyParameters.randomSeed ;
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

		newController.generateBranches = ivyParameters.generateBranch;
		newController.generateLeaves = ivyParameters.generateLeaves;
		newController.selectedPreset = selectedPreset;
	}
}
#endif