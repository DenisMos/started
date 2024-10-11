using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ActionAndActivator
{
	public static class ItemExtansion
	{
		public static void Foreach<T>(this IEnumerable<T> values, Action<T> action)
		{
			foreach(var item in values)
			{ 
				action.Invoke(item);
			}
		}
	}
}
