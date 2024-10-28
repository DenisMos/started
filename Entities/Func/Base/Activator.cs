using Assets.Scripts.Entities.Func;
using Assets.Scripts.Entities.Func.Utilits;
using System;
using System.Linq;

using UnityEngine;

namespace Assets.Scripts.Entities
{

	public class ActivatorBase3D : MonoBehaviour
	{
		[Obsolete("Используйте 'Action With Params'")]
		[SerializeField] private ActionBase3D[] _actions;
		[SerializeField] private ActionWithContext[] _actionWithParams;

		public ActionWithContext[] Action => _actionWithParams;

		public virtual void Execute()
		{
			foreach(var action in _actionWithParams)
			{
				action.Action.Execute(this, action.Context);
			}
		}

		public virtual void Execute(SwitcherTriggerContext context)
		{
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
