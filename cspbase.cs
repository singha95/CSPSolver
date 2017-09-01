using System;
using System.Linq;
using System.Linq.Expressions; 
using System.Collections.Generic;


public class Variable<T>
{	
	private string name; 
	private List<T> domain;
	private List<bool> curdom; 
	private T assignedValue;  
	
	public string Name
	{ 
		get{ 
			return name; 
		}
		
	}
	
	public List<T> Domain
	{
		get
		{
			return domain; 
		}
	}
	
	public T AssignedValue
	{
		get
		{
			return assignedValue;
		}
	}
	
	public Variable(string varName, List<T> domainVal)
	{
		name = varName; 
		domain = domainVal; 
		curdom = Enumerable.Repeat(true, domainVal.Count).ToList();
		assignedValue = default(T); 
	}
	
	public bool IsAssigned()
	{
		return !EqualityComparer<T>.Default.Equals(assignedValue, default(T));
	}
	
	public void AddDomainValues(List<T> values)
	{
		for(int i=0; i <values.Count; i++)
		{
			domain.Add(values[i]);
			curdom.Add(true);
		}
	}
	
	public int DomainSize()
	{
		return domain.Count;
	}
	
	
	///methods for current domain (pruning and unpruning)
	
	public void PruneValue(T value)
	{
		curdom[domain.IndexOf(value)] = false; 
	}
	
	public void UnpruneValue(T value)
	{
		curdom[domain.IndexOf(value)] = true; 
	}
	
	public List<T> CurrentDomain()
	{
		List<T> vals = new List<T>();
		if (!EqualityComparer<T>.Default.Equals(assignedValue, default(T)))
		{
			vals.Add(assignedValue);
		}
		else
		{
			for(int i=0; i<curdom.Count;i++)
			{
				if(curdom[i])
				{
					vals.Add(domain[i]);
				}
			}
		}
		
		return vals;
	}
	
	public bool InCurDomain(T value)
	{
		if (EqualityComparer<T>.Default.Equals(assignedValue,value)) 
		{
			return true; 
		}
		
		if(!domain.Contains(value))
		{
			return false; 
		}
		else 
		{
			return curdom[domain.IndexOf(value)];
		}
			
	}
	
	public int CurDomainSize()
	{
		if (!EqualityComparer<T>.Default.Equals(assignedValue, default(T)))
		{
			return 1; 
		}
		else 
		{
			return curdom.Where(c => c).Count();
		}
	}
	
	public void RestoreCurdom()
	{
		curdom = Enumerable.Repeat(true, domain.Count).ToList();
	}
	
	public void Assign(T value)
	{
		
		if(!EqualityComparer<T>.Default.Equals(value, default(T)) &&
			domain.Contains(value))
		{
			assignedValue = value;
		}
	
	}
	
	public void Unassign()
	{
		assignedValue = default(T);
	}
	
	public override string ToString()
	{
		return String.Format("VAR--{0}", name);
	}  
	
	public void PrintAll()
	{
		Console.WriteLine(String.Format("VAR--{0}: DOM = {1}, CURDOM = {2}", 
				name, 
				String.Join("; ", domain), 
				String.Join("; ", curdom)));
	}
}

public class Constraint<T>
{
	private List<Variable<T>> scope; 
	private String name; 
	private Func<List<T>, bool> isSatisied;
	
	public string Name
	{
		get
		{
			return name;
		}
	}
	
	public List<Variable<T>> Scope
	{
		get
		{ 
			return scope; 
		}
	}
	
	public Constraint(List<Variable<T>> scopeVar, String conName, Func<List<T>, bool> lambda)
	{
		scope = scopeVar; 
		name = conName; 
		isSatisied = lambda;
	}
	
	public bool Check(List<T> vals)
	{
		return isSatisied(vals);
	}
	
	public override string ToString()
	{
		return String.Format("CONS--{0}", name);
	}  
	
	public int GetUnasgnVarsCount()
	{
		return GetUnasgnVars().Count();
	}
	
	public List<Variable<T>> GetUnasgnVars()
	{
		List<Variable<T>> unsignedVars = new List<Variable<T>>();
		foreach(Variable<T> var in scope)
		{
			if(!var.IsAssigned())
			{
				unsignedVars.Add(var);
			}
		}
		return unsignedVars;
	}
	
