using System;
using System.Linq;
using System.Collections.Generic;

public class Ordering<T>
{
	
	public static Variable<T> OrderMRV(CSP<T> csp)
	{
		return FindMaxVariable(csp.GetUnassignedVars(), p => p.CurDomainSize());
	}
	
	private static Variable<T> FindMaxVariable(List<Variable<T>> list, Func<Variable<T>, int> func)
	{
		if (list.Count == 0)
		{
			throw new InvalidOperationException("Empty list");
		}
		int max = int.MinValue;
		Variable<T> maxVar = null; 
		foreach (Variable<T> type in list)
		{
			if (func(type) > max)
			{
				max = func(type);
				maxVar = type; 
			}
		}
		return maxVar; 
	}
	
}
