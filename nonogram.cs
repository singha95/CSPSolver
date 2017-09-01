using System;
using System.Linq;
using System.Linq.Expressions; 
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Nonogram
{
	public static Tuple<CSP<string>, Variable<string>[,]> CSPModel(List<List<List<int>>> nonogramList)
	{	
		CSP<string> finalCSP = new CSP<string>("Nonogram");
		int constraintNum = 1;
		List<string> domain = new List<string>{"1", "0"};

		int columnConNum = nonogramList[0].Count;
		int rowConNum = nonogramList[1].Count; 
		
		Variable<string>[,] variables = new Variable<string>[rowConNum, columnConNum]; 
		
		IEnumerable<IEnumerable<string>> rowStates = JoinRepeat(domain, columnConNum);
		IEnumerable<IEnumerable<string>> colStates = JoinRepeat(domain, rowConNum);
		
		
		///Intialize all variables and add them to the CSP and Variable array.
		for(int i =0;i < rowConNum; i++)
		{
			for(int x=0; x<columnConNum; x++)
			{
				Variable<string> newVar = new Variable<string>(String.Format("{0}|{1}", i, x), new List<string>(domain)); 
				finalCSP.AddVar(newVar); 
				variables[i,x] = newVar;			
			}
		}
		
		//Create the row and column constraints for the board. 
		for(int x = 0; x<rowConNum; x++)
		{
			
			string reg = "^0*";
			for(int z =0; z<nonogramList[1][x].Count;z++)
			{
				reg += "1{" + nonogramList[1][x][z] +"}";
				if(z < nonogramList[1][x].Count - 1)
				{
					reg += "0+"; 
				}
				else
				{
					reg += "0*$"; 
				}
			}
			
			List<Variable<string>> scope = GetRow(variables, x); 
			Func<List<string>, bool> isMet = y =>  Regex.IsMatch(string.Join( "", y.ToArray() ), reg);
		
			finalCSP.AddConstraint(new Constraint<string>(scope, constraintNum.ToString(), isMet));
			constraintNum++;
		}
		
		for(int x = 0; x<columnConNum; x++)
		{
			
			string reg = "^0*";
			for(int z =0; z<nonogramList[0][x].Count;z++)
			{
				reg += "1{" + nonogramList[0][x][z] +"}";
				if(z < nonogramList[0][x].Count - 1)
				{
					reg += "0+"; 
				}
				else
				{
					reg += "0*$"; 
				}
			}
			
			List<Variable<string>> scope = GetCol(variables, x);
			Func<List<string>, bool> isMet = y =>  Regex.IsMatch(string.Join( "", y.ToArray() ), reg);
			finalCSP.AddConstraint(new Constraint<string>(scope, constraintNum.ToString(), isMet));
			constraintNum++;
		}
		
		
		return new Tuple<CSP<string>, Variable<string>[,]>(finalCSP, variables); 
	}
	
	public static void PrintSoln(Variable<string>[,] vars){
		for(int i=0;i < vars.GetLength(0);i++){
			for(int j=0;j < vars.GetLength(1);j++){
                Console.Write(string.Format("{0} ", vars[i, j]));
            }
            Console.Write(Environment.NewLine + Environment.NewLine);
		}
    
	}
	
	private static List<Variable<string>> GetCol(Variable<string>[,] variables, int x)
	{
		List<Variable<string>> listOfVar = new List<Variable<string>>(); 
		
		for (int i = 0; i < variables.GetLength(0); i++)
		{
			listOfVar.Add(variables[i, x]);
		}
		return listOfVar;
	} 
	
	private static List<Variable<string>> GetRow(Variable<string>[,] variables, int x)
	{
		List<Variable<string>> listOfVar = new List<Variable<string>>(); 
		
		for (int i = 0; i < variables.GetLength(1); i++)
		{
			listOfVar.Add(variables[x, i]);
		}
		return listOfVar;
	}
	
	private static IEnumerable<IEnumerable<string>> JoinRepeat(List<String> list, int repeat)
	{
		List<List<string>> zeroOneRepeated = Enumerable.Range(0, repeat)
		.Select(i => list)
		.ToList();
		
		return CrossJoin(zeroOneRepeated);
	}
	
	private static IEnumerable<IEnumerable<string>> CrossJoin(IEnumerable<IEnumerable<string>> sequences)
	{
		IEnumerable<IEnumerable<string>> emptyProduct = new[] { Enumerable.Empty<string>() };
		IEnumerable<IEnumerable<string>> result = emptyProduct;
		foreach (IEnumerable<string> sequence in sequences)
		{
			result = from accseq in result from item in sequence select accseq.Concat(new[] {item});
		}
		return result;
	}
	
}