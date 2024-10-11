using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Teleport : ActivatorBase
{
	private Rigidbody2D[] _listenerPlayer;

	[SerializeField]
	private TeleportDestination _target;
	[SerializeField]
	private bool _state;

	/// <summary>�������������.</summary>
	void Start()
	{
		//�������� ��� ������� �� �����, ��� ����� Rigidbody2D.
		_listenerPlayer = FindObjectsOfType<Rigidbody2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//���������� ���� �������
		foreach(var player in _listenerPlayer)
		{
			//�������� ���� �� ���������� ������ ���� � _listenerPlayer
			if(player.transform == collision.transform)
			{
				player.transform.position = _target.transform.position;
			}
		}

		Execute();
	}
}
