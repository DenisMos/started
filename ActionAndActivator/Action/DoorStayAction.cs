using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ActionAndActivator.Action
{
	public class DoorStayAction : ActionBase
	{
		public override void Execute(ActivatorBase activator)
		{
			this.Active = activator.IsSignal;
		}

		public Vector3[] Target;
		public bool Active = false;
		private int _pointer;
		private Transform _player;
		private Vector3 _delta;

		private void Start()
		{
			_pointer = 0;
		}

		public bool IsActive
		{
			get;
			set;
		}

		private void FixedUpdate()
		{
			if(Active)
			{
				var targetPosition = Target[_pointer];
				var distance = Vector2.Distance(transform.position, targetPosition);
				var oldPosition = transform.position;
				transform.position = Vector3.Lerp(oldPosition, targetPosition, 1 / distance * Time.fixedDeltaTime);
				if(_player != null)
				{
					_player.position += transform.position - oldPosition;
				}
				if(distance <= 0.1f)
				{
					_pointer++;
					if(_pointer >= Target.Length)
					{
						_pointer = 0;
					}
				}
			}
		}

		private void OnDrawGizmosSelected()
		{
			var oldPositin = transform.position;
			foreach(var item in Target)
			{
				Gizmos.DrawLine(oldPositin, item);
				var pos = Vector2.Lerp(oldPositin, item, 0.5f);
				var dist = Vector2.Distance(oldPositin, item);
				oldPositin = item;
				Gizmos.DrawSphere(item, 1f / 10);
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			_player = collision.transform;
		}

		private void OnCollisionExit2D(Collision2D collision)
		{
			_player = null;
		}

		private void OnCollisionStay2D(Collision2D collision)
		{
			Debug.Log(collision.gameObject.name);
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{

		}

		private void OnTriggerStay2D(Collider2D collision)
		{

		}

		private void OnTriggerExit2D(Collider2D collision)
		{

		}
	}
}
