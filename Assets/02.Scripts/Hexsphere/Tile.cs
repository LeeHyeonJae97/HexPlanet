using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
	// HexCenter  : 0.885f
	// PentCenter : 0.904688f

	[System.Serializable]
	public struct TileType
	{
		public float extrusion;
		public Material material;
	}

	public TileType Type { get; private set; }	

	private Vector3 center;
	public Vector3 Center
	{
		get
		{
			return center * transform.lossyScale.x + transform.position;
		}
	}

	public bool IsEmpty { get; private set; } = true;

	private MeshRenderer mr;

	public void Initialize()
	{
		CalculateCenter();

		mr = GetComponent<MeshRenderer>();
	}

	private void CalculateCenter()
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		if (mesh.vertices.Length == 19)
		{
			center = (mesh.vertices[0] + mesh.vertices[3]) / 2;
		}
		else if (mesh.vertices.Length == 16)
		{
			Vector3[] centerOfTriangles = HexsphereGenerator.GetCenterOfTriangle(mesh.triangles, mesh.vertices);
			center = HexsphereGenerator.GetCenterOfTriangle(new int[3] { 0, 1, 2 }, centerOfTriangles)[0];
		}
	}

    public void SetType(TileType type)
	{
		mr.material = type.material;		
		transform.localScale *= type.extrusion;

		Type = type;
	}

	public void BuildTower()
    {
		IsEmpty = false;
    }

	public void RemoveTower()
    {
		IsEmpty = true;
    }

	public void Damaged()
	{
		transform.localScale *= 0.85f;
	}

	public void Warned(bool value, Material warningMat)
	{
		if (value)
		{
			mr.material = warningMat;
		}
		else
		{
			mr.material = Type.material;
		}
	}

	/*
	private List<Tile> neighbors = new List<Tile>();

	public void FindNeighbors(float maxTileRadius)
	{
		Collider[] colls = Physics.OverlapSphere(Center, maxTileRadius);
		for (int i = 0; i < colls.Length; i++)
		{
			if (colls[i].gameObject != gameObject)
				neighbors.Add(colls[i].gameObject.GetComponent<Tile>());
		}
	}
	*/

	/*
	private void Extrude(float amount)
	{
        Vector3[] vertices = mesh.vertices;
        List<Vector3> newVertices = new List<Vector3>();

        // Add new vertices
        for (int i = 0; i < vertices.Length; i++)
        {
            newVertices.Add(vertices[i] * amount);
        }
        for (int i = 0; i < vertices.Length; i++)
        {
            if (i == vertices.Length - 1)
            {
                newVertices.Add(newVertices[i]);
                newVertices.Add(newVertices[0]);
                newVertices.Add(vertices[0]);
                newVertices.Add(vertices[i]);
            }
            else
            {
                newVertices.Add(newVertices[i]);
                newVertices.Add(newVertices[i + 1]);
                newVertices.Add(vertices[i + 1]);
                newVertices.Add(vertices[i]);
            }
        }

        // Add new triangles
        int[] triangles = mesh.triangles;
        List<int> newTriangles = new List<int>((vertices.Length - 2) * 3 + vertices.Length * 2 * 3);
        for (int i = triangles.Length, j = vertices.Length; i < newTriangles.Capacity; i += 6, j += 4)
        {
            newTriangles.Add(j);
            newTriangles.Add(j + 2);
            newTriangles.Add(j + 1);
            newTriangles.Add(j);
            newTriangles.Add(j + 3);
            newTriangles.Add(j + 2);
        }
        if (isUpsideDown) newTriangles.Reverse();

        newTriangles.InsertRange(0, triangles);

        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        // Reset center position
        Center *= amount;

        if (TryGetComponent(out MeshCollider coll)) coll.convex = true;
    }
	*/
}
