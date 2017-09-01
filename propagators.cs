using System;
using System.Linq;
using System.Linq.Expressions; 
using System.Collections.Generic;


public class Propagators<T>
{
	public static Tuple<bool, List<Tuple<Variable<T>, T>>> PropGAC(CSP<T> csp, Variable<T> newVar)
	{
		List<Variable<T>> uasgnVars = csp.GetUnassignedVars(); 
		HashSet<Constraint<T>> GACQueue = new HashSet<Constraint<T>>();
		List<Tuple<Variable<T>, T>> pruned = new List<Tuple<Variable<T>, T>>(); 
		
		
		if(!uasgnVars.Any())
		{
			return Tuple.Create(true, new List< Tuple<Variable<T>, T> >()); 
		}
		if(newVar == null)
		{
			GACQueue = new HashSet<Constraint<T>>(csp.Cons); 
			return GACEnforce(csp, GACQueue, pruned);
		}
		
		foreach(T item in newVar.CurrentDomain())
		{
			if(!EqualityComparer<T>.Default.Equals(newVar.AssignedValue, item))
			{
				newVar.PruneValue(item);
				pruned.Add(new Tuple<Variable<T>, T>(newVar, item));
			}
		}
		GACQueue = new HashSet<Constraint<T>>(csp.GetConsWithVar(newVar)); 
		return GACEnforce(csp, GACQueue, pruned); 
	}
	
	private static Tuple<bool, List<Tuple<Variable<T>, T>>> GACEnforce(CSP<T> csp, HashSet<Constraint<T>> GACQueue, List<Tuple<Variable<T>, T>> pruned)
	{
		bool wasPruned = false; 
		while(GACQueue.Any())
		{
			Constraint<T> nextCon = GACQueue.ToList()[0];
			GACQueue.Remove(nextCon); 
			foreach(Variable<T> var in nextCon.GetUnasgnVars())
			{
				foreach(T item in var.CurrentDomain())
				{
					if(!nextCon.HasSupport(var, item))
					{
						///Console.WriteLine(String.Format("Pruning {0} = {1}", var, item));
						var.PruneValue(item);
						pruned.Add(new Tuple<Variable<T>, T>(var, item));
						wasPruned = true; 
						if(!var.CurrentDomain().Any()){
							return new Tuple<bool,List<Tuple<Variable<T>, T>>>(false, pruned); 
						}
					}
					
					if(wasPruned)
					{
						GACQueue.UnionWith(csp.GetConsWithVar(var));
						wasPruned = false; 
					}
				}
			}
		}
		return new Tuple<bool,List<Tuple<Variable<T>, T>>>(true, pruned);  
	}
}