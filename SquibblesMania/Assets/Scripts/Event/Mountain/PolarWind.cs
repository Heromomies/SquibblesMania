using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PolarWind : MonoBehaviour, IManageEvent
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
	
	public void ShowEvent()
	{
		transform.position = new Vector3(transform.position.x,  heightWind, transform.position.z);
	
		//blockAtHeight = GameObject.FindGameObjectsWithTag("Platform");

		_line = gameObject.GetComponent<LineRenderer>();
		_line.SetVertexCount (segments + 1);
		_line.useWorldSpace = false;
		
		_stopLoop = false;
	}

	public void LaunchEvent()
	{
		StartCoroutine(CreatePoints ());
	}
	IEnumerator CreatePoints ()
	{
		for (int i = 0; i < (segments +1); i++)
		{
			if (_stopLoop)
			{
				break;
			}
			gameObject.GetComponent<MeshCollider>().convex = false;
			yield return new WaitForSeconds(speed);
			
			var x = Mathf.Sin (Mathf.Deg2Rad * startAngle) * xRadius;
			var z = Mathf.Cos (Mathf.Deg2Rad * startAngle) * yRadius;

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
		if(other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<PlayerStateManager>().StunPlayer(other.gameObject.GetComponent<PlayerStateManager>(), 3);
		}
	}

	private void OnApplicationQuit()
	{
		meshOfLine.Clear();
	}
	//TODO Stun player if they are touched by it

}