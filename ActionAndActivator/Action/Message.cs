using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.started.ActionAndActivator.Action
{
	internal class Message : ActionBase
	{
		[SerializeField]
		private string _message;

		public override void Execute(ActivatorBase activator)
		{
			Debug.Log($"{_message}");
		}
	}
}
