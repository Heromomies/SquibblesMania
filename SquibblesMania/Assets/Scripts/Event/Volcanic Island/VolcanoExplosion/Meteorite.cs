using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
	private Rigidbody _rb;
	private int _turn;
	public GameObject particleSystemExplosion;
	public GameObject particleSystemFire;
	public float speedTurnAround;
	private bool _stopRotating;
	private GameObject _particleFireToDelete;
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
			
			Destroy(_particleFireToDelete);
			Destroy(gameObject);
		}

		if (!_stopRotating)
		{
			var rotate = Random.Range(0.5f, 3f);
			transform.Rotate(new Vector3(rotate,rotate,rotate) * (speedTurnAround * Time.deltaTime), Space.World);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Black Block"))
		{
			StartCoroutine(SetActiveFalseBullet());
			
			other.gameObject.GetComponent<Node>().isActive = false;
			transform.parent = other.transform;
			_turn = GameManager.Instance.turnCount;

			transform.rotation = new Quaternion(0,0,0,0);
			_stopRotating = true;
			
			GameObject explosionPS = PoolManager.Instance.SpawnObjectFromPool("ExplosionVFXMeteorite", transform.position, Quaternion.identity);
			Destroy(explosionPS, 2f);
			
			GameObject firePS = PoolManager.Instance.SpawnObjectFromPool("FireVFXMeteorite", transform.position, Quaternion.identity);
			_particleFireToDelete = firePS;
			_rb.constraints = RigidbodyConstraints.FreezeAll;
			
			transform.position = new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);

			transform.rotation = other.transform.rotation;
		}
		else if (other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<PlayerStateManager>().StunPlayer(other.gameObject.GetComponent<PlayerStateManager>(), 1);
			StartCoroutine(SetActiveFalseBullet());
		}
	}

	IEnumerator SetActiveFalseBullet()
	{
		yield return new WaitForSeconds(2f);
		gameObject.SetActive(false);
	}
}