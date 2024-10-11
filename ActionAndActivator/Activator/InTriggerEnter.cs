using UnityEngine;

namespace Assets.ActionAndActivator.Activator
{
	[RequireComponent(typeof(Collider2D))]
	public class InTriggerEnter : ActivatorBase
	{
		[SerializeField]
		[Header("Включается когда объект заходит в зону")]
		private bool OnWhenEnter;
		[SerializeField]
		[Header("Выключается когда объект заходит в зону")]
		private bool OffWhenEnter;

		[SerializeField]
		[Header("Включается когда объект выходит из зоны")]
		private bool OnWhenExit;
		[SerializeField]
		[Header("Выключается когда объект выходит из зоны")]
		private bool OffWhenExit;

		[SerializeField]
		[Header("Выключает отображение объекта")]
		private bool OffRender;

		private void Start()
		{
			this.GetComponents<Collider2D>()?.Foreach(x=> {
				if(!x.isTrigger)
				{
					Debug.LogWarning($"Включите {x.isTrigger} в колидере. Объект: {this.name}");
					x.isTrigger = true;
				}
			});

			if(OffRender) {
				var mesh = this.GetComponent<Renderer>();
				if(mesh != null)
				{ 
					mesh.enabled = false;
				}
			}
		}

		private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
		{
			if(OffWhenEnter && OnWhenEnter)
			{
				IsSignal = !IsSignal;
				Execute();
			}
			else
			{
				if(OnWhenEnter)
				{
					IsSignal = true;
					Execute();
				}
				if(OffWhenEnter)
				{
					IsSignal = false;
					Execute();
				}
			}
		}

		private void OnTriggerExit2D(UnityEngine.Collider2D collision)
		{
			if(OnWhenExit && OffWhenExit)
			{
				IsSignal = !IsSignal;
				Execute();
			}
			else
			{
				if(OnWhenExit)
				{
					IsSignal = true;
					Execute();
				}
				if(OffWhenExit)
				{
					IsSignal = false;
					Execute();
				}
			}
		}
	}
}
