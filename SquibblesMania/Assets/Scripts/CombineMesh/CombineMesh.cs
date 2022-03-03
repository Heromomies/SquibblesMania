using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class CombineMesh : MonoBehaviour
{
	[SerializeField] private List<MeshFilter> sourceMeshFilters;
	[SerializeField] private MeshFilter targetMeshFilter;

	private void Start()
	{
		CombineMeshes();
	}

	private void CombineMeshes()
	{
		var combine = new CombineInstance[sourceMeshFilters.Count];

		for (int i = 0; i < sourceMeshFilters.Count; i++)
		{
			combine[i].mesh = sourceMeshFilters[i].sharedMesh;
			combine[i].transform = sourceMeshFilters[i].transform.localToWorldMatrix;
		}

		var mesh = new Mesh();
		mesh.CombineMeshes(combine);
		targetMeshFilter.mesh = mesh;
	}
}