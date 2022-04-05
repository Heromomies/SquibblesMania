using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
	public float speedTurnAround;
	public int lifeParticle;
	
	private Rigidbody _rb;
	private int _turn;
	
	public bool stopRotating;
	private GameObject _particleFireToDelete;
	
	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (_turn != 0 && GameManager.Instance.turnCount >= _turn + lifeParticle)
		{
			gameObject.GetComponentInParent<Node>().isActive = true;
			_particleFireToDelete.gameObject.SetActive(false);
			gameObject.SetActive(false);
		}

		if (!stopRotating)
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

			AudioManager.Instance.Play("FireballEnd");
			
			transform.rotation = new Quaternion(0,0,0,0);
			stopRotating = true;

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
			AudioManager.Instance.Play("Stun");
			
			//other.gameObject.GetComponent<PlayerStateManager>().StunPlayer(other.gameObject.GetComponent<PlayerStateManager>(), 1);
			var otherPos = other.transform.position;
			//GameObject stunVFX = PoolManager.Instance.SpawnObjectFromPool("StunVFX",new Vector3(otherPos.x, otherPos.y + 0.75f, otherPos.z), Quaternion.identity, null);

			//other.gameObject.GetComponent<PlayerStateManager>().psStun = stunVFX;
			
			StartCoroutine(SetActiveFalseBullet(0.01f));
		}
	}

	IEnumerator SetActiveFalseBullet(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		gameObject.transform.position = new Vector3(1500,-1500, 1500);
		gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
	}
}