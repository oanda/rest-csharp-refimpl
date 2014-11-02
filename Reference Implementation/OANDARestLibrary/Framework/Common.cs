using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OANDARestLibrary.Framework
{
	public class Common
	{
		public static object GetDefault(Type t)
		{
			return typeof(Common).GetTypeInfo().GetDeclaredMethod("GetDefaultGeneric").MakeGenericMethod(t).Invoke(null, null);
		}

		public static T GetDefaultGeneric<T>()
		{
			return default(T);
		}
	}
}
