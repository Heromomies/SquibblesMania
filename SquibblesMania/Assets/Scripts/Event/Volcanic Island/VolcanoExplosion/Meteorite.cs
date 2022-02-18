using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
	public float speedTurnAround;
	
	private Rigidbody _rb;
	private int _turn;
	
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
		if (other.gameObject.CompareTag("BlackBlock"))
		{
			StartCoroutine(SetActiveFalseBullet(2f));
			
			other.gameObject.GetComponent<Node>().isActive = false;
			transform.parent = other.transform;
			_turn = GameManager.Instance.turnCount;

			transform.rotation = new Quaternion(0,0,0,0);
			_stopRotating = true;

			var transformPlayer = transform.position;
			
			GameObject explosionPS = PoolManager.Instance.SpawnObjectFromPool("ExplosionVFXMeteorite", transformPlayer, Quaternion.identity, null);
			Destroy(explosionPS, 2f);

			var otherPosition = other.transform.position;
			GameObject firePS = PoolManager.Instance.SpawnObjectFromPool("FireVFXMeteorite",
				new Vector3(otherPosition.x, otherPosition.y + 1.25f, otherPosition.z), Quaternion.identity, null);
			_particleFireToDelete = firePS;
			
			_rb.constraints = RigidbodyConstraints.FreezeAll;
			
			transform.position = new Vector3(otherPosition.x, transformPlayer.y -0.1f, otherPosition.z);

			transform.rotation = other.transform.rotation;
		}
		else if (other.gameObject.CompareTag("Player"))
		{
			other.gameObject.GetComponent<PlayerStateManager>().StunPlayer(other.gameObject.GetComponent<PlayerStateManager>(), 1);
			StartCoroutine(SetActiveFalseBullet(0.01f));
		}
	}

	IEnumerator SetActiveFalseBullet(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		gameObject.SetActive(false);
	}
}