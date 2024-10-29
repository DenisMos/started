using Assets.Scripts.Entities.Func;
using Assets.Scripts.Entities.Func.Utilits;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Entities
{
	public class ActivatorBase3D : MonoBehaviour
	{
		#region Activator

		[Header("Activator API")]
		[SerializeField] private ActionWithContext[] _actionWithParams;
		[Header("Unity API")]
		[SerializeField] private UnityEvent _events;

		#endregion

		public ActionWithContext[] Action => _actionWithParams;

		private void ExecuteUnityEvent()
		{
			if(_events != null)
			{
				_events.Invoke();
			}
		}

		public virtual void Execute()
		{
			ExecuteUnityEvent();
			foreach(var action in _actionWithParams)
			{
				action.Action.Execute(this, action.Context);
			}
		}

		public virtual void Execute(SwitcherTriggerContext context)
		{
			ExecuteUnityEvent();
			foreach(var action in _actionWithParams)
			{
				action.Action.Execute(this, action.Context);
			}
		}

#if UNITY_EDITOR_64

		private void OnDrawGizmos()
		{
			if(_actionWithParams != null)
			{
				foreach(var item in _actionWithParams)
				{
					if(item == null || item.Action == null) continue;

					item.name = $"{item.Action} : {item.Context}";

					Gizmos.DrawLine(transform.position, item.Action.transform.position);
				}
			}
		}

#endif
	}

	public abstract class ActionBase3D : MonoBehaviour
	{
		private IActionOtherModule[] _actionModuleStart;
		private IActionOtherModule[] _actionModuleEnd;

		/// <summary>Инциализирует доп модули для действий.</summary>
		protected void InitialModules()
		{
			var modules = GetComponents<IActionOtherModule>();

			_actionModuleEnd = modules.Where(x => x.Mode == ActionOtherMode.End).ToArray();
			_actionModuleStart = modules.Where(x => x.Mode == ActionOtherMode.Start).ToArray();
		}

		/// <summary>Выполняет доп модули конца действия.</summary>
		/// <param name="switherTriggerContext"></param>
		protected void ExecuteStartModules(SwitcherTriggerContext switherTriggerContext)
		{
			foreach(var item in _actionModuleStart)
			{
				item.Execute(switherTriggerContext);
			}
		}

		/// <summary>Выполняет доп модули начала действия.</summary>
		/// <param name="switherTriggerContext"></param>
		protected void ExecuteEndModules(SwitcherTriggerContext switherTriggerContext)
		{
			foreach(var item in _actionModuleEnd)
			{
				item.Execute(switherTriggerContext);
			}
		}

		public virtual bool Execute(ActivatorBase3D trigger)
		{
			return Execute(trigger, SwitcherTriggerContext.Toggle);
		}

		public virtual bool Execute(ActivatorBase3D trigger, SwitcherTriggerContext context = SwitcherTriggerContext.Toggle)
		{
			return false;
		}
	}
}
