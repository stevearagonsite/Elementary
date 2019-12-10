using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IvyParameters{
	//Parámetros editables de la enredadera
	public Material branchMaterial;
	public string branchMaterialGUID;
	public Material leavesMaterial;
	public string leavesMaterialGUID;
	public Mesh leavesMesh;
	public string leavesMeshGUID;

	//Parámetros editables del growing
	public int randomSeed = 26;
	public float stepSize = 0.2f;
	public Vector3 gravity = new Vector3 (0, -1, 0);
	public int maxBranchs = 1;
	public float branchProvability = 0.02f;
	public float fallProbavilityOnCorner = 0.3f;
	public float directionAmplitude = 0.2f;
	public float directionFrequenzy = 0.2f;
	public float directionRandomness = 0.5f;
	public float distanceToSurface = 0.2f;
	public float distanceToSurfaceAmplitude = 0.2f;
	public float distanceToSurfaceFrequenzy = 0.2f;
	public int leaveEvery = 1;
	public int leaveEveryRandomness = 0;

	//parámetros editables de branch
	public 	bool generateBranch = true;
	public int sides = 6;
	public float maxRadius = 0.05f;
	public 	float minRadius = 0.03f;
	public int tipInfluenceBranch = 5;
	public float radiusVariationFrequenzy = 0.2f;
	public float radiusVariationPhase = 0f;
	public float uvU = 1f;
	public float uvV = 0.1f;

	//parámetros de leaves
	public 	bool generateLeaves = true;
	public float maxScale = 4f;
	public float minScale = 1f;
	public float tipInfluenceLeave = 0.5f;
	public Vector3 positionOffset;
	public Vector3 positionOffsetRandomness;
	public bool globalDirection = false;
	public float globalDirectionInfluence;
	public Vector3 globalDirectionVector;
	public Vector3 globalDirectionVectorRandomness;
	public Vector3 directionVector = new Vector3(-60f, 0f, 0f);
	public Vector3 directionVectorRandomness = new Vector3( 10f, 40f, 40f);
}

[System.Serializable]
public class Preset{
	public bool deletable = true;
	public string presetName;
	public string path;
	public IvyParameters presetParameters;
}