using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PolarWind : MonoBehaviour
{
	[Range(0,50)]
	public int segments = 50;
	[Range(0,5)]
	public float xRadius = 5;
	[Range(0,5)]
	public float yRadius = 5;
	LineRenderer line;
	[Tooltip("Attention ne pas faire de bêtises avec, demandez à Loann")] public Mesh meshOfLine;
	
	void Start ()
	{
		line = gameObject.GetComponent<LineRenderer>();

		line.SetVertexCount (segments + 1);
		line.useWorldSpace = false;
		CreatePoints ();
	}

	private void Update()
	{
		RaycastHit hit;
		// Does the ray intersect any objects excluding the player layer
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
		{
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
			Debug.Log("Did Hit");
		}
	}

	void CreatePoints ()
	{
		float x;
		float z;

		float angle = 20f;

		for (int i = 0; i < (segments + 1); i++)
		{
			x = Mathf.Sin (Mathf.Deg2Rad * angle) * xRadius;
			z = Mathf.Cos (Mathf.Deg2Rad * angle) * yRadius;

			line.SetPosition (i,new Vector3(x,0,z) );

			angle += (360f / segments);
		}
		
		line.BakeMesh(meshOfLine);
	}
	//TODO Wind on horizontal axis

	//TODO Stun player if they are touched by it
	

	/*private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position, radius);
		Gizmos.DrawWireSphere(transform.position, radius);
		Gizmos.DrawWireCube(transform.position, new Vector3(10, 10, 10));
	}*/
}