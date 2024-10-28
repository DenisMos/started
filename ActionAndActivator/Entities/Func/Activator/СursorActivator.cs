using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Func.Utilits;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class СursorActivator : ActivatorBase3D
{
	private MeshRenderer _renderer;
	private Color _cacheColor;

	[Header("Activators -----")]
	[SerializeField] private SwitcherTriggerContext _mode;
	[Header("Highlighting -----")]
	[SerializeField] private bool IsSelectable;
	[ColorUsage(true)][SerializeField] private Color _color;
	[Header("Listeners -----")]
	[SerializeField] private float _distance = 10;
	[SerializeField] private EqualsMode _equelsMode = EqualsMode.Less;
	[SerializeField] private bool _autoFindPlayer;
	[SerializeField] private List<GameObject> _listeners;

	private void Start()
	{
		_renderer = GetComponent<MeshRenderer>();
		_cacheColor = _renderer.material.color;

		if(_autoFindPlayer)
		{
			_listeners = GameObject.FindGameObjectsWithTag("Player").ToList();
		}
	}

	private bool CheckNeedConditions()
	{
		if(_listeners == null || _listeners.Count == 0)
		{
			return true;
		}

		switch(_equelsMode)
		{
			case EqualsMode.Equels:
				return _listeners.Any(x => Vector3.Distance(x.transform.position, transform.position) == _distance);
			case EqualsMode.Less:
				return _listeners.Any(x => Vector3.Distance(x.transform.position, transform.position) <= _distance);
			case EqualsMode.More:
				return _listeners.Any(x => Vector3.Distance(x.transform.position, transform.position) >= _distance);
		}

		return false;
	}

	private void OnMouseDown()
	{
		if(!CheckNeedConditions()) return;

		Execute(_mode);
	}

	private void OnMouseEnter()
	{
		if(!CheckNeedConditions()) return;

		if(IsSelectable)
		{
			_cacheColor              = _renderer.material.color;
			_renderer.material.color = _color;
		}
	}

	private void OnMouseExit()
	{
		if(!CheckNeedConditions()) return;

		if(IsSelectable)
		{
			_renderer.material.color = _cacheColor;
		}
	}

	private void OnMouseUp()
	{
	}
}
