using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
	private Rigidbody _rb;
	public GameObject particleSystem;

	public List<Transform> t = new List<Transform>();

	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Black Block"))
		{
			foreach (var g in other.gameObject.GetComponent<Node>().possiblePath)
			{
				t.Add(g.nextPath);
				Debug.Log(t);
			}

			for (int i = 0; i < t.Count; i++)
			{
				if (t[i].GetComponent<Node>().possiblePath[i].nextPath.gameObject.name == other.gameObject.name)
				{
					Debug.Log("I'm here ma boy");
				}
			}

			Instantiate(particleSystem, transform.position, Quaternion.identity);
			_rb.constraints = RigidbodyConstraints.FreezeAll;
			transform.position = new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);

			transform.rotation = other.transform.rotation;
		}
	}
}