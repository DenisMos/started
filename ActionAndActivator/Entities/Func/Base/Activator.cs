using Assets.Scripts.Entities.Func;
using Assets.Scripts.Entities.Func.Utilits;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

namespace Assets.Scripts.Entities
{
	public class ActivatorBase3D : MonoBehaviour
	{
		[SerializeField]
		private ActionBase3D[] _actions;

		public ActionBase3D[] Actions => _actions;

		public virtual void Execute(SwitcherTriggerContext context)
		{
			foreach(var action in _actions)
			{
				action.Execute(this, context);
			}
		}

		public virtual void Execute()
		{
			foreach(var action in _actions)
			{ 
				action.Execute(this);
			}
		}

#if UNITY_EDITOR_64

		private void OnDrawGizmos()
		{
			if(_actions != null)
			{
				foreach(var item in _actions)
				{
					if(item == null) continue;

					Gizmos.DrawLine(transform.position, item.transform.position);
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
