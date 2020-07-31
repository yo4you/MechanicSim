using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public static class BaseMechanics
{
	public static Lazy<List<Mechanic>> Mechanics = new Lazy<List<Mechanic>>(InitializeMechanics);

	private static List<Mechanic> InitializeMechanics()
	{
		return (from info in typeof(BaseMechanicDefinitions).GetMethods()
				where info.DeclaringType != typeof(object)
				let parameters = from param in info.GetParameters()
								 select new KeyValuePair<string, Type>(param.Name, param.ParameterType)

				select new Mechanic()
				{
					Name = info.Name,
					TimeLine = null,
					ParameterTypes = parameters.ToDictionary(kv => kv.Key, kv => kv.Value),
					FuncCall = info.Name
				}).ToList();
	}
}