	public bool HasSupport(Variable<T> var, T value)
	{
		List<Variable<T>> numbers = new List<Variable<T>>(scope);
		List<List<T>> temp = new List<List<T>>();
		foreach(Variable<T> element in numbers)
		{
			if (element.Name == var.Name)
			{
				temp.Add(new List<T>{value});
			}
			else{
				temp.Add(element.CurrentDomain());
			}
		}
		
		var cross = CrossJoin(temp);
		foreach(IEnumerable<T> list in cross)
		{
			var stringList = list.OfType<string>();
			///Console.WriteLine(String.Format("{0} {1}", string.Join( ",", stringList ), Check(list.ToList())));
			if(Check(list.ToList()))
			{
				return true;
			}
		}
		return false;
	}
	
	private IEnumerable<IEnumerable<T>> CrossJoin(IEnumerable<IEnumerable<T>> sequences)
	{
		IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
		IEnumerable<IEnumerable<T>> result = emptyProduct;
		foreach (IEnumerable<T> sequence in sequences)
		{
			result = from accseq in result from item in sequence select accseq.Concat(new[] {item});
		}
		return result;
	}
}

public class CSP<T>
{	
	private String name; 
	private List<Variable<T>> vars; 
	private List<Constraint<T>> cons; 
	private Dictionary<Variable<T>, List<Constraint<T>>> varsToCons;
	
	
	public String Name
	{
		get{
			return name;
		}
	}
	public List<Variable<T>> Vars
	{
		get
		{
			return vars;
		}
	}
	
	public List<Constraint<T>>  Cons
	{
		get
		{
			return cons; 
		}
	}
	
	
	public CSP(String CSPname, List<Variable<T>> CSPvars = null)
	{
		name = CSPname;
		vars = CSPvars ?? new List<Variable<T>>();
		cons = new List<Constraint<T>>();
		varsToCons = new Dictionary<Variable<T>, List<Constraint<T>>>();
		foreach(Variable<T> variable in vars)
		{
			varsToCons.Add(variable, new List<Constraint<T>>());
		}
		
	}
	
	public void AddVar(Variable<T> var)
	{
		if(!vars.Contains(var)){
			vars.Add(var); 
			varsToCons.Add(var, new List<Constraint<T>>());
		}
	}
	
	public void AddConstraint(Constraint<T> con)
	{
		
		foreach(Variable<T> var in con.Scope)
		{
			if(!vars.Contains(var))
			{
				return;
			}
		}
		
		foreach(Variable<T> var in con.Scope)
		{
			varsToCons[var].Add(con);
		}
		cons.Add(con);
	}
	
	
	public List<Constraint<T>> GetConsWithVar(Variable<T> var)
	{
		return varsToCons[var]; 
	}
	
	public List<Variable<T>> GetUnassignedVars()
	{
		List<Variable<T>> unsignedvars = new List<Variable<T>>();
		foreach(Variable<T> var in vars)
		{
			if(!var.IsAssigned())
			{
				unsignedvars.Add(var);
			}
		}
		return unsignedvars; 
	}
	
	public void PrintSoln()
	{
		Console.WriteLine(String.Format("CSP {0} Assignments = ",name));
		foreach(Variable<T> variable in vars)
		{
			Console.WriteLine(variable.AssignedValue);
		}
	}
	
	private bool Compare(T x, T y)
	{
		return EqualityComparer<T>.Default.Equals(x, y);
	}
}

public class BT<T>
{
	private CSP<T> csp; 
	private int nDecisions;
	private int nPrunings;
	private List<Variable<T>> unasgnVars;
	private long runtime; 
	private bool trace; 
	
	public BT(CSP<T> problem)
	{
		csp = problem; 
		nDecisions = 0; 
		nPrunings = 0; 
		unasgnVars = new List<Variable<T>>();
		runtime = 0; 
		trace = true;
	}
	
	public void ClearStats()
	{
		nDecisions = 0; 
		nPrunings = 0; 
		runtime = 0; 
	}
	
	public void PrintStats()
	{
		Console.WriteLine(String.Format("Search made {0} variable assignments and pruned {1} variable values",
										nDecisions, 
										nPrunings));
	}
	
	public void RestoreValues(List<Tuple<Variable<T>, T>> prunings )
	{
		foreach(Tuple<Variable<T>, T> tup in prunings)
		{
			tup.Item1.UnpruneValue(tup.Item2);
		}
	}
	
