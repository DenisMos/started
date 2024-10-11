using System;
using UnityEngine;

/// <summary>Описывает действие на сцене.</summary>
public abstract class ActionBase : MonoBehaviour
{
	[SerializeField]
	private bool _isTest;

	public bool IsTest 
	{ 
		get => _isTest;
		set
		{
			if(_isTest != value)
			{
				if(value)
				{
					Test();
				}
			}
		}
	}

	public abstract void Execute(ActivatorBase activator);

	public virtual void Test()
	{
		Debug.Log("Test complited");
	}
}

/// <summary>Описывает действие на сцене.</summary>
public abstract class ActivatorBase : MonoBehaviour
{
	[SerializeField]
	private bool _isSignal;

	[SerializeField]
	public ActionBase[] _action;

	public bool IsSignal 
	{
		get => _isSignal;
		set
		{ 
			_isSignal = value;
		}
	}

	public virtual void Execute()
	{
		if(_action == null) return;

		foreach(var item in _action)
		{
			item.Execute(this);
		}
	}

#if UNITY_EDITOR_64

	private void OnDrawGizmos()
	{
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
