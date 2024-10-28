using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Func.Utilits;
using System.Collections;
using UnityEngine;

public class EasyFuncPlatform : ActionBase3D
{
	private Coroutine _coroutine;
	private Vector3 _startPosition;

	[SerializeField] private Vector3 _targetPosition;
	[SerializeField] private bool _isOpened;
	[SerializeField] private MethodsAction _methodMove;
	[SerializeField] private float _speed;

	private IInterpolateAlgs<Vector3> _interpAlgs;

	private void Start()
	{
		_startPosition = transform.position;
		_interpAlgs = VectorInterpolate.CreateAlgs(_methodMove);

		InitialModules();
	}

	private void MoveFromStartToTarget(Vector3 start, Vector3 end)
	{
		transform.position = _interpAlgs.Interp(start, end, _speed * Time.fixedDeltaTime);
	}

	private IEnumerator Open()
	{
		ExecuteStartModules(SwitcherTriggerContext.On);

		while(_targetPosition != transform.position)
		{
			MoveFromStartToTarget(transform.position, _targetPosition);
			yield return new WaitForFixedUpdate();
		}
		transform.position = _targetPosition;

		ExecuteEndModules(SwitcherTriggerContext.On);
	}

	private IEnumerator Close()
	{
		ExecuteStartModules(SwitcherTriggerContext.Off);

		while(_startPosition != transform.position)
		{
			MoveFromStartToTarget(transform.position, _startPosition);
			yield return new WaitForFixedUpdate();
		}
		transform.position = _startPosition;

		ExecuteEndModules(SwitcherTriggerContext.Off);
	}

	public override bool Execute(ActivatorBase3D trigger, SwitcherTriggerContext context)
	{
		if(_coroutine != null)
		{
			StopCoroutine(_coroutine);
			_coroutine = null;
		}

		if(context == SwitcherTriggerContext.On)
		{
			if(!_isOpened)
			{
				_isOpened = true;
				_coroutine = StartCoroutine(Open());
				return true;
			}
		}
		else if(context == SwitcherTriggerContext.Off)
		{
			if(_isOpened)
			{
				_isOpened = false;
				_coroutine = StartCoroutine(Close());
				return true;
			}
		}
		else if(context == SwitcherTriggerContext.Toggle)
		{
			if(_isOpened)
			{
				_isOpened = false;
				_coroutine = StartCoroutine(Close());
				return true;
			}
			else
			{
				_isOpened = true;
				_coroutine = StartCoroutine(Open());
			}
		}
		return true;
	}

	[UnityEngine.ContextMenu("Set position")]
	public void SetPosition()
	{
		_targetPosition = transform.position;
	}
}
