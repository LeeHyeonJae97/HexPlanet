using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HexsphereGenerator
{	
	public static void Generate(HexsphereProperties properties)
	{
		// Hex vertices
		Icosahedron.Generate(properties.pointsOnSphere, properties.triangleIndices);
		for (int i = 0; i < properties.resolution; i++)
			Icosahedron.Subdivide(properties.pointsOnSphere, properties.triangleIndices, true);

		// Normalize vectors to "inflate" the icosahedron into a sphere.
		for (int i = 0; i < properties.pointsOnSphere.Count; i++)
			properties.pointsOnSphere[i] = properties.pointsOnSphere[i].normalized * properties.resolution; // * Mathf.Pow(2.17f, (float)resolution);		

		// Get properties
		List<Vector3> vertices = GetCenterOfTriangle(properties.triangleIndices, properties.pointsOnSphere);
		properties.maxEdgeLength = GetMaxEdgeDistance(vertices);
		properties.maxTileRadius = GetMaxTileRadius(vertices, properties.pointsOnSphere);

		// Generate mesh and find neighbors of tile itself		
		GenerateSubMeshes(properties, vertices);
		//for (int i = 0; i < properties.tiles.Count; i++)
		//	properties.tiles[i].GetComponent<Tile>().FindNeighbors(properties.maxTileRadius);

		// Get properties
		properties.sphereRadius = vertices[0].magnitude;
	}

	private static void GenerateSubMeshes(HexsphereProperties properties, List<Vector3> vertices)
	{
		for (int i = 0; i < properties.pointsOnSphere.Count; i++)
		{
			GameObject tileGo = GameObject.Instantiate(properties.tilePrefab);

			Mesh subMesh = new Mesh();
			List<Vector3> subMeshVertices = new List<Vector3>();

			for (int j = 0; j < vertices.Count; j++)
			{
				if ((properties.pointsOnSphere[i] - vertices[j]).sqrMagnitude <= properties.maxTileRadius * properties.maxTileRadius)
					subMeshVertices.Add(vertices[j]);
			}

			// Pent
			if (subMeshVertices.Count == 5) GeneratePentMesh(subMesh, subMeshVertices, properties.minEdgeLength, properties.maxEdgeLength);

			// Hex
			else if (subMeshVertices.Count == 6) GenerateHexMesh(subMesh, subMeshVertices, properties.minEdgeLength, properties.maxEdgeLength);

			// Error
			else
			{
				Debug.LogError("Error");
				break;
			}

			//Assign mesh
			tileGo.GetComponent<MeshFilter>().mesh = subMesh;
			subMesh.RecalculateBounds();
			subMesh.RecalculateNormals();

			// Fix any upsidedown tiles by checking their normal vector			
			if ((subMesh.normals[0] + properties.pointsOnSphere[i]).sqrMagnitude < properties.pointsOnSphere[i].sqrMagnitude)
			{
				subMesh.triangles = subMesh.triangles.Reverse().ToArray();
				subMesh.RecalculateBounds();
				subMesh.RecalculateNormals();
			}

			// Initialize
			tileGo.tag = properties.tileTag;
			tileGo.transform.SetParent(properties.tileHolder);
			Tile tile = tileGo.GetComponent<Tile>();			
			tile.Initialize();
			properties.tiles.Add(tile);

			// Add Collider
			MeshCollider coll = tileGo.AddComponent<MeshCollider>();
			coll.convex = true;
			coll.isTrigger = true;
		}
	}

	private static void GeneratePentMesh(Mesh subMesh, List<Vector3> subMeshVertices, float minEdgeLength, float maxEdgeLength)
	{
		bool[] isUsed = new bool[5];
		List<int> orderedIndices = new List<int>();
		Vector3 curVertex = subMeshVertices[0];

		orderedIndices.Add(0);
		isUsed[0] = true;

		// Find a point on the perimeter of the tile that is within one edgelength from point current, then add its index to the list
		while (orderedIndices.Count < 5)
		{
			for (int index = 0; index < subMeshVertices.Count; index++)
			{
				Vector3 vertex = subMeshVertices[index];

				float sqrDst = (vertex - curVertex).sqrMagnitude;
				if (minEdgeLength * minEdgeLength <= sqrDst && sqrDst <= maxEdgeLength * maxEdgeLength && !isUsed[index])
				{
					orderedIndices.Add(index);
					isUsed[index] = true;
					curVertex = vertex;
					break;
				}
			}
		}

		// Vertices
		// Sort
		Vector3[] orderedSubMeshVertices = subMeshVertices.ToArray();
		for (int i = 0; i < orderedSubMeshVertices.Length; i++)
			orderedSubMeshVertices[i] = subMeshVertices[orderedIndices[i]];

		// Add to list
		subMeshVertices.Clear();
		subMeshVertices.AddRange(orderedSubMeshVertices);
		for (int i = 0; i < orderedSubMeshVertices.Length; i++)
		{
			subMeshVertices.Add(orderedSubMeshVertices[i]);
			if (i + 1 == orderedSubMeshVertices.Length)
			{
				subMeshVertices.Add(orderedSubMeshVertices[0]);
			}
			else
			{
				subMeshVertices.Add(orderedSubMeshVertices[i + 1]);
			}
		}
		subMeshVertices.Add(Vector3.zero);

		// assign vertices
        subMesh.vertices = subMeshVertices.ToArray();

		// Triangles
		int[] triangles = new int[24];
		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 2;
		triangles[3] = 2;
		triangles[4] = 3;
		triangles[5] = 0;
		triangles[6] = 3;
		triangles[7] = 4;
		triangles[8] = 0;
		for (int i = 9, j = 0; i < triangles.Length; i += 3, j += 2)
		{
			triangles[i] = subMeshVertices.Count - 1;
			triangles[i + 1] = 5 + j + 1;
			triangles[i + 2] = 5 + j;
		}
		subMesh.triangles = triangles;

		// UVs
		//Vector2[] uvs = new Vector2[subMeshVertices.Count];
		//uvs[orderedIndices[0]] = new Vector2(0f, 0.625f);
		//uvs[orderedIndices[1]] = new Vector2(0.5f, 1f);
		//uvs[orderedIndices[2]] = new Vector2(1f, 0.625f);
		//uvs[orderedIndices[3]] = new Vector2(0.8f, 0.0162f);
		//uvs[orderedIndices[4]] = new Vector2(.1875f, 0.0162f);
		//subMesh.uv = uvs;

		// When using multiple materials..
		// Material[] pentMats = new Material[2];
		// pentMats[0] = pentInsideMat;
		// pentMats[1] = pentMat;
		// tile.GetComponent<Renderer>().materials = pentMats;
	}

	private static void GenerateHexMesh(Mesh subMesh, List<Vector3> subMeshVertices, float minEdgeLength, float maxEdgeLength)
	{
		bool[] isUsed = new bool[6];
		List<int> orderedIndices = new List<int>();
		Vector3 curVertex = subMeshVertices[0];

		orderedIndices.Add(0);
		isUsed[0] = true;

		// Find a point on the perimeter of the tile that is within one edgelength from point current, then add its index to the list
		while (orderedIndices.Count < 6)
		{
			for (int index = 0; index < subMeshVertices.Count; index++)
			{
				Vector3 vertex = subMeshVertices[index];

				float sqrDst = (vertex - curVertex).sqrMagnitude;
				if (minEdgeLength * minEdgeLength <= sqrDst && sqrDst <= maxEdgeLength * maxEdgeLength && !isUsed[index])
				{
					orderedIndices.Add(index);
					isUsed[index] = true;
					curVertex = vertex;
					break;
				}
			}
		}

		// Vertices
		// Sort
		Vector3[] orderedSubMeshVertices = subMeshVertices.ToArray();
		for (int i = 0; i < orderedSubMeshVertices.Length; i++)
			orderedSubMeshVertices[i] = subMeshVertices[orderedIndices[i]];

		// Add to list
		subMeshVertices.Clear();
		subMeshVertices.AddRange(orderedSubMeshVertices);
		for (int i = 0; i < orderedSubMeshVertices.Length; i++)
		{
			subMeshVertices.Add(orderedSubMeshVertices[i]);
			if (i + 1 == orderedSubMeshVertices.Length)
			{
				subMeshVertices.Add(orderedSubMeshVertices[0]);
			}
			else
			{
				subMeshVertices.Add(orderedSubMeshVertices[i + 1]);
			}
		}
		subMeshVertices.Add(Vector3.zero);

		// assign vertices
		subMesh.vertices = subMeshVertices.ToArray();

		// Triangles
		int[] triangles = new int[30];
		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 2;
		triangles[3] = 2;
		triangles[4] = 3;
		triangles[5] = 0;
		triangles[6] = 3;
		triangles[7] = 4;
		triangles[8] = 0;
		triangles[9] = 4;
		triangles[10] = 5;
		triangles[11] = 0;
		for (int i = 12, j = 0; i < triangles.Length; i += 3, j += 2)
		{
			triangles[i] = subMeshVertices.Count - 1;
			triangles[i + 1] = 6 + j + 1;
			triangles[i + 2] = 6 + j;
		}
		subMesh.triangles = triangles;

		//// UVs
		//Vector2[] uvs = new Vector2[subMeshVertices.Count];
		//uvs[orderedIndices[0]] = new Vector2(0.0543f, 0.2702f);
		//uvs[orderedIndices[1]] = new Vector2(0.0543f, 0.7272f);
		//uvs[orderedIndices[2]] = new Vector2(0.5f, 1f);
		//uvs[orderedIndices[3]] = new Vector2(0.946f, 0.7272f);
		//uvs[orderedIndices[4]] = new Vector2(0.946f, 0.2702f);
		//uvs[orderedIndices[5]] = new Vector2(0.5f, 0f);
		//subMesh.uv = uvs;
	}

	private static float GetMaxTileRadius(List<Vector3> centers, List<Vector3> vertices)
	{
		// why 1.5f? why vertices[12]?
		float delta = 1.5f;
		Vector3 vertex = vertices[12];

		float minDst = Mathf.Infinity;
		for (int i = 0; i < centers.Count; i++)
		{
			float sqrDst = (vertex - centers[i]).sqrMagnitude;
			if (sqrDst < minDst) minDst = sqrDst;
		}
		minDst = Mathf.Sqrt(minDst) * delta;

		return minDst;
	}

	private static float GetMaxEdgeDistance(List<Vector3> centers)
	{
		//delta is the approximate variation in edge lengths, as not all edges are the same length
		float delta = 1.4f;
		Vector3 center = centers[0];

		// scan all vertices to find nearest
		float minDst = Mathf.Infinity;
		for (int i = 1; i < centers.Count; i++)
		{
			float sqrDst = (center - centers[i]).sqrMagnitude;
			if (sqrDst < minDst) minDst = sqrDst;
		}
		minDst = Mathf.Sqrt(minDst) * delta;

		return minDst;
	}

	private static List<Vector3> GetCenterOfTriangle(List<int> triangles, List<Vector3> vertices)
	{
		List<Vector3> centers = new List<Vector3>();
		for (int i = 0; i < triangles.Count - 2; i += 3)
		{
			Vector3 A = vertices[triangles[i]];
			Vector3 B = vertices[triangles[i + 1]];
			Vector3 C = vertices[triangles[i + 2]];

			float a = Vector3.Distance(B, C);
			float b = Vector3.Distance(A, C);
			float c = Vector3.Distance(A, B);

			float P = a + b + c;

			Vector3 abc = new Vector3(a, b, c);

			float x = Vector3.Dot(abc, new Vector3(A.x, B.x, C.x)) / P;
			float y = Vector3.Dot(abc, new Vector3(A.y, B.y, C.y)) / P;
			float z = Vector3.Dot(abc, new Vector3(A.z, B.z, C.z)) / P;

			centers.Add(new Vector3(x, y, z));
		}

		return centers;
	}

	public static Vector3[] GetCenterOfTriangle(int[] triangles, Vector3[] vertices)
	{
		List<Vector3> centers = new List<Vector3>();
		for (int i = 0; i < triangles.Length - 2; i += 3)
		{
			Vector3 A = vertices[triangles[i]];
			Vector3 B = vertices[triangles[i + 1]];
			Vector3 C = vertices[triangles[i + 2]];

			float a = Vector3.Distance(B, C);
			float b = Vector3.Distance(A, C);
			float c = Vector3.Distance(A, B);

			float P = a + b + c;

			Vector3 abc = new Vector3(a, b, c);

			float x = Vector3.Dot(abc, new Vector3(A.x, B.x, C.x)) / P;
			float y = Vector3.Dot(abc, new Vector3(A.y, B.y, C.y)) / P;
			float z = Vector3.Dot(abc, new Vector3(A.z, B.z, C.z)) / P;

			centers.Add(new Vector3(x, y, z));
		}

		return centers.ToArray();
	}
}
