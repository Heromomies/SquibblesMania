using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
	private Rigidbody _rb;
	private int _turn;
	public GameObject particleSystem;
	public Material m;
	public List<Transform> t = new List<Transform>();

	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (_turn != 0 && GameManager.Instance.turnCount >= _turn + 4)
		{
			gameObject.GetComponentInParent<Node>().isActive = true;
			
			_turn = GameManager.Instance.turnCount;
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Black Block"))
		{
			other.gameObject.GetComponent<Node>().isActive = false;
			transform.parent = other.transform;
			_turn = GameManager.Instance.turnCount;
			
			Instantiate(particleSystem, transform.position, Quaternion.identity);
			_rb.constraints = RigidbodyConstraints.FreezeAll;
			transform.position = new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);

			transform.rotation = other.transform.rotation;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			EventManager.Instance.StunPlayer(other.gameObject);
			Destroy(gameObject);
		}
	}
}