	public void RestoreAllVariableDom()
	{
		foreach(Variable<T> var in csp.Vars)
		{
			if(var.IsAssigned())
			{
				var.Unassign();
			}
			var.RestoreCurdom();
		}
	}
	
	public void ToggleTrace(){
		trace = !trace; 
	}
	
	public void RestoreUnasignVar(Variable<T> var)
	{
		unasgnVars.Add(var);
	}
	
	public void BTSearch(Func<CSP<T>, Variable<T>, Tuple<bool, 
															List<
																Tuple<Variable<T>, 
																T>
																>	
															> 				
								>prop, 
						Func<CSP<T>, Variable<T>> varOrd = null, 
						Func<Variable<T>, T> valOrd = null)
	{
		ClearStats();
		var watch = System.Diagnostics.Stopwatch.StartNew();
		RestoreAllVariableDom();
		
		foreach(Variable<T> var in csp.Vars)
		{	
			if(!var.IsAssigned())
			{
				unasgnVars.Add(var);
			}
		}
		Tuple<bool, List<Tuple<Variable<T>, T>>> status = prop(csp, null);
		Console.WriteLine("Completed Prop");
		nPrunings = nPrunings + status.Item2.Count;
		
		if(trace)
		{
			Console.WriteLine(String.Format("{0} unassigend variables at the start of search", unasgnVars.Count));
			Console.WriteLine(String.Format("Root PRunings: {0}", status.Item2.Count));
		}
		var isFound = BTRecurse(prop, 1, varOrd, valOrd);
		RestoreValues(status.Item2);
		
		watch.Stop();
		runtime = watch.ElapsedMilliseconds;
		
		if(!isFound)
		{
			Console.WriteLine(String.Format("CSP {0} unsolved. Has no solutions.", 
							  csp.Name));
		}
		else
		{
			Console.WriteLine(String.Format("CSP {0} solved. CPU Time used = {1}.", 
							  csp.Name,
							  runtime));
		}
		Console.WriteLine("BT Search Finished");
		PrintStats();
		
	}
	
	private bool BTRecurse(Func<CSP<T>, Variable<T>, Tuple<bool, List<Tuple<Variable<T>, T>> > >prop, 
						int level,
						Func<CSP<T>, Variable<T>> varOrd = null, 
						Func<Variable<T>, T> valOrd = null 
						)
	{
		if(trace){
			Console.WriteLine(String.Format("{0}BTRecurse level {1}", new String(' ', level), level));
		}
		if(!unasgnVars.Any())
		{
			return true; 
		}
		else
		{
			///Figure out which variable to assign,
            ///Then remove it from the list of unassigned vars
			Variable<T> nextVar = unasgnVars[0]; 
			if(varOrd != null)
			{
			    nextVar = varOrd(csp);
			}
			unasgnVars.Remove(nextVar); 
			
			List<T> valueOrder = nextVar.Domain; 
			
			if(trace)
			{
				Console.WriteLine(String.Format("{0}BTRecurse var = {1}", new String(' ', level), nextVar.Name));
			}
			
			foreach(T item in valueOrder)
			{
				if(trace)
				{
					Console.WriteLine(String.Format("{0}BTRecurse trying {1} = {2}", new String(' ', level), nextVar.Name, item));
				}
				nextVar.Assign(item);
				nDecisions++; 
				
				Tuple<bool, List<Tuple<Variable<T>, T>>> status = prop(csp, nextVar);
				nPrunings = nPrunings + status.Item2.Count;
				
				if(trace)
				{
					Console.WriteLine(String.Format("{0}BTRecurse status = {1}", new String(' ', level), status.Item1));
					Console.WriteLine(String.Format("{0}BTRecurse prunings = {1}", new String(' ', level), status.Item2.Count));
				}
				
				if(status.Item1)
				{
					if(BTRecurse(prop,level+1, varOrd, valOrd))
					{
						return true;
					}
				}
				if(trace)
				{
					Console.WriteLine(String.Format("{0}BTRecurse restoring {1}", new String(' ', level), status.Item2.Count));
				}
				
				RestoreValues(status.Item2);
				nextVar.Unassign();
				
			}
			RestoreUnasignVar(nextVar);
			return false;
		}
		 
	}
}
