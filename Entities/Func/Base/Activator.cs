using System;
using Assets.Scripts.Entities.Func.Utilits;

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

		public virtual void Execute()
		{
			UnityAPI.ExecuteUnityEvent(_events);
			FuncFrameworkAPI.ExecuteFuncAPI(this, _actionWithParams);
		}

		public virtual void Execute(SwitcherTriggerContext context)
		{
			UnityAPI.ExecuteUnityEvent(_events);
			FuncFrameworkAPI.ExecuteFuncAPI(this, _actionWithParams, context);
		}

#if UNITY_EDITOR_64

		private void OnDrawGizmos()
		{
			_actionWithParams.InfoDrawDistance(transform.position);
		}
#endif
	}
}
