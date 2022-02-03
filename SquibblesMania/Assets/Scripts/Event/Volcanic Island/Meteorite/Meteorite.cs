using UnityEngine;

public class Meteorite : MonoBehaviour
{
	private Rigidbody _rb;
	private int _turn;
	public GameObject particleSystem;
	public float speedTurnAround;
	private bool _stopRotating;
	
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

		if (!_stopRotating)
		{
			var rotate = Random.Range(0.5f, 3f);
			transform.Rotate(new Vector3(rotate,rotate,rotate) * speedTurnAround * Time.deltaTime, Space.World);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Black Block"))
		{
			other.gameObject.GetComponent<Node>().isActive = false;
			transform.parent = other.transform;
			_turn = GameManager.Instance.turnCount;

			transform.rotation = new Quaternion(0,0,0,0);
			_stopRotating = true;
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
			other.GetComponent<PlayerStateManager>().StunPlayer(other.gameObject.GetComponent<PlayerStateManager>(), 2);
			Destroy(gameObject);
		}
	}
}