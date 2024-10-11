using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveController : MonoBehaviour
{
	private Rigidbody2D _rigidbody2D;
	private bool _isAir;

	[SerializeField]
	private float Speed = 1f;
	[SerializeField]
	private float JumpAmpl = 2f;

	private void Start()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		if(Input.GetButton("Jump") && !_isAir)
		{
			_rigidbody2D.AddForce(Vector2.up * JumpAmpl, ForceMode2D.Impulse);
			_isAir = true;
		}

		var delta = Input.GetAxisRaw("Horizontal");
		if(delta != 0 && _rigidbody2D.velocity.x < 5 && _rigidbody2D.velocity.x > -5)
		{
			_rigidbody2D.AddForce(new Vector2(delta, 0) * 0.05f * Speed, ForceMode2D.Impulse);
		}
		else
		{
			if(_isAir)
			{
				_rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x / 1.01f, _rigidbody2D.velocity.y);
			}
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if(collision.contacts[0].normal == Vector2.up)
		{
			_isAir = false;
		}

		Debug.DrawRay(_rigidbody2D.position, collision.contacts[0].normal);
		Debug.DrawRay(_rigidbody2D.position, _rigidbody2D.velocity, Color.red);
	} 

	private void OnCollisionExit2D(Collision2D collision)
	{
		_isAir = true;
	}
}
