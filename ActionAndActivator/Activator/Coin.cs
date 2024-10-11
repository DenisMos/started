using UnityEngine;

namespace Assets.Scripts
{
	public class Coin : ActivatorBase
	{
		private void OnTriggerEnter2D(Collider2D collision)
		{
			IsSignal = !IsSignal;
			Execute();
			Destroy(this.gameObject, 0.1f);
		}
	}
}
