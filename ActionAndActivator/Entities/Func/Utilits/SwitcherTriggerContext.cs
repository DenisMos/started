using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities.Func.Utilits
{
	public enum SwitcherTriggerContext
	{
		Toggle,
		None,
		On,
		Off,
		Block,
		Unblock,
		Wait,
	}

	public enum EqualsMode
	{
		More,
		Less,
		Equels
	}
}
