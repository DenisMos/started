using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ActionAndActivator.Activator
{
	public class LeverActivator : ActivatorBase
	{
		public KeyCode KeyCode = KeyCode.F;

		public GameObject Player;
		public float Distance = 1;

		public Sprite Sprite1;
		public Sprite Sprite2;

		private SpriteRenderer SpriteRenderer;

		private void Start()
		{
			if(Player == null)
			{
				Player = GameObject.FindGameObjectWithTag("Player");
			}

			SpriteRenderer = this.GetComponent<SpriteRenderer>();
		}

		private void Update()
		{
			if(transform.GetDistance2D(Player.transform) >= Distance) return;

			if(Input.GetKeyDown(KeyCode))
			{
				IsSignal = !IsSignal;

				Execute();
			}

			if(IsSignal)
			{
				if(SpriteRenderer != null)
				{
					if(Sprite1 != null)
					{
						SpriteRenderer.sprite = Sprite1;
					}
				}
			}
			else
			{
				if(SpriteRenderer != null)
				{
					if(Sprite2 != null)
					{
						SpriteRenderer.sprite = Sprite2;
					}
				}
			}
		}

#if UNITY_EDITOR_64

		private void OnDrawGizmos()
		{
			if(SpriteRenderer == null)
			{
				SpriteRenderer = this.GetComponent<SpriteRenderer>();
			}
			if(SpriteRenderer == null)
			{
				return;
			}

			if(IsSignal)
			{
				if(Sprite1 != null)
				{
					SpriteRenderer.sprite = Sprite1;
				}
			}
			else
			{
				if(Sprite2 != null)
				{
					SpriteRenderer.sprite = Sprite2;
				}
			}

			if(_action != null)
			{
				Gizmos.color = IsSignal ? Color.red : Color.green;
				foreach(var item in _action)
				{
					if(item != null)
					{
						Gizmos.DrawLine(transform.position, item.transform.position);
					}
				}

			}
			if(_action == null || _action.Length == 0)
			{
				Gizmos.color = new Color(255, 0, 0, 10);
				Gizmos.DrawWireSphere(transform.position, 3);
			}
		}

#endif
	}
}
