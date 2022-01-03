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
	[Range(0,0.5f)]
	public float speed;
	
	public float startAngle = 20f;

	public int heightWind;
	
	
	[Tooltip("Attention ne pas faire de bêtises avec, demandez à Loann")] public Mesh meshOfLine;
	
	private LineRenderer _line;
	void Start ()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y + heightWind, transform.position.z);
		
		_line = gameObject.GetComponent<LineRenderer>();
		_line.SetVertexCount (segments + 1);
		_line.useWorldSpace = false;
		
		StartCoroutine(CreatePoints ());
	}

	private void Update()
	{
		RaycastHit hit;
		// Does the ray intersect any objects excluding the player layer
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
		{
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
		}
	}

	IEnumerator CreatePoints ()
	{
		float x;
		float z;

		for (int i = 0; i < (segments + 1); i++)
		{
			yield return new WaitForSeconds(speed);
			
			x = Mathf.Sin (Mathf.Deg2Rad * startAngle) * xRadius;
			z = Mathf.Cos (Mathf.Deg2Rad * startAngle) * yRadius;

			_line.SetPosition (i,new Vector3(x,0,z) );

			startAngle += (360f / segments);

			_line.BakeMesh(meshOfLine);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		Debug.Log("I touched a thing and i break the coroutine");
		StopCoroutine(CreatePoints());
	}

	private void OnApplicationQuit()
	{
		Debug.Log("I'm going through this debug");
		meshOfLine.Clear();
	}
	//TODO Stun player if they are touched by it
}