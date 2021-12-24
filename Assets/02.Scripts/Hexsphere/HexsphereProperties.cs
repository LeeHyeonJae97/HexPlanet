using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexsphereProperties
{
	[Range(1, 4)] public int resolution;
	[Min(0.1f)] public float scale;
	public string tileTag;

	[HideInInspector] public float maxEdgeLength;
	[HideInInspector] public float minEdgeLength = 0.001f;
	[HideInInspector] public float maxTileRadius;
	[HideInInspector] public float sphereRadius;

	public GameObject tilePrefab;
	public Transform tileHolder;

	[HideInInspector] public List<Vector3> pointsOnSphere = new List<Vector3>();
	[HideInInspector] public List<int> triangleIndices = new List<int>();
	[HideInInspector] public List<Tile> tiles = new List<Tile>();	
}
