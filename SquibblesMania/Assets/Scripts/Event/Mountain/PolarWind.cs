using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PolarWind : MonoBehaviour
{
	[Range(0,50)]
	public int segments = 50;
	[Range(0,5)]
	public float xRadius;
	[Range(0,5)]
	public float yRadius = 5;
	[Range(0,0.5f)]
	public float speed;
	public float startAngle = 20f;
	public float heightWind;

	[Tooltip("Attention ne pas faire de bêtises avec, demandez à Loann")] public Mesh meshOfLine;

	private bool _stopLoop;
	private LineRenderer _line;
	
	void Start ()
	{
		transform.position = new Vector3(transform.position.x,  heightWind, transform.position.z);
	
		//blockAtHeight = GameObject.FindGameObjectsWithTag("Platform");

		_line = gameObject.GetComponent<LineRenderer>();
		_line.SetVertexCount (segments + 1);
		_line.useWorldSpace = false;
		
		_stopLoop = false;
		
		StartCoroutine(CreatePoints ());
	}

	IEnumerator CreatePoints ()
	{
		float x;
		float z;

		for (int i = 0; i < (segments +1); i++)
		{
			if (_stopLoop)
			{
				break;
			}
			gameObject.GetComponent<MeshCollider>().convex = false;
			yield return new WaitForSeconds(speed);
			
			x = Mathf.Sin (Mathf.Deg2Rad * startAngle) * xRadius;
			z = Mathf.Cos (Mathf.Deg2Rad * startAngle) * yRadius;

			_line.SetPosition (i,new Vector3(x,0,z) );

			startAngle += (360f / segments +1);

			_line.BakeMesh(meshOfLine);
			
			gameObject.GetComponent<MeshCollider>().convex = true;
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Black Block"))
		{
			Debug.Log("I touched a thing and i break the coroutine");
			_stopLoop = true;
			StopCoroutine(CreatePoints());
		}
	}

	private void OnApplicationQuit()
	{
		meshOfLine.Clear();
	}
	//TODO Stun player if they are touched by it
}